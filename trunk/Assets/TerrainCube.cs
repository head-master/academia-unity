using UnityEngine;
using System.Collections;

public class TerrainCube
{
	//public GameObject prefab;
	
	public string PrefabString;
	public int CubeCode;
	public int X
	{
		get;
		set;
	}
	
	public int Y
	{
		get;
		set;
	}
	
	public int Z
	{
		get;
		set;
	}
	
	public void SetXYZ(int x, int y, int z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}
	
	public Vector3 GetXYZVector3()
	{
		return new UnityEngine.Vector3(X, Y, Z);
	}
	
	public TerrainCube()
	{
		PrefabString = "None";
		X = 0;
		Y = 0;
		Z = 0;
		CubeCode = -1;
	}
	
	public TerrainCube(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
		/* -1 is open air */
		CubeCode = -1;
	}
	
	public override string ToString ()
	{
		return string.Format ("[TerrainCube: X={0}, Y={1}, Z={2}]", X, Y, Z);
	}
}
