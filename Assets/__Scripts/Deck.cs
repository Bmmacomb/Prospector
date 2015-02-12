using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : MonoBehaviour {
	// suits
	public Sprite suitClub;
	public Sprite suitDiamond;
	public Sprite suitHeart;
	public Sprite suitSpade;

	public Sprite[] faceSprites;
	public Sprite[] rankSprites;

	public Sprite cardBack;
	public Sprite cardBackGold;
	public Sprite cardFront;
	public Sprite cardFrontGold;

	// prefabs
	public GameObject prefabSprite;
	public GameObject prefabCard;



	public bool ________________;
	public PT_XMLReader xmlr;
	public List<string> cardNames;
	public List<Card>   cards;
	public List<Decorator> decorators;
	public List<CardDefinition> cardDefs;
	public Transform deckAnchor;
	public Dictionary<string,Sprite> dictSuits;

	// init deck called by solitare when it is ready
	public void InitDeck(string deckXMLText){
		// this creates an anchor for card gameobjects in the hiarachy
		if (GameObject.Find ("deck") == null) {
			GameObject anchorGO = new GameObject("deck");
			deckAnchor = anchorGO.transform;
		}
		// initalize dictionary of suit sprites
		dictSuits = new Dictionary<string,Sprite> (){
			{"C" , suitClub},
			{"D" , suitDiamond},
			{"H",suitHeart},
			{"S",suitSpade}

		};

		ReadDeck (deckXMLText);
		MakeCards ();
	}

	// this parses the xml file
	public void ReadDeck(string deckXMLText){
		xmlr = new PT_XMLReader ();
		xmlr.Parse (deckXMLText);

		//this prints a test line
		string s = "xml[0] decorator[0] ";
		s += "type=" + xmlr.xml["xml"][0]["decorator"][0].att ("type");
		s += " x=" + xmlr.xml ["xml"] [0] ["decorator"] [0].att ("x");
		s += " y=" + xmlr.xml ["xml"] [0] ["decorator"] [0].att ("y");
		s += " scale="+xmlr.xml["xml"][0]["decorator"][0].att("scale");
		//print (s);


		//read decorators for all cards
		decorators = new List<Decorator> (); //init the list
		// grab a PT_XMLHashlist of all decorators in XML file
		PT_XMLHashList xDecos = xmlr.xml ["xml"] [0] ["decorator"];
		Decorator deco;
		for (int i = 0; i < xDecos.Count; i++) {
		// for each decorator
			deco =  new Decorator();
			//copy attributes of the <deorator> to the Decorator
			deco.type = xDecos[i].att ("type");
			// set the flip bool based on if the text is 1
			deco.flip = (xDecos[i].att ("flip") =="1");
			// floats need to be parsed from the attribute strings
			deco.scale = float.Parse(xDecos[i].att ("scale"));

			// location vectors
			deco.loc.x = float.Parse(xDecos[i].att ("x"));
			deco.loc.y = float.Parse(xDecos[i].att ("y"));
			deco.loc.z = float.Parse(xDecos[i].att ("z"));
			// add the temporary decorator to the list
			decorators.Add(deco);

		
		
		
		
		}


		// read pip locations for each card num
		cardDefs = new List<CardDefinition> ();// init list of cards
		// grab a PT_XMLHashlist of all the cards
		PT_XMLHashList xCardDefs = xmlr.xml ["xml"] [0] ["card"];
		for (int i =0; i < xCardDefs.Count; i++) {
			// for each card
			// create a card defintion
			CardDefinition cDef = new CardDefinition();
			// parse attribute values and add them to Cdf
			cDef.rank = int.Parse(xCardDefs[i].att ("rank"));
			// grab  a PT_XMLHashtable of all the pips on the card
			PT_XMLHashList xPips = xCardDefs[i]["pip"];
			if (xPips != null){
				for(int j = 0; j < xPips.Count; j++){
					// iterate through all pips
					deco = new Decorator();
					// pips on the card are handled through the decorator class
					deco.type = "pip";
					deco.flip = (xPips[j].att ("flip") == "1");
					deco.loc.x = float.Parse(xPips[j].att ("x"));
					deco.loc.y = float.Parse(xPips[j].att ("y"));
					deco.loc.z = float.Parse(xPips[j].att ("z"));
					if (xPips[j].HasAtt("scale")){
						deco.scale = float.Parse(xPips[j].att ("scale"));
					}
					cDef.pips.Add(deco);


					

				}

			}
			// face cards have a face attribute
			// cdef.face is the name of this
			if (xCardDefs[i].HasAtt("face")){
				cDef.face = xCardDefs[i].att ("face");
			}
			cardDefs.Add (cDef);
				
		
		
		}
	}
	// get proper card definition based on rank (1 -14) Ace to King
	public CardDefinition GetCardDefinitionByRank(int rnk){
		// search through all card defintions 
		foreach (CardDefinition cd in cardDefs) {
			// if rank is correct return this definition
			if (cd.rank == rnk){
				return(cd);
			}
		}
		return(null);
	
	}

	// make card Game Objects
	public void MakeCards(){
		// card names will be names of cards to build
		// each suit goes from 1 to 13
		cardNames = new List<string>();
		string[] letters = new string[]{"C","D","H","S"};
		foreach (string s in letters) {
			for (int i = 0 ; i<13; i++){
				cardNames.Add(s + (i+1));
			}
		}
		// make list to hold all the cards
		cards = new List<Card> ();
		// several varibles are reused several times
		Sprite tS = null;
		GameObject tGO = null;
		SpriteRenderer tSR = null;

		// itterate thru all cards just created
		for (int i = 0; i < cardNames.Count; i++){

			// create new card game object
			GameObject cgo = Instantiate (prefabCard) as GameObject;
			// set transform.parent to anchor
			cgo.transform.parent = deckAnchor;
			Card card = cgo.GetComponent<Card>();

			// This line is for debug purposes as it displays all the cards
			cgo.transform.localPosition = new Vector3((i%13)*3, i/13*4,0);

			// assign basic values to card
			card.name = cardNames[i];
			card.suit = card.name[0].ToString();
			card.rank = int.Parse(card.name.Substring(1));
			if (card.suit == "D" || card.suit == "H"){
				card.colS = "Red";
				card.color = Color.red;
			}
			// pull card def for this card
			card.def = GetCardDefinitionByRank(card.rank);

			// add decorators
			foreach(Decorator deco in decorators){
				if (deco.type == "suit"){
					// instanciate a game object
					tGO = Instantiate(prefabSprite) as GameObject;
					// get sprite renderer
					tSR=tGO.GetComponent<SpriteRenderer>();
					// set to proper suit
					tSR.sprite = dictSuits[card.suit];
				}else{
					tGO= Instantiate(prefabSprite) as GameObject;
					tSR = tGO.GetComponent<SpriteRenderer>();
					// get proper sprite for this rank
					tS = rankSprites[card.rank];
					// assign to sprite renderer
					tSR.sprite = tS;
					// set color to match suit
					tSR.color = card.color;


				}
				// make deco sprites render above card
				tSR.sortingOrder= 1;
				// make decorator sprite a child of card
				tGO.transform.parent = cgo.transform;
				// set local pos based on loc from DeckXML
				tGO.transform.localPosition = deco.loc;
				// flip if needed
				if (deco.flip){
					tGO.transform.rotation = Quaternion.Euler(0,0,180);
				}
				// set scale
				if (deco.scale != 1){
					tGO.transform.localScale= Vector3.one * deco.scale;
				}
				// name it so it's easy to find
				tGO.name = deco.type;
				// add to list
				card.decoGOs.Add(tGO);
			}
			// add pips
			foreach(Decorator pip in card.def.pips){
				// instantiate a sprite
				tGO = Instantiate(prefabSprite) as GameObject;
				// set parent to game object
				tGO.transform.parent = cgo.transform;
				// set position as specified in the xml
				tGO.transform.localPosition = pip.loc;
				// flip if needed
				if (pip.flip){
					tGO.transform.rotation = Quaternion.Euler(0,0,180);

				}
				// scale if nessicary, only for ace
				if (pip.scale != 1){
					tGO.transform.localScale = Vector3.one * pip.scale;
				}

				// give a name
				tGO.name = "pip";
				// get sprite renderer
				tSR = tGO.GetComponent<SpriteRenderer>();
				// set to proper suit
				tSR.sprite = dictSuits[card.suit];
				// set sort order so sprite is rendered above front
				tSR.sortingOrder = 1;
				// add to list of pips
				card.pipGOs.Add (tGO);
			}

			// face cards
			if (card.def.face != ""){
				tGO = Instantiate(prefabSprite) as GameObject;
				tSR = tGO.GetComponent<SpriteRenderer>();
				// gen the right name and pass to get face()
				tS = GetFace(card.def.face+card.suit);
				tSR.sprite = tS;
				tSR.sortingOrder=1;
				tGO.transform.parent = card.transform;
				tGO.transform.localPosition = Vector3.zero;
				tGO.name = "face";
			}

			// add card back
			// back covers everything on card
			tGO = Instantiate(prefabSprite) as GameObject;
			tSR = tGO.GetComponent<SpriteRenderer>();
		

			tSR.sprite=cardBack;
			
			tGO.transform.parent = card.transform;
			tGO.transform.localPosition = Vector3.zero;
			// give it higher sort order then anything else
			tSR.sortingOrder = 2;
			tGO.name = "back";
			card.back = tGO;

			// face up is default

			card.faceUp = true;







			cards.Add (card);
		}
	}
	// find proper face sprite
	public Sprite GetFace(string faceS){

		foreach (Sprite tS in faceSprites) {
			//print (tS.name + " " + faceS);
			// if it has correct name 
			if (tS.name == faceS){

				return (tS);
			}

		}
		return (null);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	static public void Shuffle(ref List<Card>oCards){
		// create a temp list to hold new order
		List<Card> tCards = new List<Card> ();

		int ndx; // this will hold the index of the card to be moved
		tCards = new List<Card>();// init temp list
		// repeat as long as there are cards in original list
		while (oCards.Count > 0) {
			// pick random card
			ndx = Random.Range(0,oCards.Count);
			// add that card to temp list
			tCards.Add (oCards[ndx]);
			// remove from orijinal list
			oCards.RemoveAt(ndx);
		}
		// replace original list with temp list
		oCards = tCards;
	}
}
