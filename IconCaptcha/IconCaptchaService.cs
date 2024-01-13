/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license

    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IconCaptcha.Dto;
using IconCaptcha.Enums;
using IconCaptcha.Exceptions;
using IconCaptcha.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace IconCaptcha
{
    public class IconCaptchaService
    {
        private const string SessionName = "iconcaptcha";
        private const string CaptchaFieldSelection = "ic-hf-se";
        private const string CaptchaFieldId = "ic-hf-id";
        private const string CaptchaFieldHoneypot = "ic-hf-hp";
        private const string CaptchaFieldToken = "_iconcaptcha-token";
        
        /// <summary>
        /// The default length of a captcha token.
        /// </summary>
        private const int CaptchaTokenLength = 20;
        
        /// <summary>
        /// The default image width of a captcha challenge, in pixels.
        /// </summary>
        private const int CaptchaImageWidth = 320;

        /// <summary>
        /// The default image height of a captcha challenge, in pixels.
        /// </summary>
        private const int CaptchaImageHeight = 50;

        /// <summary>
        /// The size in pixels of each icon on the challenge image based on the amount of icons per challenge.
        /// For each entry, the key indicates the icons per challenge, and the value the width of each icon.
        /// </summary>
        private static readonly IDictionary<int, int> CaptchaIconSizes = new Dictionary<int, int>
        {
            [5] = 50,
            [6] = 40,
            [7] = 30,
            [8] = 20,
        };

        /// <summary>
        /// The lowest amount of different icon possibilities per amount of icons per challenge.
        /// For each entry, the key indicates of icons per challenge, and the value the
        /// amount of different possible icons can be used while generating a challenge.
        /// </summary>
        private static readonly IDictionary<int, int> CaptchaMaxLowestIconCount = new Dictionary<int, int>
        {
            [5] = 2,
            [6] = 2,
            [7] = 3,
            [8] = 3,
        };

        /// <summary>
        /// The default challenge border color as RGB.
        /// </summary>
        private static readonly byte[] CaptchaDefaultBorderColor = { 240, 240, 240 };

        /// <summary>
        /// The list of default themes. For each entry, the key indicates the name of the theme, and the value
        /// contains a new <see cref="Theme"/> containing the icon and icon separator colors.
        /// </summary>
        public static readonly IDictionary<string, Theme> CaptchaDefaultThemeColors = new Dictionary<string, Theme>
        {
            ["light"] = new(Mode.light, CaptchaDefaultBorderColor),
            ["legacy-light"] = new(Mode.light, CaptchaDefaultBorderColor),
            ["dark"] = new(Mode.dark, new byte[] { 64, 64, 64 }),
            ["legacy-dark"] = new(Mode.dark, new byte[] { 64, 64, 64 }),
        };
        
        private Random Rand { get; }

        private ISessionProvider SessionProvider { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private IOptions<IconCaptchaOptions> Options { get; }

        public IconCaptchaService(ISessionProvider sessionProvider, IHttpContextAccessor httpContextAccessor,
            IOptions<IconCaptchaOptions> options)
        {
            SessionProvider = sessionProvider;
            HttpContextAccessor = httpContextAccessor;
            Options = options;
            Rand = new Random();
        }
        
        private CaptchaSession _session;
        private CaptchaSession Session
        {
            get
            {
                if (_session != null)
                {
                    return _session;
                }

                if (!SessionProvider.TryGetSession(SessionName, out var session) || session == null)
                {
                    session = new CaptchaSession();
                }

                _session = session;
                SaveSession();

                return _session;
            }
        }
        
        /// <summary>
        /// Generates and returns a secure random string which will serve as a CSRF token for the current session. After
        /// generating the token, it will be saved in the global session variable. The length of the token will be
        /// determined by the value of the global constant {@see CAPTCHA_TOKEN_LENGTH}. A token will only be generated
        /// when no token has been generated before in the current session. If a token already exists, this token will
        /// be returned instead.
        /// </summary>
        /// <returns>The captcha token.</returns>
        public string Token()
        {
            // Make sure to only generate a token if none exists.
            if (Session.Token != null) 
                return Session.Token;
            
            // Create a secure captcha session token.
            var bytes = RandomNumberGenerator.GetBytes(CaptchaTokenLength);
            var token = Convert.ToHexString(bytes);

            Session.Token = token;

            SaveSession();

            return Session.Token;
        }

        /// <summary>
        /// Initializes the state of a captcha. The amount of icons shown in the captcha image, their positions,
        /// which icon is correct and which icon identifiers should be used will all be determined in this function.
        /// This information will be stored in the {@see CaptchaSession}. The details required to initialize the client
        /// will be returned as a base64 encoded JSON string.
        ///
        /// In case a timeout is detected, no state will be initialized and an error message
        /// will be returned, also as a base64 encoded JSON string.
        /// </summary>
        /// <param name="payload">The payload containing the theme and the identifier of the captcha.</param>
        /// <returns>Captcha details required to initialize the client.</returns>
        public CaptchaResult GetCaptchaData(Payload payload)
        {
            // Set the captcha id property
            var sessionData = CreateSession(payload.CaptchaId);

            // Check if the max attempts limit has been reached and a timeout is active.
            // If reached, return an error and the remaining time.
            if (sessionData.AttemptsTimeout != null)
            {
                if (DateTime.Now <= sessionData.AttemptsTimeout)
                {
                    return new CaptchaResult
                    {
                        Error = 1,
                        Data = (sessionData.AttemptsTimeout.Value - DateTime.Now).TotalSeconds // remaining time.
                    };
                }

                sessionData.AttemptsTimeout = null;
                sessionData.Attempts = 0;
            }

            var iconAmountOptions = Options.Value.Image.Amount;

            // Determine the number of icons to add to the image.
            var iconAmount = iconAmountOptions.Min;
            if (iconAmountOptions.Min != iconAmountOptions.Max)
            {
                iconAmount = Rand.Next(iconAmountOptions.Min, iconAmountOptions.Max);
            }

            // Number of times the correct image will be placed onto the placeholder.
            var correctIconAmount = Rand.Next(1, CaptchaMaxLowestIconCount[iconAmount]);
            var totalIconAmount = CalculateIconAmounts(iconAmount, correctIconAmount);
            totalIconAmount.Add(correctIconAmount);

            // Icon position and ID information.
            var iconPositions = new int[iconAmount];
            var iconIds = new List<int>();
            var correctIconId = -1;
            
            // Create a random 'icon position' order.
            var tempPositions = Enumerable.Range(0, iconAmount)
                .OrderBy(c => Rand.Next())
                .ToList();

            // Generate the icon positions/IDs array.
            var i = 0;
            while (iconIds.Count < totalIconAmount.Count)
            {
                // Generate a random icon ID. If it is not in use yet, process it.
                var tempIconId = Rand.Next(1, Options.Value.Image.AvailableIcons);

                if (!iconIds.Contains(tempIconId))
                {
                    iconIds.Add(tempIconId);

                    // Assign the current icon ID to one or more positions.
                    for (var j = 0; j < totalIconAmount[i]; j++)
                    {
                        // Pop element
                        if (tempPositions.Any()) // FIXME
                        {
                            var tempKey = tempPositions.Last();
                            tempPositions.RemoveAt(tempPositions.Count - 1);

                            iconPositions[tempKey] = tempIconId;
                        }
                    }

                    // Set the least appearing icon ID as the correct icon ID.
                    if (correctIconId == -1 && totalIconAmount.Min() == totalIconAmount[i])
                    {
                        correctIconId = tempIconId;
                    }

                    i++;
                }
            }

            // Get the last attempts count to restore, after clearing the session.
            var attemptsCount = sessionData.Attempts;

            // Unset the previous session data.
            sessionData.Clear();

            // Set the chosen icons and position and reset the requested status.
            sessionData.Mode = payload.Mode;
            sessionData.Icons = iconPositions.ToList();
            sessionData.IconIds = iconIds;
            sessionData.CorrectId = correctIconId;
            sessionData.Requested = false;
            sessionData.Attempts = attemptsCount;

            SaveSession();

            // Return the captcha details.
            return new CaptchaResult
            {
                Id = payload.CaptchaId
            };
        }
        
        /// <summary>
        /// Validates the user form submission. If the captcha is incorrect, it
        /// will set the global error variable and return FALSE, else TRUE.
        /// </summary>
        /// <exception cref="IconCaptchaSubmissionException">Throws when the validation fails.</exception>
        public void ValidateSubmission()
        {
            var post = GetHttpContext().Request.Form;

            // Make sure the form data is set.
            if (!post.Any())
            {
                throw new IconCaptchaSubmissionException(3, Options.Value.Messages.EmptyForm);
            }

            // Check if the captcha ID is set.
            if (!post.TryGetValue(CaptchaFieldId, out var captchaIdString)
                || !long.TryParse(captchaIdString.First(), out var captchaId)
                || !Session.ContainsKey(captchaId))
            {
                throw new IconCaptchaSubmissionException(4, Options.Value.Messages.InvalidId);
            }

            // Check if the honeypot value is set.
            if (!post.TryGetValue(CaptchaFieldHoneypot, out var honeyPot) || !string.IsNullOrEmpty(honeyPot))
            {
                throw new IconCaptchaSubmissionException(5, Options.Value.Messages.InvalidId);
            }

            // Verify if the captcha token is correct.
            if (!post.TryGetValue(CaptchaFieldToken, out var token) || !ValidateToken(token))
            {
                throw new IconCaptchaSubmissionException(6, Options.Value.Messages.FormToken);
            }
            
            // Initialize the session.
            var sessionData = CreateSession(captchaId);

            // Check if the selection field is set.
            if (post.TryGetValue(CaptchaFieldSelection, out var selectionString))
            {
                // Parse the selection.
                var selection = selectionString.First().Split(',');

                int? clickedPosition = null;
                if (selection.Length == 3
                    && int.TryParse(selection[0], out var clickedXPos)
                    && int.TryParse(selection[1], out var clickedYPos)
                    && int.TryParse(selection[2], out var captchaWidth))
                {
                    clickedPosition =
                        DetermineClickedIcon(clickedXPos, clickedYPos, captchaWidth, sessionData.Icons.Count);
                }

                // If the clicked position matches the stored position, the form can be submitted.
                if (sessionData.Completed &&
                    (clickedPosition != null && sessionData.Icons[clickedPosition.Value] == sessionData.CorrectId))
                {
                    // Invalidate the captcha to prevent resubmission of a form on the same captcha.
                    InvalidateSession(captchaId);

                    return;
                }

                throw new IconCaptchaSubmissionException(1, Options.Value.Messages.WrongIcon);
            }

            throw new IconCaptchaSubmissionException(2, Options.Value.Messages.NoSelection);
        }

        /// <summary>
        /// Validates the global captcha session token against the given payload token and sometimes against a header token
        /// as well. All the given tokens must match the global captcha session token to pass the check. This function
        /// will only validate the given tokens if the 'token' option is set to TRUE. If the 'token' option is set to anything
        /// else other than TRUE, the check will be skipped.
        /// </summary>
        /// <param name="payloadToken">The token string received via the HTTP request body.</param>
        /// <param name="headerToken">The token string received via the HTTP request headers. This value is optional,
        /// as not every request will contain custom HTTP headers and thus this token should be able to be skipped. Default
        /// value is NULL. When the value is set to anything else other than NULL, the given value will be checked against
        /// the captcha session token.</param>
        /// <returns>TRUE if the captcha session token matches the given tokens or if the token option is disabled,
        /// FALSE if the captcha session token does not match the given tokens.</returns>
        public bool ValidateToken(string payloadToken, string headerToken = null)
        {
            var options = Options.Value;

            // Only validate if the token option is enabled.
            if (!options.Token) 
                return true;
            
            var sessionToken = GetToken();

            // If the token is empty but the option is enabled, the token was never requested.
            if (string.IsNullOrEmpty(sessionToken))
            {
                return false;
            }

            // Validate the payload and header token (if set) against the session token.
            if (headerToken != null)
            {
                return sessionToken == payloadToken && sessionToken == headerToken;
            }

            return sessionToken == payloadToken;
        }

        /// <summary>
        /// Checks if the by the user selected icon is the correct icon. Whether the clicked icon is correct or not
        /// will be determined based on the clicked X and Y coordinates and the width of the IconCaptcha DOM element.
        ///
        /// If the selected icon is indeed the correct icon, the {@see CaptchaSession} linked to the captcha identifier
        /// will be marked as completed and TRUE will be returned. If an incorrect icon was selected, the session will
        /// be marked as incomplete, the 'attempts' counter will be incremented by 1 and FALSE will be returned.
        /// A check will also take place to see if a timeout should set for the user, based on the options and attempts counter.
        /// </summary>
        /// <param name="payload">The payload of the HTTP Post request, containing the captcha identifier, clicked X
        /// and Y coordinates and the width of the captcha element.</param>
        /// <returns>TRUE if the correct icon was selected, FALSE if not.</returns>
        public bool SetSelectedAnswer(Payload payload)
        {
            if (payload == null) 
                return false;
            
            // Check if the captcha ID and required other payload data is set.
            if (payload.CaptchaId == default || payload.XPos == null || payload.YPos == null || payload.Width == null)
            {
                return false;
            }

            // Initialize the session.
            var sessionData = CreateSession(payload.CaptchaId);

            // Get the clicked position.
            var clickedPosition = DetermineClickedIcon(payload.XPos.Value, payload.YPos.Value, payload.Width.Value,
                sessionData.Icons.Count);

            // Check if the selection is set and matches the position from the session.
            if (clickedPosition > -1 && sessionData.Icons[clickedPosition] == sessionData.CorrectId)
            {
                sessionData.Attempts = 0;
                sessionData.AttemptsTimeout = null;
                sessionData.Completed = true;

                SaveSession();

                return true;
            }

            sessionData.Completed = false;

            // Increase the attempts counter.
            sessionData.Attempts += 1;

            // If the max amount has been reached, set a timeout (if set).
            if (sessionData.Attempts == Options.Value.Attempts.Amount
                && Options.Value.Attempts.Timeout != null)
            {
                sessionData.AttemptsTimeout = DateTime.Now + Options.Value.Attempts.Timeout;
            }

            SaveSession();

            return false;
        }

        /// <summary>
        /// Displays an image containing multiple icons in a random order for the current captcha instance, linked
        /// to the given captcha identifier. Headers will be set to prevent caching of the image. In case the captcha
        /// image was already requested once, a HTTP status '403 Forbidden' will be set and no image will be returned.
        ///
        /// The image will only be rendered once as a PNG, and be destroyed right after rendering.
        /// </summary>
        /// <param name="identifier">The identifier of the captcha.</param>
        public async Task GetImage(long identifier)
        {
            // Check if the captcha id is set
            if (identifier <= -1)
            {
                return;
            }

            var httpContext = GetHttpContext();

            // Initialize the session.
            var sessionData = CreateSession(identifier);

            // Check the amount of times an icon has been requested
            if (sessionData.Requested)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                return;
            }

            sessionData.Requested = true;
            SaveSession();

            var isEmbedded = Options.Value.IconPath == null;

            string iconsDirectoryPath;
            string placeholder;

            if (isEmbedded)
            {
                iconsDirectoryPath = "icons";
                placeholder = "placeholder.png";
            }
            else
            {
                iconsDirectoryPath = Options.Value.IconPath;
                placeholder = Path.Combine(iconsDirectoryPath, "..", "placeholder.png");
            }

            // Check if the placeholder icon exists.
            if (!isEmbedded && !File.Exists(placeholder))
            {
                throw new IconCaptchaException("Placeholder file could not be loaded.");
            }

            // Format the path to the icon directory.
            var themeIconColor = Options.Value.Themes[sessionData.Mode].Icons;
            var iconPath = Path.Combine(iconsDirectoryPath, themeIconColor.ToString());

            // Generate the captcha image.
            await using var placeholderStream = isEmbedded
                ? GetType().Assembly.GetManifestResourceStream(placeholder)
                : File.OpenRead(placeholder);

            var generatedImage = GenerateImage(sessionData, iconPath, placeholderStream, isEmbedded);

            // Set the content type header to the PNG MIME-type.
            httpContext.Response.ContentType = "image/png";

            // Disable caching of the image.
            httpContext.Response.Headers.Expires = "0";
            httpContext.Response.Headers.CacheControl = new[]
            {
                "no-cache, no-store, must-revalidate",
                "post-check=0, pre-check=0",
            };
            httpContext.Response.Headers.Pragma = "no-cache";

            // Show the image and exit the code
            await generatedImage.CopyToAsync(httpContext.Response.Body);
        }

        /// <summary>
        /// Returns a generated image containing the icons for the current captcha instance. The icons will be copied
        /// onto a placeholder image, located at the $placeholderPath. The icons will be randomly rotated and flipped
        /// based on the captcha options.
        /// </summary>
        /// <param name="challenge">The current session.</param>
        /// <param name="iconPath">The path to the folder holding the icons.</param>
        /// <param name="placeholderStream">The stream to the placeholder image, with the name of the file included.</param>
        /// <param name="embeddedFiles">TRUE when reading files from assembly.</param>
        /// <returns>The generated image.</returns>
        private Stream GenerateImage(CaptchaChallenge challenge,
            string iconPath,
            Stream placeholderStream,
            bool embeddedFiles = false
        )
        {
            // Prepare the placeholder image.
            SKBitmap placeholder = CreateImage(placeholderStream);
            var canvas = new SKCanvas(placeholder);

            // Prepare the icon images.
            var iconImages = challenge
                .IconIds
                .ToDictionary(
                    id => id,
                    id =>
                    {
                        var icon = Path.Combine(iconPath, $"icon-{id}.png");

                        using var stream = embeddedFiles
                            ? GetType().Assembly.GetManifestResourceStream(icon)
                            : File.OpenRead(icon);

                        return CreateImage(stream);
                    });

            // Image pixel information.
            var iconCount = challenge.Icons.Count;
            var iconSize = CaptchaIconSizes[iconCount];
            var iconOffset = (CaptchaImageWidth / iconCount - 30) / 2;
            var iconOffsetAdd = CaptchaImageWidth / iconCount - iconSize;
            var iconLineSize = CaptchaImageWidth / iconCount;

            // Options.
            var rotateEnabled = Options.Value.Image.Rotate;
            var flipHorizontally = Options.Value.Image.Flip.Horizontally;
            var flipVertically = Options.Value.Image.Flip.Vertically;
            var borderEnabled = Options.Value.Image.Border;

            // Create the border color, if enabled.
            SKColor? borderColor = null;

            if (borderEnabled)
            {
                // Determine border color.
                byte[] color;
                if (Options.Value.Themes.TryGetValue(challenge.Mode, out var mode) && mode.Color.Length == 3)
                {
                    color = mode.Color;
                }
                else
                {
                    color = CaptchaDefaultBorderColor;
                }

                borderColor = new SKColor(color[0], color[1], color[2]);
            }

            // Copy the icons onto the placeholder.
            var xOffset = iconOffset;
            for (var i = 0; i < iconCount; i++)
            {
                // Get the icon image from the array. Use position to get the icon ID.
                var icon = iconImages[challenge.Icons[i]];

                // Rotate icon, if enabled.
                if (rotateEnabled)
                {
                    var degree = Rand.Next(1, 4);
                    
                    // Only if the 'degree' is not the same as what it would already be at.
                    if (degree != 4)
                    {
                        var rotated = new SKBitmap(
                            degree % 2 == 0 ? icon.Width : icon.Height,
                            degree % 2 == 0 ? icon.Height : icon.Width
                        );
                        
                        var surface = new SKCanvas(rotated);
                        surface.Translate(rotated.Width / 2, rotated.Height / 2);
                        surface.RotateDegrees(degree * 90);
                        surface.Translate(-rotated.Width / 2, -rotated.Height / 2);
                        surface.DrawBitmap(icon, 0, 0);

                        icon = rotated;
                    }
                }

                // Flip icon horizontally, if enabled.
                if (flipHorizontally && Rand.Next(1, 2) == 1)
                {
                    var flipped = new SKBitmap(icon.Width, icon.Height);
                    var surface = new SKCanvas(flipped);
                    surface.Scale(-1, 1, icon.Width / 2.0f, 0);
                    surface.DrawBitmap(icon, 0, 0);

                    icon = flipped;
                }

                // Flip icon vertically, if enabled.
                if (flipVertically && Rand.Next(1, 2) == 1)
                {
                    var flipped = new SKBitmap(icon.Width, icon.Height);
                    var surface = new SKCanvas(flipped);
                    surface.Scale(1, -1, 0, icon.Height / 2.0f);
                    surface.DrawBitmap(icon, 0, 0);

                    icon = flipped;
                }

                // Copy the icon onto the placeholder.
                canvas.DrawBitmap(icon,
                    new SKRect((iconSize * i) + xOffset, 10, (iconSize * i) + xOffset + 30, 10 + 30));

                xOffset += iconOffsetAdd;

                // Add the vertical separator lines to the placeholder, if enabled.
                if (borderEnabled && i > 0)
                {
                    var paint = new SKPaint
                    {
                        Color = borderColor.Value
                    };

                    canvas.DrawLine(iconLineSize * i, 0, iconLineSize * i, 50, paint);
                }
            }

            var encoded = placeholder.Encode(SKEncodedImageFormat.Png, 100);

            // get a stream over the encoded data
            return encoded.AsStream();
        }

        private static SKBitmap CreateImage(Stream file)
        {
            var image = SKImage.FromEncodedData(file);

            return SKBitmap.FromImage(image);
        }
        
        /// <summary>
        /// Returns the clicked icon position based on the X and Y position and the captcha width.
        /// </summary>
        /// <param name="clickedXPos">The X position of the click.</param>
        /// <param name="clickedYPos">The Y position of the click.</param>
        /// <param name="captchaWidth">The width of the captcha.</param>
        /// <param name="iconAmount"></param>
        private int DetermineClickedIcon(int clickedXPos, int clickedYPos, int captchaWidth, int iconAmount)
        {
            // Check if the clicked position is valid.
            if (clickedXPos < 0 || clickedXPos > captchaWidth || clickedYPos < 0 || clickedYPos > CaptchaImageHeight)
            {
                return -1;
            }

            return (int)Math.Floor(clickedXPos / ((decimal)captchaWidth / iconAmount));
        }
        
        /// <summary>
        /// Calculates the amount of times 1 or more other icons can be present in the captcha image besides the correct icon.
        /// Each other icons should be at least present 1 time more than the correct icon. When calculating the icon
        /// amount(s), the remainder of the calculation ($iconCount - $smallestIconCount) will be used.
        /// Example 1: When $smallestIconCount is 1, and the $iconCount is 8, the return value can be [3, 4].
        /// Example 2: When $smallestIconCount is 2, and the $iconCount is 6, the return value can be [4]. This is because
        /// dividing the remainder (4 / 2 = 2) is equal to the $smallestIconCount, which is not possible.
        /// Example 3: When the $smallestIconCount is 2, and the $iconCount is 8, the return value will be [3, 3].
        /// </summary>
        /// <param name="iconCount">The total amount of icons which will be present in the generated image.</param>
        /// <param name="smallestIconCount">The amount of times the correct icon will be present in the generated image.</param>
        /// <returns>The number of times an icon should be rendered onto the captcha image. Each value in the returned
        /// array represents a new unique icon.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private IList<int> CalculateIconAmounts(int iconCount, int smallestIconCount = 1)
        {
            var remainder = iconCount - smallestIconCount;
            var remainderDivided = (decimal)remainder / 2;
            var pickDivided = Rand.Next(1, 2) == 1; // 50/50 chance.

            // If division leads to decimal.
            if (remainderDivided % 1 != 0 && pickDivided)
            {
                var left = (int)Math.Floor(remainderDivided);
                var right = (int)Math.Ceiling(remainderDivided);

                // Only return the divided numbers if both are larger than the smallest number.
                if (left > smallestIconCount && right > smallestIconCount)
                {
                    return new List<int> { left, right };
                }
            }
            else if (pickDivided && remainderDivided > smallestIconCount)
            {
                // If no decimals: only return the divided numbers if it is larger than the smallest number.
                return new List<int> { (int)remainderDivided, (int)remainderDivided };
            }

            // Return the whole remainder.
            return new List<int> { remainder };
        }

        /// <summary>
        /// Invalidates the {@see CaptchaSession} linked to the given captcha identifier.
        /// The data stored inside the session will be destroyed, as the session will be unset.
        /// </summary>
        /// <param name="identifier">The identifier of the captcha.</param>
        public void InvalidateSession(long identifier)
        {
            // Unset the previous session data
            Session?.Remove(identifier);

            SaveSession();
        }

        /// <summary>
        /// Tries to load/initialize a {@see CaptchaSession} with the given captcha identifier.
        /// When an existing session is found, it's data will be loaded, else a new session will be created.
        /// </summary>
        /// <param name="identifier">The identifier of the captcha.</param>
        private CaptchaChallenge CreateSession(long identifier = 0)
        {
            // Load the captcha session for the current identifier.
            if (!Session.TryGetValue(identifier, out var sessionData))
            {
                sessionData = new CaptchaChallenge();

                Session.Add(identifier, sessionData);
            }
            
            return sessionData;
        }

        private void SaveSession()
        {
            SessionProvider.SetSession(SessionName, _session);
        }

        /// <summary>
        /// Returns the captcha session/CSRF token.
        /// </summary>
        /// <returns>A token as a string, or NULL if no token exists.</returns>
        private string GetToken()
        {
            return Session.Token;
        }

        private HttpContext GetHttpContext()
        {
            return HttpContextAccessor.HttpContext ??
                   throw new IconCaptchaException("No HTTP context could be accessed.");
        }
    }
}