using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
	public PlayerShipController player;

	public Texture2D healthBar;
	public Texture2D shieldBar;
	public Texture2D barBackground;
	private Rect _rectHealth;
	private Rect _rectShield;

	void Start ()
	{
		_rectHealth = new Rect(Screen.width * .02f, Screen.width * .02f, Screen.width * .3f, Screen.height * .05f);
		_rectShield = new Rect(_rectHealth.x, _rectHealth.y + _rectHealth.height, _rectHealth.width, _rectHealth.height);
	}

	void OnGUI()
	{
		GUI.DrawTexture(_rectHealth, barBackground);
		GUI.DrawTexture(new Rect(_rectHealth.x, _rectHealth.y, _rectHealth.width * player.healthRatio, _rectHealth.height), healthBar);

		GUI.DrawTexture(_rectShield, barBackground);
		GUI.DrawTexture(new Rect(_rectShield.x, _rectShield.y, _rectShield.width * player.shieldRatio, _rectShield.height), shieldBar);
	}
}