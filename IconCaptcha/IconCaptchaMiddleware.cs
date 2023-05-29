using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IconCaptcha
{
    public class IconCaptchaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IconCaptcha _captcha;

        public IconCaptchaMiddleware(RequestDelegate next, IconCaptcha captcha)
        {
            _next = next;
            _captcha = captcha;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var isAjaxRequest = IsAjaxRequest(context.Request);
            
            StringValues payloadString;
            
            // HTTP GET
            if (HttpMethods.IsGet(context.Request.Method) && !isAjaxRequest && context.Request.Query.TryGetValue("payload", out payloadString))
            {
                // Decode the payload.
                var payload = DecodePayload(payloadString, context.Request);
                
                // Validate the payload content. TODO
                // if (!isset($payload, $payload['i']) || !is_numeric($payload['i'])) {
                //     badRequest();
                // }
                
                await _captcha.GetImage(payload.CaptchaId);
                
                return;
            }

            // HTTP POST
            if (HttpMethods.IsPost(context.Request.Method) && isAjaxRequest && context.Request.Form.TryGetValue("payload", out payloadString))
            {
                // Decode the payload.
                var payload = DecodePayload(payloadString, context.Request);
                
                // Validate the payload content.
                // if (!isset($payload, $payload['a'], $payload['i']) || !is_numeric($payload['a']) || !is_numeric($payload['i'])) {
                //     badRequest();
                // }

                switch (payload.Action)
                {
                    case ActionType.RequestImageHashes: // Requesting the image hashes
                        // Echo the captcha data.
                        var jsonData = JsonSerializer.Serialize(_captcha.GetCaptchaData(payload));
                        
                        await context.Response.WriteAsync(Util.Base64Encode(jsonData));
                        return;
                    
                    case ActionType.SetUserChoice: // Setting the user's choice
                        if (_captcha.SetSelectedAnswer(payload)) {
                            return;
                        }
                        break;
                    
                    case ActionType.TimeExpired: // Captcha interaction time expired.
                        _captcha.InvalidateSession(payload.CaptchaId);
                        return;
                }
                
                throw new SubmissionException(4, "OOOPS"); // TODO FIXME
            }
            
            await _next(context);
        }

        /// <summary>
        /// Adds another level of security to the Ajax call. Only requests made through Ajax are allowed.
        /// </summary>
        private bool IsAjaxRequest(HttpRequest contextRequest)
        {
            return contextRequest.Headers.TryGetValue("X-Requested-With", out var requestedWidth) 
                   && "xmlhttprequest".Equals(requestedWidth, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates the payload and possibly the header tokens.
        /// </summary>
        private bool IsValidToken(Payload payload, bool checkHeader, HttpRequest contextRequest)
        {
            // Get the token from the request headers, can be empty for some requests.
            string headerToken = null;
            if (checkHeader && contextRequest.Headers.TryGetValue("X-IconCaptcha-Token", out var token))
            {
                headerToken = token.First();
            }

            // Validate the tokens.
            return _captcha.ValidateToken(payload.Token, headerToken);
        }

        /// <summary>
        /// Tries to decode the given base64 and json encoded payload.
        /// </summary>
        private Payload DecodePayload(string payloadString, HttpRequest contextRequest)
        {
            var payload = JsonSerializer.Deserialize<Payload>(Util.Base64Decode(payloadString));

            // TODO check values
            if (!IsValidToken(payload, false, contextRequest))
            {
                throw new Exception("TODO");
            }

            return payload;
        }
    }
}
