// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEditor;

    /// <summary>
    /// This wizard creates a new RuntimeSet type for representing a set containing objects of the the type you enter.
    /// </summary>
    public class RuntimeSetCreationWizard : GameObjectReferenceTypeCreationWizardBase
    {
        [MenuItem(SodaEditorHelpers.MENU_ITEM_ROOT + "Create/RuntimeSet Type")]
        private static void Open()
        {
            OpenWizard<RuntimeSetCreationWizard>("Soda RuntimeSet Creation Wizard");
        }

        protected override string typeFamilyName => "RuntimeSet";
        protected override string newClassNamePrefix => "RuntimeSet";
        protected override string templatePath { get { return singleComponent ? "RuntimeSetTemplateSingle.cs" : "RuntimeSetTemplateMultiple.cs"; } }
        protected override string successMessage { get { return "The RuntimeSet class file has been created."; } }
    }
}
