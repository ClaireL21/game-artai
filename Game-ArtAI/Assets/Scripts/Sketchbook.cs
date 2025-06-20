using System;
using System.Linq;
using UnityEngine;

public class Sketchbook : MonoBehaviour
{
    //public GameObject Eraser;
    //public GameObject Pencil;
    //public Transform Sizes;
    //public GameObject Palette;
    //public GameObject Canvas;

    // can change this to be hex or smthing
    
    private bool eraserMode;
    private Color lineColor; 
    private float lineWidth; 

    Transform clickedObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eraserMode = false;
        lineColor = Color.black;
        lineWidth = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // getting obj that is hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                clickedObj = hit.transform;

                if (clickedObj.tag == "Color")
                {
                    lineColor = clickedObj.gameObject.GetComponent<SpriteRenderer>().color;
                }

                else if (clickedObj.tag == "Size")
                {
                    lineWidth = float.Parse(clickedObj.name);
                }

                else if (clickedObj.tag == "Options")
                {
                    if (clickedObj.name == "Eraser")
                    {
                        eraserMode = true; 
                    }
                    else
                    {
                        eraserMode = false; 
                    }
                }

                else if (clickedObj.tag == "Sketchbook")
                {
                    SpriteRenderer sr = clickedObj.GetComponent<SpriteRenderer>();

                    // Clone the texture if we haven't already
                    if (sr.sprite.texture.isReadable == false)
                    {
                        Debug.LogError("Texture is not readable! Enable Read/Write in import settings.");
                        return;
                    }

                    // Get or create a clone of the texture
                    Texture2D tex = GetOrCreateTextureClone(sr);

                    // Convert mouse position to texture coordinates
                    Vector2 pixelUV = hit.textureCoord;
                    pixelUV.x *= tex.width;
                    pixelUV.y *= tex.height;

                    int pixelX = (int)pixelUV.x;
                    int pixelY = (int)pixelUV.y;

                    Debug.Log($"Drawing at: ({pixelX}, {pixelY}) on texture size {tex.width}x{tex.height}");

                    // Draw with brush size
                    int brushSize = Mathf.Max(1, (int)lineWidth);
                    Color drawColor = eraserMode ? Color.white : lineColor;

                    Color[] pixels = new Color[tex.width * tex.height];
                    for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.red;
                    tex.SetPixels(pixels);
                    tex.Apply();

                    //for (int x = pixelX - brushSize; x <= pixelX + brushSize; x++)
                    //{
                    //    for (int y = pixelY - brushSize; y <= pixelY + brushSize; y++)
                    //    {
                    //        if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    //        {
                    //            // Simple circular brush check
                    //            if (Vector2.Distance(new Vector2(x, y), new Vector2(pixelX, pixelY)) <= brushSize)
                    //            {
                    //                tex.SetPixel(x, y, drawColor);
                    //            }
                    //        }
                    //    }
                    //}

                    //if (pixelX >= 0 && pixelX < tex.width && pixelY >= 0 && pixelY < tex.height)
                    //{
                    //    // Draw a solid circle
                    //    for (int y = pixelY - brushSize; y <= pixelY + brushSize; y++)
                    //    {
                    //        for (int x = pixelX - brushSize; x <= pixelX + brushSize; x++)
                    //        {
                    //            if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    //            {
                    //                float dist = Mathf.Sqrt(
                    //                    Mathf.Pow(x - pixelX, 2) +
                    //                    Mathf.Pow(y - pixelY, 2)
                    //                );

                    //                if (dist <= brushSize)
                    //                {
                    //                    // For eraser, use clear color with alpha blending
                    //                    if (eraserMode)
                    //                    {
                    //                        Color current = tex.GetPixel(x, y);
                    //                        current.a = Mathf.Lerp(current.a, 0, 0.5f);
                    //                        tex.SetPixel(x, y, current);
                    //                    }
                    //                    else
                    //                    {
                    //                        tex.SetPixel(x, y, drawColor);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }

                    //    tex.Apply();

                    //    UpdateSpriteFromTexture(sr, tex);
                    //}
                }

            }
        }
    }
    private Texture2D GetOrCreateTextureClone(SpriteRenderer sr)
    {
        // If we already have a cloned texture, use it
        if (sr.sprite != null && sr.sprite.texture != null && sr.sprite.texture.name.EndsWith("(Clone)"))
        {
            return sr.sprite.texture;
        }

        if (sr.sprite == null)
        {
            Debug.LogError("SpriteRenderer has no sprite assigned!");
            return null;
        }

        Sprite originalSprite = sr.sprite;
        Texture2D originalTexture = originalSprite.texture;

        // Get the actual sprite rectangle in pixels
        Rect spriteRect = originalSprite.rect;
        int width = (int)spriteRect.width;
        int height = (int)spriteRect.height;

        // Create new texture with the exact sprite dimensions
        Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            filterMode = originalTexture.filterMode,
            wrapMode = originalTexture.wrapMode
        };

        // Get pixels from the original sprite region only
        Color[] pixels = originalTexture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            width,
            height
        );

        newTexture.SetPixels(pixels);
        newTexture.Apply();

        // Calculate the pivot point in the new texture's coordinate space
        Vector2 pivot = originalSprite.pivot;
        pivot.x /= spriteRect.width;
        pivot.y /= spriteRect.height;

        // Create new sprite with proper settings
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, width, height),
            pivot,
            originalSprite.pixelsPerUnit,
            0,
            SpriteMeshType.FullRect,
            originalSprite.border
        );

        newSprite.name = originalSprite.name + " (Clone)";
        sr.sprite = newSprite;

        return newTexture;
    }

    void UpdateSpriteFromTexture(SpriteRenderer sr, Texture2D tex)
    {
        Sprite oldSprite = sr.sprite;
        Sprite newSprite = Sprite.Create(
            tex, 
            new Rect(0, 0, tex.width, tex.height),
            oldSprite.pivot / oldSprite.pixelsPerUnit,
            oldSprite.pixelsPerUnit
        );

        newSprite.name = oldSprite.name;
        sr.sprite = newSprite;

        if (Application.isEditor && !Application.isPlaying)
        {
            DestroyImmediate(oldSprite);
        }
        else
        {
            Destroy(oldSprite);
        }

    }

}
