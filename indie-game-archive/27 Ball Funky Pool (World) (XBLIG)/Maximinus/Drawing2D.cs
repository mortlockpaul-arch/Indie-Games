using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Drawing2D : RoundLineManager
{
	public class LineOrthoSB
	{
		protected Color colorStart;

		private Vector2 p0;

		private Vector2 p1;

		private float width;

		protected Rectangle rectangle;

		public Vector2 P0
		{
			get
			{
				return p0;
			}
			set
			{
				p0 = value;
				UpdateData();
			}
		}

		public Vector2 P1
		{
			get
			{
				return p1;
			}
			set
			{
				p1 = value;
				UpdateData();
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				UpdateData();
			}
		}

		public LineOrthoSB(Vector2 P0, Vector2 P1, float width, Color color)
		{
			this.width = width;
			colorStart = color;
			p0 = P0;
			p1 = P1;
			UpdateData();
		}

		public virtual void render(Drawing2D draw2D, byte A)
		{
			draw2D.SpriteBatch.Draw(draw2D.BlankTex, rectangle, new Color(colorStart.R, colorStart.G, colorStart.B, A));
		}

		public virtual void render(Drawing2D draw2D)
		{
			render(draw2D, byte.MaxValue);
		}

		protected virtual void UpdateData()
		{
			bool flag;
			if (P0.X == P1.X)
			{
				flag = true;
			}
			else
			{
				if (P0.Y != P1.Y)
				{
					throw new Exception("not orthogonal");
				}
				flag = false;
			}
			Vector2 vector;
			Vector2 vector2;
			if (!flag)
			{
				vector = new Vector2(P1.X - P0.X, 2f * width);
				vector2 = new Vector2(P0.X, P0.Y - width);
				if (vector.X < 0f)
				{
					vector.X *= -1f;
					vector2.X = P1.X;
				}
			}
			else
			{
				vector = new Vector2(2f * width, P1.Y - P0.Y);
				vector2 = new Vector2(P0.X - width, P0.Y);
				if (vector.Y < 0f)
				{
					vector.Y *= -1f;
					vector2.Y = P1.Y;
				}
			}
			rectangle = new Rectangle((int)vector2.X, (int)vector2.Y, (int)vector.X, (int)vector.Y);
		}
	}

	public class LineSB : LineOrthoSB
	{
		private Color colorEnd;

		private float length;

		private Texture2D tex;

		private Color[] texData;

		private Rectangle destRectangle;

		private float rotation;

		private int lengthInt => (int)length;

		public LineSB(Vector2 P0, Vector2 P1, float width, Color colorStart, Color colorEnd)
			: base(P0, P1, width, colorStart)
		{
			this.colorEnd = colorEnd;
			UpdateData();
		}

		protected override void UpdateData()
		{
			length = (int)Vector2.Distance(base.P0, base.P1);
			if (length != 0f)
			{
				tex = null;
				texData = new Color[lengthInt];
				ChangeColors(colorStart, colorEnd);
				float degrees = (float)MyMath.AngleDegBetweenVectors(Vector2.Normalize(base.P1 - base.P0), new Vector2(1f, 0f));
				rotation = MathHelper.ToRadians(degrees);
				destRectangle = new Rectangle((int)base.P0.X, (int)(base.P0.Y - base.Width / 2f), lengthInt, (int)base.Width);
			}
		}

		public override void render(Drawing2D draw2D)
		{
			if (length != 0f)
			{
				if (tex == null)
				{
					tex = new Texture2D(draw2D.Device, lengthInt, 1);
					tex.SetData(texData);
				}
				draw2D.SpriteBatch.Draw(tex, destRectangle, null, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0f);
			}
		}

		public static void DrawList(List<LineSB> list, Drawing2D draw2D)
		{
			foreach (LineSB item in list)
			{
				item.render(draw2D);
			}
		}

		public void ChangeColors(Color c0, Color c1)
		{
			colorStart = c0;
			colorEnd = c1;
			for (int i = 0; i < lengthInt; i++)
			{
				ref Color reference = ref texData[i];
				reference = Utils.LerpColor(colorStart, colorEnd, (float)(i + 1) / (float)lengthInt);
			}
			tex = null;
		}
	}

	public class RectangleOutlineOrtho
	{
		private Vector2 start;

		private Vector2 size;

		public Color color;

		private int width;

		private List<LineOrthoSB> list;

		public int Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
				UpdateLines();
			}
		}

		public Vector2 Start
		{
			get
			{
				return start;
			}
			set
			{
				start = value;
				UpdateLines();
			}
		}

		public Vector2 Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				UpdateLines();
			}
		}

		public RectangleOutlineOrtho(Vector2 start, Vector2 size, int width, Color color)
		{
			this.width = width;
			this.color = color;
			this.start = start;
			this.size = size;
			UpdateLines();
		}

		public void render(Drawing2D draw2D, byte A)
		{
			foreach (LineOrthoSB item in list)
			{
				item.render(draw2D, A);
			}
		}

		private void UpdateLines()
		{
			list = new List<LineOrthoSB>();
			Vector2 vector = start;
			vector.X -= width;
			Vector2 vector2 = vector;
			vector2.X += size.X;
			list.Add(new LineOrthoSB(vector, vector2, width, color));
			vector2.X += width;
			vector2.Y -= width;
			Vector2 vector3 = vector2;
			vector3.Y += size.Y;
			list.Add(new LineOrthoSB(vector2, vector3, width, color));
			vector3.X += width;
			vector3.Y += width;
			vector3.Y--;
			Vector2 vector4 = vector3;
			vector4.X -= size.X;
			list.Add(new LineOrthoSB(vector3, vector4, width, color));
			vector4.X -= width;
			vector4.Y += width;
			vector4.Y++;
			Vector2 p = vector4;
			p.Y -= size.Y;
			list.Add(new LineOrthoSB(vector4, p, width, color));
		}
	}

	private Matrix viewProjMatrixForRoundLine;

	private bool useCustomMatrixForRoundLine;

	private Vector2 screenSize;

	private SpriteBatch spriteBatch;

	private SpriteFont font720;

	private SpriteFont font1080;

	private SpriteFont fontTitle720;

	private SpriteFont fontTitle1080;

	private GraphicsDevice device;

	private Texture2D blankTex;

	private PrimitiveBatch primitiveBatch;

	private static Color[] blankTexData = new Color[1] { Color.White };

	public static float fieldOfViewDeg = 45f;

	public static float fieldOfViewRad = MathHelper.ToRadians(fieldOfViewDeg);

	public Vector2 ScreenSize => screenSize;

	public Point ScreenSizePoint => new Point((int)screenSize.X, (int)screenSize.Y);

	public SpriteBatch SpriteBatch => spriteBatch;

	public GraphicsDevice Device => device;

	public SpriteFont Font
	{
		get
		{
			if (MaximinusGame.BackBufferSize == MaximinusGame.BackBufferSizeValue.HD_720)
			{
				return font720;
			}
			return font1080;
		}
	}

	public SpriteFont FontTitle
	{
		get
		{
			if (MaximinusGame.BackBufferSize == MaximinusGame.BackBufferSizeValue.HD_720)
			{
				return fontTitle720;
			}
			return fontTitle1080;
		}
	}

	public Texture2D BlankTex => blankTex;

	public static void SetAndClearRender(GraphicsDevice dev, RenderTarget2D r, Color c)
	{
		dev.SetRenderTarget(r);
		dev.Clear(c);
	}

	public static Vector2 RoundFloatPos(Vector2 floatPos)
	{
		return new Vector2((int)floatPos.X, (int)floatPos.Y);
	}

	public Drawing2D(GraphicsDevice d, ContentManager c, SpriteBatch spriteBatch, SpriteFont defaultSpriteFont720, SpriteFont defaultSpriteFont1080)
		: this(d, c, spriteBatch, defaultSpriteFont720, defaultSpriteFont1080, defaultSpriteFont720, defaultSpriteFont1080)
	{
	}

	public Drawing2D(GraphicsDevice d, ContentManager c, SpriteBatch spriteBatch, SpriteFont defaultSpriteFont720, SpriteFont defaultSpriteFont1080, SpriteFont fontTitle720, SpriteFont fontTitle1080)
		: this(d, c, spriteBatch, defaultSpriteFont720, defaultSpriteFont1080, fontTitle720, fontTitle1080, ViewProjMatrixForRoundLine_2DGame(new Vector2(d.Viewport.Width, d.Viewport.Height)), useCustomMatrixForRoundLine: false)
	{
	}

	public Drawing2D(GraphicsDevice d, ContentManager c, SpriteBatch spriteBatch, SpriteFont defaultSpriteFont720, SpriteFont defaultSpriteFont1080, SpriteFont fontTitle720, SpriteFont fontTitle1080, Matrix viewProjMatrixForRoundLine)
		: this(d, c, spriteBatch, defaultSpriteFont720, defaultSpriteFont1080, fontTitle720, fontTitle1080, viewProjMatrixForRoundLine, useCustomMatrixForRoundLine: true)
	{
	}

	public Drawing2D(GraphicsDevice d, ContentManager c, SpriteBatch spriteBatch, SpriteFont defaultSpriteFont720, SpriteFont defaultSpriteFont1080, SpriteFont fontTitle720, SpriteFont fontTitle1080, Matrix viewProjMatrixForRoundLine, bool useCustomMatrixForRoundLine)
	{
		Init(d, c);
		screenSize = new Vector2(d.Viewport.Width, d.Viewport.Height);
		device = d;
		this.spriteBatch = spriteBatch;
		font720 = defaultSpriteFont720;
		font1080 = defaultSpriteFont1080;
		this.fontTitle720 = ((fontTitle720 == null) ? font720 : fontTitle720);
		this.fontTitle1080 = ((fontTitle1080 == null) ? font1080 : fontTitle1080);
		this.viewProjMatrixForRoundLine = viewProjMatrixForRoundLine;
		this.useCustomMatrixForRoundLine = useCustomMatrixForRoundLine;
		blankTex = new Texture2D(d, 1, 1);
		blankTex.SetData(blankTexData);
		primitiveBatch = new PrimitiveBatch(d);
	}

	public void PrepareFor3D()
	{
		PrepareFor3D(device);
	}

	public static void PrepareFor3D(GraphicsDevice thisDevice)
	{
		thisDevice.BlendState = BlendState.AlphaBlend;
		thisDevice.DepthStencilState = DepthStencilState.Default;
		thisDevice.SamplerStates[0] = SamplerState.LinearWrap;
	}

	public void DrawLines(List<Vector2> vertex, Color color)
	{
		primitiveBatch.Begin(PrimitiveType.LineList);
		foreach (Vector2 item in vertex)
		{
			primitiveBatch.AddVertex(item, color);
		}
		primitiveBatch.End();
	}

	public void DrawLine(Vector2 p0, Vector2 p1, Color c0, Color c1)
	{
		primitiveBatch.Begin(PrimitiveType.LineList);
		primitiveBatch.AddVertex(p0, c0);
		primitiveBatch.AddVertex(p1, c1);
		primitiveBatch.End();
	}

	public void Draw(RoundLine r, float radius, Color color, GameTime gameTime, string techniqueName)
	{
		Draw(useCustomMatrixForRoundLine ? r : ChangePositionFor2D(r), radius, color, viewProjMatrixForRoundLine, (float)gameTime.TotalGameTime.TotalSeconds, techniqueName);
	}

	public void Draw(RoundLine r, float radius, Color color, GameTime gameTime)
	{
		Draw(r, radius, color, gameTime, "Standard");
	}

	public void Draw(RoundLine[] r, float radius, Color color, GameTime gameTime)
	{
		foreach (RoundLine r2 in r)
		{
			Draw(r2, radius, color, gameTime);
		}
	}

	public void Draw(RoundLine[] r, float radius, Color color, GameTime gameTime, string techniqueName)
	{
		foreach (RoundLine r2 in r)
		{
			Draw(r2, radius, color, gameTime, techniqueName);
		}
	}

	public void Draw(List<RoundLine> r, float radius, Color color, GameTime gameTime)
	{
		Draw(r, radius, color, gameTime, "Standard");
	}

	public void Draw(List<RoundLine> r, float radius, Color color, GameTime gameTime, string techniqueName)
	{
		Draw(useCustomMatrixForRoundLine ? r : ChangePositionFor2D(r), radius, color, viewProjMatrixForRoundLine, (float)gameTime.TotalGameTime.TotalSeconds, techniqueName);
	}

	public Vector3 ChangePositionFor2D(Vector2 v)
	{
		return new Vector3(v.X, ScreenSize.Y - v.Y, 0f);
	}

	private RoundLine ChangePositionFor2D(RoundLine r)
	{
		return new RoundLine(new Vector2(r.P0.X, screenSize.Y - r.P0.Y), new Vector2(r.P1.X, screenSize.Y - r.P1.Y));
	}

	private List<RoundLine> ChangePositionFor2D(List<RoundLine> r)
	{
		List<RoundLine> list = new List<RoundLine>();
		foreach (RoundLine item in r)
		{
			list.Add(ChangePositionFor2D(item));
		}
		return list;
	}

	public static Matrix ViewMatrixForRoundLine_2DGame(Vector2 screenSize)
	{
		return ViewMatrixForRoundLine_2DGame(screenSize, UpVectorIsNegative: false);
	}

	public static Matrix ViewMatrixForRoundLine_2DGame(Vector2 screenSize, bool UpVectorIsNegative)
	{
		Vector3 cameraUpVector = new Vector3(0f, 1f, 0f);
		if (UpVectorIsNegative)
		{
			cameraUpVector *= -1f;
		}
		float x = screenSize.X;
		float y = screenSize.Y;
		Vector3 vector = new Vector3(x * 0.5f, y * 0.5f, 0f);
		float z = vector.X / (float)Math.Tan(fieldOfViewRad / 2f);
		Vector3 cameraPosition = vector + new Vector3(0f, 0f, z);
		return Matrix.CreateLookAt(cameraPosition, vector, cameraUpVector);
	}

	public static Matrix ProjMatrixForRoundLine_2DGame(Vector2 screenSize)
	{
		float x = screenSize.X;
		float y = screenSize.Y;
		Vector3 vector = new Vector3(x * 0.5f, y * 0.5f, 0f);
		float num = vector.X / (float)Math.Tan(fieldOfViewRad / 2f);
		_ = vector + new Vector3(0f, 0f, num);
		return Matrix.CreatePerspective(x, y, num, num + 10f);
	}

	public static Matrix ViewProjMatrixForRoundLine_2DGame(Vector2 screenSize)
	{
		return ViewMatrixForRoundLine_2DGame(screenSize) * ProjMatrixForRoundLine_2DGame(screenSize);
	}

	public static Matrix ViewMatrixFor3DObjects(Vector2 screenSize)
	{
		float num = screenSize.X / screenSize.Y;
		return Matrix.CreateLookAt(new Vector3(screenSize.X / 2f, screenSize.Y / 2f, 1546.6666f / num), new Vector3(screenSize.X / 2f, screenSize.Y / 2f, 0f), Vector3.Up);
	}

	public static Matrix ProjMatrixFor3DObjects(Vector2 screenSize)
	{
		float aspectRatio = screenSize.X / screenSize.Y;
		return Matrix.CreatePerspectiveFieldOfView(fieldOfViewRad, aspectRatio, 1f, 1600f);
	}

	public void DrawString(string text, Vector2 pos, Color color, SpriteFont otherFont)
	{
		spriteBatch.DrawString(otherFont, text, pos, color);
	}

	public void DrawString(string text, Vector2 pos, Color color)
	{
		spriteBatch.DrawString(Font, text, pos, color);
	}

	public void DrawString(string text, Vector2 pos, Color color, float scale)
	{
		spriteBatch.DrawString(Font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
	}

	public void DrawStringWithSelectEffect(string text, SpriteFont overrideFont, Vector2 pos, Color selectColor, Color secondColor, float scaleMax, float selectionTransition)
	{
		Vector2 vector = pos + Font.MeasureString(text) / 2f;
		Vector2 origin = Font.MeasureString(text) / 2f;
		if (text == "Add CPU players ?" || text == "How many opponents ?")
		{
			vector.X += ScreenSize.X * 0.03f;
		}
		spriteBatch.DrawString(overrideFont, text, vector, Utils.LerpColor(secondColor, selectColor, selectionTransition), 0f, origin, MathHelper.Lerp(1f, scaleMax, selectionTransition), SpriteEffects.None, 0f);
		Vector2 vector2 = Vector2.One * 3f;
		if (MaximinusGame.BackBufferSize == MaximinusGame.BackBufferSizeValue.HD_1080)
		{
			vector2 *= 3f;
			vector2 /= 2f;
		}
		spriteBatch.DrawString(overrideFont, text, vector + vector2, Utils.LerpColor(Utils.ColorTransparentWhite, secondColor, selectionTransition), 0f, origin, MathHelper.Lerp(1f, scaleMax, selectionTransition), SpriteEffects.None, 0.5f);
	}

	public void DrawStringWithSelectEffect(string text, Vector2 pos, Color selectColor, Color secondColor, float scaleMax, float selectionTransition)
	{
		DrawStringWithSelectEffect(text, Font, pos, selectColor, secondColor, scaleMax, selectionTransition);
	}
}
