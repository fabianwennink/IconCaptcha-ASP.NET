/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using Microsoft.AspNetCore.Mvc;

namespace IconCaptcha.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IconCaptcha _captcha; 

        private readonly string _contentFolder = "assets/icons";

        public ActionResult Index()
        {
            return View();
        }

        public HomeController(IconCaptcha captcha)
        {
            _captcha = captcha;
            // ISessionProvider sessionProvider = new HttpContextSession(HttpContext);

            // // Combine the absolute root path with the folder in which the icons are stored.
            // // Make sure this is correct, else the captcha breaks.
            // string pathToContent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contentFolder);
            //
            // _captcha = new IconCaptcha(sessionProvider, pathToContent);
        }

        [HttpGet]
        [Route("regular-form")]
        public ActionResult RegularForm()
        {
            return View();
        }

        [HttpGet]
        [Route("ajax-form")]
        public ActionResult AjaxForm()
        {
            return View();
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
        // [HttpGet]
        // [HttpPost]
        // public object GetCaptcha()
        // {
        //     HttpRequest request = HttpContext.Request;
        //     HttpResponse response = HttpContext.Response;
        //
        //     IconCaptchaResult result = IconCaptchaExtension.CallIconCaptcha(_captcha, request, response);
        //
        //     // Go through every result state. Custom code can be added here as well, if you like.
        //     switch (result.CaptchaState)
        //     {
        //         case IconCaptchaState.CaptchaHashesReturned:
        //             return Json(result.CaptchaResult as string[]);
        //         case IconCaptchaState.CaptchaImageReturned:
        //             return File(result.CaptchaResult as FileStream, "image/png");
        //         case IconCaptchaState.CaptchaIconSelected:
        //         case IconCaptchaState.CaptchaGeneralFail:
        //         default:
        //             return null;
        //     }
        // }

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
            HttpRequest request = HttpContext.Request;

            try
            {
                // Validate the captcha.
                _captcha.ValidateSubmission(request.Form);

                // Return the success view.
                return View("Success");
            }
            catch(SubmissionException ex)
            {
                // You can use ex.Message to show the custom error message.
                // Debug.WriteLine(ex.Message);

                // Return the error view.
                return View("Error");
            }
        }
    }
}
