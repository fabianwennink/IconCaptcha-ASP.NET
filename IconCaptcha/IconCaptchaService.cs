/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace IconCaptcha
{
    public class IconCaptchaService
    {
        public const string SESSION_NAME = "iconcaptcha";
        public const string SESSION_SETTINGS = "settings";
        public const string SESSION_TOKEN = "csrf";
        public const string CAPTCHA_FIELD_SELECTION = "ic-hf-se";
        public const string CAPTCHA_FIELD_ID = "ic-hf-id";
        public const string CAPTCHA_FIELD_HONEYPOT = "ic-hf-hp";
        public const string CAPTCHA_FIELD_TOKEN = "_iconcaptcha-token";
        public const int CAPTCHA_TOKEN_LENGTH = 20;
        public const int CAPTCHA_IMAGE_SIZE = 320;

        public static readonly IDictionary<int, int> CAPTCHA_ICON_SIZES = new Dictionary<int, int>
        {
            [5] = 50,
            [6] = 40,
            [7] = 30,
            [8] = 20,
        };

        public static readonly IDictionary<int, int> CAPTCHA_MAX_LOWEST_ICON_COUNT = new Dictionary<int, int>
        {
            [5] = 2,
            [6] = 2,
            [7] = 3,
            [8] = 3,
        };

        public static readonly byte[] CAPTCHA_DEFAULT_BORDER_COLOR = { 240, 240, 240 };

        public static readonly IDictionary<string, Theme> CAPTCHA_DEFAULT_THEME_COLORS = new Dictionary<string, Theme>
        {
            ["light"] = new(Mode.light, CAPTCHA_DEFAULT_BORDER_COLOR),
            ["legacy-light"] = new(Mode.light, CAPTCHA_DEFAULT_BORDER_COLOR),
            ["dark"] = new(Mode.dark, new byte[] { 64, 64, 64 }),
            ["legacy-dark"] = new(Mode.dark, new byte[] { 64, 64, 64 }),
        };

        private CaptchaSession _session;

        public ISessionProvider SessionProvider { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public IOptions<IconCaptchaOptions> Options { get; }

        public IconCaptchaService(ISessionProvider sessionProvider, IHttpContextAccessor httpContextAccessor,
            IOptions<IconCaptchaOptions> options)
        {
            SessionProvider = sessionProvider;
            HttpContextAccessor = httpContextAccessor;
            Options = options;
            Rand = new Random();
        }

        public Random Rand { get; }

        public CaptchaSession Session
        {
            get
            {
                if (_session != null)
                {
                    return _session;
                }

                if (!SessionProvider.TryGetSession(SESSION_NAME, out var session) || session == null)
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
            if (Session.Token == null)
            {
                // Create a secure captcha session token.
                var bytes = RandomNumberGenerator.GetBytes(CAPTCHA_TOKEN_LENGTH);
                var token = Convert.ToHexString(bytes);

                Session.Token = token;

                SaveSession();
            }

            return Session.Token;
        }

        /// <summary>
        /// Tries to load/initialize a {@see CaptchaSession} with the given captcha identifier.
        /// When an existing session is found, it's data will be loaded, else a new session will be created.
        /// </summary>
        /// <param name="identifier">The identifier of the captcha.</param>
        private CaptchaSessionData CreateSession(int identifier = 0)
        {
            // Load the captcha session for the current identifier.
            if (!Session.TryGetValue(identifier, out var sessionData))
            {
                sessionData = new CaptchaSessionData();

                Session.Add(identifier, sessionData);
            }

            return sessionData;
        }

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
            var correctIconAmount = Rand.Next(1, CAPTCHA_MAX_LOWEST_ICON_COUNT[iconAmount]);
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


        /**
     * Validates the user form submission. If the captcha is incorrect, it
     * will set the global error variable and return FALSE, else TRUE.
     *
     * @param array $post The HTTP POST request variable ($_POST).
     *
     * @return boolean TRUE if the captcha was correct, FALSE if not.
     */
        public void ValidateSubmission()
        {
            var post = HttpContextAccessor.HttpContext.Request.Form;

            // Make sure the form data is set.
            if (!post.Any())
            {
                throw new SubmissionException(3, Options.Value.Messages.EmptyForm);
            }

            // Check if the captcha ID is set.
            if (!post.TryGetValue(CAPTCHA_FIELD_ID, out var captchaIdString)
                || !int.TryParse(captchaIdString.First(), out var captchaId)
                || !Session.ContainsKey(captchaId))
            {
                throw new SubmissionException(4, Options.Value.Messages.InvalidId);
            }

            // Check if the honeypot value is set.
            if (!post.TryGetValue(CAPTCHA_FIELD_HONEYPOT, out var honeyPot) || !string.IsNullOrEmpty(honeyPot))
            {
                throw new SubmissionException(5, Options.Value.Messages.InvalidId);
            }

            // Verify if the captcha token is correct.
            if (!post.TryGetValue(CAPTCHA_FIELD_TOKEN, out var token) || !ValidateToken(token))
            {
                throw new SubmissionException(6, Options.Value.Messages.FormToken);
            }

            // Get the captcha identifier.
            var identifier = captchaId;

            // Initialize the session.
            var sessionData = CreateSession(identifier);

            // Check if the selection field is set.
            if (post.TryGetValue(CAPTCHA_FIELD_SELECTION, out var selectionString))
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
                    InvalidateSession(identifier);
                    
                    return;
                }

                throw new SubmissionException(1, Options.Value.Messages.WrongIcon);
            }

            throw new SubmissionException(2, Options.Value.Messages.NoSelection);
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
            if (clickedXPos < 0 || clickedXPos > captchaWidth || clickedYPos < 0 || clickedYPos > 50)
            {
                return -1;
            }

            return (int)Math.Floor(clickedXPos / ((decimal)captchaWidth / iconAmount));
        }


        public bool ValidateToken(string payloadToken, string headerToken = null)
        {
            return true;
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
            if (payload != null)
            {
                // Check if the captcha ID and required other payload data is set.
                if (payload.CaptchaId == null || payload.XPos == null || payload.YPos == null || payload.Width == null)
                {
                    return false;
                }

                // Initialize the session.
                var sessionData = CreateSession(payload.CaptchaId);

                // Get the clicked position.
                var clickedPosition = DetermineClickedIcon(payload.XPos.Value, payload.YPos.Value, payload.Width.Value,
                    sessionData.Icons.Count);

                // Check if the selection is set and matches the position from the session.
                if (sessionData.Icons[clickedPosition] == sessionData.CorrectId)
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
            }

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
        public async Task GetImage(int identifier)
        {
            HttpContext httpContext = HttpContextAccessor.HttpContext;
            
            // Check if the captcha id is set
            if (identifier > -1)
            {
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

                var iconsDirectoryPath = Options.Value.IconPath;
                var placeholder = Path.Combine(iconsDirectoryPath, "..", "placeholder.png");

                // Check if the placeholder icon exists.
                if (File.Exists(placeholder))
                {
                    // Format the path to the icon directory.
                    var themeIconColor = Options.Value.Themes[sessionData.Mode].Icons;
                    var iconPath = Path.Combine(iconsDirectoryPath, themeIconColor.ToString());

                    // Generate the captcha image.
                    var generatedImage = GenerateImage(sessionData, iconPath, placeholder);

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
            }
        }

        /// <summary>
        /// Returns a generated image containing the icons for the current captcha instance. The icons will be copied
        /// onto a placeholder image, located at the $placeholderPath. The icons will be randomly rotated and flipped
        /// based on the captcha options.
        /// </summary>
        /// <param name="sessionData">The current session.</param>
        /// <param name="iconPath">The path to the folder holding the icons.</param>
        /// <param name="placeholderPath">The path to the placeholder image, with the name of the file included.</param>
        /// <returns>The generated image.</returns>
        private Stream GenerateImage(CaptchaSessionData sessionData, string iconPath,
            string placeholderPath)
        {
            // Prepare the placeholder image.
            SKBitmap placeholder = CreateImage(placeholderPath);
            var canvas = new SKCanvas(placeholder);

            // Prepare the icon images.
            var iconImages = sessionData
                .IconIds
                .ToDictionary(
                    id => id, 
                    id => CreateImage(Path.Combine(iconPath, $"icon-{id}.png"))
                );

            // Image pixel information.
            var iconCount = sessionData.Icons.Count;
            var iconSize = CAPTCHA_ICON_SIZES[iconCount];
            var iconOffset = (CAPTCHA_IMAGE_SIZE / iconCount - 30) / 2;
            var iconOffsetAdd = CAPTCHA_IMAGE_SIZE / iconCount - iconSize;
            var iconLineSize = CAPTCHA_IMAGE_SIZE / iconCount;

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
                if (Options.Value.Themes.TryGetValue(sessionData.Mode, out var mode) && mode.Color.Length == 3)
                {
                    color = mode.Color;
                }
                else
                {
                    color = CAPTCHA_DEFAULT_BORDER_COLOR;
                }

                borderColor = new SKColor(color[0], color[1], color[2]);
            }

            // Copy the icons onto the placeholder.
            var xOffset = iconOffset;
            for (var i = 0; i < iconCount; i++)
            {
                // Get the icon image from the array. Use position to get the icon ID.
                var icon = iconImages[sessionData.Icons[i]];

                // Rotate icon, if enabled.
                if (rotateEnabled)
                {
                    var degree = Rand.Next(1, 4);
                    if (degree != 4)
                    {
                        // Only if the 'degree' is not the same as what it would already be at.
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

            SKData encoded = placeholder.Encode(SKEncodedImageFormat.Png, 100);

            // get a stream over the encoded data
            return encoded.AsStream();
        }

        private static SKBitmap CreateImage(string filePath)
        {
            var image = SKImage.FromEncodedData(filePath);
            
            return SKBitmap.FromImage(image);
        }

        private void SaveSession()
        {
            SessionProvider.SetSession(SESSION_NAME, _session);
        }

        /// <summary>
        /// Invalidates the {@see CaptchaSession} linked to the given captcha identifier.
        /// The data stored inside the session will be destroyed, as the session will be unset.
        /// </summary>
        /// <param name="identifier">The identifier of the captcha.</param>
        public void InvalidateSession(int identifier)
        {
            // Unset the previous session data
            Session?.Remove(identifier);

            SaveSession();
        }
    }
}
