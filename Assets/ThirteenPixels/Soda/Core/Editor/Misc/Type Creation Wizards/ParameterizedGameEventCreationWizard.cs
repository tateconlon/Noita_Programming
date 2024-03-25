// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This wizard creates a new parameterized GameEvent type that represents an event with a parameter of the specified type.
    /// </summary>
    public class ParameterizedGameEventCreationWizard : TypeCreationWizardBase
    {
        [MenuItem(SodaEditorHelpers.MENU_ITEM_ROOT + "Create/Parameterized GameEvent Type")]
        private static void Open()
        {
            OpenWizard<ParameterizedGameEventCreationWizard>("Soda GameEvent Type Creation Wizard");
        }

        protected override string templatePath { get { return "ParameterizedGameEventTemplate.cs"; } }
        protected override string successMessage { get { return "The GameEvent class file has been created."; } }

        protected override void DisplayTextAndOptions()
        {
            GUILayout.Label("Please enter the parameter type for the GameEvent type.\n"
                + "Remember to be case-sensitive.");

            GUI.FocusControl("TextField");
        }

        protected override bool IsValidInput(string input)
        {
            return Regex.IsMatch(input, "\\w+");
        }

        protected override string GenerateFilename(string typeName)
        {
            return "GameEvent" + typeName + ".cs";
        }

        protected override string ParseTemplate(string content, string input)
        {
            content = content.Replace("#TYPE#", input);
            return content;
        }
    }
}
