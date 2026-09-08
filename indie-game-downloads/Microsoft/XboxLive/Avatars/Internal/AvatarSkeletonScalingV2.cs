using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.Avatars.Internal.Version1;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarSkeletonScalingV2
{
	public enum Joint_e
	{
		BASE,
		BACKA,
		LF_H,
		RT_H,
		SC_BASE,
		BACKB,
		LF_K,
		LF_SC_H,
		RT_K,
		RT_SC_H,
		SC_BACKA,
		LF_A,
		LF_C,
		LF_SC_K,
		NECK,
		RT_A,
		RT_C,
		RT_SC_K,
		SC_BACKB,
		HEAD,
		LF_S,
		LF_T,
		RT_S,
		RT_T,
		SC_NECK,
		LF_E,
		LF_SC_S,
		LF_SC_TWIST_S,
		RT_E,
		RT_SC_S,
		RT_SC_TWIST_S,
		LF_E_TWIST,
		LF_SC_E,
		LF_W,
		RT_E_TWIST,
		RT_SC_E,
		RT_W,
		LF_FINGA,
		LF_FINGB,
		LF_FINGC,
		LF_FINGD,
		LF_PROP,
		LF_SPECIAL,
		LF_THUMB,
		RT_FINGA,
		RT_FINGB,
		RT_FINGC,
		RT_FINGD,
		RT_PROP,
		RT_SPECIAL,
		RT_THUMB,
		LF_FINGA1,
		LF_FINGB1,
		LF_FINGC1,
		LF_FINGD1,
		LF_THUMB1,
		RT_FINGA1,
		RT_FINGB1,
		RT_FINGC1,
		RT_FINGD1,
		RT_THUMB1,
		LF_FINGA2,
		LF_FINGB2,
		LF_FINGC2,
		LF_FINGD2,
		LF_THUMB2,
		RT_FINGA2,
		RT_FINGB2,
		RT_FINGC2,
		RT_FINGD2,
		RT_THUMB2
	}

	public static void ApplyTo(AvatarGender bodyType, float weightFactor, float heightFactor, Skeleton skeleton)
	{
		Vector3[] Scale;
		if (heightFactor >= 0f)
		{
			InitTallScalingWeights(out Scale);
			ApplyTo(skeleton, heightFactor, Scale);
		}
		else
		{
			InitShortScalingWeights(out Scale);
			ApplyTo(skeleton, 0f - heightFactor, Scale);
		}
		switch (bodyType)
		{
		case AvatarGender.Male:
			if (weightFactor >= 0f)
			{
				InitFatMaleScalingWeights(out Scale);
				ApplyTo(skeleton, weightFactor, Scale);
			}
			else
			{
				InitThinMaleScalingWeights(out Scale);
				ApplyTo(skeleton, 0f - weightFactor, Scale);
			}
			break;
		case AvatarGender.Female:
			if (weightFactor > 0f)
			{
				InitFatFemaleScalingWeights(out Scale);
				ApplyTo(skeleton, weightFactor, Scale);
			}
			else
			{
				InitThinFemaleScalingWeights(out Scale);
				ApplyTo(skeleton, 0f - weightFactor, Scale);
			}
			break;
		}
	}

	public static Vector3 VectorLerp(Vector3 V0, Vector3 V1, float t)
	{
		return new Vector3
		{
			X = V0.X + t * (V1.X - V0.X),
			Y = V0.Y + t * (V1.Y - V0.Y),
			Z = V0.Z + t * (V1.Z - V0.Z)
		};
	}

	public static void ApplyTo(Skeleton skeleton, float blendValue, Vector3[] scaleTemplate)
	{
		blendValue = ((blendValue < 0f) ? 0f : ((blendValue > 1f) ? 1f : blendValue));
		Vector3 v = new Vector3(1f, 1f, 1f);
		int num = skeleton.Joints.Length;
		Vector3 vector;
		while (--num >= 0)
		{
			vector = VectorLerp(v, scaleTemplate[num], blendValue);
			skeleton.Joints[num].Local.scale.X *= vector.X;
			skeleton.Joints[num].Local.scale.Y *= vector.Y;
			skeleton.Joints[num].Local.scale.Z *= vector.Z;
		}
		vector = VectorLerp(v, scaleTemplate[0], blendValue);
		skeleton.Joints[0].Local.position.X *= vector.X;
		skeleton.Joints[0].Local.position.Y *= vector.Y;
		skeleton.Joints[0].Local.position.Z *= vector.Z;
	}

	public static void InitIdentityScales(out Vector3[] scales)
	{
		Vector3 vector = new Vector3(1f, 1f, 1f);
		scales = new Vector3[72];
		int num = 72;
		while (--num >= 0)
		{
			scales[num] = vector;
		}
	}

	public static void InitFatMaleScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[32];
		ref Vector3 reference2 = ref Scale[35];
		ref Vector3 reference3 = ref Scale[26];
		ref Vector3 reference4 = ref Scale[29];
		ref Vector3 reference5 = ref Scale[27];
		ref Vector3 reference6 = ref Scale[30];
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 1.5f, 1.5f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(1.5f, 1f, 1.5f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(1.9f, 1f, 1.5f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(1.5f, 1f, 1.4f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(1.8f, 1f, 1.9f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(1.6f, 1.6f, 2.2f);
		Vector3 v = new Vector3(1f, 1f, 1f);
		int num = 72;
		while (--num >= 0)
		{
			ref Vector3 reference15 = ref Scale[num];
			reference15 = VectorLerp(v, Scale[num], 0.6f);
		}
	}

	public static void InitFatFemaleScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[32];
		ref Vector3 reference2 = ref Scale[35];
		ref Vector3 reference3 = ref Scale[26];
		ref Vector3 reference4 = ref Scale[29];
		ref Vector3 reference5 = ref Scale[27];
		ref Vector3 reference6 = ref Scale[30];
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 1.6f, 1.6f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(1.6f, 1f, 1.6f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(2f, 1f, 1.6f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(1.6f, 1f, 1.6f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(1.6f, 1f, 2f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(1.5f, 1.5f, 2f);
		Vector3 v = new Vector3(1f, 1f, 1f);
		int num = 72;
		while (--num >= 0)
		{
			ref Vector3 reference15 = ref Scale[num];
			reference15 = VectorLerp(v, Scale[num], 0.6f);
		}
	}

	public static void InitThinMaleScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[32];
		ref Vector3 reference2 = ref Scale[35];
		ref Vector3 reference3 = ref Scale[26];
		ref Vector3 reference4 = ref Scale[29];
		ref Vector3 reference5 = ref Scale[27];
		ref Vector3 reference6 = ref Scale[30];
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 0.76f, 0.76f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(0.76f, 1f, 0.76f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(0.76f, 1f, 0.76f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(0.92f, 1f, 0.84f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(0.68f, 1f, 0.68f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(0.84f, 1f, 0.92f);
	}

	public static void InitThinFemaleScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[32];
		ref Vector3 reference2 = ref Scale[35];
		ref Vector3 reference3 = ref Scale[26];
		ref Vector3 reference4 = ref Scale[29];
		ref Vector3 reference5 = ref Scale[27];
		ref Vector3 reference6 = ref Scale[30];
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 0.82f, 0.82f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(0.82f, 1f, 0.82f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(0.82f, 1f, 0.82f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(0.88f, 1f, 0.82f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(0.82f, 1f, 0.7f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(0.79f, 1f, 0.82f);
	}

	public static void InitTallScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[0];
		reference = new Vector3(1.1f, 1.1f, 1.1f);
		ref Vector3 reference2 = ref Scale[19];
		reference2 = new Vector3(0.9f, 0.9f, 0.9f);
	}

	public static void InitShortScalingWeights(out Vector3[] Scale)
	{
		InitIdentityScales(out Scale);
		ref Vector3 reference = ref Scale[0];
		reference = new Vector3(0.9f, 0.9f, 0.9f);
		ref Vector3 reference2 = ref Scale[19];
		reference2 = new Vector3(1.05f, 1.05f, 1.05f);
	}

	public static void RescaleV1toV2(ref Skeleton skeleton)
	{
		float num = 0.85f;
		float num2 = 0.85f;
		float num3 = 0.85f;
		float num4 = 0.024031019f;
		float num5 = 1.2924f;
		float num6 = 0.8351f;
		float num7 = 1.1524f;
		float num8 = 0.7606f;
		float num9 = 0.95f;
		float num10 = 0.95f;
		float num11 = 1f;
		float num12 = 0.95f;
		float num13 = 1.12f;
		num5 *= num13;
		num6 *= num13;
		num7 *= num13;
		num8 *= num13;
		float num14 = 1.1f;
		if (num6 < num14)
		{
			float num15 = 16.96154f * num5 + 15.419582f * num6;
			num6 = num14;
			num5 = (num15 - 15.419582f * num6) / 16.96154f;
		}
		float num16 = 26.87384f * num7 + 25.968426f * num8;
		num7 = num16 / 52.842266f;
		num8 = num7;
		float num17 = (num8 - 1f) * 0.75f + 1f;
		float num18 = num8 / num17;
		skeleton.Joints[1].Local.position.Y += num4;
		skeleton.Joints[5].Local.position.Y += num4;
		skeleton.Joints[20].Local.scale = (skeleton.Joints[22].Local.scale = new Vector3(num5, num9, num9));
		skeleton.Joints[25].Local.scale = (skeleton.Joints[28].Local.scale = new Vector3(num6 / num5, num10 / num9, num10 / num9));
		skeleton.Joints[25].Local.position.Y *= num5 / num9;
		skeleton.Joints[28].Local.position.Y *= num5 / num9;
		skeleton.Joints[25].Local.position.Z *= num5 / num9;
		skeleton.Joints[28].Local.position.Z *= num5 / num9;
		Joint_e[] array = new Joint_e[4]
		{
			Joint_e.LF_W,
			Joint_e.RT_W,
			Joint_e.LF_E_TWIST,
			Joint_e.RT_E_TWIST
		};
		Joint_e[] array2 = array;
		Joint_e[] array3 = array2;
		foreach (Joint_e joint_e in array3)
		{
			skeleton.Joints[(int)joint_e].Local.position.Y *= num6 / num10;
			skeleton.Joints[(int)joint_e].Local.position.Z *= num6 / num10;
		}
		skeleton.Joints[2].Local.scale = (skeleton.Joints[3].Local.scale = new Vector3(num11, num7, num11));
		Joint_e[] array4 = new Joint_e[2]
		{
			Joint_e.LF_K,
			Joint_e.RT_K
		};
		array2 = array4;
		Joint_e[] array5 = array2;
		foreach (Joint_e joint_e2 in array5)
		{
			skeleton.Joints[(int)joint_e2].Local.scale = new Vector3(num12 / num11, num17 / num7, num12 / num11);
			skeleton.Joints[(int)joint_e2].Local.position.X *= num7 / num11;
			skeleton.Joints[(int)joint_e2].Local.position.Z *= num7 / num11;
		}
		Joint_e[] array6 = new Joint_e[2]
		{
			Joint_e.LF_A,
			Joint_e.RT_A
		};
		array2 = array6;
		Joint_e[] array7 = array2;
		foreach (Joint_e joint_e3 in array7)
		{
			skeleton.Joints[(int)joint_e3].Local.scale = new Vector3(num3 / num12, num3 / num17, num3 / num12);
			skeleton.Joints[(int)joint_e3].Local.position.X *= num8 / num12;
			skeleton.Joints[(int)joint_e3].Local.position.Y *= num18;
			skeleton.Joints[(int)joint_e3].Local.position.Z *= num8 / num12;
		}
		skeleton.Joints[14].Local.scale = new Vector3(num, num, num);
		skeleton.Joints[33].Local.scale = new Vector3(num2 / num6, num2 / num10, num2 / num10);
		skeleton.Joints[36].Local.scale = new Vector3(num2 / num6, num2 / num10, num2 / num10);
		skeleton.Joints[31].Local.scale = new Vector3(1f / num6, num2 / num10, num2 / num10);
		skeleton.Joints[34].Local.scale = new Vector3(1f / num6, num2 / num10, num2 / num10);
		skeleton.Joints[27].Local.scale = new Vector3(1f / num5, 1f, 1f);
		skeleton.Joints[30].Local.scale = new Vector3(1f / num5, 1f, 1f);
		skeleton.Joints[27].Local.position.X = skeleton.Joints[25].Local.position.X * 0.02f;
		skeleton.Joints[30].Local.position.X = skeleton.Joints[28].Local.position.X * 0.02f;
		skeleton.Joints[0].Local.position.Y = 0.77692485f;
	}

	public static void RescaleV2toV1(ref Skeleton skeleton)
	{
		float num = 1.1764705f;
		float num2 = 1.1764705f;
		float num3 = 1.1764705f;
		float num4 = 0.024031019f;
		float num5 = 0.77055156f;
		float num6 = 0.9090909f;
		float num7 = 0.9301985f;
		float num8 = 0.9301985f;
		float num9 = 1.0526316f;
		float num10 = 1.0526316f;
		float num11 = 1f;
		float num12 = 1.0526316f;
		float num13 = (num8 - 1f) * 0.75f + 1f;
		float num14 = num8 / num13;
		skeleton.Joints[1].Local.position.Y -= num4;
		skeleton.Joints[5].Local.position.Y -= num4;
		skeleton.Joints[20].Local.scale = (skeleton.Joints[22].Local.scale = new Vector3(num5, num9, num9));
		skeleton.Joints[25].Local.scale = (skeleton.Joints[28].Local.scale = new Vector3(num6 / num5, num10 / num9, num10 / num9));
		skeleton.Joints[25].Local.position.Y *= num5 / num9;
		skeleton.Joints[28].Local.position.Y *= num5 / num9;
		skeleton.Joints[25].Local.position.Z *= num5 / num9;
		skeleton.Joints[28].Local.position.Z *= num5 / num9;
		Joint_e[] array = new Joint_e[4]
		{
			Joint_e.LF_W,
			Joint_e.RT_W,
			Joint_e.LF_E_TWIST,
			Joint_e.RT_E_TWIST
		};
		Joint_e[] array2 = array;
		Joint_e[] array3 = array2;
		foreach (Joint_e joint_e in array3)
		{
			skeleton.Joints[(int)joint_e].Local.position.Y *= num6 / num10;
			skeleton.Joints[(int)joint_e].Local.position.Z *= num6 / num10;
		}
		skeleton.Joints[2].Local.scale = (skeleton.Joints[3].Local.scale = new Vector3(num11, num7, num11));
		Joint_e[] array4 = new Joint_e[2]
		{
			Joint_e.LF_K,
			Joint_e.RT_K
		};
		array2 = array4;
		Joint_e[] array5 = array2;
		foreach (Joint_e joint_e2 in array5)
		{
			skeleton.Joints[(int)joint_e2].Local.scale = new Vector3(num12 / num11, num13 / num7, num12 / num11);
			skeleton.Joints[(int)joint_e2].Local.position.X *= num7 / num11;
			skeleton.Joints[(int)joint_e2].Local.position.Z *= num7 / num11;
		}
		Joint_e[] array6 = new Joint_e[2]
		{
			Joint_e.LF_A,
			Joint_e.RT_A
		};
		array2 = array6;
		Joint_e[] array7 = array2;
		foreach (Joint_e joint_e3 in array7)
		{
			skeleton.Joints[(int)joint_e3].Local.scale = new Vector3(num2 / num12, num2 / num13, num2 / num12);
			skeleton.Joints[(int)joint_e3].Local.position.X *= num8 / num12;
			skeleton.Joints[(int)joint_e3].Local.position.Y *= num14;
			skeleton.Joints[(int)joint_e3].Local.position.Z *= num8 / num12;
		}
		skeleton.Joints[14].Local.scale = new Vector3(num3, num3, num3);
		skeleton.Joints[33].Local.scale = new Vector3(num / num6, num / num10, num / num10);
		skeleton.Joints[36].Local.scale = new Vector3(num / num6, num / num10, num / num10);
		skeleton.Joints[31].Local.scale = (skeleton.Joints[34].Local.scale = new Vector3(1f / num6, num / num10, num / num10));
		skeleton.Joints[27].Local.scale = (skeleton.Joints[30].Local.scale = new Vector3(1f / num5, 1f, 1f));
		skeleton.Joints[27].Local.position.X = skeleton.Joints[25].Local.position.X * 0.02f;
		skeleton.Joints[30].Local.position.X = skeleton.Joints[28].Local.position.X * 0.02f;
		skeleton.Joints[27].Local.position.X -= 0.00440244f;
		skeleton.Joints[30].Local.position.X += 0.00440244f;
		skeleton.Joints[0].Local.position.Y = 0.7551987f;
	}

	public static void GetRescaling(CoordinateSystem coordinateSystem, Skeleton.SkeletonVersion baseVersion, Skeleton.SkeletonVersion targetVersion, out Skeleton skeleton)
	{
		if (baseVersion == Skeleton.SkeletonVersion.Invalid)
		{
			baseVersion = Skeleton.SkeletonVersion.Natal;
		}
		if (targetVersion == Skeleton.SkeletonVersion.Invalid)
		{
			targetVersion = Skeleton.SkeletonVersion.Natal;
		}
		skeleton = EmbeddedSkeleton.GetEmbeddedSkeleton(coordinateSystem, baseVersion);
		if (baseVersion == Skeleton.SkeletonVersion.Nxe && targetVersion == Skeleton.SkeletonVersion.Natal)
		{
			RescaleV1toV2(ref skeleton);
		}
		else if (baseVersion == Skeleton.SkeletonVersion.Natal && targetVersion == Skeleton.SkeletonVersion.Nxe)
		{
			RescaleV2toV1(ref skeleton);
		}
	}
}
