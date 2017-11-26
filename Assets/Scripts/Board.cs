using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public int nPlayers = 2;

    [SerializeField]
    Color[] playerColors;

    public int boardSize = 7;

    [SerializeField]
    Tile _tilePrefab;

    [SerializeField]
    Player _playerPrefab;

    public float tileSize = 1.05f;

    List<Tile> tiles = new List<Tile>();

	void Start () {
	    if (boardSize % 2 == 0)
        {
            boardSize++;
            Debug.LogWarning("Increased board size because it must be odd");
        }
        SetupMainTiles();
        SetupStartTiles();
	}

	void SetupMainTiles()
    {
        for (int row = 0; row < boardSize; row ++)
        {
            for (int col = 0; col < boardSize; col ++)
            {
                Vector3 pos = GetPosition(row, col);
                Tile tile = Instantiate(_tilePrefab, pos, Quaternion.identity, transform);

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

    Tile[] startTiles;
    Player[] players;

    void SetupStartTiles()
    {
        startTiles = new Tile[nPlayers];
        players = new Player[nPlayers];

        if (nPlayers == 2)
        {
            int row = (boardSize - 1) / 2;

            Vector3 pos = GetPosition(row, -1);
            startTiles[0] = Instantiate(_tilePrefab, pos, Quaternion.identity, transform);
            startTiles[0].Setup(TileType.Stationary, row, -1);

            players[0] = Instantiate(_playerPrefab, startTiles[0].transform, true);
            players[0].SetColor(playerColors[0]);            
            players[0].Position(startTiles[0]);
            players[0].LookTowards(GetTile(row, 0));

            pos = GetPosition(row, boardSize);
            startTiles[1] = Instantiate(_tilePrefab, pos, Quaternion.identity, transform);
            startTiles[1].Setup(TileType.Stationary, row, boardSize);

            players[1] = Instantiate(_playerPrefab, startTiles[1].transform, true);
            players[1].SetColor(playerColors[1]);
            players[1].Position(startTiles[1]);
            players[1].LookTowards(GetTile(row, boardSize - 1));
        }
        else
        {
            throw new System.NotImplementedException("Only supports 2 payers");
        }
    }

    Vector3 GetPosition(int row, int col)
    {
        int halfSize = (boardSize - 1) / 2;
        return new Vector3((row - halfSize) * tileSize, 0, (col - halfSize) * tileSize);
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
    bool isSlidingRow;
    bool isSlidingCol;

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
        if (isSlidingCol)
        {
            int offset = this.moveOffset.x > 0 ? 1 : -1;
            for (int idx = 0; idx < moveCol.Length; idx++)
            {
                Point pt = moveCol[idx]
                    .GetRelativePoint(0, offset)
                    .Wrapped(boardSize, boardSize);
                Vector3 targetPos = GetPosition(pt.row, pt.col);
                moveCol[idx].MoveTo(pt.row, pt.col, targetPos);
            }
        } else if (isSlidingRow)
        {
            int offset = this.moveOffset.x > 0 ? 1 : -1;
            for (int idx = 0; idx < moveRow.Length; idx++)
            {
                Point pt = moveRow[idx]
                    .GetRelativePoint(offset, 0)
                    .Wrapped(boardSize, boardSize);
                Vector3 targetPos = GetPosition(pt.row, pt.col);
                moveRow[idx].MoveTo(pt.row, pt.col, targetPos);
            }
        }
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
            isSlidingCol = false;
            isSlidingRow = false;

            if (slideIsRow && canSlideRow && Mathf.Abs(offset.z) > noSlideThreshold * tileSize)
            {
                Sliding(moveRow);
                ResetSliding(moveCol);
                isSlidingRow = true;
            }
            else if (!slideIsRow && canSlideCol && Mathf.Abs(offset.x) > noSlideThreshold * tileSize)
            {
                Sliding(moveCol);
                ResetSliding(moveRow);
                isSlidingCol = true;
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
