using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffectOutline : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Material outlineMaterial;

    void OnEnable()
    {
        cam.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, outlineMaterial);
    }
}
