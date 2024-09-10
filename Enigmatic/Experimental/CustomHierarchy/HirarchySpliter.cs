using UnityEditor;
using UnityEngine;

namespace Enigmatic.Experimental.CustomHierarchy
{
    public sealed class HirarchySpliter : MonoBehaviour
    {
        [SerializeField] private Color m_Color;

        public Color Color => m_Color;

        public void SetRandomColor()
        {
            m_Color.r = Random.Range(0.272f, 0.550f);
            m_Color.g = Random.Range(0.272f, 0.550f);
            m_Color.b = Random.Range(0.272f, 0.550f);

            m_Color.a = 1;
        }

        public void OnValidate()
        {
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
