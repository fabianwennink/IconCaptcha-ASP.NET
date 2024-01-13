/*
    IconCaptcha - Copyright 2024, Fabian Wennink (https://www.fabianwennink.nl)
    Licensed under the MIT license: https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license
 
    The above copyright notice and license shall be included in all copies or substantial portions of the software.
*/

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IconCaptcha.Dto;
using IconCaptcha.Enums;
using IconCaptcha.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IconCaptcha
{
    public class IconCaptchaMiddleware : IMiddleware
    {
        private readonly IconCaptchaService _captcha;

        private readonly JsonSerializerOptions _serializeOptions;
        
        public IconCaptchaMiddleware(IconCaptchaService captcha)
        {
            _captcha = captcha;
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var isAjaxRequest = IsAjaxRequest(context.Request);

            // HTTP GET
            if (HttpMethods.IsGet(context.Request.Method) && !isAjaxRequest && context.Request.Query.TryGetValue("payload", out var payloadString))
            {
                // Decode the payload.
                var payload = DecodePayload(payloadString, context.Request);

                await _captcha.GetImage(payload.CaptchaId);
                
                return;
            }

            // HTTP POST
            if (HttpMethods.IsPost(context.Request.Method) && isAjaxRequest && context.Request.Form.TryGetValue("payload", out payloadString))
            {
                // Decode the payload.
                var payload = DecodePayload(payloadString, context.Request);

                switch (payload.Action)
                {
                    // The visitor initiated a challenge.
                    case ActionType.InitiateChallenge:
                        var jsonData = JsonSerializer.Serialize(_captcha.GetCaptchaData(payload), _serializeOptions);
                        await context.Response.WriteAsync(Utils.Base64Encode(jsonData));
                        return;
                    
                    // The visitor selected an icon.
                    case ActionType.SetSelectedIcon:
                        if (_captcha.SetSelectedAnswer(payload)) {
                            return;
                        }
                        break;
                    
                    // The captcha expired client-side, expire the challenge.
                    case ActionType.TimeExpired:
                        _captcha.InvalidateSession(payload.CaptchaId);
                        return;
                }
            }
            
            await next(context);
        }

        /// <summary>
        /// Checks if the current HTTP request was an Ajax request.
        /// </summary>
        private bool IsAjaxRequest(HttpRequest contextRequest)
        {
            return contextRequest.Headers.TryGetValue("X-Requested-With", out var requestedWidth) 
                   && "xmlhttprequest".Equals(requestedWidth, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates the payload and possibly the header tokens.
        /// <param name="payload">The request payload to validate.</param>
        /// <param name="checkHeader">Whether the header token should also be checked.</param>
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
        /// <param name="payloadString">The encoded payload string to decode.</param>
        /// </summary>
        private Payload DecodePayload(string payloadString, HttpRequest contextRequest)
        {
            var payload = JsonSerializer.Deserialize<Payload>(Utils.Base64Decode(payloadString));

            if (!IsValidToken(payload, false, contextRequest))
            {
                throw new IconCaptchaException("Invalid token format.", 2);
            }

            return payload;
        }
    }
}
