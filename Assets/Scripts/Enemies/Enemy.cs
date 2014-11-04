using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	public int Health = 2;
	private int _current_health;

	void Start ()
	{
		_current_health = Health;
		init();
	}

	protected virtual void init(){}
	
	void Update ()
	{

	}

	public void Damage(int amount)
	{
		if (_current_health <= 0) return;

		_current_health -= amount;

		if (_current_health <= 0)
			Destroy(gameObject);
	}
}