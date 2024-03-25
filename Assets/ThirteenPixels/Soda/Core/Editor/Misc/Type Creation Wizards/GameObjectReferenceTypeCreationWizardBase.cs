// Copyright © Sascha Graeff/13Pixels.

namespace ThirteenPixels.Soda.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base class for type creation wizards that create types that references GameObjects and one or more of their components.
    /// Namely RuntimeSets and GlobalGameObjectWithComponentCache.
    /// </summary>
    public abstract class GameObjectReferenceTypeCreationWizardBase : TypeCreationWizardBase
    {
        protected bool singleComponent = true;
        private string className = "";

        protected abstract string typeFamilyName { get; } // RuntimeSet / GlobalVariable
        protected abstract string newClassNamePrefix { get; } // RuntimeSet / Global

        protected sealed override void DisplayTextAndOptions()
        {
            GUILayout.Label($"Please select whether your new {typeFamilyName} should store a single component\n"
                + "or multiple components of a given GameObject.");

            GUILayout.BeginHorizontal();
            singleComponent = GUILayout.Toggle(singleComponent, "Single component");
            singleComponent = !GUILayout.Toggle(!singleComponent, "Multiple components");
            GUILayout.EndHorizontal();

            if (singleComponent)
            {
                GUILayout.Label("Please enter the name of the component\n"
                    + $"that the {typeFamilyName} is supposed to reference for a given GameObject.\n"
                    + "Remember to be case-sensitive.");

                className = "";
            }
            else
            {
                GUILayout.Label($"Next, please complete the class name for your new {typeFamilyName} class.\n"
                    + "Examples: \"MeshRendererAndFilter\" or \"VisibleObject\"");

                className = EditorGUILayout.TextField($"Class Name: {newClassNamePrefix}", className);

                GUILayout.Label($"Now, please enter all the component types that are to be referenced by your new {typeFamilyName}.\n"
                    + "Remember to be case-sensitive. Seperate type names with spaces.\n"
                    + "Example: MeshRenderer MeshFilter");
            }
        }

        protected sealed override bool IsValidInput(string input)
        {
            if (singleComponent)
            {
                return Regex.IsMatch(input, @"\w+");
            }
            else
            {
                return Regex.IsMatch(input, @"(\w+)( \w+)+") &&
                       Regex.IsMatch(className, @"\w+");
            }
        }

        protected sealed override string GenerateFilename(string typeName)
        {
            return newClassNamePrefix + (singleComponent ? typeName : className) + ".cs";
        }

        protected sealed override string ParseTemplate(string content, string input)
        {
            if (singleComponent)
            {
                content = ParseTemplateForSingleComponent(content, input);
            }
            else
            {
                var inputParts = input.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (inputParts.Length <= 1)
                {
                    throw new System.Exception("This should not happen.");
                }
                else
                {
                    content = ParseTemplateForMultiComponent(content, className, inputParts);
                }
            }

            return content;
        }

        private string ParseTemplateForSingleComponent(string content, string typeName)
        {
            content = ReplaceDependingOnMode(content, true);
            content = content.Replace("#TYPE#", typeName);
            return content;
        }

        private string ParseTemplateForMultiComponent(string content, string className, string[] typeNames)
        {
            content = ReplaceDependingOnMode(content, false);
            content = content.Replace("#TYPE#", className);

            string[] fieldNames = GenerateFieldNamesForTypes(typeNames);

            var allFieldNamesConcatenatedWithAnd = "";
            for (var i = 0; i < fieldNames.Length; i++)
            {
                var firstItem = i == 0;
                if (!firstItem)
                {
                    allFieldNamesConcatenatedWithAnd += " && ";
                }
                allFieldNamesConcatenatedWithAnd += fieldNames[i];
            }

            content = Regex.Replace(content, @"#ALL_CNAMES#", allFieldNamesConcatenatedWithAnd);

            // Replace all #PER_COMPONENT lines with a block of one line per passed class name.
            // In each line, replace occurrences of #CTYPE# with the current type
            // and all occurrences of #CNAME# with a variable name generated from that type.
            content = Regex.Replace(content, @"( *?#PER_COMPONENT .+?)\r?\n", match =>
            {
                var templateLine = match.Value;
                templateLine = templateLine.Replace("#PER_COMPONENT ", "");

                var result = "";
                for (var i = 0; i < typeNames.Length; i++)
                {
                    var line = templateLine;

                    line = line.Replace("#CTYPE#", typeNames[i]);
                    line = line.Replace("#CNAME#", fieldNames[i]);

                    result += line;
                }
                return result;
            });

            return content;
        }

        private static string ReplaceDependingOnMode(string content, bool singleComponent)
        {
            content = Regex.Replace(content, @"#SINGLE_COMPONENT\?(.*?)#:#(.*?)\?#", match =>
            {
                return match.Groups[singleComponent ? 1 : 2].Value.Trim(' ');
            },
            RegexOptions.Singleline);
            return content;
        }

        private static string[] GenerateFieldNamesForTypes(string[] typeNames)
        {
            var fieldNames = new string[typeNames.Length];
            for (var i = 0; i < typeNames.Length; i++)
            {
                var typeName = typeNames[i];
                var fieldName = typeName.Substring(0, 1).ToLower() + typeName.Substring(1);
                // Just in case someone thinks class names should be lowercase...
                if (fieldName == typeName)
                {
                    fieldName = "_" + fieldName;
                }
                fieldNames[i] = fieldName;
            }

            return fieldNames;
        }
    }
}
