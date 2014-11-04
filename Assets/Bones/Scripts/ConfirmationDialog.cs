using UnityEngine;
using System.Collections;

public class ConfirmationDialog
{
	public enum Decision { None, Confirm, Cancel };
	private Decision _decision;
	public Decision decision { get { return _decision; } }

	public string text;
	public delegate void Callback(Decision decision);
	public Callback callback;

	public ConfirmationDialog(Callback callback)
	{
		this.callback = callback;
		_decision = Decision.None;
		text = "Confirm?";
	}

	public ConfirmationDialog(Callback callback, string text)
	{
		this.callback = callback;
		_decision = Decision.None;
		this.text = text;
	}
	
	public void Render()
	{
		float width = 100f;
		float height = 90f;

		float x = Screen.width - width;

		GUI.Box(new Rect(x, (Screen.height - height) * .5f, width, height), text);

		if(GUI.Button(new Rect(x + 5f, (Screen.height - height) * .5f + 20, width - 10f, 30f), "Yes"))
			callback(Decision.Confirm);
		
		if(GUI.Button(new Rect(x + 5f, (Screen.height - height) * .5f + 55, width - 10f, 30f), "No"))
			callback(Decision.Cancel);
	}
}
