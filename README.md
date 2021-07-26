# Umbraco UmbNav

[![Build status](https://dev.azure.com/TerrabitHost/UmbNav/_apis/build/status/UmbNavV8?branchName=develop)](https://dev.azure.com/TerrabitHost/UmbNav/_build/latest?definitionId=28)

[![NuGet Badge](https://buildstats.info/nuget/Our.Umbraco.UmbNav.Web?includePreReleases=true)](https://www.nuget.org/packages/Our.Umbraco.UmbNav.Web)


UmbNav adds a drag and drop menu builder to the Umbraco V8 backoffice.

## Getting started

This package is supported on Umbraco 8.7+ and Umbraco V9.0+

### Features

- Set maximum child levels
- Hide menu items where `umbracoNaviHide` is true
- Auto expand the backoffice menu tree on hover
- Set the delay of the auto expand on hover (in ms)
- Add `noopener` to external links by clicking a checkbox
- Add `noreferrer` to external links by clicking a checkbox
- Auto add child nodes when rendering on the front end
- Allow menu items to be shown / hidden depending on member authentication status
- Add custom CSS classes to each menu item in the backoffice
- Display the property editor as full width in the back office (Hide the label)
- Enter an image url, id, guid or udi for each menu item this will be converted to the relevant url on render
- TagHelper for Umbraco V9
- GetLinkHtml extension for Umbraco V8 and V9

### Installation

UmbNav is available from [Our Umbraco (V8 only)](https://our.umbraco.com/packages/backoffice-extensions/umbnav), [NuGet](https://www.nuget.org/packages/Our.Umbraco.UmbNav.Web), or as a manual download directly from GitHub.

#### NuGet package repository
To [install UI from NuGet](https://www.nuget.org/packages/Our.Umbraco.UmbNav.Web), run the following command in your instance of Visual Studio.

    PM> Install-Package Our.Umbraco.UmbNav.Web
	
To [install Core from NuGet](https://www.nuget.org/packages/Our.Umbraco.UmbNav.Core), run the following command in your instance of Visual Studio.

    PM> Install-Package Our.Umbraco.UmbNav.Core

To [install API from NuGet](https://www.nuget.org/packages/Our.Umbraco.UmbNav.Api), run the following command in your instance of Visual Studio.

    PM> Install-Package Our.Umbraco.UmbNav.Api

## Umbraco Cloud Supported

UmbNav fully supports Umbraco Cloud including the content synchroniser, it has been fully tested transferring and restoring between environments.

### Documentation

After installing the package, you will have a new property editor called UmbNav in the Umbraco backoffice, typically this would get added to your sites "Site Settings" or "Home" node.

Check out the integration guide [integration guide](docs/integration-guide.md) to learn how to embed the package in your site.

### Screenshots

![](https://raw.githubusercontent.com/AaronSadlerUK/Our.Umbraco.UmbNav/develop/Screenshots/UmbNav.1.jpeg)

![](https://raw.githubusercontent.com/AaronSadlerUK/Our.Umbraco.UmbNav/develop/Screenshots/UmbNav.2.jpeg)

![](https://raw.githubusercontent.com/AaronSadlerUK/Our.Umbraco.UmbNav/develop/Screenshots/UmbNav.3.jpeg)

![](https://raw.githubusercontent.com/AaronSadlerUK/Our.Umbraco.UmbNav/develop/Screenshots/UmbNav.4.jpeg)

![](https://raw.githubusercontent.com/AaronSadlerUK/Our.Umbraco.UmbNav/develop/Screenshots/UmbNav.5.jpeg)


### Contribution guidelines

To raise a new bug, create an issue on the GitHub repository. To fix a bug or add new features, fork the repository and send a pull request with your changes. Feel free to add ideas to the repository's issues list if you would to discuss anything related to the package.

### Who do I talk to?
This project is maintained by [Aaron Sadler](https://aaronsadler.uk) and contributors. If you have any questions about the project please contact me through [Twitter](https://twitter.com/AaronSadlerUK), or by raising an issue on GitHub.

## License

Copyright &copy; 2021 [UmbHost Limited](https://umbhost.net), and other contributors

Licensed under the MIT License.

As per the spirit of the MIT Licence, feel free to fork and do what you wish with the source code, all I ask is that if you find a bug or add a feature please create a to PR this repository.
