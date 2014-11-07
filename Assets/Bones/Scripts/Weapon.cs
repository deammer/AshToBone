using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public Texture2D icon;
	public int damage;
	public int minRoll;
	public enum AttackPattern { SmallCross, LargeCross, CircleSmall, InverseSmallCross };
	public AttackPattern attackPattern;

	private SpriteRenderer _renderer;

	public static int [,] patternSmallCross = {{1,0}, {-1,0}, {0,1}, {0,-1}};
	public static int [,] patternLargeCross = {{1,0}, {-1,0}, {0,1}, {0,-1}, {2,0}, {-2,0}, {0,2}, {0,-2}};
	public static int [,] patternCircleSmall = {{1,0}, {-1,0}, {0,1}, {0,-1}, {1,1}, {1,-1}, {-1,1}, {-1,-1}};

	void Start ()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
	{
		Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		bool over = _renderer.bounds.Contains(new Vector3(mouseLocation.x, mouseLocation.y, transform.position.z));

		if (Input.GetMouseButtonUp(0) && over)
			transform.parent.gameObject.SendMessage("WeaponSelected", this, SendMessageOptions.RequireReceiver);
	}
}