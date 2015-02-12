using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CardState{
	drawpile,
	tableau,
	target,
	discard
}

public class CardProspector : Card {
	public CardState state = CardState.drawpile;
	public List<CardProspector> hiddenBy = new List<CardProspector>();
	public int layoutID;
	public SlotDef slotDef;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public void OnMouseUpAsButton(){
		Solitaire.S.CardClicked (this);
		base.OnMouseUpAsButton ();
	}
}
