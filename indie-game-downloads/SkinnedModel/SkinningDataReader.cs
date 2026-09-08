using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkinnedModel;

public class SkinningDataReader : ContentTypeReader<SkinningData>
{
	protected override SkinningData Read(ContentReader input, SkinningData existingInstance)
	{
		IDictionary<string, AnimationClip> animationClips = input.ReadObject<IDictionary<string, AnimationClip>>();
		IList<Matrix> bindPose = input.ReadObject<IList<Matrix>>();
		IList<Matrix> inverseBindPose = input.ReadObject<IList<Matrix>>();
		IList<int> skeletonHierarchy = input.ReadObject<IList<int>>();
		Texture2D animationTexture = input.ReadObject<Texture2D>();
		IList<int> boneIndices = input.ReadObject<IList<int>>();
		return new SkinningData(boneIndices, animationClips, bindPose, inverseBindPose, skeletonHierarchy, animationTexture);
	}
}
