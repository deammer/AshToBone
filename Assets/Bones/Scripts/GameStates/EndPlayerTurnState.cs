using UnityEngine;
using System.Collections;

public class EndPlayerTurnState : GameState
{

	void Start ()
	{
		instructions = "Out of actions! Beginning enemy turn..";
	}

	override public void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width * .4f, Screen.height * .4f, Screen.width * .2f, Screen.height * .2f), "Continue"))
		{
			// sswitch to Enemy state
			BonesGame.instance.SwitchState(BonesGame.State.EnemyActions);
		}
	}
}
