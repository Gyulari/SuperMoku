using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

enum StoneType
{
    Empty,
    Black,
    White
}

class GoBoard
{
    public StoneType[,] m_Grid;

    public GoBoard(StoneType[,] grid)
    {
        m_Grid = grid;
    }
}

public class GoBoardManager : MonoBehaviour
{
    Vector3 mousePos;

    void Start()
    {
        GoBoard goBoard = new GoBoard(new StoneType[15, 15]);
    }

    void Update()
    {
        (int x, int y) gridIndex = MousePosToGridIndex(Input.mousePosition);

        Debug.Log("[" + gridIndex.x + "][" + gridIndex.y + "]");
    }

    private (int xIdx, int yIdx) MousePosToGridIndex(Vector3 mousePos)
    {
        float boardCameraRatio = 10f * gameObject.transform.localScale.x / Camera.main.transform.position.y;

        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));

        int x = Mathf.CeilToInt((mousePos.x + 0.15f * boardCameraRatio) / (0.02f * boardCameraRatio));
        int y = Mathf.CeilToInt((mousePos.z + 0.15f * boardCameraRatio) / (0.02f * boardCameraRatio));

        return (x-1, y-1);
    }
}
