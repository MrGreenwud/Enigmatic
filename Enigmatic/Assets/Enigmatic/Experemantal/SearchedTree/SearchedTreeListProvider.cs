using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

namespace Enigmatic.Experemental.SearchedWindowUtility
{
    public class SearchedTreeListProvider : ScriptableObject, ISearchWindowProvider
    {
        public event Action<string, List<string>> OnSelected;

        private string m_GrupName;
        private string m_BranchName;

        private string m_SenderUID;

        public static SearchedTreeListProvider Create(string grupName, string branchName, string senderUID = "0")
        {
            SearchedTreeListProvider provider = CreateInstance<SearchedTreeListProvider>();
            provider.Init(grupName.Replace("_", " "), branchName, senderUID);

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

            throw new Exception("Invalid Loaded Searched Tree!");
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            OnSelected?.Invoke
                (m_SenderUID, SearchTreeEntry.userData as List<string>);

            OnSelected = null;

            return true;
        }

        private List<SearchTreeEntry> GetSearchTreeEntries(string stpFile)
        {
            string[] stp = stpFile.Split('\n');

            List<SearchTreeEntry> searchTreeEntrys = new List<SearchTreeEntry>();
            List<string> path = new List<string>();

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

                    if(path.Count - 1 >= 0)
                        path.Remove(path[path.Count - 1]);
                }   
                else
                {
                    if (i + 1 < stp.Length - 1)
                    {
                        if (FileEditor.Replace(stp[i + 1].Replace(" ", string.Empty), '_', ' ') == "[")
                        {
                            searchTreeEntrys.Add(new SearchTreeGroupEntry
                                (new GUIContent(lineFixed), (int)depthLevel));

                            path.Add(lineFixed);
                        }
                        else
                        {
                            path.Add(lineFixed);

                            SearchTreeEntry searchTreeEntry = new SearchTreeEntry(new GUIContent(lineFixed));
                            searchTreeEntry.level = (int)depthLevel;
                            searchTreeEntry.userData = path.ToList();
                            searchTreeEntrys.Add(searchTreeEntry);

                            path.Remove(path[path.Count - 1]);
                        }
                    }
                }
            }

            return searchTreeEntrys;
        }
    }
}
