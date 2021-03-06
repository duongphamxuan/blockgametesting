using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject[] tilePrefab;
    public GameObject[,] allDots;
    BackgroundTile[,] allTiles;
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width,height];
        SetUp();
    }

    public void SetUp()
    {
        for (int i = 0; i < width; i++ ) 
        {
            for (int j = 0; j < height; j++ )
            {
                Vector2 tempPosition = new Vector2(i,j);
                int tileIndex = UnityEngine.Random.Range(0, tilePrefab.Length);
                int maxIterations = 0; //check how many time to call re-MatchesAt
                while(MatchesAt(i,j,tilePrefab[tileIndex]) && maxIterations < 100)
                {
                    // If have matched, restart chosing Object
                    tileIndex = UnityEngine.Random.Range(0, tilePrefab.Length);
                    maxIterations++;
                }
                GameObject tile = Instantiate(tilePrefab[tileIndex], tempPosition, Quaternion.identity) as GameObject;
                tile.transform.parent = this.transform; //put all instantiate inside tile
                tile.name = "( " + i + ", " + j + " )" ; //name Instantiate
                allDots[i,j] = tile;
            }
        }
    }


    
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        // This function to ensure that have no matching when Start
        if(column > 1 && row > 1)
        {
            if(allDots[column-1, row].tag == piece.tag && allDots[column-2, row].tag == piece.tag)
            {
                return true;
            }
            else if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag) 
            {
                return true;
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column,row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column,row]);
            allDots[column,row] = null;
        }
    }
}
