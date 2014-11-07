using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerToken : Token
{
	[HideInInspector]
	public Weapon weapon;

	override protected void Init()
	{}

	public List<EnemyToken> GetEnemiesInRange()
	{
		// get a list of the tiles we can attack
		List<Tile> attackable = new List<Tile>();

		switch (weapon.attackPattern)
		{
		case Weapon.AttackPattern.CircleSmall:
			attackable = GetTilesFromPattern(Weapon.patternCircleSmall, true);
			break;
		case Weapon.AttackPattern.LargeCross:
			attackable = GetTilesFromPattern(Weapon.patternLargeCross, true);
			break;
		case Weapon.AttackPattern.SmallCross:
			attackable = GetTilesFromPattern(Weapon.patternSmallCross, true);
			break;
		case Weapon.AttackPattern.InverseSmallCross:
			attackable = GetTilesFromPattern(Weapon.patternSmallCross, false);
			break;
		}

		// for each attackable tile, check if it has an enemy on it
		List<EnemyToken> enemies = new List<EnemyToken>();
		foreach (Tile tile in attackable)
		{
			if (tile.currentToken != null)
			{
				if (tile.currentToken != this)
					enemies.Add(tile.currentToken as EnemyToken);
			}
		}
		return enemies;
	}

	private List<Tile> GetTilesFromPattern(int[,] pattern, bool inclusive)
	{
		List<Tile> included = new List<Tile>();
		List<Tile> excluded = new List<Tile>(); 
		int x = currentTile.column;
		int y = currentTile.row;
		Point point;
		for (int i = 0; i < pattern.GetLength(0); i++)
		{
			point = new Point(pattern[i,0] + x, pattern[i,1] + y);

			// check that the point is on the board
			if (point.x >= 0 && point.x < BonesGame.tiles.GetLength(0) && point.y >= 0 && point.y < BonesGame.tiles.GetLength(1))
			{
				// if we're inclusing and there is an enemy
				if (inclusive)
					included.Add(BonesGame.tiles[point.x, point.y]);
				else
					excluded.Add(BonesGame.tiles[point.x, point.y]);
			}
		}

		if (!inclusive)
		{
			foreach (Tile tile in BonesGame.tiles)
			{
				// if the tile is NOT exluced, include it
				if (excluded.IndexOf(tile) == -1)
					included.Add(tile);
			}
		}

		return included;
	}

	#region Input Handling
	override protected void OnMouseEnter()
	{
		base.OnMouseEnter();
		transform.localScale = new Vector3(1.2f, 1.2f, 1f);
	}

	override protected void OnMouseExit()
	{
		base.OnMouseExit();
		transform.localScale = new Vector3(1f, 1f, 1f);
	}

	override protected void OnMousePressed()
	{
		spriteRenderer.color = Color.white;
		BonesGame.tokenBeingDragged = this;
		_lastPosition = transform.position;
	}

	override protected void OnMouseReleased()
	{
		spriteRenderer.color = Color.yellow;
		BonesGame.tokenBeingDragged = null;

		Tile newTile = BonesGame.GetTileAt(transform.position);
		if (newTile == null || !newTile.enabled || BonesGame.instance.path.Count > 4)
			transform.position = _lastPosition;
		else
			OnDropped(newTile);
	}
	#endregion
}