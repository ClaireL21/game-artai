using UnityEngine;

public class RequestObject : MonoBehaviour
{
    private int patternIndex;
    private int colorIndex;
    private int thirdIndex;
    private int requestSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RequestObject(int patternIndex, int colorIndex, int thirdIndex, int requestSize)
    {
        this.patternIndex = patternIndex;
        this.colorIndex = colorIndex;
        this.thirdIndex = thirdIndex;
        this.requestSize = requestSize;
    }

    public int getPatternIndex()
    {
        return this.patternIndex;
    }
    public int getColorIndex()
    {
        return this.colorIndex;
    }
    public int getThirdIndex()
    {
        return this.thirdIndex;
    }
}
