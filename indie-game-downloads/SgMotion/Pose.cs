using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using SgMotion.Controllers;

namespace SgMotion;

public struct Pose : IEquatable<Pose>
{
	public Vector3 Translation;

	public Quaternion Orientation;

	public Vector3 Scale;

	public static readonly Pose Identity;

	static Pose()
	{
		Identity.Orientation = Quaternion.Identity;
		Identity.Translation = Vector3.Zero;
		Identity.Scale = Vector3.One;
	}

	public static Pose Interpolate(Pose pose1, Pose pose2, float amount, InterpolationMode translationInterpolation, InterpolationMode orientationInterpolation, InterpolationMode scaleInterpolation)
	{
		if (amount < 0f || amount > 1f)
		{
			throw new ArgumentException("Amount must be between 0.0 and 1.0 inclusive.");
		}
		Pose result = default(Pose);
		switch (translationInterpolation)
		{
		case InterpolationMode.None:
			result.Translation = pose1.Translation;
			break;
		case InterpolationMode.Linear:
			Vector3.Lerp(ref pose1.Translation, ref pose2.Translation, amount, out result.Translation);
			break;
		case InterpolationMode.Cubic:
			Vector3.SmoothStep(ref pose1.Translation, ref pose2.Translation, amount, out result.Translation);
			break;
		default:
			throw new ArgumentException("Translation interpolation method not supported");
		}
		switch (orientationInterpolation)
		{
		case InterpolationMode.None:
			result.Orientation = pose1.Orientation;
			break;
		case InterpolationMode.Linear:
			Quaternion.Lerp(ref pose1.Orientation, ref pose2.Orientation, amount, out result.Orientation);
			break;
		case InterpolationMode.Spherical:
			Quaternion.Slerp(ref pose1.Orientation, ref pose2.Orientation, amount, out result.Orientation);
			break;
		default:
			throw new ArgumentException("Orientation interpolation method not supported");
		}
		switch (scaleInterpolation)
		{
		case InterpolationMode.None:
			result.Scale = pose1.Scale;
			break;
		case InterpolationMode.Linear:
			Vector3.Lerp(ref pose1.Scale, ref pose2.Scale, amount, out result.Scale);
			break;
		case InterpolationMode.Cubic:
			Vector3.SmoothStep(ref pose1.Scale, ref pose2.Scale, amount, out result.Scale);
			break;
		default:
			throw new ArgumentException("Scale interpolation method not supported");
		}
		return result;
	}

	public override int GetHashCode()
	{
		return Translation.GetHashCode() + Orientation.GetHashCode() + Scale.GetHashCode();
	}

	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{{Translation:{0}\n Orientation:{1}\n Scale:{2}\n}}", Translation.ToString(), Orientation.ToString(), Scale.ToString());
	}

	public bool Equals(Pose other)
	{
		if (Translation == other.Translation && Orientation == other.Orientation)
		{
			return Scale == other.Scale;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is Pose)
		{
			result = Equals((Pose)obj);
		}
		return result;
	}

	public static bool operator ==(Pose pose1, Pose pose2)
	{
		if (pose1.Translation == pose2.Translation && pose1.Orientation == pose2.Orientation)
		{
			return pose1.Scale == pose2.Scale;
		}
		return false;
	}

	public static bool operator !=(Pose pose1, Pose pose2)
	{
		if (!(pose1.Translation != pose2.Translation) && !(pose1.Orientation != pose2.Orientation))
		{
			return pose1.Scale != pose2.Scale;
		}
		return true;
	}
}
