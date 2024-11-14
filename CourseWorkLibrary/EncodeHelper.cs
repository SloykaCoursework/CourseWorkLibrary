using System;
using System.Numerics;

namespace CourseWorkLibrary;

public static class EncodeHelper
{
    public static uint NearestBase2(uint find)
    {
        uint r = 1;
        while (r < find)
        {
            r *= 2;
        }

        return r;
    }

    public static byte EncodeLengthToBase2Upper6Bit(ushort value)
    {
        var n = NearestBase2(value);
        var l = (byte)(BitOperations.Log2(n) + 1);
        return (byte)(l << 2);
    }

    public static ushort DecodeBase2Upper6BitToLength(byte value)
    {
        var s = value >> 2;
        var n = Math.Pow(2, s - 1);
        var n1 = NearestBase2((uint)n);
        return (ushort)n1;
    }
}
