using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthUp : MonoBehaviour {

	// Public Variables
	[Tooltip("How much health will be recovered for the player")]
	public float healthToRecover;
	[Tooltip("The amount of time that needs to pass before the collectible is able to be collected again")]
	public float spawnCooldown;
	[Tooltip("How much time needs to pass from the start of the game before the collectible is active for the first time")]
	public float initialSpawnDelay;
	[Tooltip("Boolean controlling whether the collectible will respawn after being used or not")]
	public bool respawnOnlyOnce;
	// Protected Variables
	protected float spawnCooldownTimer;
	protected Collider powerupCollider;
	protected MeshRenderer powerupMesh;
	protected PlayerInformation pInfo;

	// Use this for initialization
	void Start () {
		powerupMesh = gameObject.GetComponent<MeshRenderer>();
		powerupCollider = gameObject.GetComponent<Collider>();
		powerupCollider.enabled = false;
		powerupMesh.enabled = false;
		spawnCooldownTimer = initialSpawnDelay;
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnCooldownTimer > 0f)
			spawnCooldownTimer -= Time.deltaTime;
		if (spawnCooldownTimer <= 0f)
		{
			powerupCollider.enabled = true;
			powerupMesh.enabled = true;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player")
		{
			pInfo = other.gameObject.GetComponent<PlayerInformation>();
			pInfo.ChangeHealth(healthToRecover);
			powerupCollider.enabled = false;
			powerupMesh.enabled = false;
			if (!respawnOnlyOnce)
				spawnCooldownTimer = spawnCooldown;
		}
	}
}
