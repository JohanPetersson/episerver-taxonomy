define("taxonomy/widget/TaxonomySelector", [
    "dojo/_base/array",
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/promise/all",
    "dojo/dom-attr",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/query",
    "dojo/Deferred",

    "dijit/_TemplatedMixin",
    "dijit/_Widget",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/Button",
    "dijit/Tooltip",

    "taxonomy/widget/TaxonomySelectorDialog",
    "epi-cms/widget/_HasChildDialogMixin",
    "epi/shell/widget/_ValueRequiredMixin",
    "epi/dependency",
    "epi/epi",
    "epi/shell/widget/dialog/Dialog",

    "dojo/text!./template/TaxonomySelector.html",
    "epi/i18n!epi/cms/nls/episerver.cms.widget.contentselector"
],

function (
    array,
    declare,
    lang,
    promiseAll,
    domAttr,
    domClass,
    domConstruct,
    domStyle,
    query,
    Deferred,

    _TemplatedMixin,
    _Widget,
    _WidgetsInTemplateMixin,
    Button,
    Tooltip,

    TaxonomySelectorDialog,
    _HasChildDialogMixin,
    _ValueRequiredMixin,
    dependency,
    epi,
    Dialog,

    template,
    res
) {
    return declare([_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _HasChildDialogMixin, _ValueRequiredMixin], {
        _values: null,
        _preventSetDialogValueOnShow: false,
        _updateDisplayPromise: null,

        allowedTypes: null,
        localization: res,
        repositoryKey: null,
        restrictedTypes: null,
        roots: null,
        searchArea: 'CMS/categories',
        settings: null,
        showSearchBox: true,
        store: null,
        templateString: template,
        value: null,

        onChange: function (value) {
        },

        postCreate: function () {
            if (!this.store) {
                var registry = dependency.resolve('epi.storeregistry');
                this.store = registry.get('epi.cms.content.light');
            }
        },

        destroy: function () {
            this.inherited(arguments);

            if (this._updateDisplayPromise) {
                this._updateDisplayPromise.cancel();
                this._updateDisplayPromise = null;
            }
        },

        destroyDescendants: function () {
            if (this._tooltip) {
                this._tooltip.destroy();
                delete this._tooltip;
            }

            this.inherited(arguments);
        },

        ////onCreateTaxonomyCommandExecuted: function () {
        ////    this.dialog.hide();
        ////    this._preventSetDialogValueOnShow = true;
        ////},

        ////onNewTaxonomyCreated: function (item) {
        ////    this.dialog.show(true);
        ////    this.taxonomySelectorDialog.appendValue(item.contentLink);
        ////    this._preventSetDialogValueOnShow = false;
        ////},

        _setValueAttr: function (value) {
            if (!lang.isArray(value)) {
                value = [value];
            }

            var filteredValue = array.filter(value, function (v) {
                return !!v;
            });

            this._setValueAndFireOnChange(filteredValue);

            if (filteredValue.length > 0) {
                this._set("value", filteredValue);
            } else {
                this._set("value", null);
            }

            this._started && this.validate();
        },

        _setValueAndFireOnChange: function (value) {
            // Compare arrays
            if (epi.areEqual(this._values, value)) {
                return;
            }

            this._values = value;
            this.onChange(this._values);
            this._updateDisplayNode();
        },

        _getValueAttr: function () {
            return this._values;
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);

            domStyle.set(this.button.domNode, "display", value ? "none" : "");
            domClass.toggle(this.domNode, "dijitReadOnly", value);
        },

        focus: function () {
            this.button.focus();
        },

        _createItemButton: function (item) {
            //
            // TODO: create a widget for item with a template instead of creating dom nodes
            //

            // Don't add a button if it's already added.
            if (query("div[data-epi-item-id=" + item.contentLink + "]", this.itemsGroupContainer).length !== 0) {
                return;
            }

            var containerDiv = domConstruct.create("div", { "class": "dijitReset dijitLeft dijitInputField dijitInputContainer epi-categoryButton" });
            var buttonWrapperDiv = domConstruct.create("div", { "class": "dijitInline epi-resourceName" });
            var itemNameDiv = domConstruct.create("div", { "class": "dojoxEllipsis", innerHTML: item.name });
            domConstruct.place(itemNameDiv, buttonWrapperDiv);

            domConstruct.place(buttonWrapperDiv, containerDiv);

            // create tooltip for the div
            this._tooltip = new Tooltip({
                connectId: itemNameDiv,
                label: item.name
            });

            var removeButtonDiv = domConstruct.create("div", { "class": "epi-removeButton", innerHTML: "&nbsp;" });
            domAttr.set(removeButtonDiv, "data-epi-item-id", item.contentLink);
            var eventName = removeButtonDiv.onClick ? "onClick" : "onclick";

            if (!this.readOnly) {
                this.connect(removeButtonDiv, eventName, lang.hitch(this, this._onRemoveClick));
                domConstruct.place(removeButtonDiv, buttonWrapperDiv);
            } else {
                domConstruct.place(domConstruct.create("span", { innerHTML: "&nbsp;" }), buttonWrapperDiv);
            }

            domConstruct.place(containerDiv, this.itemsGroupContainer);
        },

        _createNoItemsChosenSpan: function () {
            domConstruct.create("div", {
                innerHTML: this.localization.nocategorieschosen,
                "class": "epi-categoriesGroup__message"
            }, this.itemsGroupContainer, "only");
        },

        _updateDisplayNode: function () {
            this.itemsGroupContainer.innerHTML = "";

            if (!this._values || this._values.length === 0) {
                this._createNoTaxonomyChosenSpan();
                return;
            }

            var dfdList = [];

            this._values.forEach(function (itemId) {
                dfdList.push(this.store.get(itemId));
            }, this);

            this._updateDisplayPromise = promiseAll(dfdList).then(lang.hitch(this, function (values) {
                // Clear the pointer to the promise since it is resolved.
                this._updateDisplayPromise = null;

                values.forEach(function (item) {
                    if (item) {
                        this._createItemButton(item);
                    } else {
                        var itemIndex = this._values.indexOf(item.contentLink);

                        if (itemIndex !== -1) {
                            this._values.splice(itemIndex, 1);
                        }
                    }

                    // Create no items chosen span if haven't any items in list.
                    if (this._values.length === 0) {
                        this._createNoItemsChosenSpan();
                    }
                }, this);
            }));
        },

        _onRemoveClick: function (arg) {
            var itemId = domAttr.get(arg.target, "data-epi-item-id");
            var itemIndex = this._categories.indexOf(itemId);

            if (itemIndex === -1) {
                return;
            }

            var remainingItems = lang.clone(this._values);
            remainingItems.splice(itemIndex, 1);
            this.set("value", remainingItems);
        },

        _onShow: function () {
            if (!this._preventSetDialogValueOnShow) {
                this.taxonomySelectorDialog.set("value", lang.clone(this._values));
            }

            this.isShowingChildDialog = true;
            this.taxonomySelectorDialog.onShow();
        },

        _onExecute: function () {
            var itemsSelected = this.taxonomySelectorDialog.get("value");
            this.set("value", itemsSelected);
        },

        _onDialogHide: function () {
            this.focus();
            this.isShowingChildDialog = false;
        },

        _createDialog: function () {
            this.taxonomySelectorDialog = new TaxonomySelectorDialog({
                repositoryKey: this.repositoryKey,
                roots: this.roots,
                allowedTypes: this.allowedTypes,
                restrictedTypes: this.restrictedTypes,
                showSearchBox: this.showSearchBox,
                searchArea: this.searchArea,
                settings: this.settings
            });

            ////this.taxonomySelectorDialog.on('onCreateTaxonomyCommandExecuted', lang.hitch(this, this.onCreateTaxonomyCommandExecuted));
            ////this.taxonomySelectorDialog.on('onNewTaxonomyCreated', lang.hitch(this, this.onNewTaxonomyCreated));

            this.dialog = new Dialog({
                title: this.localization.title,
                content: this.taxonomySelectorDialog,
                destroyOnHide: false,
                dialogClass: "epi-dialog-portrait"
            });

            this.own(this.dialog);
            this.connect(this.dialog, "onExecute", "_onExecute");
            this.connect(this.dialog, "onShow", "_onShow");
            this.connect(this.dialog, "onHide", "_onDialogHide");

            this.dialog.startup();
        },

        _onButtonClick: function () {
            if (!this.dialog) {
                this._createDialog();
            }

            this.dialog.show(true);
        }
    });
});