using UnityEngine;
using System.Collections;

public class EnemyToken : Token
{
	public int damage = 1;

	protected override void OnEnabled ()
	{
		base.OnEnabled ();
		spriteRenderer.color = new Color(1,1,1);
	}

	protected override void OnDisabled ()
	{
		base.OnDisabled ();
		spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
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
			BonesGame.tokenBeingDragged = this;
			_lastPosition = transform.position;
			OnStartDrag();
		}
	}
	
	override protected void OnMouseReleased()
	{
		if (draggable)
		{
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
