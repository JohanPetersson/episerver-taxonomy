define("taxonomy/component/TaxonomyNavigationTree", [
    "dojo/_base/array",
    "dojo/_base/declare",

	"epi/shell/TypeDescriptorManager",

    "epi-cms/component/PageNavigationTree"
],

function (
    array,
    declare,

	TypeDescriptorManager,

    PageNavigationTree
) {
	return declare([PageNavigationTree], {
		getIconClass: function (/*dojo/data/Item*/item, /*Boolean*/opened) {
			// summary:
			//      Overridable function to return CSS class name to display icon,
			// item:
			//      The current contentdata.
			// opened:
			//      Indicate the node is expanded or not.
			// tags:
			//      extension

			var isRoot = array.some(this.model.roots, function (root) {
				return root === this.model.getIdentity(item);
			}, this);

			if (isRoot) {
				var iconNodeClass = TypeDescriptorManager.getValue(item.typeIdentifier, "iconClass");

				return iconNodeClass;
			} else {
				return this.inherited(arguments);
			}
		}
	});
});