using UnityEngine;
using System.Collections;

public class TerrainCubeController : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void ActivatePanelOverlay()
	{
		SetPanelOverlayRender(true);
	}
	
	public void DeactivatePanelOverlay()
	{
		SetPanelOverlayRender (false);
	}
	
	private void SetPanelOverlayRender(bool val)
	{
		Component render = GetComponentsInChildren<MeshRenderer>()[1];

		((MeshRenderer)render).enabled = val;
	}
	
	public void SetPanelOverlayColor(Color color)
	{
		MeshRenderer render = GetComponentsInChildren<MeshRenderer>()[1];
		Color c = new Color(0f, 0f, .75f, .5f);
		render.material.color = c;
	}
}
