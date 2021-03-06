using UnityEngine;
using System;
using System.Collections.Generic;

public class ShootingEnemy : PathfindingEnemy {
	
	public Weapon weapon;
	public float semiAutoFireDelay = 0.25f;
	
	float lastShot = -10f;
	
	public bool pastReady;
	
	public GameObject head;
	
	public AmmoType ammoType;
	public int startingReserveAmmo = 60;
	public int[] ammo;
	
	public WeaponPickup startingWeapon;
	public PickupGrenadeLauncher startingWeaponGL;
	
	public bool isAimed = false;
	
	
	public float rotSpd = 5f;
	public float scopedRotSpd = 1f;
	public float satisfactoryAimInDegrees = 5;
		
	
	
	Quaternion rotation;
	
	
	
	public override void childStart () {
		
		if (startingWeaponGL == null) weapon = startingWeapon.thisGun.duplicate();
		if (startingWeaponGL != null) weapon = startingWeaponGL.thisGun.duplicate(); 
		
		weapon.player = false;
		ammo = new int[Enum.GetValues(typeof(AmmoType)).Length];
		ammo[(int)ammoType] = startingReserveAmmo;
		
		head = transform.FindChild("head").gameObject;
		weapon.create(head, false);
		weapon.withdraw();
		
		targets = listEnemies();
		print("Targets: " + targets.Count);
	}
	
	/*public virtual void checkAnyVisible () {
		foreach (Enemy e in targets) {
			if (e.faction != faction) {
				if (debug) print ("Checking to see if alerted by " +  e.name);
				var rayDirection = e.transform.position - transform.position;
				if (Vector3.Angle(rayDirection, transform.forward) < fieldOfViewRadiusInDegrees) {
					if (debug) print ("Angle satisfied.");
					if (Vector3.Distance(transform.position, e.transform.position) < visionRange) {
						if (debug) print ("Distance satisfied.");
						//RaycastHit hitinfo;
						//Physics.Linecast (head.transform.position, e.transform.position, hitinfo);
						//if (hitinfo.transform = e.transform) {
						//	if (debug) print ("Linecast satisfied.");
						alerted = true;
						target = e;
						if (debug) print ("Found target. Starting to kill!");
						//}
					}
				}
			}
		}
	}*/
	
	
	public override void childFixedUpdate () {
		weapon.AnimUpdate();
		
		//Look towards predetermined target
		if (target != null) {
			Vector3 targetPos = target.transform.position;
			Vector3 currentPos = head.transform.position;
			Vector3 relativePos = targetPos - currentPos ;
			rotation = Quaternion.LookRotation(relativePos);
			if (!isAimed){
				head.transform.rotation = Quaternion.Slerp(head.transform.rotation, rotation, Time.deltaTime * rotSpd);
				if (weapon.isAimed) {
					weapon.aim();
				}
			} else {
				head.transform.rotation = Quaternion.Slerp(head.transform.rotation, rotation, Time.deltaTime * scopedRotSpd);
				if (!weapon.isAimed) {
					weapon.aim();
				}
			}
			
		}
		
		isAimed = Quaternion.Angle(head.transform.rotation, rotation) < satisfactoryAimInDegrees;
		
		if (!alerted) checkAnyVisible();
		
		
		// BEGIN weapon handling system.
		if (ready && alerted && isAimed) {
			//target = getNearestEnemy();
			if (weapon.CurAmmo == 0 && weapon.AnimClock == 0) {
				weapon.Reload(ammo);
				if (debug) print ("Reloading");
			}
			if (!weapon.Automatic && Time.time > lastShot + semiAutoFireDelay) {
				if (debug) print ("Attempting shot!");
				if (weapon.AIShoot(head, this)) {
					lastShot = Time.time;
					if (debug) print ("Firing!");
				}
			}
			if (weapon.Automatic) {
				if (debug) print ("Holding trigger!");
				weapon.AIShoot(head, this);
			}
			if (debug) print("Targeting "+target.name);
		}
		// END weapon handling system.
		
		
		
																	// Satisfactory aiming criteria. In degrees.
		//&&
			//!Physics.Linecast (head.transform.position, target.transform.position);
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
}
