define("taxonomy/widget/TaxonomySelectionTree", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/connect",
    "dojo/dom-class",
    "dojo/when",
    "dojo/promise/all",
    "dojo/Deferred",
    "dojo/Evented",

    "epi/shell/ClipboardManager",
    "epi/shell/selection",
    "epi-cms/widget/ContentTree",
    "epi-cms/widget/ContentForestStoreModel",
    "epi/shell/TypeDescriptorManager",
    "epi/epi",

    "taxonomy/widget/TaxonomySelectionTreeNode"
],

function (
    array,
    declare,
    lang,
    connect,
    domClass,
    when,
    promiseAll,
    Deferred,
    Evented,

    ClipboardManager,
    Selection,
    ContentTree,
    ContentForestStoreModel,
    TypeDescriptorManager,
    epi,

    TaxonomySelectionTreeNode
) {
    return declare([ContentTree, Evented], {
        _clipboardManager: null,
        _contextMenu: null,
        _contextMenuCommandProvider: null,
        _focusNode: null,

        nodeConstructor: TaxonomySelectionTreeNode,
        selectedContentLinks: null,
        selection: null,
        showRoot: false,

        constructor: function () {
            this.selectedContentLinks = [];
        },

        postMixInProperties: function () {
            this.inherited(arguments);

            this._clipboardManager = new ClipboardManager();
            this.selection = new Selection();

            this.model.roots = this.roots;
            this.model.notAllowToDelete = this.settings.preventDeletionFor;
            this.model.notAllowToCopy = this.settings.preventCopyingFor;
        },

        expandSelectedNodes: function () {
            when(this.onLoadDeferred, lang.hitch(this, function () {
                array.forEach(this.selectedContentLinks, function (contentLink) {
                    this.selectNodeById(contentLink, false);
                }, this);
            }));
        },

        getNodeById: function (contentLink) {
            var nodes = this.getNodesByItem(contentLink);

            if (!nodes) {
                return null;
            }

            return nodes[0];
        },

        selectNodeById: function (contentLink, highlight) {
            this.selectContent(contentLink, true).then(function (node) {
                if (node.item.contentLink === contentLink) {
                    node.setSelected(highlight);
                    node.set('checked', true);
                }
            });
        },

        _createTreeModel: function () {
            return new ContentForestStoreModel({
                roots: this.roots,
                typeIdentifiers: this.typeIdentifiers
            });
        },

        _createTreeNode: function () {
            var node = this.inherited(arguments);

            if (this._isItemSelectable(node.item)) {
                node.connect(node, "onNodeSelectChanged", lang.hitch(this, function (checked, item) {
                    this._onNodeSelectChanged(checked, item);
                }));

                var selectedContentLinks = this.get('selectedContentLinks');

                if (selectedContentLinks && selectedContentLinks.indexOf(node.item.contentLink) !== -1) {
                    node.set('checked', true);
                }
            }

            return node;
        },

        _expandExtraNodes: function () {
            this.expandSelectedNodes();
            return this.inherited(arguments);
        },

        _getSelectionData: function (itemData) {
            return itemData ? [{ type: "epi.cms.contentdata", data: itemData }] : [];
        },

        _isItemSelectable: function (item) {
            var acceptedTypes = TypeDescriptorManager.getValidAcceptedTypes([item.typeIdentifier], this.typeIdentifiers, this.restrictedTypes);

            return acceptedTypes.length > 0;
        },

        _onNodeSelectChanged: function (checked, item) {
            if (!this.getNodeById(item.contentLink)) {
                return;
            }

            if (this.selectedContentLinks == null) {
                this.selectedContentLinks = [];
            }

            var index = this.selectedContentLinks.indexOf(item.contentLink),
                exist = index !== -1;

            if (checked && !exist) {
                this.selectedContentLinks.push(item.contentLink);
            }

            if (!checked && exist) {
                this.selectedContentLinks.splice(index, 1);
            }
        },

        _removeHighlightClass: function () {
            if (this._focusNode && this._focusNode !== this.selectedNode && this._focusNode.rowNode) {
                domClass.remove(this._focusNode.rowNode, "dijitTreeRowSelected");
            }
        },

        _set_focusNodeAttr: function (value) {
            this._removeHighlightClass();
            this._focusNode = value;
        },

        _setSelectedContentLinksAttr: function (value) {
            if (!lang.isArray(value)) {
                value = [value];
            }

            var filteredValue = array.filter(value, function (v) {
                return !!v;
            });

            this._set('selectedContentLinks', filteredValue);
        },
    });
});