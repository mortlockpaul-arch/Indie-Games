using System.Collections.Generic;
using System.Runtime.Intrinsics;

namespace System.Buffers;

internal static class TeddyBucketizer
{
	public static (Vector512<byte> Low, Vector512<byte> High) GenerateNonBucketizedFingerprint(ReadOnlySpan<string> values, int offset)
	{
		Vector128<byte> vector = default(Vector128<byte>);
		Vector128<byte> vector2 = default(Vector128<byte>);
		for (int i = 0; i < values.Length; i++)
		{
			string obj = values[i];
			int num = 1 << i;
			char num2 = obj[offset];
			int index = num2 & 0xF;
			int index2 = (int)num2 >> 4;
			vector.SetElementUnsafe(index, (byte)(Vector128.GetElementUnsafe(in vector, index) | num));
			vector2.SetElementUnsafe(index2, (byte)(Vector128.GetElementUnsafe(in vector2, index2) | num));
		}
		return (Low: Vector512.Create(vector), High: Vector512.Create(vector2));
	}

	public static (Vector512<byte> Low, Vector512<byte> High) GenerateBucketizedFingerprint(string[][] valueBuckets, int offset)
	{
		Vector128<byte> vector = default(Vector128<byte>);
		Vector128<byte> vector2 = default(Vector128<byte>);
		for (int i = 0; i < valueBuckets.Length; i++)
		{
			int num = 1 << i;
			string[] array = valueBuckets[i];
			for (int j = 0; j < array.Length; j++)
			{
				char num2 = array[j][offset];
				int index = num2 & 0xF;
				int index2 = (int)num2 >> 4;
				vector.SetElementUnsafe(index, (byte)(Vector128.GetElementUnsafe(in vector, index) | num));
				vector2.SetElementUnsafe(index2, (byte)(Vector128.GetElementUnsafe(in vector2, index2) | num));
			}
		}
		return (Low: Vector512.Create(vector), High: Vector512.Create(vector2));
	}

	public static string[][] Bucketize(ReadOnlySpan<string> values, int bucketCount, int n)
	{
		Span<int> span = stackalloc int[80].Slice(0, values.Length);
		Dictionary<int, int> dictionary = new Dictionary<int, int>(bucketCount);
		int num = 0;
		for (int i = 0; i < values.Length; i++)
		{
			string text = values[i];
			int num2 = 0;
			for (int j = 0; j < n; j++)
			{
				num2 = (num2 << 8) | text[j];
			}
			if (!dictionary.TryGetValue(num2, out var value))
			{
				value = num++ % bucketCount;
				dictionary.Add(num2, value);
			}
			span[i] = value;
		}
		string[][] array = new string[bucketCount][];
		for (int k = 0; k < array.Length; k++)
		{
			string[] array2 = (array[k] = new string[((ReadOnlySpan<int>)span).Count(k)]);
			int num3 = 0;
			for (int l = 0; l < span.Length; l++)
			{
				if (span[l] == k)
				{
					array2[num3++] = values[l];
				}
			}
		}
		return array;
	}
}
