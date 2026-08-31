using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Class to hold all of the SpriteBatch-specific drawing Settings
/// </summary>
public class SpriteBatchSettings
{
	/// <summary>
	/// The Sort Mode to use in the SpriteBatch.Begin() function call.
	/// </summary>
	public SpriteSortMode SortMode;

	/// <summary>
	/// The Transformation Matrix used in the SpriteBatch.Begin() function call.
	/// </summary>
	public Matrix TransformationMatrix = Matrix.Identity;
}
