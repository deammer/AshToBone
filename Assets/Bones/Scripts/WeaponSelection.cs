using UnityEngine;
using System.Collections;

public class WeaponSelection : MonoBehaviour
{
	public GameObject handToken;
	public GameObject [] weapons;
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
		if (_selection != _playerWeapon)
		{
			Rect rect = new Rect(Screen.width * .35f, Screen.height * .7f, Screen.width * .3f, Screen.height * .1f);
			if (GUI.Button(rect, "Switch to " + _selection.name + "!\nCost: 1 Action"))
			{
				BonesGame.instance.playerToken.GetComponent<PlayerToken>().weapon = _selection;
				BonesGame.instance.OnWeaponChanged(_selection);
				_playerWeapon = _selection;
			}
		}
	}
}