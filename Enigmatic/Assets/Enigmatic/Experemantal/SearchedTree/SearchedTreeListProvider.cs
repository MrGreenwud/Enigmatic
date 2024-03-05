using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using TMPro;

namespace Enigmatic.Experemental.SearchedWindowUtility
{
    public class SearchedTreeListProvider : ScriptableObject, ISearchWindowProvider
    {
        public event Action<string, string> OnSelected;

        private string m_GrupName;
        private string m_BranchName;

        private string m_SenderUID;

        public static SearchedTreeListProvider Create(string grupName, string branchName, string senderUID = "0")
        {
            SearchedTreeListProvider provider = CreateInstance<SearchedTreeListProvider>();
            provider.Init(grupName, branchName, senderUID);

            return provider;
        }

        private void Init(string GrupName, string BranchName, string senderUID)
        {
            m_GrupName = GrupName;
            m_BranchName = BranchName;
            m_SenderUID = senderUID;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            if(STPFile.TryLoadTree(m_GrupName, m_BranchName, out string stpFile))
                return GetSearchTreeEntries(stpFile);

            throw null;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            OnSelected?.Invoke
                (m_SenderUID, SearchTreeEntry.userData.ToString());

            return true;
        }

        private List<SearchTreeEntry> GetSearchTreeEntries(string stpFile)
        {
            string[] stp = stpFile.Split('\n');

            List<SearchTreeEntry> searchTreeEntrys = new List<SearchTreeEntry>();

            uint depthLevel = 0;

            for(int i = 2; i < stp.Length; i++)
            {
                string lineFixed = FileEditor.Replace(stp[i].Replace(" ", string.Empty), '_', ' ');

                if (lineFixed == "[")
                {
                    depthLevel++;
                }
                else if (lineFixed == "]")
                {
                    depthLevel--;
                }   
                else
                {
                    if (i + 1 < stp.Length - 1)
                    {
                        if (FileEditor.Replace(stp[i + 1].Replace(" ", string.Empty), '_', ' ') == "[")
                        {
                            searchTreeEntrys.Add(new SearchTreeGroupEntry
                                (new GUIContent(lineFixed), (int)depthLevel));
                        }
                        else
                        {
                            SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(lineFixed));
                            searchTreeEntry.level = (int)depthLevel;
                            searchTreeEntry.userData = lineFixed;
                            searchTreeEntrys.Add(searchTreeEntry);
                        }
                    }
                }
            }

            return searchTreeEntrys;
        }
    }
}
