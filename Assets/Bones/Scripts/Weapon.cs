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

	private static int [,] _patternSmallCross = {{1,0}, {-1,0}, {0,1}, {0,-1}};
	private static int [,] _patternLargeCross = {{1,0}, {-1,0}, {0,1}, {0,-1}, {2,0}, {-2,0}, {0,2}, {0,-2}};
	private static int [,] _patternCircleSmall = {{1,0}, {-1,0}, {0,1}, {0,-1}, {1,1}, {1,-1}, {-1,1}, {-1,-1}};

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