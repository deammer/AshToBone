using UnityEngine;
using System.Collections;

public class BonesMenu : MonoBehaviour
{
	public TextMesh title;
	public TextMesh subtitle;

	void Start ()
	{
	
	}
	
	void Update ()
	{
		if (Input.anyKey)
			Application.LoadLevel("Bones");

		Color color = subtitle.color;
		color.a = Mathf.Sin(Time.time * 4f) * .5f + .5f;
		subtitle.color = color;
	}
}