using UnityEngine;
using System.Collections;
using TouchScript.Gestures.Simple;
using TouchScript.Gestures;

public class PanCamera : MonoBehaviour 
{
	public float speed_multiplier = .25f;
	Vector3 TargetLocation = Vector3.zero;
	

	// Use this for initialization
	void Start () 
	{
		GetComponent<SimplePanGesture>().StateChanged += HandleStateChanged;
		TargetLocation = transform.localPosition;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float dt = Time.deltaTime * 5.0f;
		
		transform.localPosition = Vector3.Lerp (transform.localPosition, TargetLocation, dt);
	}
	
	void HandleStateChanged(object sender, TouchScript.Events.GestureStateChangeEventArgs e)
	{
		Debug.Log("In PanCamera HandleStateChanged!!!????!?!");
		
		switch(e.State)
		{
		case Gesture.GestureState.Began:
		case Gesture.GestureState.Changed:
			
			var gesture = (SimplePanGesture)sender;
			
			if (gesture.LocalDeltaPosition != Vector3.zero)
			{
				Debug.Log ("Got Delta!");
				Vector3 delta = gesture.LocalDeltaPosition;
				
				TargetLocation += new Vector3(delta.x, 0, delta.y) * Time.deltaTime * speed_multiplier;
			}
			break;
			
		}
		
	}
}
