using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    Vector3 offset;

    Board board;

    private void Start()
    {
        board = GetComponentInParent<Board>();
    }

    public void SetColor(Color color)
    {
        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.material.color = color;
        }
    }

    public void Position(Tile tile)
    {
        transform.parent = tile.transform;
        transform.localPosition = offset;
        current = tile;
    }

    public void LookTowards(Tile other)
    {
        Vector3 playerForward = other.transform.position - transform.parent.position;
        transform.rotation = Quaternion.LookRotation(playerForward, Vector3.up);
        lookDirection = GetLookDirection(current, other);
    }

    Tile current;
    Point lookDirection;

    public void Move()
    {
        Tile next = board.GetTile(current.AsPoint() + lookDirection);
        Debug.Log(
            string.Format(
                "Move [{0}], {1}->{2} ({4}) looking {3}",
                name,
                current,
                next,
                lookDirection.ToRowColString(),
                (current.AsPoint() + lookDirection).ToRowColString()
        ));

        if (next)
        {
            Position(next);
        }                
    }

    public void ActOnPlayer()
    {
        foreach (TileAction action in current.GetComponentsInChildren<TileAction>())
        {
            lookDirection = action.ActOnPlayer(this, lookDirection);
        }
    }

    public Point GetLookDirection(Tile from, Tile to)
    {
        return to.AsPoint() - from.AsPoint();
    }
}
