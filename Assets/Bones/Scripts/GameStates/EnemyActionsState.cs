using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyActionsState : GameState
{
	private List<EnemyToken> _enemies;
	private int _currentEnemyIndex;
	private bool _turnOver;

	public EnemyActionsState()
	{
		instructions = "Defend yourself!";
		_turnOver = false;

		// get the enemies that can hit the player
		_enemies = new List<EnemyToken>();
		for (int i = 0; i < BonesGame.instance.enemies.Count; i++)
		{
			if (BonesGame.instance.enemies[i].CanAttackPlayer())
			{
				_enemies.Add(BonesGame.instance.enemies[i]);
			}
		}

		_enemies.Shuffle();
		_currentEnemyIndex = 0;
		
		// show the Defend roll window
		diceWindow.gameObject.SetActive(true);
		diceWindow.SetBackground(DiceRollWindow.BackgroundStyle.Defend);
		diceWindow.SetLocation(_enemies[_currentEnemyIndex].transform.position);
	}

	public override void OnDiceRoll (int roll)
	{
		if (roll >= 4)
		{
			// TODO: damage the player
		}

		// defend against the next enemy
		if (_currentEnemyIndex < _enemies.Count - 1)
		{
			_currentEnemyIndex ++;
			diceWindow.SetLocation(_enemies[_currentEnemyIndex].transform.position);
		}
		else
		{
			// hide the dice roll window
			diceWindow.gameObject.SetActive(false);

			_turnOver = true;
			instructions = "Turn over!";
		}
	}

	public override void OnGUI ()
	{
		if (_turnOver && GUI.Button(new Rect(Screen.width * .4f, Screen.height * .4f, Screen.width * .2f, Screen.height * .2f), "Continue"))
		{
			BonesGame.instance.SwitchState(BonesGame.State.PlaceEnemy);
		}
	}
}