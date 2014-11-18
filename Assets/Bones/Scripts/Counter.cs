using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Counter : MonoBehaviour
{
	public string text = "";
	
	private int _currentValue;
	public int currentValue {
		set { _currentValue = value; UpdateText(); }
		get { return _currentValue; } }

	private int _maxValue;
	public int maxValue {
		set { _maxValue = value; UpdateText(); }
		get { return _maxValue; } }


	private Text _text;

	void Awake()
	{
		_text = transform.FindChild("Text").GetComponent<Text>();
	}

	private void UpdateText()
	{
		_text.text = text + _currentValue + "/" +  _maxValue;
	}
}