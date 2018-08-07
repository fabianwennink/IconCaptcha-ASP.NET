/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace IconCaptcha_ASP.NET.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly IconCaptcha _captcha; 

        private readonly string _contentFolder = "Content/icons";

        /// <summary>
        /// CaptchaController constructor.
        /// </summary>
        /// <param name="sessionProvider">The session provider, which acts like a wrapper for the session methods.</param>
        public CaptchaController()
        {
            ISessionProvider sessionProvider = new HttpContextSession();

            // Combine the absolute root path with the folder in which the icons are stored.
            // Make sure this is correct, else the captcha breaks.
            string pathToContent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contentFolder);

            _captcha = new IconCaptcha(sessionProvider, pathToContent);
        }

        /// <summary>
        /// Will be callled multiple times during the usage of the captcha. 
        /// Only HTTP GET and POST should be allowed here.
        /// 
        /// Calls:
        /// - Requesting the captcha data (hashes) from the servers.
        /// - Requesting the icons from the server based on the captcha ID and hash.
        /// - Validating the user's selected icon input.
        /// </summary>
        /// <returns>
        /// A JSON string, or NULL. The reason for multiple return types is to not 
        /// break compatibility with the JavaScript front-end script (since it has only 1 request endpoint).
        /// </returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public object GetCaptcha()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;

            IconCaptchaResult result = IconCaptchaExtension.CallIconCaptcha(_captcha, request, response);

            // Go through every result state. Custom code can be added here as well, if you like.
            switch (result.CaptchaState)
            {
                case IconCaptchaState.CaptchaHashesReturned:
                    return Json(result.CaptchaResult as string[], JsonRequestBehavior.AllowGet);
                case IconCaptchaState.CaptchaImageReturned:
                    return File(result.CaptchaResult as FileStream, "image/png");
                case IconCaptchaState.CaptchaIconSelected:
                case IconCaptchaState.CaptchaGeneralFail:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Will be called when the captcha is submitted to the server. It validated the captcha 
        /// once more to make sure the captcha was really completed.
        /// 
        /// The implementation of the ValidateSubmission() method is just one example, there are other implementations possible.
        /// In case the validation fails, an IconCaptchaException will be thrown. ValidateSubmission() also returns a boolean TRUE when it's validated.
        /// </summary>
        /// <returns>A new view.</returns>
        [HttpPost]
        public ActionResult SubmitForm()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;

            try
            {
                // Validate the captcha.
                _captcha.ValidateSubmission(request);

                // Return the success view.
                return View("Success");
            }
            catch(IconCaptchaException ex)
            {
                // You can use ex.Message to show the custom error message.
                // Debug.WriteLine(ex.Message);

                // Return the error view.
                return View("Error");
            }
        }

        public ActionResult Index()
        {
            return View();
        }
    }

    /// <summary>
    /// Acts as a wrapper for the session handler. Put it somewhere in a class if 
    /// you want to use the captcha in multiple controllers.
    /// </summary>
    public class HttpContextSession : ISessionProvider
    {
        public IconCaptchaSession GetSession(string key)
        {
            return (IconCaptchaSession)HttpContext.Current.Session[key];
        }

        public void SetSession(string key, IconCaptchaSession value)
        {
            HttpContext.Current.Session[key] = value;
        }
    }
}