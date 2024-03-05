using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace EngineUtitlity.SearchedWindow
{
    public class SearchedTreeListProvider : ScriptableObject, ISearchWindowProvider
    {
        public event Action<SearchedTree, string> OnSelected;
        
        private string m_Path;
        private string m_SenderCode;

        public void Create(TreeTags tag, string senderCode = "")
        {
            m_Path = $"{STP.s_Path}{tag}.stp";
            m_SenderCode = senderCode;
        }

        public void Create(string tag, string senderCode = "")
        {
            m_Path = $"{STP.s_Path}{tag}.stp";
            m_SenderCode = senderCode;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntrys = new List<SearchTreeEntry>();

            return STP.Load(m_Path).GetSearchTreeEntry();

            throw new Exception("SearchTreeContainer not found key");
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (SearchTreeEntry.userData is SearchedTree searchedTree)
            {
                OnSelected?.Invoke(searchedTree, m_SenderCode);
                OnSelected = null;
            }

            return true;
        }
    }
}
