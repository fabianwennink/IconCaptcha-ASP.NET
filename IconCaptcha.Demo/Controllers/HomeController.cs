/*
    Icon Captcha Plugin ASP.NET MVC: v3.1.2
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using IconCaptcha.Demo.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IconCaptcha.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IconCaptchaService _captcha; 

        private readonly string _contentFolder = "assets/icons";

        public ActionResult Index()
        {
            return View();
        }

        public HomeController(IconCaptchaService captcha)
        {
            _captcha = captcha;
            // ISessionProvider sessionProvider = new HttpContextSession(HttpContext);

            // // Combine the absolute root path with the folder in which the icons are stored.
            // // Make sure this is correct, else the captcha breaks.
            // string pathToContent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contentFolder);
            //
            // _captcha = new IconCaptcha(sessionProvider, pathToContent);
        }

        [HttpGet("regular-form")]
        public ActionResult RegularForm()
        {
            return View(new SubmissionViewModel());
        }

        [HttpPost("regular-form-submit")]
        public ActionResult RegularFormSubmit()
        {
            var submissionViewModel = new SubmissionViewModel
            {
                Error = false,
            };
            
            try
            {
                _captcha.ValidateSubmission();
            }
            catch (SubmissionException e)
            {
                submissionViewModel.Error = true;
                submissionViewModel.ErrorMessage = e.Message;
            }

            return View("RegularForm", submissionViewModel);
        }

        [HttpGet("ajax-form")]
        public ActionResult AjaxForm()
        {
            return View();
        }

        [HttpPost("ajax-form-submit")]
        public ActionResult<string> AjaxFormSubmit()
        {
            try
            {
                _captcha.ValidateSubmission();

                return "It looks like you are a human";
            }
            catch (SubmissionException e)
            {
                return e.Message;
            }
        }
    }
}
