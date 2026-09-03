using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public sealed class MultiBlendController : GameComponent, IAnimationController
{
	private Dictionary<IAnimationController, float> controllerDict;

	public Dictionary<IAnimationController, float> ControllerWeightDictionary => controllerDict;

	public event EventHandler AnimationTracksChanged;

	public MultiBlendController(Game game)
		: base(game)
	{
		((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)this);
		controllerDict = new Dictionary<IAnimationController, float>();
	}

	public Matrix GetCurrentBoneTransform(BonePose pose)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (controllerDict.Count == 0)
		{
			return pose.DefaultTransform;
		}
		Matrix val = default(Matrix);
		foreach (KeyValuePair<IAnimationController, float> item in controllerDict)
		{
			if (item.Key.ContainsAnimationTrack(pose))
			{
				Matrix currentBoneTransform = item.Key.GetCurrentBoneTransform(pose);
				val += item.Value * currentBoneTransform;
			}
			else
			{
				val += item.Value * pose.DefaultTransform;
			}
		}
		return val;
	}

	public bool ContainsAnimationTrack(BonePose pose)
	{
		return true;
	}

	private void OnAnimationTracksChanged(EventArgs e)
	{
		if (AnimationTracksChanged != null)
		{
			AnimationTracksChanged(this, e);
		}
	}
}
