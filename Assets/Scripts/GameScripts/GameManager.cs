using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private GameObject gameCanvas;
	private PlayerClass[] playersArray;
	private LoadPlayers loader =  new LoadPlayers ();
	private int playerCreated = 0;

	[Header("Cars Starting Point")]
	public GameObject startingPoint;

	[Header("Game Management")]
	public string filepath = "/StreamingAssets/data.txt";
	public int numberOfCars = 8;


	[Header("Relevant Info")]
	public int lapsToComplete = 0;
	public float spawnTime = 0.0f;

	[Header("Leader Board")]
	//public List<Vector3> leaderBoardPos = new List<Vector3>();
	public List<GameObject> carObjects = new List<GameObject>();
	public List<GameObject> carInfoObjects = new List<GameObject>();
	public GameObject resetCanvas;
	public Text winner;

	#region Base Functions
	void Awake(){
		CreatePlayersArray ();
		gameCanvas = GameObject.Find ("GameCanvas");
	}

	void Start () {
		StartCoroutine( "InstantiatePlayers");
	}
		
	#endregion

	#region Game Logic Functions

	void CreatePlayersArray(){
		loader.decodeString ( filepath, numberOfCars  );
		lapsToComplete = loader.numberOfLaps;
		spawnTime =  float.Parse(loader.miliSecondsDelay) / 1000 ;
		playersArray = loader.playersArray;
	}

	IEnumerator InstantiatePlayers (){
		
		GameObject raceCar;
		AICarScript raceCarAI;

		GameObject playerCarData;
		PlayerInfoUI playerCarDataUI;


		if (playerCreated < playersArray.Length) {
			
			raceCar = Instantiate (Resources.Load ("SkyCar")) as GameObject;
			raceCar.transform.position = startingPoint.transform.position;
			raceCarAI = raceCar.GetComponent<AICarScript> ();
			raceCarAI.playerName = playersArray [playerCreated].playerName;
			raceCarAI.velocity = playersArray [playerCreated].velocity;
			raceCarAI.bodyColor = playersArray [playerCreated].bodyColor;
			raceCarAI.iconString = playersArray [playerCreated].iconString;
			raceCarAI.gameManager = gameObject.GetComponent<GameManager> ();

			carObjects.Add (raceCar);

			playerCarData = Instantiate (Resources.Load("PlayerData") ) as GameObject;
			playerCarData.transform.SetParent (gameCanvas.transform);

			playerCarData.transform.localPosition = new Vector3(-335,( 210 - (40 * playerCreated)));
			playerCarData.transform.localRotation = Quaternion.identity;
			playerCarData.transform.localScale = new Vector3 (1,1,1);

			//leaderBoardPos.Add (playerCarData.transform.localPosition);

			playerCarDataUI = playerCarData.GetComponent<PlayerInfoUI> ();
			raceCarAI.carInfoUI = playerCarDataUI;

			carInfoObjects.Add (playerCarData);

			playerCreated++;

			yield return new WaitForSeconds ( spawnTime );
			StartCoroutine ("InstantiatePlayers");

		} else {
			playerCreated = 0;
			yield return new WaitForSeconds ( 0.000001f);
		}
			
	}

	public void CheckConditions (GameObject car,int lap, int remaingNode){
		if (lap >= lapsToComplete) {
			CleanGameElements ();
			winner.text += '\n'+ car.GetComponent<AICarScript>().playerName;
			resetCanvas.SetActive (true);

		} 
	}



	void CleanGameElements(){
		
		foreach (GameObject carObject in carObjects) {
			Destroy (carObject);
		}

		carObjects.Clear ();

		foreach (GameObject carInfoObject in carInfoObjects) {
			Destroy (carInfoObject);
		}
		carInfoObjects.Clear ();


	}

	public void ResetGame(){
		Debug.Log ("Reset Function");
		CreatePlayersArray ();
		resetCanvas.SetActive (false);
		Debug.Log ("Re-creatiing Assets");
		StartCoroutine( "InstantiatePlayers");



	
	}

	#endregion


}
