using UnityEngine;
using System.Collections;

public class BeginNewTurnState : GameState
{
	private float _timer;

	public BeginNewTurnState()
	{
		_timer = 1.5f;

		// reset the number of actions
		BonesGame.instance.counterActions.currentValue = BonesGame.instance.counterActions.maxValue;
	}

	public override void Update ()
	{
		_timer -= Time.deltaTime;

		if (_timer <= 0)
			BonesGame.instance.SwitchState(BonesGame.State.PlaceEnemy);
	}
}