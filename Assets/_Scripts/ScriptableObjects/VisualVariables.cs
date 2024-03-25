using UnityEngine;

[CreateAssetMenu(fileName = "VisualVariables", menuName = "ScriptableObject/VisualVariables", order = 0)]
public class VisualVariables : ScriptableObject
{
    [Header("Tweening Defaults")]
    public float tweenBounceScale = 1.25f;
    public float tweenBounceDuration = 0.25f;

    [Header("SNKRX Config Values")]
    [Tooltip("'ww' in SNKRX source code (width of the window")]
    public int windowWidth = 960;
    [Tooltip("'wh' in SNKRX source code (height of the window)")]
    public int windowHeight = 540;
    [Tooltip("'gw' in SNKRX source code (width of the camera)")]
    public int gameWidth = 480;
    [Tooltip("'gh' in SNKRX source code (height of the camera)")]
    public int gameHeight = 270;

    [Header("Uncategorized")]
    [Tooltip("target_distance from Player.update() in source code")]
    public float targetHeroFollowDistance = 10.4f;
}