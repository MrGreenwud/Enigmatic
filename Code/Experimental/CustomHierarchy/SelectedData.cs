using UnityEngine;

namespace Enigmatic.Experimental.CustomHierarchy
{
    public struct SelectedData
    {
        public GameObject GameObject;
        public string ElementName;

        public SelectedData(GameObject gameObject, string elmentName)
        {
            GameObject = gameObject;
            ElementName = elmentName;
        }
    }
}
