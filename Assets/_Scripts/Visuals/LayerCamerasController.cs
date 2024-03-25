using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class LayerCamerasController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;

    [SerializeField] List<LayerEffectsConfig> layerEffectsConfigs;

    [Header("Shadows")]
    [SerializeField] Material shadowCasterAccumulationMat;
    [SerializeField] RenderTexture shadowCasterAccumulationTex;

    CommandBuffer _mainCamCommands;
    
    // layers(
    // {'default'},
    // {'bg'},
    // {'bg_2'},
    // {'shadow', x = 4*game.sx, y = 4*game.sy, shadow = true, layers = {'game', 'effects'}},
    // {'game', outline = 2},
    // {'game_2', outline = 2},
    // {'effects', outline = 2},
    // {'ui_bg'},
    // {'ui', outline = 2},
    // {'ui_2', outline = 2},
    // {'screen'}
    // )

    [Button]
    void GenerateCameras()
    {
        for (int i = 0; i < layerEffectsConfigs.Count; i++)
        {
            LayerEffectsConfig layer = layerEffectsConfigs[i];
            
            GameObject layerGameObject = new($"LayerCamera{i}");
            layerGameObject.transform.SetParent(transform);

            layer.camera = layerGameObject.AddComponent<Camera>();
            layer.camera.enabled = false;
            layer.camera.CopyFrom(mainCamera);
            layer.camera.backgroundColor = new Color(1, 1, 1, 0);  // TODO: use color from mainCam but 0 alpha?
            layer.camera.cullingMask = layer.layers;
            layer.camera.targetTexture = layer.targetTexture;
        }
    }

    void Awake()
    {
        for (int i = 0; i < layerEffectsConfigs.Count; i++)
        {
            LayerEffectsConfig layer = layerEffectsConfigs[i];

            mainCamera.cullingMask &= ~layer.camera.cullingMask;  // So mainCamera doesn't render same layers
        }
        
        _mainCamCommands = new CommandBuffer();
        _mainCamCommands.name = nameof(_mainCamCommands);
        mainCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, _mainCamCommands);
    }

    void OnPreRender()
    {
        _mainCamCommands.Clear();
        shadowCasterAccumulationTex.Clear();

        foreach (LayerEffectsConfig layer in layerEffectsConfigs)
        {
            layer.camera.orthographicSize = mainCamera.orthographicSize;
            // TODO: anything else to update from mainCamera?
            
            layer.camera.Render();

            if (layer.castShadow)
            {
                Graphics.Blit(layer.targetTexture, shadowCasterAccumulationTex, shadowCasterAccumulationMat);
            }
        }
        
        foreach (LayerEffectsConfig layer in layerEffectsConfigs)
        {
            _mainCamCommands.Blit(layer.targetTexture, BuiltinRenderTextureType.CurrentActive, layer.blitMaterial);
        }
    }

    [Serializable]
    class LayerEffectsConfig
    {
        public LayerMask layers;
        
        [Tooltip("Can include fullscreen image effects like screen-space outlines")]
        public Material blitMaterial;
        
        // TODO: outline material and shadow material instead of default BlitBlend
        
        public bool castShadow = false;
        
        public RenderTexture targetTexture;

        [HideInInspector] public Camera camera;
    }
}
