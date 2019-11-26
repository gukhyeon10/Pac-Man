using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    int itemScore;
    [SerializeField]
    bool isNormal;
    [SerializeField]
    bool isSuper;

    string pacTag;
    void Start()
    {
        pacTag = CharacterBase.pac.tag;
    }

    //아이템 충돌 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals(pacTag))
        {
            ItemBuff();
            gameObject.SetActive(false);

        }
    }

    //아이템 점수 및 효과
    void ItemBuff()
    {
        if(isNormal)
        {
            StageManager.Instance.EatNormal();
        }
        else if(isSuper)
        {
            CharacterBase.pac.SuperMode();
        }

        UIManager.Instance.UpdateScore(itemScore);

    }
}
