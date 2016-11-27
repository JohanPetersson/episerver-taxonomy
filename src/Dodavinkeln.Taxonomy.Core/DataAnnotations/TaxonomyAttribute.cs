namespace Dodavinkeln.Taxonomy.Core.DataAnnotations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EPiServer.Core;
    using EPiServer.Framework.DataAnnotations;

    /// <summary>
    ///     Attribute to use on <see cref="ContentReference"/> properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TaxonomyAttribute : UIHintAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyAttribute"/> class.
        /// </summary>
        public TaxonomyAttribute()
            : base(Core.UIHint.Taxonomy)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyAttribute"/> class.
        /// </summary>
        /// <param name="presentationLayer">The presentation layer that uses the class. See <see cref="PresentationLayer"/> for examples.</param>
        public TaxonomyAttribute(string presentationLayer)
            : base(Core.UIHint.Taxonomy, presentationLayer)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyAttribute"/> class.
        /// </summary>
        /// <param name="presentationLayer">The presentation layer that uses the class. See <see cref="PresentationLayer"/> for examples.</param>
        /// <param name="controlParameters">The object to use to retrieve values from any data sources.</param>
        public TaxonomyAttribute(string presentationLayer, params object[] controlParameters)
            : base(Core.UIHint.Taxonomy, presentationLayer, controlParameters)
        {
        }
    }
}