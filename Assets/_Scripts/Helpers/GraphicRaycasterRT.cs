using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    /// <summary>
    /// Adjusts raycast position based on the target texture of the target camera,
    /// for use with cameras rendering to RenderTextures that aren't the same size as the screen
    /// </summary>
    public class GraphicRaycasterRT : GraphicRaycaster
    {
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            RenderTexture targetTexture = eventCamera.targetTexture;
            Vector2 targetTextureDimensions = new(targetTexture.width, targetTexture.height);

            Vector2 displayDimensions = new(Display.main.renderingWidth, Display.main.renderingHeight);

            // This properly adjusts the pointer position to account for the scale of the camera's RT
            Vector2 originalPosition = eventData.position;
            eventData.position *= targetTextureDimensions / displayDimensions;
        
            base.Raycast(eventData, resultAppendList);
            
            eventData.position = originalPosition;
        }
    }
}