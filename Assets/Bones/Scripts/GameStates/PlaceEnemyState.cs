using UnityEngine;
using System.Collections;

public class PlaceEnemyState : GameState
{
	private EnemyToken _enemyToken;

	public PlaceEnemyState()
	{
		instructions = "Place an enemy token.";
		confirmationText = "Confirm enemy placement?";

		GameObject enemy = (GameObject)GameObject.Instantiate(BonesGame.instance.enemyPrefab);
		_enemyToken = enemy.GetComponent<EnemyToken>();
		_enemyToken.transform.position = new Vector3(-7.8f, 5.5f, 0);
		_enemyToken.draggable = true;
		BonesGame.instance.enemies.Add(_enemyToken);

		// disable the player
		player.enabled = false;
	}

	override public void OnTokenDropped(Token token, Tile tile)
	{
		if (_enemyToken == token)
		{
			showConfirmationDialog = true;
		}
	}

	override protected void ConfirmDecision()
	{
		// spawn the enemy and switch game state
		_enemyToken.enabled = false;
		_enemyToken.draggable = false;
		BonesGame.instance.SwitchState(BonesGame.State.PlayerMovement);
	}
	
	override protected void CancelDecision()
	{
		// hide the dialog
		showConfirmationDialog = false;
		_enemyToken.RemoveFromBoard();
		_enemyToken.transform.position = new Vector3(-7.8f, 5.5f, 0);
	}
}
