namespace Dodavinkeln.Taxonomy.UI
{
    using Core;
    using EPiServer.Shell;
    using EPiServer.Shell.ViewComposition;

    /// <summary>
    ///     The component that registers the UI.
    /// </summary>
    [Component]
    public class TaxonomyComponent : ComponentDefinitionBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyComponent"/> class.
        /// </summary>
        public TaxonomyComponent()
            : base("epi-cms/component/MainNavigationComponent")
        {
            this.Categories = new[] { "content" };
            this.LanguagePath = "/components/taxonomy";
            this.PlugInAreas = new[] { PlugInArea.AssetsDefaultGroup };
            this.SortOrder = 102;

            this.Settings.Add(new Setting("repositoryKey", TaxonomyRepositoryDescriptor.RepositoryKey));

            this.AllowedRoles.Add("CmsEditors");
            this.AllowedRoles.Add("CmsAdmins");
        }
    }
}