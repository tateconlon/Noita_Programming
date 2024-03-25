using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// References:
// https://stackoverflow.com/a/6869625
// https://hextantstudios.com/unity-selection-history/
// https://github.com/mminer/selection-history-navigator

[InitializeOnLoad]
static class NavigationHistory
{
    // NOTE: I didn't put size caps on these stacks because I domain reload frequently enough that them occupying too
    // much memory isn't really a concern.
    private static readonly Stack<Object[]> BackStack = new();
    private static readonly Stack<Object[]> ForwardStack = new();
    private static Object[] _curSelection;
    
    private static bool _isSimulatedSelectionChange = false;
    
    static NavigationHistory()
    {
        EditorMouseNavigation.BackButtonPressed -= Back;
        EditorMouseNavigation.BackButtonPressed += Back;
        
        EditorMouseNavigation.ForwardButtonPressed -= Forward;
        EditorMouseNavigation.ForwardButtonPressed += Forward;
        
        Selection.selectionChanged -= OnSelectionChanged;
        Selection.selectionChanged += OnSelectionChanged;
        
        _curSelection = (Object[])Selection.objects.Clone();
    }
    
    private static void OnSelectionChanged()
    {
        if (_isSimulatedSelectionChange)
        {
            // Note that we change unset this flag here because Selection.selectionChanged can happen on next frame
            _isSimulatedSelectionChange = false;
            return;
        }
        
        TryAddCurSelectionToStack(BackStack);
        
        _curSelection = (Object[])Selection.objects.Clone();
        
        ForwardStack.Clear();
    }
    
    [MenuItem("Fwd+Back/Back %&'")] //option+command+APOSTROPHE NOT TILDE - TILDE HAS A SHORTCUT IN UNITY
    private static void Back()
    {
        if (BackStack.Count < 1) return;
        
        TryAddCurSelectionToStack(ForwardStack);
        
        _isSimulatedSelectionChange = true;
        Selection.objects = BackStack.Pop();
        _curSelection = (Object[])Selection.objects.Clone();
    }
    
    [MenuItem("Fwd+Back/Forward #%&'")] //option+command+APOSTROPHE NOT TILDE - TILDE HAS A SHORTCUT IN UNITY
    private static void Forward()
    {
        if (ForwardStack.Count < 1) return;
        
        TryAddCurSelectionToStack(BackStack);
        
        _isSimulatedSelectionChange = true;
        Selection.objects = ForwardStack.Pop();
        _curSelection = (Object[])Selection.objects.Clone();
    }
    
    private static void TryAddCurSelectionToStack(Stack<Object[]> targetStack)
    {
        // Ignore empty selections since we assume that users want to navigate around to select objects, not nothing.
        if (_curSelection.Length <= 0) return;
        
        // If the current selection is already at the top of the stack, don't add it again. This can happen if the user
        // selects an object, then selects nothing, then selects the object again.
        // We have to use SequenceEqual because _curSelection is a cloned array, so we need memberwise comparison.
        if (targetStack.Count > 0 && _curSelection.SequenceEqual(targetStack.Peek())) return;
        
        targetStack.Push(_curSelection);
    }
}
