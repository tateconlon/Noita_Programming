using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[AddComponentMenu("_Visuals/Sprite/Sprite Shadow")]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShadow : MonoBehaviour
{
    [SerializeField] private Vector2 offset = new Vector2(0,-0.3f);
    
    [FoldoutGroup("Bindings", expanded: false)]
    [SerializeField] private Material shadowMaterial;

    private Transform ourTransform;
    private Transform shadowTransform;
    private SpriteRenderer ourSprRnd;
    private SpriteRenderer shadowSprRnd;

    void Awake()
    {
        ourTransform = transform;
        ourSprRnd = GetComponent<SpriteRenderer>();
        
        GameObject shadow = new GameObject($"{name} Shadow Sprite");
        shadowTransform = shadow.transform;
        shadowTransform.SetParent(ourTransform);
        shadowSprRnd = shadow.AddComponent<SpriteRenderer>();
        shadowSprRnd.material = shadowMaterial;
        shadowTransform.localRotation = Quaternion.identity; //Edge case fix from youtube video that I'm not questioning https://youtu.be/ft4HUL2bFSQ?si=XdmGiIMqxQ1CZXE5&t=360
        shadowTransform.localScale = Vector3.one;
        
        //Drop shadow layering
        shadowSprRnd.sortingLayerName = ourSprRnd.sortingLayerName;
        shadowSprRnd.sortingOrder = ourSprRnd.sortingOrder - 1;
    }
    void LateUpdate()
    {
        shadowTransform.position = ourTransform.position + new Vector3(offset.x, offset.y, 0f);
        shadowSprRnd.sprite = ourSprRnd.sprite;
    }
}
