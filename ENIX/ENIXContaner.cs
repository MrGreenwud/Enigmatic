using System.Collections.Generic;
using UnityEngine;

namespace Enigmatic.Experemental.ENIX
{
    [CreateAssetMenu(fileName = "ENIXContaner", menuName = "Enigmatic/ENIXContaner")]
    public class ENIXContaner : ScriptableObject
    {
        [SerializeField] private List<string> m_SerializedObject = new List<string>();

        public string[] SerializedObject => m_SerializedObject.ToArray();

        public void AddObject(string serializedObject)
        {
            m_SerializedObject.Add(serializedObject);
        }

        public void AddObjects(List<string> serializedObjects)
        {
            m_SerializedObject = serializedObjects;
        }
    }
}