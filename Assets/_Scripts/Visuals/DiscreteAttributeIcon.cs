using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DiscreteAttributeIcon : MonoBehaviour, IBindable<DiscreteAttributeIcon.IconData>
{
    [SerializeField, Required] private Image _image;
    [SerializeField, Required] private Sprite _spriteFull;
    [SerializeField, Required] private Sprite _spriteHalf;
    [SerializeField, Required] private Sprite _spriteEmpty;
    
    public IconData BoundTarget { get; private set; }
    
    public void Bind(IconData target)
    {
        BoundTarget = target;
        
        if (target.FillValue >= 1.0f)
        {
            _image.sprite = _spriteFull;
        }
        else if (target.FillValue <= 0.0f)
        {
            _image.sprite = _spriteEmpty;
        }
        else
        {
            _image.sprite = _spriteHalf;
        }
    }
    
    private void Reset()
    {
        _image = GetComponent<Image>();
    }
    
    // Need to use a class instead of just a float because CollectionBoundPrefabs tracks elements in a dictionary
    [Serializable]
    public class IconData
    {
        public readonly float FillValue;
        
        public IconData(float fillValue)
        {
            FillValue = fillValue;
        }
    }
}