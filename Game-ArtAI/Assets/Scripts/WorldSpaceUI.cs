using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldSpaceUI : MonoBehaviour
{
    const string k_transparentShader = "Unlit/Transparent";
    const string k_textureShader = "Unlit/Texture";
    const string k_mainTex = "_MainTex";
    static readonly int MainTex = Shader.PropertyToID(k_mainTex);

    [SerializeField] int panelW = 720;
    [SerializeField] int panelH = 720;
    [SerializeField] float panelScale = 1.0f;
    [SerializeField] float pixelsPerUnit = 500.0f;
    [SerializeField] VisualTreeAsset visualTreeAsset;
    [SerializeField] PanelSettings panelSettingsAsset;
    [SerializeField] RenderTexture renderTextureAsset;

    MeshRenderer meshRenderer;
    UIDocument uiDocument;
    PanelSettings panelSettings;
    RenderTexture renderTexture;
    Material material; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    InitializeComponents();
    //    BuildPanel();   
    //}

    private void Awake()
    {
        InitializeComponents();
        BuildPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLabelText(string label, string text)
    {
        if (uiDocument.rootVisualElement == null)
        {
            uiDocument.visualTreeAsset = visualTreeAsset;
        }

        uiDocument.rootVisualElement.Q<Label>(label).text = text;
    }

    void BuildPanel()
    {
        CreateRenderTexture();  
        CreatePanelSettings();
        CreateUIDocument();
        CreateMaterial();

        SetMaterialToRenderer();
        SetPanelSize();
    }

    void SetMaterialToRenderer()
    {
        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial = material;
        }
    }

    void SetPanelSize()
    {
        if (renderTexture != null && (renderTexture.width != panelW || renderTexture.height != panelH))
        {
            renderTexture.Release();
            renderTexture.width = panelW; 
            renderTexture.height = panelH;
            renderTexture.Create();

            uiDocument.rootVisualElement?.MarkDirtyRepaint();
        }

        transform.localScale = new Vector3(panelW / pixelsPerUnit, panelH / pixelsPerUnit, 1.0f);
    }

    void CreateMaterial()
    {
        string shaderName = panelSettings.colorClearValue.a < 1.0f ? k_transparentShader : k_textureShader;
        //material = new Material(Shader.Find("UI/Default"));
        material = new Material(Shader.Find(shaderName));
        material.SetTexture(MainTex, renderTexture);
    }

    void CreateUIDocument()
    {
        uiDocument = gameObject.GetOrAddComponent<UIDocument>();
        uiDocument.panelSettings = panelSettings;
        uiDocument.visualTreeAsset = visualTreeAsset;
    }

    void CreatePanelSettings()
    {
        panelSettings = Instantiate(panelSettingsAsset);
        panelSettings.targetTexture = renderTexture;
        panelSettings.clearColor = true;
        panelSettings.scaleMode = PanelScaleMode.ConstantPhysicalSize;
        panelSettings.scale = panelScale;
        panelSettings.name = $"{name} - PanelSettings";
    }

    void CreateRenderTexture()
    {
        RenderTextureDescriptor descriptor = renderTextureAsset.descriptor;
        descriptor.width = panelW;
        descriptor.height = panelH;
        renderTexture = new RenderTexture(descriptor)
        {
            name = $"{name} - RenderTexture"
        };
    }

    public void InitializeComponents()
    {
        InitializeMeshRenderer();

        // can create a box collider 
        //BoxCollider boxCollider = gameObject.GetOrAddComponent<BoxCollider>();
        //boxCollider.size = new Vector3(1, 1, 0);

        MeshFilter meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
        meshFilter.sharedMesh = GetQuadMesh();
    }

    public void InitializeMeshRenderer()
    {
        meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = null;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion; 
        meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
    }

    public static Mesh GetQuadMesh()
    {
        GameObject tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Mesh quadMesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;
        Destroy(tempQuad);

        return quadMesh;
    }
}
