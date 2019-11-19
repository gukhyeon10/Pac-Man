using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : CharacterBase
{
    [SerializeField]
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!isContinue)
        {
            return;
        }

        animator.SetInteger("DIRECT", moveDirect);
        //base.GhostRespawn();
        base.CharacterMove();
        //base.PathTracking();
    }
}
