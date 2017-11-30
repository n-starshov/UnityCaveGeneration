using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Policy;
using System.Security;

public class MapGenerator : MonoBehaviour {


	[Range(0, 100)]
	public int randomFillPercent;
	public int width;
	public int height;
	public string seed;
	public bool useRandomSeed;


	private int[,] map;


	private void Start(){
		GenerateMap();
	}


	private void Update(){
		if (Input.GetMouseButton(0)) {
			GenerateMap();
		}
	}


	private void GenerateMap(){
		map = new int[width, height];
		RandomFillMap();

		for (int i = 0; i < 6; i++) {
			SmoothMap();
		}

		int borderSize = 5;
		int[,] borderMap = new int[width + borderSize * 2, height + borderSize * 2];
		for (int x = 0; x < borderMap.GetLength(0); x++) {
			for (int y = 0; y < borderMap.GetLength(1); y++) {
//				if (x > ){
//					
//				}
			}
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(map, 1);
	}


	private void RandomFillMap(){
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random psuedoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y++){
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
					map[x, y] = 1;
				} else {
					map[x, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}


	private void SmoothMap(){
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				
				int neighbourWallTiles = GetSurroundingWallCount(x, y);

				if (neighbourWallTiles > 4) {
					map[x, y] = 1;
				} else if (neighbourWallTiles < 4){
					map[x, y] = 0;
				}
			}
		}
	}


	private int GetSurroundingWallCount(int gridX, int gridY){
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				} else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}


//	private void OnDrawGizmos(){
//		if (map != null) {
//			for (int x = 0; x < width; x++){
//				for (int y = 0; y < height; y++){
//					Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
//					Vector3 pos = new Vector3(-width / 2 + x + 0.5f, 0, -height / 2 + y + 0.5f);
//					Gizmos.DrawCube(pos, Vector3.one);
//				}
//			}
//		}
//	}
}