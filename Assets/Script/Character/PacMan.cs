using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class PacMan : CharacterBase
    {
        private int inputDirect = (int)EDirect.EAST;
        private bool isSuperMode = false;
        private bool isInput = false;
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

            animator.SetInteger("DIRECT", moveDirect);
            
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
            character.position = Vector3.MoveTowards(character.position, tileArray[coord.row, coord.col].transform.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(character.position, tileArray[coord.row, coord.col].transform.position) < 0.01f)
            {
                if (!WrapCoordinate()) // 반대편으로 전환하는게 아니라면
                {
                    if (IsKeepMove((EDirect)inputDirect))
                    {
                        moveDirect = inputDirect;
                    }
                    
                    if(IsKeepMove((EDirect)moveDirect))
                    {
                        UpdateCoord();
                    }
                }
            }
            else
            {
                ReverseMove(); // 타일에서 타일 넘어가는 중에 반대 방향으로 전환 가능
            }

            isInput = false;
        }

        // 역방향 이동
        void ReverseMove()
        {
            if (isInput)
            {
                if (inputDirect == (int)EDirect.EAST && moveDirect == (int)EDirect.WEST)
                {
                    coord.col++;
                    moveDirect = inputDirect;
                }
                else if (inputDirect == (int)EDirect.WEST && moveDirect == (int)EDirect.EAST)
                {
                    coord.col--;
                    moveDirect = inputDirect;
                }
                else if (inputDirect == (int)EDirect.SOUTH && moveDirect == (int)EDirect.NORTH)
                {
                    coord.row++;
                    moveDirect = inputDirect;
                }
                else if (inputDirect == (int)EDirect.NORTH && moveDirect == (int)EDirect.SOUTH)
                {
                    coord.row--;
                    moveDirect = inputDirect;
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
            inputDirect = (int)EDirect.WEST;
            isInput = true;
        }

        public void PacRight()
        {
            inputDirect = (int)EDirect.EAST;
            isInput = true;
        }

        public void PacUp()
        {
            inputDirect = (int)EDirect.NORTH;
            isInput = true;
        }

        public void PacDown()
        {
            inputDirect = (int)EDirect.SOUTH;
            isInput = true;
        }
    }
}