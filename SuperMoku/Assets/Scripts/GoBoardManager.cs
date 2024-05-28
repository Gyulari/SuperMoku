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
    GoBoard goBoard;

    public List<GameObject> stones;
    public GameObject cursorStone;

    TurnManager _TurnManager;

    void Start()
    {
        goBoard = new GoBoard(new StoneType[15, 15]);
        _TurnManager = new TurnManager();

        cursorStone = Instantiate(cursorStone, transform);
    }

    void Update()
    {
        (int x, int y) gridIndex = MousePosToGridIndex(Input.mousePosition);

        ActivateCursorStone(gridIndex);

        if (Input.GetMouseButtonDown(0) && goBoard.m_Grid[gridIndex.x, gridIndex.y] == StoneType.Empty) {
            goBoard.m_Grid[gridIndex.x, gridIndex.y] = (StoneType)(_TurnManager.CurrentTurn + 1);            
            GameObject stone = Instantiate(stones[(int)_TurnManager.CurrentTurn], new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f), Quaternion.identity);

            _TurnManager.ChangeTurn();
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

    private void ActivateCursorStone((int x, int y) gridIndex)
    {
        if (ValidateGridIndex(gridIndex) && goBoard.m_Grid[gridIndex.x, gridIndex.y] == StoneType.Empty) {
            cursorStone.SetActive(true);
            cursorStone.transform.position = new Vector3((gridIndex.x - 7) / 15f * 10f, 0.05f, (gridIndex.y - 7) / 15f * 10f);

            if (_TurnManager.CurrentTurn == TurnManager.Turn.Black) {
                cursorStone.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            }
            else if (_TurnManager.CurrentTurn == TurnManager.Turn.White) {
                cursorStone.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.25f);
            }
        }
        else {
            cursorStone.SetActive(false);
        }
    }

    private bool ValidateGridIndex((int x, int y) gridIndex)
    {
        if (gridIndex.x >= 0 && gridIndex.y >= 0 && gridIndex.x < 15 && gridIndex.y < 15)
            return true;
        else
            return false;
    }
}
