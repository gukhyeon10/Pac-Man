using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance = null;

    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
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

    [SerializeField]
    GameObject scoreObject;
    [SerializeField]
    GameObject scorePivotObject; // 화면 아래 특정 타일을 기준으로 위치 대응

    [SerializeField]
    SpriteRenderer[] scoreSpriteRendererArray = new SpriteRenderer[5]; // UI 스프라이트
    [SerializeField]
    Sprite[] numberSpriteArray = new Sprite[10]; // 숫자 스프라이트

    int playerScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCorutine());  // 타일이 정렬되기 전이라 위치가 올바르게 안잡히는거라 추측. 따라서 한 프레임 텀을 준다.
    }

    IEnumerator InitCorutine()
    {
        yield return null;
        scoreObject.transform.position = scorePivotObject.transform.position;
    }

    public void UpdateScore(int plusScore)
    {
        playerScore += plusScore;
       // Debug.Log(playerScore);
    }


}
