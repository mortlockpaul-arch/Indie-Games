using System;
using Microsoft.Xna.Framework;
using PlatformerFromHell.Asset_Classes;

namespace PlatformerFromHell;

internal class CollisionManager
{
	public class Hit
	{
		public bool hitRed = false;

		public bool hitGreen = false;

		public bool hitBlue = false;

		public Vector2 depth = new Vector2(-8888f, -8888f);

		public bool hitNone => !hitRed && !hitGreen && !hitBlue;
	}

	private Rectangle blockRectangle = default(Rectangle);

	public static Hit hit;

	public static Gravity.GravDir GetGravity(Player p)
	{
		Gravity.GravDir result = Gravity.GravDir.Down;
		foreach (Asset asset in p.level.assets)
		{
			if (asset is Gravity)
			{
				Gravity gravity = (Gravity)asset;
				if (getGravityHit(p, gravity))
				{
					result = gravity.GetGravDir;
				}
			}
		}
		return result;
	}

	public static bool getIsOnGround(Player p, Gravity.GravDir groundDir)
	{
		int num = 0;
		foreach (Asset item in p.level.getAssetsInPlayerSquare(p))
		{
			num++;
			if ((!(item is Platform) && !(item is Wall)) || item.texturename.Contains("money") || item.disabled || (groundDir != Gravity.GravDir.Down && groundDir != Gravity.GravDir.Up) || !getGreenEdgeHit(p, item, groundDir))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	private static bool getGreenEdgeHit(Player p, Asset a, Gravity.GravDir dir)
	{
		Rectangle personRectangle = p.personRectangle;
		Color[] personTextureData = p.personTextureData;
		Rectangle value = new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight);
		Color[] hitmapData = a.getHitmapData();
		Rectangle rectangle = personRectangle;
		rectangle.Offset(-1, -1);
		rectangle.Inflate(2, 2);
		if (!rectangle.Intersects(value))
		{
			return false;
		}
		switch (dir)
		{
		case Gravity.GravDir.Up:
		{
			rectangle = personRectangle;
			rectangle.Height = 1;
			int i = rectangle.Top;
			for (int left = rectangle.Left; left <= rectangle.Right; left++)
			{
				if (!Transparent(personTextureData[left - rectangle.Left + (i - rectangle.Top) * rectangle.Width]) && value.Contains(left, i - 1) && hitmapData[left - value.Left + (i - 1 - value.Top) * value.Width].Equals(new Color(0, 255, 0, 255)))
				{
					return true;
				}
			}
			break;
		}
		case Gravity.GravDir.Down:
		{
			rectangle = personRectangle;
			rectangle.Offset(0, rectangle.Height - 1);
			rectangle.Height = 1;
			int i = rectangle.Top;
			for (int left = rectangle.Left; left <= rectangle.Right; left++)
			{
				if (!Transparent(personTextureData[left - rectangle.Left + (i - rectangle.Top) * rectangle.Width]) && value.Contains(left, i + 1) && hitmapData[left - value.Left + (i + 1 - value.Top) * value.Width].Equals(new Color(0, 255, 0, 255)))
				{
					return true;
				}
			}
			break;
		}
		case Gravity.GravDir.Left:
		{
			rectangle = personRectangle;
			rectangle.Width = 1;
			int left = rectangle.Left;
			for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
			{
				if (value.Contains(left - 1, i) && hitmapData[left - 1 - value.Left + (i - value.Top) * value.Width].Equals(new Color(0, 255, 0, 255)))
				{
					return true;
				}
			}
			break;
		}
		case Gravity.GravDir.Right:
		{
			rectangle = personRectangle;
			rectangle.Offset(rectangle.Width - 1, 0);
			rectangle.Width = 1;
			int left = rectangle.Left;
			for (int i = rectangle.Top; i <= rectangle.Bottom; i++)
			{
				if (value.Contains(left + 1, i) && hitmapData[left + 1 - value.Left + (i - value.Top) * value.Width].Equals(new Color(0, 255, 0, 255)))
				{
					return true;
				}
			}
			break;
		}
		}
		return false;
	}

	private static Rectangle GetIntersect(Rectangle rectangleA, Rectangle rectangleB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(rectangleA.Left, rectangleB.Left);
		int num4 = Math.Min(rectangleA.Right, rectangleB.Right);
		if (num4 - num3 < 0 || num2 - num < 0)
		{
			return new Rectangle(0, 0, 0, 0);
		}
		return new Rectangle(num3, num, num4 - num3, num2 - num);
	}

	private static void Bump(Player p, Asset a, Gravity.GravDir dir)
	{
		switch (bumpDirBySideTime(p, a, dir))
		{
		case Gravity.GravDir.Up:
			BumpUp(p, a);
			break;
		case Gravity.GravDir.Down:
			BumpDown(p, a);
			break;
		case Gravity.GravDir.Left:
			BumpLeft(p, a);
			break;
		case Gravity.GravDir.Right:
			BumpRight(p, a);
			break;
		case Gravity.GravDir.None:
			Console.Out.WriteLine("--------No Bump!!!--------");
			break;
		}
	}

	private static Gravity.GravDir bumpDirBySideTime(Player p, Asset a, Gravity.GravDir dir)
	{
		Rectangle previousRectangle = p.previousRectangle;
		Rectangle rectangle = new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight);
		Rectangle personRectangle = p.personRectangle;
		int num = Math.Abs(previousRectangle.Top - rectangle.Bottom);
		int num2 = Math.Abs(previousRectangle.Bottom - rectangle.Top);
		int num3 = Math.Abs(previousRectangle.Left - rectangle.Right);
		int num4 = Math.Abs(previousRectangle.Right - rectangle.Left);
		float num5 = Math.Abs((float)num / p.Velocity.Y);
		float num6 = Math.Abs((float)num2 / p.Velocity.Y);
		float num7 = Math.Abs((float)num3 / p.Velocity.X);
		float num8 = Math.Abs((float)num4 / p.Velocity.X);
		if (double.IsInfinity(num5) && double.IsInfinity(num6) && double.IsInfinity(num7) && double.IsInfinity(num8))
		{
			return (p.mostRecentY < 0f) ? Gravity.GravDir.Down : Gravity.GravDir.Up;
		}
		float num9 = Math.Min(Math.Min(num5, num6), Math.Min(num7, num8));
		if (num9 == num7)
		{
			return Gravity.GravDir.Right;
		}
		if (num9 == num8)
		{
			return Gravity.GravDir.Left;
		}
		if (num9 == num5)
		{
			return Gravity.GravDir.Down;
		}
		return Gravity.GravDir.Up;
	}

	private static Gravity.GravDir bumpDirByPrevAndIntersect(Player p, Asset a, Gravity.GravDir dir)
	{
		Rectangle previousRectangle = p.previousRectangle;
		Rectangle rectangle = new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight);
		Rectangle personRectangle = p.personRectangle;
		Rectangle intersect = GetIntersect(personRectangle, rectangle);
		if (intersect.Height > intersect.Width)
		{
			if (previousRectangle.Bottom <= rectangle.Top)
			{
				return Gravity.GravDir.Up;
			}
			if (previousRectangle.Top >= rectangle.Bottom)
			{
				return Gravity.GravDir.Down;
			}
			if (personRectangle.Left <= rectangle.Right && p.Velocity.X < 0f)
			{
				return Gravity.GravDir.Right;
			}
			if (personRectangle.Right >= rectangle.Left && p.Velocity.X > 0f)
			{
				return Gravity.GravDir.Left;
			}
			Console.Out.WriteLine(string.Concat("Uncaught Bump. recP=", personRectangle, " prevA=", previousRectangle, " recB=", rectangle));
			return Gravity.GravDir.None;
		}
		if (intersect.Width < intersect.Height)
		{
			if (previousRectangle.Left >= rectangle.Right)
			{
				return Gravity.GravDir.Right;
			}
			if (previousRectangle.Right <= rectangle.Left)
			{
				return Gravity.GravDir.Left;
			}
			if (previousRectangle.Bottom <= rectangle.Top)
			{
				return Gravity.GravDir.Up;
			}
			if (previousRectangle.Top >= rectangle.Bottom)
			{
				return Gravity.GravDir.Down;
			}
			Console.Out.WriteLine(string.Concat("Uncaught Bump. recP=", personRectangle, " prevA=", previousRectangle, " recB=", rectangle));
			return Gravity.GravDir.None;
		}
		Console.Out.WriteLine("intersect rectangle width == height");
		return Gravity.GravDir.None;
	}

	private static Gravity.GravDir bumpDirByPrevAndGravity(Player p, Asset a, Gravity.GravDir dir)
	{
		Rectangle previousRectangle = p.previousRectangle;
		Rectangle rectangle = new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight);
		Rectangle personRectangle = p.personRectangle;
		if (previousRectangle.Left >= rectangle.Right)
		{
			return Gravity.GravDir.Right;
		}
		if (previousRectangle.Right <= rectangle.Left)
		{
			return Gravity.GravDir.Left;
		}
		if (previousRectangle.Bottom <= rectangle.Top)
		{
			return Gravity.GravDir.Up;
		}
		if (previousRectangle.Top >= rectangle.Bottom)
		{
			return Gravity.GravDir.Down;
		}
		return Gravity.GravDir.None;
	}

	private static void BumpUp(Player p, Asset asset)
	{
		p.position.Y = (int)p.lastPosition.Y;
		p.velocity.Y = 0f;
		Console.Out.WriteLine("Bump Up ");
		if (p.isOnGround)
		{
			p.jumpTime = -1f;
		}
		if (!(asset is Platform))
		{
			return;
		}
		if (!p.isOnGround && p.previousRectangle.Bottom >= asset.currRect.Center.Y)
		{
			if (p.previousRectangle.Left > asset.currRect.Center.X)
			{
				BumpRightWall(p, asset);
				p.position.X++;
			}
			else if (p.previousRectangle.Right < asset.currRect.Center.X)
			{
				BumpLeftWall(p, asset);
				p.position.X--;
			}
		}
		else if (p.previousRectangle.Right < asset.currRect.Center.X)
		{
			p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
		}
		else if (p.previousRectangle.Left > asset.currRect.Center.X)
		{
			p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
		}
	}

	private static void BumpDown(Player p, Asset asset)
	{
		Console.Out.WriteLine("Bump Down ");
		p.position.Y = (int)p.lastPosition.Y;
		p.velocity.Y = 0f;
		if (p.isOnGround)
		{
			p.jumpTime = -1f;
		}
		if (!(asset is Platform))
		{
			return;
		}
		if (!p.isOnGround && p.previousRectangle.Top <= asset.currRect.Center.Y)
		{
			if (p.previousRectangle.Left > asset.currRect.Center.X)
			{
				BumpRightWall(p, asset);
				p.position.X++;
			}
			else if (p.previousRectangle.Right < asset.currRect.Center.X)
			{
				BumpLeftWall(p, asset);
				p.position.X--;
			}
		}
		else if (p.previousRectangle.Right < asset.currRect.Center.X)
		{
			p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
		}
		else if (p.previousRectangle.Left > asset.currRect.Center.X)
		{
			p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
		}
	}

	private static void BumpRight(Player p, Asset asset)
	{
		if (p.velocity.Y == 0f && p.isOnGround)
		{
			BumpRightWall(p, asset);
			return;
		}
		p.blockRectangle.X = (int)asset.Position.X;
		p.blockRectangle.Y = (int)asset.Position.Y;
		p.blockRectangle.Width = asset.frameWidth;
		p.blockRectangle.Height = asset.frameHeight;
		float num = IntersectDepthX(p.previousRectangle, p.blockRectangle);
		p.position.X += num + 1f;
		p.velocity.X = 0f;
		Console.Out.WriteLine("Bump Right ");
	}

	private static void BumpLeft(Player p, Asset asset)
	{
		if (p.velocity.Y == 0f && p.isOnGround)
		{
			BumpLeftWall(p, asset);
			return;
		}
		p.blockRectangle.X = (int)asset.Position.X;
		p.blockRectangle.Y = (int)asset.Position.Y;
		p.blockRectangle.Width = asset.frameWidth;
		p.blockRectangle.Height = asset.frameHeight;
		float num = IntersectDepthX(p.previousRectangle, p.blockRectangle);
		p.position.X -= num + 1f;
		p.velocity.X = 0f;
		Console.Out.WriteLine("Bump Left ");
	}

	private static void BumpUpWall(Player p, Asset asset)
	{
		p.position.Y = (int)p.lastPosition.Y;
		p.velocity.Y = 0f;
		Console.Out.WriteLine("Bump Up Wall" + asset.texturename + " " + asset.GetFlip());
		if (p.isOnGround)
		{
			p.jumpTime = -1f;
		}
	}

	private static void BumpDownWall(Player p, Asset asset)
	{
		Console.Out.WriteLine("Bump Down Wall" + asset.texturename + " " + asset.GetFlip());
		p.position.Y = (int)p.lastPosition.Y;
		p.velocity.Y = 0f;
		if (p.isOnGround)
		{
			p.jumpTime = -1f;
		}
	}

	private static void BumpRightWall(Player p, Asset asset)
	{
		p.position.X = (int)p.lastPosition.X;
		p.velocity.X = 0f;
		Console.Out.WriteLine("Bump Right Wall " + asset.texturename + " " + asset.GetFlip());
	}

	private static void BumpLeftWall(Player p, Asset asset)
	{
		p.position.X = (int)p.lastPosition.X;
		p.velocity.X = 0f;
		Console.Out.WriteLine("Bump Left Wall" + asset.texturename + " " + asset.GetFlip());
	}

	public static void HandleCollisions(Player p, Gravity.GravDir grav)
	{
		foreach (Asset item in p.level.getAssetsInPlayerSquare(p))
		{
			while (!(item is Gravity) && !(item is Background) && !item.disabled)
			{
				Hit hit = getHit(p, item);
				if (hit.hitNone)
				{
					break;
				}
				if (item is Wall)
				{
					CollisionManager.hit = hit;
					if (hit.hitRed)
					{
						p.OnKilled(item.texturename, p.Position.Y > item.Position.Y);
						return;
					}
					if (!Program.game.hasMoney(p.level.levelNumber) && item.texturename.Equals("money"))
					{
						Program.game.chaChingInstance.Play();
						p.level.moneyGrabbed = true;
						Console.Out.WriteLine("Money got!");
						item.disabled = true;
						return;
					}
					if (item.texturename.Equals("money"))
					{
						if (!item.disabled)
						{
							Program.game.chaChingInstance.Play();
						}
						item.disabled = true;
						return;
					}
					if (item.texturename.Contains("top"))
					{
						switch (item.GetFlip())
						{
						case Asset.Dir.UpRight:
							BumpUpWall(p, item);
							break;
						case Asset.Dir.UpLeft:
							BumpUpWall(p, item);
							break;
						case Asset.Dir.DownRight:
							BumpDownWall(p, item);
							break;
						case Asset.Dir.DownLeft:
							BumpDownWall(p, item);
							break;
						}
					}
					else if (item.texturename.Contains("edge"))
					{
						switch (item.GetFlip())
						{
						case Asset.Dir.UpLeft:
							BumpLeftWall(p, item);
							break;
						case Asset.Dir.UpRight:
							BumpRightWall(p, item);
							break;
						case Asset.Dir.DownLeft:
							BumpLeftWall(p, item);
							break;
						case Asset.Dir.DownRight:
							BumpRightWall(p, item);
							break;
						}
					}
					else if (item.texturename.Contains("corner"))
					{
						switch (item.GetFlip())
						{
						case Asset.Dir.UpRight:
							if (p.previousRectangle.Bottom <= item.currRect.Top && p.previousRectangle.Right >= item.currRect.Left)
							{
								BumpUpWall(p, item);
								if (!p.isOnGround)
								{
									p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
								}
							}
							else
							{
								BumpLeftWall(p, item);
								p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
							}
							break;
						case Asset.Dir.UpLeft:
							if (p.previousRectangle.Bottom <= item.currRect.Top && p.previousRectangle.Left <= item.currRect.Right)
							{
								BumpUpWall(p, item);
								if (!p.isOnGround)
								{
									p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
								}
							}
							else
							{
								BumpRightWall(p, item);
								p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
							}
							break;
						case Asset.Dir.DownRight:
							if (p.previousRectangle.Top >= item.currRect.Bottom && p.previousRectangle.Right >= item.currRect.Left)
							{
								BumpDownWall(p, item);
								if (!p.isOnGround)
								{
									p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
								}
							}
							else
							{
								BumpLeftWall(p, item);
								p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
							}
							break;
						case Asset.Dir.DownLeft:
							if (p.previousRectangle.Top >= item.currRect.Bottom && p.previousRectangle.Left <= item.currRect.Right)
							{
								BumpDownWall(p, item);
								p.position.X -= 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
							}
							else
							{
								BumpRightWall(p, item);
								p.position.X += 1f + (float)p.playerGameTime.ElapsedGameTime.TotalSeconds;
							}
							break;
						}
					}
					else if (item.texturename.Contains("joint"))
					{
						switch (item.GetFlip())
						{
						case Asset.Dir.UpRight:
							BumpLeftWall(p, item);
							break;
						case Asset.Dir.UpLeft:
							BumpRightWall(p, item);
							break;
						case Asset.Dir.DownRight:
							BumpLeftWall(p, item);
							break;
						case Asset.Dir.DownLeft:
							BumpRightWall(p, item);
							break;
						}
					}
					else
					{
						Bump(p, item, grav);
					}
					break;
				}
				if (item is Platform)
				{
					if (p.isJumping)
					{
						if (hit.hitGreen && !hit.hitRed)
						{
							if (!Program.game.hasMoney(p.level.levelNumber) && item.texturename.Equals("money"))
							{
								Program.game.chaChingInstance.Play();
								p.level.moneyGrabbed = true;
								Console.Out.WriteLine("Money got!");
								item.disabled = true;
								return;
							}
							if (item.texturename.Equals("money"))
							{
								if (!item.disabled)
								{
									Program.game.chaChingInstance.Play();
								}
								item.disabled = true;
								return;
							}
							CollisionManager.hit = hit;
							Bump(p, item, grav);
							continue;
						}
						if (hit.hitRed)
						{
							p.OnKilled(item.texturename, p.position.Y > item.Position.Y);
							return;
						}
						break;
					}
					if (hit.hitRed && !hit.hitGreen)
					{
						p.OnKilled(item.texturename, p.Position.Y > item.Position.Y);
						return;
					}
					if (!hit.hitGreen)
					{
						break;
					}
					if (!Program.game.hasMoney(p.level.levelNumber) && item.texturename.Equals("money"))
					{
						Program.game.chaChingInstance.Play();
						p.level.moneyGrabbed = true;
						Console.Out.WriteLine("Money got!");
						item.disabled = true;
						return;
					}
					if (item.texturename.Equals("money"))
					{
						if (!item.disabled)
						{
							Program.game.chaChingInstance.Play();
						}
						item.disabled = true;
						return;
					}
					CollisionManager.hit = hit;
					Bump(p, item, grav);
					continue;
				}
				if (item is Switch)
				{
					if (hit.hitGreen && p.timeTillSwitch <= 0.0)
					{
						p.flipPlatforms(((Switch)item).GetSwitchChar());
						p.timeTillSwitch = 60.0;
					}
				}
				else if (item is ExitAsset && p.isOnGround && hit.hitGreen && p.isAlive)
				{
					p.level.OnExitReached();
				}
				break;
			}
		}
	}

	public static bool Transparent(Color c)
	{
		return c.A == 0 || (c.B == byte.MaxValue && c.R == byte.MaxValue);
	}

	private static float IntersectDepthY(Rectangle rectangleA, Rectangle rectangleB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(num2 - num, 0);
		return num3;
	}

	private static float IntersectDepthX(Rectangle rectangleA, Rectangle rectangleB)
	{
		int num = Math.Max(rectangleA.Left, rectangleB.Left);
		int num2 = Math.Min(rectangleA.Right, rectangleB.Right);
		return Math.Max(num2 - num, 0);
	}

	private static Hit getHit(Player p, Asset a)
	{
		return getHit(rectangleB: new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight), rectangleA: p.personRectangle, dataA: p.personTextureData, dataB: a.getHitmapData());
	}

	private static bool getGravityHit(Player p, Asset a)
	{
		return GetGravityHit(rectangleB: new Rectangle((int)a.Position.X, (int)a.Position.Y, a.frameWidth, a.frameHeight), rectangleA: p.personRectangle, dataA: p.personTextureData);
	}

	private static Hit getHit(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		Rectangle intersect = GetIntersect(rectangleA, rectangleB);
		if (intersect.Equals(new Rectangle(0, 0, 0, 0)))
		{
			return new Hit();
		}
		Hit hit = new Hit();
		Vector2 vector = new Vector2(-9999f, -9999f);
		for (int i = intersect.Top; i < intersect.Bottom; i++)
		{
			for (int j = intersect.Left; j < intersect.Right; j++)
			{
				Color c = dataA[j - rectangleA.Left + (i - rectangleA.Top) * rectangleA.Width];
				Color c2 = dataB[j - rectangleB.Left + (i - rectangleB.Top) * rectangleB.Width];
				if (Transparent(c) || Transparent(c2))
				{
					continue;
				}
				if (c2.Equals(new Color(255, 0, 0, 255)))
				{
					hit.hitRed = true;
					hit.depth = new Vector2((float)(j - intersect.X) - vector.X, (float)(i - intersect.Y) - vector.Y);
				}
				else if (c2.Equals(new Color(0, 255, 0, 255)))
				{
					if (vector.X < 0f && vector.Y < 0f)
					{
						vector = new Vector2(j - intersect.X - 1, i - intersect.Y - 1);
					}
					hit.hitGreen = true;
					hit.depth = new Vector2((float)(j - intersect.X) - vector.X, (float)(i - intersect.Y) - vector.Y);
				}
				else if (c2.Equals(new Color(0, 0, 255, 255)))
				{
					hit.hitBlue = true;
					hit.depth = new Vector2((float)(j - intersect.X) - vector.X, (float)(i - intersect.Y) - vector.Y);
				}
			}
		}
		return hit;
	}

	private static bool GetGravityHit(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB)
	{
		Rectangle intersect = GetIntersect(rectangleA, rectangleB);
		if (intersect.Equals(new Rectangle(0, 0, 0, 0)))
		{
			return false;
		}
		Vector2 vector = new Vector2(-9999f, -9999f);
		for (int i = intersect.Top; i < intersect.Bottom; i++)
		{
			for (int j = intersect.Left; j < intersect.Right; j++)
			{
				Color c = dataA[j - rectangleA.Left + (i - rectangleA.Top) * rectangleA.Width];
				if (!Transparent(c))
				{
					return true;
				}
			}
		}
		return false;
	}
}
