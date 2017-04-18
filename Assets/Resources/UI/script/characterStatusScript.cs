using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class characterStatusScript : MonoBehaviour {

	public GameObject cellgrid;
	bool current = false;
	float tempHP = 0;
	float tempIP = 0;
	float tempRB = 0;
	float tempCT = 0;
    public GameObject tempSliderHP;
    GameObject tempSliderIP;
    GameObject tempSliderRB;
    GameObject tempSliderCT;
    Unit tempUnit;


    void Start()
    {
        tempSliderHP = GameObject.Find("charHP");
        tempSliderHP = GameObject.Find("charIP");
        tempSliderHP = GameObject.Find("charRB");
        tempSliderHP = GameObject.Find("charCT");
        tempUnit = cellgrid.GetComponent<CellGrid>().CurrentUnit;
    }

	// Update is called once per frame
	void Update ()
    {
        tempUnit = cellgrid.GetComponent<CellGrid>().CurrentUnit;
        foreach (Transform child in transform) {
			if (child.tag == "charname") {
				//Change name
				if (child.GetComponent<Text> ().text != cellgrid.GetComponent<CellGrid> ().CurrentUnit.name.ToString()) {
					child.GetComponent<Text> ().text = cellgrid.GetComponent<CellGrid> ().CurrentUnit.name.ToString();
					current = true;
				}
				//else current = false;
			}
			if (child.tag == "charavatar" && current) {
				//Set Image
			}
			if (child.tag == "charstats" && current) {
                //Get stats
                //tempSliderHP = GameObject.Find("charHP").GetComponent<Slider>();
                foreach (Transform ch in child.transform) {
					if (ch.tag == "charHP") {
                        //ch.GetComponent<Slider> ().maxValue = cellgrid.GetComponent<CellGrid>().CurrentUnit.TotalHitPoints;
                        float temp = tempHP + (tempHP - tempUnit.HitPoints) * Time.deltaTime;
                        Debug.Log("temp is " + temp);
                        //ch.GetComponent<Slider> ().value = temp;
					}
					if (ch.tag == "charIP") {
						//ch.GetComponent<Slider> ().value = cellgrid.GetComponent<CellGrid>().CurrentUnit.IP;
					}
					if (ch.tag == "charRB") {
						//ch.GetComponent<Slider> ().value = tempUnit.RealityBreak;
					}
					if (ch.tag == "charCT") {
						//ch.GetComponent<Slider> ().value = tempUnit.ChargeTime;
					}
				}
            }
        }
        Debug.Log("temp is " + tempUnit.HitPoints + ", total " + tempUnit.TotalHitPoints);
        //tempSliderHP.GetComponent<Slider>().maxValue = tempUnit.TotalHitPoints;
        tempHP = tempHP + (tempHP - tempUnit.HitPoints) * Time.deltaTime;
        //tempSliderHP.GetComponent<Slider>().value = tempHP;
        (tempSliderHP.GetComponent<Slider>()).value = tempUnit.HitPoints;
        Debug.Log("slider is " + tempSliderHP.GetComponent<Slider>().value);
    }

    public void unitValue(float sliderValue)
    {
        tempUnit = cellgrid.GetComponent<CellGrid>().CurrentUnit;
        sliderValue = tempUnit.HitPoints;
    }
}
