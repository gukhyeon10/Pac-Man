using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : CharacterBase
{
    [SerializeField]
    Animator animator;

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

        animator.SetInteger("DIRECT", moveDirect);

        //팩맨 슈퍼모드일 경우 유령 모습 변화
        if (pac.GetIsSuperMode)
        {

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Pac"))
        {
            if(pac.GetIsSuperMode)
            {
                
            }
        }
    }
}
