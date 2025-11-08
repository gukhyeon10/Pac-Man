
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GUtility
{
    /// <summary>
    /// 스트링 읽기 헬퍼 클래스
    /// </summary>
    public static class TextHelper
    {
        private static readonly Dictionary<string, string> stringCaching = new Dictionary<string, string>();

        public static async Task Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                DebugHelper.Log(DebugHelper.DEBUG.NO_VALUE);
                
                return;
            }

            stringCaching.Clear();
            
            var jsonContent = await Task.Run(() => File.ReadAllText(filePath));
            
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
            
            foreach (var key in json.Keys)
            {
                if (stringCaching.ContainsKey(key))
                {
                    DebugHelper.Log(DebugHelper.DEBUG.ALREADY_KEY);
                }
                else
                {
                    stringCaching[key] = json[key];   
                }
            }
        }

        public static string GetString(string key)
        {
            if (stringCaching.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                DebugHelper.Log(DebugHelper.DEBUG.NO_VALUE);
                
                return string.Empty;   
            }
        }
    }   
}
