using UnityEngine;
using System.Collections;

public class Counter : MonoBehaviour
{
	public string text = "";

	[HideInInspector]
	public int currentValue;
	[HideInInspector]
	public int maxValue;

	void OnGUI()
	{
		GUI.skin = GM.skin;

		float width = renderer.bounds.extents.x;
		Vector3 topLeft = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x - width, -transform.position.y - width, 0));
		Vector3 bottomRight = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x + width, - transform.position.y + width, 0));

		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.fontSize = 32;
		GUI.Label(new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, Mathf.Abs(topLeft.y - bottomRight.y)), text + "\n" + currentValue + "/" + maxValue);
	}
}