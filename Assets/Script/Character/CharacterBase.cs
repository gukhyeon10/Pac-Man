
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected BoxCollider2D boxCollider;
        
        //이동 목표 Transform
        protected Transform target;
        protected Transform character;

        //팩맨 캐릭터
        public static PacMan pac;

        protected EDirect moveDirect = EDirect.EAST;
        
        protected float speed = 2f;
        
        public bool isContinue = true;

        //목표 위치 좌표
        protected (int row, int col) coord;
        public (int row, int col) Coord => coord;
        
        // 캐릭터 초기화
        public virtual void InitCharacter(int x, int y)
        {
            coord.row = x;
            coord.col = y;

            target = StageManager.Instance.tileModel[x, y].transform;
            
            character = this.transform;
            character.position = target.position;

            boxCollider.enabled = true;
        }
        
        /// <summary>
        /// 바라보는 방향으로 계속 가는지 검사
        /// </summary>
        protected bool IsKeepMove(EDirect direct)
        {
            var directCoord = coord.Calculate(direct);

            return StageManager.Instance.moveModel[directCoord.row, directCoord.col];
        }

        /// <summary>
        /// 이동 방향 갱신
        /// </summary>
        private void UpdateDirect()
        {
            var movableList = new List<EDirect>(4); // 갈 수 있는 방향 리스트
            for (int i = 0; i < movableList.Capacity; i++)
            {
                var directCoord = coord.Calculate((EDirect)i);
                
                if (StageManager.Instance.moveModel[directCoord.row, directCoord.col])
                {
                    movableList.Add((EDirect)i);
                }
            }

            if (movableList.Count > 0)
            {
                var index = Random.Range(0, movableList.Count);

                moveDirect = movableList[index];
            }
        }

        // 각 캐릭터 별 재정의 함수
        protected virtual void CharacterMove()
        {
            if(target == null)
                return;

            if (Vector3.Distance(character.position, target.position) < 0.01f)
            {
                if (!WrapCoordinate())
                {
                    if (!IsKeepMove(moveDirect))
                    {
                        UpdateDirect();
                    }
                    
                    coord.Update(moveDirect);

                    target = StageManager.Instance.tileModel[coord.row, coord.col].transform;
                }
            }

            if (target != null)
            {
                var pos = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
                character.SafeSetPosition(pos);   
            }
        }

        /// <summary>
        /// 맵 끝으로 이동시 반대편으로 이동
        /// </summary>
        protected bool WrapCoordinate()
        {
            var tileArray = StageManager.Instance.tileModel;
            
            if (coord.row == 0)
            {
                character.SafeSetPosition(tileArray[StageManager.line - 1, coord.col].transform.position);
                coord.row = StageManager.line - 2;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.row == StageManager.line - 1)
            {
                character.SafeSetPosition(tileArray[0, coord.col].transform.position);
                coord.row = 1;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.col == 0)
            {
                character.SafeSetPosition(tileArray[coord.row, StageManager.column - 1].transform.position);
                coord.col = StageManager.column - 2;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else if (coord.col == StageManager.column - 1)
            {
                character.SafeSetPosition(tileArray[coord.row, 0].transform.position);
                coord.col = 1;
                target = tileArray[coord.row, coord.col].transform;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}