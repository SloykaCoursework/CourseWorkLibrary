using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CourseWorkLibrary;

public static class CommandHelper
{
    public static bool TryDeserializeCommand(this byte[] requestArray, out Command? command)
    {
        var commandCode = requestArray[1] >> 2;
        var lengthBase = requestArray[2];
        var dataLength = EncodeHelper.DecodeBase2Upper6BitToLength(lengthBase);

        command = null;

        if (requestArray[3] != Const.STX && requestArray[4 + dataLength] != Const.ETX)
        {
            Console.WriteLine("Wrong message");
            return false;
        }

        var dataList = new List<byte>();
        for (int i = 4; i < requestArray.Length - 1; i++)
        {
            if (requestArray[i] == 0)
            {
                break;
            }

            dataList.Add(requestArray[i]);
        }

        var dataBytes = dataList.ToArray();

        Console.WriteLine($"> C: {commandCode}");
        Console.WriteLine($"> L: {lengthBase}");
        Console.WriteLine($"> len: {dataLength}");
        var base64Json = Encoding.ASCII.GetString(dataBytes);
        Console.WriteLine($"> BASE64: {base64Json}");
        var jsonBytes = Convert.FromBase64String(base64Json);

        var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonBytes);

        Console.WriteLine($"> RAW: [{ToReadableByteArray(requestArray)}]");

        command = new Command()
        {
            Code = (byte)commandCode,
            Arguments = data ?? new Dictionary<string, object?>()
        };
        Console.WriteLine($"> JSON: {JsonSerializer.Serialize(command)}");
        return true;
    }

    public static byte[] SerializeCommand(this Command command)
    {
        var bytesList = new List<byte>
        {
            Const.SOH,
            (byte)(command.Code << 2)
        };

        var argsJson = JsonSerializer.Serialize(command.Arguments);
        var jsonBytes = Encoding.ASCII.GetBytes(argsJson);

        var base64Json = Convert.ToBase64String(jsonBytes);
        var base64Bytes = Encoding.ASCII.GetBytes(base64Json);
        Console.WriteLine($"< JSON: {JsonSerializer.Serialize(command)}");
        Console.WriteLine($"< BASE64: {base64Json}");

        var base2Length = EncodeHelper.EncodeLengthToBase2Upper6Bit((ushort)base64Bytes.Length);
        var nearest2Length = EncodeHelper.NearestBase2((ushort)base64Bytes.Length);
        Array.Resize(ref base64Bytes, (int)nearest2Length);

        Console.WriteLine($"< C: {command.Code}");
        Console.WriteLine($"< L: {base2Length}");
        Console.WriteLine($"< len: {nearest2Length}");


        bytesList.Add(base2Length);

        bytesList.Add(Const.STX);
        bytesList.AddRange(base64Bytes);
        bytesList.Add(Const.ETX);

        var full = bytesList.ToArray();
        Console.WriteLine($"< RAW: [{ToReadableByteArray(full)}]");
        return full;
    }

    private static string ToReadableByteArray(byte[] bytes)
    {
        return string.Join(", ", bytes);
    }
}
