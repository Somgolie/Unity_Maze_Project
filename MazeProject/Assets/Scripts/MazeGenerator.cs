
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    private MazeCell[,] _mazeGrid;

    private int cellnumber;
    IEnumerator Start()
    {
        cellnumber = 0;


        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];



        for (int x = 0; x < _mazeWidth; x+=1)
        {

            for (int z = 0; z < _mazeDepth; z +=1)
            {
                cellnumber++;


                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);

                _mazeCellPrefab.cellNumber = cellnumber;
            }

        }

        yield return GenerateMaze(null, _mazeGrid[0, 0]);
    }

    //this method visits each maze cell
    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
 

        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);
        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
                
            }
        } while (nextCell != null);
    }
    //this method finds the next wall to remove, right now its random
    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
        
    }

    //see all unvisited cells
    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)(currentCell.transform.position.x);
        int z = (int)(currentCell.transform.position.z);
        //left right
        if (x + 1 < _mazeWidth)
        {
            MazeCell cellToRight = _mazeGrid[x + 1, z];
            if (cellToRight != null && cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        if (x - 1 >= 0)
        {
           MazeCell cellToLeft = _mazeGrid[x - 1, z];
            if (cellToLeft != null && cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
        //front back
        if (z + 1 < _mazeDepth)
        {
            MazeCell cellToFront = _mazeGrid[x, z+1];
            if (cellToFront != null && cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if (z - 1 >= 0)
        {
            MazeCell cellToBack = _mazeGrid[x, z-1];
            if (cellToBack != null && cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }
    //this method removes the walls to form a maze
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            Debug.Log("nulled");
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            Debug.Log("clear");
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            if (currentCell.cellNumber==(_mazeDepth * _mazeWidth)-1)
            {
                currentCell.ClearRightWall();
            }
            return;
        }
        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            Debug.Log("clear");
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            if (currentCell.cellNumber == (_mazeDepth * _mazeWidth)-1)
            {
                currentCell.ClearLeftWall();
            }
            return;
        }
        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            Debug.Log("clear");
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            if (currentCell.cellNumber == (_mazeDepth * _mazeWidth)-1)
            {
                currentCell.ClearFrontWall();
            }
            return;
        }
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            Debug.Log("clear");
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            if (currentCell.cellNumber == (_mazeDepth * _mazeWidth)-1)
            {
                currentCell.ClearBackWall();
            }
            return;
        }
    }
}
