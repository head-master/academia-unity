/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;

var targetPosition = Vector3.zero;
// The distance in the x-z plane to the target
var distance = 10.0;
// the height we want the camera to be above the target
var height = 5.0;
// How much we 
var heightDamping = 2.0;
var rotationDamping = 3.0;

var theHeight;

var currentTime = 0.0;
var TimeToMove = 5.0;

var previousHeight = 5.3f;

var HeightOfCamera = 5.3f;

var previousPosition = Vector3.zero;

// Place the script in the Camera-Control group in the component menu
@script AddComponentMenu("Camera-Control/Smooth Follow")

function DoTargetUpdate()
{
	// Calculate the current rotation angles
	var wantedRotationAngle = target.eulerAngles.y;
	theHeight = target.position.y + height;
		
	var currentRotationAngle = transform.eulerAngles.y;
	var currentHeight = transform.position.y;
	
	// Damp the rotation around the y-axis
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

	// Damp the height
	//currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

	// Convert the angle into a rotation
	var currentRotation = Quaternion.Euler (0, 0f, 0);
	
	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	
	//Vector3 targetposition = target.position - currentRotation * Vector3.forward * distance;
	
	targetPosition = target.position;
	targetPosition -= currentRotation * Vector3.forward * distance;

	// Set the height of the camera
	targetPosition.y = theHeight;
	
	// Always look at the target
	target.LookAt (target);
}
function LateUpdate () {
	// Early out if we don't have a target
	if (!target)
		return;
	
	currentTime += Time.deltaTime;
	var percent = currentTime / TimeToMove;
		
	this.transform.localPosition = Vector3.Lerp(previousPosition,
		targetPosition, percent);
		
	
		
	this.transform.position.y = Mathf.Lerp(previousHeight, HeightOfCamera, percent);
}

function SetTarget(target, cubeHeight)
{
	previousHeight = HeightOfCamera;
	HeightOfCamera = cubeHeight + 5.3f;
	this.target = target;
	currentTime = 0.0;
	previousPosition = this.transform.position;
	DoTargetUpdate();
	
	
}