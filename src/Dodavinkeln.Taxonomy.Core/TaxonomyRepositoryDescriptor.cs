namespace Dodavinkeln.Taxonomy.Core
{
    using System;
    using System.Collections.Generic;
    using EPiServer.Cms.Shell.UI.CompositeViews.Internal;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Framework.Localization;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell;

    /// <summary>
    ///     The taxnonomy repository descriptor.
    /// </summary>
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class TaxonomyRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        private readonly ContentRootService contentRootService;

        private readonly ContentReference root;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TaxonomyRepositoryDescriptor"/> class.
        /// </summary>
        /// <param name="contentRootService">The content root service to use.</param>
        public TaxonomyRepositoryDescriptor(ContentRootService contentRootService)
        {
            this.contentRootService = contentRootService;

            this.root = this.contentRootService.Get(this.Key);
        }

        /// <summary>
        ///     Gets the key for the repository.
        /// </summary>
        public static string RepositoryKey => "taxonomy";

        /// <summary>
        ///     Gets the key for the repository.
        /// </summary>
        public override string Key => RepositoryKey;

        /// <summary>
        ///     Gets the name of the repository.
        /// </summary>
        public override string Name => LocalizationService.Current.GetString("/contentrepositories/taxonomy/name");

        /// <summary>
        ///     Gets the roots for the repository.
        /// </summary>
        public override IEnumerable<ContentReference> Roots => new[] { this.root };

        /// <summary>
        ///     Gets the contained types for the repository.
        /// </summary>
        public override IEnumerable<Type> ContainedTypes => new[] { typeof(TaxonomyData) };

        /// <summary>
        ///     Gets the creatable types for the repository.
        /// </summary>
        public override IEnumerable<Type> CreatableTypes => new[] { typeof(TaxonomyData) };

        /// <summary>
        ///     Gets the linkable types for the repository.
        /// </summary>
        public override IEnumerable<Type> LinkableTypes => new[] { typeof(TaxonomyData) };

        /// <summary>
        ///     Gets the items that shouldn't have "For this content" in the repository.
        /// </summary>
        public override IEnumerable<string> PreventContextualContentFor => null;

        /// <summary>
        ///     Gets the items that shouldn't be able to copy in the repository.
        /// </summary>
        public override IEnumerable<string> PreventCopyingFor => new[] { this.root.ToString() };

        /// <summary>
        ///     Gets the items that shouldn't be able to delete in the repository.
        /// </summary>
        public override IEnumerable<string> PreventDeletionFor => new[] { this.root.ToString() };

        /// <summary>
        ///     Gets the main view in the repository.
        /// </summary>
        public override IEnumerable<string> MainViews => new[] { HomeView.ViewName };

        /// <summary>
        ///     Gets the navigation widget.
        /// </summary>
        public override string CustomNavigationWidget => UISettings.CustomNavigationWidget;
    }
}