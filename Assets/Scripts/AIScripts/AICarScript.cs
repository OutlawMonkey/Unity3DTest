using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarScript : MonoBehaviour {

	private Rigidbody rigidBody;


	public PlayerInfoUI carInfoUI;
	public GameManager gameManager;

	[Header ("Player Data")]
	public string playerName = " ";
	public float velocity = 0.0f;
	public Color bodyColor;
	public string iconString = "";
	public int lapsCompleted = 0;

	[Header ("Navigation")]
	public Transform[] path;
	public GameObject pathGroup;
	public int currentPathObj;
	public int remainingNodes;
	public float distFromPath = 20f;

	[Header ("Specs")]
	public float  maxSteer = 22f;
	public float maxTorque = 500f;
	public float currentSpeed;
	public float topSpeed = 150f;
	public float decelerationSpeed = 15f;
	public Transform centerOfMass;

	[Header ("Wheel Colliders")]
	public WheelCollider frontLeft;
	public WheelCollider frontRight;
	public WheelCollider rearLeft;
	public WheelCollider rearRight;


	[Header ("AI Sensors")]
	public Color sensorColor = Color.white;
	public float sensorLength = 30f;
	public float frontSensorStartPoint = 2.52f;
	public float frontSensorSideDistance =  1f;
	public float frontSensorAngle = 30f;
	public float sidewaySensorLength = 25f;
	public float avoidSpeed = 15f;
	private int detectionFlag = 0;
	public float respawnWait = 1.5f;
	public float respawnCounter = 0.0f; 


	#region Base Functions

	void Awake(){
		pathGroup = GameObject.Find ("Path");
		rigidBody = gameObject.GetComponent<Rigidbody> ();
		rigidBody.centerOfMass = centerOfMass.localPosition;
	}

	void Start () {
		Renderer renderer = transform.GetChild (0).gameObject.GetComponent<Renderer>();
		Material material = renderer.material;

		material.SetColor ("_Color",bodyColor);
		GetPath ();
		carInfoUI.playerName.text = playerName;
		carInfoUI.iconFile = iconString;
		carInfoUI.updateLaps(lapsCompleted+1);
		//topSpeed = velocity;
	}
	

	void Update () {

		if (detectionFlag == 0){
			GetSteer ();
		}

		Move ();
		Sensors ();
		Respawn ();
	}

	#endregion


	#region Navigation Functions

	void GetPath(){

		Transform [] path_nodes = pathGroup.GetComponentsInChildren <Transform>() ;

		path = new Transform[path_nodes.Length-1];

		for(int j = 1 ; j < path_nodes.Length; j++){
			path[j-1] = path_nodes[j];
		}
		remainingNodes = path.Length;
	}

	#endregion

	#region Movement Functions

	void GetSteer(){
		Vector3 steerVector = transform.InverseTransformPoint ( new Vector3 (path[currentPathObj].position.x,
																		transform.position.y,
																		path[currentPathObj].position.z) );
		
		float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
		frontLeft.steerAngle = newSteer;
		frontRight.steerAngle = newSteer;

		if (steerVector.magnitude <= distFromPath) {
			currentPathObj++;
			remainingNodes--;
			if (currentPathObj >= path.Length) {
				currentPathObj = 0;
				remainingNodes = path.Length;
				lapsCompleted++;
				carInfoUI.updateLaps( lapsCompleted+1 );
				gameManager.CheckConditions (gameObject, lapsCompleted, remainingNodes);
			}
		}


	}


	void Move(){

		//currentSpeed = 2 * Mathf.PI * rearLeft.radius * rearLeft.rpm * 60 / 1000;
		//currentSpeed = Mathf.Round (currentSpeed);

		currentSpeed = velocity;

		if (currentSpeed <= topSpeed) {
			
			rearLeft.motorTorque = maxTorque;
			rearRight.motorTorque = maxTorque;

			rearLeft.brakeTorque = 0f;
			rearRight.brakeTorque = 0f;

		} else {
			rearLeft.motorTorque = 0f;
			rearRight.motorTorque = 0f;
			rearLeft.brakeTorque = decelerationSpeed;
			rearRight.brakeTorque = decelerationSpeed;
		}


	}


	#endregion

	#region AI Functions

	void Sensors(){

		detectionFlag = 0;
		float avoidSensitivity = 0f;


		Vector3 pos; 
		RaycastHit hit;

		Vector3 rightAngle = Quaternion.AngleAxis (frontSensorAngle,transform.up) * transform.forward;
		Vector3 leftAngle = Quaternion.AngleAxis (-frontSensorAngle,transform.up) * transform.forward;


		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;

		//----------------------------- Braking Sensor -------------------------------------------

		if ( Physics.Raycast (pos, transform.forward, out hit ,sensorLength) ) {

			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				rearLeft.brakeTorque = decelerationSpeed;
				rearRight.brakeTorque = decelerationSpeed;
				Debug.Log ("Avoiding Right");
				Debug.DrawLine (pos, hit.point, sensorColor);
			} else {
				rearLeft.brakeTorque = 0;
				rearRight.brakeTorque = 0;
			}


		}


		//----------------------------- Right Sensor -------------------------------------------

		pos += transform.right * frontSensorSideDistance;

		if ( Physics.Raycast (pos, transform.forward, out hit ,sensorLength) ) {

			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity -= 1f;
				Debug.Log ("Avoiding Right");
				Debug.DrawLine (pos, hit.point, sensorColor);
			}

				
		}else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength)){
			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity -= 0.5f;
				Debug.Log ("Avoiding Right");
				Debug.DrawLine (pos, hit.point, sensorColor);
			}
			//Debug.DrawLine(pos, hit.point, sensorColor);
		}

		//----------------------------- Left Sensor -------------------------------------------

		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;
		pos -= transform.right * frontSensorSideDistance;

		if ( Physics.Raycast (pos, transform.forward, out hit ,sensorLength) ) {

			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity += 1f;
				Debug.Log ("Avoiding Left");
				Debug.DrawLine (pos, hit.point, sensorColor);
			}


		} else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength))
		{
			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity += 0.5f;
				Debug.Log ("Avoiding Left");
				Debug.DrawLine (pos, hit.point, sensorColor);
			}

			//Debug.DrawLine(pos, hit.point, sensorColor);
		}


		// ----------------------------- Side Right -----------------------------------------

		if(Physics.Raycast(transform.position, transform.right, out hit, sidewaySensorLength))
		{
			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity -= 0.5f;
				Debug.Log ("Avoiding Right");
				Debug.DrawLine(transform.position, hit.point, sensorColor);
			}

		}

		// ----------------------------- Side Left --------------------------------------------

		if (Physics.Raycast(transform.position, -transform.right, out hit, sidewaySensorLength))
		{
			if (hit.transform.tag != "Untagged") {
				detectionFlag++;
				avoidSensitivity += 0.5f;
				Debug.Log ("Avoiding Left");
				Debug.DrawLine(transform.position, hit.point, sensorColor);
			}
			//Debug.DrawLine(transform.position, hit.point, sensorColor);
		}

		//----------------------------- Mid Sensor -------------------------------------------

		pos = transform.position;
		pos += transform.forward * frontSensorStartPoint;

		if (avoidSensitivity == 0) {
			if ( Physics.Raycast (pos, transform.forward, out hit ,sensorLength) ) {

				if (hit.transform.tag != "Untagged") {
					//detectionFlag++;
					if (hit.normal.x < 0) {
						avoidSensitivity = -1f;
					}
					else { 
						avoidSensitivity = 1f;
					}
					Debug.Log ("Avoiding Mid");
					Debug.DrawLine (pos, hit.point, sensorColor);
				}


			}
		}
			

		//------------------------------- Flag Validation-------------------------------------
		
		if (detectionFlag != 0){
			AvoidSteer (avoidSensitivity);
		}

	}

	void AvoidSteer(float sensitivity){
		frontLeft.steerAngle = avoidSpeed * sensitivity;
		frontRight.steerAngle = avoidSpeed * sensitivity;
	}


	void Respawn(){
	
		if (rigidBody.velocity.magnitude < 2) {
			respawnCounter += Time.deltaTime;
			if(respawnCounter >= respawnWait){
				
				if (currentPathObj == 0) {
					gameObject.transform.position = path [path.Length - 1].position;
				} else {
					gameObject.transform.position = path [currentPathObj - 1].position;
				}
				respawnCounter = 0;
				gameObject.transform.Rotate(gameObject.transform.rotation.x, gameObject.transform.rotation.y, 0);
			}
		}	
		
	}

	#endregion

}
