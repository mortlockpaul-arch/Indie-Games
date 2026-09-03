using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public sealed class Util
{
	public const long TICKS_PER_60FPS = 166666L;

	private static Quaternion qStart;

	private static Quaternion qEnd;

	private static Quaternion qResult;

	private static Vector3 curTrans;

	private static Vector3 nextTrans;

	private static Vector3 lerpedTrans;

	private static Vector3 curScale;

	private static Vector3 nextScale;

	private static Vector3 lerpedScale;

	private static Matrix startRotation;

	private static Matrix endRotation;

	private static Matrix returnMatrix;

	public static SkinningType GetSkinningType(VertexElement[] elements)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < elements.Length; i++)
		{
			VertexElement val = elements[i];
			if ((int)((VertexElement)(ref val)).VertexElementUsage == 2)
			{
				num++;
			}
			else if ((int)((VertexElement)(ref val)).VertexElementUsage == 1)
			{
				num2++;
			}
		}
		if (num == 3 || num2 == 3)
		{
			return SkinningType.TwelveBonesPerVertex;
		}
		if (num == 2 || num2 == 2)
		{
			return SkinningType.EightBonesPerVertex;
		}
		if (num == 1 || num2 == 1)
		{
			return SkinningType.FourBonesPerVertex;
		}
		return SkinningType.None;
	}

	public static void ReflectMatrix(ref Matrix m)
	{
		m.M13 *= -1f;
		m.M23 *= -1f;
		m.M33 *= -1f;
		m.M43 *= -1f;
		m.M31 *= -1f;
		m.M32 *= -1f;
		m.M33 *= -1f;
		m.M34 *= -1f;
	}

	private static T Max<T>(params T[] items) where T : IComparable
	{
		IComparable comparable = null;
		foreach (IComparable comparable2 in items)
		{
			if (comparable == null)
			{
				comparable = comparable2;
			}
			else if (comparable2.CompareTo(comparable) > 0)
			{
				comparable = comparable2;
			}
		}
		return (T)comparable;
	}

	public static T[] Convert<T>(byte[] data, int vertexSize, GraphicsDevice device) where T : struct
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		T[] array = new T[data.Length / vertexSize];
		VertexBuffer val = new VertexBuffer(device, data.Length, (BufferUsage)0);
		try
		{
			val.SetData<byte>(data);
			val.GetData<T>(array);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return array;
	}

	public static Matrix SlerpMatrix(Matrix start, Matrix end, float slerpAmount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		if (start == end)
		{
			return start;
		}
		Quaternion.CreateFromRotationMatrix(ref start, ref qStart);
		Quaternion.CreateFromRotationMatrix(ref end, ref qEnd);
		Quaternion.Lerp(ref qStart, ref qEnd, slerpAmount, ref qResult);
		curTrans.X = start.M41;
		curTrans.Y = start.M42;
		curTrans.Z = start.M43;
		nextTrans.X = end.M41;
		nextTrans.Y = end.M42;
		nextTrans.Z = end.M43;
		Vector3.Lerp(ref curTrans, ref nextTrans, slerpAmount, ref lerpedTrans);
		Matrix.CreateFromQuaternion(ref qStart, ref startRotation);
		Matrix.CreateFromQuaternion(ref qEnd, ref endRotation);
		curScale.X = start.M11 - startRotation.M11;
		curScale.Y = start.M22 - startRotation.M22;
		curScale.Z = start.M33 - startRotation.M33;
		nextScale.X = end.M11 - endRotation.M11;
		nextScale.Y = end.M22 - endRotation.M22;
		nextScale.Z = end.M33 - endRotation.M33;
		Vector3.Lerp(ref curScale, ref nextScale, slerpAmount, ref lerpedScale);
		Matrix.CreateFromQuaternion(ref qResult, ref returnMatrix);
		returnMatrix.M41 = lerpedTrans.X;
		returnMatrix.M42 = lerpedTrans.Y;
		returnMatrix.M43 = lerpedTrans.Z;
		returnMatrix.M11 += lerpedScale.X;
		returnMatrix.M22 += lerpedScale.Y;
		returnMatrix.M33 += lerpedScale.Z;
		return returnMatrix;
	}

	public static void SlerpMatrix(ref Matrix start, ref Matrix end, float slerpAmount, out Matrix result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (start == end)
		{
			result = start;
			return;
		}
		Quaternion.CreateFromRotationMatrix(ref start, ref qStart);
		Quaternion.CreateFromRotationMatrix(ref end, ref qEnd);
		Quaternion.Lerp(ref qStart, ref qEnd, slerpAmount, ref qResult);
		curTrans.X = start.M41;
		curTrans.Y = start.M42;
		curTrans.Z = start.M43;
		nextTrans.X = end.M41;
		nextTrans.Y = end.M42;
		nextTrans.Z = end.M43;
		Vector3.Lerp(ref curTrans, ref nextTrans, slerpAmount, ref lerpedTrans);
		Matrix.CreateFromQuaternion(ref qStart, ref startRotation);
		Matrix.CreateFromQuaternion(ref qEnd, ref endRotation);
		curScale.X = start.M11 - startRotation.M11;
		curScale.Y = start.M22 - startRotation.M22;
		curScale.Z = start.M33 - startRotation.M33;
		nextScale.X = end.M11 - endRotation.M11;
		nextScale.Y = end.M22 - endRotation.M22;
		nextScale.Z = end.M33 - endRotation.M33;
		Vector3.Lerp(ref curScale, ref nextScale, slerpAmount, ref lerpedScale);
		Matrix.CreateFromQuaternion(ref qResult, ref result);
		result.M41 = lerpedTrans.X;
		result.M42 = lerpedTrans.Y;
		result.M43 = lerpedTrans.Z;
		result.M11 += lerpedScale.X;
		result.M22 += lerpedScale.Y;
		result.M33 += lerpedScale.Z;
	}

	public static bool IsSkinned(ModelMeshPart meshPart)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		VertexElement[] vertexElements = meshPart.VertexDeclaration.GetVertexElements();
		VertexElement[] array = vertexElements;
		for (int i = 0; i < array.Length; i++)
		{
			VertexElement val = array[i];
			if ((int)((VertexElement)(ref val)).VertexElementUsage == 2 && ((VertexElement)(ref val)).UsageIndex == 0)
			{
				return true;
			}
		}
		return false;
	}

	public unsafe static bool IsSkinned(ModelMesh mesh)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = mesh.MeshParts.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMeshPart current = ((Enumerator)(ref enumerator)).Current;
				if (IsSkinned(current))
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}

	public unsafe static bool IsSkinned(Model model)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				if (IsSkinned(current))
				{
					return true;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		return false;
	}
}
