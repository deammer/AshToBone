using UnityEngine;
using System.Collections;

public class CameraFollowX : MonoBehaviour
{
	void Start ()
	{
	}

	void Update ()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, camera.transform.position.y, camera.transform.position.z);
	}
}