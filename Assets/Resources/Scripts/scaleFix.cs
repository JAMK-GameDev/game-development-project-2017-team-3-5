using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleFix : MonoBehaviour {

	// Scale the game for more managable mode, this scales it back to small so the bugs will fix themselves.
	void Start () {
		transform.localScale = new Vector3 (1, 1, 1);
	}
}
