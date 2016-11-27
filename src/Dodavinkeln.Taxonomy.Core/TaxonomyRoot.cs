namespace Dodavinkeln.Taxonomy.Core
{
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Framework.Localization;

    [AdministrationSettings(
        GroupName = UISettings.GroupName,
        Order = 10,
        ContentTypeFields = ContentTypeFields.ACL | ContentTypeFields.AvailablePageTypes | ContentTypeFields.DefaultValues | ContentTypeFields.SortOrder,
        PropertyDefinitionFields = PropertyDefinitionFields.None)]
    [ContentType(
        GUID = "75181665-16a6-4250-909e-00e6975a88b7",
        AvailableInEditMode = false)]
    public class TaxonomyRoot : TaxonomyData
    {
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            this.Name = LocalizationService.Current.GetString("/contentrepositories/taxonomy/name");
        }
    }
}