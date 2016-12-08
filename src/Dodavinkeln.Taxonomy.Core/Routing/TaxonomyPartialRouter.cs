namespace Dodavinkeln.Taxonomy.Core.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Routing;
    using EPiServer;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Framework;
    using EPiServer.Framework.Cache;
    using EPiServer.ServiceLocation;
    using EPiServer.Web.Routing;
    using EPiServer.Web.Routing.Segments;

    /// <summary>
    ///     Partial router for <see cref="TaxonomyData"/>.
    /// </summary>
    /// <typeparam name="TForPageType">
    ///     The type of <see cref="PageData"/> for the routing should be extended.
    /// </typeparam>
    /// <typeparam name="TRootPageType">
    ///     The type of <see cref="PageData"/> for which the routing should route to.
    /// </typeparam>
    /// <example>The following example shows how to register a partial router.</example>
    /// <code>
    /// <![CDATA[
    ///     var taxonomyPartialRouter = new TaxonomyPartialRouter<NewsArticle, NewsArchive>(x =>
    ///      {
    ///          // Here we can return different roots depending on x.
    ///          // E.g. we could traverse and find the nearest news archive.
    ///
    ///          return new ContentReference(7);
    ///      });
    ///
    ///     RouteTable.Routes.RegisterPartialRouter(taxonomyPartialRouter);
    /// ]]>
    /// </code>
    /// <example>Register the router(s) in an <see cref="IInitializableModule"/> with a dependency to <see cref="TaxonomyInitializationModule"/>.</example>
    /// <code>
    /// <![CDATA[
    ///     [InitializableModule]
    ///     [ModuleDependency(typeof(TaxonomyInitializationModule))]
    ///     public class PartialRoutingInitialization : IInitializableModule
    ///     {
    ///         public void Initialize(InitializationEngine context)
    ///         {
    ///             // Put routers here
    ///         }
    ///
    ///         public void Uninitialize(InitializationEngine context)
    ///         {
    ///         }
    ///     }
    /// ]]>
    /// </code>
    public class TaxonomyPartialRouter<TForPageType, TRootPageType> : IPartialRouter<PageData, TaxonomyData>
        where TForPageType : PageData
        where TRootPageType : PageData
    {
        private readonly ContentRootService contentRootService;
        private readonly ContentReference taxonomyRoot;
        private readonly UrlResolver urlResolver;
        private readonly IContentLoader contentLoader;
        private readonly IContentCacheKeyCreator contentCacheKeyCreator;
        private readonly ISynchronizedObjectInstanceCache cache;

        private readonly Func<TForPageType, ContentReference> getBasePathRoot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyPartialRouter{TForPageType, TRootPageType}"/> class.
        /// </summary>
        /// <param name="basePathRoot">The function to use when evaluating the root for the router.</param>
        public TaxonomyPartialRouter(Func<TForPageType, ContentReference> basePathRoot)
        {
            this.getBasePathRoot = basePathRoot;

            this.contentRootService = ServiceLocator.Current.GetInstance<ContentRootService>();
            this.urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            this.contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            this.contentCacheKeyCreator = ServiceLocator.Current.GetInstance<IContentCacheKeyCreator>();
            this.cache = ServiceLocator.Current.GetInstance<ISynchronizedObjectInstanceCache>();

            this.taxonomyRoot = this.contentRootService.Get(TaxonomyRepositoryDescriptor.RepositoryKey);
        }

        /// <summary>
        ///     Gets a partial virtual path for a content item during routing.
        /// </summary>
        /// <param name="content">The content to generate a virtual path for.</param>
        /// <param name="language">The language to generate the url for.</param>
        /// <param name="routeValues">The route values.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns>
        ///     A <see cref="PartialRouteData"/> containing the partial virtual path
        ///     for the content and a <see cref="ContentReference"/> to the item to get base
        ///     path from or null if the remaining part did not match.
        /// </returns>
        public PartialRouteData GetPartialVirtualPath(TaxonomyData content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            // No need for routing in edit mode.
            if (requestContext.IsInEditMode())
            {
                return null;
            }

            TForPageType currentPage = null;

            if (requestContext.HttpContext != null)
            {
                // TODO: Is there a better way to get the current page from requestContext?
                currentPage = this.urlResolver.Route(new UrlBuilder(requestContext.HttpContext.Request.Url)) as TForPageType;
            }

            // We should only use this router if the current page type is matching our instance.
            if (currentPage == null)
            {
                return null;
            }

            var basePathRoot = this.GetBasePathRoot(currentPage);

            return new PartialRouteData
            {
                BasePathRoot = basePathRoot,
                PartialVirtualPath = this.GetPartialVirtualPath(basePathRoot, content)
            };
        }

        /// <summary>
        ///     Takes care of partial routing of <see cref="TaxonomyData"/> below a routed content instance.
        /// </summary>
        /// <param name="content">The content that the page route has been able to route to.</param>
        /// <param name="segmentContext">The segment context containing the remaining part of url.</param>
        /// <returns>
        ///     A <see cref="ContentReference"/> to <paramref name="content"/> and if any routed
        ///     object is found, it's added to the custom routed data object.
        /// </returns>
        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            // We should only use this router if the current page type is matching our instance.
            if (content is TRootPageType == false)
            {
                return null;
            }

            var taxonomyData = this.GetTaxonomyDataRecursively(segmentContext);

            if (taxonomyData != null)
            {
                segmentContext.SetCustomRouteData(TaxonomyDataValueProvider.Prefix, taxonomyData);

                // We should still route to the "root" content even if we found a taxonomy item.
                // We're storing the taxonomy in route data so we have a reference to it and make use
                // of it in a value provider.
                segmentContext.RoutedContentLink = content.ContentLink;

                return content;
            }

            segmentContext.RoutedContentLink = null;

            return null;
        }

        private TaxonomyData GetTaxonomyDataRecursively(SegmentContext segmentContext)
        {
            var contentReference = this.taxonomyRoot;

            var loaderOptions = new LoaderOptions
            {
                LanguageLoaderOption.FallbackWithMaster(CultureInfo.GetCultureInfo(segmentContext.Language))
            };

            TaxonomyData taxonomyData = null;

            while (true)
            {
                var segment = this.ReadNextSegment(segmentContext);

                if (string.IsNullOrEmpty(segment.Next))
                {
                    break;
                }

                var content = this.contentLoader.GetBySegment(contentReference, segment.Next, loaderOptions);

                if (content == null)
                {
                    break;
                }

                contentReference = content.ContentLink;

                if (content is TaxonomyData)
                {
                    taxonomyData = content as TaxonomyData;
                }
            }

            return taxonomyData;
        }

        private string GetPartialVirtualPath(ContentReference basePathRoot, TaxonomyData content)
        {
            var virtualPath = string.Empty;

            var cacheKey = $"Taxonomy-Url:{basePathRoot}:{content.ContentLink}";

            if (this.cache.TryGet(cacheKey, ReadStrategy.Wait, out virtualPath) == false)
            {
                var dependencies = new List<string>();
                var ancestors = this.contentLoader.GetAncestors(content.ContentLink).ToList();
                var path = new StringBuilder();

                ancestors.Insert(0, content);

                foreach (var ancestor in ancestors)
                {
                    if (ancestor is TaxonomyRoot)
                    {
                        break;
                    }

                    var routable = ancestor as IRoutable;

                    dependencies.Add(this.contentCacheKeyCreator.CreateCommonCacheKey(ancestor.ContentLink));

                    path.Insert(0, "/");
                    path.Insert(0, routable.RouteSegment);
                }

                virtualPath = path.ToString();

                this.cache.Insert(cacheKey, virtualPath, new CacheEvictionPolicy(dependencies));
            }

            return virtualPath;
        }

        private ContentReference GetBasePathRoot(TForPageType currentPage)
        {
            ContentReference basePathRoot;

            var cacheKey = $"Taxonomy-Base-Url:{currentPage.ContentLink}";

            if (this.cache.TryGet(cacheKey, ReadStrategy.Wait, out basePathRoot) == false)
            {
                basePathRoot = this.getBasePathRoot(currentPage);

                this.cache.Insert(
                    cacheKey,
                    basePathRoot,
                    new CacheEvictionPolicy(new[]
                    {
                        this.contentCacheKeyCreator.CreateCommonCacheKey(basePathRoot)
                    }));
            }

            return basePathRoot;
        }

        private SegmentPair ReadNextSegment(SegmentContext segmentContext)
        {
            var segment = segmentContext.GetNextValue(segmentContext.RemainingPath);
            segmentContext.RemainingPath = segment.Remaining;

            return segment;
        }
    }
}