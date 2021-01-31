using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linkage : MonoBehaviour
{
    public float linkThickness = 0.1f;


    Vertex[] verts;
    List<RigidComponent> rigidComps;


    private Transform flexibleLinkHolder;

    private void Start()
    {
        verts = GetComponentsInChildren<Vertex>();
        MakeEdgesUndirected();
        rigidComps = new List<RigidComponent>();

        flexibleLinkHolder = new GameObject("Linkages").transform;
        flexibleLinkHolder.parent = transform;
        flexibleLinkHolder.localPosition = Vector3.zero;

        Lamanizer lama = new Lamanizer(verts);
        List<List<Vertex>> rigidVertCollections = lama.FindRigidComponents();

        foreach (List<Vertex> vertCollection in rigidVertCollections)
        {
            rigidComps.Add(CreateRigidComponent(vertCollection));
        }

        foreach(Vertex v in verts)
        {
            foreach (Vertex adj in v.edges)
            {
                if (v.rigidComponent != adj.rigidComponent)
                {
                    AddFlexibleLink(v, adj);
                }
                else
                {
                    AddVisualLink(v, adj);
                }
            }
        }
    }


    private void MakeEdgesUndirected()
    {
        foreach(Vertex v in verts)
        {
            foreach(Vertex adj in v.edges)
            {
                if (!adj.edges.Contains(v)) adj.edges.Add(v);
            }
        }
    }

    private RigidComponent CreateRigidComponent(List<Vertex> vertsOfComponent)
    {
        Vector3 center = CenterOfVerts(vertsOfComponent);
        GameObject newGO = new GameObject("RigidComponent");
        newGO.transform.parent = transform;
        newGO.transform.position = center;
        RigidComponent rigidComp = newGO.AddComponent<RigidComponent>();

        foreach(Vertex vert in vertsOfComponent)
        {
            rigidComp.verts.Add(vert);
            vert.transform.parent = newGO.transform;
            vert.rigidComponent = rigidComp;
        }

        newGO.AddComponent<Rigidbody>();
        return rigidComp;
    }


    private Vector3 CenterOfVerts(List<Vertex> verts)
    {
        Vector3 center = Vector3.zero;
        foreach(Vertex v in verts)
        {
            center += v.transform.position;
        }
        center /= verts.Count;
        return center;
    }


    private void AddFlexibleLink(Vertex v1, Vertex v2)
    {
        GameObject cylinder = CylinderBetweenPoints(v1.transform.position, v2.transform.position, flexibleLinkHolder);
        Rigidbody cylinderRB = cylinder.AddComponent<Rigidbody>();

        UseCylinderAsLink(v1.rigidComponent, v1, cylinderRB);
        UseCylinderAsLink(v2.rigidComponent, v2, cylinderRB);

    }

    private void AddVisualLink(Vertex v1, Vertex v2)
    {
        CylinderBetweenPoints(v1.transform.position, v2.transform.position, v1.rigidComponent.transform);
    }

    private GameObject CylinderBetweenPoints(Vector3 vec1, Vector3 vec2, Transform parent)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.parent = parent;
        cylinder.transform.position = (vec1 + vec2) / 2f;
        cylinder.transform.localScale = new Vector3(
            linkThickness,
            (vec1 - vec2).magnitude / 2f,
            linkThickness);

        Vector3 vec1to2 = vec2 - vec1;
        Vector3 orthogonal = Vector3.Cross(vec1to2, Vector3.right); // it shouldn't matter what second vector to take here
        cylinder.transform.LookAt(
            cylinder.transform.position + orthogonal,
            vec1to2
            );
        
        
        return cylinder;
    }

    private void UseCylinderAsLink(RigidComponent comp, Vertex v, Rigidbody cylinderRB)
    {
        ConfigurableJoint joint = comp.gameObject.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        //joint.angularYMotion = ConfigurableJointMotion.Locked;

        joint.anchor = v.transform.localPosition;
        // TODO: set better values for simulation?

        joint.connectedBody = cylinderRB;
    }
}
