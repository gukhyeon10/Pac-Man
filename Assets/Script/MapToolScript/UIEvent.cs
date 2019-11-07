using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvent : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    // 맵툴 화면 아래 타일 클릭
    public void OnMouseDown()
    {
        if(spriteRenderer != null)
        {
            MapToolCursor.Instance.cursorTileSprite = spriteRenderer.sprite;
            Debug.Log("Cursor Change : " + MapToolCursor.Instance.cursorTileSprite.name);
        }
        else
        {
            Debug.Log("Select Tile is null");
        }
    }

}
