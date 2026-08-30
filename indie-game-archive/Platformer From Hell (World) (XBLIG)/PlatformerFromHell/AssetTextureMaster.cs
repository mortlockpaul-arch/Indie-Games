using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell;

internal class AssetTextureMaster
{
	private static Dictionary<string, Dictionary<int, Texture2D>> animationFrames = new Dictionary<string, Dictionary<int, Texture2D>>();

	private static Dictionary<string, Dictionary<int, Texture2D>> hitmapFrames = new Dictionary<string, Dictionary<int, Texture2D>>();

	private Animation currAnimation;

	private Texture2D staticTexture;

	private Texture2D staticHitmap;

	private int frameIndex;

	private float startTime;

	public Animation CurrAnimation => currAnimation;

	public int FrameIndex => frameIndex;

	public AssetTextureMaster(Texture2D staticTexture, Texture2D staticHitmap, int frameCount)
	{
		if (frameCount > 1)
		{
			int num = 0;
			int num2 = staticTexture.Width / frameCount;
			Rectangle area = new Rectangle(num * num2, 0, num2, staticTexture.Height);
			this.staticTexture = Crop(staticTexture, area);
			num2 = staticHitmap.Width / frameCount;
			area = new Rectangle(num * num2, 0, num2, staticHitmap.Height);
			this.staticHitmap = Crop(staticHitmap, area);
		}
		else
		{
			this.staticTexture = staticTexture;
			this.staticHitmap = staticHitmap;
		}
	}

	public void PlayAnimation(Animation animation)
	{
		if (currAnimation != animation)
		{
			currAnimation = animation;
			frameIndex = 0;
			startTime = -1f;
		}
	}

	public void StopAnimation()
	{
		currAnimation = null;
	}

	public Rectangle getRect(GameTime gameTime)
	{
		if (currAnimation == null)
		{
			return new Rectangle(0, 0, staticTexture.Width, staticTexture.Height);
		}
		if (startTime == -1f)
		{
			startTime = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		float num = ((float)gameTime.TotalGameTime.TotalSeconds - startTime) / currAnimation.FrameTime;
		float num2 = (currAnimation.IsLooping ? (num % (float)currAnimation.FrameCount) : Math.Min(num, currAnimation.FrameCount - 1));
		int num3 = (int)num2;
		int num4 = currAnimation.Texture.Width / currAnimation.FrameCount;
		return new Rectangle(num3 * num4, 0, num4, currAnimation.Texture.Height);
	}

	public Rectangle getHitmapRect(GameTime gameTime)
	{
		if (currAnimation == null)
		{
			return new Rectangle(0, 0, staticTexture.Width, staticTexture.Height);
		}
		if (currAnimation.HitmapTexture == null)
		{
			return new Rectangle(0, 0, staticTexture.Width, staticTexture.Height);
		}
		if (startTime == -1f)
		{
			startTime = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		float num = ((float)gameTime.TotalGameTime.TotalSeconds - startTime) / currAnimation.FrameTime;
		float num2 = (currAnimation.IsLooping ? (num % (float)currAnimation.FrameCount) : Math.Min(num, currAnimation.FrameCount - 1));
		int num3 = (int)num2;
		int num4 = currAnimation.HitmapTexture.Width / currAnimation.FrameCount;
		return new Rectangle(num3 * num4, 0, num4, currAnimation.Texture.Height);
	}

	public Texture2D getFrame(GameTime gameTime)
	{
		if (currAnimation == null)
		{
			return staticTexture;
		}
		if (startTime == -1f)
		{
			startTime = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		float num = ((float)gameTime.TotalGameTime.TotalSeconds - startTime) / currAnimation.FrameTime;
		float num2 = (currAnimation.IsLooping ? (num % (float)currAnimation.FrameCount) : Math.Min(num, currAnimation.FrameCount - 1));
		int num3 = (int)num2;
		int num4 = currAnimation.Texture.Width / currAnimation.FrameCount;
		Rectangle area = new Rectangle(num3 * num4, 0, num4, currAnimation.Texture.Height);
		string name = currAnimation.Texture.Name;
		if (!animationFrames.ContainsKey(name))
		{
			animationFrames.Add(name, new Dictionary<int, Texture2D>());
			animationFrames[name].Add(num3, Crop(currAnimation.Texture, area));
		}
		else if (!animationFrames[name].ContainsKey(num3))
		{
			animationFrames[name].Add(num3, Crop(currAnimation.Texture, area));
		}
		return animationFrames[name][num3];
	}

	public Texture2D getHitmapFrame(GameTime gameTime)
	{
		if (currAnimation == null)
		{
			return staticHitmap;
		}
		if (currAnimation.HitmapTexture == null)
		{
			return staticHitmap;
		}
		if (startTime == -1f)
		{
			startTime = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		float num = ((float)gameTime.TotalGameTime.TotalSeconds - startTime) / currAnimation.FrameTime;
		float num2 = (currAnimation.IsLooping ? (num % (float)currAnimation.FrameCount) : Math.Min(num, currAnimation.FrameCount - 1));
		int num3 = (int)num2;
		int num4 = currAnimation.HitmapTexture.Width / currAnimation.FrameCount;
		Rectangle area = new Rectangle(num3 * num4, 0, num4, currAnimation.Texture.Height);
		string name = currAnimation.Texture.Name;
		if (!hitmapFrames.ContainsKey(name))
		{
			hitmapFrames.Add(name, new Dictionary<int, Texture2D>());
			hitmapFrames[name].Add(num3, Crop(currAnimation.HitmapTexture, area));
		}
		else if (!hitmapFrames[name].ContainsKey(num3))
		{
			hitmapFrames[name].Add(num3, Crop(currAnimation.HitmapTexture, area));
		}
		return hitmapFrames[name][num3];
	}

	public static Texture2D Crop(Texture2D source, Rectangle area)
	{
		if (source == null)
		{
			return null;
		}
		Texture2D texture2D = new Texture2D(source.GraphicsDevice, area.Width, area.Height);
		Color[] array = new Color[source.Width * source.Height];
		Color[] array2 = new Color[texture2D.Width * texture2D.Height];
		source.GetData(array);
		int num = 0;
		for (int i = area.Y; i < area.Y + area.Height; i++)
		{
			for (int j = area.X; j < area.X + area.Width; j++)
			{
				ref Color reference = ref array2[num];
				reference = array[j + i * source.Width];
				num++;
			}
		}
		texture2D.SetData(array2);
		return texture2D;
	}

	public static void StaticDispose()
	{
		foreach (Dictionary<int, Texture2D> value in animationFrames.Values)
		{
			foreach (Texture2D value2 in value.Values)
			{
				value2.Dispose();
			}
			value.Clear();
		}
		animationFrames.Clear();
		foreach (Dictionary<int, Texture2D> value3 in hitmapFrames.Values)
		{
			foreach (Texture2D value4 in value3.Values)
			{
				value4.Dispose();
			}
			value3.Clear();
		}
		hitmapFrames.Clear();
	}
}
