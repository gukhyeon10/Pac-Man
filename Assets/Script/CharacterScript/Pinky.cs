using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : CharacterBase
{

    bool isLookPac = false;


    void OnEnable()
    {
        isLookPac = false;
        respawnCoolTime = 5f;
        StartCoroutine(GhostRespawnCoolTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isContinue)
        {
            return;
        }

        animator.SetInteger("DIRECT", moveDirect);

        if (isRespawn)
        {
            base.GhostRespawn();
        }
        else if (isReturn)
        {
            base.GhostReturn();
        }
        else
        {
            //팩맨 슈퍼모드일 경우 유령 모습 변화
            if (pac.GetIsSuperMode)
            {
                animator.SetBool("SCARE", true);
            }
            else
            {
                animator.SetBool("SCARE", false);
            }

            if (isLookPac == false && GhostLookPac())
            {
                isLookPac = true;
                StartCoroutine(TrackingTime());
            }

            if (isLookPac)
            {
                base.PathTracking();
            }
            else
            {
                base.CharacterMove();
            }
        }
    }

    IEnumerator GhostRespawnCoolTime()
    {
        yield return new WaitForSeconds(respawnCoolTime);
        isRespawn = true;
    }

    IEnumerator TrackingTime()
    {
        yield return new WaitForSeconds(5f);
        isLookPac = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Pac"))
        {
            if (pac.GetIsSuperMode)
            {
                boxCollider.enabled = false;
                isReturn = true;
                animator.SetBool("RETURN", true);
            }
        }
    }
}
