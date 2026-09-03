using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BezierHelper
{
	public Matrix BezierPos;

	public Matrix BezierVel;

	public Vector4[] pos;

	public float scale;

	public BezierHelper()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		BezierPos = default(Matrix);
		BezierVel = default(Matrix);
	}

	public BezierHelper(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(P0, P1, P2, P3, swapYZ: true);
	}

	public BezierHelper(Vector4 P0, Vector4 P1, Vector4 P2, Vector4 P3)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(new Vector3(P0.X, P0.Y, P0.Z), new Vector3(P1.X, P1.Y, P1.Z), new Vector3(P2.X, P2.Y, P2.Z), new Vector3(P3.X, P3.Y, P3.Z), swapYZ: true);
	}

	public BezierHelper(Matrix[] transformMatrix, Dictionary<ModelBone, int> boneMap, ModelBoneCollection bones, Matrix transformation, string parentBone)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(transformMatrix, boneMap, bones, transformation, parentBone, 1f);
	}

	public BezierHelper(Matrix[] transformMatrix, Dictionary<ModelBone, int> boneMap, ModelBoneCollection bones, Matrix transformation, string parentBone, float scale)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		Vector3 val = Vector3.Transform(Vector3.Zero, transformMatrix[boneMap[bones[parentBone]]] * transformation);
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = Vector3.Transform(Vector3.Zero, transformMatrix[boneMap[bones[parentBone]]] * transformation);
			ref Vector3 reference2 = ref array[i];
			reference2 -= val;
			ref Vector3 reference3 = ref array[i];
			reference3 *= scale;
			ref Vector3 reference4 = ref array[i];
			reference4 += val;
			if (i < 3)
			{
				parentBone = ((ReadOnlyCollection<ModelBone>)(object)bones[parentBone].Children)[0].Name;
			}
		}
		BuildHelper(array[0], array[1], array[2], array[3], swapYZ: true);
	}

	public BezierHelper(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, bool swapYZ)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		BuildHelper(P0, P1, P2, P3, swapYZ);
	}

	public void BuildHelper(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, bool swapYZ)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		pos = (Vector4[])(object)new Vector4[4];
		ref Vector4 reference = ref pos[0];
		reference = new Vector4(P0, 1f);
		ref Vector4 reference2 = ref pos[1];
		reference2 = new Vector4(P1, 1f);
		ref Vector4 reference3 = ref pos[2];
		reference3 = new Vector4(P2, 1f);
		ref Vector4 reference4 = ref pos[3];
		reference4 = new Vector4(P3, 1f);
		ref Vector3 reference5 = ref array[3];
		reference5 = -P0 + 3f * P1 - 3f * P2 + P3;
		ref Vector3 reference6 = ref array[2];
		reference6 = 3f * P0 - 6f * P1 + 3f * P2;
		ref Vector3 reference7 = ref array[1];
		reference7 = -3f * P0 + 3f * P1;
		array[0] = P0;
		BezierPos.M11 = array[3].X;
		BezierPos.M21 = (swapYZ ? array[3].Z : array[3].Y);
		BezierPos.M31 = (swapYZ ? array[3].Y : array[3].Z);
		BezierPos.M12 = array[2].X;
		BezierPos.M22 = (swapYZ ? array[2].Z : array[2].Y);
		BezierPos.M32 = (swapYZ ? array[2].Y : array[2].Z);
		BezierPos.M13 = array[1].X;
		BezierPos.M23 = (swapYZ ? array[1].Z : array[1].Y);
		BezierPos.M33 = (swapYZ ? array[1].Y : array[1].Z);
		BezierPos.M14 = array[0].X;
		BezierPos.M24 = (swapYZ ? array[0].Z : array[0].Y);
		BezierPos.M34 = (swapYZ ? array[0].Y : array[0].Z);
		BezierPos.M44 = 1f;
		BezierPos = Matrix.Transpose(BezierPos);
		ref Vector3 reference8 = ref array[0];
		reference8 = array[1];
		ref Vector3 reference9 = ref array[1];
		reference9 = array[2] * 2f;
		ref Vector3 reference10 = ref array[2];
		reference10 = array[3] * 3f;
		BezierVel.M11 = 0f;
		BezierVel.M21 = 0f;
		BezierVel.M31 = 0f;
		BezierVel.M12 = array[2].X;
		BezierVel.M22 = (swapYZ ? array[2].Z : array[2].Y);
		BezierVel.M32 = (swapYZ ? array[2].Y : array[2].Z);
		BezierVel.M13 = array[1].X;
		BezierVel.M23 = (swapYZ ? array[1].Z : array[1].Y);
		BezierVel.M33 = (swapYZ ? array[1].Y : array[1].Z);
		BezierVel.M14 = array[0].X;
		BezierVel.M24 = (swapYZ ? array[0].Z : array[0].Y);
		BezierVel.M34 = (swapYZ ? array[0].Y : array[0].Z);
		BezierVel.M44 = 1f;
		BezierVel = Matrix.Transpose(BezierVel);
		Vector4 val = pos[3] - pos[0];
		scale = ((Vector4)(ref val)).Length() / 4f;
	}

	public static Vector4 GetBezierCoords(float t)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector4(t * t * t, t * t, t, 1f);
	}
}
