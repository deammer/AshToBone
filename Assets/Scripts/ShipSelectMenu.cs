using UnityEngine;
using System.Collections;

public class ShipSelectMenu : MonoBehaviour
{
	public ShipInfo [] ships;

	private int _selection = -1;
	private GUIContent [] _contents;
	private Rect _rectSelection;
	private Rect _rectButton;

	void Start()
	{
		_rectSelection = new Rect(Screen.width * .1f, Screen.height * .2f, Screen.width * .8f, Screen.height * .6f);
		_rectButton = new Rect(Screen.width * .4f, Screen.height * .82f, Screen.width * .2f, Screen.height * .1f);

		_contents = new GUIContent[ships.Length];
		for (int i = 0; i < ships.Length; i++)
			_contents[i] = new GUIContent(ships[i].name, ships[i].image, "This is a tooltip!");
	}

	void OnGUI()
	{
		_selection = GUI.SelectionGrid(_rectSelection, _selection, _contents, ships.Length);

		if (_selection != -1)
		{
			if (GUI.Button(_rectButton, "Confirm"))
			{
				Global.shipName = ships[_selection].name;
				Application.LoadLevel("Game");
			}
		}
	}
}

[System.Serializable]
public class ShipInfo
{
	public string name;
	public Texture image;
}