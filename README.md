<br/>
<p align="center">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://i.imgur.com/k8sIUQI.png">
      <source media="(prefers-color-scheme: light)" srcset="https://i.imgur.com/RMUALSz.png">
      <img alt="IconCaptcha Logo" src="https://i.imgur.com/RMUALSz.png">
    </picture>
</p>

<p align="center">
    <strong>A self-hosted, customizable, easy-to-implement and user-friendly captcha.</strong>
</p>

<p align="center">
    <a href="https://github.com/fabianwennink/IconCaptcha-ASP.NET/releases"><img src="https://img.shields.io/badge/version-3.0.0-orange.svg?style=flat-square" alt="Version" /></a>
    <a href="https://www.nuget.org/packages/IconCaptcha"><img src="https://img.shields.io/nuget/dt/IconCaptcha?style=flat-square" alt="Downloads on NuGet" /></a>
    <a href="https://fabianwennink.nl/projects/IconCaptcha-ASP.NET/license"><img src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square" alt="License" /></a>
    <a href="https://paypal.me/nlgamevideosnl"><img src="https://img.shields.io/badge/support-PayPal-lightblue.svg?style=flat-square" alt="Support via PayPal" /></a>
    <a href="https://www.buymeacoffee.com/fabianwennink"><img src="https://img.shields.io/badge/support-Buy_Me_A_Coffee-lightblue.svg?style=flat-square" alt="Buy me a coffee" /></a>
</p>

___

Introducing IconCaptcha, a self-hosted captcha solution that's designed to be fast, user-friendly, and highly customizable. Unlike other captchas, IconCaptcha spares users the need of deciphering hard-to-read text images, solving complex math problems, or engaging with perplexing puzzle games. Instead, with IconCaptcha it's as straightforward as comparing up to 8 icons and selecting the least common one.

IconCaptcha doesn't just prioritize users; it's also developer-friendly. In just a few steps, you can have IconCaptcha integrated into your website. Even if you're new to ASP.NET and JavaScript, installing IconCaptcha is a straightforward process. The included demo pages in this repository provide all the necessary code to get IconCaptcha up and running. For more in-depth insights, take a moment to explore the information provided on this page and the wiki.

___

### [▶ Try the live demo here!](https://www.fabianwennink.nl/projects/IconCaptcha/#!demonstration)

![IconCaptcha widget examples](https://i.imgur.com/WsWdBRL.png)

**[Using PHP instead? Try IconCaptcha for PHP](https://github.com/fabianwennink/IconCaptcha-PHP)**
___

## Installation

Package Manager: `Install-Package IconCaptcha`   
.NET CLI: `dotnet add package IconCaptcha`

Once the package has been installed, continue with the remaining [setup instructions](https://github.com/fabianwennink/IconCaptcha-ASP.NET/wiki).

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

## What's New in IconCaptcha 3
In version 3 of IconCaptcha, the whole plugin got an overhaul - both client-side and server-side. With better security features, more customizations/options, new
themes, no more jQuery dependency and 250 icons, version 3 is the biggest change to IconCaptcha yet.

* No longer required to use jQuery, although IconCaptcha can still be used with jQuery.
* More captcha image generation options to increase the difficulty.
* Automatic captcha invalidation after a period of no user interaction.
* Automatic timeouts when too many incorrect selections were made by the user.
* New light and dark themes with more modern designs, with improved support for custom themes.
* Includes 250 new modern icons, created by [BlendIcons](https://blendicons.com/).
* Better stability, general code improvements and bug fixes.

# Wiki
For information on how to install, implement and configure IconCaptcha, please check the [wiki pages](https://github.com/fabianwennink/IconCaptcha-ASP.NET/wiki).

## Credits
The icons used in this project are made by [BlendIcons](https://blendicons.com/).

## License
This project is licensed under the [MIT](https://www.fabianwennink.nl/projects/IconCaptcha-ASP.NET/license) license.
