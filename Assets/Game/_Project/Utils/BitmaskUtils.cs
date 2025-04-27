using System;
using System.Collections.Generic;

public static class BitmaskUtils
{
    private static int PackBoolsToInt(bool[] bools)
    {
        if (bools.Length > 32)
            throw new ArgumentException("Нельзя упаковать больше 32 bool-значений в один int!");

        var result = 0;
        for (var i = 0; i < bools.Length; i++)
        {
            if (bools[i])
                result |= 1 << i;
        }
        return result;
    }

    private static bool[] UnpackIntToBools(int packedValue)
    {
        const int MaxBits = sizeof(int) * 8;
        var bools = new List<bool>(MaxBits);

        for (var i = 0; i < MaxBits; i++)
        {
            bools.Add((packedValue & (1 << i)) != 0);
        }

        // Удаляем хвостовые false (опционально)
        while (bools.Count > 0 && !bools[^1])
        {
            bools.RemoveAt(bools.Count - 1);
        }

        return bools.ToArray();
    }
}