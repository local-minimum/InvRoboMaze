using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public int players = 2;

    public int boardSize = 7;

    [SerializeField]
    Tile _prefab;

    public float tileSize = 1.05f;

    List<Tile> tiles = new List<Tile>();

	void Start () {
	    if (boardSize % 2 == 0)
        {
            boardSize++;
            Debug.LogWarning("Increased board size because it must be odd");
        }
        SetupTiles();
	}

	void SetupTiles()
    {
        int halfSize = (boardSize - 1) / 2;

        for (int row = 0; row < boardSize; row ++)
        {
            for (int col = 0; col < boardSize; col ++)
            {
                Vector3 pos = new Vector3((row - halfSize) * tileSize, 0, (col - halfSize) * tileSize);
                Tile tile = Instantiate(_prefab, pos, Quaternion.identity, transform);

                tile.Setup(
                    (row % 2) == 1 && (col % 2) == 1 ? TileType.Swapping : TileType.Sliding,
                    row,
                    col);

                tile.onMove += Tile_onMove;
                tile.onMoveEnd += Tile_onMoveEnd;
                tile.onMoveStart += Tile_onMoveStart;

                tiles.Add(tile);
            }
        }
    }

    bool sourceCanMove;
    int sourceRow;
    int sourceCol;
    Tile source;
    Tile target;
    Vector3 moveOffset;
    Tile[] moveRow = new Tile[0];
    Tile[] moveCol = new Tile[0];
    float noSlideThreshold = 0.33f;
    bool canSlideRow;
    bool canSlideCol;
    bool canSlide;

    private void Tile_onMoveStart(int row, int col)
    {
        source = GetTile(row, col);
        sourceCanMove = source.canSlide || source.canSwap;
        sourceRow = row;
        sourceCol = col;
        source.GetComponent<MeshRenderer>().material.color = Color.yellow;
        if (source.canSlide)
        {
            moveRow = GetRowTiles(row);
            moveCol = GetColTiles(col);
            canSlideCol = CanSlide(moveCol);
            canSlideRow = CanSlide(moveRow);
            canSlide = canSlideCol || canSlideRow;
        }
    }

    private void Tile_onMoveEnd()
    {
        SetNewTilePositions();
        ClearMoveSource();
        if (source != null)
        {
            source.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        
    }

    void SetNewTilePositions()
    {
 
    }

    void ClearMoveSource()
    {
        sourceCanMove = false;
        sourceRow = 0;
        sourceCol = 0;
        if (canSlide)
        {
            ResetSliding(moveCol);
            ResetSliding(moveRow);
        }
        moveRow = new Tile[0];
        moveCol = new Tile[0];
        canSlideRow = false;
        canSlideCol = false;
        canSlide = false;
    }

    private void Tile_onMove(Vector3 offset)
    {
        moveOffset = offset;
        if (canSlide)
        {
            bool slideIsRow = Mathf.Abs(offset.x) < Mathf.Abs(offset.z);

            if (slideIsRow && canSlideRow && Mathf.Abs(offset.z) > noSlideThreshold * tileSize)
            {
                Sliding(moveRow);
                ResetSliding(moveCol);
            }
            else if (!slideIsRow && canSlideCol && Mathf.Abs(offset.x) > noSlideThreshold * tileSize)
            {
                Sliding(moveCol);
                ResetSliding(moveRow);
            }
        }
    }

    void Sliding(Tile[] tiles)
    {
        for(int i=0; i<tiles.Length; i++)
        {
            Tile t = tiles[i];
            if (t != source)
            {
                t.GetComponent<MeshRenderer>().material.color = Color.magenta;
            }
        }
    }

    void ResetSliding(Tile[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i];
            if (t != source)
            {
                t.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public Tile GetTile(int row, int col)
    {
        foreach(Tile t in tiles)
        {
            if (t.hasPosition(row, col))
            {
                return t;
            }
        }
        return null;
    }

    public Tile[] GetRowTiles(int row)
    {
        Tile[] tiles = new Tile[boardSize];
        int curTile = 0;

        foreach(Tile tile in this.tiles)
        {
            if (tile.isOnRow(row))
            {
                tiles[curTile] = tile;
                curTile++;
            }
        }
        return tiles;
    }

    public Tile[] GetColTiles(int row)
    {
        Tile[] tiles = new Tile[boardSize];
        int curTile = 0;

        foreach (Tile tile in this.tiles)
        {
            if (tile.isOnCol(row))
            {
                tiles[curTile] = tile;
                curTile++;
            }
        }
        return tiles;
    }

    public static bool CanSlide(Tile[] tiles)
    {
        for (int i=0; i<tiles.Length; i++)
        {
            if (!tiles[i].canSlide)
            {
                return false;
            }
        }
        return true;
    }

}
