using UnityEngine;
using Random = System.Random;

/// <summary>
/// 随机数静态实例，可以使用set指定种子。
/// </summary>
public class MyRandom
{
    private static Random _random;
    private static MyRandom _instance;

    public static MyRandom Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MyRandom();
                Debug.LogWarning("注意随机数没有手动设置种子!已自动生成随机生成");
            }

            return _instance;
        }
    }

    /// <summary>
    /// 设置指定随机种子
    /// </summary>
    /// <param name="seed"></param>
    public static void Set(int seed)
    {
        _instance = new MyRandom(seed);
    }


    public static int NextInt()
    {
        return _random.Next();
    }

    public static int NextInt(int max)
    {
        return _random.Next(max);
    }

    public static int NextInt(int min, int max)
    {
        return _random.Next(min, max);
    }

    public static float NextFloat()
    {
        return (float)_random.NextDouble();
    }

    public static float NextFloat(float min, float max)
    {
        return (float)(min + (_random.NextDouble() * (max - min)));
    }

    public static bool NextBool()
    {
        return _random.Next(2) == 0;
    }

    private MyRandom()
    {
        _random = new Random();
    }

    private MyRandom(int seed)
    {
        _random = new Random(seed);
    }
}