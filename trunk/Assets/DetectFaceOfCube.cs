using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DetectFaceOfCube : MonoBehaviour 
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
	
	/* These represent triangle indices of a given cube
	 * each face of a cube is built of 2 triangles, each of which
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
	
	private Plane plane = new Plane(Vector3.up, Vector3.zero);
    private Vector3 v3Center = new Vector3(0.5f,0.5f,0.0f);
	
	/* In terms of placement of newly added blocks:
	 * left = -x
	 * right = +x
	 * back = -z
	 * front = +z
	 * top = +y
	 * bottom = -y */
	
	/* We need to keep track of each cube and their locations
	 * to do that, we'll create a 3-D array of Cubes
	 * which will correspond to Cubes in the real world */
	
	public TerrainCube[,,] terrain_map;
	
	Component previousComponent = null;
	
	public int MAX_WIDTH = 5;
	public int MAX_DEPTH = 5;
	public int MAX_HEIGHT = 5;
	
	public KeyValuePair<int, GameObject> GrassCube;
	public KeyValuePair<int, GameObject> DirtCube;
	public KeyValuePair<int, GameObject> RockCube;
	
	public KeyValuePair<int, GameObject> SelectedCube;
	
	public List<GameObject> prefabs = new List<GameObject>();
	
	public string open_file_path = "";
	
	public Vector3 TargetLocation = Vector3.zero;
	
	// Use this for initialization
	void Start () 
	{
		TargetLocation = Camera.main.transform.localPosition;
		GrassCube = new KeyValuePair<int, GameObject>(0, prefabs[0]);
		DirtCube = new KeyValuePair<int, GameObject>(1, prefabs[1]);
		RockCube = new KeyValuePair<int, GameObject>(2, prefabs[2]);
		terrain_map = new TerrainCube[MAX_WIDTH, MAX_HEIGHT, MAX_DEPTH];
		for(int x = 0; x < MAX_WIDTH; ++x)
		{
			for(int y = 0; y < MAX_HEIGHT; ++y)
			{
				for(int z = 0; z < MAX_DEPTH; ++z)
				{
					terrain_map[x, y, z] = new TerrainCube(x, y, z);
				}
			}
		}
		SelectedCube = GrassCube;
		CreateInitialFloor();
	}
	float speed = 10f;
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Input.GetKey(KeyCode.S))
				OnMouseDown ();
			else
				Movement();
		}
		
		if (Input.GetKeyDown(KeyCode.Q))
			SelectedCube = GrassCube;
		
		if (Input.GetKeyDown(KeyCode.W))
			SelectedCube = DirtCube;
		
		if (Input.GetKeyDown(KeyCode.E))
			SelectedCube = RockCube;
		
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, TargetLocation, Time.deltaTime * 5f);
			
		transform.localPosition = new Vector3(transform.localPosition.x, 5.315834f, transform.localPosition.z);
	
	}
	
	private void Movement()
	{
		RaycastHit hit;
		
		if (Physics.Raycast(camera.ScreenPointToRay (Input.mousePosition), out hit))
		{
			GameObject hitCube = hit.collider.gameObject;
			
			Vector3 cubeLocation = hitCube.transform.localPosition;
			
			Debug.Log (cubeLocation);
			
			DoMovement(cubeLocation + new Vector3(0f, 0, 0f));
			Component render = hitCube.gameObject.GetComponentsInChildren<MeshRenderer>()[1];
			
			if (previousComponent != null)
			{
				((MeshRenderer)previousComponent).enabled = false;
			}
			previousComponent = render;
			((MeshRenderer)render).enabled = true;
			
			
		}
	}
	
	private void DoMovement(Vector3 location)
	{
		//TargetLocation = location + camera.transform.forward * (260-location.y)/Camera.main.transform.forward.y;
		Ray ray = Camera.main.ViewportPointToRay(v3Center);
    	float fDist;
    	if (plane.Raycast(ray, out fDist))
       	{
	        Vector3 v3Hit   = ray.GetPoint (fDist);
	        Vector3 v3Delta = location - v3Hit;
	        //Camera.main.transform.Translate (v3Delta);
			//v3Delta.y = 5.3f;
			TargetLocation += v3Delta;
			
			Debug.Log (v3Delta);
			
       	}
		
		//var position = Camera.main.transform.rotation * new Vector3(0f, -5.3f, 0f) + new Vector3(location.x,
			//0f, location.z);
		
		
		
		//this.transform.LookAt(location);
	}
	
	private void OnMouseDown()
	{
		RaycastHit hit;
		
		if (Physics.Raycast(camera.ScreenPointToRay (Input.mousePosition), out hit))
		{
			//print("Hit Triangle Index: " + hit.triangleIndex);
			GameObject hitCube = hit.collider.gameObject;
			//Debug.Log (hitCube.gameObject.name);
			
			
			Vector3 cubeLocation = hitCube.transform.localPosition;
			Debug.Log (cubeLocation);
			HandleFaceClick(cubeLocation.x, cubeLocation.y * 2.5f, cubeLocation.z, hit.triangleIndex);
		}
	}
	
	private void CreateInitialFloor()
	{
		for(int x = 0; x < MAX_WIDTH; ++x)
		{
			for (int z = 0; z < MAX_DEPTH; ++z)
			{
				AddCubeAtLocation(GrassCube.Value, x, 0, z);
			}
		}
	}
	
	private void AddCubeAtLocation(GameObject cubeToInstantiate, float x, float y, float z)
	{
		GameObject go = Instantiate(cubeToInstantiate, new Vector3(x * cubeToInstantiate.transform.localScale.x, 
			y * cubeToInstantiate.transform.localScale.y, z * cubeToInstantiate.transform.localScale.z), 
			Quaternion.identity) as GameObject;
		
		if (x >= MAX_WIDTH || y >= MAX_HEIGHT || z >= MAX_DEPTH)
			return;
		terrain_map[(int)x, (int)y, (int)z].CubeCode = SelectedCube.Key;
		
		//go.GetComponent<TerrainCube>().SetXYZ(x, y, z);
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
		/* Trying to add a cube on top of <x, y, z> */
		/* Can guarantee that x and z are within bounds.
		 * We should check to see our height is within
		 * bounds */
		Debug.Log ("Add Cube Top");
		if (y + 1 > MAX_HEIGHT)
		{
			Debug.Log("Cube placed to high");
			return;
		}
		
		AddCubeAtLocation(SelectedCube.Value, x, y + 1, z);
	}
	
	public void OnGUI()
	{
		if (GUI.Button (new Rect(Screen.width * .05f, Screen.height * .05f, 100f, 25f), "Save"))
		{
			WriteToFile("Test.map");
		}
		
		open_file_path = GUI.TextField(new Rect(Screen.width * .05f, Screen.height * .15f, 250, 25), open_file_path);
	
		if (GUI.Button (new Rect(Screen.width * .05f, (Screen.height * .05f) + 30, 100f, 25f), "Load Map"))
		{
			LoadFromFile(open_file_path);
		}
	}
	
	private void WriteToFile(string path)
	{
		using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
		{
			/* Lets write the width, height and depth of the map */
			writer.Write(MAX_WIDTH);
			writer.Write(',');
			writer.Write(MAX_HEIGHT);
			writer.Write(',');
			writer.Write(MAX_DEPTH);
			writer.Write(',');
			
			for(int x = 0; x < MAX_WIDTH; ++x)
			{
				for(int y = 0; y < MAX_HEIGHT; ++y)
				{
					for(int z = 0; z < MAX_DEPTH; ++z)
					{
						
						/* If we're at the last element (each condition in the for will become false */
						if (x == (MAX_WIDTH - 1) && y == (MAX_HEIGHT - 1) && z == (MAX_DEPTH - 1))
						{
							writer.Write(terrain_map[x,y,z].CubeCode);
						}
						else
						{
							/* Normal write */
							writer.Write(terrain_map[x,y,z].CubeCode);
							writer.Write(',');
						}
					}
				}
			}
		}
	}
	
	private void LoadFromFile(string path)
	{
		/* Delete all cubes */
		GameObject[] cubes = GameObject.FindGameObjectsWithTag("TerrainCube");
		
		foreach (GameObject go in cubes)
		{
			GameObject.Destroy(go);
		}
		
		using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
		{
			string contents_of_file = reader.ReadToEnd();
			
			string[] seperated_contents = contents_of_file.Split(',');
			
			MAX_WIDTH = System.Convert.ToInt32(seperated_contents[0]);
			MAX_HEIGHT = System.Convert.ToInt32(seperated_contents[1]);
			MAX_DEPTH = System.Convert.ToInt32(seperated_contents[2]);
			
			terrain_map = new TerrainCube[MAX_WIDTH, MAX_HEIGHT, MAX_DEPTH];
			
			int current_index = 3;
			
			for(int x = 0; x < MAX_WIDTH; ++x)
			{
				for(int y = 0; y < MAX_HEIGHT; ++y)
				{
					for(int z = 0; z < MAX_DEPTH; ++z)
					{	
						terrain_map[x, y, z] = new TerrainCube(x, y, z);
						
						terrain_map[x, y, z].CubeCode = System.Convert.ToInt32 (seperated_contents[current_index]);
						
						if (terrain_map[x, y, z].CubeCode != -1)
						{
							Debug.Log (terrain_map[x, y, z].CubeCode);
							AddCubeAtLocation(prefabs[terrain_map[x, y, z].CubeCode], x, y, z);
							
						}
						current_index++;
					}
				}
			}
		}
	}
}
