using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using NUnit.Framework;

public class MeshGenerator : MonoBehaviour {

	#region classes

	public class SquareGrid{


		public Square[,] squares;


		public SquareGrid(int[,] map, float squareSize){
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControlNode[,] controlNode = new ControlNode[nodeCountX, nodeCountY];
			for (int x = 0; x < nodeCountX; x++){
				for (int y = 0; y < nodeCountY; y++){
					Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2.0f, 0.0f, -mapHeight / 2.0f + y * squareSize + squareSize / 2.0f);
					controlNode[x, y] = new ControlNode(pos, map[x,y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];
			for (int x = 0; x < nodeCountX - 1; x++){
				for (int y = 0; y < nodeCountY - 1; y++){
					squares[x, y] = new Square(
						controlNode[x, y + 1],
						controlNode[x + 1, y + 1],
						controlNode[x + 1, y],
						controlNode[x, y]
					);
				}
			}
		}
	}


	public class Square{


		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public int configuration;


		public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft){
			topLeft = _topLeft;
			topRight = _topRight;
			bottomLeft = _bottomLeft;
			bottomRight = _bottomRight;

			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;

			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}

	}


	public class Node{


		public Vector3 position;
		public int vertexIndex = -1;


		public Node(Vector3 _pos){
			position = _pos;
		}
	}


	public class ControlNode : Node {


		public bool active;
		public Node above, right;


		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * (squareSize / 2.0f));
			right = new Node(position + Vector3.right * (squareSize / 2.0f));
		}
	}

	#endregion	

	#region parametrs
	public SquareGrid squareGrid;


	private List<Vector3> vertices;
	private List<int> triangles;
	#endregion

	#region methods

	private void TriangulateSquare(Square square){
		switch (square.configuration) {
			case 0:
				break;

			// 1 points
			case 1:
				MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
				break;
			case 2:
				MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
				break;
			case 4:
				MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
				break;
			case 8:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
				break;

			// 2 points
			case 3:
				MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
				break;
			case 6:
				MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
				break;
			case 9:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
				break;
			case 12:
				MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
				break;
			case 5:
				MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
				break;
			case 10:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
				break;
			
			// 3 points
			case 7:
				MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
				break;
			case 11:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
				break;
			case 13:
				MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
				break;
			case 14:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
				break;

			// 4 points
			case 15:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
				break;

		}
	}


	private void MeshFromPoints(params Node[] points){
		AssignVertices(points);

		if (points.Length >= 3) {
			CreateTriangle(points[0], points[1], points[2]);
		}
		if (points.Length >= 4) {
			CreateTriangle(points[0], points[2], points[3]);
		}
		if (points.Length >= 5) {
			CreateTriangle(points[0], points[3], points[4]);
		}
		if (points.Length >= 6) {
			CreateTriangle(points[0], points[4], points[5]);
		}
	}


	private void AssignVertices(Node[] points){
		for (int i = 0; i < points.Length; i++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}


	private void CreateTriangle(Node a, Node b, Node c){
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);
	}


//	private void OnDrawGizmos(){
//		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
//			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
//				
//				Gizmos.color = (squareGrid.squares[x, y].topLeft.active) ? Color.black : Color.white;
//				Gizmos.DrawCube(squareGrid.squares[x, y].topLeft.position, Vector3.one * 0.4f);
//
//				Gizmos.color = (squareGrid.squares[x, y].topRight.active) ? Color.black : Color.white;
//				Gizmos.DrawCube(squareGrid.squares[x, y].topRight.position, Vector3.one * 0.4f);
//
//				Gizmos.color = (squareGrid.squares[x, y].bottomLeft.active) ? Color.black : Color.white;
//				Gizmos.DrawCube(squareGrid.squares[x, y].bottomLeft.position, Vector3.one * 0.4f);
//
//				Gizmos.color = (squareGrid.squares[x, y].bottomRight.active) ? Color.black : Color.white;
//				Gizmos.DrawCube(squareGrid.squares[x, y].bottomRight.position, Vector3.one * 0.4f);
//
//				Gizmos.color = Color.gray;
//				Gizmos.DrawCube(squareGrid.squares[x, y].centreTop.position, Vector3.one * 0.15f);
//				Gizmos.DrawCube(squareGrid.squares[x, y].centreRight.position, Vector3.one * 0.15f);
//				Gizmos.DrawCube(squareGrid.squares[x, y].centreBottom.position, Vector3.one * 0.15f);
//				Gizmos.DrawCube(squareGrid.squares[x, y].centreLeft.position, Vector3.one * 0.15f);
//
//			}
//		}
//	}


	public void GenerateMesh(int[,] map, float squareSize){
		squareGrid = new SquareGrid(map, squareSize);

		vertices = new List<Vector3>();
		triangles = new List<int>();

		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
				TriangulateSquare(squareGrid.squares[x, y]);
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}

	#endregion

}
