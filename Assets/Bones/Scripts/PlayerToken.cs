using UnityEngine;
using System.Collections;

public class PlayerToken : Token
{

	override protected void Init()
	{
	}

	void Update()
	{
		if (BonesGame.tokenBeingDragged == this)
		{
			Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			location.z = 0;
			transform.position = location;
		}
	}

	override protected void OnMouseEnter()
	{
		spriteRenderer.color = Color.yellow;
	}

	override protected void OnMouseExit()
	{
		spriteRenderer.color = Color.white;
	}

	override protected void OnMousePressed()
	{
		spriteRenderer.color = Color.white;
		transform.localScale = new Vector3(1.2f, 1.2f, 1f);
		BonesGame.tokenBeingDragged = this;
		_lastPosition = transform.position;
	}

	override protected void OnMouseReleased()
	{
		spriteRenderer.color = Color.yellow;
		transform.localScale = new Vector3(1f, 1f, 1f);
		BonesGame.tokenBeingDragged = null;

		Tile newTile = BonesGame.GetTileAt(transform.position);
		if (newTile == null || !newTile.enabled || BonesGame.instance.path.Count > 4)
			transform.position = _lastPosition;
		else
			OnDropped(newTile);
	}
}