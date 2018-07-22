/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System.IO;
using System.Net;
using System.Web;

namespace IconCaptcha_ASP
{
    public static class IconCaptchaExtension
    {
        public static IconCaptchaResult CallIconCaptcha(IconCaptcha captcha, HttpRequest request, HttpResponse response)
        {
            bool isAjaxRequest = request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // HTTP POST - Either the captcha has been submitted or an image has been selected by the user.
            if (isAjaxRequest && request.Form != null)
            {
                int captchaId = Utils.ConvertToInt(request["cID"]);
                int requestType = Utils.ConvertToInt(request["rT"]);

                // Make sure both the captcha ID and request type are valid.
                if (captchaId > -1 && requestType > -1)
                {
                    switch (requestType)
                    {
                        case 1:

                            // Return the JSON string containing the captcha hashes.
                            return new IconCaptchaResult(IconCaptchaState.CaptchaHashesReturned, captcha.GetCaptchaData(captchaId, request["tM"]));
                        case 2:

                            // If the correct image was selected, return with HTTP code 200.
                            if (captcha.SetSelectedAnswer(captchaId, request["pC"]))
                            {
                                // Set the response code as 200 OK
                                response.StatusCode = (int)HttpStatusCode.OK;
                                response.End();

                                // Return nothing, other than the captcha state.
                                return new IconCaptchaResult(IconCaptchaState.CaptchaIconSelected, null);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else // HTTP GET - Requesting the actual image.
            {
                int captchaId = Utils.ConvertToInt(request["cid"]);

                if (captchaId > -1)
                {
                    // Only return a new File if the value is actually set.
                    FileStream image = captcha.GetIconFromHash(captchaId, request["hash"]);
                    if (image != null)
                    {
                        // Return the image stream.
                        return new IconCaptchaResult(IconCaptchaState.CaptchaImageReturned, image);
                    }
                }
            }

            // Set the response code as a 400 Bad Request.
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.End();

            // Return nothing, other than the captcha state.
            return new IconCaptchaResult(IconCaptchaState.CaptchaGeneralFail, null);
        }
    }
}