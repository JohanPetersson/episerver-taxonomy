# Episerver Taxonomy

By Johan Petersson [http://dodavinkeln.se](http://dodavinkeln.se)

## What it is

This an Episerver Add-on that helps editors to manage taxonomy within Episerver's editing UI. Developers can define different types of taxonomy items, please see the [how to use section](#how-to-use) below.

The Add-on is installed through NuGet. The packages are, at this moment, not published to a public feed yet so you have to [download them](releases) manually, version 1.0 will be published to a public feed. The Add-on is split into two packages, one for the core functionality that you can install in any class library without getting a lot of UI files and one package for the UI that you can install in the web project.

Please see [known issues](#known-issues) and [planned features](#planned-features).

## Requirements

* EPiServer.CMS.UI.Core 10.1.1
* EPiServer.CMS.Core 10.1.0

## How to use

At least one taxonomy data must be defined to give editors anything to work with:

    using Dodavinkeln.Taxonomy.Core;
    using EPiServer.DataAnnotations;

    [ContentType(GUID = "974ebaf3-c6fc-4332-a809-344b7e372f21")]
    public class CategoryData : TaxonomyData
    {
    }

The only thing that's needed is to inherit from `TaxonomyData`. You can define different types of taxonomy items, e.g. `CategoryData`, `CountryData` and add more properties to the content types.

Then you can add a properties to content types so editors can select taxonomy items. Since `TaxonomyData` is an implementation of `IContent` you can use any property that accepts `ContentReference`. This Add-on has a convenient way to define a `ContentReference` property by marking it with the `[Taxonomy]` attribute:

    [Taxonomy]
    [Display(Name = "News category")]
    public virtual ContentReference NewsCategory { get; set; }

Now the property is limitied to only accepts `TaxonomyData` and the select dialog is scoped to the taxonomy root. You can also use `ContentArea` in conjuction with `AllowedTypes(typeof(TaxonomyData))` to create a property for "tagging".

To render the category in a view you can use the built-in functionality:

    @Html.PropertyFor(x => x.CurrentPage.NewsCategory)

This will output an anchor link `<a href="/taxonomy/categories/the-category">The category</a>`. This is probably not what we want to render, because the category might be used on different kinds of content types. What we want is a link that points to different kinds of "archives" depending on where it's used. That's why this Add-on also includes a partial router. You can define multiple routers for different kinds of content types, but at least one is needed:

    using System.Web.Routing;
    using Dodavinkeln.Taxonomy.Core;
    using Dodavinkeln.Taxonomy.Core.Routing;
    using EPiServer.Core;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Web.Routing;

    [InitializableModule]
    [ModuleDependency(typeof(TaxonomyInitializationModule))]
    public class TaxonomyPartialRouterInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var taxonomyPartialRouter = new TaxonomyPartialRouter<NewsArticlePage, NewsArchivePage>(x =>
            {
                // Here we can return different roots depending on where x is located in the structure.
                // E.g. we could traverse and find the nearest news archive if we have multiple ones.

                return new ContentReference(7);
            });

            RouteTable.Routes.RegisterPartialRouter(taxonomyPartialRouter);
        }

        public void Uninitialize(InitializationEngine context)
        {
            // Nothing
        }
    }

It's important to have a module dependency to `TaxonomyInitializationModule` since the Add-on must be initialized before the router.
Given that the page with id **7** is a news archive (or we have some logic that traverses the tree and find the nearest one) the link will now look like this `<a href="/en/news-archive/categories/the-category">The category</a>`. Note that the URL is now based from the news archive instead.
If you want to e.g. filter the news articles based on this selected category, you only have to add a parameter to your action method in the controller for the news archive:

    public ActionResult Index(NewsArchivePage currentPage, CategoryData currentTaxonomyData)
    {
        var viewModel = GetYourViewModel();

        if (currentTaxonomyData != null)
        {
            // Filter news articles based on the category

            viewModel.CurrentCategory = currentTaxonomyData;
        }

        return this.View(viewModel);
    }

Important part here is the name `currentTaxonomyData`.

## Known issues

The Add-on doesn't support sorting of taxonomy yet and will throw errors when sorting items.

## Contribute

Please report issues here on GitHub, and why not clone the repository and create pull requests! Please see the [planned features](#planned-features) section for what you can contribute with.

## Planned features

These features are under consideration.

* Sorting
* Query API based on Episerver Find and/or `IContentSoftLinkRepository`.
* More routing options, e.g. based on taxonomy id and support for multiples items.
* Linking between taxonomy.
* Property type based on `ContentArea`.