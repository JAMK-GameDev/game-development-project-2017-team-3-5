using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paperController : MonoBehaviour {

    Animator anim;

    void Start()
    {
        //Initialize boolean
        anim = GetComponent<Animator>();
        anim.SetBool("isEnable", false);
    }

    public void toggle()
    {
        //Reverse Boolean
        anim.SetBool("isEnable", !anim.GetBool("isEnable"));
    }
}
