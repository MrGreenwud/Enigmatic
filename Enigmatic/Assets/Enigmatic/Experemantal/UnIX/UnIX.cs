using System;
using System.Reflection;
using UnityEngine;
using Enigmatic.Experemental.ENIX;

namespace Enigmatic.Experemental.UnIX
{
    [CustomSerializer]
    public static class UnIX
    {
        public static string SerializeProperty(object property, string name, Type type)
        {
            string serializedProperty = string.Empty;

            if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4))
            {
                serializedProperty += SerializeVector(property, name, type);
            }
            else
            {
                serializedProperty += ENIXFile.SerializeProperty(property, name, type);
            }

            return serializedProperty;
        }

        public static string SerializeObject<T>(T obj) where T : class
        {
            string serializedObject = string.Empty;

            MethodInfo[] methods = typeof(UnIX).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            if (obj.GetType() == typeof(Transform)) { }

            return serializedObject;
        }

        [CustomPropertySerializerMethod(typeof(Vector2))]
        public static string SerializeVector2(object property, string name, Type type)
        {
            return SerializeVector(property, name, type);
        }

        [CustomPropertySerializerMethod(typeof(Vector3))]
        public static string SerializeVector3(object property, string name, Type type)
        {
            return SerializeVector(property, name, type);
        }

        public static string SerializeVector(object property, string name, Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            return ENIXFile.SerializeStruct(property, name, fields, false);
        }
    }
}
