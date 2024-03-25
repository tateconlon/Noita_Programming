using UnityEngine;
using UnityEngine.UI;
using ThirteenPixels.Soda;

/// <summary>
/// Displays a float using a UnityEngine.UI.Image by setting its fillAmount.
/// </summary>
[AddComponentMenu("Soda/Essentials/UI/Display Float as Image Fill")]
[RequireComponent(typeof(Image))]
public class UIDisplayFloatAsImageFill : MonoBehaviour
{
    private Image image;
    [SerializeField]
    private ScopedVariable<float> number = default;
    [SerializeField]
    private ScopedVariable<float> maximum = default;


    private void Reset()
    {
        maximum = new ScopedVariable<float>(1);
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        number.onChangeValue.AddResponseAndInvoke(DisplayNumber);
    }

    private void OnDisable()
    {
        number.onChangeValue.RemoveResponse(DisplayNumber);
    }

    private void DisplayNumber(float number)
    {
        image.fillAmount = number / maximum.value;
    }
}
