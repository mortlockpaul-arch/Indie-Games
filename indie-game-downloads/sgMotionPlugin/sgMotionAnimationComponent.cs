using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Rendering;

namespace sgMotionPlugin;

public class sgMotionAnimationComponent : BaseComponentAutoSerialization<ISceneEntity>
{
	private InterpolationMode _InterpolationMode;

	private AnimationController _AnimationController;

	private SkinnedModel _SkinnedModel;

	private static ReadOnlyCollection<string> _EmptyAnimationList = new ReadOnlyCollection<string>(new List<string>());

	public InterpolationMode InterpolationMode
	{
		get
		{
			return _InterpolationMode;
		}
		set
		{
			_InterpolationMode = value;
			if (_AnimationController != null)
			{
				switch (_InterpolationMode)
				{
				case InterpolationMode.None:
					_AnimationController.TranslationInterpolation = InterpolationMode.None;
					_AnimationController.OrientationInterpolation = InterpolationMode.None;
					_AnimationController.ScaleInterpolation = InterpolationMode.None;
					break;
				case InterpolationMode.Linear:
					_AnimationController.TranslationInterpolation = InterpolationMode.Linear;
					_AnimationController.OrientationInterpolation = InterpolationMode.Linear;
					_AnimationController.ScaleInterpolation = InterpolationMode.Linear;
					break;
				case InterpolationMode.Cubic:
					_AnimationController.TranslationInterpolation = InterpolationMode.Cubic;
					_AnimationController.OrientationInterpolation = InterpolationMode.Linear;
					_AnimationController.ScaleInterpolation = InterpolationMode.Cubic;
					break;
				case InterpolationMode.Spherical:
					_AnimationController.TranslationInterpolation = InterpolationMode.Linear;
					_AnimationController.OrientationInterpolation = InterpolationMode.Spherical;
					_AnimationController.ScaleInterpolation = InterpolationMode.Linear;
					break;
				}
			}
		}
	}

	public ReadOnlyCollection<string> AnimationNames
	{
		get
		{
			if (_SkinnedModel == null || _SkinnedModel.AnimationClips == null)
			{
				return _EmptyAnimationList;
			}
			return _SkinnedModel.AnimationClips.Keys;
		}
	}

	public AnimationController AnimationController => _AnimationController;

	public SkinnedModel SkinnedModel => _SkinnedModel;

	public void SwitchToAnimation(string name, TimeSpan transitiontime)
	{
		if (_AnimationController != null && _SkinnedModel != null && _SkinnedModel.AnimationClips.TryGetValue(name, out var value))
		{
			_AnimationController.CrossFade(value, transitiontime);
		}
	}

	public override void OnAddedToParentObject()
	{
		base.OnAddedToParentObject();
		if (base.ParentObject is ISceneObject sceneObject && sceneObject.ModelAsset.Asset != null)
		{
			_SkinnedModel = sceneObject.ModelAsset.Asset.Tag as SkinnedModel;
			if (_SkinnedModel != null)
			{
				_AnimationController = new AnimationController(_SkinnedModel.SkeletonBones);
				InterpolationMode = InterpolationMode.Linear;
			}
		}
	}

	public override void OnUpdate(GameTime gametime)
	{
		base.OnUpdate(gametime);
		if (_AnimationController != null)
		{
			_AnimationController.Update(gametime.ElapsedGameTime, Matrix.Identity);
			if (base.ParentObject is ISceneObject sceneObject)
			{
				sceneObject.SkinBones = _AnimationController.SkinnedBoneTransforms;
			}
		}
	}
}
