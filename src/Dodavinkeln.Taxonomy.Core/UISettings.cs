namespace Dodavinkeln.Taxonomy.Core
{
    /// <summary>
    ///     Settings for this library
    /// </summary>
    public static class UISettings
    {
        /// <summary>
        ///     The group name in admin mode for all content types.
        /// </summary>
        public const string GroupName = "taxonomy";

        /// <summary>
        ///     The default icon all content items should have in the UI.
        /// </summary>
        public const string DefaultIcon = "epi-iconObjectProduct";

        /// <summary>
        ///     The icon for the root.
        /// </summary>
        public const string RootIcon = "epi-iconObjectProductContextual";

        /// <summary>
        ///     The view that should be disabled for all content items.
        /// </summary>
        public const string DisabledView = "onpageedit";

        /// <summary>
        ///     The default view for all content items.
        /// </summary>
        public const string DefaultView = "formedit";

        /// <summary>
        ///     The widget that should be used by this library
        /// </summary>
        public const string CustomNavigationWidget = "taxonomy/component/TaxonomyNavigationTree";
    }
}