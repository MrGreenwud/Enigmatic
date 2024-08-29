using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace Enigmatic.Experemental.ENIX
{
    // *.enix

    public struct PropertyData
    {
        private object Owner;
        private object Property;
        
        public object Key { get; private set; }
        public object Value { get; private set; }
        
        public Type GetPropertyType() => Property.GetType();

        public PropertyData(object owner, object property, object value, object key = null)
        {
            Owner = owner; 
            Property = property;

            Key = key;
            Value = value;
        }

        public void SetValue(object value, object key = null)
        {
            Type propertyType = Property.GetType();

            if (propertyType.IsArray == true)
            {
                Array array = (Array)Property;
                Type elementType = array.GetType().GetElementType();

                if(elementType.IsAssignableFrom(value.GetType()) == false)
                {
                    Debug.LogWarning("");
                    return;
                }

                for (int i = 0; i < array.Length; i++)
                {
                    if (array.GetValue(i) != null)
                        continue;

                    array.SetValue(value, i);
                }
            }
            else if (propertyType.IsGenericType == true)
            {
                if (propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) 
                {
                    IDictionary dictionary = (IDictionary)Property;
                    
                    Type[] argsType = dictionary.GetType().GetGenericArguments();
                    Type keyType = argsType[0];
                    Type valueType = argsType[1];

                    if (keyType.IsAssignableFrom(key.GetType()) == false  
                        ||  valueType.IsAssignableFrom(value.GetType()) == false)
                    {
                        Debug.Log($"{keyType} => {key.GetType()}");
                        Debug.Log($"{valueType} => {value.GetType()}");

                        Debug.LogWarning($"");
                        return;
                    }

                    dictionary.Add(key, value);
                }
                else if (propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList list = (IList)Property;
                    Type elementType = list.GetType().GetGenericArguments()[0];

                    if(elementType.IsAssignableFrom(value.GetType()) == false)
                    {
                        Debug.LogWarning("");
                        return;
                    }

                    list.Add(value);
                }
            }
            else if (propertyType == Type.GetType("System.Reflection.RuntimeFieldInfo"))
            {
                FieldInfo field = Property as FieldInfo;
                
                if (field.FieldType.IsAssignableFrom(value.GetType()) == false)
                {
                    Debug.LogWarning("");
                    return;
                }

                field.SetValue(Owner, value);
            }
        }
    }

    public static class ENIXFile
    {
        private static Dictionary<object, string> sm_RegisteredSerializeObjects = new Dictionary<object, string>();

        private static Dictionary<string, object> sm_RegisteredDeserializeObject = new Dictionary<string, object>();
        private static List<PropertyData> sm_RegisteredPropertyRequaredObject = new List<PropertyData>();

        private static Dictionary<string, string> sm_SerializedObjects = new Dictionary<string, string>();

        private static uint sm_DepthSerialization;

        private static string Tab => GetTab(sm_DepthSerialization);

        private static string GetTab(uint depth)
        {
            string tab = "\n";

            for (int i = 0; i < depth; i++)
                tab += "\t";

            return tab;
        }

        public static Dictionary<Type, List<object>> FilterObjectsByType(object[] objects, Type[] typeFilter, bool isConsiderBasicTypes = false)
        {
            Dictionary<Type, List<object>> result = new Dictionary<Type, List<object>>(typeFilter.Length);

            foreach (object obj in objects)
            {
                foreach (Type type in typeFilter)
                {
                    if (isConsiderBasicTypes)
                    {
                        if (type.ToString() == obj.GetType().ToString())
                        {
                            if (result.ContainsKey(type) == false)
                                result.Add(type, new List<object>());

                            result[type].Add(obj);
                        }
                    }
                    else
                    {
                        if (type.IsAssignableFrom(obj.GetType()))
                        {
                            if (result.ContainsKey(type) == false)
                                result.Add(type, new List<object>());

                            result[type].Add(obj);
                        }
                    }
                }
            }

            return result;
        }

        public static object[] Deserialize(string enixFile)
        {
            ResetAll();

            string[] lines = enixFile.Split("\n");

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Object"))
                {
                    string serializedObject = string.Empty;
                    string guid = lines[i].Split(':')[1].Trim();

                    for (int j = i; j < lines.Length; j++)
                    {
                        serializedObject += lines[j];

                        if (lines[j].Contains("{"))
                            sm_DepthSerialization++;
                        else if (lines[j].Contains("}"))
                            sm_DepthSerialization--;

                        if (sm_DepthSerialization == 0)
                            break;
                    }

                    sm_SerializedObjects.Add(guid, serializedObject);
                }
            }

            DeserializeObjects(sm_SerializedObjects.Values.ToArray());

            return sm_RegisteredDeserializeObject.Values.ToArray();
        }

        public static object[] Deserialize(ENIXContaner contaner)
        {
            ResetAll();

            return DeserializeObjects(contaner.SerializedObject.ToArray());
        }

        public static object[] DeserializeObjects(string[] serializedObjects)
        {
            ResetAll();

            foreach (string obj in serializedObjects)
            {
                string guid = obj.Split("\n")[1].Split(":")[2].Trim();
                sm_SerializedObjects.Add(guid, obj);
            }

            foreach (string guid in sm_SerializedObjects.Keys)
            {
                if (sm_RegisteredDeserializeObject.ContainsKey(guid))
                    continue;

                DeserializeObject(sm_SerializedObjects[guid]);
            }

            foreach (PropertyData data in sm_RegisteredPropertyRequaredObject)
            {
                object key = null;
                object value = null;

                Type propertyType = data.GetPropertyType();

                if (propertyType.IsGenericType == true && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] argsType = propertyType.GetGenericArguments();
                    Type keyType = argsType[0];
                    Type valueType = argsType[1];

                    if (IsClass(keyType) == true)
                    {
                        if (sm_RegisteredDeserializeObject.ContainsKey(data.Key.ToString()) == false)
                            continue;

                        key = sm_RegisteredDeserializeObject[data.Key.ToString()];
                    }
                    else
                    {
                        key = data.Key;
                    }

                    if (IsClass(valueType) == true)
                    {
                        //Debug.Log(data.Value.ToString());
                        value = sm_RegisteredDeserializeObject[data.Value.ToString()];
                    }
                    else
                        value = data.Value;
                }
                else
                {
                    if (sm_RegisteredDeserializeObject.ContainsKey(data.Value.ToString()) == false)
                        continue;

                    value = sm_RegisteredDeserializeObject[data.Value.ToString()];
                }

                data.SetValue(value, key);
            }

            return sm_RegisteredDeserializeObject.Values.ToArray();
        }

        public static void DeserializeObject(string serializedObject)
        {
            string[] lines = serializedObject.Split('\n');
            string line = lines[1].Trim();

            string[] part = line.Split(":");
            Type ObjectType = Type.GetType(part[1].Trim());
            string guid = part[2].Trim();

            if (ObjectType == null)
            {
                Debug.LogWarning("");
                return;
            }

            object obj = Activator.CreateInstance(ObjectType);

            FieldInfo[] fields = GetAllFieldsByType(ObjectType, BindingFlags.Public
              | BindingFlags.NonPublic | BindingFlags.Instance);

            Dictionary<string, string> serializedPropertes = GetPropertes(serializedObject);

            foreach (string propertyName in serializedPropertes.Keys)
            {
                string serializedProperty = serializedPropertes[propertyName];

                FieldInfo field = GetFieldByName(fields, propertyName);

                if (field == null)
                    continue;

                object value = DeserializeProperty(serializedProperty, field.FieldType);

                if (IsClass(field.FieldType) == true)
                {
                    PropertyData propertyData = new PropertyData(obj, field, value.ToString());
                    sm_RegisteredPropertyRequaredObject.Add(propertyData);
                }
                else
                {
                    field.SetValue(obj, value);
                }
            }

            sm_RegisteredDeserializeObject.Add(guid, obj);
        }

        public static object DeserializeProperty(string serializedProperty, Type propertyType)
        {
            if (propertyType.IsArray)
            {
                Type elementType = propertyType.GetElementType();
                Dictionary<string, string> elements = GetPropertes(serializedProperty);

                Array array = Array.CreateInstance(elementType, elements.Count);

                uint iteration = 0;
                foreach (string elementName in elements.Keys)
                {
                    object value = DeserializeProperty(elements[elementName], elementType);

                    if (elementType.IsClass == true && elementType != typeof(string))
                    {
                        PropertyData propertyData = new PropertyData(null, array, value.ToString());
                        sm_RegisteredPropertyRequaredObject.Add(propertyData);
                    }
                    else
                    {
                        array.SetValue(value, iteration);
                    }

                    iteration++;
                }

                return array;
            }
            else if (propertyType.IsGenericType)
            {
                if (propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] argsType = propertyType.GetGenericArguments();
                    Type keyType = argsType[0];
                    Type valueType = argsType[1];

                    Dictionary<string, string> elements = GetPropertes(serializedProperty);

                    Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                    IDictionary dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);

                    foreach (string elementName in elements.Keys)
                    {
                        Dictionary<string, string> pairs = GetPropertes(elements[elementName]);

                        object key = DeserializeProperty(pairs["Key"], keyType);
                        object value = DeserializeProperty(pairs["Value"], valueType);

                        if (IsClass(keyType) == true || IsClass(valueType) == true)
                        {
                            PropertyData data = new PropertyData(null, dictionary, value, key);
                            sm_RegisteredPropertyRequaredObject.Add(data);
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }

                    return dictionary;
                }
                else if (propertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type elementType = propertyType.GetGenericArguments()[0];
                    Dictionary<string, string> elements = GetPropertes(serializedProperty);

                    Type listType = typeof(List<>).MakeGenericType(elementType);
                    IList list = (IList)Activator.CreateInstance(listType);

                    foreach (string elementName in elements.Keys)
                    {
                        object value = DeserializeProperty(elements[elementName], elementType);

                        if (elementType.IsClass())
                        {
                            PropertyData propertyData = new PropertyData(null, list, value.ToString());
                            sm_RegisteredPropertyRequaredObject.Add(propertyData);
                        }
                        else
                        {
                            list.Add(value);
                        }
                    }

                    return list;
                }

                return null;
            }
            else if (propertyType.IsStruct())
            {
                object property = Activator.CreateInstance(propertyType);
                Dictionary<string, string> childPropertes = GetPropertes(serializedProperty);

                foreach (string propertyName in childPropertes.Keys)
                {
                    FieldInfo field = propertyType.GetField(propertyName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    object value = DeserializeProperty(childPropertes[propertyName], field.FieldType);

                    if (IsClass(field.FieldType) == true)
                    {
                        Debug.Log($"{propertyName} {value}");
                        PropertyData propertyData = new PropertyData(property, field, value.ToString());
                        sm_RegisteredPropertyRequaredObject.Add(propertyData);
                    }
                    else
                    {
                        field.SetValue(property, value);
                    }
                }

                return property;
            }
            else if (propertyType.IsClass)
            {
                string guid = serializedProperty.Split(":")[1].Trim();
                return guid;
            }
            else if (propertyType.IsEnum)
            {
                int value = int.Parse(serializedProperty.Split(":")[1].Trim());
                return Enum.ToObject(propertyType, value);
            }
            else
            {
                string value = serializedProperty.Split(":")[1].Trim();

                if (propertyType == typeof(float))
                {
                    if (float.TryParse(value, out float outValue) == false)
                        return string.Empty;

                    return outValue;
                }
                else if (propertyType == typeof(int))
                {
                    if (int.TryParse(value, out int outValue) == false)
                        return 0;

                    return outValue;
                }
                else if (propertyType == typeof(string))
                {
                    return value;
                }
                else if (propertyType == typeof(bool))
                {
                    return bool.Parse(value);
                }
            }

            throw new Exception($"Property with type : {propertyType} " +
                $"сan't be deserialize! \n {serializedProperty}");
        }

        public static bool IsClass(this Type type)
        {
            return type.IsClass == true && type.IsGenericType == false
                && type != typeof(string) && type.IsArray == false;
        }

        public static bool IsList(this Type type)
        {
            return type.IsGenericType == true
                && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsDictionary(this Type type)
        {
            return type.IsGenericType == true
                && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        private static Dictionary<string, string> GetPropertes(string serializedElement)
        {
            List<string> lines = serializedElement.Split("\n").ToList();

            if (lines[0] == string.Empty)
                lines.Remove(lines[0]);

            uint depthSerialization = 0;

            Dictionary<string, string> serializedPropertes = new Dictionary<string, string>();

            for (int i = 1; i < lines.Count; i++)
            {
                string fixedLine = lines[i].Trim();

                if (fixedLine.Contains("Object") || fixedLine == string.Empty)
                    continue;

                if (fixedLine.Contains("{"))
                {
                    depthSerialization++;
                }
                else if (fixedLine.Contains("}"))
                {
                    depthSerialization--;
                }
                else if (fixedLine.Contains(":"))
                {
                    string[] parts = fixedLine.Split(":");
                    serializedPropertes.Add(parts[0].Trim(), fixedLine);
                }
                else
                {
                    string propertyName = fixedLine;
                    string serializedProperty = string.Empty;

                    uint curentDepthSerialization = depthSerialization;
                    uint depth;
                    uint lineCount = 0;

                    for (int j = i; j < lines.Count; j++)
                    {
                        if (lines[j].Contains("{"))
                        {
                            depth = curentDepthSerialization - depthSerialization;
                            serializedProperty += $"{GetTab(depth)}{lines[j].Trim()}";
                            curentDepthSerialization++;
                        }
                        else if (lines[j].Contains("}"))
                        {
                            curentDepthSerialization--;
                            depth = curentDepthSerialization - depthSerialization;
                            serializedProperty += $"{GetTab(depth)}{lines[j].Trim()}";
                        }
                        else
                        {
                            depth = curentDepthSerialization - depthSerialization;
                            serializedProperty += $"{GetTab(depth)}{lines[j].Trim()}";
                        }

                        if (curentDepthSerialization == depthSerialization
                            && lines[j].Contains("}"))
                        {
                            lineCount = (uint)(j - i);
                            i = j;
                            break;
                        }
                    }

                    serializedPropertes.Add(propertyName, serializedProperty);
                }
            }

            return serializedPropertes;
        }

        public static void Serialize(string name, object[] objects, string path,
            bool isPackingOnContaner = true, ENIXContaner enixContaner = null)
        {
            sm_RegisteredSerializeObjects.Clear();

            if (Directory.Exists(EnigmaticData.GetFullPath(path)) == false)
                Directory.CreateDirectory(EnigmaticData.GetFullPath(path));

            List<string> serializedObjects = Serialize(objects);

            if (isPackingOnContaner)
            {
                if (enixContaner == null)
                {
                    ENIXContaner contaner = ScriptableObject.CreateInstance<ENIXContaner>();
                    contaner.AddObjects(serializedObjects);
                    string pathWithFile = $"{EnigmaticData.GetUnityPath(path)}/{name}ENIX.asset";
                    AssetDatabase.CreateAsset(contaner, pathWithFile);
                }
                else
                {
                    enixContaner.AddObjects(serializedObjects);
                }
            }
            else
            {
                string enix = "#ENIX v0.1";

                foreach (string serializedObject in serializedObjects)
                    enix += serializedObject;

                string pathWithFile = $"{EnigmaticData.GetFullPath(path)}/{name}.enix";
                File.WriteAllText(pathWithFile, enix);
                Debug.Log(pathWithFile);
            }
        }

        public static List<string> Serialize(object[] objects)
        {
            ResetAll();

            foreach (object obj in objects)
                SerializeObject(obj);

            return sm_SerializedObjects.Values.ToList();
        }

        public static string SerializeObject<T>(T obj) where T : class
        {
            sm_DepthSerialization = 0;
            string serializedObject = string.Empty;

            SerializebleObject serializebleObject = (SerializebleObject)Attribute.GetCustomAttribute(obj.GetType(), typeof(SerializebleObject));

            if (serializebleObject == null)
            {
                Debug.LogError(obj.GetType());
                return string.Empty;
            }

            string guid = Guid.NewGuid().ToString();
            sm_RegisteredSerializeObjects.Add(obj, guid);

            serializedObject += $"{Tab}Object : {obj.GetType()} : {guid}";
            serializedObject += $"{Tab}{{";
            sm_DepthSerialization++;

            //FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public
            //    | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            FieldInfo[] fields = GetAllFieldsByType(obj.GetType(), BindingFlags.Public
              | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo property in fields)
                serializedObject += SerializeProperty(property, obj);

            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
                serializedObject += SerializeProperty(property, obj);

            sm_DepthSerialization--;
            serializedObject += $"{Tab}}}";

            sm_SerializedObjects.Add(guid, serializedObject);
            return serializedObject;
        }

        public static string SerializeProperty(FieldInfo property, object obj, bool isSerializebleProperty = true)
        {
            if (isSerializebleProperty)
            {
                SerializebleProperty serializebleProperty = property.GetAttribute<SerializebleProperty>();

                if (serializebleProperty == null)
                    return string.Empty;
            }

            object value = property.GetValue(obj);
            string name = property.Name;

            return SerializeProperty(value, name, property.FieldType);
        }

        public static string SerializeProperty(PropertyInfo property, object obj)
        {
            SerializebleProperty serializebleProperty = property.GetAttribute<SerializebleProperty>();

            if (serializebleProperty == null)
                return string.Empty;

            object value = property.GetValue(obj);
            string name = property.Name;

            return SerializeProperty(value, name, property.PropertyType);
        }

        public static string SerializeProperty(object property, string name, Type type)
        {
            if (property == null)
                return string.Empty;

            string serializedProperty = string.Empty;

            if (RegisterCustomSerializer.TryGetPropertySerializeMethod(type, out MethodInfo method))
            {
                object[] args = { property, name, type };
                object result = method.Invoke(null, args);
                serializedProperty += result.ToString();
            }
            else if (type.IsArray)
            {
                serializedProperty += $"{Tab}{name}";
                serializedProperty += $"{Tab}{{";
                sm_DepthSerialization++;

                Array array = (Array)property;
                Type elementType = array.GetType().GetElementType();

                for (int i = 0; i < array.Length; i++)
                {
                    serializedProperty += SerializeProperty(array.GetValue(i),
                        $"Element{i}", elementType);
                }

                sm_DepthSerialization--;
                serializedProperty += $"{Tab}}}";
            }
            else if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    IDictionary dictionary = (IDictionary)property;

                    Type[] argsType = type.GetGenericArguments();
                    Type keyType = argsType[0];
                    Type valueType = argsType[1];

                    serializedProperty += $"{Tab}{name}";
                    serializedProperty += $"{Tab}{{";
                    sm_DepthSerialization++;

                    uint elementIndex = 0;
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        serializedProperty += $"{Tab}Element{elementIndex}";
                        serializedProperty += $"{Tab}{{";
                        sm_DepthSerialization++;

                        serializedProperty += SerializeProperty(entry.Key, "Key", keyType);
                        serializedProperty += SerializeProperty(entry.Value, "Value", valueType);

                        sm_DepthSerialization--;
                        serializedProperty += $"{Tab}}}";

                        elementIndex++;
                    }

                    sm_DepthSerialization--;
                    serializedProperty += $"{Tab}}}";
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList list = (IList)property;
                    Type elementType = type.GetGenericArguments()[0];

                    serializedProperty += $"{Tab}{name}";
                    serializedProperty += $"{Tab}{{";
                    sm_DepthSerialization++;

                    uint elementIndex = 0;
                    foreach (object element in list)
                    {
                        serializedProperty += SerializeProperty(element,
                            $"Element{elementIndex}", elementType);

                        elementIndex++;
                    }

                    sm_DepthSerialization--;
                    serializedProperty += $"{Tab}}}";
                }
            }
            else if (type.IsClass && type != typeof(string))
            {
                uint death = sm_DepthSerialization;

                if (sm_RegisteredSerializeObjects.ContainsKey(property) == false)
                    SerializeObject(property);

                sm_DepthSerialization = death;

                serializedProperty = $"{Tab}{name} : {sm_RegisteredSerializeObjects[property]}";
            }
            else if (type.IsStruct())
            {
                SerializebleObject serializebleObject = (SerializebleObject)Attribute.GetCustomAttribute(type, typeof(SerializebleObject));

                if (serializebleObject == null)
                    return string.Empty;

                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                serializedProperty += SerializeStruct(property, name, fields);
            }
            else if (type.IsEnum)
            {
                serializedProperty = $"{Tab}{name} : {(int)property}";
            }
            else
            {
                serializedProperty = $"{Tab}{name} : {property}";
            }

            return serializedProperty;
        }

        public static string SerializeStruct(object property, string name, FieldInfo[] fields, bool isSerializebleProperty = true)
        {
            string serializedStruct = string.Empty;

            serializedStruct += $"{Tab}{name}";
            serializedStruct += $"{Tab}{{";
            sm_DepthSerialization++;

            foreach (FieldInfo field in fields)
                serializedStruct += SerializeProperty(field, property, isSerializebleProperty);

            sm_DepthSerialization--;
            serializedStruct += $"{Tab}}}";

            return serializedStruct;
        }

        public static Type GetSerializedObjectType(string obj)
        {
            string firtLine = obj.Split("\n")[1];

            if (firtLine.Contains("Object") == false)
                throw new ArgumentException();

            string type = firtLine.Split(":")[1].Trim();
            return Type.GetType(type);
        }

        private static void ResetAll()
        {
            sm_DepthSerialization = 0;

            sm_RegisteredDeserializeObject.Clear();
            sm_RegisteredPropertyRequaredObject.Clear();
            sm_RegisteredSerializeObjects.Clear();
            sm_SerializedObjects.Clear();
        }

        public static FieldInfo[] GetAllFieldsByType(Type type, BindingFlags flags)
        {
            List<FieldInfo> result = new List<FieldInfo>();
            HashSet<string> addedFieldNames = new HashSet<string>();

            result.AddRange(type.GetFields(flags));

            foreach (FieldInfo fieldInfo in result)
            {
                addedFieldNames.Add(fieldInfo.Name);
            }

            if (type.BaseType != null)
            {
                FieldInfo[] fieldInfos = GetAllFieldsByType(type.BaseType, flags);

                foreach (FieldInfo field in fieldInfos)
                {
                    if (!addedFieldNames.Contains(field.Name))
                    {
                        result.Add(field);
                        addedFieldNames.Add(field.Name);
                    }
                }
            }

            return result.ToArray();
        }

        public static FieldInfo GetFieldFlattenHierarchy(Type type, string fieldName, BindingFlags flags)
        {
            FieldInfo[] fields = GetAllFieldsByType(type, flags);
            return GetFieldByName(fields, fieldName);
        }

        public static FieldInfo GetFieldByName(FieldInfo[] fields, string fieldName)
        {
            foreach (FieldInfo field in fields)
            {
                if (field.Name == fieldName)
                    return field;
            }

            return null;
        }
    }

    public static class RegisterCustomSerializer
    {
        private static bool s_IsInit;

        private static Dictionary<Type, MethodInfo> s_RegisteredCustomObjectSerializer;
        private static Dictionary<Type, MethodInfo> s_RegisteredCustomPropertySerializer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [InitializeOnLoadMethod]
        private static void Register()
        {
            if (s_IsInit)
                return;

            s_RegisteredCustomObjectSerializer = new Dictionary<Type, MethodInfo>();
            s_RegisteredCustomPropertySerializer = new Dictionary<Type, MethodInfo>();

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach(Type type in assembly.GetTypes())
            {
                if (Attribute.GetCustomAttribute(type, typeof(CustomSerializer)) == null)
                    continue;

                MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach(MethodInfo method in methods)
                {
                    CustomPropertySerializerMethod propertySerializerMethod = (CustomPropertySerializerMethod)method.GetAttribute(typeof(CustomPropertySerializerMethod));
                    CustomPropertySerializerMethod objectSerializerMethod = null;

                    if (propertySerializerMethod != null
                        && objectSerializerMethod != null)
                    {
                        Debug.LogError($"Method {method.Name} use two " +
                            $"custom serializer attribut!");

                        continue;
                    }
                    else if(propertySerializerMethod == null
                        && objectSerializerMethod == null)
                    {
                        continue;
                    }

                    if (propertySerializerMethod != null)
                    {
                        Type objType = propertySerializerMethod.Type;
                        s_RegisteredCustomPropertySerializer.Add(objType, method);
                    }
                    else if (objectSerializerMethod != null)
                    {
                        Type objType = objectSerializerMethod.Type;
                        s_RegisteredCustomObjectSerializer.Add(objType, method);
                    }
                }
            }

            s_IsInit = true;
        }

        public static bool TryGetPropertySerializeMethod(Type propertyType, out MethodInfo method)
        {
            method = null;

            if(s_RegisteredCustomPropertySerializer.ContainsKey(propertyType) == false)
                return false;

            method = s_RegisteredCustomPropertySerializer[propertyType];
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class SerializebleObject : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SerializebleProperty : Attribute { }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomSerializer : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CustomPropertySerializerMethod : Attribute 
    {
        public Type Type { get; private set; }

        public CustomPropertySerializerMethod(Type type) => Type = type;
    }
}

///
/// GameObject : 04ad4206-bcd4-4262-813c-3a5ab80fb1fc
/// {
///     
/// }
/// 
/// Object : 04ad4206-bcd4-4262-813c-3a5ab80fb1fc
/// {
///     Type : Root
///     Property
///     {
///         PropertyType : 1
///         Type  : string
///         Name  : m_UID
///         Value : 04ad4206-bcd4-4262-813c-3a5ab80fb1fc
///     }
///     Property
///     {
///         PropertyType : 6
///         Type : MoveSettings
///         Name : m_MoveSettings
///         Property
///         {
///             PropertyType : 1
///             Type  : float
///             Name  : m_WalkSpeed
///             Value : 5
///         }
///         Property
///         {
///             PropertyType : 1
///             Type  : float
///             Name  : m_RunSpeed
///             Value : 10
///         }
///         Property
///         {
///             PropertyType : 1
///             Type  : float
///             Name  : m_DuckingSpeed
///             Value : 2
///         }
///     }
///     Property
///     {
///         PropertyType : 7
///         Name  : m_MoveSettings
///         Value : 04ad4206-bcd4-4262-897c-3a5ab80fb1fc
///     }
///     Property
///     {
///         PropertyType : 4
///         Type  : Port
///         Name  : m_Ports
///         Element
///         {
///             Port
///             {
///                 ....
///             }
///         }
///         Element
///         {
///             Port
///             {
///                 ....
///             }
///         }
///         Element
///         {
///             Port
///             {
///                 ....
///             }
///         }
///     }
///     Property
///     {
///         Type sting
///         name : f;dgf;dgf
///         Element1
///         {
///             Property        
///             {
///                 Element1 { ... }
///                 Element2 { ... }
///             }
///         }
///         Element2
///         {
///             Property        
///             {
///                 Element1 { ... }
///                 Element2 { ... }
///             }
///         }
///     }
/// }
///

///
/// Object : Root : 04ad4206-bcd4-4262-813c-3a5ab80fb1fc
/// {
///     m_FloatValue : 10
///     m_IntValue : 5
///     m_String : Hello
///     m_MoveSettings
///     {
///         m_WalkSpeed : 5
///         m_RunSpeed : 10
///         m_DuckingSpeed : 2
///     }
///     m_Target : 32ad4206-bcf4-4322-897c-3a5ab85451fc
///     m_IntArray
///     {
///         Element1 : 6
///         Element2 : 5
///         Element3 : 5
///     }
///     m_Settings
///     {
///         Element1
///         {
///             m_Value : 5
///             m_Pinch : 10
///         }
///         Element2
///         {
///             m_Value : 2
///             m_Pinch : 5
///         }
///     }
///     m_ListInList
///     {
///         Element
///         {
///             Element
///             {
///                 ...
///             }
///         }
///         Element
///         {
///             ...
///         }
///     }
/// }
///

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

///
/// index : 0
/// {
///     text
/// }
///
