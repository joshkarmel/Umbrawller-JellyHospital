using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MiniMapCamera : MonoBehaviour 
{
	public Shader replacementShader;
	public string replacementTag = "MiniMap";

	public Color myColor;
	public Material material;
	public string colorPropertyName;

	public Material newMaterial;

	public GameObject[] map;

	private void Start()
	{
		map = GameObject.FindGameObjectsWithTag("MiniMap");
		for(int i = 0; i < map.Length; i++)
		{
			map[i].GetComponent<MeshRenderer>().material = newMaterial;
		}
	}
}
