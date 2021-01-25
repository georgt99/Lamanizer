using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Linkage))]
public class LinkageEditor : Editor
{
    private Vertex previousSelection;


    // inspired by https://forum.unity.com/threads/solved-custom-editor-onscenegui-scripting.34137/
    public void OnSceneGUI()
    {
        Linkage links = (Linkage)target;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                Vertex selected;
                if (selected = hitInfo.collider.GetComponent<Vertex>())
                {
                    if (previousSelection == null) // first selection
                    {
                        previousSelection = selected;
                    }
                    else // second selection
                    {
                        if (previousSelection == selected) // selected again, undo selection
                        {
                            previousSelection = null;
                        }
                        else if (previousSelection.edges.Contains(selected))
                        {
                            Undo.RecordObject(previousSelection, "Remove Edge");
                            previousSelection.edges.Remove(selected);
                        }
                        else if (selected.edges.Contains(previousSelection))
                        {
                            Undo.RecordObject(selected, "Remove Edge");
                            selected.edges.Remove(previousSelection);
                        }
                        else // didn't exist yet, add from previous to new
                        {
                            Undo.RecordObject(previousSelection, "Add Edge");
                            previousSelection.edges.Add(selected);
                        }
                        previousSelection = null;
                    }
                    Event.current.Use();
                }
            }
        }
        if (previousSelection)
        {
            Handles.DrawWireCube(previousSelection.transform.position, previousSelection.transform.lossyScale);
        }

        foreach(Vertex vert in links.GetComponentsInChildren<Vertex>())
        {
            foreach(Vertex v2 in vert.edges)
            {
                Handles.DrawLine(vert.transform.position, v2.transform.position);
            }
        }
    }


}
