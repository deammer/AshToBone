using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Token : MonoBehaviour
{
	public int maxHealth = 2;
	[HideInInspector]
	public int currentHealth;

	public GameObject hitEffect;

	public string label = "";

	[HideInInspector]
	public new bool enabled = true;
	[HideInInspector]
	public bool draggable = false;

	private bool _mouseOver = false;
	protected bool mouseOver { get { return _mouseOver; } }
	protected Vector3 _lastPosition;
	public Tile currentTile { get { return _currentTile; } }
	protected Tile _currentTile = null;

	protected SpriteRenderer spriteRenderer;
	protected GameObject glowRenderer;

	public bool selected { get { return _isSelected; } }
	private bool _isSelected = false;

	void Start ()
	{
		glowRenderer = transform.FindChild("Glow").gameObject;
		glowRenderer.SetActive(false);
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentHealth = maxHealth;

		Init();
	}

	public void Select() { _isSelected = true; glowRenderer.SetActive(true); }
	public void Deselect() { _isSelected = false; glowRenderer.SetActive(false); }

	protected virtual void Init() {}
	
	public virtual void Update ()
	{
		Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		bool over = spriteRenderer.bounds.Contains(new Vector3(mouseLocation.x, mouseLocation.y, transform.position.z));

		if (enabled)
		{
			if (!_mouseOver && over)
				OnMouseEnter();
			else if (_mouseOver && !over)
				OnMouseExit();
		}

		if (BonesGame.tokenBeingDragged == this)
		{
			Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			location.z = 0;
			transform.position = location;
		}
	}

	public void MoveToTile(Tile tile)
	{
		_currentTile = tile;
		transform.position = tile.transform.position;
	}

	void OnMouseDown() { if (enabled) OnMousePressed(); }
	void OnMouseUp() { if (enabled) OnMouseReleased(); }

	protected void OnStartDrag()
	{
		BonesGame.instance.OnTokenStartDrag(this);
	}

	protected void OnDropped(Tile newTile)
	{
		if (_currentTile != null)
			_currentTile.currentToken = null;

		_currentTile = newTile;
		_currentTile.currentToken = this;
		transform.position = newTile.transform.position;
		BonesGame.instance.OnTokenDropped(this, newTile);
	}

	public void RemoveFromBoard()
	{
		if (_currentTile != null)
			_currentTile.currentToken = null;
		_currentTile = null;
	}

	public bool IsNextTo(Token other)
	{
		if (other.currentTile.column == currentTile.column)
			return Mathf.Abs(other.currentTile.row - currentTile.row) == 1f;
		if (other.currentTile.row == currentTile.row)
			return Mathf.Abs(other.currentTile.column - currentTile.column) == 1f;
		return false;
	}

	void OnGUI()
	{
		if (label.Length > 0)
		{
			Vector3 location = Camera.main.WorldToScreenPoint(transform.position);
			GUI.skin.label.fontSize = (int)(Screen.width * .03f);
			GUI.skin.label.wordWrap = true;
			GUI.Label(new Rect(location.x- Screen.width * .1f, Screen.height - location.y - Screen.height * .05f, Screen.width * .2f, Screen.height * .1f), label);
		}
	}

	virtual protected void OnMouseEnter() { _mouseOver = true; }
	virtual protected void OnMouseExit() { _mouseOver = false; }
	virtual protected void OnMousePressed() {}
	virtual protected void OnMouseReleased() {}
}