using System;
using Microsoft.Xna.Framework;
using PlatformerFromHell.Asset_Classes;

namespace PlatformerFromHell;

internal class OldCollisionManager
{
	private static float InteresectDepthY(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(rectangleA.Left, rectangleB.Left);
		int num4 = Math.Min(rectangleA.Right, rectangleB.Right);
		int num5 = num2 - num;
		return num5;
	}

	private static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(rectangleA.Left, rectangleB.Left);
		int num4 = Math.Min(rectangleA.Right, rectangleB.Right);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Color color = dataA[j - rectangleA.Left + (i - rectangleA.Top) * rectangleA.Width];
				Color color2 = dataB[j - rectangleB.Left + (i - rectangleB.Top) * rectangleB.Width];
				if (color.A != 0 && color2.A != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool CollisiontType(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(rectangleA.Left, rectangleB.Left);
		int num4 = Math.Min(rectangleA.Right, rectangleB.Right);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Color color = dataA[j - rectangleA.Left + (i - rectangleA.Top) * rectangleA.Width];
				Color color2 = dataB[j - rectangleB.Left + (i - rectangleB.Top) * rectangleB.Width];
				if (color.A != 0 && color2.A != 0)
				{
					if (color2.R != byte.MaxValue || color2.B != 0 || color2.G == 0)
					{
					}
					if (color2.R == 0 && color2.B == 0 && color2.G == byte.MaxValue)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void HandleCollisions(Player p)
	{
		p.isOnGround = false;
		foreach (Asset asset in p.level.assets)
		{
			if (asset is Platform)
			{
				if (asset.GetFlip() < Asset.Dir.DownLeft)
				{
					p.blockRectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y - 1, asset.frameWidth, 8);
					if (IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData))
					{
						p.isOnGround = true;
						float num = InteresectDepthY(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData);
						p.position = new Vector2(p.position.X, (float)(int)p.position.Y - num + 1f);
						foreach (Asset asset2 in p.level.assets)
						{
							if (asset2 is ExitAsset)
							{
								p.blockRectangle = new Rectangle((int)asset2.Position.X, (int)asset2.Position.Y, asset2.frameWidth, asset2.frameHeight);
								if (p.isOnGround && IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset2.textureData))
								{
									p.level.OnExitReached();
								}
							}
						}
					}
					p.blockRectangle = new Rectangle((int)asset.Position.X + 5, (int)asset.Position.Y + 8, asset.frameWidth - 10, 4);
					if (IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData) && p.isAlive)
					{
						p.OnKilled("spike");
					}
				}
				else if (asset.GetFlip() > Asset.Dir.UpRight)
				{
					p.blockRectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y + 4, asset.frameWidth, 8);
					if (!p.isOnGround && IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData))
					{
						p.isOnGround = true;
						float num = InteresectDepthY(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData);
						p.position = new Vector2(p.position.X, (float)(int)p.position.Y - num + 1f);
					}
					p.blockRectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y, asset.frameWidth - 10, 4);
					if (IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData) && p.isAlive)
					{
						p.OnKilled("spike");
					}
				}
			}
			if (asset is Switch)
			{
				p.blockRectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y, asset.frameWidth, asset.frameHeight);
				if (IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData) && asset.canBeTouched >= 101f)
				{
					p.flipPlatforms(((Switch)asset).GetSwitchChar());
					asset.canBeTouched = 0f;
				}
			}
			if (asset is Wall)
			{
				p.blockRectangle = new Rectangle((int)asset.Position.X, (int)asset.Position.Y, asset.frameWidth, asset.frameHeight);
				if (IntersectPixels(p.personRectangle, p.personTextureData, p.blockRectangle, asset.textureData))
				{
					p.OnKilled("lava");
				}
			}
		}
	}
}
