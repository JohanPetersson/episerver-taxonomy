namespace Dodavinkeln.Taxonomy.Core
{
    using System;
    using System.Web.Mvc;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;
    using Routing;

    /// <summary>
    ///     Responsible for initializing everything, e.g. setting up the root and
    ///     adding the <see cref="TaxonomyDataValueProvider"/>.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class TaxonomyInitializationModule : IInitializableModule
    {
        private bool isInitialized;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Initialize(InitializationEngine context)
        {
            if (this.isInitialized == false)
            {
                var contentRootService = ServiceLocator.Current.GetInstance<ContentRootService>();

                contentRootService.Register<TaxonomyRoot>(
                    TaxonomyRepositoryDescriptor.RepositoryKey,
                    new Guid("9dfb2190-d121-4242-bdcf-145acf986945"),
                    ContentReference.RootPage);

                ValueProviderFactories.Factories.Add(new TaxonomyDataValueProviderFactory());

                this.isInitialized = true;
            }
        }

        /// <summary>
        ///     Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Uninitialize(InitializationEngine context)
        {
            // Nothing to uninitialize
        }
    }
}