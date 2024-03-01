using System.Linq;
using UnityEngine;

namespace TabularFrameSystem
{
    [CreateAssetMenu(fileName = "Tag Map", menuName = "Tabular Frame Tag Map", order = 0)]
    public class TagMap : ScriptableObject
    {
        [SerializeField] private string[] m_FrameTags;
        [SerializeField] private string[] m_WidnowTags;
        [SerializeField] private string[] m_ElementTags;

        public void GenerateTag()
        {
            string path = $"{Application.dataPath}/Tags/TabularFrameTag.cs";

            CodeGen.CodeGenerator.AddNamespace(nameof(TabularFrameSystem));

            CodeGen.CodeGenerator.AddEnum("FrameTag", m_FrameTags.ToArray(), nameof(TabularFrameSystem));
            CodeGen.CodeGenerator.AddEnum("WindowTag", m_WidnowTags.ToArray(), nameof(TabularFrameSystem));
            CodeGen.CodeGenerator.AddEnum("ElementTag", m_ElementTags.ToArray(), nameof(TabularFrameSystem));

            CodeGen.CodeGenerator.GenerateCode("TabularFrameTag", path);
        }
    }
}