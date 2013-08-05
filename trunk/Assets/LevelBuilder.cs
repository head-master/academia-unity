using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* This class is responsible for detecting clicks against the level editor scene,
 * adding or removing blocks from that scene and exposing functionality for
 * saving and loading of maps. (NOTE: This explanation includes behavior of embedded
 * classes, this class is the top level script for the level-editor scene).
 */

public class LevelBuilder : MonoBehaviour 
{
	
	/* 4 or 5
	 * back
	 * ---------
	 * 2 or 3
	 * top
	 * ---------
	 * 10 or 11
	 * right
	 * ---------
	 * 8 or 9
	 * left
	 * ---------
	 * 0 or 1
	 * front
	 * ---------
	 * 6 or 7
	 * bottom
	 */
	
	/* These represent triangle indices of a given cube.
	 * Each face of a cube is built of 2 triangles, each of which
	 * has an index. These followig indices correspond to which side of
	 * the cube the triangle exists. We can use that to determine
	 * which side of the cube has been clicked and
	 * react appropriately.
	 */
	const int FRONT1 = 0;
	const int FRONT2 = 1;
	const int TOP1 = 2;
	const int TOP2 = 3;
	const int BACK1 = 4;
	const int BACK2 = 5;
	const int BOTTOM1 = 6;
	const int BOTTOM2 = 7;
	const int LEFT1 = 8;
	const int LEFT2 = 9;
	const int RIGHT1 = 10;
	const int RIGHT2 = 11;

    private GameObject previousComponent = null;
	
	private LevelBuilder_Map map = new LevelBuilder_Map(5, 5, 5);
	
	public PrefabCube SelectedCube;
	public List<PrefabCube> Cubes;
	
	string open_file_path = "";
	
	// Use this for initialization
	void Start () 
	{
		SelectedCube = Cubes[0];
		CreateInitialFloor();
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.GetKey(KeyCode.S))
				HandleAddCube();
			else
				HandleMoveToBlock();
		}
		
		if (Input.GetKeyDown(KeyCode.Q))
			SelectedCube = Cubes[0];
		
		if (Input.GetKeyDown(KeyCode.W))
			SelectedCube = Cubes[1];
		
		if (Input.GetKeyDown(KeyCode.E))
			SelectedCube = Cubes[2];
	}
	
	private void HandleMoveToBlock()
	{
		RaycastHit hit;
		
		if (Physics.Raycast(camera.ScreenPointToRay (Input.mousePosition), out hit))
		{
			GameObject hitCube = hit.collider.gameObject;
			//Camera.main.GetComponent<SmoothFollow>().target = hitCube.transform;
			Camera.main.GetComponent<SmoothFollow>().SetTarget(hitCube.transform, hitCube.transform.position.y);
			hitCube.GetComponent<TerrainCubeController>().ActivatePanelOverlay();
			hitCube.GetComponent<TerrainCubeController>().SetPanelOverlayColor(Color.blue);
			if (previousComponent != null)
			{
				/* need to disable the previously selected panel */
				previousComponent.GetComponent<TerrainCubeController>().DeactivatePanelOverlay();
			}
			
			/* Regardless if we deactivated the previously selected 
			 * cube, we need to set the previous cube to the currently
			 * selected cube */
			previousComponent = hitCube;
			
		}
	}
	
	private void HandleAddCube()
	{
		RaycastHit hit;
		
		if (Physics.Raycast(camera.ScreenPointToRay (Input.mousePosition), out hit))
		{
			GameObject hitCube = hit.collider.gameObject;
			Vector3 cubeLocation = hitCube.transform.localPosition;
			HandleFaceClick(cubeLocation.x, cubeLocation.y * 2.5f, cubeLocation.z, hit.triangleIndex);
		}
	}
	
	private void CreateInitialFloor()
	{
		for(int x = 0; x < map.width; ++x)
		{
			for (int z = 0; z < map.depth; ++z)
			{
				AddCubeAtLocation(Cubes[0], x, 0, z);
			}
		}
	}
	
	private void AddCubeAtLocation(PrefabCube cubeToInstantiate, float x, float y, float z)
	{
		if (x >= map.width || y >= map.height || z >= map.depth)
			return;
		
		GameObject go = Instantiate(cubeToInstantiate.Prefab, new Vector3(x * cubeToInstantiate.Prefab.transform.localScale.x, 
			y * cubeToInstantiate.Prefab.transform.localScale.y, z * cubeToInstantiate.Prefab.transform.localScale.z), 
			Quaternion.identity) as GameObject;
		
		map.AddBlock(cubeToInstantiate.CubeCode, (int)x, (int)y, (int)z);
	}
	
	private void HandleFaceClick(float x, float y, float z, int triangle_index)
	{
		switch (triangle_index)
		{
		case FRONT1:
		case FRONT2:
			break;
		case TOP1:
		case TOP2:
			AddCubeTop(x, y, z);
			break;
		case LEFT1:
		case LEFT2:
			break;
		case RIGHT1:
		case RIGHT2:
			break;
		case BOTTOM1:
		case BOTTOM2:
			break;
		case BACK1:
		case BACK2:
			break;
		default:
			Debug.LogWarning("Triangle Index: " + triangle_index + " not supported!");
			break;
		}
	}
	
	private void AddCubeTop(float x, float y, float z)
	{
		if (y + 1 > map.height)
		{
			Debug.Log("Cube placed to high");
			return;
		}
		
		AddCubeAtLocation(SelectedCube, x, y + 1, z);
	}
	
	public void OnGUI()
	{
		if (GUI.Button (new Rect(Screen.width * .05f, Screen.height * .05f, 100f, 25f), "Save"))
		{
			map.WriteToFile("Test.map");
		}
		
		open_file_path = GUI.TextField(new Rect(Screen.width * .05f, Screen.height * .15f, 250, 25), open_file_path);
	
		if (GUI.Button (new Rect(Screen.width * .05f, (Screen.height * .05f) + 30, 100f, 25f), "Load Map"))
		{
			map.LoadFromFile(open_file_path);
			
			InstantiateFromMap ();
		}
	}
	
	private void InstantiateFromMap()
	{
		/* Clear the currently loaded game objects
		 * instantiate the objects from our map */
		for(int x = 0; x < map.width; ++x)
		{
			for(int y = 0; y < map.height; ++y)
			{
				for(int z = 0; z < map.depth; ++z)
				{
					if (map.GetCubeCode(x, y, z) == -1)
					{
						/* Spawn air block */
						/* ... which is a skip for now */
					}
					else
					{
						/* Treat it normal */
						SelectedCube = GetPrefabCubeFromCode(map.GetCubeCode (x, y, z));
						AddCubeAtLocation(SelectedCube, x, y, z);
					}

				}
			}
		}
	}
	
	private PrefabCube GetPrefabCubeFromCode(int code)
	{
		foreach (PrefabCube cube in Cubes)
		{
			if (cube.CubeCode == code)
				return cube;
		}
		
		return null;
	}

}