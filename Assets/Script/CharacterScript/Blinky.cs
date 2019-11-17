﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : CharacterBase
{
    
    bool isLookPac = false;

    void OnEnable()
    {
        isLookPac = false;    
    }

    // Update is called once per frame
    void Update()
    {
        if(!isContinue)
        {
            return;
        }

        if(isLookPac == false && GhostLookPac())
        {
            isLookPac = true;
            StartCoroutine(TrackingTime());
        }

        if(isLookPac)
        {
            base.PathTracking();
        }
        else
        {
            base.CharacterMove();

        }
    }

    IEnumerator TrackingTime()
    {

        yield return new WaitForSeconds(5f);
        isLookPac = false;
    }
}
