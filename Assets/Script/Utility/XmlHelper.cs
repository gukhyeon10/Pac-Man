
using System.Xml;

namespace GUtility
{
    /// <summary>
    /// Xml 관련 유틸리티 클래스
    /// </summary>
    public static class XmlHelper
    {
        public static XmlNode GetNode(this XmlNode node, string name)
        {
            if (node != null)
            {
                var childNode = node.SelectSingleNode(name);

                if (childNode != null)
                {
                    return childNode;
                }
            }
            
            DebugHelper.Log(DebugHelper.DEBUG.NULL);
            
            return null;
        }
        public static int? GetInt(this XmlNode node, string name)
        {
            if (node != null)
            {
                if (int.TryParse(node.InnerText, out var val))
                {
                    return val;
                }
                else
                {
                    DebugHelper.Log(DebugHelper.DEBUG.NO_VALUE);
                    
                    return 0;
                }
            }
            
            DebugHelper.Log(DebugHelper.DEBUG.NULL);

            return null;
        }

        public static float? GetFloat(this XmlNode node, string name)
        {
            if (node != null)
            {
                if (float.TryParse(node.InnerText, out var val))
                {
                    return val;
                }
                else
                {
                    DebugHelper.Log(DebugHelper.DEBUG.NO_VALUE);
                    
                    return 0f;
                }
            }
            
            DebugHelper.Log(DebugHelper.DEBUG.NULL);

            return null;
        }
    }   
}
