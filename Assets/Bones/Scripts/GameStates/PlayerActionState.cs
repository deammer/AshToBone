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
	private List<EnemyToken> _enemiesInRange;

	public PlayerActionState()
	{
		instructions = "Move and/or attack!";
		confirmationText = "Confirm move?";
		_pathDrawer = BonesGame.instance.pathDrawer;
		_selectedEnemy = null;
		canSwitchWeapon = true;

		// disable all the tiles
		BonesGame.instance.SetTilesInput(false);

		// display the tiles we can move to
		DisplayMovementTiles();

		// enable/update the tiles for the enemies we can attack
		UpdateEnemiesWithinRange();
	}

	#region Movement
	override public void OnTileClicked(Tile tile)
	{
		// if the tile can be moved to, switch to Movement state
		if (tile.currentToken == null)
		{
			// deselect the current enemy, if any
			if (_selectedEnemy != null)
			{
				_selectedEnemy.Deselect();
				_selectedEnemy = null;
				diceWindow.gameObject.SetActive(false);
			}

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

		if (!AreWeDoneWithThisState())
		{
			DisplayMovementTiles();
			UpdateEnemiesWithinRange();
		}
	}

	// cancel the planned move
	override protected void CancelDecision()
	{
		// hide the dialog
		showConfirmationDialog = false;
		
		_pathDrawer.Clear();
	}

	// show the tiles we can move to
	private void DisplayMovementTiles()
	{
		// setup the tiles we can move to
		Tile currentTile = player.currentTile;
		
		// setup the pathfinder
		_pathfinder = new Pathfinder(tiles);
		currentTile.SetState(Tile.TileState.Green);
		currentTile.enabled = true;

		// highlight the tiles we can move to
		List<Tile> path;
		int maxLength = BonesGame.instance.counterActions.currentValue;
		foreach (Tile tile in tiles)
		{
			if (tile.currentToken == null && tile != currentTile)
			{
				path = _pathfinder.FindPath(currentTile, tile);
				if (path.Count > 0 && path.Count <= maxLength)
				{
					tile.SetState(Tile.TileState.Green);
					tile.enabled = true;
				}
				else
				{
					tile.SetState(Tile.TileState.Normal);
					tile.enabled = false;
				}
			}
		}
	}
	#endregion Movement

	#region Attacking
	public override void OnEnemyClicked (EnemyToken enemy)
	{
		// cancel path, if any
		CancelDecision();

		// TODO some UI work for enemies we can't attack

		// deselect the currently-selected enemy
		if (_selectedEnemy != null)
		{
			_selectedEnemy.Deselect();
			if (_selectedEnemy == enemy)
			{
				diceWindow.gameObject.SetActive(false);
				_selectedEnemy = null;
				GM.freezeTiles = false;
				return;
			}
		}
		
		_selectedEnemy = enemy;
		_selectedEnemy.Select();
		
		diceWindow.gameObject.SetActive(true);
		diceWindow.SetBackground(DiceRollWindow.BackgroundStyle.Attack);
		diceWindow.SetLocation(enemy.transform.position);

		// freeze the tiles
		GM.freezeTiles = true;
	}

	override public void OnDiceRoll(int roll)
	{
		if (_selectedEnemy != null)
		{
			// spend an action
			BonesGame.instance.counterActions.currentValue --;

			if (roll >= 4)
			{
				_selectedEnemy.currentHealth -= player.weapon.damage;
				
				// show a HIT effect
				GameObject.Instantiate(_selectedEnemy.hitEffect, _selectedEnemy.transform.position, Quaternion.identity);

				// if the enemy deaded
				if (_selectedEnemy.currentHealth <= 0)
				{
					// destroy that punk
					_selectedEnemy.Deselect();
					
					// remove from the cache
					BonesGame.instance.enemies.Remove(_selectedEnemy);
					_selectedEnemy.currentTile.SetState(Tile.TileState.Normal);
					_selectedEnemy.currentTile.currentToken = null;
					GameObject.Destroy(_selectedEnemy.gameObject);
					_selectedEnemy = null;


					// hide the dice roll window
					diceWindow.gameObject.SetActive(false);

					// enable the tiles
					GM.freezeTiles = false;
				}
			}

			if (!AreWeDoneWithThisState())
				DisplayMovementTiles(); // we lost an action, we can't move as far
		}
	}

	private void UpdateEnemiesWithinRange()
	{
		_enemiesInRange = player.GetEnemiesInRange();

		foreach (EnemyToken enemy in BonesGame.instance.enemies)
			enemy.enabled = false;
		foreach (EnemyToken enemy in _enemiesInRange)
		{
			enemy.currentTile.SetState(Tile.TileState.Red);
			enemy.enabled = true;
		}
	}
	#endregion Attacking

	public override void OnWeaponChanged (Weapon newWeapon)
	{
		base.OnWeaponChanged (newWeapon);

		// since the weapon changed, we lost an action point
		if (!AreWeDoneWithThisState())
		{
			DisplayMovementTiles();
			UpdateEnemiesWithinRange();
		}
	}

	private bool AreWeDoneWithThisState()
	{
		if (BonesGame.instance.counterActions.currentValue <= 0)
		{
			diceWindow.gameObject.SetActive(false);
			
			if (_selectedEnemy != null)
				_selectedEnemy.Deselect();
			
			foreach (EnemyToken enemy in BonesGame.instance.enemies)
				enemy.enabled = false;

			foreach (Tile tile in tiles)
			{
				tile.enabled = false;
				tile.SetState(Tile.TileState.Normal);
			}

			BonesGame.instance.SwitchState(BonesGame.State.EndPlayerTurn);

			return true;
		}

		return false;
	}
}