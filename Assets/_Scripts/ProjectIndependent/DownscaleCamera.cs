using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class DownscaleCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private FilterMode upscaleFilterMode = FilterMode.Point;
    [SerializeField] private int targetScreenPixelHeight = 240;

    private RenderTexture _downscaleRT;

    private void Awake()
    {
        Debug.Assert(gameObject.GetComponents(typeof(Component)).Last() == this, 
            "The DownscaleCamera must be the last child of the Camera GameObject to downscale Post-Processing");
        
        if (upscaleFilterMode == FilterMode.Point && cam.allowMSAA)
        {
            Debug.LogWarning("Disable MSAA on the Camera to achieve better visuals with Point filter mode");
        }
    }

    private void OnPreRender()
    {
        _downscaleRT = CreateDownscaleTexture();
        cam.targetTexture = _downscaleRT;
    }
    
    private RenderTexture CreateDownscaleTexture()
    {
        int width = Mathf.RoundToInt(cam.aspect * targetScreenPixelHeight);
        int height = targetScreenPixelHeight;
    
        RenderTextureDescriptor descriptor = new(width, height)
        {
            graphicsFormat = SystemInfo.GetGraphicsFormat(cam.allowHDR ? DefaultFormat.HDR : DefaultFormat.LDR),
            useMipMap = false,
            autoGenerateMips = false,
            depthBufferBits = 0,
            msaaSamples = 1,
            dimension = TextureDimension.Tex2D
        };

        return RenderTexture.GetTemporary(descriptor);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)  // Happens after camera rendering + Post Processing
    {
        cam.targetTexture = null;  // Sets camera target to screen
        
        src.filterMode = upscaleFilterMode;  // Changing the filterMode here affects final output to the screen

        Graphics.Blit(src, null as RenderTexture);  // Blit from _downscaleRT to the screen
        
        RenderTexture.ReleaseTemporary(_downscaleRT);  // MUST INCLUDE to properly free _downscaleRT

        RenderTexture.active = dest;  // Hides warning that we're not blitting from src to dest
    }
}