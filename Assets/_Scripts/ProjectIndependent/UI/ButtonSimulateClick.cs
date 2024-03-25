using UnityEngine.EventSystems;
using UnityEngine.UI;

// From: https://discussions.unity.com/t/how-to-trigger-a-button-click-from-script/135868/3
// (This code was also used in Scoober Splat without any issues)

public static class ButtonSimulateClick
{
    /// <summary>
    /// This presses a button, including the color change/animation triggered by clicking the button through UI
    /// </summary>
    /// <param name="button"></param>
    public static void SimulateClick(this Button button)
    {
        ExecuteEvents.Execute(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}