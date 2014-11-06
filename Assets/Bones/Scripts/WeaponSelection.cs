using UnityEngine;
using System.Collections;

public class WeaponSelection : MonoBehaviour
{
	public GameObject handToken;
	public GameObject [] weapons;
	private Weapon _selection;

	void Start ()
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
}