using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public void SetColor(Color color)
    {
        foreach(MeshRenderer rend in GetComponentsInChildren<MeshRenderer>())
        {
            rend.material.color = color;
        }
    }
}
