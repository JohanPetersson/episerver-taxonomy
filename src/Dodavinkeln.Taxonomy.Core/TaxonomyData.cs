namespace Dodavinkeln.Taxonomy.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using EPiServer.Core;
    using EPiServer.Core.Internal;
    using EPiServer.Data.Entity;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Security;
    using EPiServer.Web.Routing;

    /// <summary>
    ///
    /// </summary>
    [AdministrationSettings(
        GroupName = UISettings.GroupName,
        Order = 10,
        ContentTypeFields = ContentTypeFields.ACL | ContentTypeFields.AvailableInEditMode | ContentTypeFields.AvailablePageTypes | ContentTypeFields.DefaultValues | ContentTypeFields.SortOrder)]
    [AvailableContentTypes(
        Availability = Availability.Specific,
        Include = new[]
        {
            typeof(TaxonomyData)
        })]
    public abstract class TaxonomyData : BasicContent, IVersionable, IReadOnly<TaxonomyData>, IReadOnly, IModifiedTrackable, IContentSecurable, ISecurable, IRoutable, ILocalizable, ILocale
    {
        #region Fields

        private LanguageData languageData = new LanguageData();

        private ContentAccessControlList contentAccessControlList = new ContentAccessControlList();

        private bool isModified;

        private string routeSegment;

        private bool isPendingPublished;

        private DateTime? startPublish;

        private DateTime? stoptPublish;

        private VersionStatus status;

        #endregion

        [UIHint(UIHint.PreviewableText)]
        public virtual string RouteSegment
        {
            get
            {
                return this.routeSegment;
            }

            set
            {
                this.ThrowIfReadOnly();
                this.isModified = true;
                this.routeSegment = value;
            }
        }

        public virtual bool IsPendingPublish
        {
            get
            {
                return this.isPendingPublished;
            }

            set
            {
                this.ThrowIfReadOnly();
                this.isModified = true;
                this.isPendingPublished = value;
            }
        }

        public virtual DateTime? StartPublish
        {
            get
            {
                return this.startPublish;
            }

            set
            {
                this.ThrowIfReadOnly();
                this.isModified = true;
                this.startPublish = value;
            }
        }

        public virtual DateTime? StopPublish
        {
            get
            {
                return this.stoptPublish;
            }

            set
            {
                this.ThrowIfReadOnly();
                this.isModified = true;
                this.stoptPublish = value;
            }
        }

        public virtual VersionStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.ThrowIfReadOnly();
                this.isModified = true;
                this.status = value;
            }
        }

        public IEnumerable<CultureInfo> ExistingLanguages
        {
            get
            {
                return this.languageData.ExistingLanguages;
            }

            set
            {
                this.languageData.ExistingLanguages = value;
            }
        }

        public CultureInfo MasterLanguage
        {
            get
            {
                return this.languageData.MasterLanguage;
            }

            set
            {
                this.languageData.MasterLanguage = value;
            }
        }

        public CultureInfo Language
        {
            get
            {
                return this.languageData.Language;
            }

            set
            {
                this.languageData.Language = value;
            }
        }

        public IContentSecurityDescriptor GetContentSecurityDescriptor()
        {
            return this.contentAccessControlList;
        }

        public ISecurityDescriptor GetSecurityDescriptor()
        {
            return this.contentAccessControlList;
        }

        public TaxonomyData CreateWritableClone()
        {
            return this.CreateWriteableCloneImplementation() as TaxonomyData;
        }

        public override void MakeReadOnly()
        {
            base.MakeReadOnly();

            this.contentAccessControlList.MakeReadOnly();
            this.languageData.MakeReadOnly();
        }

        protected override bool IsModified
        {
            get
            {
                return base.IsModified ||
                       this.isModified ||
                       this.contentAccessControlList.IsModified ||
                       this.languageData.IsModified;
            }
        }

        protected override void ResetModified()
        {
            base.ResetModified();

            this.ThrowIfReadOnly();

            this.isModified = false;

            this.contentAccessControlList.ResetModified();
            this.languageData.ResetModified();
        }

        protected override object CreateWriteableCloneImplementation()
        {
            var clone = base.CreateWriteableCloneImplementation() as TaxonomyData;

            clone.contentAccessControlList = this.contentAccessControlList.CreateWritableClone();

            clone.languageData = this.languageData.CreateWritableClone();

            return clone;
        }
    }
}