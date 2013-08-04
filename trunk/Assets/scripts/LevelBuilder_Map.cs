using UnityEngine;

public class LevelBuilder_Map
{
    private TerrainCube[, ,] terrain_map;

    public int width;
	public int height;
	public int depth;
	
	public LevelBuilder_Map()
	{
		width = 1;
		height = 1;
		depth = 1;
		
		InitMap();
	}
	
	public LevelBuilder_Map(int width, int height, int depth)
	{
		this.width = width;
		this.height = height;
		this.depth = depth;
		
		InitMap();
	}
	
	public int GetCubeCode(int x, int y, int z)
	{
		return terrain_map[x, y, z].CubeCode;
	}
	
	private void InitMap()
	{
		terrain_map = new TerrainCube[width, height, depth];
		for(int x = 0; x < width; ++x)
		{
			for(int y = 0; y < height; ++y)
			{
				for(int z = 0; z < depth; ++z)
				{
					terrain_map[x, y, z] = new TerrainCube(x, y, z);
				}
			}
		}
	}
	
	public void AddBlock(int cubeCode, int x, int y, int z)
	{
		terrain_map[x,y,z].CubeCode = cubeCode;
	}
	
		
	public void WriteToFile(string path)
	{
		using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
		{
			/* Lets write the width, height and depth of the map */
			writer.Write(width);
			writer.Write(',');
			writer.Write(height);
			writer.Write(',');
			writer.Write(depth);
			writer.Write(',');
			
			for(int x = 0; x < width; ++x)
			{
				for(int y = 0; y < height; ++y)
				{
					for(int z = 0; z < depth; ++z)
					{
						
						/* If we're at the last element (each condition in the for will become false */
						if (x == (width - 1) && y == (height - 1) && z == (depth - 1))
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
	
	public void LoadFromFile(string path)
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
			
			width = System.Convert.ToInt32(seperated_contents[0]);
			height = System.Convert.ToInt32(seperated_contents[1]);
			depth = System.Convert.ToInt32(seperated_contents[2]);
			
			terrain_map = new TerrainCube[width, height, depth];
			
			int current_index = 3;
			
			for(int x = 0; x < width; ++x)
			{
				for(int y = 0; y < height; ++y)
				{
					for(int z = 0; z < depth; ++z)
					{	
						terrain_map[x, y, z] = new TerrainCube(x, y, z);
						
						terrain_map[x, y, z].CubeCode = System.Convert.ToInt32 (seperated_contents[current_index]);
						
						current_index++;
					}
				}
			}
		}
	}
}

