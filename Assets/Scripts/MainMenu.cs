using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width * .25f, Screen.height * .25f, Screen.width * .5f, Screen.height * .3f), "Start"))
			Application.LoadLevel("ShipSelect");
	}
}