using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Token : MonoBehaviour
{
	public int maxHealth = 2;
	[HideInInspector]
	public int currentHealth;

	public GameObject hitEffect;

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
	
	void Update ()
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
	}

	public void MoveToTile(Tile tile)
	{
		_currentTile = tile;
		transform.position = tile.transform.position;
	}

	void OnMouseDown() { if (enabled) OnMousePressed(); }
	void OnMouseUp() { if (enabled) OnMouseReleased(); }

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
		_currentTile = null;
	}

	virtual protected void OnMouseEnter() {}
	virtual protected void OnMouseExit() {}
	virtual protected void OnMousePressed() {}
	virtual protected void OnMouseReleased() {}
}