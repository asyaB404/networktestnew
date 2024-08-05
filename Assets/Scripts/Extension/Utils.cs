using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public static class Utils
{
    /// <summary>
    ///     得到枚举中的随机值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <returns></returns>
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(MyRandom.NextInt(values.Length));
    }

    public static Transform[] GetAllChildTransforms(this Transform transform)
    {
        var res = new Transform[transform.childCount];
        for (var i = 0; i < transform.childCount; i++) res[i] = transform.GetChild(i);

        return res;
    }

    /// <summary>
    ///     销毁一个transform下的所有子物体
    /// </summary>
    /// <param name="transform"></param>
    public static void DestroyAllChildren(this Transform transform)
    {
        for (var i = transform.childCount - 1; i >= 0; i--) Object.DestroyImmediate(transform.GetChild(i).gameObject);
    }

    /// <summary>
    ///     洗牌算法，将一个数组或者列表以以O(N)的复杂度随机打乱
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    public static void ShuffleArray<T>(this IList<T> array)
    {
        Random rand = new();

        for (var i = array.Count - 1; i > 0; i--)
        {
            var j = rand.Next(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }

    public static void ShuffleArray<T>(this IList<T> array, int seed)
    {
        Random rand = new(seed);

        for (var i = array.Count - 1; i > 0; i--)
        {
            var j = rand.Next(0, i + 1);
            (array[j], array[i]) = (array[i], array[j]);
        }
    }

    /// <summary>
    ///     bezier2D抛物线插值
    /// </summary>
    /// <param name="t"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Vector2 Bezier(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(ab, bc, t);
    }
}