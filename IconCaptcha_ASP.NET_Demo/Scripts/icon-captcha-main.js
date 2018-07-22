/*
    Icon Captcha Plugin ASP.NET MVC: v1.0.0
    Copyright © 2018, Fabian Wennink (https://www.fabianwennink.nl)
    
    Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

$(function() {
	$('.captcha-holder').iconCaptcha({
		captchaTheme: ["light"], // Select the theme(s) of the Captcha(s). Available: light, dark
		captchaFontFamily: '', // Change the font family of the captcha. Leaving it blank will add the default font to the end of the <body> tag.
		captchaClickDelay: 500, // The delay during which the user can't select an image.
		captchaHoverDetection: true, // Enable or disable the cursor hover detection.
		enableLoadingAnimation: true, // Enable of disable the fake loading animation. Doesn't actually do anything other than look nice.
		loadingAnimationDelay: 1500, // How long the fake loading animation should play.
		showCredits: 'show', // Show, hide or disable the credits element. Valid values: 'show', 'hide', 'disabled' (please leave it enabled).
		requestIconsDelay: 1500, // How long should the script wait before requesting the hashes and icons? (to prevent a high(er) CPU usage during a DDoS attack)
		captchaAjaxFile: '/Captcha/GetCaptcha/', // The path to the Captcha validation file.
		captchaMessages: { // You can put whatever message you want in the captcha.
			header: "Select the image that does not belong in the row",
			correct: {
				top: "Great!",
				bottom: "You do not appear to be a robot."
			},
			incorrect: {
				top: "Oops!",
				bottom: "You've selected the wrong image."
			}
		}
	});
});