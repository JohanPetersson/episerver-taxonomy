namespace Dodavinkeln.Taxonomy.Core.Routing
{
    using System.Web.Mvc;

    /// <summary>
    ///     Represents the factory for creating the <see cref="TaxonomyDataValueProvider"/> object.
    /// </summary>
    public class TaxonomyDataValueProviderFactory : ValueProviderFactory
    {
        /// <summary>
        ///     Returns the <see cref="TaxonomyDataValueProvider"/> object for the specified controller context.
        /// </summary>
        /// <param name="controllerContext">An object that encapsulates information about the current HTTP request.</param>
        /// <returns>Returns the <see cref="TaxonomyDataValueProvider"/></returns>
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new TaxonomyDataValueProvider(controllerContext);
        }
    }
}