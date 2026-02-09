using System;

namespace EU_values.Utilities;

public static class GameRandom
{
    private static readonly Random _random = new Random();

    public static int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue) minValue = maxValue;
        return _random.Next(minValue, maxValue);
    }
    public static int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }
    public static int Next()
    {
        return _random.Next();
    }

    public static List<int> GetRandomIndexes(int arrayLength, List<int>? indexes = null)
    {
        if (indexes == null)
        {
            indexes = new List<int>();
            for (int i = 0; i < arrayLength; i++) indexes.Add(i);
        }
        if (indexes.Count == 0) return new List<int>();


        int ind = GameRandom.Next(0, indexes.Count);

        List<int> newIndexes = [];

        for (int i = 0; i < indexes.Count; i++)
        {
            if (i != ind) newIndexes.Add(indexes[i]);
        }

        var listToReturn = GetRandomIndexes(arrayLength, newIndexes);
        listToReturn.Add(indexes[ind]);

        return listToReturn;
    }
}
