using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SGSCore;

public class Keyframe
{
	[CompilerGenerated]
	private Matrix _003CTransform_003Ek__BackingField;

	[ContentSerializer]
	public int Bone { get; private set; }

	[ContentSerializer]
	public TimeSpan Time { get; private set; }

	[ContentSerializer]
	public Matrix Transform
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CTransform_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CTransform_003Ek__BackingField = value;
		}
	}

	public Keyframe(int bone, TimeSpan time, Matrix transform)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Bone = bone;
		Time = time;
		Transform = transform;
	}

	private Keyframe()
	{
	}
}
