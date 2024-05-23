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

    GoBoard goBoard;

    public List<GameObject> stones;

    GameObject cursorStone;

    int count = 0;

    void Start()
    {
        goBoard = new GoBoard(new StoneType[15, 15]);
        cursorStone = Instantiate(stones[2], transform);
    }

    void Update()
    {
        (int x, int y) gridIndex = MousePosToGridIndex(Input.mousePosition);

        if(gridIndex.x >= 0 && gridIndex.y >= 0 && gridIndex.x < 15 && gridIndex.y < 15){
            if (goBoard.m_Grid[gridIndex.x, gridIndex.y] == StoneType.Empty) {
                // goBoard.m_Grid[gridIndex.x, gridIndex.y] = StoneType.Black;
                cursorStone.SetActive(true);
                cursorStone.transform.position = new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f);
                // stone = Instantiate(stones[(int)StoneType.Black - 1], new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f), Quaternion.identity);
                // cursorStone.transform.parent = gameObject.transform;
                // Debug.Log("[" + gridIndex.x + "][" + gridIndex.y + "]");
            }
        }
        else {
            cursorStone.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0) && goBoard.m_Grid[gridIndex.x, gridIndex.y] == StoneType.Empty) {
            if (count % 2 == 0) {
                goBoard.m_Grid[gridIndex.x, gridIndex.y] = StoneType.Black;
                GameObject stone = Instantiate(stones[(int)StoneType.Black - 1], new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f), Quaternion.identity);
            }
            else {
                goBoard.m_Grid[gridIndex.x, gridIndex.y] = StoneType.White;
                GameObject stone = Instantiate(stones[(int)StoneType.White - 1], new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f), Quaternion.identity);
            }
            count++;
        }
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
