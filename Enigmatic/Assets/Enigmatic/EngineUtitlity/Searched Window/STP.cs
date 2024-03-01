using System.IO;
using EngineUtitlity.SearchedWindow;
using UnityEngine;

public static class STP
{
    public static string s_Path = $"{Application.dataPath}/EnigmaticSource/SearchTreesPath/";
    private static int s_GetCildTreeCallCount = 0;

    public static void Save(SearchedTree searchedTree)
    {
        if(Directory.Exists(s_Path) == false)
            Directory.CreateDirectory(s_Path);

        string path = s_Path + $"{searchedTree.Value}.stp";

        File.WriteAllText(path, GenerateLpdFile(searchedTree));
        Debug.Log($"stp file saved in path {path}");
    }

    public static SearchedTree Load(string path)
    { 
        s_GetCildTreeCallCount = 0;
        
        StreamReader streamReader = new StreamReader(path);
        string lpdFile = streamReader.ReadToEnd();

        string value = "";
        int charNumber = 0;

        for (int i = 0; i < lpdFile.Length; i++)
        {
            if (lpdFile[i] != '[' && lpdFile[i] != ':' && lpdFile[i] != ']')
            {
                value += lpdFile[i];
                continue;
            }
            else
            {
                charNumber = i + 1;
                break;
            }
        }

        SearchedTree searchedTree = GetCildTree(ref lpdFile, charNumber, value, 0);
        
        //Debug.Log("Loaded");
        return searchedTree;
    }

    public static SearchedTree Load(TreeTags treeTags)
    {
        string path = $"{s_Path}{treeTags}.stp";
        return Load(path);
    }

    private static SearchedTree GetCildTree(ref string stpFile, int readStartCharNumber, 
        out int readEndCharNumber, string value, int level)
    {
        s_GetCildTreeCallCount++;
        
        SearchedTree parentTree = new SearchedTree(value, level);

        string cildeName = "";
        int treeLevel = level + 1;

        for (int i = readStartCharNumber; i < stpFile.Length; i++)
        {
            if (stpFile[i] == ' ' || stpFile[i] == '\n')
                continue;
            
            if (stpFile[i] == '[' || stpFile[i] == ':' || stpFile[i] == ']')
            {
                if (stpFile[i] == ':')
                {
                    parentTree.AddCild(new SearchedTree(cildeName, treeLevel));
                    cildeName = "";
                    continue;
                }
                else if (stpFile[i] == '[')
                {
                    SearchedTree cildTree = GetCildTree(ref stpFile, i + 1, out i, cildeName, treeLevel);
                    parentTree.AddCild(cildTree);
                    
                    cildeName = "";
                }
                else if (stpFile[i] == ']')
                {
                    readEndCharNumber = i;
                    return parentTree;
                }
            }
            else
            {
                cildeName += stpFile[i];
                continue;
            }
        }

        readEndCharNumber = 0;
        return parentTree;
    }

    private static SearchedTree GetCildTree(ref string lpdFile, int synbleNumber, string value, int level)
    {
        int end;
        return GetCildTree(ref lpdFile, synbleNumber, out end, value, level);
    }

    private static string GenerateLpdFile(SearchedTree searchedTree)
    {
        return FixLpdFile(GetCildTree(searchedTree));
    }

    private static string GetCildTree(SearchedTree searchedTree)
    {
        string childTree = "";

        childTree += $"{searchedTree.Value}[";

        var cilds = searchedTree.SearchedTrees;

        foreach (SearchedTree searchedTreeCild in cilds)
        {
            if (searchedTreeCild.SearchedTrees.Length == 0)
            {
                childTree += $"\n{searchedTreeCild.Value}:";
                continue;
            }

            childTree += $"\n{GetCildTree(searchedTreeCild)}";
        }

        childTree += $"]";

        return childTree;
    }

    private static string FixLpdFile(string lpd)
    {
        string lpdFix = "";

        int indentCount = 0;

        for (int i = 0; i < lpd.Length; i++)
        {
            if (lpd[i] == '[' || lpd[i] == ']')
            {
                if (lpd[i] == ']')
                    indentCount--;

                lpdFix += "\n";
                lpdFix += GetIndent(indentCount);
                lpdFix += lpd[i];

                if (lpd[i] == '[')
                    indentCount++;

                lpdFix += GetIndent(indentCount);
            }
            else if(lpd[i] == '\n')
            {
                lpdFix += "\n";
                lpdFix += GetIndent(indentCount);
            }
            else
            {
                lpdFix += lpd[i];
            }
        }

        return lpdFix;
    }

    private static string GetIndent(int indentCount)
    {
        string indent = "    ";

        string indentResult = "";

        for (int i = 0; i < indentCount; i++)
            indentResult += indent;

        return indentResult;
    }

}

///lpd file scutcure

///ParentTree
///[
///     ChildTree
///     [
///         LateChildTree
///         [
///             ...
///         ]
///     ]
///     
///     ChildTree
///     ChildTree
///     ChildTree
///     ...
///]