using UnityEngine;
using System.Collections;

public class GearConfirmationBox : MonoBehaviour
{
	public enum Decision { None, Confirm, Cancel };
	public delegate void Callback(Decision decision);
	public Callback callback;

	public void ButtonClicked(Decision decision)
	{
		if (callback != null)
			callback(decision);
	}
}
