using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { Sliding, Swapping, Stationary };

public delegate void MoveEvent(Vector3 offset);
public delegate void MoveStartEvent(int row, int col);
public delegate void MoveEndEvent();


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
}
