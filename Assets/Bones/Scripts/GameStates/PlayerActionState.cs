using UnityEngine;
using System.Collections;

public class PlayerActionState : GameState
{
	private EnemyToken _selectedEnemy;

	public PlayerActionState()
	{
		instructions = "Select an action.";

		foreach (EnemyToken enemy in BonesGame.instance.enemies)
			enemy.enabled = true;

		CheckForNextState();
	}

	override public void OnEnemyClicked(EnemyToken enemy)
	{
		_selectedEnemy = enemy;
		_selectedEnemy.Select();

		diceWindow.gameObject.SetActive(true);
		diceWindow.SetBackground(DiceRollWindow.BackgroundStyle.Attack);
		diceWindow.SetLocation(enemy.transform.position);
	}

	override public void OnDiceRoll(int roll)
	{
		if (_selectedEnemy != null)
		{
			if (roll >= 4)
			{
				_selectedEnemy.currentHealth -= 1;

				// show a HIT effect
				GameObject.Instantiate(_selectedEnemy.hitEffect, _selectedEnemy.transform.position, Quaternion.identity);

				if (_selectedEnemy.currentHealth <= 0)
				{
					// destroy that punk
					_selectedEnemy.Deselect();

					// remove from the cache
					BonesGame.instance.enemies.Remove(_selectedEnemy);
					GameObject.Destroy(_selectedEnemy.gameObject);
					_selectedEnemy = null;
				}
			}

			// spend an action
			BonesGame.instance.counterActions.currentValue --;

			CheckForNextState();
		}
	}

	private void CheckForNextState()
	{
		if (BonesGame.instance.counterActions.currentValue <= 0)
		{
			BonesGame.instance.SwitchState(BonesGame.State.EndPlayerTurn);

			diceWindow.gameObject.SetActive(false);

			if (_selectedEnemy != null)
				_selectedEnemy.Deselect();

			foreach (EnemyToken enemy in BonesGame.instance.enemies)
				enemy.enabled = false;
		}
	}
}