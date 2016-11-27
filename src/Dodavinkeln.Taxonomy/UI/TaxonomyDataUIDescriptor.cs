namespace Dodavinkeln.Taxonomy.UI
{
    using Core;
    using EPiServer.Shell;

    /// <summary>
    ///     Sets metadata for <see cref="TaxonomyData"/>.
    /// </summary>
    [UIDescriptorRegistration]
    public class TaxonomyDataUIDescriptor : UIDescriptor<TaxonomyData>, IEditorDropBehavior
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyDataUIDescriptor"/> class.
        /// </summary>
        public TaxonomyDataUIDescriptor()
            : base(UISettings.DefaultIcon)
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