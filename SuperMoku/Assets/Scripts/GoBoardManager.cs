using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

enum StoneType
{
    Empty,
    Black,
    White
}

enum OccupyingPlayer
{
    None,
    First,
    Second
}

class GoBoard
{
    public StoneType[,] m_Grid;
    public OccupyingPlayer[,] m_Occup;

    public GoBoard()
    {
        // m_Grid = InitGrid();
        m_Grid = new StoneType[15, 15];
        m_Occup = new OccupyingPlayer[15, 15];
    }

    /*
    private StoneType[,] InitGrid()
    {
        StoneType[,] grid = new StoneType[15, 15];

        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                grid[i, j] = StoneType.Empty;
            }
        }

        return grid;
    }
    */
}

public class GoBoardManager : MonoBehaviour
{
    GoBoard goBoard;

    public List<GameObject> stones;
    public GameObject cursorStone;

    public List<GameObject> flags;

    TurnManager _TurnManager;

    void Start()
    {
        goBoard = new GoBoard();
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

            CheckGomoku(goBoard.m_Grid, goBoard.m_Occup);
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

            if (_TurnManager.CurrentTurn == TurnManager.Turn.First) {
                cursorStone.GetComponent<MeshRenderer>().material.color = new Color(0f, 0f, 0f, 0.25f);
            }
            else if (_TurnManager.CurrentTurn == TurnManager.Turn.Second) {
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

    private void CheckGomoku(StoneType[,] grid, OccupyingPlayer[,] occup)
    {
        int xOffset;
        int yOffset;

        for (int x=0; x<15; x++) {
            for (int y = 0; y < 15; y++) {
                StoneType curStone = grid[x, y];

                /*
                if (curStone == StoneType.Empty || IsAlreadyOccupied(curStone, occup, x, y)) {
                    continue;
                }
                */

                if (curStone == StoneType.Empty) {
                    continue;
                }

                #region Vertical
                // »óÇÏ
                int vCount = 1;
                yOffset = 0;
                while (y + (++yOffset) < 15) {
                    if (grid[x, y + yOffset] == curStone) {
                        vCount++;
                    }
                    else {
                        break;
                    }
                }
                yOffset = 0;
                while (y - (++yOffset) >= 0) {
                    if (grid[x, y - yOffset] == curStone) {
                        vCount++;
                    }
                    else {
                        break;
                    }
                }

                if (vCount >= 5) {
                    Debug.Log("VEREITCAL GOMOKU!");
                    occup[x, y] = (OccupyingPlayer)curStone;
                    Occupying(occup, x, y);
                    continue;
                }
                #endregion

                #region Horizontal
                // ÁÂ¿ì
                int hCount = 1;
                xOffset = 0;
                while (x + (++xOffset) < 15) {
                    if (grid[x + xOffset, y] == curStone) {
                        hCount++;
                    }
                    else {
                        break;
                    }
                }
                xOffset = 0;
                while (x - (++xOffset) >= 0) {
                    if (grid[x - xOffset, y] == curStone) {
                        hCount++;
                    }
                    else {
                        break;
                    }
                }

                if (hCount >= 5) {
                    Debug.Log("HORIZONTAL GOMOKU!");
                    occup[x, y] = (OccupyingPlayer)curStone;
                    Occupying(occup, x, y);
                    continue;
                }
                #endregion

                #region Left Diagonal
                // ÁÂ´ë°¢
                int ldCount = 1;
                xOffset = 0;
                yOffset = 0;
                while (x + (++xOffset) < 15 && y - (++yOffset) >= 0) {
                    if (grid[x + xOffset, y - yOffset] == curStone) {
                        ldCount++;
                    }
                    else {
                        break;
                    }
                }
                xOffset = 0;
                yOffset = 0;
                while (x - (++xOffset) >= 0 && y + (++yOffset) < 15) {
                    if (grid[x - xOffset, y + yOffset] == curStone) {
                        ldCount++;
                    }
                    else {
                        break;
                    }
                }

                if (ldCount >= 5) {
                    Debug.Log("LEFT DIAGONAL GOMOKU!");
                    occup[x, y] = (OccupyingPlayer)curStone;
                    Occupying(occup, x, y);
                    continue;
                }
                #endregion

                #region Right Diagonal
                // ¿ì´ë°¢
                int rdCount = 1;
                xOffset = 0;
                yOffset = 0;
                while (x + (++xOffset) < 15 && y + (++yOffset) < 15) {
                    if (grid[x + xOffset, y + yOffset] == curStone) {
                        rdCount++;
                    }
                    else {
                        break;
                    }
                }
                xOffset = 0;
                yOffset = 0;
                while (x - (++xOffset) >= 0 && y - (++yOffset) >= 0) {
                    if (grid[x - xOffset, y - yOffset] == curStone) {
                        rdCount++;
                    }
                    else {
                        break;
                    }
                }

                if (rdCount >= 5) {
                    Debug.Log("RIGHT DIAGONAL GOMOKU!");
                    occup[x, y] = (OccupyingPlayer)curStone;
                    Occupying(occup, x, y);
                    continue;
                }
                #endregion
            }
        }
    }

    private void Occupying(OccupyingPlayer[,] occup, int x, int y)
    {
        if (occup[x, y] != OccupyingPlayer.None) {
            GameObject flag = Instantiate(flags[(int)occup[x,y] - 1], new Vector3((x - 7) / 15f * 10f, 0.05f, (y - 7) / 15f * 10f), Quaternion.identity);
        }
        
    }

    /*
    private bool IsAlreadyOccupied(StoneType curStone, OccupyingPlayer[,] occup, int x, int y)
    {
        OccupyingPlayer curPlayer = (OccupyingPlayer)curStone;

        if (y + 1 < 15) {
            if(curPlayer == occup[x, y + 1]) {

            }
        } 

        if(curPlayer == occup[x, y+1] || curPlayer == occup[x, y-1]
            || curPlayer == occup[x+1, y] || curPlayer == occup[x-1, y]
            || curPlayer == occup[x+1, y-1] || curPlayer == occup[x-1, y+1]
            || curPlayer == occup[x+1, y+1] || curPlayer == occup[x-1, y-1])
        {
            occup[x, y] = curPlayer;
            Debug.Log("x : " + x + " / y : " + y + " is already occupied.");
            return true;
        }

        return false;
    }
    */
}
