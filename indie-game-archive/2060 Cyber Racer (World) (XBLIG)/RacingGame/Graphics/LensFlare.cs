using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Helpers;

namespace RacingGame.Graphics;

public class LensFlare : IDisposable
{
	protected struct FlareData
	{
		public int type;

		public float position;

		public float scale;

		public Color color;

		public FlareData(int setType, float setPosition, float setScale, Color setColor)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			type = setType;
			position = setPosition;
			scale = setScale;
			color = setColor;
		}
	}

	protected const int SunFlareType = 0;

	protected const int GlowFlareType = 1;

	protected const int LensFlareType = 2;

	protected const int StreaksType = 3;

	protected const int RingType = 4;

	protected const int HaloType = 5;

	protected const int CircleType = 6;

	protected const int NumberOfFlareTypes = 7;

	public static Vector3 DefaultSunPos;

	public static Vector3 DefaultLightPos;

	private static Vector3 lensOrigin3D;

	private static int ScreenFlareSize;

	protected Texture[] flareTextures;

	private string[] flareTextureNames;

	protected FlareData[] flareTypes;

	private float sunIntensity;

	public static Vector3 Origin3D
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return lensOrigin3D;
		}
		set
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			lensOrigin3D = value;
		}
	}

	public static Vector3 RotateSun(float rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 defaultSunPos = DefaultSunPos;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)Math.Cos(rotation), (float)Math.Sin(rotation));
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector((float)Math.Sin(rotation), 0f - (float)Math.Cos(rotation));
		return new Vector3((0f - val.X) * defaultSunPos.X - val2.X * defaultSunPos.Z, defaultSunPos.Y, (0f - val.Y) * defaultSunPos.X - val2.Y * defaultSunPos.Z);
	}

	private void LoadTextures()
	{
		for (int i = 0; i < 7; i++)
		{
			flareTextures[i] = new Texture(flareTextureNames[i]);
		}
	}

	public LensFlare(Vector3 setLensOrigin3D)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		flareTextures = new Texture[7];
		flareTextureNames = new string[7] { "Sun", "Glow", "Lens", "Streaks", "Ring", "Halo", "Circle" };
		flareTypes = new FlareData[17]
		{
			new FlareData(6, 1.2f, 0.55f, new Color((byte)175, (byte)175, byte.MaxValue, (byte)20)),
			new FlareData(0, 1f, 0.9f, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)),
			new FlareData(3, 1f, 1.8f, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)128)),
			new FlareData(1, 1f, 2.6f, new Color(byte.MaxValue, byte.MaxValue, (byte)200, (byte)100)),
			new FlareData(6, 0.5f, 0.12f, new Color((byte)60, (byte)60, (byte)180, (byte)35)),
			new FlareData(6, 0.45f, 0.46f, new Color((byte)100, (byte)100, (byte)200, (byte)60)),
			new FlareData(6, 0.4f, 0.17f, new Color((byte)120, (byte)120, (byte)220, (byte)40)),
			new FlareData(4, 0.15f, 0.2f, new Color((byte)60, (byte)60, byte.MaxValue, (byte)100)),
			new FlareData(2, -0.5f, 0.2f, new Color(byte.MaxValue, (byte)60, (byte)60, (byte)130)),
			new FlareData(2, -0.15f, 0.15f, new Color(byte.MaxValue, (byte)60, (byte)60, (byte)90)),
			new FlareData(5, -0.3f, 0.6f, new Color((byte)60, (byte)60, byte.MaxValue, (byte)180)),
			new FlareData(5, -0.4f, 0.2f, new Color((byte)220, (byte)80, (byte)80, (byte)98)),
			new FlareData(6, -0.5f, 0.1f, new Color((byte)220, (byte)80, (byte)80, (byte)85)),
			new FlareData(5, -0.6f, 0.5f, new Color((byte)60, (byte)60, byte.MaxValue, (byte)80)),
			new FlareData(4, -0.8f, 0.3f, new Color((byte)90, (byte)60, byte.MaxValue, (byte)110)),
			new FlareData(5, -0.95f, 0.5f, new Color((byte)60, (byte)60, byte.MaxValue, (byte)120)),
			new FlareData(6, -1f, 0.15f, new Color((byte)60, (byte)60, byte.MaxValue, (byte)85))
		};
		base._002Ector();
		lensOrigin3D = setLensOrigin3D;
		LoadTextures();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		for (int i = 0; i < 7; i++)
		{
			if (flareTextures[i] != null)
			{
				flareTextures[i].Dispose();
			}
		}
	}

	public void Render(Color sunColor)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		ScreenFlareSize = 250 * BaseGame.Width / 1024;
		Vector3 point = lensOrigin3D + BaseGame.CameraPos;
		if (!BaseGame.IsInFrontOfCamera(point))
		{
			return;
		}
		Point val = BaseGame.Convert3DPointTo2D(point);
		float num = 0.75f;
		sunIntensity = num * 0.1f + sunIntensity * 0.9f;
		if (sunIntensity < 0.01f)
		{
			return;
		}
		int width = BaseGame.Width;
		int height = BaseGame.Height;
		Point val2 = default(Point);
		((Point)(ref val2))._002Ector(width / 2, height / 2);
		Point val3 = default(Point);
		((Point)(ref val3))._002Ector(val2.X - val.X, val2.Y - val.Y);
		float num2 = 1f;
		float num3 = Math.Abs(Math.Max(val3.X, val3.Y));
		if (num3 > (float)height / 1.75f)
		{
			num3 -= (float)height / 1.75f;
			if (num3 > (float)height / 1.75f)
			{
				return;
			}
			num2 = 1f - num3 / ((float)height / 1.75f);
			if (num2 > 1f)
			{
				num2 = 1f;
			}
		}
		num2 *= sunIntensity * sunIntensity;
		FlareData[] array = flareTypes;
		for (int i = 0; i < array.Length; i++)
		{
			FlareData flareData = array[i];
			int num4 = (int)((float)ScreenFlareSize * flareData.scale);
			Texture obj = flareTextures[flareData.type];
			Rectangle rect = new Rectangle((int)((float)val2.X - (float)val3.X * flareData.position - (float)(num4 / 2)), (int)((float)val2.Y - (float)val3.Y * flareData.position - (float)(num4 / 2)), num4, num4);
			Rectangle gfxRectangle = flareTextures[flareData.type].GfxRectangle;
			Color col = ColorHelper.MultiplyColors(sunColor, flareData.color);
			Color color = flareData.color;
			obj.RenderOnScreen(rect, gfxRectangle, ColorHelper.ApplyAlphaToColor(col, (float)(int)((Color)(ref color)).A / 255f * ((flareData.type == 0 || flareData.type == 1) ? sunIntensity : num2)), (SpriteBlendMode)2);
		}
	}

	static LensFlare()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		DefaultSunPos = new Vector3(2500f, -22500f, 15000f);
		DefaultLightPos = new Vector3(8500f, -7250f, 15000f);
		ScreenFlareSize = 225;
	}
}
