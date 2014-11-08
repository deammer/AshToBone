using UnityEngine;
using System.Collections;

public class WeaponSelection : MonoBehaviour
{
	public GameObject handToken;
	public Weapon [] weapons;
	public Weapon currentWeapon { get { return _selection; } }
	private Weapon _selection;
	private Weapon _playerWeapon;

	void Awake()
	{
		_selection = weapons[0].transform.GetComponent<Weapon>();
	}

	void WeaponSelected(Weapon weapon)
	{
		_selection = weapon;

		// move the hand
		Vector3 handLocation = handToken.transform.position;
		handLocation.y = weapon.transform.position.y;
		handToken.transform.position = handLocation;
	}

	public void Reset()
	{
		// move the selection back to the current weapon
		_selection = BonesGame.instance.playerToken.GetComponent<PlayerToken>().weapon;
		_playerWeapon = _selection;
		Vector3 handLocation = handToken.transform.position;
		handLocation.y = _selection.transform.position.y;
		handToken.transform.position = handLocation;
	}
	
	void OnGUI()
	{
		GUI.skin = GM.skin;
		Rect rect = new Rect(Screen.width * .1f, Screen.height * .1f, Screen.width * .8f, Screen.height * .8f);
		GUI.Box(rect, "Weapon Selection\nCurrent weapon: " + _playerWeapon.name);

		float height = Screen.height * .18f;
		float width = Screen.width * .5f;
		for (int i = 0; i < weapons.Length; i++)
		{
			rect = new Rect((Screen.width - width) * .75f, Screen.height * .25f + i * (height + height * .1f), width, height);
			GUI.DrawTexture(rect, weapons[i].graphic);
			if (rect.Contains(Input.mousePosition) && Input.GetMouseButtonUp(0))
				WeaponSelected(weapons[i]);
		}

		if (_selection != _playerWeapon)
		{
			rect = new Rect(Screen.width * .35f, Screen.height * .7f, Screen.width * .3f, Screen.height * .1f);
			if (GUI.Button(rect, "Switch to " + _selection.name + "!\nCost: 1 Action", GM.skin.customStyles[2]))
			{
				BonesGame.instance.playerToken.GetComponent<PlayerToken>().weapon = _selection;
				BonesGame.instance.OnWeaponChanged(_selection);
				_playerWeapon = _selection;
			}
		}
	}
}