// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEditor;

    /// <summary>
    /// This wizard creates a new RuntimeSet type for representing a set containing objects of the the type you enter.
    /// </summary>
    public class GlobalGameObjectWithComponentCacheCreationWizard : GameObjectReferenceTypeCreationWizardBase
    {
        [MenuItem(SodaEditorHelpers.MENU_ITEM_ROOT + "Create/GlobalGameObjectWithComponentCache Type")]
        private static void Open()
        {
            OpenWizard<GlobalGameObjectWithComponentCacheCreationWizard>("Soda GlobalGameObjectWithComponentCache Creation Wizard");
        }

        protected override string typeFamilyName => "GlobalGameObject";
        protected override string newClassNamePrefix => "Global";
        protected override string templatePath { get { return singleComponent ? "GlobalGameObjectWithComponentCacheTemplateSingle.cs" : "GlobalGameObjectWithComponentCacheTemplateMultiple.cs"; } }
        protected override string successMessage { get { return "The GlobalGameObjectWithComponentCache class file has been created."; } }
    }
}
