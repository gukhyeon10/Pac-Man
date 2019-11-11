using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemEvent : MonoBehaviour
{
    public int itemNumber;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.ToUpper().Equals(Enum.GetName(typeof(ECharacter), (int)ECharacter.PAC)))
        {

            ItemBuff(); 
            gameObject.SetActive(false);

        }
    }

    void ItemBuff()
    {
        int score;
        switch(itemNumber)
        {
            case (int)EItem.NORMAL:
                {
                    StageManager.Instance.EatNormal();
                    score = 10;
                    break;
                }
            case (int)EItem.SUPER:
                {
                    score = 50;
                    break;
                }
            case (int)EItem.CHERRY:
                {
                    score = 100;
                    break;
                }
            case (int)EItem.BERRY:
                {
                    score = 200;
                    break;
                }
            case (int)EItem.PEAR:
                {
                    score = 300;
                    break;
                }
            case (int)EItem.APPLE:
                {
                    score = 400;
                    break;
                }
            case (int)EItem.MELON:
                {
                    score = 500;
                    break;
                }
            default:
                {
                    score = 0;
                    break;
                }
        }
        UIManager.Instance.UpdateScore(score);
    }
}
