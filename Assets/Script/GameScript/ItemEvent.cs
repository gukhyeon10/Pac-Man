﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 이 스크립트는 사용하지 않음 -> ItemScript로 대체

public class ItemEvent : MonoBehaviour
{
    public int itemNumber;

    //아이템 충돌 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.ToUpper().Equals(Enum.GetName(typeof(ECharacter), (int)ECharacter.PAC)))
        {
            ItemBuff(); 
            gameObject.SetActive(false);

        }
    }

    //아이템 점수 및 효과
    void ItemBuff()
    {
    }
}
