using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public List<Vertex> edges; // edges are technically directed, but that's just easier to store, directions have no impact

    public RigidComponent rigidComponent;


}
