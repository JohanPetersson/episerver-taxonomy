namespace Dodavinkeln.Taxonomy.UI
{
    using System.Collections.Generic;
    using Core;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Shell.ObjectEditing;
    using EPiServer.Shell.ObjectEditing.EditorDescriptors;

    /// <summary>
    ///     The descriptor for <see cref="ContentReference"/> properties that uses the <see cref="UIHint.Taxonomy"/> UI hint.
    /// </summary>
    [EditorDescriptorRegistration(
        TargetType = typeof(ContentReference),
        UIHint = UIHint.Taxonomy)]
    public class TaxonomyContentReferenceEditorDescriptor : ContentReferenceEditorDescriptor<TaxonomyData>
    {
        private readonly ContentRootService contentRootService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyContentReferenceEditorDescriptor"/> class.
        /// </summary>
        /// <param name="contentRootService">The content root service to use.</param>
        public TaxonomyContentReferenceEditorDescriptor(ContentRootService contentRootService)
        {
            this.contentRootService = contentRootService;

            this.AllowedTypes = new[] { typeof(TaxonomyData) };
        }

        /// <summary>
        ///     Gets the repository key.
        /// </summary>
        public override string RepositoryKey => TaxonomyRepositoryDescriptor.RepositoryKey;

        /// <summary>
        ///     Gets the roots.
        /// </summary>
        public override IEnumerable<ContentReference> Roots => new[] { this.contentRootService.Get(TaxonomyRepositoryDescriptor.RepositoryKey) };

        /// <summary>
        ///     Overridden to set editor configuration.
        /// </summary>
        /// <param name="metadata">The meta data.</param>
        protected override void SetEditorConfiguration(ExtendedMetadata metadata)
        {
            base.SetEditorConfiguration(metadata);

            // Needs to be set explicitly since we have a custom root.
            this.EditorConfiguration["showSearchBox"] = true;
            this.EditorConfiguration["searchArea"] = UISettings.SearchArea;
        }
    }
}