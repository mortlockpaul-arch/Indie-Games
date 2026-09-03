using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public class BonePose
{
	private Matrix defaultMatrix;

	private static Matrix returnMatrix;

	private static Matrix blendMatrix;

	private static Matrix currentMatrixBuffer;

	private int index;

	private string name;

	private BonePose parent;

	private IAnimationController currentAnimation;

	private IAnimationController currentBlendAnimation;

	private float blendFactor;

	private BonePoseCollection children;

	private bool doesAnimContainChannel;

	private bool doesBlendContainChannel;

	public bool enabled;

	public BonePoseCollection Children => children;

	public BonePose Parent => parent;

	public int Index => index;

	public string Name => name;

	public IAnimationController CurrentController
	{
		get
		{
			return currentAnimation;
		}
		set
		{
			if (currentAnimation == value)
			{
				return;
			}
			if (value != null)
			{
				if (currentAnimation != null)
				{
					currentAnimation.AnimationTracksChanged -= current_AnimationTracksChanged;
				}
				if (name != null)
				{
					doesAnimContainChannel = value.ContainsAnimationTrack(this);
					value.AnimationTracksChanged += current_AnimationTracksChanged;
				}
			}
			else
			{
				doesAnimContainChannel = false;
			}
			currentAnimation = value;
		}
	}

	public IAnimationController CurrentBlendController
	{
		get
		{
			return currentBlendAnimation;
		}
		set
		{
			if (currentBlendAnimation == value)
			{
				return;
			}
			if (value != null)
			{
				if (currentBlendAnimation != null)
				{
					currentBlendAnimation.AnimationTracksChanged -= blend_AnimationTracksChanged;
				}
				if (name != null)
				{
					doesBlendContainChannel = value.ContainsAnimationTrack(this);
					value.AnimationTracksChanged += blend_AnimationTracksChanged;
				}
			}
			else
			{
				doesBlendContainChannel = false;
			}
			currentBlendAnimation = value;
		}
	}

	public float BlendFactor
	{
		get
		{
			return blendFactor;
		}
		set
		{
			blendFactor = value;
		}
	}

	public Matrix DefaultTransform
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return defaultMatrix;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			defaultMatrix = value;
		}
	}

	internal unsafe BonePose(ModelBone bone, ModelBoneCollection bones, BonePose[] anims)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		parent = null;
		currentAnimation = null;
		currentBlendAnimation = null;
		blendFactor = 0f;
		doesAnimContainChannel = false;
		doesBlendContainChannel = false;
		base._002Ector();
		index = bone.Index;
		name = bone.Name;
		defaultMatrix = bone.Transform;
		if (bone.Parent != null)
		{
			parent = anims[bone.Parent.Index];
		}
		anims[index] = this;
		List<BonePose> list = new List<BonePose>();
		Enumerator enumerator = bone.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelBone current = ((Enumerator)(ref enumerator)).Current;
				BonePose item = new BonePose(((ReadOnlyCollection<ModelBone>)(object)bones)[current.Index], bones, anims);
				list.Add(item);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		children = new BonePoseCollection(list);
		enabled = true;
	}

	private void FindHierarchy(List<BonePose> poses)
	{
		poses.Add(this);
		foreach (BonePose child in children)
		{
			child.FindHierarchy(poses);
		}
	}

	public BonePoseCollection GetHierarchy()
	{
		List<BonePose> list = new List<BonePose>();
		FindHierarchy(list);
		return new BonePoseCollection(list);
	}

	private void current_AnimationTracksChanged(object sender, EventArgs e)
	{
		doesAnimContainChannel = currentAnimation.ContainsAnimationTrack(this);
	}

	private void blend_AnimationTracksChanged(object sender, EventArgs e)
	{
		doesBlendContainChannel = currentBlendAnimation.ContainsAnimationTrack(this);
	}

	public Matrix GetCurrentTransform()
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			if (currentAnimation == null || !doesAnimContainChannel)
			{
				if (currentBlendAnimation == null || !doesBlendContainChannel)
				{
					return defaultMatrix;
				}
				blendMatrix = currentBlendAnimation.GetCurrentBoneTransform(this);
				Util.SlerpMatrix(ref defaultMatrix, ref blendMatrix, BlendFactor, out returnMatrix);
			}
			else
			{
				currentMatrixBuffer = currentAnimation.GetCurrentBoneTransform(this);
				if (currentBlendAnimation == null || !doesBlendContainChannel)
				{
					return currentMatrixBuffer;
				}
				blendMatrix = currentBlendAnimation.GetCurrentBoneTransform(this);
				Util.SlerpMatrix(ref currentMatrixBuffer, ref blendMatrix, BlendFactor, out returnMatrix);
			}
		}
		else
		{
			returnMatrix = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		}
		return returnMatrix;
	}
}
