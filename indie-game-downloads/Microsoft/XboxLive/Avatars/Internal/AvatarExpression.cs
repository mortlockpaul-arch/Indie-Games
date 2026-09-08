using System;

namespace Microsoft.XboxLive.Avatars.Internal;

public struct AvatarExpression
{
	public int MouthLayer;

	public int LeftEyebrowLayer;

	public int RightEyebrowLayer;

	public int LeftEyeLayer;

	public int RightEyeLayer;

	public AvatarMouthTextureIndex Mouth
	{
		get
		{
			return (AvatarMouthTextureIndex)MouthLayer;
		}
		set
		{
			if (value > AvatarMouthTextureIndex.PhoneticDTH || value < AvatarMouthTextureIndex.Invalid)
			{
				throw new ArgumentOutOfRangeException("value", "Invalid index");
			}
			MouthLayer = (int)value;
		}
	}

	public AvatarLeftEyeTextureIndex LeftEye
	{
		get
		{
			return (AvatarLeftEyeTextureIndex)LeftEyeLayer;
		}
		set
		{
			if (value > AvatarLeftEyeTextureIndex.Blink || value < AvatarLeftEyeTextureIndex.Invalid)
			{
				throw new ArgumentOutOfRangeException("value", "Invalid index");
			}
			LeftEyeLayer = (int)value;
		}
	}

	public AvatarRightEyeTextureIndex RightEye
	{
		get
		{
			return (AvatarRightEyeTextureIndex)RightEyeLayer;
		}
		set
		{
			if (value > AvatarRightEyeTextureIndex.Blink || value < AvatarRightEyeTextureIndex.Invalid)
			{
				throw new ArgumentOutOfRangeException("value", "Invalid index");
			}
			RightEyeLayer = (int)value;
		}
	}

	public AvatarEyebrowTextureIndex LeftEyebrow
	{
		get
		{
			return (AvatarEyebrowTextureIndex)LeftEyebrowLayer;
		}
		set
		{
			if (value > AvatarEyebrowTextureIndex.Raised || value < AvatarEyebrowTextureIndex.Invalid)
			{
				throw new ArgumentOutOfRangeException("value", "Invalid index");
			}
			LeftEyebrowLayer = (int)value;
		}
	}

	public AvatarEyebrowTextureIndex RightEyebrow
	{
		get
		{
			return (AvatarEyebrowTextureIndex)RightEyebrowLayer;
		}
		set
		{
			if (value > AvatarEyebrowTextureIndex.Raised || value < AvatarEyebrowTextureIndex.Invalid)
			{
				throw new ArgumentOutOfRangeException("value", "Invalid index");
			}
			RightEyebrowLayer = (int)value;
		}
	}
}
