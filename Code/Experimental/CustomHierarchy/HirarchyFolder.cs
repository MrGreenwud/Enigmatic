using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experimental.CustomHierarchy
{
    public sealed class HirarchyFolder : MonoBehaviour
    {
        [SerializeField] [ColorUsage(false)] private Color m_Color;

        public Color Color => m_Color;

        public void SetRandomColor()
        {
            m_Color.r = Random.Range(0f, 1.1f);
            m_Color.g = Random.Range(0f, 1.1f);
            m_Color.b = Random.Range(0f, 1.1f);
            m_Color.a = 1;
        }

        public void OnValidate()
        {
            m_Color.a = 1;
            Hierarchy.UpdateFolderIcon(this);
        }
    }
}
