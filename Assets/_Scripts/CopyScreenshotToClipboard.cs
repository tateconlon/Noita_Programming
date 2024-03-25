using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_STANDALONE_WIN
using System.Drawing;
#endif

public class CopyScreenshotToClipboard : MonoBehaviour
{
    [Button]
    public void CaptureAndCopy()
    {
        if (!Application.isPlaying) return;
        
        StartCoroutine(InternalCaptureAndCopy());
    }

    IEnumerator InternalCaptureAndCopy()
    {
        yield return new WaitForEndOfFrame();
        
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        #if UNITY_STANDALONE_WIN
        System.IO.Stream s = new System.IO.MemoryStream(screenshot.width * screenshot.height);
        byte[] bits = screenshot.EncodeToJPG();
        s.Write(bits, 0, bits.Length);
        Image image = Image.FromStream(s);
        System.Windows.Forms.Clipboard.SetImage(image);
        s.Close();
        s.Dispose();
        #else
        Debug.LogWarning("Copying screenshot to clipboard only supported on Windows64");
        #endif
        
        Destroy(screenshot);
    }
}
