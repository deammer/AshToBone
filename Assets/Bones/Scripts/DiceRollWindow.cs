using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceRollWindow : MonoBehaviour
{
	// backgrounds
	public BackgroundSet attackBackgrounds;
	public BackgroundSet defenseBackgrounds;
	public enum BackgroundStyle { Attack, Defend };
	private BackgroundStyle _backgroundStyle = BackgroundStyle.Defend;

	private int _roll;
	private bool _rolling = false;

	// dice settings
	private const int MIN = 1; // inclusive
	private const int MAX = 8 + 1; // non-inclusive
	private const float ROLL_TIME = 1f; // time spent rolling, in seconds

	// swipe stuff
	private Vector2 _swipeStart;
	private Vector2 _swipeEnd;
	private float _minSwipeDistance;
	private float _maxSwipeTimer;
	private float _timeStamp;

	// text anchor (empty gameobject)
	private Transform _anchorRight;
	private Transform _anchorLeft;
	private Rect _textRect;

	private GUIStyle _style;
	private SpriteRenderer _renderer;

	void Awake ()
	{
		_style = new GUIStyle();
		_style.normal.textColor = new Color(52f/255, 52f/255, 52f/255);
		_style.alignment = TextAnchor.MiddleCenter;

		_renderer = GetComponent<SpriteRenderer>();
		_anchorRight = transform.FindChild("TextAnchorRight").transform;
		_anchorLeft = transform.FindChild("TextAnchorLeft").transform;

		// setup parameters
		_minSwipeDistance = Screen.height * .3f;
		_maxSwipeTimer = .8f; // in seconds
		_roll = -1;
	}
	
	private void RollDice()
	{
		_rolling = true;
		UpdateUI();
		StartCoroutine("AnimateRoll");
	}

	IEnumerator AnimateRoll()
	{
		float stamp = Time.time;
		float delay = 0.05f;
		while (stamp + ROLL_TIME > Time.time)
		{
			List<int> numbers = new List<int>();
			for (int i = 0; i < MAX - MIN; i ++)
				numbers.Add(i + 1);

			// remove the current value from the pool
			if (_roll != -1)
				numbers.RemoveAt(_roll - 1);

			// random roll that's NOT the previous roll
			_roll = numbers[Random.Range(0, numbers.Count)];

			yield return new WaitForSeconds(delay);

			delay += 0.01f;
		}
		_roll = Random.Range(MIN, MAX);
		_rolling = false;

		// notify the game manager
		BonesGame.instance.DiceRolled(_roll);
	}

	void OnGUI()
	{
		if (_roll != -1)
		{
			GUI.Label(_textRect, _roll.ToString(), _style);
		}
		if (!_rolling)
		{
			Rect rect = new Rect(_textRect.x, _textRect.y + Screen.height * .2f, Screen.width * .2f, Screen.height * .1f);
			if (GUI.Button(rect, "Roll!"))
				RollDice();
		}
	}

	public void SetBackground(BackgroundStyle bg)
	{
		_backgroundStyle = bg;
		SetLocation(transform.position);
	}

	public void SetLocation(Vector3 position)
	{
		position.z = transform.position.z;

		float screenX = Camera.main.WorldToScreenPoint(position).x;

		if (screenX < Screen.width * .5f)
		{
			if (_backgroundStyle == BackgroundStyle.Attack)
				_renderer.sprite = attackBackgrounds.textureLeft;
			else if (_backgroundStyle == BackgroundStyle.Defend)
				_renderer.sprite = defenseBackgrounds.textureLeft;
			position.x += _renderer.bounds.extents.x;
		}
		else
		{
			if (_backgroundStyle == BackgroundStyle.Attack)
					_renderer.sprite = attackBackgrounds.textureRight;
			else if (_backgroundStyle == BackgroundStyle.Defend)
				_renderer.sprite = defenseBackgrounds.textureRight;
			position.x -= _renderer.bounds.extents.x;
		}
		transform.position = position;

		UpdateUI();
	}
	
	private void UpdateUI()
	{
		_style.fontSize = (int)(Screen.width * .15f);

		Vector3 location = Camera.main.WorldToScreenPoint(_anchorRight.position);
		float width = Screen.width * .18f;
		_textRect = new Rect(location.x - width * .5f, Screen.height - location.y - width * .5f, width, width);
	}

	void OnMouseDown()
	{
//		_swipeStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//		_timeStamp = Time.time;
	}

	void OnMouseUp()
	{
//		_swipeEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//
//		float distance = Vector2.Distance(_swipeStart, _swipeEnd);
//		_timeStamp = Time.time - _timeStamp;
//
//		Debug.Log("Timestamp = " + _timeStamp);
//		if (_timeStamp <= _maxSwipeTimer && distance >= _minSwipeDistance)
			RollDice();
	}
}

[System.Serializable]
public class BackgroundSet
{
	public Sprite textureLeft;
	public Sprite textureRight;
}