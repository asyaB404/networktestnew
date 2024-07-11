using System;

public class MyRandom
{
    private readonly Random _random;
    private static MyRandom _instance;

    public static MyRandom Instance
    {
        get
        {
            _instance ??= new MyRandom();
            return _instance;
        }
    }

    public static void Set(int seed)
    {
        _instance = new MyRandom(seed);
    }

    private MyRandom()
    {
        _random = new Random();
    }

    private MyRandom(int seed)
    {
        _random = new Random(seed);
    }

    public int NextInt()
    {
        return _random.Next();
    }

    public int NextInt(int max)
    {
        return _random.Next(max);
    }

    public int NextInt(int min, int max)
    {
        return _random.Next(min, max);
    }

    public float NextFloat()
    {
        return (float)_random.NextDouble();
    }

    public float NextFloat(float min, float max)
    {
        return (float)(min + (_random.NextDouble() * (max - min)));
    }

    public bool NextBool()
    {
        return _random.Next(0, 2) == 0;
    }
}