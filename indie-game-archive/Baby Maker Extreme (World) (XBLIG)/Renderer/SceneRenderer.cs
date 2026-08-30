using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public static class SceneRenderer
{
	public struct textData
	{
		public fonts f;

		public string s;

		public StringBuilder b;

		public Vector2 pos;

		public Color c;

		public Vector2 size;

		public bool isScreenSpace;
	}

	public struct PointDef
	{
		public Vector2 pos;

		public Color c;
	}

	private static BasicEffect sm_defaultEffect;

	private static int sm_iEffectPasses;

	private static Color sm_color;

	private static GraphicsDevice device;

	private static GraphicsDeviceManager deviceManager;

	private static SpriteBatch batch;

	private static ContentManager manager;

	private static Dictionary<int, SpriteFont> fontMap;

	private static Rectangle projectionRect;

	private static float sm_alpha;

	private static CameraManager sm_cameraManager;

	private static Random sm_rand;

	private static bool sm_bCurAdditive;

	private static VertexDeclaration texPositionColorTexDeclaration;

	private static VertexDeclaration texPositionColorDeclaration;

	private static AvatarHandler sm_avatar;

	private static List<PointDef> sm_points = new List<PointDef>();

	private static VertexPositionColorTexture[] vertices = (VertexPositionColorTexture[])(object)new VertexPositionColorTexture[600];

	private static int indexVertex;

	private static Vector2 posOffset;

	private static Matrix sm_batchMatrix;

	public static AvatarHandler Avatar
	{
		get
		{
			return sm_avatar;
		}
		set
		{
			sm_avatar = value;
		}
	}

	public static ContentManager GetContentManager()
	{
		return manager;
	}

	public static GraphicsDevice GetGraphicsDevice()
	{
		return device;
	}

	public static void SetProjectionRectSpace(Rectangle r)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		projectionRect = r;
	}

	public static Rectangle GetProjectionRect()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return projectionRect;
	}

	public static Vector2 GetScreenDim()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)deviceManager.PreferredBackBufferWidth, (float)deviceManager.PreferredBackBufferHeight);
	}

	public static Vector2 GetViewportDim()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = device.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = device.Viewport;
		return new Vector2(num, (float)((Viewport)(ref viewport2)).Height);
	}

	public static Rectangle GetScreenSafeArea()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector2 screenDim = GetScreenDim();
		Rectangle result = default(Rectangle);
		((Rectangle)(ref result))._002Ector((int)(screenDim.X * 0.1f), (int)(screenDim.Y * 0.1f), (int)(screenDim.X * 0.8f), (int)(screenDim.Y * 0.8f));
		return result;
	}

	public static Vector2 ConvertToScreenSpace(Vector2 pixelPos)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(pixelPos.X / (float)projectionRect.Width, (pixelPos.Y - (float)((Rectangle)(ref projectionRect)).Top) / (float)projectionRect.Height);
		float x = val.X;
		Viewport viewport = device.Viewport;
		float num = x * (float)((Viewport)(ref viewport)).Width;
		float y = val.Y;
		Viewport viewport2 = device.Viewport;
		Vector2 result = default(Vector2);
		((Vector2)(ref result))._002Ector(num, y * (float)((Viewport)(ref viewport2)).Height);
		return result;
	}

	public static Vector2 ConvertToProjectionSpace(Vector2 screenPos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		float x = screenPos.X;
		Viewport viewport = device.Viewport;
		float num = x / (float)((Viewport)(ref viewport)).Width;
		float y = screenPos.Y;
		Viewport viewport2 = device.Viewport;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(num, y / (float)((Viewport)(ref viewport2)).Height);
		Vector2 result = default(Vector2);
		((Vector2)(ref result))._002Ector((float)((Rectangle)(ref projectionRect)).Left + val.X * (float)projectionRect.Width, (float)((Rectangle)(ref projectionRect)).Top + val.Y * (float)projectionRect.Height);
		return result;
	}

	public static float GetStringWidth(fonts f, string s)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return fontMap[(int)f].MeasureString(s).X;
	}

	public static bool FontHasCharacters(fonts f, string s)
	{
		foreach (char value in s)
		{
			if (!fontMap[(int)f].Characters.Contains(value))
			{
				return false;
			}
		}
		return true;
	}

	public static void SetGraphicsDevice(GraphicsDevice d, GraphicsDeviceManager dm, BasicEffect b, SpriteBatch btch, ContentManager c)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		device = d;
		deviceManager = dm;
		batch = btch;
		manager = c;
		fontMap = new Dictionary<int, SpriteFont>();
		SpriteFont value = manager.Load<SpriteFont>("BasicFont");
		fontMap.Add(0, value);
		value = manager.Load<SpriteFont>("ButtonFont");
		fontMap.Add(1, value);
		value = manager.Load<SpriteFont>("ItemCountFont");
		fontMap.Add(2, value);
		value = manager.Load<SpriteFont>("CashCountFont");
		fontMap.Add(3, value);
		value = manager.Load<SpriteFont>("MiramonteOutline5");
		fontMap.Add(4, value);
		value = manager.Load<SpriteFont>("Grunge1");
		fontMap.Add(5, value);
		sm_defaultEffect = b;
		sm_iEffectPasses = ((IEnumerable<EffectPass>)((Effect)b).Techniques[0].Passes).Count();
		sm_cameraManager = new CameraManager(b);
		sm_rand = new Random();
		sm_bCurAdditive = false;
		SetBlendMode(isAdditive: true);
		texPositionColorTexDeclaration = new VertexDeclaration(device, VertexPositionColorTexture.VertexElements);
		texPositionColorDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
		sm_avatar = null;
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, float depth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DrawString(f, s, pos, c, new Vector2(1f, 1f), depth);
	}

	public static void DrawFinalString(RenderText text, Vector2 cameraPosition)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		textData textData2 = text.GetTextData();
		if (textData2.s != null)
		{
			batch.DrawString(fontMap[(int)textData2.f], textData2.s, textData2.pos - cameraPosition, textData2.c, 0f, default(Vector2), textData2.size, (SpriteEffects)0, 0f);
		}
		else
		{
			batch.DrawString(fontMap[(int)textData2.f], textData2.b, textData2.pos - cameraPosition, textData2.c, 0f, default(Vector2), textData2.size, (SpriteEffects)0, 0f);
		}
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DrawString(f, s, pos, c, size, isScreenSpace: false, depth);
	}

	public static void DrawString(fonts f, string s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float depth)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		SpriteManager.AddTextRefForDraw(new textData
		{
			f = f,
			s = s,
			pos = pos,
			c = c,
			size = size,
			isScreenSpace = isScreenSpace
		}, depth);
	}

	public static void DrawString(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, float depth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DrawString(f, s, pos, c, size, isScreenSpace: false, depth);
	}

	public static void DrawString(fonts f, StringBuilder s, Vector2 pos, Color c, Vector2 size, bool isScreenSpace, float depth)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		SpriteManager.AddTextRefForDraw(new textData
		{
			f = f,
			b = s,
			pos = pos,
			c = c,
			size = size,
			isScreenSpace = isScreenSpace
		}, depth);
	}

	public static void DrawPixelSpaceString(fonts f, string s, Vector2 pos, Color c, float depth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DrawString(f, s, pos, c, new Vector2(1f, 1f), isScreenSpace: true, depth);
	}

	public static void DrawPixelSpaceString(fonts f, StringBuilder b, Vector2 pos, Color c, float depth)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DrawString(f, b, pos, c, new Vector2(1f, 1f), isScreenSpace: true, depth);
	}

	public static void DrawRotatedRect(Vector2 pos, Vector2 size, float depth, float rotation)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		SetActiveTexture(null);
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(pos.X, pos.Y, 1f),
			new Vector3(pos.X + size.X, pos.Y, 1f),
			new Vector3(pos.X + size.X, pos.Y + size.Y, 1f),
			new Vector3(pos.X, pos.Y + size.Y, 1f)
		};
		VertexPositionColor[] array2 = (VertexPositionColor[])(object)new VertexPositionColor[5];
		sm_color = Color.White;
		device.RenderState.PointSize = 10f;
		array2[0].Position = array[0];
		array2[0].Color = sm_color;
		array2[1].Position = array[1];
		array2[1].Color = sm_color;
		array2[2].Position = array[2];
		array2[2].Color = sm_color;
		array2[3].Position = array[3];
		array2[3].Color = sm_color;
		array2[4].Position = array[0];
		array2[4].Color = sm_color;
		device.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)3, array2, 0, 4);
	}

	public static void DrawLine(Vector2 pos1, Vector2 pos2, Color c)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		SetActiveTexture(null);
		sm_defaultEffect.World = Matrix.CreateTranslation(0f, 0f, -1f);
		((Effect)sm_defaultEffect).CommitChanges();
		Vector3[] array = (Vector3[])(object)new Vector3[2]
		{
			new Vector3(pos1.X, pos1.Y, 1f),
			new Vector3(pos2.X, pos2.Y, 1f)
		};
		VertexPositionColor[] array2 = (VertexPositionColor[])(object)new VertexPositionColor[2];
		SetActiveColor(c);
		device.RenderState.PointSize = 10f;
		array2[0].Position = array[0];
		array2[0].Color = sm_color;
		array2[1].Position = array[1];
		array2[1].Color = sm_color;
		device.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)3, array2, 0, 1);
	}

	public static void DrawPoint(Vector2 pos, Color c)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		PointDef item = new PointDef
		{
			pos = pos,
			c = c
		};
		sm_points.Add(item);
	}

	public static void DrawFinalPoints()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		foreach (PointDef sm_point in sm_points)
		{
			SetActiveTexture(null);
			sm_defaultEffect.World = Matrix.CreateTranslation(0f, 0f, -1f);
			((Effect)sm_defaultEffect).CommitChanges();
			Vector3[] array = (Vector3[])(object)new Vector3[1]
			{
				new Vector3(sm_point.pos.X, sm_point.pos.Y, 1f)
			};
			VertexPositionColor[] array2 = (VertexPositionColor[])(object)new VertexPositionColor[2];
			SetActiveColor(sm_point.c);
			device.RenderState.PointSize = 10f;
			array2[0].Position = array[0];
			array2[0].Color = sm_color;
			device.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)1, array2, 0, 1);
		}
		sm_points.Clear();
	}

	public static void DrawRenderSprite(RenderSprite sprite)
	{
		SpriteManager.AddSpriteRefForDraw(sprite);
	}

	public static void IniFrame(bool b)
	{
	}

	public static Effect GetEffect()
	{
		return (Effect)(object)sm_defaultEffect;
	}

	public static int GetEffectPasses()
	{
		return sm_iEffectPasses;
	}

	public static void DrawRenderSpriteFromManager(RenderSprite sprite, bool inPass)
	{
		DrawTexturedRect(sprite, drawImmediate: false);
	}

	public static void SetActiveTexture(Texture2D texture)
	{
		sm_defaultEffect.Texture = texture;
	}

	public static void SetActiveColor(Color c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		sm_color = c;
	}

	public static void SetActiveAlpha(float alpha)
	{
		sm_alpha = alpha;
	}

	public static void SetBlendMode(bool isAdditive)
	{
		if (isAdditive && !sm_bCurAdditive)
		{
			sm_bCurAdditive = true;
			device.RenderState.AlphaBlendEnable = true;
			device.RenderState.SourceBlend = (Blend)5;
			device.RenderState.DestinationBlend = (Blend)2;
		}
		else if (!isAdditive && sm_bCurAdditive)
		{
			sm_bCurAdditive = false;
			device.RenderState.AlphaBlendEnable = true;
			device.RenderState.SourceBlend = (Blend)5;
			device.RenderState.DestinationBlend = (Blend)6;
		}
	}

	public static void ResetBatch()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		batch.End();
		batch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0, GetBatchMatrix());
	}

	public static void SetTexturedRectData()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		posOffset = sm_cameraManager.Position * sm_cameraManager.Zoom - GetScreenDim() / 2f;
	}

	public static void SetViewport(Vector2 position, Vector2 size, bool clear)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = batch.GraphicsDevice.Viewport;
		((Viewport)(ref viewport)).Width = (int)size.X;
		((Viewport)(ref viewport)).Height = (int)size.Y;
		((Viewport)(ref viewport)).X = (int)position.X;
		((Viewport)(ref viewport)).Y = (int)position.Y;
		batch.GraphicsDevice.Viewport = viewport;
		if (clear)
		{
			batch.GraphicsDevice.Clear(new Color((byte)90, (byte)103, (byte)70));
		}
	}

	public static Matrix GetBatchMatrix()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return sm_batchMatrix;
	}

	public static void DrawTexturedRect(RenderSprite sprite, bool drawImmediate)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		((Color)(ref sm_color)).A = (byte)((float)(int)((Color)(ref sm_color)).A * sm_alpha);
		Vector2 val = default(Vector2);
		Vector2 val2 = new Vector2(sprite.GetSpriteImage().Width, sprite.GetSpriteImage().Height);
		Vector2 surfaceScale = sprite.SurfaceScale;
		Vector2.Divide(ref surfaceScale, ref val2, ref val);
		SpriteEffects val3 = (SpriteEffects)0;
		if (val.X < 0f)
		{
			val.X = 0f - val.X;
			val3 = (SpriteEffects)1;
		}
		Vector2 val4 = sm_cameraManager.Position - GetViewportDim() / (sm_cameraManager.GetZoom() * 2f);
		if (sprite.MirrorPos > -800f)
		{
			val3 = (((int)val3 != 0) ? ((SpriteEffects)0) : ((SpriteEffects)1));
			Vector2 val5 = default(Vector2);
			((Vector2)(ref val5))._002Ector(2f * sprite.MirrorPos - sprite.Position.X, sprite.Position.Y);
			batch.Draw(sprite.Texture, val5 - val4, (Rectangle?)sprite.GetSpriteImage().GetPageRect(), sm_color, 0f - sprite.Rotation, new Vector2((float)sprite.GetSpriteImage().GetPageRect().Width, (float)sprite.GetSpriteImage().GetPageRect().Height) / 2f + new Vector2(0f - sprite.Origin.X, sprite.Origin.Y) / val, val, val3, 0.5f);
		}
		else
		{
			batch.Draw(sprite.Texture, sprite.Position - val4, (Rectangle?)sprite.GetSpriteImage().GetPageRect(), sm_color, sprite.Rotation, sprite.Origin / val + new Vector2((float)sprite.GetSpriteImage().GetPageRect().Width, (float)sprite.GetSpriteImage().GetPageRect().Height) / 2f, val, val3, 0.5f);
		}
		indexVertex = 0;
	}

	public static void DrawCachedPrimitives()
	{
		if (indexVertex > 0)
		{
			device.DrawUserPrimitives<VertexPositionColorTexture>((PrimitiveType)4, vertices, 0, indexVertex / 3);
		}
		indexVertex = 0;
	}

	public static void MoveCamera(Vector2 pos, float rotation, float zoom)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		sm_cameraManager.MoveCamera(pos, rotation, zoom);
		Matrix val = Matrix.CreateScale(sm_cameraManager.Zoom);
		sm_batchMatrix = val;
	}

	public static void PushCamera(Vector2 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		sm_cameraManager.PushCamera(pos);
	}

	public static void ZoomCamera(float amount)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		sm_cameraManager.ZoomCamera(amount);
		Matrix val = Matrix.CreateScale(sm_cameraManager.Zoom);
		sm_batchMatrix = val;
	}

	public static float GetCameraZoom()
	{
		return sm_cameraManager.GetZoom();
	}

	public static Vector2 GetCameraPosition()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return sm_cameraManager.Position;
	}

	public static void Update(TimeTracker gameTime)
	{
		sm_cameraManager.Update(gameTime);
	}

	public static float GetRand(float startRange, float endRange)
	{
		float num = (float)sm_rand.NextDouble();
		return num * (endRange - startRange) + startRange;
	}

	public static float GetRandSqr(float startRange, float endRange)
	{
		float num = (float)sm_rand.NextDouble();
		num *= num;
		return num * (endRange - startRange) + startRange;
	}
}
