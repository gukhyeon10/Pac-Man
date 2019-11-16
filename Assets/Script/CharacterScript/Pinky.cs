using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : CharacterBase
{
    // Start is called before the first frame update
    void Start()
    {
      //  StartCoroutine(GhostRespawnCoolTime());
    }

    void OnEnable()
    {
        StartCoroutine(GhostRespawnCoolTime());
    }

    // Update is called once per frame
    void Update()
    {
        if(isRespawn)
        {
            base.GhostRespawn();
        }
        else if(isReturn)
        {
            base.GhostReturn();
        }
        else
        {
            base.CharacterMove();
        }
        //base.PathTracking();
    }

    IEnumerator GhostRespawnCoolTime()
    {
        yield return new WaitForSeconds(3f);
        isRespawn = true;

        yield return new WaitForSeconds(5f);
        isReturn = true;
    }
}
