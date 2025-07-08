using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class Sketchbook : MonoBehaviour
{
    // can change this to be hex or smthing

    [SerializeField]
    GameObject options;
    [SerializeField]
    GameObject sizes;
    [SerializeField]
    GameObject palette;


    private bool eraserMode;
    private UnityEngine.Color lineColor;
    private float lineWidth;
    private Vector2? previousDrawPosition;
    private Texture2D clonedTexture;

    private int sketchNum;

    private bool isLoad;
    private Texture2D loadText;

    Transform clickedObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eraserMode = false;
        lineColor = UnityEngine.Color.black;
        lineWidth = 1.0f;
        previousDrawPosition = null;
        sketchNum = 0;
        isLoad = false;

        // visual feedback 

        // setting eraser as inactive
        optionFeedback(false, options.transform.GetChild(0));

        // setting width 

        // setting color
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
#if UNITY_EDITOR
            SaveTextureToAssets(clonedTexture, $"Sketchbook_{sketchNum}");
#else
            SaveTextureRuntime(clonedTexture, $"Sketchbook_{sketchNum}");
#endif
            sketchNum++;
        }

        // hardcoding for time being for testing
        if (Input.GetKeyDown(KeyCode.L))
        {
#if UNITY_EDITOR
            // Assets/Textures/Sketchbook_0.png
            string path = $"Assets/Textures/Sketchbook_{0}.png";
            loadText = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (loadText == null)
            {
                Debug.Log("not loading!!");
            }
            else
            {
                clonedTexture = CloneTexture(loadText);

                GameObject sketchObj = GameObject.FindWithTag("Sketchbook");
                if (sketchObj == null)
                {
                    Debug.Log("can't find sketchbook");
                }
                else
                {
                    SpriteRenderer sr = sketchObj.GetComponent<SpriteRenderer>();

                    Sprite newSprite = Sprite.Create(
                        clonedTexture,
                        new Rect(0, 0, clonedTexture.width, clonedTexture.height),
                        new Vector2(0.5f, 0.5f),
                        sr.sprite.pixelsPerUnit
                    );

                    sr.sprite = newSprite;

                }

            }


#else
            string path = $"Assets/Textures/Sketchbook_{0}.png";
            clonedTexture = LoadTextureFromDisk(string path);
#endif
        }

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

                        optionFeedback(true, options.transform.GetChild(0));
                        optionFeedback(false, options.transform.GetChild(1));

                    }
                    else
                    {
                        eraserMode = false;

                        optionFeedback(false, options.transform.GetChild(0));
                        optionFeedback(true, options.transform.GetChild(1));
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

                    if (clonedTexture == null /*|| sr.sprite.texture != clonedTexture*/)
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

                    int baseCellWidth = 50;

                    UnityEngine.Color[] pixels = clonedTexture.GetPixels();

                    DrawSquare(baseCellWidth, currentPos, pixels);

                    if (previousDrawPosition != null)
                    {

                        ConnectStroke(currentPos, (Vector2)previousDrawPosition, baseCellWidth, pixels);

                    }

                    previousDrawPosition = currentPos;

                    // connect curr square & prev pos w/ bresenham algorithm 
                    // draw square there 

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

    Texture2D CloneTexture(Texture2D source)
    {
        // Create a RenderTexture with the same dimensions
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

        // Blit the source texture to the render texture
        Graphics.Blit(source, rt);

        // Backup the currently active RenderTexture
        RenderTexture previous = RenderTexture.active;

        // Set the active RenderTexture to the temporary one we created
        RenderTexture.active = rt;

        // Create a new readable Texture2D and read the pixels from the RenderTexture
        Texture2D readableTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        readableTex.Apply();

        // Restore the previous active RenderTexture
        RenderTexture.active = previous;

        // Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(rt);

        return readableTex;
    }


    Texture2D GetOrCreateTextureClone(SpriteRenderer sr)
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
        UnityEngine.Color[] pixels = originalTexture.GetPixels(
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

    public void SaveTextureToAssets(Texture2D texture, string fileName = "SavedTexture")
    {
#if UNITY_EDITOR
        // Convert texture to PNG byte array
        byte[] bytes = texture.EncodeToPNG();

        // Create the path (Assets folder by default)
        string folderPath = "Assets/Textures/";
        string filePath = Path.Combine(folderPath, fileName + ".png");

        // Create directory if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Write the file
        File.WriteAllBytes(filePath, bytes);

        // Refresh the asset database
        AssetDatabase.Refresh();

        Debug.Log($"Texture saved to: {filePath}");
#else
        Debug.LogWarning("Texture saving is only available in the Unity Editor");
#endif
    }

    // need to test these
    public void SaveTextureRuntime(Texture2D texture, string fileName = "SavedTexture")
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log($"Texture saved to: {filePath}");
    }

    public Texture2D LoadTextureFromDisk(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Texture file not found: " + path);
            return null;
        }

        byte[] data = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(2, 2); // size will be replaced by LoadImage
        tex.LoadImage(data); // PNG/JPG will load correctly
        return tex;
    }

    void DrawSquare(int cellSize, Vector2 currentPos, UnityEngine.Color[] px)
    {
        float sizeMultiplier = Mathf.Min(1.0f, lineWidth);

        int squareWidth = Mathf.FloorToInt(cellSize * sizeMultiplier);
        int squareHeight = Mathf.FloorToInt(cellSize * sizeMultiplier);

        int startX = (int)(currentPos.x - (squareWidth / 2));
        int startY = (int)(currentPos.y - (squareHeight / 2));

        for (int i = startX; i < startX + squareWidth /*/ 2*/; i++)
        {
            for (int j = startY; j < startY + squareHeight /*/ 2*/; j++)
            {
                int pos = j * clonedTexture.width + i;
                if (pos >= 0 && pos < px.Length)
                {
                    px[pos] = eraserMode ? UnityEngine.Color.white : lineColor;

                }
            }
        }
    }

    void ConnectStroke(Vector2 squareOne, Vector2 squareTwo, int size, UnityEngine.Color[] textPx)
    {

        float distance = Vector2.Distance(squareOne, squareTwo);
        int steps = Mathf.CeilToInt(distance / (size * 0.25f)); // draw enough squares to fill the gap

        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 interpolated = Vector2.Lerp(squareOne, squareTwo, t);
            DrawSquare(size, interpolated, textPx);
        }

    }

    void optionFeedback(bool activity, Transform tool)
    {
        var hex = "#4A4A4A";
        if (activity)
        {
            hex = "#FFFFFF";
        }

        UnityEngine.Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            tool.gameObject.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
