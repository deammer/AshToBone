using UnityEngine;
using System.Collections;

public class EnemyToken : Token
{
	public int damage = 1;

	public override void Update ()
	{
		base.Update ();
		
		spriteRenderer.color = enabled ? new Color(1,1,1) : new Color(1f, 1f, 1f, .5f);
		label = enabled ? "Enabled" : "Disabled";
	}

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
		if (draggable)
		{
			spriteRenderer.color = Color.white;
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

	void OnDestroy()
	{
		_currentTile = null;
	}
}
