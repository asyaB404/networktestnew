using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public static class Utils
{
    //得到枚举中的随机值
    public static T GetRandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(MyRandom.Instance.NextInt(values.Length));
    }
    
    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    
    public static void ShuffleArray<T>(this IList<T> array)
    {
        Random rand = new();

        for (int i = array.Count() - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }

    public static void ShuffleArray<T>(this IList<T> array, int iseed)
    {
        Random rand = new(iseed);

        for (int i = array.Count() - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }
}