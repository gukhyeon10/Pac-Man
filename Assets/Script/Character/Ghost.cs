using System.Collections;
using System.Collections.Generic;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class Ghost : CharacterBase
    {
        private readonly BfsNode[][] bfsArray; // Bfs 탐색 array
        private readonly Queue<BfsNode> bfsQueue = new Queue<BfsNode>(); // Bfs 탐색 queue
        
        private bool isLookPac = false;

        private bool isRespawn = false;
        private bool isReturn = false;
        
        protected virtual float RespawnCoolTime => 3f;
        protected virtual float TrackingTime => 5f;

        public Ghost()
        {
            int rows = StageManager.line;
            int cols = StageManager.column;

            bfsArray = new BfsNode[rows][];
            for (int i = 0; i < rows; i++)
            {
                bfsArray[i] = new BfsNode[cols];
                for (int j = 0; j < cols; j++)
                    bfsArray[i][j] = new BfsNode();
            }
        }
        
        public override void InitCharacter(int x, int y)
        {
            base.InitCharacter(x, y);
            
            isRespawn = false;
            isReturn = false;
        }

        private void OnEnable()
        {
            isLookPac = false;
            
            StopAllCoroutines();

            if (RespawnCoolTime > 0f)
            {
                StartCoroutine(GhostRespawnCoolTime());   
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isContinue)
            {
                return;
            }

            animator.SetInteger("DIRECT", (int)moveDirect);

            if (isRespawn)
            {
                GhostRespawn();
            }
            else if (isReturn)
            {
                GhostReturn();
            }
            else
            {
                //팩맨 슈퍼모드일 경우 유령 모습 변화
                animator.SetBool("SCARE", pac.GetIsSuperMode);

                if (isLookPac == false && GhostLookPac())
                {
                    isLookPac = true;
                    
                    StartCoroutine(TrackingPacMan());
                }

                if (isLookPac)
                {
                    PacTracking();
                }
                else
                {
                    CharacterMove();
                }
            }
        }
        
        /// <summary>
        /// 타겟 좌표 경로 탐색 시작
        /// </summary>
        private void PathFinding((int row, int col) coord, (int row, int col) targetCoord, MoveModel model)
        {
            bfsArray[coord.row][coord.col].Modify(coord, coord, true);

            bfsQueue.Clear();
            bfsQueue.Enqueue(bfsArray[coord.row][coord.col]);

            while (bfsQueue.Count > 0)
            {
                var node = bfsQueue.Dequeue();
                if (node.Coord.Equals(targetCoord))
                {
                    BackTracking(node.Coord);
                    break;
                }

                PathFindingProcess(node.Coord, node.Coord.Calculate(EDirect.EAST), model);

                PathFindingProcess(node.Coord, node.Coord.Calculate(EDirect.WEST), model);

                PathFindingProcess(node.Coord, node.Coord.Calculate(EDirect.SOUTH), model);

                PathFindingProcess(node.Coord, node.Coord.Calculate(EDirect.NORTH), model);
            }
        }

        /// <summary>
        /// 경로 탐색 처리
        /// </summary>
        private void PathFindingProcess((int row, int col) src, (int row, int col) dest, MoveModel model)
        {
            if (model[dest.row, dest.col])
            {
                if (bfsArray[dest.row][dest.col].IsVisited == false)
                {
                    bfsArray[dest.row][dest.col].Modify(dest, src, true);

                    bfsQueue.Enqueue(bfsArray[dest.row][dest.col]);
                }   
            }
        }
        
        /// <summary>
        /// 타겟 좌표까지의 경로 중 첫번째 좌표를 역추적
        /// </summary>
        private void BackTracking((int row, int col) reverseCoord)
        {
            var loopSafeCount = 0;
            var maxSafeCount = StageManager.column * StageManager.line * 2;

            while (!coord.Equals(reverseCoord))
            {
                var preCoord = bfsArray[reverseCoord.row][reverseCoord.col].PreCoord;

                if (preCoord.Equals(coord))
                {
                    var direct = GetDirect(reverseCoord, coord);

                    moveDirect = direct ?? moveDirect;
                    
                    coord = reverseCoord;
                    
                    return;
                }
                else
                {
                    reverseCoord = preCoord;
                }

                if (++loopSafeCount > maxSafeCount)
                {
                    //DebugHelper.Log($"무한 루프 방지 {loopSafeCount}");

                    break;
                }
            }
        }
        
        /// <summary>
        /// bfs 탐색 시작 전 초기화
        /// </summary>
        private void InitBfsArray()
        {
            for (int i = 0; i < StageManager.line; i++)
            {
                for (int j = 0; j < StageManager.column; j++)
                {
                    bfsArray[i][j].Modify((i, j), (i, j), false);
                }
            }
        }
        
        /// <summary>
        /// 방향 전환
        /// </summary>
        private EDirect? GetDirect((int row, int col) targetCoord, (int row, int col) pivotCoord)
        {
            for (var direct = EDirect.EAST; direct <= EDirect.NORTH; direct++)
            {
                if (targetCoord == pivotCoord.Calculate(direct))
                {
                    return direct;
                }
            }

            return null;
        }

        // 유령 리스폰
        private void GhostRespawn()
        {
            if (Vector3.Distance(character.position, target.position) < 0.01f)
            {
                if (coord.Equals(StageManager.Instance.ghostRespawnModel.RespawnCoord.Calculate(EDirect.NORTH)))
                {
                    isRespawn = false;
                    
                    return;
                }

                InitBfsArray();

                var model = StageManager.Instance.ghostRespawnModel;
                PathFinding(coord, model.RespawnCoord.Calculate(EDirect.NORTH), model);
                
                target = StageManager.Instance.tileModel[coord.row, coord.col].transform;
            }

            character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
        }

        // 유령 리턴
        private void GhostReturn()
        {
            if (Vector3.Distance(character.position, target.position) < 0.01f)
            {
                if (coord.Equals(StageManager.Instance.ghostRespawnModel.RespawnCoord.Calculate(EDirect.SOUTH)))
                {
                    isReturn = false;
                    animator.SetBool("RETURN", false);
                    boxCollider.enabled = true;
                    
                    return;
                }

                InitBfsArray();
                
                var model = StageManager.Instance.ghostRespawnModel;
                PathFinding(coord, model.RespawnCoord.Calculate(EDirect.SOUTH), model);
                
                target = StageManager.Instance.tileModel[coord.row, coord.col].transform;
            }

            character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime * 1.5f);
        }

        /// <summary>
        /// 가는 뱡향 일정 거리 이내에 팩맨이 있는지 확인
        /// </summary>
        private bool GhostLookPac()
        {
            var tempCoord = Coord;

            for (int i = 0; i < 5; i++)
            {
                tempCoord.Update(moveDirect);
                
                if (StageManager.Instance.moveModel[tempCoord.row, tempCoord.col])
                {
                    if(tempCoord.Equals(pac.Coord))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 팩맨 추적
        /// </summary>
        private void PacTracking()
        {
            if (pac.transform != null)
            {
                if (character.position == target.position)
                {
                    InitBfsArray();
                    
                    PathFinding(coord, pac.Coord, StageManager.Instance.moveModel);
                    
                    target = StageManager.Instance.tileModel[coord.row, coord.col].transform;
                }

                character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
            }
        }

        private IEnumerator GhostRespawnCoolTime()
        {
            yield return new WaitForSeconds(RespawnCoolTime);
            
            isRespawn = true;
        }

        private IEnumerator TrackingPacMan()
        {
            yield return new WaitForSeconds(TrackingTime);
            
            isLookPac = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag.Equals("Pac"))
            {
                if (pac.GetIsSuperMode)
                {
                    boxCollider.SafeSetEnable(false);
                    
                    isReturn = true;
                    
                    animator.SetBool("RETURN", true);
                }
            }
        }
    }
}
