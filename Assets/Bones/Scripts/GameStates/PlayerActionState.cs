using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerActionState : GameState
{
	// movement
	private Pathfinder _pathfinder;
	private List<Tile> _path;
	private PathDrawer _pathDrawer;

	// actions
	private EnemyToken _selectedEnemy;

	public PlayerActionState()
	{
		instructions = "Move and/or attack!";
		canSwitchWeapon = true;

		// disable all the tiles
		BonesGame.instance.SetTilesInput(false);
		
		// setup the tiles we can move to
		Tile currentTile = player.currentTile;
		
		// setup the pathfinder
		_pathfinder = new Pathfinder(tiles);
		currentTile.SetState(Tile.TileState.Green);
		currentTile.enabled = true;
		// highlight the tiles we can move to
		List<Tile> path;
		foreach (Tile tile in tiles)
		{
			if (tile.currentToken == null && tile != currentTile)
			{
				path = _pathfinder.FindPath(currentTile, tile);
				if (path.Count > 0 && path.Count <= BonesGame.instance.counterActions.currentValue)
				{
					tile.SetState(Tile.TileState.Green);
					tile.enabled = true;
				}
			}
		}

		// enable all the enemies
		foreach (EnemyToken enemy in BonesGame.instance.enemies)
			enemy.enabled = true;
	}

	#region Movement
	override public void OnTileClicked(Tile tile)
	{
		// if the tile can be moved to, switch to Movement state
		if (tile.currentToken == null)
		{
			Tile currentTile = player.currentTile;
			if (currentTile == tile)
			{
				_path = new List<Tile>();
				_pathDrawer.Clear();
				showConfirmationDialog = false;
			}
			else
			{
				_path = _pathfinder.FindPath(currentTile, tile);
				_path.Insert(0, currentTile);
				showConfirmationDialog = true;
			}
			_pathDrawer.DrawPath(_path);
		}
	}

	// confirm the move
	override protected void ConfirmDecision()
	{
		// move to the target tile
		player.MoveToTile(_path[_path.Count - 1]);
		
		// remove action points
		BonesGame.instance.counterActions.currentValue -= _path.Count - 1;
		
		// clear tile highlights
		foreach (Tile tile in tiles)
		{
			tile.SetState(Tile.TileState.Normal);
			tile.enabled = false;
		}
		
		// clear the pathdrawer
		_pathDrawer.Clear();

		// check the number of actions remaining
		CheckForNextState();
	}

	// cancel the planned move
	override protected void CancelDecision()
	{
		// hide the dialog
		showConfirmationDialog = false;
		
		_pathDrawer.Clear();
	}
	#endregion Movement

	#region Attacking
	public override void OnEnemyClicked (EnemyToken enemy)
	{
		// TODO some UI work for enemies we can't attack

		// deselect the currently-selected enemy
		if (_selectedEnemy != null)
			_selectedEnemy.Deselect();
		
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
				_selectedEnemy.currentHealth -= player.weapon.damage;
				
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
					
					// hide the roll window
					diceWindow.gameObject.SetActive(true);
				}
			}
			
			// spend an action
			BonesGame.instance.counterActions.currentValue --;
			
			CheckForNextState();
		}
	}
	#endregion Attacking

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