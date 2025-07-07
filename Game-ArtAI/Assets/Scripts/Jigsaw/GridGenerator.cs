using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.HableCurve;

public class GridGenerator : MonoBehaviour
{
    [Header("Elements")]
    //[SerializeField] private GameObject spherePrefab;

    [SerializeField] private Material puzzleMaterial;

    [Header("Settings")]
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 5;
    [SerializeField] private float gridScale = 1.0f;

    private float[] columnWidths;
    private float[][] puzzlePieceHeights;
    private float[][] puzzleAccHeights;

    private PuzzlePiece[][] puzzlePieces;
    private GameObject[][] pieceObjects;

    private static MeshGenerator MG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeColumnWidths();
        InitializePuzzlePieceHeights();
        InitializePuzzlePieces();
        MG = new MeshGenerator(this.transform, this.rows, this.columns, 0.1f, 5, this.gridScale, puzzleMaterial);
        GenerateUnevenTabsGrid();

    }

    // Unit column widths, where each column by default is 1 unit wide
    // Vary column width by offsetting in the negative or positive direction by 1/(column + 1) of column width)
    private void InitializeColumnWidths()
    {
        columnWidths = new float[columns];
        for (int i = 0; i < columnWidths.Length; i++)
        {
            columnWidths[i] = 1.0f;
        }

        float totalWidth = columns;
        float offset = 1.0f / (1.0f + columns);
        for (int i = 0; i < columnWidths.Length; i++)
        {
            if (i == columnWidths.Length - 1)
            {
                columnWidths[i] = totalWidth;
            } else
            {
                float offsetVal = (float)Math.Round(UnityEngine.Random.Range(-offset, offset), 1, MidpointRounding.AwayFromZero);
                columnWidths[i] += offsetVal;
                totalWidth -= columnWidths[i];
            }
        }
    }

    // Heights of the sides of each puzzle piece, organize by "column lines"
    // There are (column + 1) column lines
    // Store column lines to ensure that adjacent puzzle pieces have the same side height
    private void InitializePuzzlePieceHeights()
    {
        puzzlePieceHeights = new float[columns + 1][];
        puzzleAccHeights = new float[columns + 1][];
        for (int i = 0; i < puzzlePieceHeights.Length; i++)
        {
            puzzlePieceHeights[i] = new float[rows];
            puzzleAccHeights[i] = new float[rows];
        }

        
        float offset = 1.0f / (1.0f + this.rows);
        for (int lineIndex = 0; lineIndex < puzzlePieceHeights.Length; lineIndex++)
        {
            float totalHeight = this.rows;
            int rows = puzzlePieceHeights[lineIndex].Length;
            float acc = 0.0f;
            for (int i = 0; i < rows; i++)
            {
                float height;

                if (i == this.rows - 1)
                {
                    height = totalHeight;
                } else
                {
                    float offsetVal = (float)Math.Round(UnityEngine.Random.Range(-offset, offset), 1, MidpointRounding.AwayFromZero);
                    height = 1 + offsetVal;
                    totalHeight -= height;
                }

                acc += height;
                puzzlePieceHeights[lineIndex][i] = height;
                puzzleAccHeights[lineIndex][i] = acc;
            }
        }
    }

    private void InitializePuzzlePieces()
    {
        puzzlePieces = new PuzzlePiece[this.rows][];
        pieceObjects = new GameObject[this.rows][];

        for (int r = 0; r < puzzlePieces.Length; r++)
        {
            puzzlePieces[r] = new PuzzlePiece[this.columns];
            pieceObjects[r] = new GameObject[this.columns];

            for (int c = 0; c < puzzlePieces[r].Length; c++)
            {
                PuzzlePiece piece = new PuzzlePiece(0, 0, 0, 0);

                // bottom
                if (r == 0)
                {
                    piece.bottom = 0;
                } else
                {
                    piece.bottom = puzzlePieces[r - 1][c].top * -1;
                }

                // left
                if (c == 0)
                {
                    piece.left = 0;
                } else
                {
                    piece.left = puzzlePieces[r][c - 1].right * -1;
                }

                // right
                if (c == puzzlePieces[r].Length - 1)
                {
                    piece.right = 0;
                } else
                {
                    piece.right = UnityEngine.Random.value < 0.5f ? -1 : 1;
                }

                // top
                if (r == puzzlePieces.Length - 1)
                {
                    piece.top = 0;
                } else
                {
                    piece.top = UnityEngine.Random.value < 0.5f ? -1 : 1;
                }

                puzzlePieces[r][c] = piece;
            }
        }
    }
    private void GenerateUnevenTabsGrid()
    {
        Vector3 startPos = Vector2.left * (gridScale * columns) / 2 + Vector2.down * (gridScale * rows) / 2;
        startPos.x += 0.5f * gridScale;
        startPos.y += 0.5f * gridScale;

        float currWidth = 0;
        float halfWidth = gridScale * columns * 0.5f;


        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                Vector3 spawnPosition = new Vector2((currWidth + columnWidths[x] * 0.5f) * gridScale - halfWidth, startPos.y + y * gridScale);
                float width = columnWidths[x];

                float heightA = puzzlePieceHeights[x][y];       // left side
                float heightB = puzzlePieceHeights[x + 1][y];   // right side

                PuzzlePiece piece = puzzlePieces[y][x];

                GameObject pieceInstance = MG.MakeTrapezoidMesh(spawnPosition, width, heightA, heightB, piece, y, x, puzzleAccHeights);
                pieceInstance.layer = LayerMask.NameToLayer("Movable");
                pieceObjects[y][x] = pieceInstance;
            }
            currWidth += columnWidths[x];
        }
        this.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);

        // Unparent:
        /*for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //pieceObjects[y][x].transform.SetParent(null);
            }
        }*/
    }
}
