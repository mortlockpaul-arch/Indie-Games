using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarSkeletonScaling
{
	public enum Joint_e
	{
		BASE = 0,
		HEAD = 19,
		LF_SC_E = 32,
		RT_SC_E = 35,
		LF_SC_S = 26,
		RT_SC_S = 29,
		LF_SC_S_SKIN = 27,
		RT_SC_S_SKIN = 30,
		LF_SC_H = 7,
		LF_SC_K = 13,
		RT_SC_H = 9,
		RT_SC_K = 17,
		SC_NECK = 24,
		SC_BACKB = 18,
		SC_BACKA = 10,
		SC_BASE = 4
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
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 0.7f, 0.7f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(0.7f, 1f, 0.7f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(0.7f, 1f, 0.7f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(0.9f, 1f, 0.8f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(0.6f, 1f, 0.6f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(0.8f, 1f, 0.9f);
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
		reference = (reference2 = (reference3 = (reference4 = (reference5 = (reference6 = new Vector3(1f, 0.7f, 0.7f))))));
		ref Vector3 reference7 = ref Scale[7];
		ref Vector3 reference8 = ref Scale[13];
		ref Vector3 reference9 = ref Scale[9];
		ref Vector3 reference10 = ref Scale[17];
		reference7 = (reference8 = (reference9 = (reference10 = new Vector3(0.7f, 1f, 0.7f))));
		ref Vector3 reference11 = ref Scale[24];
		reference11 = new Vector3(0.7f, 1f, 0.7f);
		ref Vector3 reference12 = ref Scale[18];
		reference12 = new Vector3(0.8f, 1f, 0.7f);
		ref Vector3 reference13 = ref Scale[10];
		reference13 = new Vector3(0.7f, 1f, 0.5f);
		ref Vector3 reference14 = ref Scale[4];
		reference14 = new Vector3(0.65f, 1f, 0.7f);
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
}
