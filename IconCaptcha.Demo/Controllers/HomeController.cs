using IconCaptcha.Demo.ViewModels;
using IconCaptcha.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IconCaptcha.Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IconCaptchaService _captcha; 

        public ActionResult Index()
        {
            return View();
        }

        public HomeController(IconCaptchaService captcha)
        {
            _captcha = captcha;
        }

        /// <summary>
        /// Handles the HTTP GET request for the regular, non-AJAX form page.
        /// </summary>
        [HttpGet("regular-form")]
        public ActionResult RegularForm()
        {
            return View(new SubmissionViewModel());
        }

        /// <summary>
        /// Handles the HTTP POST request for submitting a regular, non-AJAX form.
        /// </summary>
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
            catch (IconCaptchaSubmissionException e)
            {
                submissionViewModel.Error = true;
                submissionViewModel.ErrorMessage = e.Message;
            }

            return View("RegularForm", submissionViewModel);
        }

        /// <summary>
        /// Handles the HTTP GET request for the AJAX form page.
        /// </summary>
        [HttpGet("ajax-form")]
        public ActionResult AjaxForm()
        {
            return View();
        }

        /// <summary>
        /// Handles the HTTP POST request for submitting the AJAX form.
        /// The AJAX form simply expects either a success or error message to be returned.
        /// </summary>
        [HttpPost("ajax-form-submit")]
        public ActionResult<string> AjaxFormSubmit()
        {
            try
            {
                _captcha.ValidateSubmission();

                return "It looks like you are a human";
            }
            catch (IconCaptchaSubmissionException e)
            {
                return e.Message;
            }
        }
    }
}
