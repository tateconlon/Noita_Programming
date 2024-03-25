## [1.5.4] - 2023-04-07
- Added more null checks

## [1.5.3] - 2023-02-26
- Added "optional" parameter to SubAssetsAttribute
- Added new option to persistently save a value set in play mode for fields that have the DisplayInsteadInPlaymodeAttribute (requires Unity 2022.2 or higher)
- Improved robustness of the ModuleSettings System

## [1.5.2] - 2023-02-10
- Fixed new ScopedVariable class not being serializable
- Fixed warnings caused by GlobalGameObjectRegister under certain conditions
- Added test classes for GlobalGameObjectRegister and RuntimeSetElement

## [1.5.1] - 2023-02-01
- Fixed compatibility issue with ScopedVariables created by using the wizard before 1.5.0
- Fixed Sub Asset System related error message that could pop up after recompilation

## [1.5.0] - 2023-01-31
- Added SubAssets System
- Updated ScopedVariables to be used without subtypes for Unity 2020.1 and higher
- Improved compatibility of editor code with other custom editor UIs
- Improved multiple elements of Soda's editor UI

## [1.4.12] - 2023-01-04
- Fixed multiple issues with Soda's editor UI

## [1.4.11] - 2022-12-14
- Fixed a GlobalVariable bug that could produce values from a previous play mode run when editing the value through the inspector

## [1.4.10] - 2022-08-03
- Fixed ModuleSettings not working in editor under random conditions due to late initialization
- Made various small improvements

## [1.4.9] - 2022-05-29
- Fixed ModuleSettings bug that prevented properties from saving new values
- Fixed ModuleSettings bug that would cause settings objects to only load in play mode if/once the ProjectSettings tab was opened and enabled
- Added GameEvent.deactivateAfterRaise

## [1.4.8] - 2022-02-10
- Created new ScriptableObject icons
- Added ScriptableObject icons to Scenebound editor window
- Move Runtime and Editor folder into new "Core" folder to better distinguish between core features and ModuleSettings System/Scenebound System

## [1.4.7] - 2022-01-21
- Expanded RuntimeSetBase to have index-based access
- Improved the RuntimeSetElement and GlobalGameObjectRegister components to display whether its GameObject matches the specifications of the referenced RuntimeSet/GlobalGameObject
- Added type creation wizard for GlobalGameObjectWithComponentCache classes
- Improved type creation wizards to create new class files in the folder that is currently open in the project view
- Fixed an issue that could reset a GlobalVariable's value in editor play mode

## [1.4.6] - 2021-12-06
- Fixed a NullReferenceException caused by the Scenebound System
- Added Scenebound System test class

## [1.4.5] - 2021-12-03
- Allowed using GameEvent<T>.onRaise to be used with and without parameters
- Accordingly, marked GameEventBase<T>.onRaiseWithParameter as obsolete
- Fixed documentation base url

## [1.4.4] - 2021-12-01
- Added support for parameterless responses in SodaEvents with parameters
- Added instance count to Scenebound editor type selection menu
- Fixed an issue with the Scenebound editor window popping up when creating a new, scene-bindable ScriptableObject

## [1.4.3] - 2021-09-23
- Fixed an issue with the Scenebound editor window popping up too often

## [1.4.2] - 2021-09-01
- Fixed error noise "Unexpected recursive transfer of scripted class" during ModuleSettings initialization
- Focus the Scenebound editor window when a scenebound ScriptableObject is selected
- Made various small improvements and fixes

## [1.4.1] - 2021-08-25
- Added HelpUrl attributes to ScriptableObject and MonoBehaviour (base) classes

## [1.4.0] - 2021-08-23
- Added ModuleSettings System

## [1.3.0] - 2021-08-01
- Added Scenebound System
- Made GameEventBase.debug a serialized field which allows it to be used to debug builds
- Improved editor support for scoped variables defined in base classes
- Improved RuntimeSet iteration options
- Improved robustness of editor classes
- Moved MenuItems from "Window" to "Tools"
- Made various small improvements

## [1.2.8] - 2021-03-23
- Improved support for GlobalVariables with non-serialized types
- Removed console output clutter from tests
- Added support for more numeric types to DisplayInsteadInPlaymodeAttribute

## [1.2.7] - 2021-01-15
- Re-enabled inspector subtitles with meta information, rendered differently in Unity 2019.1 and newer

## [1.2.6] - 2020-11-27
- Added GlobalGameObjectWithComponentCache.onChangeComponentCache
- Properly display SodaEvent responses coming from ScopedVariables as proxies
- Use nameof keyword for DisplayInsteadInPlaymodeAttribute

## [1.2.5] - 2020-02-17
- Added GlobalVariablePropertyDrawer
- Added unit tests
- Improved editor GUI for new Unity skin

## [1.2.4] - 2020-01-28
- Added parameterized GameEvent base class
- Fixed and improved GlobalGameObjectWithComponentCache

## [1.2.3] - 2020-01-07
- Improved SodaEvents to handle removal of responses by other responses

## [1.2.2] - 2019-12-02
- Prevent cyclic/recursive invocation in SodaEvents
- Let ScriptableObject editors display additional serialized fields

## [1.2.1] - 2019-11-08
- Redesigned icons
- Added changelog

## [1.2.0] - 2019-10-26
- Removed the need to pass a reference to the listener to SodaEvent.AddResponse for debugging
- Added uint support to DisplayInsteadInPlaymodeAttribute

## [1.1.0] -
- Enabled editor classes to work for subclasses of their targets, removing the need for editor subclasses
- Changed class creation templates to not include editor classes anymore
- Fixed CreateAssetMenu menuNames for RuntimeSets (including template)
- Added default implementations for GlobalGameObjectWithComponentCacheBase.TryCreateComponentCache and RuntimeSetBase.TryCreateElement
- Added package.json for package manager support
