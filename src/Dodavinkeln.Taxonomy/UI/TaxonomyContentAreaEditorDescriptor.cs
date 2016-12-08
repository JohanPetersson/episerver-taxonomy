namespace Dodavinkeln.Taxonomy.UI
{
    using Core;
    using EPiServer.Core;
    using EPiServer.Shell.ObjectEditing.EditorDescriptors;

    /// <summary>
    ///     The descriptor for <see cref="ContentArea"/> properties that uses the <see cref="UIHint.Taxonomy"/> UI hint.
    /// </summary>
    [EditorDescriptorRegistration(
        EditorDescriptorBehavior = EditorDescriptorBehavior.ExtendBase,
        TargetType = typeof(ContentArea),
        UIHint = UIHint.Taxonomy)]
    public class TaxonomyContentAreaEditorDescriptor : EditorDescriptor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyContentAreaEditorDescriptor"/> class.
        /// </summary>
        public TaxonomyContentAreaEditorDescriptor()
        {
            this.AllowedTypes = new[] { typeof(TaxonomyData) };
        }
    }
}