using UnityEngine;

namespace AdvancedFeedbackRatingForm
{
    public class DrawingSettings : MonoBehaviour
    {
        public static bool isCursorOverUI = false;
        public float Transparency = 1f;

        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }
        public void SetMarkerWidth(int new_width)
        {
            Drawable.Pen_Width = new_width;
        }
        public void SetMarkerWidth(float new_width)
        {
            SetMarkerWidth((int)new_width);
        }

        public void SetTransparency(float amount)
        {
            Transparency = amount;
            Color c = Drawable.Pen_Colour;
            c.a = amount;
            Drawable.Pen_Colour = c;
        }

        public void SetMarkerRed()
        {
            Color c = Color.red;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerWhite()
        {
            Color c = Color.white;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerBlack()
        {
            Color c = Color.black;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerYellow()
        {
            Color c = Color.yellow;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerGrey()
        {
            Color c = Color.grey;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerGreen()
        {
            Color c = Color.green;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetMarkerBlue()
        {
            Color c = Color.blue;
            c.a = Transparency;
            SetMarkerColour(c);
            Drawable.drawable.SetPenBrush();
        }
        public void SetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0f));
        }

        public void PartialSetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0.5f));
        }
    }
}