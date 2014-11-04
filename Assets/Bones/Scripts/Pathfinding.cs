﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Represents one node in the search space
public class SearchNode
{
	// Location on the map
	public Point Position;

	// If true, this tile can be walked on
	public bool Walkable;

	// This contains references to the for nodes surrounding this tile (Up, Down, Left, Right)
	public SearchNode[] Neighbors;

	// A reference to the node that transfered this node to the open list. This will be used
	// to trace our path back from the goal node to the start node.
	public SearchNode Parent;

	// Provides an easy way to check if this node is in the open list
	public bool InOpenList;

	// Provides an easy way to check if this node is in the closed list
	public bool InClosedList;

	// The approximate distance from the start node to the goal node if the path goes through this node (F)
	public float DistanceToGoal;

	// Distance traveled from the spawn point. (G)
	public float DistanceTraveled;
}

public class Pathfinder
{
	// Stores an array of the walkable search nodes.
	private SearchNode[,] searchNodes;
	
	// The width of the map.
	private int levelWidth;
	// The height of the map.
	private int levelHeight;
	
	// Holds search nodes that are avaliable to search.
	private List<SearchNode> openList = new List<SearchNode>();
	// Holds the nodes that have already been searched.
	private List<SearchNode> closedList = new List<SearchNode>();

	public Pathfinder(Tile[,] map)
	{
		levelWidth = map.GetLength(0);
		levelHeight = map.GetLength(1);
		
		InitializeSearchNodes(map);
	}

	// Returns an estimate of the distance between two points. (H)
	private float Heuristic(Point point1, Point point2)
	{
		return Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
	}

	// Splits our level up into a grid of nodes.
	private void InitializeSearchNodes(Tile[,] map)
	{
		searchNodes = new SearchNode[levelWidth, levelHeight];
		
		//For each of the tiles in our map, we
		// will create a search node for it.
		for (int x = 0; x < levelWidth; x++)
		{
			for (int y = 0; y < levelHeight; y++)
			{
				//Create a search node to represent this tile.
				SearchNode node = new SearchNode();
				node.Position = new Point(x, y);
				
				// Our enemies can only walk on grass tiles.
				node.Walkable = map[x, y].currentToken == null;
				
				// We only want to store nodes
				// that can be walked on.
				if (node.Walkable == true)
				{
					node.Neighbors = new SearchNode[4];
					searchNodes[x, y] = node;
				}
			}
		}
		
		// Now for each of the search nodes, we will
		// connect it to each of its neighbours.
		for (int x = 0; x < levelWidth; x++)
		{
			for (int y = 0; y < levelHeight; y++)
			{
				SearchNode node = searchNodes[x, y];
				
				// We only want to look at the nodes that 
				// our enemies can walk on.
				if (node == null || node.Walkable == false)
				{
					continue;
				}
				
				// An array of all of the possible neighbors this 
				// node could have. (We will ignore diagonals for now.)
				Point[] neighbors = new Point[]
				{
					new Point (x, y - 1), // The node above the current node
					new Point (x, y + 1), // The node below the current node.
					new Point (x - 1, y), // The node left of the current node.
					new Point (x + 1, y), // The node right of the current node
				};
				
				// We loop through each of the possible neighbors
				for (int i = 0; i < neighbors.Length; i++)
				{
					Point position = neighbors[i];
					
					// We need to make sure this neighbour is part of the level.
					if (position.x < 0 || position.x > levelWidth - 1 ||
					    position.y < 0 || position.y > levelHeight - 1)
					{
						continue;
					}
					
					SearchNode neighbor = searchNodes[position.x, position.y];
					
					// We will only bother keeping a reference 
					// to the nodes that can be walked on.
					if (neighbor == null || neighbor.Walkable == false)
					{
						continue;
					}
					
					// Store a reference to the neighbor.
					node.Neighbors[i] = neighbor;
				}
			}
		}
	}

	// Resets the state of the search nodes.
	private void ResetSearchNodes()
	{
		openList.Clear();
		closedList.Clear();
		
		for (int x = 0; x < levelWidth; x++)
		{
			for (int y = 0; y < levelHeight; y++)
			{
				SearchNode node = searchNodes[x, y];
				
				if (node == null)
				{
					continue;
				}
				
				node.InOpenList = false;
				node.InClosedList = false;
				
				node.DistanceTraveled = float.MaxValue;
				node.DistanceToGoal = float.MaxValue;
			}
		}
	}

	// Use the parent field of the search nodes to trace
	// a path from the end node to the start node.
	private List<Point> FindFinalPath(SearchNode startNode, SearchNode endNode)
	{
		closedList.Add(endNode);
		
		SearchNode parentTile = endNode.Parent;
		
		// Trace back through the nodes using the parent fields
		// to find the best path.
		while (parentTile != startNode)
		{
			closedList.Add(parentTile);
			parentTile = parentTile.Parent;
		}
		
		List<Point> finalPath = new List<Point>();
		
		// Reverse the path and transform into world space.
		for (int i = closedList.Count - 1; i >= 0; i--)
		{
			finalPath.Add(new Point(closedList[i].Position.x,
			                          closedList[i].Position.y));
		}
		
		return finalPath;
	}

	// Returns the node with the smallest distance to goal.
	private SearchNode FindBestNode()
	{
		SearchNode currentTile = openList[0];
		
		float smallestDistanceToGoal = float.MaxValue;
		
		// Find the closest node to the goal.
		for (int i = 0; i < openList.Count; i++)
		{
			if (openList[i].DistanceToGoal < smallestDistanceToGoal)
			{
				currentTile = openList[i];
				smallestDistanceToGoal = currentTile.DistanceToGoal;
			}
		}
		return currentTile;
	}

	public List<Tile> FindPath(Tile startTile, Tile endTile)
	{
		List<Point> points = FindPath(new Point(startTile.column, startTile.row),
		                            new Point(endTile.column, endTile.row));

		List<Tile> path = new List<Tile>();
		for (int i = 0; i < points.Count; i++)
			path.Add(BonesGame.tiles[points[i].x, points[i].y]);
		return path;
	}

	// Finds the optimal path from one point to another.
	public List<Point> FindPath(Point startPoint, Point endPoint)
	{
		// Only try to find a path if the start and end points are different.
		if (startPoint == endPoint)
		{
			return new List<Point>();
		}
		
		/////////////////////////////////////////////////////////////////////
		// Step 1 : Clear the Open and Closed Lists and reset each node’s F 
		//          and G values in case they are still set from the last 
		//          time we tried to find a path. 
		/////////////////////////////////////////////////////////////////////
		ResetSearchNodes();
		
		// Store references to the start and end nodes for convenience.
		SearchNode startNode = searchNodes[startPoint.x, startPoint.y];
		SearchNode endNode = searchNodes[endPoint.x, endPoint.y];
		
		/////////////////////////////////////////////////////////////////////
		// Step 2 : Set the start node’s G value to 0 and its F value to the 
		//          estimated distance between the start node and goal node 
		//          (this is where our H function comes in) and add it to the 
		//          Open List. 
		/////////////////////////////////////////////////////////////////////
		startNode.InOpenList = true;
		
		startNode.DistanceToGoal = Heuristic(startPoint, endPoint);
		startNode.DistanceTraveled = 0;
		
		openList.Add(startNode);
		
		/////////////////////////////////////////////////////////////////////
		// Setp 3 : While there are still nodes to look at in the Open list : 
		/////////////////////////////////////////////////////////////////////
		while (openList.Count > 0)
		{
			/////////////////////////////////////////////////////////////////
			// a) : Loop through the Open List and find the node that 
			//      has the smallest F value.
			/////////////////////////////////////////////////////////////////
			SearchNode currentNode = FindBestNode();
			
			/////////////////////////////////////////////////////////////////
			// b) : If the Open List empty or no node can be found, 
			//      no path can be found so the algorithm terminates.
			/////////////////////////////////////////////////////////////////
			if (currentNode == null)
			{
				break;
			}
			
			/////////////////////////////////////////////////////////////////
			// c) : If the Active Node is the goal node, we will 
			//      find and return the final path.
			/////////////////////////////////////////////////////////////////
			if (currentNode == endNode)
			{
				// Trace our path back to the start.
				return FindFinalPath(startNode, endNode);
			}
			
			/////////////////////////////////////////////////////////////////
			// d) : Else, for each of the Active Node’s neighbours :
			/////////////////////////////////////////////////////////////////
			for (int i = 0; i < currentNode.Neighbors.Length; i++)
			{
				SearchNode neighbor = currentNode.Neighbors[i];
				
				//////////////////////////////////////////////////
				// i) : Make sure that the neighbouring node can 
				//      be walked across. 
				//////////////////////////////////////////////////
				if (neighbor == null || neighbor.Walkable == false)
				{
					continue;
				}
				
				//////////////////////////////////////////////////
				// ii) Calculate a new G value for the neighbouring node.
				//////////////////////////////////////////////////
				float distanceTraveled = currentNode.DistanceTraveled + 1;
				
				// An estimate of the distance from this node to the end node.
				float heuristic = Heuristic(neighbor.Position, endPoint);
				
				//////////////////////////////////////////////////
				// iii) If the neighbouring node is not in either the Open 
				//      List or the Closed List : 
				//////////////////////////////////////////////////
				if (neighbor.InOpenList == false && neighbor.InClosedList == false)
				{
					// (1) Set the neighbouring node’s G value to the G value we just calculated.
					neighbor.DistanceTraveled = distanceTraveled;
					// (2) Set the neighbouring node’s F value to the new G value + the estimated 
					//     distance between the neighbouring node and goal node.
					neighbor.DistanceToGoal = distanceTraveled + heuristic;
					// (3) Set the neighbouring node’s Parent property to point at the Active Node.
					neighbor.Parent = currentNode;
					// (4) Add the neighbouring node to the Open List.
					neighbor.InOpenList = true;
					openList.Add(neighbor);
				}
				//////////////////////////////////////////////////
				// iv) Else if the neighbouring node is in either the Open 
				//     List or the Closed List :
				//////////////////////////////////////////////////
				else if (neighbor.InOpenList || neighbor.InClosedList)
				{
					// (1) If our new G value is less than the neighbouring 
					//     node’s G value, we basically do exactly the same 
					//     steps as if the nodes are not in the Open and 
					//     Closed Lists except we do not need to add this node 
					//     the Open List again.
					if (neighbor.DistanceTraveled > distanceTraveled)
					{
						neighbor.DistanceTraveled = distanceTraveled;
						neighbor.DistanceToGoal = distanceTraveled + heuristic;
						
						neighbor.Parent = currentNode;
					}
				}
			}
			
			/////////////////////////////////////////////////////////////////
			// e) Remove the Active Node from the Open List and add it to the 
			//    Closed List
			/////////////////////////////////////////////////////////////////
			openList.Remove(currentNode);
			currentNode.InClosedList = true;
		}
		
		// No path could be found.
		return new List<Point>();
	}
}

public class Point
{
	public int x;
	public int y;

	public Point() { x = 0; y = 0; }
	public Point(int x, int y) { this.x = x; this.y = y; }
}