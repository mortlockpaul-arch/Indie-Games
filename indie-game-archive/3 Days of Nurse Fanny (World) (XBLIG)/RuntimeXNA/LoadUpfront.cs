using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RuntimeXNA;

public static class LoadUpfront
{
	public static volatile bool Active;

	public static string gatherAssetNames;

	private static ContentManager Content;

	private static GraphicsDevice graphicsDevice;

	private static SpriteBatch spriteBatch;

	private static Texture2D loadingSwirl;

	private static float progressTime;

	internal static void LoadDataUpfront()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 3 });
		Content.Load<Texture2D>("Img0000");
		Content.Load<Texture2D>("Img0010");
		Content.Load<Texture2D>("Img0012");
		Content.Load<Texture2D>("Img0013");
		Content.Load<Texture2D>("Img0014");
		Content.Load<Texture2D>("Img0015");
		Content.Load<Texture2D>("Img0016");
		Content.Load<Texture2D>("Img0017");
		Content.Load<Texture2D>("ImgM0005");
		Content.Load<Texture2D>("Img0022");
		Content.Load<Texture2D>("Img0023");
		Content.Load<Texture2D>("ImgM0006");
		Content.Load<Texture2D>("Img0057");
		Content.Load<Texture2D>("Img0058");
		Content.Load<Texture2D>("Img0067");
		Content.Load<Texture2D>("Img0084");
		Content.Load<Texture2D>("Img0085");
		Content.Load<Texture2D>("Img0086");
		Content.Load<Texture2D>("Img0089");
		Content.Load<Texture2D>("Img0091");
		Content.Load<Texture2D>("Img0092");
		Content.Load<Texture2D>("Img0093");
		Content.Load<Texture2D>("Img0094");
		Content.Load<Texture2D>("Img0095");
		Content.Load<Texture2D>("Img0096");
		Content.Load<Texture2D>("Img0097");
		Content.Load<Texture2D>("Img0098");
		Content.Load<Texture2D>("Img0101");
		Content.Load<Texture2D>("Img0105");
		Content.Load<Texture2D>("Img0107");
		Content.Load<Texture2D>("Img0108");
		Content.Load<Texture2D>("Img0109");
		Content.Load<Texture2D>("Img0110");
		Content.Load<Texture2D>("Img0111");
		Content.Load<Texture2D>("Img0124");
		Content.Load<Texture2D>("Img0125");
		Content.Load<Texture2D>("Img0126");
		Content.Load<Texture2D>("Img0129");
		Content.Load<Texture2D>("Img0136");
		Content.Load<Texture2D>("Img0137");
		Content.Load<Texture2D>("Img0172");
		Content.Load<Texture2D>("Img0173");
		Content.Load<Texture2D>("Img0174");
		Content.Load<Texture2D>("Img0178");
		Content.Load<Texture2D>("Img0179");
		Content.Load<Texture2D>("Img0181");
		Content.Load<Texture2D>("Img0182");
		Content.Load<Texture2D>("Img0183");
		Content.Load<Texture2D>("Img0193");
		Content.Load<Texture2D>("Img0194");
		Content.Load<Texture2D>("Img0195");
		Content.Load<Texture2D>("Img0196");
		Content.Load<Texture2D>("Img0197");
		Content.Load<Texture2D>("Img0198");
		Content.Load<Texture2D>("Img0199");
		Content.Load<Texture2D>("Img0200");
		Content.Load<Texture2D>("Img0201");
		Content.Load<Texture2D>("Img0202");
		Content.Load<Texture2D>("Img0203");
		Content.Load<Texture2D>("Img0204");
		Content.Load<Texture2D>("Img0205");
		Content.Load<Texture2D>("Img0206");
		Content.Load<Texture2D>("Img0207");
		Content.Load<Texture2D>("Img0208");
		Content.Load<Texture2D>("Img0209");
		Content.Load<Texture2D>("Img0210");
		Content.Load<Texture2D>("Img0211");
		Content.Load<Texture2D>("Img0212");
		Content.Load<Texture2D>("Img0213");
		Content.Load<Texture2D>("Img0214");
		Content.Load<Texture2D>("Img0215");
		Content.Load<Texture2D>("Img0216");
		Content.Load<Texture2D>("Img0217");
		Content.Load<Texture2D>("Img0218");
		Content.Load<Texture2D>("Img0219");
		Content.Load<Texture2D>("Img0220");
		Content.Load<Texture2D>("Img0221");
		Content.Load<Texture2D>("Img0222");
		Content.Load<Texture2D>("Img0223");
		Content.Load<Texture2D>("Img0224");
		Content.Load<Texture2D>("Img0225");
		Active = true;
	}

	static LoadUpfront()
	{
		Active = false;
		gatherAssetNames = "";
		Content = null;
		loadingSwirl = null;
		progressTime = 0f;
	}

	public static void LoadContentUpfront(ContentManager content, GraphicsDevice graphics)
	{
		Content = content;
		graphicsDevice = graphics;
		spriteBatch = new SpriteBatch(graphics);
		CreateLoadingProgressTexture();
		Thread thread = new Thread(LoadDataUpfront);
		thread.Start();
	}

	private static void CreateLoadingProgressTexture()
	{
		loadingSwirl = new Texture2D(graphicsDevice, 1, 1, mipMap: false, SurfaceFormat.Color);
		Color[] data = new Color[1]
		{
			new Color(1f, 1f, 1f, 1f)
		};
		loadingSwirl.SetData(data);
	}

	public static void DrawLoadingScreen()
	{
		graphicsDevice.Clear(Color.Black);
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		DrawSwirl();
		spriteBatch.End();
		progressTime += 0.1f;
		if (progressTime > (float)Math.PI * 2f)
		{
			progressTime -= (float)Math.PI * 2f;
		}
	}

	private static void DrawSwirl()
	{
		float num = 20f;
		float num2 = 10f;
		float num3 = 1152f - num - num2 * 0.5f;
		float num4 = 648f - num - num2 * 0.5f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0.3f;
		int num8 = 15;
		Color color = default(Color);
		float num9 = 0f;
		for (int i = 0; i < num8; i++)
		{
			num5 = num3 + (float)Math.Cos(progressTime + num7 * (float)i) * num;
			num6 = num4 + (float)Math.Sin(progressTime + num7 * (float)i) * num;
			num9 = (float)(i + 1) / (float)num8;
			color = new Color(num9, num9, num9, num9);
			spriteBatch.Draw(loadingSwirl, new Vector2(num5, num6), null, color, 0f, new Vector2(0.5f, 0.5f), num2, SpriteEffects.None, 0f);
		}
	}

	public static void BuildLoadInfo_NewLineForNewFrame()
	{
	}

	public static void BuildLoadInfo_Texture2D(string dataToLoad)
	{
	}

	public static void BuildLoadInfo_Song(string dataToLoad)
	{
	}

	public static void BuildLoadInfo_SoundEffect(string dataToLoad)
	{
	}
}
