using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxRocksContainer : MonoBehaviour
{
	public Sprite [] rockSprites;
	public int maxObjects = 10;
	public float minimumSpeed = 1f;
	public float maximumSpeed = 10f;

	[HideInInspector]
	public Vector2 parallaxModifier;

	// object pooling
	private List<GameObject> _rocks;

	void Start ()
	{
		InvokeRepeating("SpawnRandomObject", 0, 1f);

		_rocks = new List<GameObject>();
	}

	private void SpawnRandomObject()
	{
		Vector3 location = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 1.1f, Random.Range(0, Screen.height), 0));
		location.z = 0;
		float left = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * -.5f, 0, 0)).x;

		// if there's a rock to reset, do that
		GameObject rock = null;
		for (int i = 0; i < _rocks.Count; i++)
		{
			// if a rock is too far to the left, reset it on the right
			if (_rocks[i].transform.position.x < left)
			{
				rock = _rocks[i];
				break;
			}
		}

		// if we haven't reset anything and we can spawn more objects, do it!
		if (rock == null && _rocks.Count < maxObjects)
		{
			rock = new GameObject();
			rock.transform.parent = transform;
			rock.AddComponent<SpriteRenderer>();
			rock.GetComponent<SpriteRenderer>().sortingLayerName = "Terrain";
			rock.AddComponent<Rigidbody2D>();

			_rocks.Add(rock);
		}

		// update the rock's props
		if (rock != null)
		{
			rock.transform.position = location;
			rock.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
			float scale = Random.Range(1f, 2f);
			rock.transform.localScale = new Vector2(scale, scale);

			rock.GetComponent<SpriteRenderer>().sprite = rockSprites[Random.Range(0, rockSprites.Length - 1)];
			rock.GetComponent<Rigidbody2D>().gravityScale = 0;
			//rock.GetComponent<Rigidbody2D>().isKinematic = true;
			rock.GetComponent<Rigidbody2D>().velocity = new Vector2(- Random.Range(minimumSpeed, maximumSpeed), 0);
			rock.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-100f, 100f);

			rock.name = "Background Rock";
		}
	}
}