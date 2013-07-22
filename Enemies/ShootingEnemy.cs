using UnityEngine;
using System;
using System.Collections.Generic;

public class ShootingEnemy : PathfindingEnemy {
	
	public Weapon weapon;
	public float semiAutoFireDelay = 0.25f;
	
	float lastShot = -10f;
	
	public Enemy target;
	
	public List<Enemy> targets;
	
	public bool pastReady;
	
	public GameObject head;
	
	public AmmoType ammoType;
	public int startingReserveAmmo = 60;
	public int[] ammo;
	
	public WeaponPickup startingWeapon;
	
	public bool debug = true;
	
	public override void childStart () {
		
		weapon = startingWeapon.thisGun.duplicate();
		
		weapon.player = false;
		ammo = new int[Enum.GetValues(typeof(AmmoType)).Length];
		ammo[(int)ammoType] = startingReserveAmmo;
		
		head = transform.FindChild("head").gameObject;
		weapon.create(head);
		weapon.withdraw();
		
		listEnemies();
	}
	
	public override void childFixedUpdate () {
		weapon.AnimUpdate();
		
		listEnemies();
		target = getNearestEnemy();
		
		if (ready) {
			if (weapon.CurAmmo == 0 && weapon.AnimClock == 0) {
				weapon.Reload(ammo);
				if (debug) print ("Reloading");
			}
			if (!weapon.Automatic && Time.time > lastShot + semiAutoFireDelay) {
				if (debug) print ("Attempting shot!");
				if (weapon.AIShoot(head)) {
					if (debug) print ("Firing!");
				}
			}
			if (weapon.Automatic) {
				if (debug) print ("Holding trigger!");
				weapon.AIShoot(head);
			}
		}
		
		head.transform.LookAt(target.transform.position);
		if (debug) print("Targeting "+target.name);
	}
	
	
	
	Enemy getNearestEnemy() {
		int nearest = 0;
		float closest = 1000;
		
		int i = 0;
		
		// So, warning, this will make your enemy shoot himself after killing all other enemies. 
		
		foreach (Enemy e in targets) {
			if (Vector3.Distance(gameObject.transform.position, targets[i].transform.position) < closest &&
					!e.faction.Equals(faction) && e != this) {
				
				if (debug) print ("Contemplating " + e.name);
				nearest = i;
				closest = Vector3.Distance(gameObject.transform.position, targets[i].transform.position);
			}
			
			i ++;
		}
		if (debug) print("Nearest enemy is " + targets[nearest].name);
		return targets[nearest];
	}
	
	/// <summary>
	/// Lists the enemies.
	/// </summary>
	void listEnemies() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Combatant");
		targets = new List<Enemy>();
		int i = 0;
		foreach (GameObject go in enemies) {
			targets.Add(go.GetComponent<Enemy>());
			i++;
		}
		
		if (debug) print(enemies.Length);
	}
}
