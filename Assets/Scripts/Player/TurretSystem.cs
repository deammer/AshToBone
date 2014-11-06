using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ProjectileProperties
{
	public GameObject projectile;
	public int damage = 1;
	public int scaleX = 1;
	public int scaleY = 1;
	public int speed = 20;
}

#if UNITY_EDITOR
[CustomEditor(typeof(TurretSystem))]
public class TurretEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if (target == null) return;

		var script = (TurretSystem)target;

		if (script.burstFire)
		{
			script.burstDelay = EditorGUILayout.FloatField("Burst Delay", script.burstDelay);
			script.burstCount = EditorGUILayout.IntField("Burst Count", script.burstCount);
		}
	}
}
#endif

public class TurretSystem : MonoBehaviour
{
	public GameObject [] turretPoints;
	public ProjectileProperties projectileProps;
	
	public float fireRate = 5f;

	// burst fire stuff
	public bool burstFire = false;
	[HideInInspector]
	public int burstCount = 1;
	[HideInInspector]
	public float burstDelay = 0.05f;

	private float _shooting_timer = 0f;
	private int _turret_index = 0;

	void Update ()
	{
		_shooting_timer -= Time.deltaTime;

		if (_shooting_timer <= 0f && Input.GetButton("Fire"))
		{
			if (burstFire)
			{
				_shooting_timer = 1 / fireRate + (burstCount - 1) * burstDelay;
				StartCoroutine("FireBurstFromOneTurret", 0);
			}
			else
			{
				FireNextTurret();
				_shooting_timer = 1 / fireRate;
			}
		}
	}

	IEnumerator FireBurstFromOneTurret(int turretIndex)
	{
		int i = 0;
		while (i < burstCount - 1)
		{
			SpawnBullet(turretPoints[turretIndex].transform.position);
			i++;
			yield return new WaitForSeconds(burstDelay);
		}
		SpawnBullet(turretPoints[turretIndex].transform.position);
	}

	private void FireNextTurret()
	{
		// decide which turret to fire
		_turret_index ++;
		if (_turret_index >= turretPoints.Length)
			_turret_index = 0;

		SpawnBullet(turretPoints[_turret_index].transform.position);
	}

	private void SpawnBullet(Vector3 position)
	{
		// instantiate dat projectile
		GameObject bullet = Instantiate(projectileProps.projectile, 
		                                position,
		                                transform.rotation * Quaternion.Euler(0, 0, 0)) as GameObject;
		bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.PI / 180) * projectileProps.speed,
		                                                          Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.PI / 180) * projectileProps.speed);
		bullet.GetComponent<Projectile>().parentTag = "Player";
		bullet.GetComponent<Projectile>().damage = projectileProps.damage;
		bullet.transform.localScale = new Vector2(projectileProps.scaleX, projectileProps.scaleY);
	}
}