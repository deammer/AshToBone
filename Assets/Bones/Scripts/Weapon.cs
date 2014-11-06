using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public int damage;
	public int minRoll;
	private SpriteRenderer _renderer;


	void Start ()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
	{
		
	}

	void OnMouseUp()
	{
		transform.parent.gameObject.SendMessage("WeaponSelected", this, SendMessageOptions.RequireReceiver);
	}
}