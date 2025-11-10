using System.Collections;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class PacMan : CharacterBase
    {
        private bool isSuperMode = false;
        private Coroutine superModeCoroutine;
        
        public bool GetIsSuperMode => isSuperMode;

        void Awake()
        {
            pac = this;
        }

        // 팩맨 슈퍼모드 초기화
        public void InitPac()
        {
            StopAllCoroutines();
            
            isSuperMode = false;
        }

        void Update()
        {
            if (!isContinue)
            {
                animator.SetBool("GAMEOVER", true);
                
                return;
            }
            
            KeyboardInput();
            
            CharacterMove();
        }

        void KeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PacLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                PacRight();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PacUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PacDown();
            }
        }

        // 팩맨 이동 로직
        protected override void CharacterMove()
        {
            var targetPosition = StageManager.Instance.tileArray[coord.row, coord.col].transform.position;
            
            character.position = Vector3.MoveTowards(character.position, targetPosition, speed * Time.deltaTime);
            
            if (Vector3.Distance(character.position, targetPosition) < 0.01f)
            {
                if (!WrapCoordinate())
                {
                    animator.SetInteger("DIRECT", (int)moveDirect);
                    
                    if(IsKeepMove(moveDirect))
                    {
                        coord.Update(moveDirect);
                    }
                }
            }
        }

        // 무적 모드
        public void SuperMode()
        {
            DebugHelper.Log("Super Mode!");
            // 슈퍼모드일 경우 버프 코루틴 중지시키고 새로 코루틴 실행
            if (isSuperMode)
            {
                StopCoroutine(superModeCoroutine);
            }

            isSuperMode = true;
            superModeCoroutine = StartCoroutine(SuperModeCorutine());
        }

        // 무적 버프
        IEnumerator SuperModeCorutine()
        {
            yield return new WaitForSeconds(7f);
            
            isSuperMode = false;
        }

        //이동 중에 유령이랑 부딪히면 게임오버
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Ghost"))
            {
                if (!isSuperMode)
                {
                    StageManager.Instance.StageResult((int)EResult.GAME_OVER);
                }
            }
        }

        // 방향키 입력
        public void PacLeft()
        {
            moveDirect = EDirect.WEST;
        }

        public void PacRight()
        {
            moveDirect = EDirect.EAST;
        }

        public void PacUp()
        {
            moveDirect = EDirect.NORTH;
        }

        public void PacDown()
        {
            moveDirect = EDirect.SOUTH;
        }
    }
}