using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterStatusScript : MonoBehaviour {

	public GameObject cellgrid;
	public List<Slider> sliders;
	public List<Text> texts;
	public Image charImage;

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
		//charImage = cellgrid.GetComponent<CellGrid> ().CurrentUnit;

		SlideHP (sliders [0], cellgrid.GetComponent<CellGrid> ().CurrentUnit.TotalHitPoints, cellgrid.GetComponent<CellGrid> ().CurrentUnit.HitPoints);
		SlideIP (sliders [1], cellgrid.GetComponent<CellGrid> ().CurrentUnit.TotalImaginationPoints, cellgrid.GetComponent<CellGrid> ().CurrentUnit.ImaginationPoints);
		SlideRB (sliders [2], 100f, cellgrid.GetComponent<CellGrid> ().CurrentUnit.RealityBreak);
		SlideCT (sliders [3], 100f, cellgrid.GetComponent<CellGrid> ().CurrentUnit.ChargeTime);
	}

	//Slides the sliders based on max and current value
	private void SlideHP(Slider temp, float max, float value){
		if (tempHP <= max) {
			temp.maxValue = max;
			tempHP = tempHP + (value - tempHP) * Time.deltaTime * 3;
			temp.value = tempHP;
		} else {
			temp.value = value;
			tempHP = tempHP + (max - tempHP) * Time.deltaTime * 3;
			temp.maxValue = tempHP;
		}
	}
	private void SlideIP(Slider temp, float max, float value){
		if (tempIP <= max) {
			temp.maxValue = max;
			tempIP = tempIP + (value - tempIP) * Time.deltaTime * 3;
			temp.value = tempIP;
		} else {
			temp.value = value;
			tempHP = tempHP + (max - tempHP) * Time.deltaTime * 3;
			temp.maxValue = tempHP;
		}
	}
	private void SlideRB(Slider temp, float max, float value){
		if (tempRB <= max) {
			temp.maxValue = max;
			tempHP = tempRB + (value - tempRB) * Time.deltaTime * 3;
			temp.value = tempRB;
		} else {
			temp.value = value;
			tempHP = tempRB + (max - tempRB) * Time.deltaTime * 3;
			temp.maxValue = tempRB;
		}
	}
	private void SlideCT(Slider temp, float max, float value){
		if (tempCT <= max) {
			temp.maxValue = max;
			tempCT = tempCT + (value - tempCT) * Time.deltaTime * 3;
			temp.value = tempCT;
		} else {
			temp.value = value;
			tempHP = tempCT + (max - tempCT) * Time.deltaTime * 3;
			temp.maxValue = tempCT;
		}
	}
}
