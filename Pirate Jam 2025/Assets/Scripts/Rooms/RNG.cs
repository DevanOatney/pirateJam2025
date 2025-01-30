using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RNG
{
    public static bool Chance(float chance)
    {
        return Random.value <= chance && chance > 0;
    }

    public static T ChooseFrom<T>(params T[] args)
        => Choose(args);

    public static T Choose<T>(in IEnumerable<T> args)
    {
        if (args.Count() == 1)
            return args.First();

        return args.ElementAt(Random.Range(0, args.Count() - 1));
    }
}
