
using UnityEngine;

namespace GGame
{
    public class SpriteManager : MonoBehaviour
    {
        public Sprite[] wallSpriteArray;
        public Sprite[] itemSpriteArray;
        public Sprite[] characterSpriteArray;
    
        private static SpriteManager _instance = null;
        public static SpriteManager Instance => _instance;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }   
}
