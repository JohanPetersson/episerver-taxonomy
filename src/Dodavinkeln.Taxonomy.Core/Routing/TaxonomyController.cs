namespace Dodavinkeln.Taxonomy.Core.Routing
{
    using System.Web.Mvc;
    using EPiServer.Web.Mvc;
    using EPiServer.Web.Routing;

    /// <summary>
    ///     Dummy controller so <see cref="UrlResolver"/> can actually route to
    ///     <see cref="TaxonomyData"/> even though we don't have a valid template.
    /// </summary>
    public class TaxonomyController : ContentController<TaxonomyData>
    {
        /// <summary>
        ///     The default action method.
        /// </summary>
        /// <param name="currentContent">The current content.</param>
        /// <returns>Returns <see cref="HttpNotFoundResult"/>.</returns>
        public ActionResult Index(TaxonomyData currentContent)
        {
            return this.HttpNotFound();
        }
    }
}