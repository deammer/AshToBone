using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathDrawer : MonoBehaviour
{
	public GameObject linkPrefab;
	private List<Link> _links;
	
	public Sprite arrowBend;
	public Sprite arrowHead;

	private List<GameObject> _arrowBends;
	private GameObject _arrowHead;

	void Start ()
	{
		_links = new List<Link>();
		_arrowBends = new List<GameObject>();

		_arrowHead = new GameObject();
		_arrowHead.AddComponent<SpriteRenderer>();
		_arrowHead.GetComponent<SpriteRenderer>().sprite = arrowHead;
		_arrowHead.transform.parent = transform;
		_arrowHead.SetActive(false);
	}
	
	public void DrawPath(List<Tile> path)
	{
		if (path.Count <= 1)
		{
			Clear();
			return;
		}

		// instantiate enough links
		while (_links.Count < path.Count - 1)
		{
			GameObject g = (GameObject)Instantiate(linkPrefab);
			g.transform.parent = transform;
			_links.Add(g.GetComponent<Link>());
		}
		while (_links.Count >= path.Count)
		{
			Destroy(_links[_links.Count - 1].gameObject);
			_links.RemoveAt(_links.Count - 1);
		}

		// position, orient and scale the links
		Link link;
		float distance;
		for (int i = 0; i < _links.Count; i++)
		{
			link = _links[i].GetComponent<Link>();

			link.transform.position = path[i].transform.position;
			if (path[i + 1].column > path[i].column)
				link.SetDirection(Link.LinkDirection.Right);
			else if (path[i + 1].column < path[i].column)
				link.SetDirection(Link.LinkDirection.Left);
			else if (path[i + 1].row > path[i].row)
				link.SetDirection(Link.LinkDirection.Up);
			else
				link.SetDirection(Link.LinkDirection.Down);

			distance = Vector2.Distance(path[i].transform.position, path[i + 1].transform.position);
			link.SetSize(3.2f);
		}

		// position and orient the bends
		GameObject bend;
		while (_arrowBends.Count < _links.Count - 1)
		{
			bend = new GameObject();
			bend.name = "Bend_" + _links.Count;
			bend.transform.parent = transform;
			bend.AddComponent<SpriteRenderer>();
			bend.GetComponent<SpriteRenderer>().sprite = arrowBend;
			_arrowBends.Add(bend);
		}
		while (_arrowBends.Count >= _links.Count)
		{
			Destroy(_arrowBends[_arrowBends.Count - 1]);
			_arrowBends.RemoveAt(_arrowBends.Count - 1);
		}

		for (int i = 0; i < _arrowBends.Count; i++)
		{
			bend = _arrowBends[i];

			// hide if the two links go in the same directin
			bend.SetActive(_links[i].transform.rotation != _links[i + 1].transform.rotation);

			bend.transform.position = _links[i + 1].transform.position;
			bend.transform.rotation = _links[i + 1].transform.rotation;
		}

		// position the arrowhead
		_arrowHead.transform.position = path[path.Count - 1].transform.position;
		_arrowHead.transform.rotation = _links[_links.Count - 1].transform.rotation;
		_arrowHead.SetActive(true);
	}

	public void Clear()
	{
		while (_links.Count > 0)
		{
			Destroy(_links[_links.Count - 1].gameObject);
			_links.RemoveAt(_links.Count - 1);
		}
		while (_arrowBends.Count > 0)
		{
			Destroy(_arrowBends[_arrowBends.Count - 1].gameObject);
			_arrowBends.RemoveAt(_arrowBends.Count - 1);
		}
		_arrowHead.SetActive(false);
	}
}
