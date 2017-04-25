using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterStatusScript : MonoBehaviour {

	public GameObject cellgrid;
	public List<Slider> sliders;
	public List<Text> texts;
	public GameObject charImage;

	float tempHP = 0;
	float tempIP = 0;
	float tempRB = 0;
	float tempCT = 0;

	// Update is called once per frame
	void Update () {
		//Turn indicator, who plays
		texts[0].text = "Player " + cellgrid.GetComponent<CellGrid> ().CurrentUnit.PlayerNumber;
		//Character name
		texts[1].text = cellgrid.GetComponent<CellGrid> ().CurrentUnit.name;
		//Character image
		//Debug.Log ("HEllo??");

		if (charImage != cellgrid.GetComponent<CellGrid> ().CurrentUnit.UnitClass.Avatar) {
			//Debug.Log ("HEllo");
			if(charImage.activeInHierarchy) charImage.SetActive (false);
			charImage = cellgrid.GetComponent<CellGrid> ().CurrentUnit.UnitClass.Avatar;
			//charImage.SetActive (true);
		}
		if (!charImage.activeInHierarchy)
			charImage.SetActive (true);
		float delta = Time.deltaTime;
		SlideHP (sliders [0], cellgrid.GetComponent<CellGrid> ().CurrentUnit.TotalHitPoints, cellgrid.GetComponent<CellGrid> ().CurrentUnit.HitPoints, delta);
		SlideIP (sliders [1], cellgrid.GetComponent<CellGrid> ().CurrentUnit.TotalImaginationPoints, cellgrid.GetComponent<CellGrid> ().CurrentUnit.ImaginationPoints, delta);
		SlideRB (sliders [2], 100f, cellgrid.GetComponent<CellGrid> ().CurrentUnit.RealityBreak, delta);
		SlideCT (sliders [3], 100f, cellgrid.GetComponent<CellGrid> ().CurrentUnit.ChargeTime, delta);
	}

	//Slides the sliders based on max and current value
	private void SlideHP(Slider temp, float max, float value, float delta){
		/*if (tempHP <= max) {
			//temp.maxValue = max;
			tempHP = tempHP + (value - tempHP) * delta;
			temp.value = tempHP;
		} else {
			//temp.value = value;
			tempHP = tempHP + (max - tempHP) * delta;
			temp.maxValue = tempHP;
		}*/
		tempHP = (value / max) * 100 + ((value / max) * 100 - tempHP) * delta;
		temp.value = tempHP;
	}
	private void SlideIP(Slider temp, float max, float value, float delta){
		/*if (tempIP <= max) {
			temp.maxValue = max;
			tempIP = tempIP + (value - tempIP) * delta * 3;
			temp.value = tempIP;
		} else {
			temp.value = value;
			tempHP = tempHP + (max - tempHP) * delta * 3;
			temp.maxValue = tempHP;
		}*/
		tempIP = (value / max) * 100 + ((value / max) * 100 - tempIP) * delta;
		temp.value = tempIP;
	}
	private void SlideRB(Slider temp, float max, float value, float delta){
		/*if (tempRB <= max) {
			temp.maxValue = max;
			tempHP = tempRB + (value - tempRB) * delta * 3;
			temp.value = tempRB;
		} else {
			temp.value = value;
			tempHP = tempRB + (max - tempRB) * delta * 3;
			temp.maxValue = tempRB;
		}*/
		tempRB = (value / max) * 100 + ((value / max) * 100 - tempRB) * delta;
		temp.value = tempRB;
	}
	private void SlideCT(Slider temp, float max, float value, float delta){
		/*if (tempCT <= max) {
			temp.maxValue = max;
			tempCT = tempCT + (value - tempCT) * delta * 3;
			temp.value = tempCT;
		} else {
			temp.value = value;
			tempHP = tempCT + (max - tempCT) * delta * 3;
			temp.maxValue = tempCT;
		}*/
		tempCT= (value / max) * 100 + ((value / max) * 100 - tempCT) * delta;
		temp.value = tempCT;
	}
}
