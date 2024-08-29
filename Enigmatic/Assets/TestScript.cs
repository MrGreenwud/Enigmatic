using System;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private float floatValue = 4f;
    [SerializeField] [TextArea] private string text;
    [SerializeField] private int intValue = 10;
    [SerializeField] private int[] intArray = { 8, 20, 15};
    [SerializeField] private List<TestStruct> TestStructs;
    [SerializeField] private List<TestClass> TestClases;
    [SerializeField] private TestStruct testStruct = new TestStruct(294, 232);
    [SerializeField] private TestClass testClass = new TestClass();
    [SerializeField] private GameObject testGameObject;
    [SerializeField] private KeyCode KeyCode;

    private void Awake()
    {
        Debug.Log("Awake!");
    }
}

public static class TestStaticClass
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void TestMethod()
    {
        Debug.Log("LoadedTest");
    }
}

[Serializable]
public struct TestStruct
{
    [SerializeField] private float floatValue;
    [SerializeField] private int intValue;
    [SerializeField] private int[] intArray;

    public TestStruct(float floatValue, int intValue)
    {
        this.floatValue = floatValue;
        this.intValue = intValue;
        intArray = new int[intValue];
    }
}

[Serializable]
public class TestClass
{
    [SerializeField] private float floatValue = 6;
    [SerializeField] private int intValue = 5;
    [SerializeField] private int[] intArray;
}
