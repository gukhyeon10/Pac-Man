using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : CharacterBase
{
    
    bool isTurn = false;
    

    // Update is called once per frame
    void Update()
    {
        base.CharacterMove();
    }


    // 블링키 AI
    protected override void CharacterMove()
    {
        
    }
}
