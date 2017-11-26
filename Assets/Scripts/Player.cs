using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    Vector3 offset;

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
    }

    public void LookTowards(Tile other)
    {
        Vector3 playerForward = other.transform.position - transform.parent.position;
        transform.rotation = Quaternion.LookRotation(playerForward, Vector3.up);
        next = other;
    }

    Tile next;
}
