using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//싱글톤
public class MapToolCursor : MonoBehaviour
{
    private static MapToolCursor _instance = null;

    public static MapToolCursor Instance
    {
        get
        {
            return _instance;
        }
    }

    // 커서 활성화 타일 Sprite
    public Sprite cursorSprite;
    public int cursorType = (int)EObjectType.WALL;
    public int objectNumber = (int)EWall.DEFAULT;
    public float rot = 0;

    public bool isAutoCompleteActive = true;

    [SerializeField]
    Text autoCheckText;

    void Awake()
    {
        //싱글톤 초기화
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
    }

    //키 입력
    void InputKey()
    {
        //회전
        if(Input.GetKeyDown(KeyCode.R))
        {
            rot += 90f;
            if(rot>=450f)
            {
                rot = 90f;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rot -= 90f;
            if (rot <= -90f)
            {
                rot = 270f;
            }
        }

        // 자동완성 기능 On/Off
        if(Input.GetKeyDown(KeyCode.A))
        {
            isAutoCompleteActive = !isAutoCompleteActive;
            autoCheckText.gameObject.SetActive(isAutoCompleteActive);
        }
    }
    

}
