using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour {

	public Image playerImage;
	//public Sprite playerImage;
	public Text playerName;
	public Text lapsCompleted;
	public string iconFile;


	// Use this for initialization
	void Start () {
		StartCoroutine ("getIcon");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateLaps(int lap){
		lapsCompleted.text = "Lap: "+ lap + "/4";
	}

	public IEnumerator getIcon(){
		WWW imageTexture = new WWW (iconFile);
		yield return imageTexture;
		playerImage.overrideSprite= Sprite.Create(
										imageTexture.texture,
										new Rect(0,0,imageTexture.texture.width,imageTexture.texture.height),
										new Vector2(0.5f,0.5f)) ;

	}
}
