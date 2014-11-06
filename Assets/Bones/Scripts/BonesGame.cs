using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BonesGame : MonoBehaviour
{
	// singletonning
	public static BonesGame instance;

	// game flow
	public enum State { PlayerAction, PlaceEnemy, WaitForPlayerAction, EndPlayerTurn, EnemyActions, BeginNewTurn };
	private State _state;
	private GameState _gameState;
	private ConfirmationDialog _confirmDialog;

	// game UI
	public Counter counterLife;
	public Counter counterFavor;
	public Counter counterActions;
	public GameObject diceRollWindow;
	private WeaponSelection _weaponBox;

	// pathfinding
	private Pathfinder _pathfinder;
	public List<Tile> path { get { return _path; } }
	private List<Tile> _path;
	public PathDrawer pathDrawer;

	// tokens
	public List<EnemyToken> enemies = new List<EnemyToken>();

	public static Token tokenBeingDragged = null;

	public GameObject tilePrefab;
	public int rows = 4;
	public int columns = 3;

	public GameObject playerToken;
	public GameObject smallEnemyPrefab;
	public GameObject mediumEnemyPrefab;
	public GameObject largeEnemyPrefab;

	private int [,] _patternSmallCross = {{1,0}, {-1,0}, {0,1}, {0,-1}};
	private int [,] _patternCircleSmall = {{1,0}, {-1,0}, {0,1}, {0,-1}, {1,1}, {1,-1}, {-1,1}, {-1,-1}};

	public static Tile [,] tiles;

	public void TileExited(Tile tile) {}
	public void TileClicked(Tile tile) { _gameState.OnTileClicked(tile); }
	public void TileEntered(Tile tile) {}
	public void DiceRolled(int roll) { _gameState.OnDiceRoll(roll); }
	public void OnEnemyClicked(EnemyToken token) { _gameState.OnEnemyClicked(token); }

	void Start ()
	{
		// init singleton
		instance = this;

		// cache some stuffs
		_weaponBox = GameObject.Find ("WeaponBox").GetComponent<WeaponSelection>();
		_weaponBox.gameObject.SetActive(false);

		CreateBoard();

		// create and hide the dice roll window
		diceRollWindow = (GameObject)Instantiate(diceRollWindow);
		diceRollWindow.SetActive(false);

		// setup counters
		counterLife.maxValue = 4;
		counterFavor.maxValue = 6;
		counterActions.maxValue = 3;

		// position the player token
		playerToken = (GameObject)Instantiate(playerToken);
		playerToken.GetComponent<PlayerToken>().MoveToTile(tiles[1, 3]);
		playerToken.GetComponent<PlayerToken>().weapon = _weaponBox.currentWeapon;

		StartGame();

		_gameState = new PlaceEnemyState();
	}

	void Update()
	{
		_gameState.Update();
	}

	private void StartGame()
	{
		GM.turn = 1;

		// reset Favor, Health and Actions
		counterFavor.currentValue = 0;
		counterLife.currentValue = 3;
		counterActions.currentValue = 3;

		// only enable the 3 Start tiles
		SetTilesInput(false);
		tiles[0, 0].enabled = true;
		tiles[1, 0].enabled = true;
		tiles[2, 0].enabled = true;
	}

	public void OnTokenDropped(Token token, Tile tile)
	{
		_gameState.OnTokenDropped(token, tile);
	}

	public void OnTokenStartDrag(Token token)
	{
		_gameState.OnTokenStartDrag(token);
	}

	public static Tile GetClosestTile(Vector3 location)
	{
		Tile closest = tiles[0,0];
		location.z = 0;
		float distance = Vector3.Distance(tiles[0,0].transform.position, location);

		for (int c = 0; c < tiles.GetLength(0); c++)
		{
			for (int r = 0; r < tiles.GetLength(1); r++)
			{
				if (Vector3.Distance(tiles[c,r].transform.position, location) < distance)
				{
					closest = tiles[c,r];
					distance = Vector3.Distance(tiles[c,r].transform.position, location);
				}
			}
		}

		return closest;
	}

	public static Tile GetTileAt(Vector3 location)
	{
		for (int c = 0; c < tiles.GetLength(0); c++)
		{
			for (int r = 0; r < tiles.GetLength(1); r++)
			{
				if (tiles[c, r].renderer.bounds.Contains(location))
					return tiles[c, r];
			}
		}
		
		return null;
	}

	public void HighlightPatternAt(int column, int row, int [,] pattern)
	{
		SetTilesInput(false);

		for (int i = 0; i < pattern.GetLength(0); i++)
		{
			// if we're within the board, highlight the tile
			if (column + pattern[i,0] >= 0 &&
			    column + pattern[i,0] < columns &&
			    row + pattern[i,1] >= 0 &&
			    row + pattern[i,1] < rows)
			{
				tiles[column + pattern[i,0], row + pattern[i,1]].enabled = true;
			}
		}
	}

	public void SetTilesInput(bool enabled)
	{
		for (int c = 0; c < columns; c++)
		{
			for (int r = 0; r < rows; r++)
			{
				tiles[c, r].enabled = enabled;
			}
		}
	}
	
	private void CreateBoard()
	{
		tiles = new Tile[columns, rows];
		float tileSize = tilePrefab.renderer.bounds.extents.x * 2;
		Vector2 offset = new Vector2((float)(columns - 1) * tileSize * .5f, (float)(rows - 1) * tileSize * .5f);
		
		for (int c = 0; c < columns; c++)
		{
			for (int r = 0; r < rows; r++)
			{
				GameObject tile = (GameObject)Instantiate(tilePrefab, new Vector3(c * tileSize - offset.x, -r * tileSize + offset.y, 0), Quaternion.identity);
				tile.transform.parent = transform;
				tile.name = "Tile (" + c + ", " + r + ")";
				tiles[c, r] = tile.GetComponent<Tile>();
				tiles[c, r].column = c;
				tiles[c, r].row = r;

				tiles[c, r].isStartTile = r == 0;
			}
		}
	}

	public void SwitchState(State newState)
	{
		switch (newState)
		{
		case State.PlaceEnemy:
			_gameState = new PlaceEnemyState();
			break;
		case State.PlayerAction:
			if (counterActions.currentValue > 0)
				_gameState = new PlayerActionState();
			else // if we're out of actions, switch to the EnemyActions state
			{
				Debug.Log("Bypassing the " + newState + " state.");
				SwitchState(State.EnemyActions);
				return;
			}
			break;
		case State.EndPlayerTurn:
			_gameState = new EndPlayerTurnState();
			break;
		case State.EnemyActions:
			// check if any enemy can attack the player
			bool canAttack = false;
			foreach (EnemyToken enemy in enemies)
				if (enemy.CanAttackPlayer())
			{
					canAttack = true;
				break;
			}

			if (canAttack)
				_gameState = new EnemyActionsState();
			else
			{
				Debug.Log("Bypassing the " + newState + " state.");
				SwitchState(State.BeginNewTurn);
				return;
			}
			break;
		case State.BeginNewTurn:
			GM.turn ++;
			_gameState = new BeginNewTurnState();
			_gameState.instructions = "Beginning turn " + GM.turn;
			break;
		}

		Debug.Log("Switching to " + _state);
		_state = newState;
	}

	void OnGUI()
	{
		Rect rect = new Rect();
		if (_gameState.instructions.Length > 0)
		{
			rect = new Rect(0, 0, Screen.width, Screen.height * .2f);
			GUI.skin.label.fontSize = 20;
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.Label(rect, _gameState.instructions);
		}

		if (_gameState.canSwitchWeapon)
		{
			rect.x = Screen.width * .78f;
			rect.y = Screen.height * .02f;
			rect.width = Screen.width * .2f;
			rect.height = Screen.height * .2f;

			GUI.DrawTexture(rect, playerToken.GetComponent<PlayerToken>().weapon.icon);
			if (GUI.Button(rect, "Toggle\nWeapon\nSelection"))
			{
				_weaponBox.gameObject.SetActive(!_weaponBox.gameObject.activeInHierarchy);
				_weaponBox.Reset();
			}
		}


		if (_gameState.showConfirmationDialog)
		{
			if (_confirmDialog == null)
				_confirmDialog = new ConfirmationDialog(_gameState.OnConfirmationDialog, _gameState.confirmationText);
			_confirmDialog.Render();
		}
		else
		{
			if (_confirmDialog != null)
				_confirmDialog = null;
		}

		_gameState.OnGUI();
	}

	void OnDrawGizmos()
	{
		if (_path != null && _path.Count > 1)
		{
			if (_path.Count > 4)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.green;
			Gizmos.DrawLine(_path[0].transform.position, _path[1].transform.position);
			for (int i = 0; i < _path.Count - 1; i++)
				Gizmos.DrawLine(_path[i].transform.position, _path[i + 1].transform.position);
		}
	}
}