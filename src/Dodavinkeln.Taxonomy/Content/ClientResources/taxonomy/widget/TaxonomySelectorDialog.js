define("taxonomy/widget/TaxonomySelectorDialog", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/Evented",

    "dijit/layout/_LayoutWidget",
    "dijit/form/Button",

    "epi/shell/TypeDescriptorManager",
    "epi",
    "epi/dependency",
    "epi-cms/widget/ContextualContentForestStoreModel",
    "epi-cms/widget/SearchBox",
    "taxonomy/widget/TaxonomySelectionTree"
],

function (
    array,
    declare,
    lang,
    Evented,

    _LayoutWidget,

    Button,

    TypeDescriptorManager,
    epi,
    dependency,
    ContextualContentForestStoreModel,
    SearchBox,
    TaxonomySelectionTree
) {
    return declare([_LayoutWidget, Evented], {
        allowedTypes: null,
        model: null,
        repositoryKey: null,
        restrictedTypes: null,
        showSearchBox: true,
        searchArea: 'cms/categories',
        settings: null,
        value: [],

        constructor: function () {
            this.inherited(arguments);
            this.value = [];
        },

        buildRendering: function () {
            this.inherited(arguments);

            var typeIdentifiers = this.allowedTypes,
                model = this.model,
                roots = this.roots || (model && model.roots);

            if (!model) {
                model = new ContextualContentForestStoreModel({
                    roots: roots,
                    typeIdentifiers: typeIdentifiers,
                    showAllLanguages: this.showAllLanguages
                });
            }

            if (this.showSearchBox && this.searchArea) {
                this._searchBox = new SearchBox({
                    innerSearchBoxClass: "epi-search--full-width",
                    triggerContextChange: false,
                    parameters: {
                        allowedTypes: this.allowedTypes,
                        restrictedTypes: this.restrictedTypes
                    },
                    onItemAction: lang.hitch(this, function (item) {
                        if (item && item.metadata && this._checkAcceptance(item.metadata.typeIdentifier)) {
                            this.appendValue(item.metadata.id);
                        }
                    })
                });
                this._searchBox.set("area", this.searchArea);
                this._searchBox.set("searchRoots", this.roots);

                var searchBox = this._searchBox;
                model.getRoots(true).then(function (roots) {
                    searchBox.set("searchRoots", roots.join(","));
                });

                this.addChild(this._searchBox);
            }

            this.tree = new TaxonomySelectionTree({
                roots: roots,
                repositoryKey: this.repositoryKey,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes,
                typeIdentifiers: typeIdentifiers,
                settings: this.settings
            });

            ////this.tree.on('onCreateTaxonomyCommandExecuted', lang.hitch(this, function (command) {
            ////    this.emit('onCreateTaxonomyCommandExecuted', command);
            ////}));

            ////this.tree.on('onNewTaxonomyCreated', lang.hitch(this, function (taxonomy) {
            ////    this.emit('onNewTaxonomyCreated', taxonomy);
            ////}));

            this.addChild(this.tree);
        },

        appendValue: function (value) {
            var contentLinks = this.get('value') || [];

            if (contentLinks.indexOf(value) === -1) {
                contentLinks.push(value);
                this.set('value', contentLinks);
            }
        },

        onShow: function () {
        },

        _checkAcceptance: function (typeIdentifier) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([typeIdentifier], this.allowedTypes, this.restrictedTypes);

            return acceptedTypes.length > 0;
        },

        _getValueAttr: function () {
            return this.tree.get('selectedContentLinks');
        },

        _setValueAttr: function (value) {
            this.tree.set('selectedContentLinks', value);
        }
    });
});