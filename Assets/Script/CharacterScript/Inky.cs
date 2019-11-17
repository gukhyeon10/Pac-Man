using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : CharacterBase
{
    bool isLookPac = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        isLookPac = false;
    }

    // Update is called once per frame
    void Update()
    {
        base.CharacterMove();
        //base.PathTracking();
    }
}
