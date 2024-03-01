using System.Linq;

namespace CodeGen
{
    internal class CodeElement
    {
        private string m_Name;
        public string Name => m_Name;

        public CodeElement(string name)
        {
            m_Name = name;
        }
    }

    internal class NamespaceChild : CodeElement
    {
        private string m_Namespace;
        public string Namespace => m_Namespace;

        public NamespaceChild(string name, string newNamespace) : base(name)
        {
            m_Namespace = newNamespace;
        }
    }

    internal class CEEnum : NamespaceChild
    {
        private string[] m_Values;
        public string Values => StringConvertor.ConvertListToLine(m_Values.ToList());

        public CEEnum(string name, string namespcae, string[] values) :
            base(name, namespcae)
        {
            m_Values = values;
        }
    }       
}