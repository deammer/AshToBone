using UnityEngine;
using System.Collections;

public class WeaponSelection
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
}