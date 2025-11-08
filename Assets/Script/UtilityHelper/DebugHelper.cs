
using UnityEngine;

namespace GUtility
{
    /// <summary>
    /// 로그 함수 헬퍼 클래스
    /// 리터럴 문자열을 빌드 메모리에 안올리기 위해 enum case로 처리
    /// </summary>
    public static class DebugHelper
    {
        public enum DEBUG
        {
            NULL = 0,
            NOT_FOUND_FILE,
            NO_VALUE,
            ALREADY_KEY,
        }
        public static void Log(DEBUG logIndex)
        {
#if UNITY_EDITOR
            var message = logIndex switch
            {
                DEBUG.NULL => "NULL",
                DEBUG.NOT_FOUND_FILE => "NOT FOUND_FILE",
                DEBUG.NO_VALUE => "NO_VALUE",
                DEBUG.ALREADY_KEY => "ALREADY_KEY",
                _ => "Not Found Debug Message",
            };
            
            Debug.Log($"<DebugHelper> {message}");
#endif
        }
    }   
}
