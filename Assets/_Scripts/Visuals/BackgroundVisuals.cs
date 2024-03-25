using UnityEngine;

public class BackgroundVisuals : MonoBehaviour
{
    [Header("Background Gradient")]
    [SerializeField] Gradient backgroundGradient;
    [SerializeField] SpriteRenderer backgroundGradientRenderer;

    [Header("Checkerboard Pattern")]
    [SerializeField] Color backgroundOffColor = new((float)46 / 255, (float)46 / 255, (float)46 / 255);
    [SerializeField] int checkerboardWidthBlocks = 32;
    [SerializeField] int checkerboardHeightBlocks = 18;
    [SerializeField] int checkerboardBlockSize = 22;
    [SerializeField] SpriteRenderer checkerboardRenderer;
    
    void Start()
    {
        GenerateBackgroundGradientTexture();
        GenerateCheckerboardTexture();
    }

    void GenerateBackgroundGradientTexture()
    {
        Texture2D backgroundGradientTexture = new(D.vv.gameWidth, D.vv.gameHeight);

        Color[] backgroundPixels = new Color[backgroundGradientTexture.width * backgroundGradientTexture.height];
        
        for (int i = 0; i < backgroundGradientTexture.height; i++)
        {
            int startingIndex = i * backgroundGradientTexture.width;
            Color rowColor = backgroundGradient.Evaluate((float)i / backgroundGradientTexture.height);
            
            for (int j = 0; j < backgroundGradientTexture.width; j++)
            {
                backgroundPixels[startingIndex + j] = rowColor;
            }
        }
        
        backgroundGradientTexture.SetPixels(backgroundPixels);  // Note that this starts from bottom-left corner
        backgroundGradientTexture.Apply(false);
        
        backgroundGradientRenderer.sprite = Sprite.Create(backgroundGradientTexture, 
            new Rect(0, 0, backgroundGradientTexture.width, backgroundGradientTexture.height), 
            new Vector2(0.5f, 0.5f), 1);
    }

    void GenerateCheckerboardTexture()
    {
        Texture2D checkerboardTexture = new(checkerboardWidthBlocks * checkerboardBlockSize, 
            checkerboardHeightBlocks * checkerboardBlockSize);
        
        checkerboardTexture.ClearColor(Color.clear);

        Color[] checkerboardBlockPixels = new Color[checkerboardBlockSize * checkerboardBlockSize];

        for (int i = 0; i < checkerboardBlockPixels.Length; i++)
        {
            checkerboardBlockPixels[i] = backgroundOffColor;
        }

        for (int i = 0; i < checkerboardWidthBlocks; i++)
        {
            for (int j = 0; j < checkerboardHeightBlocks; j++)
            {
                if ((j % 2 == 1 && i % 2 == 0) || 
                    (j % 2 == 0 && i % 2 == 1))
                {
                    // TODO: love draws from top-left, does SetPixels start from bottom-left in this case?
                    checkerboardTexture.SetPixels((i * checkerboardBlockSize), (j * checkerboardBlockSize),
                        checkerboardBlockSize, checkerboardBlockSize, checkerboardBlockPixels);
                }
            }
        }
        
        checkerboardTexture.Apply(false);
        
        // The width and height of the rect should chop off half of a block's size from all edges to match
        // the behavior of love's graphics.rectangle2() method that draws centered rectangles
        checkerboardRenderer.sprite = Sprite.Create(checkerboardTexture, 
            new Rect((float)checkerboardBlockSize / 2, (float)checkerboardBlockSize / 2, D.vv.gameWidth, D.vv.gameHeight), 
            new Vector2(0.5f, 0.5f), 1);
    }
}
