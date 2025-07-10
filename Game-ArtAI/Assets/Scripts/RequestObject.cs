using UnityEngine;

public class RequestObject : MonoBehaviour
{
    private int colorIndex;     // emoji
    private int patternIndex;   // animal
    private int thirdIndex;     // shape
    private int requestSize;

    private int numMats = 0; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RequestObject(int colorIndex, int patternIndex, int thirdIndex, int requestSize)
    {
        this.colorIndex = colorIndex;
        this.patternIndex = patternIndex;
        this.thirdIndex = thirdIndex;
        this.requestSize = requestSize;
    }

    public RequestObject(int colorIndex, int patternIndex, int thirdIndex, int requestSize, int mats)
    {
        this.colorIndex = colorIndex;
        this.patternIndex = patternIndex;
        this.thirdIndex = thirdIndex;
        this.numMats = mats;
    }

    public int getColorIndex()
    {
        return this.colorIndex;
    }
    public int getPatternIndex()
    {
        return this.patternIndex;
    }
    public int getThirdIndex()
    {
        return this.thirdIndex;
    }
    public int getSize()
    {
        return this.requestSize;
    }
    public int getMats()
    {
        return this.numMats;
    }
    public string toString()
    {
        return "Color: " + this.colorIndex +
            "; Pattern: " + this.patternIndex +
            "; Third: " + this.thirdIndex +
            "; Size: " + this.requestSize;
    }
}
