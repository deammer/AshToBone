using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public int damage = 1;
	public GameObject explosion;

	[HideInInspector]
	public string parentTag;
	
	void Start ()
	{
		// Destroy the projectile after 2 seconds if it doesn't get destroyed before then
		Destroy(gameObject, 2f);
	}

	private void OnExplode()
	{
		// instantiate an explosion here
		if (explosion != null)
			Instantiate(explosion, transform.position, Quaternion.AngleAxis(Random.Range(0, 359), Vector3.forward));
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.tag == "Enemy" && parentTag == "Player")
		{
			// find the Enemy script and call the Damage function
			collider.gameObject.GetComponent<Enemy>().Damage(damage);

			OnExplode();
			Destroy(gameObject);
		}
		else if (collider.tag == "Player" && parentTag == "Enemy")
		{
			OnExplode();
			GameManager.playership.Damage(damage, transform);
			Destroy(gameObject);
		}
		else if (collider.tag == "Rock" || collider.tag == "Terrain")
		{
			StartCoroutine(DestroyNextFrame());
			if (explosion != null)
			{
				Vector3 direction = collider.transform.position - transform.position;
				direction = collider.transform.InverseTransformDirection(direction);
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

				Instantiate(explosion, transform.position, Quaternion.AngleAxis(angle, Vector3.right));
			}
		}
	}

	IEnumerator DestroyNextFrame()
	{
		yield return null;

		Destroy(gameObject);
	}
}