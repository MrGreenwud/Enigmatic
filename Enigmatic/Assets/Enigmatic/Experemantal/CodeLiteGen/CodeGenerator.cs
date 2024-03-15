using System;
using System.IO;

namespace Enigmatic.Experemental.CodeLiteGen
{
    public static class CodeGenerator
    {
        public static void Generate(string code, string path, string fileName)
        {
            string fixedCode = FixCode(code);
            File.WriteAllText($"{path}/{fileName}.cs", fixedCode);
        }

        public static string GetNames<T>(T enumType) where T : Type
        {
            string result = string.Empty;

            string[] names = Enum.GetNames(typeof(T));

            for (int i = 0; i < names.Length; i++)
            {
                result += names[i];

                if (i + 1 != names.Length)
                    result += ",";
            }

            return result;
        }

        private static string FixCode(string code)
        {
            string fixedCode = string.Empty;

            string[] lines = code.Split('\n');

            uint deathLevel = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line == "}")
                    deathLevel--;

                fixedCode += $"{FileEditor.Space(deathLevel)}{line}\n";

                if (line == "{")
                    deathLevel++;
            }

            return fixedCode;
        }
    }
}
