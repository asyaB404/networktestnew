using UnityEngine;
using Random = System.Random;

/// <summary>
///     随机数静态实例，可以使用set指定种子。
/// </summary>
public static class MyRandom
{
    private static Random _random;

    public static Random RandomInstance
    {
        get
        {
            if (_random != null) return _random;
            _random = new Random();
            Debug.LogWarning("注意随机数没有手动设置种子!已自动生成随机生成");

            return _random;
        }
    }

    /// <summary>
    ///     设置指定随机种子
    /// </summary>
    /// <param name="seed"></param>
    public static void Set(int seed)
    {
        _random = new Random(seed);
    }


    public static int NextInt()
    {
        return RandomInstance.Next();
    }

    public static int NextInt(int max)
    {
        return RandomInstance.Next(max);
    }

    public static int NextInt(int min, int max)
    {
        return RandomInstance.Next(min, max);
    }

    public static float NextFloat()
    {
        return (float)RandomInstance.NextDouble();
    }

    public static float NextFloat(float min, float max)
    {
        return (float)(min + RandomInstance.NextDouble() * (max - min));
    }

    public static bool NextBool()
    {
        return RandomInstance.Next(2) == 0;
    }
}