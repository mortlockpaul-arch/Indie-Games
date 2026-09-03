using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation;

public class BonePoseCollection : ReadOnlyCollection<BonePose>
{
	private Dictionary<string, BonePose> boneDict = new Dictionary<string, BonePose>();

	public BonePose this[string boneName] => boneDict[boneName];

	internal BonePoseCollection(IList<BonePose> anims)
		: base(anims)
	{
		for (int i = 0; i < anims.Count; i++)
		{
			string name = anims[i].Name;
			if (name != null && name != "" && !boneDict.ContainsKey(name))
			{
				boneDict.Add(name, anims[i]);
			}
		}
	}

	public static BonePoseCollection FromModelBoneCollection(ModelBoneCollection bones)
	{
		BonePose[] anims = new BonePose[((ReadOnlyCollection<ModelBone>)(object)bones).Count];
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)bones).Count; i++)
		{
			if (((ReadOnlyCollection<ModelBone>)(object)bones)[i].Parent == null)
			{
				BonePose bonePose = new BonePose(((ReadOnlyCollection<ModelBone>)(object)bones)[i], bones, anims);
			}
		}
		return new BonePoseCollection(anims);
	}

	public void CopyAbsoluteTransformsTo(Matrix[] transforms)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < transforms.Length; i++)
		{
			if (i > 0)
			{
				Matrix currentTransform = base[i].GetCurrentTransform();
				Matrix val = transforms[base[i].Parent.Index];
				ref Matrix reference = ref transforms[i];
				reference = currentTransform * val;
			}
			else
			{
				ref Matrix reference2 = ref transforms[i];
				reference2 = base[i].GetCurrentTransform();
			}
		}
	}
}
