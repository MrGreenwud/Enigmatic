using UnityEngine;

namespace EngineUtitlity.SearchedWindow
{
    [CreateAssetMenu(fileName = "Searched Tree Element Provider", menuName = "Search Utility/Searched Tree Element Provider", order = 0)]
    public class SearchedTreeProvider : ScriptableObject
    {
        [SerializeField] private SearchedTree m_SearchedTreeElements;

        public SearchedTree SearchedTree => m_SearchedTreeElements;

        public void CreateSearchTreeEntry() => STP.Save(m_SearchedTreeElements);
    }
}
