using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceEnemyState : GameState
{
	private List<EnemyToken> _enemyTokens;
	private bool _startTilesEnabled;

	// keeps track of the tiles that are open BEFORE DROPPING TOKENS
	private List<Tile> _openTiles;

	public PlaceEnemyState()
	{
		instructions = "Place an enemy token.";
		confirmationText = "Confirm enemy placement?";

		_startTilesEnabled = false;

		// disable the player
		player.enabled = false;

		BonesGame.instance.SetTilesInput(false); // disable all the tiles first

		UpdateOriginalOpenTiles();

		SpawnEnemyWave();
	}

	private void SpawnEnemyWave()
	{
		// TODO if the board is full, GAME OVER

		// otherwise, spawn the wave as normal
		_enemyTokens = new List<EnemyToken>();
		int waveNumber = GM.turn % 8;

		// special case: if the wave is 4, we spawn a random wave
		while (waveNumber == 4)
			waveNumber = Random.Range(0, 7);

		switch (waveNumber)
		{
		case 1: // small enemy
			SpawnEnemy(GM.EnemyType.Small);
			break;
		case 2: // medium enemy
			SpawnEnemy(GM.EnemyType.Medium);
			break;
		case 3: // 2 small enemies
			SpawnEnemy(GM.EnemyType.Small);
			SpawnEnemy(GM.EnemyType.Small);
			instructions = "Place the enemy tokens";
			break;
		case 4:
			// impossible case, see above
			break;
		case 5:
			SpawnEnemy(GM.EnemyType.Medium);
			break;
		case 6:
			SpawnEnemy(GM.EnemyType.Medium);
			SpawnEnemy(GM.EnemyType.Small);
			instructions = "Place the enemy tokens";
			break;
		case 7:
			SpawnEnemy(GM.EnemyType.Medium);
			break;
		case 0: // 8
			SpawnEnemy(GM.EnemyType.Large);
			break;
		default:
			Debug.LogError("Invalid turn number in SpawnEnemyWave()");
			break;
		}
	}

	private void SpawnEnemy(GM.EnemyType type)
	{
		// spawn a new enemy to place
		GameObject enemy;

		switch (type)
		{
		case GM.EnemyType.Small:
			enemy = (GameObject)GameObject.Instantiate(BonesGame.instance.smallEnemyPrefab);
			break;
		case GM.EnemyType.Medium:
			enemy = (GameObject)GameObject.Instantiate(BonesGame.instance.mediumEnemyPrefab);
			break;
		case GM.EnemyType.Large:
		default:
			enemy = (GameObject)GameObject.Instantiate(BonesGame.instance.largeEnemyPrefab);
			break;
		}

		EnemyToken token = enemy.GetComponent<EnemyToken>();
		_enemyTokens.Add(token);
		token.transform.position = new Vector3(-7.8f, 5.5f - _enemyTokens.IndexOf(token) * 1f, 0);
		token.draggable = true;
		BonesGame.instance.enemies.Add(token);
	}

	private void UpdateOpenTiles()
	{
		// disable all the tiles
		BonesGame.instance.SetTilesInput(false);

		// enable the _openTiles tiles
		foreach (Tile tile in _openTiles)
		{
			if (tile.currentToken == null)
			{
				tile.enabled = true;
				tile.SetState(Tile.TileState.Yellow);
			}
		}

		// for each token on the board, enable those as well
		List<Tile> adjacent;
		foreach (Token token in _enemyTokens)
		{
			// if the token IS on the board
			if (token.currentTile != null && token != BonesGame.tokenBeingDragged)
			{
				adjacent = token.currentTile.GetAdjacentTiles();

				foreach (Tile tile in adjacent)
				{
					if (tile.currentToken == null)
					{
						tile.enabled = true;
						tile.SetState(Tile.TileState.Yellow);
					}
				}
			}
		}
	}

	private void UpdateOriginalOpenTiles()
	{
		// enable the tiles we can drop on
		List<EnemyToken> enemies = BonesGame.instance.enemies;
		
		_openTiles = new List<Tile>();

		// if there are no enemies, only enable the start tiles
		if (enemies.Count == 0)
		{
			_startTilesEnabled = true;
			foreach (Tile tile in tiles)
			{
				if (tile.isStartTile)
				{
					_openTiles.Add(tile);
					tile.SetState(Tile.TileState.Yellow);
				}
				tile.enabled = tile.isStartTile;
			}
		}
		else // otherwise, enable the tiles around the enemies (no diagonals
		{
			// store all the tiles that are adjacent to an enemy
			foreach (EnemyToken token in enemies)
			{
				Tile tile = token.currentTile;

				if (tile == null)
					continue;

				List<Tile> adjacentTiles = tile.GetAdjacentTiles();
				foreach (Tile adjTile in adjacentTiles)
				{
					// if the tile is empty AND not already in the list
					if (adjTile.currentToken == null && _openTiles.IndexOf(adjTile) == -1)
						_openTiles.Add(adjTile);
				}
			}
			
			foreach (Tile tile in _openTiles)
			{
				tile.enabled = true;
				tile.SetState(Tile.TileState.Yellow);
			}
		}
	}

	// checks the tokens we've placed so far to make sure they're legally placed
	private void CheckForIllegalTokens()
	{
		List<EnemyToken> legalTokens = new List<EnemyToken>();
		List<EnemyToken> placedTokens = new List<EnemyToken>();

		PlayerToken player = BonesGame.instance.playerToken.GetComponent<PlayerToken>();

		foreach (EnemyToken token in _enemyTokens)
		{	
			if (token.currentTile != null)
				placedTokens.Add(token);
		}

		// for each token we've placed, check that they're adjacent to a token placed before this turn
		List<Token> adjacentTokens;
		foreach (EnemyToken token in placedTokens)
		{
			adjacentTokens = token.currentTile.GetAdjacentTokens();

			bool found = false;

			foreach (Token adjacentToken in adjacentTokens)
			{
				// skip if the adjacent token == player
				if (found || adjacentToken == player)
					continue;
				
				// if the adjacent token is NOT one of the placed tokens
				if (placedTokens.IndexOf(adjacentToken as EnemyToken) == -1)
				{
					legalTokens.Add(token); // it is legal
					found = true;
				}
			}
		}

		// count the illegal tokens at this point
		int numIllegal = placedTokens.Count - legalTokens.Count;

		// if there are none, we're golden
		if (numIllegal == 0)
			return;

		// create a list of illegal tokens
		int lastNumIllegal = numIllegal;
		List<EnemyToken> illegalTokens = new List<EnemyToken>();
		foreach(EnemyToken token in placedTokens)
		{
			if (legalTokens.IndexOf(token) == -1)
			{
				if ((_startTilesEnabled && !token.currentTile.isStartTile) || !_startTilesEnabled)
					illegalTokens.Add(token);
				else if (_startTilesEnabled && token.currentTile.isStartTile)
					legalTokens.Add(token);
			}
		}

		do
		{
			lastNumIllegal = numIllegal;

			// for each illegal token
			for (int i = illegalTokens.Count - 1; i >= 0; i--)
			{
				adjacentTokens = illegalTokens[i].currentTile.GetAdjacentTokens();

				bool found = false;

				foreach (Token adjacentToken in adjacentTokens)
				{
					if (found || adjacentToken == player)
						continue;

					// if at least one adjacent token is from the legal list, we're good
					if (legalTokens.IndexOf(adjacentToken as EnemyToken) != -1)
					{
						legalTokens.Add(illegalTokens[i]);
						illegalTokens.RemoveAt(i);
						found = true;
					}
				}
			}
			
			// count again. If the number is the same as previously, the tokens that are not placed yet are illegal
			numIllegal = illegalTokens.Count;
		}
		while (lastNumIllegal != numIllegal);
		
		// remove the illegal tokens from the board
		foreach (EnemyToken enemy in illegalTokens)
		{
			enemy.RemoveFromBoard();
			enemy.transform.position = new Vector3(-7.8f, 5.5f - _enemyTokens.IndexOf(enemy) * 1f, 0);
		}
	}

	public override void OnTokenStartDrag (Token token)
	{
		UpdateOpenTiles();
	}

	override public void OnTokenDropped (Token token, Tile tile)
	{
		// determine whether to show the confirmation dialog
		bool show = true;

		if (!_startTilesEnabled)
			UpdateOpenTiles();

		CheckForIllegalTokens();

		// if all the tokens have been placed, show the confirmation dialog
		foreach (EnemyToken enemy in _enemyTokens)
			if (enemy.currentTile == null)
				show = false;

		showConfirmationDialog = show;
	}

	override protected void ConfirmDecision()
	{
		// disable the tokens
		foreach (EnemyToken enemy in _enemyTokens)
		{
			enemy.enabled = false;
			enemy.draggable = false;
		}

		// switch the game state
		BonesGame.instance.SwitchState(BonesGame.State.PlayerAction);
	}
	
	override protected void CancelDecision()
	{
		// hide the dialog
		showConfirmationDialog = false;

		foreach (EnemyToken enemy in _enemyTokens)
		{
			enemy.RemoveFromBoard();
			enemy.transform.position = new Vector3(-7.8f, 5.5f - _enemyTokens.IndexOf(enemy) * 1f, 0);
		}
	}
}