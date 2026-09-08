using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Interface that provides custom effects with skinned animation support.
/// </summary>
public interface ISkinnedEffect
{
	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used in the model or vertex buffer.
	/// </summary>
	Matrix[] SkinBones { get; set; }

	/// <summary>
	/// Determines if the effect is currently rendering skinned objects.
	/// </summary>
	bool Skinned { get; set; }
}
