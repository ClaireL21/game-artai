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

    public PuzzlePiece(int bottom, int right, int top, int left)
    {
        this.bottom = bottom;
        this.right = right;
        this.top = top;
        this.left = left;
    }
}
