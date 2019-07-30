// This is just a test.
// We are not using this in the project

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkDisplayType { None, Connections, Paths}

public class WaypointNetwork : MonoBehaviour {

    [HideInInspector]
    public NetworkDisplayType DisplayType = NetworkDisplayType.Connections;
    [HideInInspector]
    public int UIStart = 0;
    [HideInInspector]
    public int UIEnd = 0;

    public List<Transform> Waypoints = new List<Transform>();
}
