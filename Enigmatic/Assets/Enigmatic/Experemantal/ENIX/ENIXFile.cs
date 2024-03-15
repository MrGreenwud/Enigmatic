using System.Collections.Generic;

namespace Enigmatic.Experemental.ENIX
{
    // *.enix

    public static class ENIXFile
    {
        private static SerializebleObject s_SerializebleObject;

        public static void Create(string name)
        {
            s_SerializebleObject = new SerializebleObject(name);
        }

        public static SerializebleProperty AddProperty(string name, string type, string value)
        {
            if(s_SerializebleObject == null)
                throw new System.InvalidOperationException();

            SerializebleProperty property = new SerializebleProperty(name, type, value);
            s_SerializebleObject.AddProperty(property);
            
            return property;
        }

        public static SerializebleProperty AddProperty(SerializebleProperty parrentProperty, 
            string name, string type, string value)
        {
            if(parrentProperty == null) 
                throw new System.ArgumentNullException(nameof(parrentProperty));

            SerializebleProperty property = new SerializebleProperty(name, type, value);
            parrentProperty.AddProperty(property);

            return property;
        }
    }

    public class SerializebleObject
    {
        private List<SerializebleProperty> m_ChildProperties = new List<SerializebleProperty>();

        public string Name { get; private set; }

        public SerializebleObject(string name)
        {
            Name = name;
        }

        public SerializebleProperty AddProperty(SerializebleProperty property)
        {
            if (property == null)
                throw new System.ArgumentNullException(nameof(property));

            if (m_ChildProperties.Contains(property))
                throw new System.InvalidOperationException();

            m_ChildProperties.Add(property);
            return property;
        }

        public SerializebleProperty GetChildProperty(string name)
        {
            foreach (SerializebleProperty property in m_ChildProperties)
                if (property.Name == name)
                    return property;

            return null;
        }
    }

    public class SerializebleProperty : SerializebleObject
    {
        public string Value { get; set; }
        public string Type { get; private set; }

        public SerializebleProperty(string name, string type) : base(name) 
        {
            Type = type;
        }

        public SerializebleProperty(string name, string type, string value) : base(name)
        {
            Type = type;
            Value = value;
        }
    }
}

//sting -> {string}
//int   -> {int}
//float -> {float}

//simple property sample:
//PropertyName: {type: value}

//parrent proprty sample
//PropertyName
//[
//  ...
//]

//simple propery examples:
// m_Name: {sting: Bob}
// m_Health: {int: 20}
// m_MoveSpeed: {float: 5.6}
// m_GroundLayer: {enumMask: 1,3,5}
// m_SwordType: {enumflag: 0}

///Parent Property Name
///[
///     Child Property: {type} value
///     Child Property
///     [
///         Child Next Property {type} value
///         Child Next Property
///         [
///             ...
///         ]
///     ]
///]
///
/// 
/// 
///