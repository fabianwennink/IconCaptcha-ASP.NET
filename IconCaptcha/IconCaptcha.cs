/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;

namespace IconCaptcha_ASP
{
    public class IconCaptcha
    {
        /// <summary>
        /// Dependency required to store session state.
        /// </summary>
        private readonly ISessionProvider _sessionProvider;

        /// <summary>
        /// The session containing captcha information.
        /// </summary>
        private IconCaptchaSession _sessionData;

        /// <summary>
        /// Object that will have a reference for the session object.
        /// </summary>
        private IconCaptchaSession SessionHolder
        {
            get { return _sessionProvider.GetSession("icon_captcha_asp"); }
            set { _sessionProvider.SetSession("icon_captcha_asp", value); }
        }

        /// <summary>
        /// The (possible) custom error messages.
        /// </summary>
        private List<string> errorMessages;

        /// <summary>
        /// The path to the folder where the icons are located.
        /// It should not contain the name of the theme yet, since this will be added dynamically.
        /// </summary>
        private readonly string _iconPath;

        /// <summary>
        /// IconCaptcha constructor.
        /// </summary>
        /// <param name="sessionProvider">The session provider used to store the session data.</param>
        /// <param name="path">The path to the folder where the icons are stored.</param>
        /// <param name="messages">The custom error messages (optional).</param>
        public IconCaptcha(ISessionProvider sessionProvider, string path, string[] messages = null)
        {
            // set the session provider
            _sessionProvider = sessionProvider;

            // Set the icon path
            _iconPath = path;

            SetErrorMessages(messages);

            Debug.WriteLine("This is a demo version of IconCaptcha for ASP.NET websites.");
        }

        /// <summary>
        /// Sets the custom error messages array. When set, these messages will 
        /// be returned by getErrorMessage() instead of the default messages.
        /// 
        /// Message 1 = You've selected the wrong image.
        /// Message 2 = No image has been selected.
        /// Message 3 = You've not submitted any form.
        /// Message 4 = The captcha ID was invalid.
        /// 
        /// Array format: string["", "", "", ""]
        /// 
        /// Note: All 4 messages need to be present for the custom messages to work.
        /// </summary>
        /// <param name="messages">The array containing the custom error messages.</param>
        public void SetErrorMessages(string[] messages)
        {
            if (messages != null && messages.Length == 4)
            {
                errorMessages = messages.ToList();
            }
            else
            {
                errorMessages = new string[4].ToList();
            }
        }

        /// <summary>
        /// Return a correct icon hash + multiple incorrect hashes.
        /// </summary>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <param name="theme">The theme of the captcha.</param>
        /// <returns>List containing the icon hashes.</returns>
        public string[] GetCaptchaData(int captchaId, string theme)
        {
            int a = Utils.Random.Next(1, 91); // Get a random number (correct image)
            int b = 0; // Get another random number (incorrect image)

            // Load the session data, if there is any present.
            // Default data will be used in case no data exists.
            _sessionData = SessionHolder;

            // If the session data doesn't exist, create a new instance.
            if(_sessionData == null)
            {
                theme = (String.IsNullOrEmpty(theme)) ? Utils.SanitizeString(theme) : "light";
                _sessionData = new IconCaptchaSession(theme);
            }

            // Pick a random number for the incorrect icon.
            // Loop until a number is found which doesn"t match the correct icon ID.
            int c = -1;
            while (b == 0) {
                c = Utils.Random.Next(1, 91);
                if (c != a) b = c;
            }

            int d = -1; // At which position the correct hash will be stored in the array.
            List<string> e = new List<string>(); // Array containing the hashes

            // Pick a random number for the correct icon.
            // Loop until a number is found which doesn"t match the previously clicked icon ID.
            while (d == -1) {
                int f = Utils.Random.Next(1, 5);
                int g = (_sessionData.LastClicked > -1) ? _sessionData.LastClicked : 0;

                if (f != g) d = f;
            }

            for (int i = 1; i < 6; i++) {
                if (i == d) {
                    e.Add(GetImageHash("icon-" + a + "-" + i, captchaId));
                } else {
                    e.Add(GetImageHash("icon-" + b + "-" + i, captchaId));
                }
            }

            // Unset the previous session data
            _sessionData.Clear();

            // Set (or override) the hashes and reset the icon request count.
            _sessionData.CorrectId = a;
            _sessionData.IncorrectId = b;
            _sessionData.Hashes = e;
            _sessionData.CorrectHash = e[d - 1];
            _sessionData.IconRequests = 0;

            // Save the session data
            SessionHolder = _sessionData;

            // Return the JSON encoded array
            return e.ToArray();
        }

        /// <summary>
        /// Checks and sets the captcha session. If the user selected the 
        /// correct image, the value will be true, else false.
        /// </summary>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <param name="pickedHash">The selected icon's hash.</param>
        /// <returns>TRUE if the correct image was selected, FALSE if not.</returns>
        public bool SetSelectedAnswer(int captchaId, string pickedHash)
        {
            // Check if the captcha ID is valid
            if(captchaId < 0)
            {
                return false;
            }

            // If the session is not loaded yet, load it.
            if (_sessionData == null)
            {
                _sessionData = SessionHolder;
            }

            // Check if the hash is set and matches the correct hash.
            if (!String.IsNullOrEmpty(pickedHash) && (GetCorrectIconHash(captchaId) == pickedHash)) {
                _sessionData.Completed = true;

                // Unset the data to at least save some space in the session.
                _sessionData.Clear();
                SessionHolder = _sessionData;

                return true;
            } else {
                _sessionData.Completed = false;
                SessionHolder = _sessionData;

                // Set the clicked icon ID
                if (_sessionData.Hashes.Contains(pickedHash))
                {
                    int index = _sessionData.Hashes.IndexOf(pickedHash);
                    _sessionData.LastClicked = index + 1;
                }
            }
    
            return false;
        }

        /// <summary>
        /// Validates the user form submission. If the captcha is incorrect, it 
        /// will set the error variable and return false, else true.
        /// </summary>
        /// <param name="form">The submitted form data.</param>
        /// <returns>NULL if the captcha was correct, an IconCaptchaException with an error message if it's not valid.</returns>
        public bool ValidateSubmission(HttpRequest request)
        {
            if (request.Form != null)
            {
                // Set the captcha id property
                int captchaId = Utils.ConvertToInt(request["captcha-idhf"]);

                // Check if the captcha ID is set.
                if (captchaId < 0)
                {
                    throw new IconCaptchaException(GetErrorMessage(3, "The captcha ID was invalid."));
                }

                // If the session is not loaded yet, load it.
                if (_sessionData == null)
                {
                    _sessionData = SessionHolder;
                }

                // Check if the hidden captcha field is set.
                if (!String.IsNullOrEmpty(request["captcha-hf"]))
                {
                    // If the hashes match, the form can be submitted. Return true.
                    string hash = GetCorrectIconHash(captchaId);
                    if (_sessionData.Completed == true && hash == request["captcha-hf"])
                    {
                        return true;
                    }
                    else
                    {
                        throw new IconCaptchaException(GetErrorMessage(0, "You've selected the wrong image."));
                    }
                }
                else
                {
                    throw new IconCaptchaException(GetErrorMessage(1, "No image has been selected."));
                }
            }
            else
            {
                throw new IconCaptchaException(GetErrorMessage(2, "You've not submitted any form."));
            }
        }

        /// <summary>
        /// Shows the icon image based on the hash. The hash matches either the 
        /// correct or incorrect id and will fetch and show the right image.
        /// </summary>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <param name="hash">The icon hash.</param>
        /// <returns>The icon as a FileStreamResult, or NULL.</returns>
        public FileStream GetIconFromHash(int captchaId, string hash)
        {
            // Check if the hash and captcha id are set
            if ((!String.IsNullOrEmpty(hash) && hash.Length == 48) && captchaId > -1) {

                _sessionData = SessionHolder;

                // Check the amount of times an icon has been requested
                if (_sessionData.IconRequests >= 5) {
                    throw new IconCaptchaException("You are not allowed to view this page.");
                }

                // Update the request counter
                _sessionData.IconRequests += 1;
                SessionHolder = _sessionData;

                // Check if the hash is present in the session data
                if (_sessionData.Hashes.Contains(hash))
                {
                    var imagePath = GetIconFilePath(captchaId, hash);
                    FileStream iconFile = GetFileStream(imagePath);

                    // Check if the icon exists
                    if (iconFile != null) {
                        return iconFile;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the correct icon hash. Used to validate the user's input.
        /// </summary>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <returns>The correct icon hash.</returns>
        private string GetCorrectIconHash(int captchaId) {

            // If the session data for this captcha is not set, simply return an empty string.
            if (_sessionData == null)
                return String.Empty;

            return (captchaId > -1) ? _sessionData.CorrectHash : String.Empty;
        }

        /// <summary>
        /// Returns the hash of an image name.
        /// </summary>
        /// <param name="image">The image name which will be hashed.</param>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <returns>The image hash.</returns>
        private string GetImageHash(string image, int captchaId) {

            string hash = (!String.IsNullOrEmpty(image) && captchaId > -1) 
                ? Utils.GetStringSha256Hash(image + Utils.Random.Next(1, 999999) + image.GetHashCode()) : String.Empty;

            return hash.Substring(0, 48); // trim the string to 48 characters, keeping it the same as the PHP version.
        }

        /// <summary>
        /// Returns either the custom set error message or the fallback message.
        /// </summary>
        /// <param name="message">The message index number, up til 4.</param>
        /// <param name="fallback">The fallback message.</param>
        /// <returns></returns>
        private string GetErrorMessage(int message, string fallback)
        {
            if(!String.IsNullOrEmpty(errorMessages[message]) && message <= 4)
            {
                return errorMessages[message];
            }

            return fallback;
        }


        /// <summary>
        /// Returns the full file path to the requested icon image.
        /// </summary>
        /// <param name="captchaId">The captcha identifier.</param>
        /// <param name="hash">The icon hash.</param>
        /// <returns>The path to the icon image.</returns>
        private string GetIconFilePath(int captchaId, string hash)
        {
            var imagePath = Path.Combine(_iconPath, _sessionData.Theme);
            imagePath = Path.Combine(imagePath, "icon-" + ((GetCorrectIconHash(captchaId) == hash) ? _sessionData.CorrectId : _sessionData.IncorrectId) + ".png");

            return imagePath;
        }

        /// <summary>
        /// Makes sure that the given file exists and returns the file stream.
        /// </summary>
        /// <param name="iconFilePath">The path to the icon image.</param>
        /// <returns>The FileStream of the image.</returns>
        private FileStream GetFileStream(string iconFilePath)
        {
            var file = new FileInfo(iconFilePath);
            if (file.Exists == false)
            {
                return null;
            }

            return new FileStream(file.FullName, FileMode.Open);
        }
    }
}
