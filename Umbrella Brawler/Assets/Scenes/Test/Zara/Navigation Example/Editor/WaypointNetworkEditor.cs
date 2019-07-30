// This is just a test.
// We are not using this in the project

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(WaypointNetwork))]
public class WaypointNetworkEditor : Editor {

    public override void OnInspectorGUI()
    {
        WaypointNetwork network = (WaypointNetwork)target;

        network.DisplayType = (NetworkDisplayType)EditorGUILayout.EnumPopup("Display Type", network.DisplayType);

        if (network.DisplayType == NetworkDisplayType.Paths)
        {
            network.UIStart = EditorGUILayout.IntSlider("Waypoint Start", network.UIStart, 0, network.Waypoints.Count - 1);
            network.UIEnd = EditorGUILayout.IntSlider("Waypoint End", network.UIEnd, 0, network.Waypoints.Count - 1);
        }
        DrawDefaultInspector();
        SceneView.RepaintAll();
    }

    void OnSceneGUI() {
        WaypointNetwork network = (WaypointNetwork)target;

        for (int i = 0; i < network.Waypoints.Count; i++) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.cyan;
            if (network.Waypoints[i] != null)
            {
                Handles.Label(network.Waypoints[i].position, "Waypoint " + i.ToString(), style);
            }
        }

        if (network.DisplayType == NetworkDisplayType.Connections)
        {
            Vector3[] linePoints = new Vector3[network.Waypoints.Count + 1];

            for (int i = 0; i <= network.Waypoints.Count; i++)
            {
                int index = i != network.Waypoints.Count ? i : 0;
                if (network.Waypoints[index] != null)
                {
                    linePoints[i] = network.Waypoints[index].position;
                }
                else
                    linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            }
            Handles.color = Color.cyan;
            Handles.DrawPolyLine(linePoints); // the blue connecting lines in the waypoint network
        }
        else if(network.DisplayType == NetworkDisplayType.Paths) {
            NavMeshPath path = new NavMeshPath();
            Vector3 from = network.Waypoints[network.UIStart].position;
            Vector3 to = network.Waypoints[network.UIEnd].position;

            NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);
            Handles.color = Color.yellow;
            Handles.DrawPolyLine(path.corners); // The yellow curves connecting one waypoint to another
        }
    }

}
