using UnityEngine;
using System.Collections;

public class EnemyToken : Token
{
	public int damage = 1;

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
		spriteRenderer.color = Color.red;
	}
	
	override protected void OnMouseExit()
	{
		spriteRenderer.color = Color.white;
	}
	
	override protected void OnMousePressed()
	{
		if (draggable)
		{
			spriteRenderer.color = Color.white;
			transform.localScale = new Vector3(1.2f, 1.2f, 1f);
			BonesGame.tokenBeingDragged = this;
			_lastPosition = transform.position;
			OnStartDrag();
		}
	}
	
	override protected void OnMouseReleased()
	{
		if (draggable)
		{
			spriteRenderer.color = Color.red;
			transform.localScale = new Vector3(1f, 1f, 1f);
			BonesGame.tokenBeingDragged = null;
			
			Tile newTile = BonesGame.GetTileAt(transform.position);
			if (newTile == null || !newTile.enabled || newTile.currentToken != null)
				transform.position = _lastPosition;
			else
				OnDropped(newTile);
		}
		else
			BonesGame.instance.OnEnemyClicked(this);
	}

	public bool CanAttackPlayer()
	{
		PlayerToken player = BonesGame.instance.playerToken.GetComponent<PlayerToken>();
		int playerX = player.currentTile.column;
		int playerY = player.currentTile.row;

		int x = currentTile.column;
		int y = currentTile.row;

		// return true if we're adjacent to the player (diagonal included)
		return x - 1 <= playerX && x + 1 >= playerX && y - 1 <= playerY && y + 1 >= playerY;
	}
}
