using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class Sketchbook : MonoBehaviour
{
    // can change this to be hex or smthing
    
    private bool eraserMode;
    private Color lineColor; 
    private float lineWidth;
    private Vector2? previousDrawPosition;
    private Texture2D clonedTexture; 

    Transform clickedObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eraserMode = false;
        lineColor = Color.black;
        lineWidth = 1.0f;
        previousDrawPosition = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // getting obj that is hit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                clickedObj = hit.transform;

                if (clickedObj.tag == "Color")
                {
                    eraserMode = false;
                    lineColor = clickedObj.gameObject.GetComponent<SpriteRenderer>().color;
                }

                else if (clickedObj.tag == "Size")
                {
                    lineWidth = float.Parse(clickedObj.name);
                    Debug.Log($"Width: {lineWidth}");
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

                    if (clonedTexture == null || sr.sprite.texture != clonedTexture)
                    {
                        clonedTexture = GetOrCreateTextureClone(sr);
                    }

                    Vector2 localPos = sr.transform.InverseTransformPoint(hit.point);
                    Rect spriteRect = sr.sprite.rect;
                    Vector2 pixelPos = new Vector2(
                        (localPos.x + sr.sprite.bounds.extents.x) / sr.sprite.bounds.size.x * spriteRect.width,
                        (localPos.y + sr.sprite.bounds.extents.y) / sr.sprite.bounds.size.y * spriteRect.height
                    );

                    Vector2 currentPos = new Vector2(
                        Mathf.Clamp(pixelPos.x + spriteRect.x, 0, clonedTexture.width - 1),
                        Mathf.Clamp(pixelPos.y + spriteRect.y, 0, clonedTexture.height - 1)
                    );

                    Debug.Log($"Drawing at: {currentPos} (Texture: {clonedTexture.width}x{clonedTexture.height})");

                    int baseCellWidth = Mathf.FloorToInt(clonedTexture.width / 7.0f);
                    int baseCellHeight = Mathf.FloorToInt(clonedTexture.height / 5.0f);

                    //var colnum = Mathf.Floor(currentPos.x / (clonedTexture.width / 7f));
                    //var rownum = Mathf.Floor(currentPos.y / (clonedTexture.height / 5f));

                    var colnum = Mathf.Floor(currentPos.x / baseCellWidth);
                    var rownum = Mathf.Floor(currentPos.y / baseCellHeight);

                    Color[] pixels = clonedTexture.GetPixels();

                    float sizeMultiplier = Mathf.Min(1.0f, lineWidth);
                    //int squareWidth = Mathf.FloorToInt(Mathf.FloorToInt(clonedTexture.width / 7f) * sizeMultiplier);
                    //int squareHeight = Mathf.FloorToInt(Mathf.FloorToInt(clonedTexture.height / 5f) * sizeMultiplier);

                    int squareWidth = Mathf.FloorToInt(baseCellWidth * sizeMultiplier);
                    int squareHeight = Mathf.FloorToInt(baseCellHeight * sizeMultiplier);

                    int centerX = Mathf.FloorToInt(colnum * baseCellWidth + baseCellWidth / 2);
                    int centerY = Mathf.FloorToInt(rownum * baseCellHeight + baseCellHeight / 2);

                    //int startX = (int)(colnum * squareWidth);
                    //int startY = (int)(rownum * squareHeight);
                    int startX = centerX - squareWidth / 2;
                    int startY = centerY - squareHeight / 2;

                    //for (int i = (int)(tex.width / 7 * colnum); i < (int)(tex.width / 7 * (colnum + 1)); i++)
                    //{
                    //    for (int j = (int)(tex.height / 5 * rownum); j < (int)(tex.height / 5 * (rownum + 1)); j++)
                    //    {

                    //        int pos = (int)((j * tex.width) + i);

                    //        if (pos >= 0 && pos < pixels.Length - 1)
                    //        {

                    //            pixels[pos] = Color.red;
                    //        }
                    //    }
                    //}

                    for (int i = startX; i < startX + squareWidth; i++)
                    {
                        for (int j = startY; j < startY + squareHeight; j++)
                        {
                            int pos = j * clonedTexture.width + i;
                            if (pos >= 0 && pos < pixels.Length)
                            {
                                pixels[pos] = eraserMode ? Color.white : lineColor;

                            }
                        }
                    }

                    clonedTexture.SetPixels(pixels);
                    clonedTexture.Apply();

                }

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            previousDrawPosition = null; 
        }
    }
    private Texture2D GetOrCreateTextureClone(SpriteRenderer sr)
    {        
        // Check if we already have a usable texture
        if (clonedTexture != null && sr.sprite.texture == clonedTexture)
        {
            return clonedTexture;
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

    void DrawLine(Texture2D tex, Vector2 start, Vector2 end)
    {
        int brushSize = Mathf.Max(1, (int)lineWidth);
        Color drawColor = eraserMode ? Color.white : lineColor;

        // Bresenham's line algorithm
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Draw circle at each point along the line
            DrawCircle(tex, new Vector2(x0, y0));

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    void DrawCircle(Texture2D tex, Vector2 center)
    {
        int brushSize = Mathf.Max(1, (int)lineWidth);
        Color drawColor = eraserMode ? Color.white : lineColor;

        for (int x = (int)center.x - brushSize; x <= (int)center.x + brushSize; x++)
        {
            for (int y = (int)center.y - brushSize; y <= (int)center.y + brushSize; y++)
            {
                if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                {
                    float dist = Vector2.Distance(center, new Vector2(x, y));
                    if (dist <= brushSize)
                    {
                        tex.SetPixel(x, y, drawColor);
                    }
                }
            }
        }
    }

}
