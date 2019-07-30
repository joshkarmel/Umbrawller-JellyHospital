using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorGenerator {

	public static Vector3 GenerateVector3InSpehere(GameObject sphere){
		Vector3 spawnVector;
		Vector3 centerVector = sphere.transform.position;
		Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
		Bounds bounds = mesh.bounds;
		float radius = 20; // this is temp. TODO: find radius

		spawnVector =  Random.insideUnitSphere * radius + centerVector;

		return spawnVector;
	}

	public static Vector3 GenerateVector3InRectangle(){
		//not done
		// probably wont need it
		return new Vector3();
	}

	public static Vector3 GenerateVector3InShapeList(List<GameObject> shapeList){
		//not done
		// add the area of all the shapes
		return new Vector3();
	}
	
}
