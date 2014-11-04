using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public static PlayerShipController playership;
	public static int score;

	// GUI
	private GUIText _scoreText;

	void Start ()
	{
		// instantiate the player ship
		GameObject ship = (GameObject)Instantiate(Resources.Load("PlayerShips/" + Global.shipName, typeof(GameObject)));
		playership = ship.GetComponent<PlayerShipController>();

		GetComponent<PlayerUI>().player = playership;

		_scoreText = GameObject.Find("ScoreText").GetComponent<GUIText>();

		score = 0;
	}
	
	void Update ()
	{
		
	}

	void OnGUI()
	{
		// display score
		_scoreText.pixelOffset = new Vector2(.8f * Screen.width, .8f * Screen.height);
		_scoreText.text = "Score: " + score;
	}
}