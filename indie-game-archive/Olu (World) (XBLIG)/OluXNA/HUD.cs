using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class HUD
{
	public VertexBuffer fxBuffer;

	public int numLines;

	private VertexPositionColor[] target;

	private VertexPositionColor[] yelTarget;

	private VertexPositionColor[] targetMask;

	private VertexPositionColor[] yelTargetMask;

	public float scale;

	private Texture2D powerBar;

	private Texture2D blueBar;

	private Texture2D orangeBar;

	public Texture2D blackTex;

	public ModelWrapper backBox;

	public ModelWrapper wireBox;

	public float rotAmount;

	private StretchTex glowEffect;

	public static float textScale = (float)BaseGame.HEIGHT / 1080f;

	private float offX1;

	private float offX2;

	private float offY1;

	private float offY2;

	private float border;

	public Vector3[] wirePos;

	public Vector3[] solidPos;

	private float oWidth;

	private float oHeight;

	private float glowBorder;

	private Matrix simpleView;

	private SpriteFont _HUDfont;

	private SpriteFont _bigHUDfont;

	private SpriteFont _Controllerfont;

	private Dictionary<Buttons, string> _keyMap;

	public SpriteFont HUDfont => _HUDfont;

	public SpriteFont BigHUDfont => _bigHUDfont;

	public SpriteFont ControllerFont => _Controllerfont;

	public Dictionary<Buttons, string> KeyMap => _keyMap;

	public HUD()
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		offX1 = 90f;
		offX2 = 130f;
		offY1 = 170f;
		offY2 = 240f;
		border = 6f;
		base._002Ector();
		offX1 = 50f;
		offX2 = 90f;
		offY2 = 220f;
		border = 6f;
		offX1 = offX1 / 1280f * (float)BaseGame.WIDTH;
		offY1 = offY1 / 1280f * (float)BaseGame.WIDTH;
		offX2 = offX2 / 1280f * (float)BaseGame.WIDTH;
		offY2 = offY2 / 1280f * (float)BaseGame.WIDTH;
		List<VertexPositionColor> list = new List<VertexPositionColor>();
		int c_WIDTH = BaseGame.C_WIDTH;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)c_WIDTH, (float)c_WIDTH, 0f);
		list.Add(new VertexPositionColor(val, Color.White));
		for (int i = 0; i < 4; i++)
		{
			val = Vector3.Transform(val, Matrix.CreateRotationZ((float)Math.PI / 2f));
			list.Add(new VertexPositionColor(val, Color.White));
		}
		target = list.ToArray();
		yelTarget = list.ToArray();
		Color lightGray = Color.LightGray;
		((Color)(ref lightGray)).A = 128;
		list.Clear();
		((Vector3)(ref val))._002Ector((float)c_WIDTH, (float)c_WIDTH, 0f);
		for (int j = 0; j < 2; j++)
		{
			for (int k = 0; k < 3; k++)
			{
				list.Add(new VertexPositionColor(Vector3.Transform(val, Matrix.CreateRotationZ((float)k * (float)Math.PI / 2f) * Matrix.CreateRotationZ((float)Math.PI * (float)j)), lightGray));
			}
		}
		targetMask = list.ToArray();
		yelTargetMask = list.ToArray();
		scale = 1f;
		rotAmount = 0f;
		wirePos = (Vector3[])(object)new Vector3[3];
		solidPos = (Vector3[])(object)new Vector3[3];
		simpleView = Matrix.CreateLookAt(Vector3.Zero, new Vector3(0f, 0f, 1f), Vector3.Up);
		_keyMap = new Dictionary<Buttons, string>();
		_keyMap.Add((Buttons)64, " ");
		_keyMap.Add((Buttons)1, "!");
		_keyMap.Add((Buttons)128, "\"");
		_keyMap.Add((Buttons)32, "#");
		_keyMap.Add((Buttons)16, "%");
		_keyMap.Add((Buttons)16384, "&");
		_keyMap.Add((Buttons)4096, "'");
		_keyMap.Add((Buttons)32768, "(");
		_keyMap.Add((Buttons)8192, ")");
		_keyMap.Add((Buttons)512, "*");
		_keyMap.Add((Buttons)4194304, "+");
		_keyMap.Add((Buttons)8388608, ",");
		_keyMap.Add((Buttons)256, "-");
	}

	public void LoadGraphics()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Expected O, but got Unknown
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		List<VertexPositionColor> nodes = new List<VertexPositionColor>();
		Color col = default(Color);
		((Color)(ref col))._002Ector(new Vector4(1f, 1f, 1f, 0.6f));
		oWidth = 35f * (float)BaseGame.WIDTH / 480f;
		oHeight = 16f * (float)BaseGame.WIDTH / 480f;
		glowBorder = 12f * (float)BaseGame.WIDTH / 480f;
		border = border * (float)BaseGame.WIDTH / 480f;
		DrawBox(ref nodes, offX1, offX2, offY1, (float)BaseGame.HEIGHT - offY2, col);
		DrawBox(ref nodes, (float)BaseGame.WIDTH - offX2, (float)BaseGame.WIDTH - offX1, offY1, (float)BaseGame.HEIGHT - offY2, col);
		for (int i = 0; i < wirePos.Length; i++)
		{
			ref Vector3 reference = ref wirePos[i];
			reference = new Vector3(offX1, (float)BaseGame.HEIGHT - offY2 + (float)(i + 1) * border + oHeight * (float)i, 0f);
			DrawBox(ref nodes, wirePos[i].X, wirePos[i].X + oWidth, wirePos[i].Y, wirePos[i].Y + oHeight, col);
			ref Vector3 reference2 = ref wirePos[i];
			reference2.X += border / 2f;
			ref Vector3 reference3 = ref wirePos[i];
			reference3.Y += border / 2f;
		}
		for (int j = 0; j < solidPos.Length; j++)
		{
			ref Vector3 reference4 = ref solidPos[j];
			reference4 = new Vector3((float)BaseGame.WIDTH - offX1 - oWidth, (float)BaseGame.HEIGHT - offY2 + (float)(j + 1) * border + oHeight * (float)j, 0f);
			DrawBox(ref nodes, solidPos[j].X, solidPos[j].X + oWidth, solidPos[j].Y, solidPos[j].Y + oHeight, col);
			ref Vector3 reference5 = ref solidPos[j];
			reference5.X += border / 2f;
			ref Vector3 reference6 = ref solidPos[j];
			reference6.Y += border / 2f;
		}
		offX1 += border / 2f;
		offX2 -= border / 2f;
		offY1 += border / 2f;
		offY2 += border / 2f;
		oHeight -= border;
		oWidth -= border;
		DrawBox(ref nodes, offX1, offX2, offY1, (float)BaseGame.HEIGHT - offY2, col);
		DrawBox(ref nodes, (float)BaseGame.WIDTH - offX2, (float)BaseGame.WIDTH - offX1, offY1, (float)BaseGame.HEIGHT - offY2, col);
		for (int k = 0; k < wirePos.Length; k++)
		{
			DrawBox(ref nodes, wirePos[k].X, wirePos[k].X + oWidth, wirePos[k].Y, wirePos[k].Y + oHeight, col);
		}
		for (int l = 0; l < solidPos.Length; l++)
		{
			DrawBox(ref nodes, solidPos[l].X, solidPos[l].X + oWidth, solidPos[l].Y, solidPos[l].Y + oHeight, col);
		}
		numLines = nodes.Count / 2;
		fxBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, nodes.Count * VertexPositionColor.SizeInBytes, (BufferUsage)8);
		fxBuffer.SetData<VertexPositionColor>(nodes.ToArray());
		_HUDfont = BaseGame.Get().content.Load<SpriteFont>(BaseGame.fontName);
		_bigHUDfont = BaseGame.Get().content.Load<SpriteFont>(BaseGame.bigFontName);
		_Controllerfont = BaseGame.Get().content.Load<SpriteFont>("Content/ButtonImages/ControllerSpriteFont");
		powerBar = BaseGame.Get().content.Load<Texture2D>("Content/power");
		blueBar = BaseGame.Get().content.Load<Texture2D>("Content/powerBlue");
		orangeBar = BaseGame.Get().content.Load<Texture2D>("Content/powerOrange");
		glowEffect = new StretchTex();
		glowEffect.Initialize(19, 22, 19, 22, "Content\\glowTex");
		backBox = BaseGame.Get().models.GetModel("Content\\FinishBox\\FinishBox", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(backBox.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(backBox.epc, "DiffuseColor", (object)new Vector3(0.5f, 0.5f, 0.5f));
		BaseGame.SetAllEPCs(backBox.epc, "DirLight0Direction", (object)new Vector3(0.5f, -0.5f, 0.5f));
		BaseGame.SetAllEPCs(backBox.epc, "EmissiveColor", (object)new Vector3(0f, 0f, 0f));
		BaseGame.SetAllEPCs(backBox.epc, "Alpha", 0.5f);
		wireBox = BaseGame.Get().models.GetModel("Content\\FinishBox\\FinishBoxWire", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(wireBox.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(wireBox.epc, "EmissiveColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.SetAllEPCs(wireBox.epc, "Alpha", 0.3f);
		BaseGame.SetAllEPCs(wireBox.epc, "xGlow", true);
		blackTex = BaseGame.Get().content.Load<Texture2D>("Content\\black");
	}

	public void LoadLevelColor(Vector3 levelColor)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < yelTarget.Length; i++)
		{
			yelTarget[i].Color = new Color(new Vector4(((Color)(ref target[i].Color)).ToVector3() * levelColor, (float)(int)((Color)(ref target[i].Color)).A / 255f));
		}
		for (int j = 0; j < yelTargetMask.Length; j++)
		{
			yelTargetMask[j].Color = new Color(new Vector4(((Color)(ref targetMask[j].Color)).ToVector3() * levelColor, (float)(int)((Color)(ref targetMask[j].Color)).A / 255f));
		}
	}

	public void DrawBox(ref List<VertexPositionColor> nodes, float x1, float x2, float y1, float y2, Color col)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		nodes.Add(new VertexPositionColor(new Vector3(x1, y1, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x1, y2, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x1, y2, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x2, y2, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x2, y2, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x2, y1, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x2, y1, 0f), col));
		nodes.Add(new VertexPositionColor(new Vector3(x1, y1, 0f), col));
	}

	public void Update(GameTime gametime)
	{
		rotAmount += 20f * (float)gametime.ElapsedGameTime.TotalSeconds;
		if (rotAmount > 360f)
		{
			rotAmount -= 360f;
		}
		if (scale > 1f)
		{
			scale -= (float)(2.0 * gametime.ElapsedGameTime.TotalSeconds);
		}
	}

	public void DrawInBack(GameTime gametime)
	{
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		BaseGame.Get().combineEffect.Begin();
		BaseGame.Get().combineEffect.CurrentTechnique.Passes[0].Begin();
		for (int num = BaseGame.Get().targetFX.Count - 1; num >= 0; num--)
		{
			BaseGame.Get().targetFX[num].DrawInBack();
		}
		BaseGame.Get().spriteBatch.End();
		BaseGame.Get().combineEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().combineEffect.End();
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
	}

	public void Draw_FrontCube(GameTime gametime)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Invalid comparison between Unknown and I4
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Invalid comparison between Unknown and I4
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().fogEffect.Parameters["xFogEnable"].SetValue(false);
		BaseGame.Get().fogEffect.Parameters["xView"].SetValue(simpleView);
		BaseGame.Get().fogEffect.Parameters["xVProj"].SetValue(simpleView * BaseGame.Get().projectionMatrix);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = BaseGame.Get().fillMode;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationY(MathHelper.ToRadians(45f)) * Matrix.CreateRotationX(MathHelper.ToRadians(45f)) * Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)) * Matrix.CreateScale(0.025f) * Matrix.CreateTranslation(new Vector3((0f - BaseGame.Get().weaponMode) * 6f + 3f, 5f, 10f)));
		if ((int)BaseGame.Get().fillMode == 2)
		{
			BaseGame.Get().DrawModel(ref wireBox);
		}
		else
		{
			BaseGame.Get().DrawModel(ref backBox);
		}
		BaseGame.Get().matStack.PopMatrix();
		if ((int)BaseGame.Get().fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().fogEffect.Parameters["xFogEnable"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["xView"].SetValue(BaseGame.Get().viewMatrix);
		BaseGame.Get().fogEffect.Parameters["xVProj"].SetValue(BaseGame.Get().viewMatrix * BaseGame.Get().projectionMatrix);
	}

	public void Draw(GameTime gametime)
	{
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)3, (SaveStateMode)0);
		for (int num = BaseGame.Get().targetFX.Count - 1; num >= 0; num--)
		{
			BaseGame.Get().targetFX[num].Draw();
		}
		DrawTarget();
		if (!BaseGame.Get().EasyMode)
		{
			write_score();
		}
		write_textflow();
		write_weapon();
		DrawPowerBar();
		DrawFade();
		BaseGame.Get().spriteBatch.End();
		BaseGame.Get().tdColl.Draw(gametime);
		BaseGame.Get().tdColl2.Draw(gametime);
	}

	private void DrawFade()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.Draw(blackTex, new Rectangle(0, 0, BaseGame.WIDTH, BaseGame.HEIGHT), (Rectangle?)null, new Color(1f, 1f, 1f, BaseGame.Get().channels[8]), 0f, Vector2.Zero, (SpriteEffects)0, 0.01f);
	}

	public void DrawPowerBar()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(fxBuffer, 0, VertexPositionColor.SizeInBytes);
		BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)2, 0, numLines);
		BaseGame.Get().spriteBatch.End();
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		for (int i = 0; i < 2; i++)
		{
			int num = (int)((float)BaseGame.Get().powerScore[i] / (float)BaseGame.Get().maxPower[i] * (float)(BaseGame.HEIGHT - (int)offY2 - (int)offY1));
			BaseGame.Get().spriteBatch.Draw(powerBar, new Rectangle((int)(offX1 + (float)i * ((float)BaseGame.WIDTH - offX2 - offX1)), BaseGame.HEIGHT - (int)offY2, num, (int)offX2 - (int)offX1), (Rectangle?)null, Color.White, MathHelper.ToRadians(-90f), Vector2.Zero, (SpriteEffects)0, 0.5f);
		}
		for (int j = 0; j < BaseGame.Get().powerAmounts[0] && j < 3; j++)
		{
			glowEffect.Draw(new Vector2(wirePos[j].X - glowBorder, wirePos[j].Y - glowBorder), new Vector2(glowBorder, glowBorder), new Vector2(glowBorder + oWidth, glowBorder + oHeight), new Vector2(wirePos[j].X + oWidth + glowBorder, wirePos[j].Y + oHeight + glowBorder), new Color(new Vector4(0.7f, 0.7f, 1f, BaseGame.Get().flashMod)));
			BaseGame.Get().spriteBatch.Draw(blueBar, new Rectangle((int)wirePos[j].X, (int)wirePos[j].Y, (int)oWidth, (int)oHeight), Color.White);
		}
		for (int k = 0; k < BaseGame.Get().powerAmounts[1] && k < 3; k++)
		{
			glowEffect.Draw(new Vector2(solidPos[k].X - glowBorder, solidPos[k].Y - glowBorder), new Vector2(glowBorder, glowBorder), new Vector2(glowBorder + oWidth, glowBorder + oHeight), new Vector2(solidPos[k].X + oWidth + glowBorder, solidPos[k].Y + oHeight + glowBorder), new Color(new Vector4(1f, 0.8f, 0.6f, BaseGame.Get().flashMod)));
			BaseGame.Get().spriteBatch.Draw(orangeBar, new Rectangle((int)solidPos[k].X, (int)solidPos[k].Y, (int)oWidth, (int)oHeight), Color.White);
		}
		BaseGame.Get().spriteBatch.End();
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)3, (SaveStateMode)0);
	}

	public void DrawTarget()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().flatStack.PushMatrix();
		BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateTranslation(BaseGame.Get().cursorPos));
		BaseGame.Get().flatStack.ApplyMatrix(Matrix.CreateScale(scale));
		if (BaseGame.Get().selector)
		{
			BaseGame.Get().graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)4, targetMask, 0, 2);
			BaseGame.Get().graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)3, target, 0, target.Length - 1);
		}
		else
		{
			BaseGame.Get().graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)4, yelTargetMask, 0, 2);
			BaseGame.Get().graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>((PrimitiveType)3, yelTarget, 0, target.Length - 1);
		}
		if (BaseGame.Get().numTargeted > 0)
		{
			string text;
			float num;
			if (BaseGame.Get().numTargeted > 7)
			{
				text = "FIRE";
				num = 1.2f * textScale * scale;
			}
			else
			{
				text = BaseGame.Get().numTargeted.ToString();
				num = 3f * textScale * scale;
			}
			BaseGame.Get().spriteBatch.DrawString(HUDfont, text, new Vector2(BaseGame.Get().cursorPos.X, BaseGame.Get().cursorPos.Y), Color.White, 0f, BaseGame.Get().hud.HUDfont.MeasureString(text) / 2f, num, (SpriteEffects)0, 0f);
		}
		BaseGame.Get().flatStack.PopMatrix();
	}

	private void write_textflow()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.DrawString(_HUDfont, BaseGame.Get().textFlow, new Vector2((float)(BaseGame.S_WIDTH / 10), (float)(BaseGame.S_HEIGHT / 10)), new Color(new Vector4(1f, 1f, 1f, 0.6f)), 0f, Vector2.Zero, textScale * 0.1f, (SpriteEffects)0, 0f);
	}

	public void write_score()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.DrawString(_HUDfont, BaseGame.Get().score.ToString("000000"), new Vector2((float)(BaseGame.S_WIDTH * 8 / 10), (float)(BaseGame.S_HEIGHT / 10)), Color.White, 0f, Vector2.Zero, textScale, (SpriteEffects)0, 0f);
	}

	public void write_weapon()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		string text = (((int)BaseGame.Get().fillMode == 2) ? "d1g1tal" : "ANAL0G");
		BaseGame.Get().spriteBatch.DrawString(_HUDfont, text, new Vector2((float)(BaseGame.S_WIDTH / 2), (float)(BaseGame.S_HEIGHT / 10)), Color.White, 0f, _HUDfont.MeasureString(text) / 2f, textScale, (SpriteEffects)0, 0f);
	}
}
