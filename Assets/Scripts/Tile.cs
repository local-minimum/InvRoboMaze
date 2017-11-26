using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Sliding, Swapping, Stationary };

public delegate void MoveEvent(Vector3 offset);
public delegate void MoveStartEvent(int row, int col);
public delegate void MoveEndEvent();
public delegate void TileBusyStart(Tile tile);
public delegate void TileBusyEnd(Tile tile);

public struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int row
    {
        get
        {
            return y;
        }
    }

    public int col
    {
        get
        {
            return x;
        }
    }

    public Point Wrapped(int maxX, int maxY)
    {
        while (x < 0)
        {
            x += maxX;
        }
        while (y < 0)
        {
            y += maxY;
        }
        return new Point(x % maxX, y % maxY);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(b.x + a.x, b.y + a.y);
    }

    public string ToRowColString()
    {
        return string.Format("({0}, {1})", row, col);
    }
}

public class Tile : MonoBehaviour {

    [SerializeField, HideInInspector]
    TileType tileType = TileType.Sliding;

    [SerializeField, HideInInspector]
    int row;

    [SerializeField, HideInInspector]
    int col;

    public event MoveEvent onMove;
    public event MoveStartEvent onMoveStart;
    public event MoveEndEvent onMoveEnd;

    public void Setup(TileType tileType, int row, int col)
    {
        this.row = row;
        this.col = col;
        this.tileType = tileType;
        name = string.Format("Tile ({0}, {1}) [{2}]", row, col, tileType);
    }

    public bool hasPosition(int row, int col)
    {
        return this.row == row && this.col == col;
    }

    public bool isOnRow(int row)
    {
        return this.row == row;
    }

    public bool isOnCol(int col)
    {
        return this.col == col;
    }

    public bool canSlide
    {
        get
        {
            return tileType == TileType.Sliding;
        }
    }

    public bool canSwap
    {
        get
        {
            return tileType == TileType.Sliding;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    bool moving = false;
    Vector3 moveSource;

    private void OnMouseDrag()
    {
        if (!moving)
        {
            moving = true;
            moveSource = mousePointOnPlane();
            if (onMoveStart != null)
            {
                onMoveStart(row, col);
            }
        }

        Vector3 currentPosition = mousePointOnPlane();
        Vector3 moveOffset = currentPosition - moveSource;

        if (onMove != null)
        {
            onMove(moveOffset);
        }
    }

    private void OnMouseUp()
    {
        moving = false;
        if (onMoveEnd != null)
        {
            onMoveEnd();
        }
    }

    Vector3 mousePointOnPlane()
    {
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(transform.up * -1, 0.1f);
        float t;
        if (p.Raycast(r, out t))
        {
            return r.GetPoint(t);
        }
        throw new System.InvalidOperationException("Camera should look at board at all times");
    }

    public Point GetRelativePoint(int offsetCol, int offsetRow)
    {
        return new Point(col + offsetCol, row + offsetRow);
    }

    public Point AsPoint()
    {
        return new Point(col, row);
    }

    public event TileBusyStart OnTileBusyStart;
    public event TileBusyEnd OnTileBusyEnd;

    public void MoveTo(int row, int col, Vector3 target)
    {
        if (OnTileBusyStart != null)
        {
            OnTileBusyStart(this);
        }
        StartCoroutine(_Move(row, col, target));
    }

    IEnumerator<WaitForSeconds> _Move(int row, int col, Vector3 target)
    {
        Vector3 start = transform.position;
        int steps = 15;
        for (int i = 0; i < steps; i++)
        {
            transform.position = Vector3.Lerp(start, target, i / (steps - 1f));
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = target;

        this.row = row;
        this.col = col;
        if (OnTileBusyEnd != null)
        {
            OnTileBusyEnd(this);
        }
    }
}
