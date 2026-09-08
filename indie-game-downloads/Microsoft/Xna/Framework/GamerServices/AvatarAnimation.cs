using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.GamerServices;

public class AvatarAnimation : IAvatarAnimation, IDisposable
{
	public ReadOnlyCollection<Matrix> BoneTransforms { get; private set; }

	public TimeSpan CurrentPosition { get; set; }

	public TimeSpan Length { get; private set; }

	public AvatarExpression Expression { get; private set; }

	public AvatarAnimation(AvatarAnimationPreset animationPreset)
	{
		BoneTransforms = new ReadOnlyCollection<Matrix>(new Matrix[71]);
	}

	public void Update(TimeSpan elapsedAnimationTime, bool loop)
	{
	}

	public void Dispose()
	{
	}
}
