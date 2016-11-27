namespace Dodavinkeln.Taxonomy.UI
{
    using Core;
    using EPiServer.Shell;

    /// <summary>
    ///     Sets metadata for <see cref="TaxonomyRoot"/>.
    /// </summary>
    [UIDescriptorRegistration]
    public class TaxonomyRootUIDescriptor : UIDescriptor<TaxonomyRoot>, IEditorDropBehavior
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyRootUIDescriptor"/> class.
        /// </summary>
        public TaxonomyRootUIDescriptor()
            : base(UISettings.RootIcon)
        {
            this.AddDisabledView(UISettings.DisabledView);
            this.DefaultView = UISettings.DefaultView;
            this.EditorDropBehaviour = EditorDropBehavior.CreateLink;
            this.ContainerTypes = new[] { typeof(TaxonomyData) };
        }

        /// <summary>
        ///     Gets or sets the <see cref="EditorDropBehavior"/>.
        /// </summary>
        public EditorDropBehavior EditorDropBehaviour { get; set; }
    }
}