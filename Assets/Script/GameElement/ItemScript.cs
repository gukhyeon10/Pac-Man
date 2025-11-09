using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{

    public class ItemScript : MonoBehaviour
    {
        [SerializeField] int itemScore;
        [SerializeField] bool isNormal;
        [SerializeField] bool isSuper;

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
                
                this.SafeSetActive(false);
            }
        }

        //아이템 점수 및 효과
        void ItemBuff()
        {
            UIManager.Instance.UpdateScore(itemScore);

            if (isNormal)
            {
                StageManager.Instance.EatNormal();
            }
            else if (isSuper)
            {
                CharacterBase.pac.SuperMode();
            }

        }
    }
}
