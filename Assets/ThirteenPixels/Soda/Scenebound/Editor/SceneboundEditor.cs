
namespace ThirteenPixels.Soda.Scenebound.Editor
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.SceneManagement;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using ThirteenPixels.Soda.Editor;

    public class SceneboundEditor : EditorWindow, IHasCustomMenu
    {
        private static class SupportedTypes
        {
            public class Category
            {
                public readonly string name;
                public bool isCore;
                public readonly List<Type> types = new List<Type>();

                public Category(string name)
                {
                    this.name = name;
                }

                public bool Contains(Type type)
                {
                    foreach (var baseType in types)
                    {
                        if (baseType.IsGenericType)
                        {
                            if (IsAssignableToGenericType(type, baseType))
                            {
                                return true;
                            }
                        }
                        else if (baseType.IsAssignableFrom(type))
                        {
                            return true;
                        }
                    }
                    return false;
                }

                public bool HasInstances(List<ScriptableObject> sceneSOs)
                {
                    return sceneSOs != null && sceneSOs.Any(so => Contains(so.GetType()));
                }

                public int GetInstanceCount(List<ScriptableObject> sceneSOs)
                {
                    if (sceneSOs == null)
                    {
                        return 0;
                    }

                    return sceneSOs.Count(so => Contains(so.GetType()));
                }
            }

            private static readonly List<Category> categories = new List<Category>();
            public static int categoryCount => categories.Count;

            [InitializeOnLoadMethod]
            private static void Initialize()
            {
                var cachedTypes = TypeCache.GetTypesWithAttribute<SceneBindableAttribute>().Where(type => typeof(ScriptableObject).IsAssignableFrom(type));
                categories.Clear();
                
                var categoryDict = new Dictionary<string, Category>();

                foreach (var type in cachedTypes)
                {
                    var attribute = GetSceneBindableAttribute(type);
                    var categoryName = attribute.categoryName;

                    Category category;
                    if (!categoryDict.TryGetValue(categoryName, out category))
                    {
                        category = new Category(categoryName);
                        categoryDict.Add(categoryName, category);
                    }
                    category.types.Add(type);
                    category.isCore = category.isCore || attribute.isCoreItem;
                }

                categories.AddRange(categoryDict.Values);
                
                categories.Sort((a, b) => (b.isCore ? 1 : 0) + (a.isCore ? -1 : 0));
            }

            private static bool IsCoreItem(Type type)
            {
                return GetSceneBindableAttribute(type).isCoreItem;
            }

            public static SceneBindableAttribute GetSceneBindableAttribute(Type type)
            {
                return (SceneBindableAttribute)type.GetCustomAttributes(typeof(SceneBindableAttribute), true).FirstOrDefault();
            }

            public static bool Contains(Type type)
            {
                return GetSceneBindableAttribute(type) != null;
            }

            public static Category GetCategoryAtIndex(int index)
            {
                return categories[index];
            }

            public static int GetCategoryIndexOf(Type type)
            {
                for (var index = 0; index < categories.Count; index++)
                {
                    var category = categories[index];
                    if (category.Contains(type))
                    {
                        return index;
                    }
                }
                return -1;
            }
        }
        
        private static readonly ScriptableObject[] EMPTY_SO_LIST = new ScriptableObject[0];
        private static readonly Color SELECTION_COLOR = new Color(0.5f, 1f, 1.5f, 1f);
        private static GUIStyle SELECTION_BUTTON_STYLE;
        private static GUIStyle CATEGORY_DROPDOWN_STYLE;
        private static GUIStyle ADD_BUTTON_STYLE;

        private ReferenceContainer referenceContainer;
        private List<ScriptableObject> sceneSOs => referenceContainer?.scriptableObjects;
        private SearchField searchField;
        private int selectedCategoryIndex = 0;
        private Vector2 scrollPosition;
        private string filterString = "";
        private bool isDraggingSupportedObjectFromAssets = false;
        private bool isChangingEditorMode = false;
        

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        [MenuItem(SodaEditorHelpers.MENU_ITEM_ROOT + "Scenebound Editor")]
        private static SceneboundEditor OpenWindow()
        {
            const string windowTitle = "Scenebound";
            var hierarchyWindow = SodaEditorHelpers.FindHierarchyWindow();

            SceneboundEditor window;

            if (hierarchyWindow != null)
            {
                window = GetWindow<SceneboundEditor>(windowTitle, hierarchyWindow.GetType());
            }
            else
            {
                window = GetWindow<SceneboundEditor>(windowTitle);
            }
            window.titleContent = new GUIContent(windowTitle, EditorGUIUtility.IconContent("d_UnityEditor.SceneHierarchyWindow").image);
            window.Show();

            return window;
        }

        private static void OnSelectionChanged()
        {
            var activeObject = Selection.activeObject;
            var referenceContainer = FindReferenceContainer();
            
            var activeObjectIsScenebound = referenceContainer != null && referenceContainer.scriptableObjects.Contains(activeObject);

            if (activeObject &&
                SupportedTypes.Contains(activeObject.GetType()) &&
                activeObjectIsScenebound)
            {
                var window = OpenWindow();
                window.selectedCategoryIndex = SupportedTypes.GetCategoryIndexOf(activeObject.GetType());
            }
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Check for dangling instances"), false, CheckForDanglingInstances);
        }

        private void OnEnable()
        {
            RefreshReferenceContainer();
            Undo.undoRedoPerformed += FindReferenceContainerAndScheduleRepaint;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            Selection.selectionChanged += Repaint;
            EditorApplication.update += OnUpdate;

            filterString = "";
            searchField = new SearchField();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= FindReferenceContainerAndScheduleRepaint;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            Selection.selectionChanged -= Repaint;
            EditorApplication.update -= OnUpdate;
        }

        private void OnUpdate()
        {
            var isNowDragging = DragAndDrop.paths.Length == 1 && IsSupportedScriptableObjectPath(DragAndDrop.paths[0]);
            if (isNowDragging != isDraggingSupportedObjectFromAssets)
            {
                isDraggingSupportedObjectFromAssets = isNowDragging;
                Repaint();
            }
        }

        private bool IsSupportedScriptableObjectPath(string path)
        {
            if (!PathIsInAssets(path))
            {
                return false;
            }

            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (so == null)
            {
                return false;
            }

            return SupportedTypes.Contains(so.GetType());
        }

        private bool PathIsInAssets(string path)
        {
            path = System.IO.Path.GetFullPath(path);
            var assetsPath = System.IO.Path.GetFullPath(Application.dataPath);

            return path.StartsWith(assetsPath);
        }

        private void OnActiveSceneChanged(Scene a, Scene b)
        {
            FindReferenceContainerAndScheduleRepaint();
        }

        private void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode || stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                isChangingEditorMode = true;
            }
            else
            {
                FindReferenceContainerAndScheduleRepaint();
                isChangingEditorMode = false;
            }
        }

        private void RefreshReferenceContainer()
        {
            referenceContainer = FindReferenceContainer();
        }

        private static ReferenceContainer FindReferenceContainer()
        {
            var activeScene = SceneManager.GetActiveScene();
            var referenceContainers = FindObjectsOfType<ReferenceContainer>();

            foreach (var container in referenceContainers)
            {
                if (container.gameObject.scene == activeScene)
                {
                    return container;
                }
            }

            return null;
        }

        private void FindReferenceContainerAndScheduleRepaint()
        {
            RefreshReferenceContainer();
            Schedule(Repaint);
        }

        private void Schedule(Action action)
        {
            EditorApplication.delayCall += () => action();
        }

        private static void InitializeStyles()
        {
            if (SELECTION_BUTTON_STYLE == null)
            {
                var defaultSkin = EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector);

                SELECTION_BUTTON_STYLE = new GUIStyle(defaultSkin.FindStyle("Button"));
                SELECTION_BUTTON_STYLE.alignment = TextAnchor.MiddleLeft;
                SELECTION_BUTTON_STYLE.imagePosition = ImagePosition.ImageLeft;

                CATEGORY_DROPDOWN_STYLE = new GUIStyle(EditorStyles.toolbarDropDown);
                CATEGORY_DROPDOWN_STYLE.fontSize = 16;
                CATEGORY_DROPDOWN_STYLE.fontStyle = FontStyle.Bold;
                CATEGORY_DROPDOWN_STYLE.padding = new RectOffset(10, 10, 10, 10);
                CATEGORY_DROPDOWN_STYLE.fixedHeight = 32;

                ADD_BUTTON_STYLE = new GUIStyle(EditorStyles.toolbarDropDown);
                ADD_BUTTON_STYLE.fontSize = 16;
                ADD_BUTTON_STYLE.fontStyle = FontStyle.Bold;
            }
        }

        private void OnGUI()
        {
            InitializeStyles();

            var displayedSOs = sceneSOs.AsEnumerable() ?? EMPTY_SO_LIST;

            var isCompiling = EditorApplication.isCompiling && !EditorApplication.isPlaying;
            var canDisplayList = !isCompiling && !isChangingEditorMode;
            if (canDisplayList)
            {
                if (selectedCategoryIndex < 0 || selectedCategoryIndex >= SupportedTypes.categoryCount)
                {
                    selectedCategoryIndex = 0;
                }

                var category = SupportedTypes.GetCategoryAtIndex(selectedCategoryIndex);

                DisplayCategoryPicker();

                DrawSearchToolbar(category);

                DrawInstanceList(displayedSOs, category, filterString.ToLower());
            }
            else
            {
                EditorGUILayout.HelpBox("Please wait...", MessageType.Info);
            }
        }

        private void DisplayCategoryPicker()
        {
            if (GUILayout.Button(SupportedTypes.GetCategoryAtIndex(selectedCategoryIndex).name, CATEGORY_DROPDOWN_STYLE))
            {
                var menu = new GenericMenu();
                var hadSeperator = false;
                for (var i = 0; i < SupportedTypes.categoryCount; i++)
                {
                    var category = SupportedTypes.GetCategoryAtIndex(i);

                    if (!hadSeperator && !category.isCore)
                    {
                        menu.AddSeparator("");
                        hadSeperator = true;
                    }


                    var label = category.name;
                    var instanceCount = category.GetInstanceCount(sceneSOs);
                    if (instanceCount > 0)
                    {
                        label = $"{label} ({instanceCount})";
                    }

                    var index = i;
                    menu.AddItem(new GUIContent(label),
                        false,
                        () => selectedCategoryIndex = index);
                }
                menu.ShowAsContext();
            }
        }

        private void DrawSearchToolbar(SupportedTypes.Category category)
        {
            if (isDraggingSupportedObjectFromAssets)
            {
                GUILayout.Box("Drop object here to move it into the scene.", GUILayout.ExpandWidth(true));
                SodaDragAndDrop.MakePathDragDestination(GUILayoutUtility.GetLastRect(), IsSupportedScriptableObjectPath, MoveToSceneOnConfirm);
            }
            else
            {
                DrawSearchBarAndNewButton(category);
            }
        }

        private void DrawSearchBarAndNewButton(SupportedTypes.Category category)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(20));
            GUILayout.Space(4);
            
            filterString = searchField.OnGUI(filterString);
            
            if (GUILayout.Button("+", ADD_BUTTON_STYLE))
            {
                var subtypeMenu = CreateSubtypeMenu(category.types, type =>
                {
                    CreateAndAddScriptableObject(type);
                });
                if (subtypeMenu != null)
                {
                    subtypeMenu.ShowAsContext();
                }
                else
                {
                    EditorUtility.DisplayDialog("Unable to create ScriptableObject", "The abstract ScrtipableObject base class does not have any non-abstract inheriting classes.", "OK");
                }
            }

            GUILayout.Space(2);
            GUILayout.EndHorizontal();
        }

        private void CreateAndAddScriptableObject(Type type)
        {
            if (!IsNonAbstractScriptableObjectType(type)) return;

            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Create scene-bound ScriptableObject");
            CreateReferenceContainerIfNeeded();

            var instance = CreateInstance(type);
            Undo.RegisterCreatedObjectUndo(instance, "Create ScriptableObject");

            instance.name = $"New {type.Name}";

            Undo.RecordObject(referenceContainer, "Add ScriptableObject to list");
            sceneSOs.Add(instance);

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            Selection.activeObject = instance;
        }

        private void MoveToSceneOnConfirm(string path)
        {
            const string title = "Move ScriptableObject to Scene";

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorUtility.DisplayDialog(title,
                    "You cannot move ScriptableObjects to the Scene while in play mode.",
                    "OK");
                return;
            }

            if (EditorUtility.DisplayDialog(title,
                "Are you sure that you want to move this ScriptableObject into the current scene?\nALL references to the object will break.\nThis cannot be undone.",
                "Move to Scene", "Cancel"))
            {
                var oldInstance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (SupportedTypes.Contains(oldInstance.GetType()))
                {
                    Undo.IncrementCurrentGroup();
                    Undo.SetCurrentGroupName("Move ScriptableObject to scene");
                    CreateReferenceContainerIfNeeded();

                    var newInstance = Instantiate(oldInstance);
                    Undo.RegisterCreatedObjectUndo(newInstance, "Instantiate ScriptableObject");

                    newInstance.name = oldInstance.name;

                    Undo.RecordObject(referenceContainer, "Add ScriptableObject to list");
                    sceneSOs.Add(newInstance);

                    Undo.DestroyObjectImmediate(oldInstance);
                    AssetDatabase.DeleteAsset(path);

                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

                    var categoryIndex = SupportedTypes.GetCategoryIndexOf(newInstance.GetType());
                    if (categoryIndex >= 0)
                    {
                        selectedCategoryIndex = categoryIndex;
                    }
                    
                    Selection.activeObject = newInstance;
                }
                else
                {
                    EditorUtility.DisplayDialog(title,
                        "The type of the ScriptableObject does not have the [SceneBindable] attribute.",
                        "OK");
                }
            }
        }

        private void DrawInstanceList<T>(IEnumerable<T> list, SupportedTypes.Category category, string filterString) where T : ScriptableObject
        {
            list = list.Where(so => so != null && category.Contains(so.GetType()));
            if (!string.IsNullOrEmpty(filterString))
            {
                list = list.Where(so => so.name.ToLower().Contains(filterString));
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.BeginVertical();
            foreach (var item in list)
            {
                if (item != null)
                {
                    DrawSelectAndDragButton(item);
                }
                else
                {
                    GUILayout.Box("null");
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void DrawSelectAndDragButton(ScriptableObject obj)
        {
            var isSelected = Selection.activeObject == obj;

            using (new SodaEditorHelpers.GUIColor(isSelected ? SELECTION_COLOR : GUI.color))
            {
                var originalContent = EditorGUIUtility.ObjectContent(obj, obj.GetType());
                var content = new GUIContent($" {obj.name}", originalContent.image);
                GUILayout.Box(content, SELECTION_BUTTON_STYLE, GUILayout.Height(28));

                var rect = GUILayoutUtility.GetLastRect();
                SodaDragAndDrop.MakeDragSource(rect, obj);

                var currentEvent = Event.current;
                if (currentEvent.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
                {
                    if (currentEvent.button == 0)
                    {
                        Selection.activeObject = obj;
                    }
                    else if (currentEvent.button == 1)
                    {
                        BuildSOContextMenu(obj).ShowAsContext();
                    }
                }

                if (isSelected && currentEvent.type == EventType.KeyDown)
                {
                    switch (currentEvent.keyCode)
                    {
                        case KeyCode.Delete:
                            DestroySceneboundObjectOnConfirm(obj);
                            break;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_LINUX
                        case KeyCode.F2:
#else
                        case KeyCode.Return:
#endif
                            Schedule(() => OpenRenameDialogFor(obj));
                            break;
                    }
                }
            }
        }

        private GenericMenu BuildSOContextMenu(ScriptableObject target)
        {
            var isOutsidePlaymode = !EditorApplication.isPlayingOrWillChangePlaymode;

            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Rename"),
                false,
                () => OpenRenameDialogFor(target));
            AddContextMenuItem(menu,
                new GUIContent("Move to Assets folder"),
                isOutsidePlaymode,
                () => MoveToAssetsOnConfirm(target));
            menu.AddItem(
                new GUIContent("Destroy"),
                false,
                () => DestroySceneboundObjectOnConfirm(target));

            return menu;
        }

        private void AddContextMenuItem(GenericMenu menu, GUIContent content, bool enabled, GenericMenu.MenuFunction func)
        {
            if (enabled)
            {
                menu.AddItem(content, false, func);
            }
            else
            {
                menu.AddDisabledItem(content);
            }
        }
        
        private void OpenRenameDialogFor(ScriptableObject target)
        {
            const string title = "Rename ScriptableObject";
            StringInputDialog.Show(title,
                "Please enter a new name:",
                target.name,
                "Rename",
                "Cancel",
                newName =>
                {
                    newName = newName.Trim();
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        Undo.RecordObject(target, title);
                        target.name = newName;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(title, "Invalid name.", "OK");
                    }
                });
            Focus();
        }

        private void MoveToAssetsOnConfirm(ScriptableObject target)
        {
            const string title = "Move ScriptableObject to Assets";

            if (EditorUtility.DisplayDialog(title,
                "Are you sure you want to move this ScriptableObject into the Assets?\nThis cannot be reverted, and moving it back into the scene will break all references to it.",
                "Move to Assets", "Cancel"))
            {
                var openedPath = SodaEditorHelpers.TryGetActiveFolderPath();
                if (openedPath == null)
                {
                    openedPath = "Assets/";
                }

                var filename = FormatForFilename(target.name);
                var relativePath = openedPath + filename + ".asset";
                var fullPath = System.IO.Directory.GetParent(Application.dataPath).FullName + "/" + relativePath;

                if (!System.IO.File.Exists(fullPath))
                {
                    AssetDatabase.CreateAsset(target, relativePath);
                    referenceContainer.scriptableObjects.Remove(target);

                    EditorGUIUtility.PingObject(target);
                }
                else
                {
                    EditorUtility.DisplayDialog(title,
                        "Cannot move this ScriptableObject to the Assets.\nA file with the same name already exists in the Assets folder.",
                        "OK");
                }
            }
        }

        private string FormatForFilename(string name)
        {
            return System.IO.Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }

        private void DestroySceneboundObjectOnConfirm(ScriptableObject target)
        {
            if (EditorUtility.DisplayDialog("Destroy scene-bound ScriptableObject",
                $"Are you sure you want to destroy the {target.GetType().Name} \"{target.name}\"?",
                "Destroy",
                "Cancel"))
            {
                Schedule(() => DestroySceneboundObject(target));
            }
        }

        private void DestroySceneboundObject(ScriptableObject target)
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Destroy scene-bound ScriptableObject");
            Undo.RecordObject(referenceContainer, "Delete ScriptableObject from reference container");
            sceneSOs.Remove(target);

            Undo.DestroyObjectImmediate(target);

            if (sceneSOs.Count == 0)
            {
                Undo.DestroyObjectImmediate(referenceContainer);
                referenceContainer = null;
            }
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private void CreateReferenceContainerIfNeeded()
        {
            RefreshReferenceContainer();
            if (!referenceContainer)
            {
                var go = new GameObject("Soda ScriptableObject Reference Container");
                referenceContainer = go.AddComponent<ReferenceContainer>();
                go.hideFlags = HideFlags.HideInHierarchy;

                Undo.RegisterCreatedObjectUndo(go, "Create reference container");
                Schedule(Repaint);
            }
        }

        private static GenericMenu CreateSubtypeMenu(IEnumerable<Type> baseTypes, Action<Type> clickHandler)
        {
            var types = new List<Type>();
            foreach (var baseType in baseTypes)
            {
                types.AddRange(TypeCache.GetTypesDerivedFrom(baseType));
            }

            var menu = new GenericMenu();

            foreach (var baseType in baseTypes)
            {
                if (!baseType.IsAbstract && !baseType.IsGenericType)
                {
                    menu.AddItem(new GUIContent(baseType.Name), false, () => clickHandler(baseType));
                }
            }
            
            foreach (var type in types)
            {
                var canBeCreated = !type.IsAbstract && !type.IsNested && type.IsPublic;
                if (canBeCreated)
                {
                    var t = type;
                    menu.AddItem(new GUIContent(t.Name), false, () => clickHandler(t));
                }
            }

            if (menu.GetItemCount() > 0)
            {
                return menu;
            }
            return null;
        }

        private static bool IsNonAbstractScriptableObjectType(Type type)
        {
            return !type.IsAbstract && typeof(ScriptableObject).IsAssignableFrom(type);
        }

        private static void MoveElementToPosition<T>(List<T> list, T element, int index)
        {
            if (list.Remove(element))
            {
                list.Insert(index, element);
            }
            else
            {
                throw new Exception("Item to be moved was not in the list.");
            }
        }

        private void CheckForDanglingInstances()
        {
            const string title = "Check for dangling instances";

            var unsupportedInstances = new List<ScriptableObject>();
            var unsupportedTypes = new HashSet<Type>();
            foreach (var so in sceneSOs)
            {
                if (!SupportedTypes.Contains(so.GetType()))
                {
                    unsupportedInstances.Add(so);
                    unsupportedTypes.Add(so.GetType());
                }
            }

            if (unsupportedInstances.Count > 0)
            {
                var message = $"{unsupportedInstances.Count} dangling instances were found. ";
                message += unsupportedInstances.Count == 1 ? "Its " : "Their ";
                message += unsupportedTypes.Count == 1 ? "type is:" : "types are:";

                const int MAX_UNSUPPORTED_TYPES_IN_LIST = 10;
                var typeIndex = 0;
                foreach (var type in unsupportedTypes)
                {
                    message += "\n" + type.Name;
                    typeIndex++;
                    if (typeIndex >= MAX_UNSUPPORTED_TYPES_IN_LIST)
                    {
                        break;
                    }
                }
                if (unsupportedTypes.Count > MAX_UNSUPPORTED_TYPES_IN_LIST)
                {
                    message += "\n...and more.";
                }

                if (EditorUtility.DisplayDialog(title, message, "Delete All", "Cancel"))
                {
                    foreach (var instance in unsupportedInstances)
                    {
                        referenceContainer.scriptableObjects.Remove(instance);
                        DestroyImmediate(instance);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog(title, "No dangling instances were found.", "OK");
            }
        }

        private static bool IsAssignableToGenericType(Type type, Type genericSupertype)
        {
            var interfaceTypes = type.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericSupertype)
                {
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericSupertype)
            {
                return true;
            }

            var baseType = type.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericSupertype);
        }
    }
}
