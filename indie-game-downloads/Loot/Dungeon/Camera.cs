using Microsoft.Xna.Framework;

namespace Loot.Dungeon;

public class Camera
{
	public const float MinZoom = 0.5f;

	public const float MaxZoom = 2f;

	public const float DefaultZoom = 1f;

	private Vector2 offset;

	private float currentZoom = 1f;

	private float targetZoom = 1f;

	private float cellSize;

	public Vector2 Offset => offset;

	public float Zoom => currentZoom;

	public void ZoomAdjust(float z)
	{
		currentZoom = MathHelper.Clamp(currentZoom + z, 0.5f, 2f);
		targetZoom = currentZoom;
	}

	public void ZoomReset()
	{
		ZoomTo(1f);
	}

	public void ZoomTo(float z)
	{
		targetZoom = MathHelper.Clamp(z, 0.5f, 2f);
	}

	public void Update(GameTime gameTime)
	{
		if ((double)currentZoom != (double)targetZoom)
		{
			if ((double)currentZoom < (double)targetZoom)
			{
				currentZoom += (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds);
				if ((double)currentZoom > (double)targetZoom)
				{
					currentZoom = targetZoom;
				}
			}
			else
			{
				currentZoom -= (float)(2.0 * gameTime.ElapsedGameTime.TotalSeconds);
				if ((double)currentZoom < (double)targetZoom)
				{
					currentZoom = targetZoom;
				}
			}
		}
		cellSize = 64f * currentZoom;
		offset = DungeonView.Center + new Vector2((float)((double)(-DM.Player.Location.Col) * (double)cellSize - (double)DM.Player.Offset.X * (double)currentZoom), (float)((double)(-DM.Player.Location.Row) * (double)cellSize - (double)DM.Player.Offset.Y * (double)currentZoom));
	}

	public Vector2 Position(Location loc)
	{
		return offset + new Vector2(loc.Col, loc.Row) * cellSize;
	}

	public Vector2 Position(Character c)
	{
		return offset + new Vector2(c.Location.Col, c.Location.Row) * cellSize + c.Offset * currentZoom;
	}
}
