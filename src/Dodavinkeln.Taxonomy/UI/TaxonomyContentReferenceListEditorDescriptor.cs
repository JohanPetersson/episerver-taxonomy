namespace Dodavinkeln.Taxonomy.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dodavinkeln.Taxonomy.Core;
    using EPiServer.Core;
    using EPiServer.Shell;
    using EPiServer.Shell.ObjectEditing;
    using EPiServer.Shell.ObjectEditing.EditorDescriptors;

    /// <summary>
    ///     The descriptor for <see cref="IList{ContentReference}"/> properties that uses the <see cref="UIHint.Taxonomy"/> UI hint.
    /// </summary>
    [EditorDescriptorRegistration(
        EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault,
        TargetType = typeof(IList<ContentReference>),
        UIHint = UIHint.Taxonomy)]
    public class TaxonomyContentReferenceListEditorDescriptor : EditorDescriptor
    {
        private readonly IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyContentReferenceListEditorDescriptor"/> class.
        /// </summary>
        /// <param name="contentRepositoryDescriptors">The content repository descriptors to use.</param>
        public TaxonomyContentReferenceListEditorDescriptor(IEnumerable<IContentRepositoryDescriptor> contentRepositoryDescriptors)
        {
            this.contentRepositoryDescriptors = contentRepositoryDescriptors;

            this.AllowedTypes = new[] { typeof(TaxonomyData) };
            this.ClientEditingClass = "taxonomy/widget/TaxonomySelector";
        }

        /// <summary>
        ///     Overrides the default meta data.
        /// </summary>
        /// <param name="metadata">The meta data.</param>
        /// <param name="attributes">The attributes.</param>
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            var taxonomyRepositoryDescriptor = this.contentRepositoryDescriptors
                .First(x => x.Key == TaxonomyRepositoryDescriptor.RepositoryKey);

            metadata.OverlayConfiguration["AllowedDndTypes"] = this.AllowedTypes;
            metadata.EditorConfiguration["AllowedTypes"] = this.AllowedTypes;
            metadata.EditorConfiguration["AllowedDndTypes"] = this.AllowedTypes;
            metadata.EditorConfiguration["repositoryKey"] = TaxonomyRepositoryDescriptor.RepositoryKey;
            metadata.EditorConfiguration["settings"] = taxonomyRepositoryDescriptor;
            metadata.EditorConfiguration["roots"] = taxonomyRepositoryDescriptor.Roots;
            metadata.EditorConfiguration["showSearchBox"] = true;
        }
    }
}