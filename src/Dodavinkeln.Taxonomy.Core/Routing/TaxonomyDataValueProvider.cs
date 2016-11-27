namespace Dodavinkeln.Taxonomy.Core.Routing
{
    using System;
    using System.Globalization;
    using System.Web.Mvc;
    using EPiServer.Web.Routing;

    /// <summary>
    ///     Responsible for returning the value for a routed <see cref="TaxonomyData"/> item.
    /// </summary>
    public class TaxonomyDataValueProvider : IValueProvider
    {
        /// <summary>
        ///     The prefix for the data.
        /// </summary>
        public const string Prefix = "currentTaxonomyData";

        private readonly ControllerContext controllerContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyDataValueProvider"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        public TaxonomyDataValueProvider(ControllerContext controllerContext)
        {
            this.controllerContext = controllerContext;
        }

        /// <summary>
        ///     Determines whether the collection contains the <see cref="Prefix"/> prefix.
        /// </summary>
        /// <param name="prefix">The prefix to search for.</param>
        /// <returns>True if the collection contains the specified prefix; otherwise, false.</returns>
        public bool ContainsPrefix(string prefix)
        {
            return prefix.Equals(TaxonomyDataValueProvider.Prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Retrieves a value object using the specified key.
        /// </summary>
        /// <param name="key">The key of the value object to retrieve.</param>
        /// <returns>The value object for the specified key.</returns>
        public ValueProviderResult GetValue(string key)
        {
            if (this.ContainsPrefix(key) == false)
            {
                return null;
            }

            var taxonomyData = this.controllerContext.RequestContext.GetCustomRouteData<TaxonomyData>(TaxonomyDataValueProvider.Prefix);

            if (taxonomyData == null)
            {
                return null;
            }

            return new ValueProviderResult(taxonomyData, taxonomyData.ContentLink.ToString(), CultureInfo.InvariantCulture);
        }
    }
}