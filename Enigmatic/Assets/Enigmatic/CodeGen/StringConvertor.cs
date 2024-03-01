using System.Collections.Generic;

namespace CodeGen
{
    public static class StringConvertor
    {
        public static string ConvertArrayToLine(string[] tagArray)
        {
            string tag = "";

            for (int i = 0; i < tagArray.Length; i++)
            {
                tag += $"{tagArray[i]}";

                if (i + 1 != tagArray.Length)
                    tag += $", \n";
            }

            return tag;
        }

        public static string ConvertListToLine(List<string> tagList)
        {
            string[] tagsArray = tagList.ToArray();
            return ConvertArrayToLine(tagsArray);
        }
    }
}
