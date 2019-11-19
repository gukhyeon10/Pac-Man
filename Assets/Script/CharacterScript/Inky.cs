using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : CharacterBase
{
    [SerializeField]
    Animator animator;

    bool isLookPac = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        isLookPac = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isContinue)
        {
            return;
        }

        animator.SetInteger("DIRECT", moveDirect);
        base.CharacterMove();
        //base.PathTracking();
    }
}
