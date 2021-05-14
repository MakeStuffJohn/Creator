using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Table
{
    public Vector2 surfaceHeight;
    public GameObject defaultSurfaceCollider;
    public GameObject rotatedSurfaceCollider;
    public GameObject frontEdge;
    public GameObject leftEdge;
    public GameObject backEdge;
    public GameObject rightEdge;
}
