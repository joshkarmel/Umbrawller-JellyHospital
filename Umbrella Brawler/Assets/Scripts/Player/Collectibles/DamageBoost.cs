using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoost : MonoBehaviour {

	// Public Variables
	[Tooltip("The multiplier that will be applied to every pellet/bullet fired by the player")]
	public float damageMultiplier;
	[Tooltip("How long the powerup will last in seconds")]
	public float boostDuration;
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
			pInfo.setDamageBoostActive(boostDuration);
			// Set active to false 
			powerupCollider.enabled = false;
			powerupMesh.enabled = false;
			if (!respawnOnlyOnce)
				spawnCooldownTimer = spawnCooldown;
		}
	}
}
