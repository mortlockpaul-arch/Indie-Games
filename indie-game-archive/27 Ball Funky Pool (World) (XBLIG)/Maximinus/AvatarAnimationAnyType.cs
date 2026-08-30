using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Maximinus;

public class AvatarAnimationAnyType
{
	public enum ID
	{
		Preset,
		Custom
	}

	private ID id;

	private AvatarAnimation preset;

	private CustomAvatarAnimationPlayer custom;

	private static AvatarExpression defaultExpression = default(AvatarExpression);

	public ReadOnlyCollection<Matrix> BoneTransforms => id switch
	{
		ID.Preset => preset.BoneTransforms, 
		ID.Custom => new ReadOnlyCollection<Matrix>(custom.BoneTransforms), 
		_ => throw new Exception("not supported " + id), 
	};

	public AvatarExpression Expression => id switch
	{
		ID.Preset => preset.Expression, 
		ID.Custom => defaultExpression, 
		_ => throw new Exception("not supported " + id), 
	};

	public TimeSpan CurrentPosition
	{
		get
		{
			return id switch
			{
				ID.Preset => preset.CurrentPosition, 
				ID.Custom => custom.CurrentPosition, 
				_ => throw new Exception("not supported " + id), 
			};
		}
		set
		{
			switch (id)
			{
			case ID.Preset:
				preset.CurrentPosition = value;
				break;
			case ID.Custom:
				custom.CurrentPosition = value;
				break;
			default:
				throw new Exception("not supported " + id);
			}
		}
	}

	public TimeSpan Length => id switch
	{
		ID.Preset => preset.Length, 
		ID.Custom => custom.Length, 
		_ => throw new Exception("not supported " + id), 
	};

	public AvatarAnimationAnyType(AvatarAnimationPreset a)
	{
		id = ID.Preset;
		preset = new AvatarAnimation(a);
	}

	public AvatarAnimationAnyType(CustomAvatarAnimationPlayer a)
	{
		id = ID.Custom;
		custom = a;
	}

	public void Update(TimeSpan time, bool loop)
	{
		switch (id)
		{
		case ID.Preset:
			preset.Update(time, loop);
			break;
		case ID.Custom:
			custom.Update(time, loop);
			break;
		}
	}

	public static List<int> FindInfluencedBones(AvatarBone avatarBone, ReadOnlyCollection<int> parentBones)
	{
		List<int> list = new List<int>();
		list.Add((int)avatarBone);
		for (int i = list[0] + 1; i < parentBones.Count; i++)
		{
			if (list.Contains(parentBones[i]))
			{
				list.Add(i);
			}
		}
		return list;
	}
}
