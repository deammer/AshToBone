using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	private Transform [] _items;
	private bool _active;
	private GameObject _player;

	void Start ()
	{
		_items = GetComponentsInChildren<Transform>(); // gets ALL the transform, including the one of THIS object
		foreach (Transform child in _items)
			if (child != transform)
				child.gameObject.SetActive(false);

		_active = false;
	}

	void Update ()
	{
		Vector3 viewport = Camera.main.WorldToViewportPoint(transform.position);
		if (!_active)
		{
			if (viewport.x <= 1 && viewport.x >= 0)
			{
				foreach (Transform child in transform)
					if (child != transform)
						child.gameObject.SetActive(true);
				_active = true;
			}
		}
	}
}
