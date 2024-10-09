using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigmatic.Experimental.SearchedWindowUtility
{
    public class SearchedTreeGrup
    {
        public string Name;

        private List<SearchedTree> m_SearchedTrees { get; set; }

        public SearchedTree[] SearchedTrees => m_SearchedTrees.ToArray();

        public SearchedTreeGrup(string name)
        {
            Name = name;
            m_SearchedTrees = new List<SearchedTree>();
        }

        public void Add(SearchedTree newSearchedTree) => m_SearchedTrees.Add(newSearchedTree);

        public void Remove(SearchedTree newSearchedTree) => m_SearchedTrees.Remove(newSearchedTree);

        public bool TryRemove(SearchedTree newSearchedTree)
        {
            m_SearchedTrees.Remove(newSearchedTree);
            return m_SearchedTrees.Contains(newSearchedTree);
        }
    }
}
