using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : CharacterBase
{
    bool isLookPac = false;

    // Start is called before the first frame update
    void Start()
    {
      //  StartCoroutine(GhostRespawnCoolTime());
    }

    void OnEnable()
    {
        isLookPac = false;
        StartCoroutine(GhostRespawnCoolTime());
    }

    // Update is called once per frame
    void Update()
    {
        if(!isContinue)
        {
            return;
        }

        if(isRespawn)
        {
            GhostRespawn();
        }
        else if(isReturn)
        {
            GhostReturn();
        }
        else
        {
            if (isLookPac == false && GhostLookPac())
            {
                isLookPac = true;
                StartCoroutine(TrackingTime());
            }

            if (isLookPac)
            {
                PathTracking();
            }
            else
            {
                CharacterMove();
            }
        }
    }

    IEnumerator GhostRespawnCoolTime()
    {
        yield return new WaitForSeconds(5f);
        isRespawn = true;
    }

    IEnumerator TrackingTime()
    {
        yield return new WaitForSeconds(5f);
        isLookPac = false;
    }
}
