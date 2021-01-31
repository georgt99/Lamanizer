using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamanizer
{

    private List<Vertex> originalVerts;

    private List<Vertex> remainingVerts; // reduced over time while growing components

    public Lamanizer(IEnumerable<Vertex> verts)
    {
        this.originalVerts = new List<Vertex>(verts);
    }


    public List<List<Vertex>> FindRigidComponents()
    {
        //return PlaceholderFindRigidComponents(verts);
        remainingVerts = new List<Vertex>(originalVerts);

        List<List<Vertex>> rigidComponents = new List<List<Vertex>>();

        while (remainingVerts.Count > 0)
        {
            Vertex startVert = remainingVerts[0];
            remainingVerts.Remove(startVert);
            List<Vertex> currentComponent = new List<Vertex>{ startVert };

            currentComponent = GrowComponent(currentComponent);

            rigidComponents.Add(currentComponent);
            /*foreach(Vertex v in currentComponent)
            {
                if (remainingVerts.Contains(v)) remainingVerts.Remove(v);
            }*/
        }
        return rigidComponents;
        
    }

    private bool LamanCondition(List<Vertex> comp)
    {
        int n = comp.Count;
        int links = 0;
        foreach(Vertex v in comp)
        {
            foreach(Vertex adj in v.edges)
            {
                if (comp.Contains(adj))
                {
                    links++;
                }
            }
        }
        if (links % 2 != 0) Debug.LogError(links + " outgoing edges were found in an undirected subgraph. An even number is expected.");
        links /= 2;


        return links >= 3 * n - 6;
    }


    /// <summary>
    /// Add as many vertices as possible to the component while remaining rigid
    /// </summary>
    private List<Vertex> GrowComponent(List<Vertex> comp)
    {

        foreach(Vertex v in comp)
        {
            foreach (Vertex adj in v.edges)
            {
                if (!comp.Contains(adj) && remainingVerts.Contains(adj)){
                    List<Vertex> newComp = new List<Vertex>(comp);
                    newComp.Add(adj);
                    if (LamanCondition(newComp))
                    {
                        remainingVerts.Remove(adj);
                        return GrowComponent(newComp);
                    }
                }
            }
        }
        // nothing added, leave as is
        return comp;
    }


    public List<List<Vertex>> PlaceholderFindRigidComponents()
    {
        //placeholder-implementation: each vertex is its own component
        List<List<Vertex>> components = new List<List<Vertex>>();
        foreach (Vertex v in originalVerts)
        {
            List<Vertex> newComp = new List<Vertex>();
            newComp.Add(v);
            components.Add(newComp);
        }
        return components;
    }


}
