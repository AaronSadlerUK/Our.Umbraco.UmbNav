# Integration guide

UmbNav was designed to be as clean and easy for developers as it is for editors to use.

## Strongly typed models

Out of the box UmbNav can return strongly typed models for a given navigation.

The following properties are available in the `UmbNavItem` class:

| Property          | Type              | Description |
|-------------------|-------------------|-------------|
| Id                | Int               | The node ID of the selected content item. For external linking nav items this will be "0" |
| Udi               | GuidUdi           | The node UDI of the selected content item. For external linking nav items this will be null |
| Title             | String            | The link title, often the node name |
| Target            | String            | The link target |
| Noopener          | String          	| noopener if set in the backoffice |
| Noreferrer        | String         	| noreferrer  if set in the backoffice |
| Url               | String            | The link url |
| QueryString       | String            | The link querystring / anchor |
| Level             | Int               | The level in the overall navigation that the current item sits at |
| Content           | IPublishedContent | The IPublishedContent for the selected content item. For external linking nav items this will be null |
| Children          | List              | The picked child / sub items for the current item |
| Culture           | String            | The link culture
| CustomClasses     | String            | Any CSS classes set in the backoffice
| ImageUrl     		| String            | An image url as set in the backoffice

## Implementing Razor

UmbNav was designed to closely follow the "Umbraco way" of doing things so we don't impose our own styles or markup on you.

It's easy to implement in your own Razor:

```csharp
@inherits Umbraco.Web.Mvc.UmbracoViewPage
@using UmbNav.Core.Extensions
@using UmbNav.Core.Models
@using Umbraco.Web;
@{
    var site = Model.Root();
    var umbNav = site.FirstChildOfType("siteSettings").Value<IEnumerable<UmbNavItem>>("umbNavPE");
}
@foreach (var item in umbNav)
{
    <a class="nav-link " href="@item.Url()" target="@item.Target" rel="@item.Noopener @item.Noreferrer">@item.Title</a>
}
```

There is also some helpers available to generate the each menu item:

 ```csharp
 GetLinkHtml(this UmbNavItem item, string culture = null, UrlMode mode = UrlMode.Default, object htmlAttributes = null)
 ```

and in Umbraco V9 there is a TagHelper available:

```csharp
<umbnavitem menu-item="item" mode="UrlMode" culture="Culture"></umbnavitem>
```
To make the tag helper work in Umbraco V9 you will need to add the following you your `_ViewImports.cshtml`

```csharp
@using UmbNav.Core
@addTagHelper *, UmbNav.Core
```

**Note:** For large navigations with multiple levels it is highly recommended that you cache your navigation for optimal performance.