using UnityEngine;
using System.Collections;

public class Link : MonoBehaviour
{
	public enum LinkDirection { Up, Down, Left, Right }

	private Vector3 _originalLocation;
	private Vector3 _lastMouseLocation;
	private float _originalHeight;

	void Awake()
	{
		_originalHeight = renderer.bounds.extents.y * 2f;
	}

	public void SetDirection(LinkDirection direction)
	{
		float angle = 0;
		if (direction == LinkDirection.Down)
			angle = 90;
		else if (direction == LinkDirection.Left)
			angle = 180;
		else if (direction == LinkDirection.Up)
			angle = -90;

		transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
	}

	public void SetSize(float distance)
	{
		Vector3 scale = new Vector3(1f, distance / _originalHeight, 1f);
		transform.localScale = scale;
	}
}