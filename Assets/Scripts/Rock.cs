using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{
	public GameObject childRockPrefab;
	public int numChildrenRocks = 1;
	public int speed;

	void Start ()
	{
		transform.Rotate(Vector3.forward * Random.Range(0, 360f));
	}
	
	void Update ()
	{
		rigidbody2D.AddForce(transform.up * speed);
	}

	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject.tag == "Player" || collider.gameObject.tag == "PlayerProjectile")
			StartCoroutine(DestroyRock());
	}

	public IEnumerator DestroyRock()
	{
		yield return null; // wait until next frame

		if (childRockPrefab != null)
		{
			for (int i = 0; i < numChildrenRocks; i++)
				Instantiate(childRockPrefab, transform.position, Quaternion.identity);
		}

		if (numChildrenRocks != 0)
			yield return new WaitForSeconds(0.2f);

		Destroy(gameObject);
	}
}