using UnityEngine;
using System.Collections;

public class GameState
{
	public string instructions = "";
	public string confirmationText = "";
	public bool showConfirmationDialog = false;
	protected Tile[,] tiles { get { return BonesGame.tiles; } }
	protected PlayerToken player { get { return BonesGame.instance.playerToken.GetComponent<PlayerToken>(); } }
	protected DiceRollWindow diceWindow { get { return BonesGame.instance.diceRollWindow.GetComponent<DiceRollWindow>(); } }

	public void OnConfirmationDialog(ConfirmationDialog.Decision decision)
	{
		if (decision == ConfirmationDialog.Decision.Confirm)
			ConfirmDecision();
		else if (decision == ConfirmationDialog.Decision.Cancel)
			CancelDecision();

		showConfirmationDialog = false;
	}

	public virtual void Update () {}
	protected virtual void ConfirmDecision() {}
	protected virtual void CancelDecision() {}

	public virtual void OnEnemyClicked(EnemyToken enemy) {}
	public virtual void OnTileClicked(Tile tile) {}
	public virtual void OnTokenDropped(Token token, Tile tile) {}
	public virtual void OnDiceRoll(int roll) {}
	public virtual void OnGUI() {}
}
