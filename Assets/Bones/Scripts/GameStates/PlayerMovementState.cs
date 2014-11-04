using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementState : GameState
{
	private Pathfinder _pathfinder;
	private List<Tile> _path;
	private PathDrawer _pathDrawer;

	public PlayerMovementState()
	{
		instructions = "Where do you wanna move?";
		confirmationText = "Confirm move?";
		_pathfinder = new Pathfinder(tiles);
		_pathDrawer = BonesGame.instance.pathDrawer;

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
	}
	
	override public void OnTileClicked(Tile tile)
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

		// go to the Actions phase
		if (BonesGame.instance.counterActions.currentValue > 0)
			BonesGame.instance.SwitchState(BonesGame.State.PlayerAction);
		else
			BonesGame.instance.SwitchState(BonesGame.State.EnemyActions);
	}

	override protected void CancelDecision()
	{
		// hide the dialog
		showConfirmationDialog = false;

		_pathDrawer.Clear();
	}
}