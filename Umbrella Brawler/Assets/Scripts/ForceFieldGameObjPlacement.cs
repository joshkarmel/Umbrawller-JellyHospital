using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldGameObjPlacement : MonoBehaviour {
    public GameObject PrefabToSpawn;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        createEffect(PrefabToSpawn, pos, rot);
        
    }
    
    void createEffect(GameObject Prefab, Vector3 Position, Quaternion Rotation) {

        GameObject newObj = Instantiate(Prefab, Position, Rotation) as GameObject;
        Destroy(newObj, 3f);
    }
}
