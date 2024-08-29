using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Enigmatic.Experemental.SearchedWindowUtility
{
    public static class STPFile
    {
        public readonly static string s_Path = $"{Application.dataPath}/Enigmatic/Source/SearchedTree";

        private static List<string> s_STPFils = new List<string>();

        public static void CompareAll(SearchedTreeGrup[] grups)
        {
            if (Directory.Exists(s_Path) == false)
                Directory.CreateDirectory(s_Path);

            List<string> directores = Directory.GetDirectories(s_Path).ToList();
            List<string> existDirectores = new List<string>(directores.Count);

            foreach (SearchedTreeGrup grup in grups)
            {
                foreach (string directory in directores)
                {
                    if (grup.Name == Path.GetDirectoryName(directory))
                    {
                        directores.Remove(directory);
                        existDirectores.Add(directory);
                        break;
                    }
                }
            }

            foreach (string directory in directores)
                Directory.Delete(directory, true);

            foreach (string directory in existDirectores)
            {
                string[] path = directory.Split('/');
                string grupName = path[path.Length - 1];

                List<string> stpFiles = Directory.GetFiles(directory, "*.stp").ToList();

                foreach (SearchedTreeGrup grup in grups)
                {
                    foreach (SearchedTree tree in grup.SearchedTrees)
                    {
                        foreach (string stpFile in stpFiles)
                        {
                            if (tree.Value.Replace(" ", string.Empty)
                                == Path.GetFileNameWithoutExtension(stpFile).Replace("_", string.Empty))
                            {
                                stpFiles.Remove(stpFile);
                                break;
                            }
                        }
                    }
                }

                foreach (string file in stpFiles)
                    File.Delete(file);
            }
        }

        public static void Save()
        {
            foreach (string stp in s_STPFils)
            {
                string grupName = GetGrup(stp);
                string path = $"{s_Path}/{grupName}";

                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                File.WriteAllText($"{path}/{GetBranch(stp)}.stp", stp);
            }

            s_STPFils.Clear();
        }

        public static bool TryLoadTree(string grupName, string brenchName, out string stpFile)
        {
            string path = $"{s_Path}/{grupName}/{brenchName}.stp";

            if (File.Exists(path) == false)
            {
                stpFile = string.Empty;
                return false;
            }

            StreamReader streamReader = new StreamReader(path);
            stpFile = streamReader.ReadToEnd();
            return true;
        }

        public static SearchedTreeGrup[] LoadTrees()
        {
            if(Directory.Exists(s_Path) == false)
                Directory.CreateDirectory(s_Path);

            List<SearchedTreeGrup> searchedTreeGrups = new List<SearchedTreeGrup>();

            string[] directores = Directory.GetDirectories(s_Path);

            foreach (string directory in directores)
            {
                string[] stpFiles = Directory.GetFiles(directory, "*.stp");

                foreach (string stpFile in stpFiles)
                {
                    SearchedTreeGrup newGrup = LoadTree($"{stpFile.Replace('\\', '/')}");

                    bool isContains = false;

                    foreach (SearchedTreeGrup grup in searchedTreeGrups)
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
                        searchedTreeGrups.Add(newGrup);
                }
            }

            return searchedTreeGrups.ToArray();
        }

        public static SearchedTreeGrup LoadTree(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            string stp = streamReader.ReadToEnd();

            SearchedTreeGrup grup = new SearchedTreeGrup(GetGrup(stp));
            List<SearchedTree> trees = LoadBranch(stp.Split("\n"), 2, out int i);
            grup.Add(trees[0]);

            foreach (SearchedTree tree in trees[0].GetAllTree())
                tree.UpdateLevel();

            streamReader.Close();
            return grup;
        }

        private static List<SearchedTree> LoadBranch(string[] stp, int lineNumber, out int endLineNumber)
        {
            List<SearchedTree> searchedTrees = new List<SearchedTree>();

            for (int i = lineNumber; i < stp.Length; i++)
            {
                string line = stp[i].Replace(" ", string.Empty).Replace('_', ' ');

                if (line == "[" || line == "]")
                {
                    if (line == "[")
                    {
                        List<SearchedTree> children = new List<SearchedTree>();

                        children = LoadBranch(stp, i + 1, out i);

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
                s_STPFils.Add($"{FileEditor.Replace(grup.Name, ' ', '_')} : order{{{order}}} \n[ \n{stp} \n]");
                stp = "";
            }
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
            string[] firtLine = FileEditor.Replace(stp[0], '_', ' ').Split(' ');

            string grup = "";

            foreach (string word in firtLine)
            {
                if (word == ":")
                    break;

                grup += word;
                grup += ' ';
            }

            return grup;
        }

        private static int GetOrder(string stpFile)
        {
            string[] stp = stpFile.Split("\n");

            foreach (char symbol in stp[0])
            {
                if (int.TryParse(symbol.ToString(), out int oder) == true)
                    return oder;
            }

            throw new System.InvalidOperationException();
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
            if (i != 0 && i + 1 < value.Length)
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

    public static string ReadFile(string path)
    {
        StreamReader streamReader = new StreamReader(path);
        string file = streamReader.ReadToEnd();
        streamReader.Close();

        return file;
    }

    public static string ReadLines(this string[] stringsArray, int start, int end)
    {
        string result = string.Empty;

        for (int i = start; i <= end; i++)
            result += $"{stringsArray[i]}\n";

        return result;
    }

    public static string[] SplitArea(this string text, int startLineNumber, int areaLineCount)
    {
        string[] lines = text.Split("\n");

        List<string> result = new List<string>();

        string tempResult = string.Empty;

        int j = 0;

        for (int i = startLineNumber; i < lines.Length; i++)
        {
            tempResult += $"{lines[i]}\n";

            j++;

            if (j == areaLineCount)
            {
                result.Add(tempResult);
                tempResult = string.Empty;
                j = 0;
            }
        }

        return result.ToArray();
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
