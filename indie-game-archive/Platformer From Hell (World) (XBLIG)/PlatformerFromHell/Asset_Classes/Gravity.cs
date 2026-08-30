using Microsoft.Xna.Framework;

namespace PlatformerFromHell.Asset_Classes;

internal abstract class Gravity : Asset
{
	public enum GravDir
	{
		Up,
		Down,
		Left,
		Right,
		None
	}

	protected GravDir gravDir;

	public GravDir GetGravDir => gravDir;

	public Gravity(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip)
		: base(level, position, texturename, frameCount, newFlip)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
	}
}
