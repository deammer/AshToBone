using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyCannon : MonoBehaviour
{
	public ProjectileProperties projectileProps;

	[Range(0f, 360f)]
	public float shootingAngle;
	[Range(0, 360f)]
	public float shootingDirection;
	public float rotationSpeed = 1f;

	void Update()
	{
		Vector3 dir = GameObject.FindWithTag("Player").transform.position - transform.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		while (angle < shootingDirection - 180) angle += 360f;
		while (angle >= shootingDirection + 180) angle -= 360f;

		float min = shootingDirection - Mathf.Abs(shootingAngle) * .5f;
		float max = shootingDirection + Mathf.Abs(shootingAngle) * .5f;
		while (min < shootingDirection - 180) min += 360f;
		while (min >= shootingDirection + 180) min -= 360f;
		while (max < shootingDirection - 180) max += 360f;
		while (max >= shootingDirection + 180) max -= 360f;

		// rotate towards the player
		angle = Mathf.Clamp(angle, min, max);
		Quaternion q = Quaternion.AngleAxis(angle + 180, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);
		//transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
	}

	public void Shoot()
	{
		GameObject bullet = Instantiate(projectileProps.projectile, transform.position, transform.rotation * Quaternion.Euler(0, 0, 0)) as GameObject;
		bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * -projectileProps.speed,
		                                                          Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * -projectileProps.speed);
		bullet.GetComponent<Projectile>().parentTag = "Enemy";
		bullet.GetComponent<Projectile>().damage = projectileProps.damage;
		bullet.transform.localScale = new Vector2(projectileProps.scaleX, projectileProps.scaleY);
		bullet.name = "Enemy Bullet";
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(shootingDirection - shootingAngle * .5f, Vector3.forward) * transform.parent.transform.right * 3);
		Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(shootingDirection + shootingAngle * .5f, Vector3.forward) * transform.parent.transform.right * 3);
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(shootingDirection, Vector3.forward) * transform.parent.transform.right * 3);
	}
}