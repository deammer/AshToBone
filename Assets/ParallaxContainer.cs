using UnityEngine;
using System.Collections;

public class ParallaxContainer : MonoBehaviour
{
	public Vector2 direction;
	public Vector2 speed = new Vector2(-1f, 0);
	public Sprite image;

	private float _width;
	private GameObject [] _images;

	void Start ()
	{
		_width = image.texture.width;

		int numImages = (int)Mathf.Ceil(Screen.width / _width) + 1;
		_images = new GameObject[numImages];
		for (int i = 0; i < _images.Length; i++)
		{
			// create a gameobject with a SpriteRenderer
			_images[i] = new GameObject(transform.name + "_Image", typeof(SpriteRenderer));
			_images[i].GetComponent<SpriteRenderer>().sprite = image;
			_images[i].GetComponent<SpriteRenderer>().sortingLayerName = "Background";
			_images[i].transform.position = new Vector3(i * _width, 0f, 0f);
			_images[i].transform.parent = transform;
		}
	}
	
	void Update ()
	{
		// Camera borders
		float dist = (transform.position - Camera.main.transform.position).z;
		float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x;
		float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x;
		float width = Mathf.Abs(rightBorder - leftBorder);
		float topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).y;
		float bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, dist)).y;
		float height = Mathf.Abs(topBorder - bottomBorder);
		
		// Determine entry and exit border using direction
		Vector3 exitBorder = Vector3.zero;
		Vector3 entryBorder = Vector3.zero;
		
		if (direction.x < 0)
		{
			exitBorder.x = leftBorder;
			entryBorder.x = rightBorder;
		}
		else if (direction.x > 0)
		{
			exitBorder.x = rightBorder;
			entryBorder.x = leftBorder;
		}
		
		if (direction.y < 0)
		{
			exitBorder.y = bottomBorder;
			entryBorder.y = topBorder;
		}
		else if (direction.y > 0)
		{
			exitBorder.y = topBorder;
			entryBorder.y = bottomBorder;
		}
	}
}