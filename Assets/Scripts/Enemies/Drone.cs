using UnityEngine;
using System.Collections;

public class Drone : Enemy
{
	public float speed = 3f;
	private EnemyCannon _cannon;

	override protected void init()
	{
		_cannon = GetComponentInChildren<EnemyCannon>();
		InvokeRepeating("Shoot", 2f, 1f);
	}
	
	void Update ()
	{
		// move to the left
		transform.Translate(-Time.deltaTime * speed, 0, 0);
		float leftBound = Camera.main.ViewportToWorldPoint(new Vector3(-.2f, 0, 0)).x;
		if (transform.position.x < leftBound)
		{
			Destroy(gameObject);
			CancelInvoke();
		}
	}

	void Shoot()
	{
		// shoot only if the player is on the left
		if (GameManager.playership.transform.position.x < transform.position.x)
			_cannon.Shoot();
	}
}