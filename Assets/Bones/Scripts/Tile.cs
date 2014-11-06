using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	// tile states and graphics
	public enum TileState { Normal, Green, Red, Yellow };
	[HideInInspector]
	public TileState state = TileState.Normal;
	public Sprite stateGreen;
	public Sprite stateRed;
	public Sprite stateYellow;
	private SpriteRenderer _stateRenderer;

	public Sprite startSprite;
	[HideInInspector]
	public bool isStartTile = true;

	[HideInInspector]
	public Token currentToken;
	public Color highlightColor;
	private SpriteRenderer _renderer;
	private Color _originalColor;
	private Color _disabledColor = Color.gray;

	[HideInInspector]
	public new bool enabled = true;
	[HideInInspector]
	public bool highlighted = false;

	[HideInInspector]
	public int column;
	[HideInInspector]
	public int row;

	public string label;

	// signals
	public Signal onDragEnter = new Signal();// = new Signal(typeof(Tile));
	public Signal onDragExit = new Signal(typeof(Tile));
	private bool _mouseIsOver = false;

	void Awake()
	{
		currentToken = null;
	}

	void Start ()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_originalColor = _renderer.color;

		if (isStartTile)
			_renderer.sprite = startSprite;

		// add another sprite renderer on top
		GameObject temp = new GameObject("StateRenderer");
		temp.transform.parent = transform;
		temp.AddComponent<SpriteRenderer>();
		temp.transform.position = transform.position;
		_stateRenderer = temp.GetComponent<SpriteRenderer>();
		_stateRenderer.sortingLayerName = _renderer.sortingLayerName;
		_stateRenderer.sortingOrder = _renderer.sortingOrder + 1;
	}
	
	void Update ()
	{
		Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		bool over = _renderer.bounds.Contains(new Vector3(mouseLocation.x, mouseLocation.y, transform.position.z));

		// if the mouse is over && we're enabled
		if (over && enabled)
		{
			_renderer.color = highlightColor;

			if (Input.GetMouseButtonDown(0))
			{
				BonesGame.instance.TileClicked(this);
			}

			if (!_mouseIsOver)
			{
				// if we just entered, trigger a signal
				if (BonesGame.tokenBeingDragged != null)
					BonesGame.instance.TileEntered(this);
				_mouseIsOver = true;
			}
		}
		else if (!over)
		{
			if (_mouseIsOver)
			{
				// if we just exited, trigger a signal
				if (BonesGame.tokenBeingDragged != null)
					BonesGame.instance.TileExited(this);
				_mouseIsOver = false;
			}

			if (enabled)
				_renderer.color = _originalColor;
			else
				_renderer.color = _disabledColor;
		}
	}

	public void SetState(TileState newState)
	{
		state = newState;

		switch (state)
		{
		case TileState.Green:
			_stateRenderer.sprite = stateGreen;
			break;
		case TileState.Red:
			_stateRenderer.sprite = stateRed;
			break;
		case TileState.Yellow:
			_stateRenderer.sprite = stateYellow;
			break;
		default:
			_stateRenderer.sprite = null;
			break;
		}
	}

	public List<Tile> GetAdjacentTiles()
	{
		int columns = BonesGame.tiles.GetLength(0);
		int rows = BonesGame.tiles.GetLength(1);

		List<Tile> tiles = new List<Tile>();
		if (column >= 1) tiles.Add(BonesGame.tiles[column - 1, row]);
		if (column < columns - 1) tiles.Add(BonesGame.tiles[column + 1, row]);
		if (row >= 1) tiles.Add(BonesGame.tiles[column, row - 1]);
		if (row < rows - 1) tiles.Add(BonesGame.tiles[column, row + 1]);

		return tiles;
	}

	public List<Token> GetAdjacentTokens()
	{
		List<Tile> adjacent = GetAdjacentTiles();
		List<Token> tokens = new List<Token>();

		foreach (Tile tile in adjacent)
			if (tile.currentToken != null)
				tokens.Add(tile.currentToken);
		return tokens;
	}

	void OnGUI()
	{
		if (label.Length > 0)
		{
			float width = renderer.bounds.extents.x;
			Vector3 topLeft = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x - width, -transform.position.y - width, 0));
			Vector3 bottomRight = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x + width, - transform.position.y + width, 0));
			
			GUI.skin.label.alignment = TextAnchor.UpperCenter;
			GUI.skin.label.fontSize = Screen.width / 30;
			GUI.Label(new Rect(topLeft.x, topLeft.y, bottomRight.x - topLeft.x, Mathf.Abs(topLeft.y - bottomRight.y)), label);
		}
	}
}