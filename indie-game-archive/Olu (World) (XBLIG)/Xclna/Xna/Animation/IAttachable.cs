using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public interface IAttachable
{
	Matrix LocalTransform { get; }

	Matrix CombinedTransform { get; set; }

	BonePose AttachedBone { get; }
}
