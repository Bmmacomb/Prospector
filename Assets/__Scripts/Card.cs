// code from textbook prospector chapter recoded for this project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour {
	virtual public void OnMouseUpAsButton(){
		//print (name);
	}
	public string suit; // ( C D H or S)
	public int rank; // value 1 - 14
	public Color color = Color.black;
	public string colS = "Black";
	public bool isGold;
	// this list holds all the decorators
	public List<GameObject> decoGOs = new List<GameObject>();
	// this list holds all the pips
	public List<GameObject> pipGOs = new List<GameObject>();

	public GameObject back;  // the back of the card

	public CardDefinition def; // parsed from DeckXML.xml
	public SpriteRenderer[] spriteRenderers;


	public bool faceUp{
		get {
			return (!back.activeSelf);
		}
		set{
			back.SetActive(!value);
		}
	}
	public void PopulateSpriteRenderers(){
		if (spriteRenderers == null || spriteRenderers.Length == 0) {
			spriteRenderers = GetComponentsInChildren<SpriteRenderer>();	
		}
	}

	public void SetSortingLayerName(string tSLN){
		PopulateSpriteRenderers ();
		foreach (SpriteRenderer tSR in spriteRenderers) {
			tSR.sortingLayerName = tSLN;



		}

	}

	public void SetSortOrder(int sOrd){
		PopulateSpriteRenderers ();


		foreach (SpriteRenderer tSR in spriteRenderers) {
			if (tSR.gameObject == this.gameObject){
				tSR.sortingOrder = sOrd;
				continue;
			}
			switch(tSR.gameObject.name){
			case "back":
				tSR.sortingOrder = sOrd+2;
				break;
			case "face":
			default: tSR.sortingOrder = sOrd+1;
				break;
			}
		}

	}




	// Use this for initialization
	void Start () {
		SetSortOrder (0);
	
	}


	
	// Update is called once per frame
	void Update () {
	
	}


}
[System.Serializable]
public class Decorator{

	public string type; // for card pips
	public Vector3 loc; // location of pip on the card
	public bool flip = false; // Wheather the sprite is inverted
	public float scale = 1f; // the scale of the sprite
}

[System.Serializable]
public class CardDefinition{
	public string   face; //used for face card
	public int      rank; // a number 1-13
	public List<Decorator> pips = new List<Decorator> (); //pips used



}
