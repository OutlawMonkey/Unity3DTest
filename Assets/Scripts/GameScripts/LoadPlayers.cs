using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayers {

	public int numberOfLaps = 0;
	public string miliSecondsDelay = "";
	public PlayerClass[] playersArray;

	private string ReadFile(string file){
		
		string fileContent = "";
		string filePath = Application.dataPath + file;


		StreamReader fileReader = new StreamReader(filePath,Encoding.Default);
		fileContent = fileReader.ReadToEnd ();
		fileReader.Close ();

		return fileContent;
	}

	public void decodeString (string file, int numberOfCars){

		JSONObject jsonArray = new JSONObject ( ReadFile( file) );
		PlayerClass[] decodedArray;

		string testParse = "";

		numberOfLaps = int.Parse ( jsonArray.list[0].list[0].str );

		testParse += "number of laps: "+ numberOfLaps+'\n';

		miliSecondsDelay = jsonArray.list[0].list[1].str;

		testParse += "delay miliseconds: "+ miliSecondsDelay+'\n';

		decodedArray = new PlayerClass[numberOfCars];

		testParse += "***********************************************"+'\n';
		testParse += "Data Array (Not Sorted)"+'\n';

		for (int i = 0; i < numberOfCars; i++) {
			
			PlayerClass tempPlayer = new PlayerClass ();

			Color tempColor = new Color ();

			int arrayIndex = Random.Range(0 , int.Parse(jsonArray.list[1].Count.ToString()) );

			tempPlayer.playerName = jsonArray.list[1].list[arrayIndex].list[0].str;
			tempPlayer.velocity = float.Parse( jsonArray.list[1].list[arrayIndex].list[1].str );
			ColorUtility.TryParseHtmlString( 
											jsonArray.list[1].list[arrayIndex].list[2].str,
											out tempColor);
			tempPlayer.bodyColor = tempColor;
			tempPlayer.iconString = jsonArray.list[1].list[arrayIndex].list[3].str;

			testParse += "--------------------------------------------------"+'\n';
			testParse += "name: "+ tempPlayer.playerName+'\n';
			testParse += "velocity: "+ tempPlayer.velocity+'\n';
			testParse += "bodyColor: "+ tempPlayer.bodyColor+'\n';
			testParse += "iconString: "+ tempPlayer.iconString+'\n';

			decodedArray[i] = tempPlayer;
		}
			
		testParse += "***********************************************"+'\n';

		sortArrayPlayers (decodedArray);
		Debug.Log (testParse);

	}

	private void sortArrayPlayers(PlayerClass[] inputArray){

		PlayerClass tempPlayer = new PlayerClass ();
		string testParse = "Sorted Array" + '\n';
		for (int i = 0; i<inputArray.Length; i++ ){

			for(int j = 0; j<inputArray.Length-1; j++){
			
				if( inputArray[j].velocity > inputArray[j+1].velocity){
					tempPlayer = inputArray [j + 1];
					inputArray [j + 1] = inputArray [j];
					inputArray [j] = tempPlayer;
					
				}

			}
			
		}

		for(int x = 0; x < inputArray.Length; x++){
			testParse += "--------------------------------------------------"+'\n';
			testParse += "name: "+ inputArray[x].playerName+'\n';
			testParse += "velocity: "+ inputArray[x].velocity+'\n';
			testParse += "bodyColor: "+ inputArray[x].bodyColor+'\n';
			testParse += "iconString: "+ inputArray[x].iconString+'\n';
		}

		Debug.Log (testParse);

		playersArray = inputArray;

	}

}
