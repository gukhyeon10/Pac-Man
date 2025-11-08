
using System.Collections.Generic;
using System.Threading.Tasks;
using GUtility;
using UnityEngine;

namespace GGame
{
    public class GameManager : MonoBehaviour
    {
        private bool isInit = false;
        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            var tasks = new List<Task>
            {
                TextHelper.Load("Assets/Resources/StringData/Texts.json")
            };

            Task.WhenAll(tasks).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    //TextHelper.GetString("Test1");
                    
                    isInit = true;
                }
            });
        }
    }   
}
