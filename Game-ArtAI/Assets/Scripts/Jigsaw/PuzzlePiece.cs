using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    // -1 = indent
    // 0 = straight edge
    // 1 = tab
    public int bottom;
    public int right;
    public int top;
    public int left;
    private Vector3 finishedPos;
    private GameObject puzzleObj;

    public PuzzlePiece(int bottom, int right, int top, int left)
    {
        this.bottom = bottom;
        this.right = right;
        this.top = top;
        this.left = left;
        this.finishedPos = Vector3.zero;
        this.puzzleObj = null;
    }
    public void SnapCurrPosToFinish()
    {
        this.puzzleObj.transform.position = this.finishedPos;
    }
    public Vector3 GetCurrPos()
    {
        if (this.puzzleObj == null)
        {
            Debug.Log("null object");
            return Vector3.zero;
        }
        //return this.puzzleObj.transform.localPosition;
        return this.puzzleObj.transform.position;
    }
    public Vector3 GetFinishedPos()
    {
        return this.finishedPos;
    }
    
    public void SetGameObject(GameObject gameObj)
    {
        this.puzzleObj = gameObj;
    }

    public void SetFinishedPos(Vector3 position)
    {
        this.finishedPos = position;
    }
}
