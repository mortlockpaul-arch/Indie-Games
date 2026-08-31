using System.IO;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides data for calculating light occlusion (shadowing) with composite lighting.
/// </summary>
public class LightOcclusionBuffer
{
	private float HCB;

	private Vector3 HC_0002;

	private Matrix HC_0012;

	private ushort[,] HCH;

	internal LightOcclusionBuffer(Vector3 P_0, float P_1, Matrix P_2, ushort[,] P_3)
	{
		HCB = P_1;
		HC_0002 = P_0;
		HC_0012 = P_2;
		HCH = P_3;
	}

	internal LightOcclusionBuffer()
	{
	}

	/// <summary>
	/// Calculates the occlusion amount (shadowing) for the provided world position.
	/// </summary>
	/// <param name="worldpos"></param>
	/// <returns></returns>
	public float GetOcclusionAmount(Vector3 worldpos)
	{
		Vector3.Distance(ref worldpos, ref HC_0002, out var result);
		Vector4.Transform(ref worldpos, ref HC_0012, out var result2);
		ushort num = (ushort)(65535f * MathHelper.Clamp(result / HCB, 0f, 1f));
		int length = HCH.GetLength(0);
		int length2 = HCH.GetLength(1);
		float num2 = result2.X / result2.W;
		float num3 = result2.Y / result2.W;
		num2 = num2 * 0.5f + 0.5f;
		num3 = 1f - (num3 * 0.5f + 0.5f);
		num2 = MathHelper.Clamp(num2, 0f, 1f);
		num3 = MathHelper.Clamp(num3, 0f, 1f);
		num2 *= (float)(length - 1);
		num3 *= (float)(length2 - 1);
		int num4 = (int)num2;
		int num5 = (int)num3;
		int num6 = (num4 + 1) % length;
		int num7 = (num5 + 1) % length2;
		float amount = num2 - (float)num4;
		float amount2 = num3 - (float)num5;
		float value = 0f;
		float value2 = 0f;
		float value3 = 0f;
		float value4 = 0f;
		if (HCH[num4, num5] > num)
		{
			value = 1f;
		}
		if (HCH[num6, num5] > num)
		{
			value3 = 1f;
		}
		if (HCH[num4, num7] > num)
		{
			value2 = 1f;
		}
		if (HCH[num6, num7] > num)
		{
			value4 = 1f;
		}
		float value5 = MathHelper.Lerp(value, value3, amount);
		float value6 = MathHelper.Lerp(value2, value4, amount);
		return MathHelper.Lerp(value5, value6, amount2);
	}

	internal void HL(Stream P_0)
	{
		try
		{
			using BinaryWriter binaryWriter = new BinaryWriter(P_0);
			binaryWriter.Write(HCB);
			binaryWriter.Write(HC_0002.X);
			binaryWriter.Write(HC_0002.Y);
			binaryWriter.Write(HC_0002.Z);
			binaryWriter.Write(HC_0012.M11);
			binaryWriter.Write(HC_0012.M12);
			binaryWriter.Write(HC_0012.M13);
			binaryWriter.Write(HC_0012.M14);
			binaryWriter.Write(HC_0012.M21);
			binaryWriter.Write(HC_0012.M22);
			binaryWriter.Write(HC_0012.M23);
			binaryWriter.Write(HC_0012.M24);
			binaryWriter.Write(HC_0012.M31);
			binaryWriter.Write(HC_0012.M32);
			binaryWriter.Write(HC_0012.M33);
			binaryWriter.Write(HC_0012.M34);
			binaryWriter.Write(HC_0012.M41);
			binaryWriter.Write(HC_0012.M42);
			binaryWriter.Write(HC_0012.M43);
			binaryWriter.Write(HC_0012.M44);
			int length = HCH.GetLength(0);
			int length2 = HCH.GetLength(1);
			binaryWriter.Write(length);
			binaryWriter.Write(length2);
			for (int i = 0; i < length2; i++)
			{
				for (int j = 0; j < length; j++)
				{
					binaryWriter.Write(HCH[j, i]);
				}
			}
		}
		catch
		{
		}
	}

	internal void _0002A(BinaryReader P_0)
	{
		try
		{
			HCB = P_0.ReadSingle();
			HC_0002.X = P_0.ReadSingle();
			HC_0002.Y = P_0.ReadSingle();
			HC_0002.Z = P_0.ReadSingle();
			HC_0012.M11 = P_0.ReadSingle();
			HC_0012.M12 = P_0.ReadSingle();
			HC_0012.M13 = P_0.ReadSingle();
			HC_0012.M14 = P_0.ReadSingle();
			HC_0012.M21 = P_0.ReadSingle();
			HC_0012.M22 = P_0.ReadSingle();
			HC_0012.M23 = P_0.ReadSingle();
			HC_0012.M24 = P_0.ReadSingle();
			HC_0012.M31 = P_0.ReadSingle();
			HC_0012.M32 = P_0.ReadSingle();
			HC_0012.M33 = P_0.ReadSingle();
			HC_0012.M34 = P_0.ReadSingle();
			HC_0012.M41 = P_0.ReadSingle();
			HC_0012.M42 = P_0.ReadSingle();
			HC_0012.M43 = P_0.ReadSingle();
			HC_0012.M44 = P_0.ReadSingle();
			int num = P_0.ReadInt32();
			int num2 = P_0.ReadInt32();
			HCH = new ushort[num, num2];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					HCH[j, i] = P_0.ReadUInt16();
				}
			}
		}
		catch
		{
		}
	}
}
