using UnityEngine;
using System.Collections;

public class Mine : Enemy
{
	public int damage = 10;
	public float activationTime = 1f;

	private Animator _animator;
	private CircleCollider2D _collider;

	void Start()
	{
		_animator = GetComponent<Animator>();
		_collider = GetComponent<CircleCollider2D>();
	}
	
	void Update()
	{
		var heading = GameManager.playership.transform.position - transform.position;
		var distance = heading.magnitude;

		if (distance < _collider.radius)
		{

		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			Explode();
		}
	}

	private void Explode()
	{
		// TODO: explosion

		Destroy(gameObject);
	}
}