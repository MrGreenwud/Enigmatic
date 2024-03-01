using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Enigmatic.Experemental.SearchedWindowUtility
{
    public static class STPFile
    {
        private static List<string> s_STPFils = new List<string>();
        private static string s_Path = $"{Application.dataPath}/SearchedTree/";

        public static void Create()
        {
            foreach (string stp in s_STPFils)
            {
                string path = $"{s_Path}/{GetGrup(stp)}";

                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                File.WriteAllText($"{path}/{GetBranch(stp)}.stp", stp);
            }

            s_STPFils.Clear();
        }

        public static SearchedTreeGrup[] LoadTrees()
        {
            List<SearchedTreeGrup> SearchedTreeGrups = new List<SearchedTreeGrup>();

            string[] directores = Directory.GetDirectories(s_Path);

            foreach (string directory in directores)
            {
                string[] stpFiles = Directory.GetFiles(directory);

                foreach (string stpFile in stpFiles)
                {
                    if (FileEditor.GetFileFormat(stpFile) == "stp")
                    {
                        SearchedTreeGrup newGrup = (LoadTree($"{stpFile.Replace('\\', '/')}"));

                        bool isContains = false;

                        foreach (SearchedTreeGrup grup in SearchedTreeGrups)
                        {
                            if (grup.Name == newGrup.Name)
                            {
                                isContains = true;

                                foreach (SearchedTree tree in newGrup.SearchedTrees)
                                    grup.Add(tree);

                                break;
                            }
                        }

                        if (isContains == false)
                            SearchedTreeGrups.Add(newGrup);
                    }
                }
            }

            return SearchedTreeGrups.ToArray();
        }

        public static SearchedTreeGrup LoadTree(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string stp = streamReader.ReadToEnd();

            SearchedTreeGrup grup = new SearchedTreeGrup(GetGrup(stp));
            List<SearchedTree> trees = LoadBrech(stp.Split("\n"), 2, out int i);
            grup.Add(trees[0]);

            foreach(SearchedTree tree in trees[0].GetAllTree())
                tree.UpdateLevel();

            return grup;
        }

        private static List<SearchedTree> LoadBrech(string[] stp, int lineNumber, out int endLineNumber)
        {
            List<SearchedTree> searchedTrees = new List<SearchedTree>();

            for(int i = lineNumber; i < stp.Length; i++)
            {
                string line = stp[i].Replace(" ", string.Empty).Replace('_', ' ');

                if (line == "[" || line == "]")
                {
                    if (line == "[")
                    {
                        List<SearchedTree> children = new List<SearchedTree>();

                        children = LoadBrech(stp, i + 1, out i);

                        foreach (SearchedTree child in children)
                            searchedTrees[searchedTrees.Count - 1].AddChild(child);
                    }
                    else
                    {
                        endLineNumber = i;
                        return searchedTrees;
                    }
                }
                else
                {
                    searchedTrees.Add(new SearchedTree(line));
                }
            }

            endLineNumber = 0;
            return null;
        }

        public static void Generate(SearchedTreeGrup grup, uint order)
        {
            string stp = "";

            foreach (SearchedTree tree in grup.SearchedTrees)
            {
                stp += Generate(tree);
                s_STPFils.Add($"{grup.Name} : order{{{order}}} \n[ \n{stp} \n]");
                stp = "";
            }

            Debug.Log(s_STPFils[0]);
        }

        private static string Generate(SearchedTree searchedTree)
        {
            string st = "";

            st += $"{FileEditor.Space(searchedTree.Level + 1)}";
            st += $"{FileEditor.Replace(searchedTree.Value, ' ', '_')}";

            SearchedTree[] children = searchedTree.GetÑhildren();

            if (children.Length != 0)
            {
                st += $"\n{FileEditor.Space(searchedTree.Level + 1)}";
                
                st += $"[ \n";

                foreach (SearchedTree child in children)
                    st += $"{Generate(child)} \n";

                st += $"{FileEditor.Space(searchedTree.Level + 1)}";

                st += $"]";
            }

            return st;
        }

        private static string GetGrup(string stpFile)
        {
            string[] stp = stpFile.Split("\n");
            string[] firtLine = stp[0].Split(' ');

            string grup = "";

            foreach (string word in firtLine)
            {
                if (word == ":")
                    break;

                grup += word;
            }

            return grup;
        }

        private static string GetBranch(string stpFile)
        {
            string[] stp = stpFile.Split("\n");
            string branch = stp.Length >= 4 ? stp[2] : "";
            return branch.Replace(" ", string.Empty);
        }

        
    }
}


public static class FileEditor
{
    public static string Space(uint depthLevel)
    {
        string indent = "";

            for (uint i = 0; i < depthLevel; i++)
                indent += "    ";
        
        return indent;
    }

    public static string Replace(string value, char oldChar, char newChar)
    {
        string newValue = "";

        for (int i = 0; i < value.Length; i++)
        {
            if (i != 0)
            {
                if (value[i - 1] != oldChar && value[i] == oldChar && value[i + 1] != oldChar)
                {
                    newValue += newChar;
                    continue;
                }
            }

            if (value[i] == oldChar)
                continue;

            newValue += value[i];
        }

        return newValue;
    }

    public static string GetFileFormat(string file)
    {
        return file.Split('.')[file.Split('.').Length - 1];
    }
}

/*
 * GrupName : order{0}
 * [
 *      ParentTree
 *      [
 *          ChildTree
 *          [
 *              LateChildTree
 *              [
 *                  ...
 *              ]
 *          ]
 *          
 *          ChildTree
 *          [
 *              Late_Child_Tree
 *              LateChildTree
 *              LateChildTree
 *          ]
 *          ChildTree
 *          ChildTree
 *          ChildTree
 *      ]
 *      
 *      ParentTree
 *      [
 *          ChildTree
 *          ChildTree
 *          ChildTree
 *      ]
 * ]
 */
