using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Detonator))]

public class ExplosiveDamage : MonoBehaviour {
	/// <summary>
	/// The blast radius of this explosion.
	/// </summary>
	public float range		 = 5;
	/// <summary>
	/// The blast power of this explosion.
	/// </summary>
	public float power		 = 100;
	/// <summary>
	/// The damage incurred at ground zero.
	/// </summary>
	public float maxDamage	 = 100;
	/// <summary>
	/// Has this exploded yet?
	/// </summary>
	public bool blown		 = false;
	
	public AudioClip explosionNoise;
	
	/// <summary>
	/// Explode this thing.
	/// </summary>
	public void explode() {
		//RaycastHit[] hits = Physics.SphereCastAll(transform.position, range, new Vector3(0,0,0));
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
		//print (hitColliders.Length);
		
		blown = true;
		
		GameObject AUDIO = new GameObject();
		AUDIO.transform.position = transform.position;
		AUDIO.AddComponent<AudioSource>();
		AUDIO.GetComponent<AudioSource>().PlayOneShot(explosionNoise);
		AUDIO.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		AUDIO.AddComponent<TimedObjectDestructor>();
		if (explosionNoise != null) AUDIO.GetComponent<TimedObjectDestructor>().secondsToDestroy = explosionNoise.length;
		if (explosionNoise == null) AUDIO.GetComponent<TimedObjectDestructor>().secondsToDestroy = 5;
		
		foreach (Collider hit in hitColliders) {
			//print (hit.transform.gameObject.name);
			float damage = Mathf.Lerp(0, maxDamage, Vector3.Distance(hit.transform.position, transform.position)/range);
			
			if (hit.transform.FindChild("Camera") != null) {
				if (hit.transform.FindChild("Camera").gameObject.GetComponent<Health>() != null) {
					hit.transform.FindChild("Camera").gameObject.GetComponent<Health>().Damage(damage, DamageCause.Explosion);	
				}
			}
			if (hit.gameObject.name == "RaycastLayer") {
				if (hit.transform.parent.gameObject.GetComponent<EnemyHealth>() != null) {
					hit.transform.gameObject.GetComponent<EnemyHealth>().damage(damage, DamageCause.Explosion);
				}
			}
			if (hit.transform.gameObject.GetComponent<EnemyHealth>() != null) {
				hit.transform.gameObject.GetComponent<EnemyHealth>().damage(damage, DamageCause.Explosion);
			}
			if (hit.transform.gameObject.GetComponent<Detonator>() != null) {
				hit.transform.gameObject.GetComponent<Detonator>().Explode();	
			}
			if (hit.transform.gameObject.GetComponent<ExplosiveDamage>() != null) {
				if (!hit.transform.gameObject.GetComponent<ExplosiveDamage>().blown) {
					hit.transform.gameObject.GetComponent<ExplosiveDamage>().explode();
				}
			}
		}
		print ("BOOM");
		//GetComponent<Detonator>().Explode();
	}
}
