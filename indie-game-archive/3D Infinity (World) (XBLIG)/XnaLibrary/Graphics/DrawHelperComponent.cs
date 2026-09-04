using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Graphics;

public class DrawHelperComponent : GameComponent
{
	protected Vector4 EdgeColor;

	protected Vector4 ShadowColor;

	protected readonly Vector2[] EdgeTable;

	protected readonly Vector2[] ShadowTable;

	private readonly string[] CommandCharacters;

	private readonly Dictionary<string, string> CommandMap;

	private readonly Dictionary<string, float> CommandScale;

	private readonly Vector2 safeRightDown;

	private Texture2D texture;

	private Rectangle[] quadLines;

	private Color workColor;

	private Vector2 workVector2;

	private StringBuilder workString;

	public DrawHelperComponent(Game game)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		Color val = new Color((byte)0, (byte)0, (byte)0, (byte)128);
		EdgeColor = ((Color)(ref val)).ToVector4();
		Color val2 = new Color((byte)20, (byte)40, (byte)40, (byte)192);
		ShadowColor = ((Color)(ref val2)).ToVector4();
		EdgeTable = (Vector2[])(object)new Vector2[4]
		{
			new Vector2(0f, -1f),
			new Vector2(0f, 1f),
			new Vector2(-1f, 0f),
			new Vector2(1f, 0f)
		};
		ShadowTable = (Vector2[])(object)new Vector2[2]
		{
			new Vector2(1f, 1f),
			new Vector2(2f, 2f)
		};
		safeRightDown = new Vector2(1280f, 720f) * 0.95f;
		quadLines = (Rectangle[])(object)new Rectangle[4];
		workColor = Color.White;
		workString = new StringBuilder();
		((GameComponent)this)._002Ector(game);
		CommandMap = new Dictionary<string, string>();
		CommandMap.Add("[LS]", " ");
		CommandMap.Add("[DP]", "!");
		CommandMap.Add("[RS]", "\"");
		CommandMap.Add("[BACK]", "#");
		CommandMap.Add("[GUIDE]", "$");
		CommandMap.Add("[START]", "%");
		CommandMap.Add("[X]", "&");
		CommandMap.Add("[A]", "'");
		CommandMap.Add("[Y]", "(");
		CommandMap.Add("[B]", ")");
		CommandMap.Add("[RB]", "*");
		CommandMap.Add("[RT]", "+");
		CommandMap.Add("[LT]", ",");
		CommandMap.Add("[LB]", "-");
		CommandScale = new Dictionary<string, float>();
		CommandScale.Add("[LS]", 0.25f);
		CommandScale.Add("[DP]", 0.2f);
		CommandScale.Add("[RS]", 0.25f);
		CommandScale.Add("[BACK]", 0.5f);
		CommandScale.Add("[GUIDE]", 0.25f);
		CommandScale.Add("[START]", 0.5f);
		CommandScale.Add("[X]", 0.5f);
		CommandScale.Add("[A]", 0.5f);
		CommandScale.Add("[Y]", 0.5f);
		CommandScale.Add("[B]", 0.5f);
		CommandScale.Add("[RB]", 0.25f);
		CommandScale.Add("[RT]", 0.25f);
		CommandScale.Add("[LT]", 0.25f);
		CommandScale.Add("[LB]", 0.25f);
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, string> item in CommandMap)
		{
			list.Add(item.Key);
		}
		CommandCharacters = list.ToArray();
	}

	public override void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		texture = new Texture2D(((GameComponent)this).Game.GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		texture.SetData<Color>((Color[])(object)new Color[1] { Color.White });
		((GameComponent)this).Initialize();
	}

	protected override void Dispose(bool disposing)
	{
		if (texture != null)
		{
			((GraphicsResource)texture).Dispose();
			texture = null;
		}
		((GameComponent)this).Dispose(disposing);
	}

	public override void Update(GameTime gameTime)
	{
		((GameComponent)this).Update(gameTime);
	}

	public void DrawFillRect(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Draw(texture, rectangle, color);
	}

	public void DrawLineRect(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		SetRectangle(ref quadLines[0], rectangle.X, rectangle.Y, rectangle.Width, 1);
		SetRectangle(ref quadLines[1], rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1);
		SetRectangle(ref quadLines[2], rectangle.X, rectangle.Y, 1, rectangle.Height);
		SetRectangle(ref quadLines[3], rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height);
		Rectangle[] array = quadLines;
		foreach (Rectangle val in array)
		{
			spriteBatch.Draw(texture, val, color);
		}
	}

	public void DrawLine(SpriteBatch spriteBatch, Vector2 from, Vector2 to, Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector2.Distance(from, to) + 0.5f;
		float num2 = to.X - from.X;
		float num3 = to.Y - from.Y;
		float num4 = (float)Math.Atan2(num3, num2);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(num, 1f);
		spriteBatch.Draw(texture, from, (Rectangle?)null, color, num4, Vector2.Zero, val, (SpriteEffects)0, 0f);
	}

	public void DrawString(SpriteBatch spriteBatch, SpriteFont spriteFont, StringBuilder text, ref Vector2 position, ref Vector4 color)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		((Color)(ref workColor)).R = (byte)(color.X * 255f);
		((Color)(ref workColor)).G = (byte)(color.Y * 255f);
		((Color)(ref workColor)).B = (byte)(color.Z * 255f);
		((Color)(ref workColor)).A = (byte)(color.W * 255f);
		workVector2.X = (int)position.X;
		workVector2.Y = (int)position.Y;
		spriteBatch.DrawString(spriteFont, text, workVector2, workColor);
	}

	public void DrawEdgeString(SpriteBatch spriteBatch, SpriteFont font, StringBuilder text, Vector2 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		Vector4 color = ((Color)(ref white)).ToVector4();
		DrawEdgeString(spriteBatch, font, text, position, color, Vector2.Zero, 0f);
	}

	public void DrawEdgeString(SpriteBatch spriteBatch, SpriteFont font, StringBuilder text, Vector2 position, Vector4 color)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		DrawEdgeString(spriteBatch, font, text, position, color, Vector2.Zero, 0f);
	}

	public void DrawEdgeString(SpriteBatch spriteBatch, SpriteFont font, StringBuilder text, Vector2 position, Vector4 color, Vector2 origin, float layerDepth)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		SetColor(ref ShadowColor, ref workColor, color.W);
		Vector2[] shadowTable = ShadowTable;
		foreach (Vector2 val in shadowTable)
		{
			spriteBatch.DrawString(font, text, position + val, workColor, 0f, origin, 1f, (SpriteEffects)0, layerDepth);
		}
		SetColor(ref EdgeColor, ref workColor, color.W);
		Vector2[] edgeTable = EdgeTable;
		foreach (Vector2 val2 in edgeTable)
		{
			spriteBatch.DrawString(font, text, position + val2, workColor, 0f, origin, 1f, (SpriteEffects)0, layerDepth);
		}
		SetColor(ref color, ref workColor);
		spriteBatch.DrawString(font, text, position, workColor, 0f, origin, 1f, (SpriteEffects)0, layerDepth);
	}

	public void DrawString(SpriteBatch spriteBatch, SpriteFont spriteFont, SpriteFont buttonFont, string text, ref Vector2 position, ref Vector4 color)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[CommandCharacters.Length];
		Vector2 position2 = position;
		int num = 0;
		while (num < text.Length)
		{
			int num2 = int.MaxValue;
			int num3 = -1;
			for (int i = 0; i < CommandCharacters.Length; i++)
			{
				array[i] = text.IndexOf(CommandCharacters[i], num);
				if (array[i] >= 0 && num2 > array[i])
				{
					num2 = array[i];
					num3 = i;
				}
			}
			if (num3 < 0)
			{
				string text2 = text.Substring(num);
				num += text2.Length;
				DrawCommandString(spriteBatch, spriteFont, text2, ref position2, ref color);
				continue;
			}
			int num4 = num2 - num;
			if (num4 > 0)
			{
				string text3 = text.Substring(num, num4);
				DrawCommandString(spriteBatch, spriteFont, text3, ref position2, ref color);
			}
			num += num4 + CommandCharacters[num3].Length;
			DrawButtonString(spriteBatch, buttonFont, CommandCharacters[num3], ref position2, ref color);
		}
	}

	private void DrawCommandString(SpriteBatch spriteBatch, SpriteFont spriteFont, string text, ref Vector2 position, ref Vector4 color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		Vector2 val = spriteFont.MeasureString(text);
		Vector2 origin = default(Vector2);
		((Vector2)(ref origin))._002Ector(0f, val.Y * 0.5f);
		SetColor(ref color, ref workColor);
		workString.Remove(0, workString.Length);
		workString.Append(text);
		DrawEdgeString(spriteBatch, spriteFont, workString, position, color, origin, 0f);
		position.X += val.X * num;
	}

	private void DrawButtonString(SpriteBatch spriteBatch, SpriteFont buttonFont, string commandText, ref Vector2 position, ref Vector4 color)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		string text = CommandMap[commandText];
		float num = CommandScale[commandText];
		Vector2 val = buttonFont.MeasureString(text);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(0f, (float)buttonFont.LineSpacing * 0.5f);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(0f, -6f);
		SetColor(ref color, ref workColor);
		spriteBatch.DrawString(buttonFont, text, position + val3, workColor, 0f, val2, num, (SpriteEffects)0, 0f);
		position.X += val.X * num;
	}

	public void DrawString(SpriteBatch spriteBatch, SpriteFont spriteFont, SpriteFont buttonFont, string text)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = MeasureString(spriteBatch, spriteFont, buttonFont, text);
		Vector2 position = safeRightDown;
		position.X -= val.X;
		position.Y -= (float)(spriteFont.LineSpacing / 2);
		Color white = Color.White;
		Vector4 color = ((Color)(ref white)).ToVector4();
		DrawString(spriteBatch, spriteFont, buttonFont, text, ref position, ref color);
	}

	public Vector2 MeasureString(SpriteBatch spriteBatch, SpriteFont spriteFont, SpriteFont buttonFont, string text)
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		Vector2 result = default(Vector2);
		((Vector2)(ref result))._002Ector(0f, (float)spriteFont.LineSpacing);
		int[] array = new int[CommandCharacters.Length];
		int num = 0;
		while (num < text.Length)
		{
			int num2 = int.MaxValue;
			int num3 = -1;
			for (int i = 0; i < CommandCharacters.Length; i++)
			{
				array[i] = text.IndexOf(CommandCharacters[i], num);
				if (array[i] >= 0 && num2 > array[i])
				{
					num2 = array[i];
					num3 = i;
				}
			}
			if (num3 < 0)
			{
				string text2 = text.Substring(num);
				num += text2.Length;
				Vector2 val = spriteFont.MeasureString(text2);
				result.X += val.X;
				continue;
			}
			int num4 = num2 - num;
			if (num4 > 0)
			{
				string text3 = text.Substring(num, num4);
				Vector2 val2 = spriteFont.MeasureString(text3);
				result.X += val2.X;
			}
			num += num4 + CommandCharacters[num3].Length;
			string key = CommandCharacters[num3];
			string text4 = CommandMap[key];
			float num5 = CommandScale[key];
			Vector2 val3 = buttonFont.MeasureString(text4);
			val3 *= num5;
			result.X += val3.X;
		}
		return result;
	}

	public void SetRectangle(ref Rectangle rectangle, int x, int y, int width, int height)
	{
		rectangle.X = x;
		rectangle.Y = y;
		rectangle.Width = width;
		rectangle.Height = height;
	}

	private void SetColor(ref Vector4 srcColor, ref Color destColor)
	{
		((Color)(ref destColor)).R = (byte)(srcColor.X * 255f);
		((Color)(ref destColor)).G = (byte)(srcColor.Y * 255f);
		((Color)(ref destColor)).B = (byte)(srcColor.Z * 255f);
		((Color)(ref destColor)).A = (byte)(srcColor.W * 255f);
	}

	private void SetColor(ref Vector4 srcColor, ref Color destColor, float alpha)
	{
		((Color)(ref destColor)).R = (byte)(srcColor.X * 255f);
		((Color)(ref destColor)).G = (byte)(srcColor.Y * 255f);
		((Color)(ref destColor)).B = (byte)(srcColor.Z * 255f);
		((Color)(ref destColor)).A = (byte)(alpha * 255f);
	}

	public void SetRenderState(SpriteBlendMode mode)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDevice graphicsDevice = ((GameComponent)this).Game.GraphicsDevice;
		if ((int)mode == 1)
		{
			graphicsDevice.RenderState.AlphaBlendEnable = true;
			graphicsDevice.RenderState.AlphaBlendOperation = (BlendFunction)1;
			graphicsDevice.RenderState.SourceBlend = (Blend)5;
			graphicsDevice.RenderState.DestinationBlend = (Blend)6;
			graphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
			graphicsDevice.RenderState.AlphaTestEnable = true;
			graphicsDevice.RenderState.AlphaFunction = (CompareFunction)5;
			graphicsDevice.RenderState.ReferenceAlpha = 0;
		}
		else if ((int)mode == 2)
		{
			graphicsDevice.RenderState.AlphaBlendEnable = true;
			graphicsDevice.RenderState.AlphaBlendOperation = (BlendFunction)1;
			graphicsDevice.RenderState.SourceBlend = (Blend)5;
			graphicsDevice.RenderState.DestinationBlend = (Blend)7;
			graphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
			graphicsDevice.RenderState.AlphaTestEnable = true;
			graphicsDevice.RenderState.AlphaFunction = (CompareFunction)5;
			graphicsDevice.RenderState.ReferenceAlpha = 0;
		}
		else if ((int)mode == 0)
		{
			graphicsDevice.RenderState.AlphaBlendEnable = false;
			graphicsDevice.RenderState.AlphaTestEnable = false;
		}
	}
}
