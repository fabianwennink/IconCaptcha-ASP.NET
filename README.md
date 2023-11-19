# IconCaptcha Plugin - ASP.NET

[![Version](https://img.shields.io/badge/Version-1.0.0-orange.svg?style=flat-square)]() [![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)]()
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg?style=flat-square)](https://paypal.me/nlgamevideosnl)

<br>![IconCaptcha Logo](http://i.imgur.com/RMUALSz.png)

IconCaptcha is a faster and more user-friendly captcha than most other captchas. You no longer have to read any annoying 
text-images, with IconCaptcha you only have to compare two images and select the image which is only present once.

Besides being user-friendly, IconCaptcha is also developer-friendly. With just a few steps you can get the captcha up and running. 
Even developers new to ASP.NET can easily install IconCaptcha. The demo web application contains all the code needed to get the captcha working.

![Preview](https://i.imgur.com/WsWdBRL.png)
___
### [View live demo](https://www.fabianwennink.nl/projects/IconCaptcha/)
### [Download IconCaptcha for ASP.NET now](https://github.com/fabianwennink/IconCaptcha-ASP.NET/releases)
___

##### [Visit IconCaptcha for PHP here.](https://github.com/fabianwennink/IconCaptcha-PHP/releases)
___

## Features
* __User Friendly:__ The captcha uses easily understandable images instead of hard to read texts to complete the captcha.
* __Server-side Validation:__ All validation done by the captcha will be performed on the server-side instead of the client-side.
* __Self Hosted:__ Because IconCaptcha is a self-hosted plugin, you are not relying on any third party.
* __No Data Sharing:__ Unlike captchas such as Google ReCaptcha, no user data will be stored or sold to third parties.
* __jQuery Support:__ IconCaptcha is written in plain JavaScript, but hooks into jQuery to allow you to integrate it in your jQuery code.
* __Modern Design:__ The look and feel of IconCaptcha fits every modern website design.
* __Events:__ Events are triggered at various points in the code, allowing you to hook into them and execute custom code if necessary.
* __Themes:__ Select the design of the captcha without having to ever touch a stylesheet, or create your own custom theme.
* __SASS:__ The project contains a SASS file, allowing you to easily style and compile the stylesheet.
* __IE 10+ Support:__ IconCaptcha has been tested in Internet Explorer 10 & 11 and is functional in both versions.

## New in v3
In version 3 of IconCaptcha, the whole plugin got an overhaul - both client-side and server-side. With better security features, more customizations/options, new
themes, no more jQuery dependency and 250 icons, version 3 is the biggest change to IconCaptcha yet.

* No longer required to use jQuery, although IconCaptcha can still be used with jQuery.
* More captcha image generation options to increase the difficulty.
* Automatic captcha invalidation after a period of no user interaction.
* Automatic timeouts when too many incorrect selections were made by the user.
* New light and dark themes with more modern designs, with improved support for custom themes.
* Includes 250 new modern icons, created by [BlendIcons](https://blendicons.com/).
* Better stability, general code improvements and bug fixes.

## Requirements
* __.NET framework >= 4.0__
* __ASP.NET MVC >= 5.0__
* __jQuery >= 1.12.3__

## Installation

### __Install via NuGet:__
1. Install IconCaptcha with NuGet by executing one of the following commands:

* Package Manager: ```Install-Package IconCaptcha```
* .NET CLI: ```dotnet add package IconCaptcha```

2. Change the namespace of the ```Controllers/CaptchaController.cs``` to your liking.
3. Change the ```icon-captcha-main.js``` file settings to your liking, make sure the _captchaAjaxFile_ setting is set correctly.
4. Include ```~/Scripts/icon-captcha.min.js```, ```~/Scripts/icon-captcha-main.js``` and ```~/Content/icon-captcha.css``` into your layout by either adding them to the ```App_Start/BundleConfig.cs``` or by adding them directly into the HTML.


### __Install manually:__
1. Download [IconCaptcha for ASP.NET](https://github.com/fabianwennink/IconCaptcha-ASP.NET/releases).
2. Download the [IconCaptcha Front-End package](https://github.com/fabianwennink/IconCaptcha-Widget/releases).
3. Unpack both repositories to somewhere on your computer.
4. Drag the content of the ```dist/css/``` and ```dist/icons/``` folders of the IconCaptcha Front-End package into the ```Content/``` folder of your ASP.NET Web Application.
5. Drag the content of the ```dist/js/``` folder of the IconCaptcha Front-End package into the ```Scipts/``` folder of your ASP.NET Web Application.
6. Create a new ```icon-captcha-main.js``` script containing the IconCaptcha script initializer (see demo webapp): ```$('.captcha-holder').iconCaptcha();```
7. Include ```~/Scripts/icon-captcha.min.js```, ```~/Scripts/icon-captcha-main.js``` and ```~/Content/icon-captcha.css``` into your layout by either adding them to the ```App_Start/BundleConfig.cs``` or by adding them directly into the HTML.
8. Build the code inside of the ```IconCaptcha/``` folder and add the generated .DLL to your ASP.NET Web Application's references _(Solution -> Add -> Reference -> Browse -> Select IconCaptcha.dll)_.
9. Copy the code from the ```Controllers/CaptchaController.cs``` or write your own validation implementation.

Before running the demo Web Application, make sure to restore the missing NuGet packages first.

_Note: To make it easier to test the example captchas, the Front-End package is already included in this repository. 
However, these files might not always be up-to-date. To ensure you always have the latest version, please follow the instructions above._

## Usage

__HTML form:__
```html
<form action="SubmitForm/" method="post">
    ...
    
    <!-- The captcha will be generated in this element -->
    <div class="captcha-holder"></div>

    ...
</form>

...

<!-- Either add the initializer into the HTML, or follow the installation instructions.  -->
<script>
    $('.captcha-holder').iconCaptcha({
        // The captcha options go here
    });
</script>
```


__ASP.NET implementation and validation:__

```csharp
 // See implementation at /Controllers/CaptchaController.cs
```

## Options

The following options allow you to customize IconCaptcha to your liking. All of the options are __optional__, however you might still want to take a look at the ```captchaAjaxFile``` option and make sure the path is set correctly.

| Option | Description |
| ------ | ------ |
| captchaTheme | Allows you to select the theme of the captcha. At the moment you can only choose between _light_ and _dark_. You can always add your own custom themes by changing the SCSS file. |
| captchaFontFamily | Allows you to select the font family of the captcha. Leaving this option blank will result in the use of the default font ```Arial```. |
| captchaClickDelay | The time _(in milliseconds)_ during which the user can't select an image. Set to 0 to disable. |
| captchaHoverDetection | Prevent clicking on any captcha icon until the cursor hovered over the captcha at least once. |
| enableLoadingAnimation | Enable or disable the _fake_ loading animation after clicking on an image.  |
| loadingAnimationDelay | The time _(in milliseconds)_ during which the _fake_ loading animation will play until the user input actually gets validated. |
| showCredits | Show, hide or disable the credits element of the captcha. Hiding the credits will still add the credits to the HTML, but it will not be visible (only to crawlers). Disabling the credits will neither show or add the HTML. Use _'show'_, _'hide'_ or _'disabled'_.<br>_Please leave it enabled so people can find and use the captcha themselves._ |
| captchaAjaxFile | The path to the Controller. Make sure you use the correct path else the captcha won't be able to request the hashes, images or validate the input. |
| captchaMessages | Change the messages used by the captcha. All the changeable strings can be found down below. |
| requestIconsDelay | The captcha will not request hashes or images from the server until after this delay _(in milliseconds)_. If a page displaying one or more captchas gets constantly refreshed (during an attack?), it will not request the resources right away. |

## Messages
The following strings will be used by the captcha and can be changed / translated to your liking.

| Error/event | Default | C#/JS |
| ------ | ------ | ------ |
| Captcha Header | Select the icon that does not belong in the row. | JS |
| Captcha Correct Title | Great! | JS |
| Captcha Correct Subtitle | You do not appear to be a robot. | JS |
| Captcha Incorrect Title | Oops! | JS |
| Captcha Incorrect Subtitle | You've selected the wrong image. | JS |
| Wrong Image | You've selected the wrong image. | C# |
| No Image Selected | No image has been selected. | C# |
| Empty Form | You've not submitted any form. | C# |
| Invalid Captcha ID | The captcha ID was invalid. | C# |

## Events
Events will be triggered at various point in the code. You can use a custom script to hook into the events and execute your own code if necessary.

| Event | Description |
| ------ | ------ |
| init.iconCaptcha | Will fire when the captcha has been fully built and the hashes and icons have been requested from the server. |
| refreshed.iconCaptcha | Will fire when the user selected the incorrect icon and the captcha is done refreshing it's hashes and icons. |
| selected.iconCaptcha | Will fire when the user selects an icon, regarless of if the icon is correct or not. |
| success.iconCaptcha | Will fire when the user selected the correct icon. |
| error.iconCaptcha | Will fire when the user selected the incorrect icon. |

## Credits
The icons used in this project are made by [BlendIcons](https://blendicons.com/).

## License
This project is licensed under the [MIT](https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license) license.
