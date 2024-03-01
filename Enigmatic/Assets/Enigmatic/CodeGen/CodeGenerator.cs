using System.Collections.Generic;
using UnityEngine;

namespace CodeGen
{
    public static class CodeGenerator
    {
        private static string s_Indent = "    ";
        
        private static string s_Code;

        private static List<string> s_Usings = new List<string>();
        private static List<string> s_Namespaces = new List<string>();

        private static List<CEEnum> s_Enums = new List<CEEnum>();
        
        public static void GenerateCode(string fileName, string path)
        {
            foreach (string @using in s_Usings)
                s_Code += $"using {@using} \n";

            //namespaces
            foreach (string @namespace in s_Namespaces)
            {
                s_Code += GenerateNamespcae(@namespace);
                
                //enums
                foreach (CEEnum @enum in s_Enums)
                {
                    s_Code += GenerateEnum(@enum);
                    s_Code += $"\n";
                }

                //End Namespace
                s_Code += $"}}";
            }

            FixCode();

            System.IO.File.WriteAllText(path, s_Code);
            Clear();
            
            Debug.Log($"{fileName} SAVED IN PATH {path}"); 
        }

        public static void AddNamespace(string @namespace)
        {
            s_Namespaces.Add(@namespace);
        }

        public static void AddEnum(string enumName, string[] enumValues, string @namespace = "")
        {
            if (@namespace != "")
            {
                if (CheckNamespace(@namespace) == false)
                    AddNamespace(@namespace);
            }

            s_Enums.Add(new CEEnum(enumName, @namespace, enumValues));
        }

        private static string GenerateNamespcae(string @namespace)
        {
            string code = $"namespace {@namespace}" +
                                       $"{{";

            return code;
        }

        private static string GenerateEnum(CEEnum @enum)
        {
            string code = $"public enum {@enum.Name}" +
                          $"{{" +
                          $"{@enum.Values}" +
                          $"}}";

            return code;
        }

        private static bool CheckNamespace(string @namespace)
        {
            foreach (string nameSpace in s_Namespaces)
                if (nameSpace == @namespace)
                    return true;

            return false;
        }

        private static void FixCode()
        {
            int indentCount = 0;
            string newCode = "";

            for (int i = 0; i < s_Code.Length; i++)
            {
                if (s_Code[i] == '{' || s_Code[i] == '}')
                {
                    if(s_Code[i] == '}')
                        indentCount--;

                    newCode += "\n";

                    newCode += GetIndents(indentCount);

                    newCode += s_Code[i];
                    newCode += "\n";

                    if (s_Code[i] == '{')
                        indentCount++;

                    newCode += GetIndents(indentCount);
                }
                else if (s_Code[i] == '\n')
                {
                    newCode += '\n';
                    newCode += GetIndents(indentCount);
                }
                else
                {
                    newCode += s_Code[i];
                }
            }

            s_Code = newCode;
            //Debug.Log(indentCount);
        }

        private static string GetIndents(int count)
        {
            string indents = "";

            for (int j = 0; j < count; j++)
                indents += s_Indent;

            return indents;
        }

        private static void Clear()
        {
            s_Code = "";
            s_Namespaces = new List<string>();
            s_Enums = new List<CEEnum>();
        }
    }
}

// Method       CreateMethod(string returnedValue, )

//              Data Types : int, float, string, bool, char
//              Data Types : #int, #float, #sting, #bool, #char #castom(Class name)

//              /m *method* /m
//              /f *field* /f
//              /p *property* /p