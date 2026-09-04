using System;
using System.Collections.Generic;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XSIXNARuntime;

namespace Infinity;

public class AnaglyphRender : IDisposable
{
	private RenderTarget2D anagliphLeft;

	private RenderTarget2D anagliphRight;

	private readonly Rectangle[][][] xpolFields;

	private Random random;

	private SpriteBatch spriteBatch;

	public Game Game { get; private set; }

	public AnaglyphSettings Settings { get; private set; }

	public DrawMode Mode { get; set; }

	public event Action<GameTime> DrawInitializeLeft;

	public event Action<GameTime> DrawInitializeRight;

	public event Action<GameTime> DrawFinishedLeft;

	public event Action<GameTime> DrawFinishedRight;

	public event Action<GameTime> DrawScene;

	public AnaglyphRender(Game game, AnaglyphSettings settings)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		random = new Random();
		base._002Ector();
		Game = game;
		Settings = settings;
		spriteBatch = new SpriteBatch(game.GraphicsDevice);
		GraphicsDevice graphicsDevice = game.GraphicsDevice;
		Viewport viewport = graphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = graphicsDevice.Viewport;
		int height = ((Viewport)(ref viewport2)).Height;
		DisplayMode displayMode = graphicsDevice.DisplayMode;
		anagliphLeft = new RenderTarget2D(graphicsDevice, width, height, 1, ((DisplayMode)(ref displayMode)).Format, graphicsDevice.PresentationParameters.MultiSampleType, graphicsDevice.PresentationParameters.MultiSampleQuality);
		Viewport viewport3 = graphicsDevice.Viewport;
		int width2 = ((Viewport)(ref viewport3)).Width;
		Viewport viewport4 = graphicsDevice.Viewport;
		int height2 = ((Viewport)(ref viewport4)).Height;
		DisplayMode displayMode2 = graphicsDevice.DisplayMode;
		anagliphRight = new RenderTarget2D(graphicsDevice, width2, height2, 1, ((DisplayMode)(ref displayMode2)).Format, graphicsDevice.PresentationParameters.MultiSampleType, graphicsDevice.PresentationParameters.MultiSampleQuality);
		List<Rectangle[][]> list = new List<Rectangle[][]>();
		Rectangle item = default(Rectangle);
		for (int i = 0; i < 2; i++)
		{
			List<Rectangle[]> list2 = new List<Rectangle[]>();
			List<Rectangle> list3 = new List<Rectangle>();
			int num = i;
			while (true)
			{
				int num2 = num;
				Viewport viewport5 = game.GraphicsDevice.Viewport;
				if (num2 >= ((Viewport)(ref viewport5)).Height)
				{
					break;
				}
				int num3 = num;
				Viewport viewport6 = game.GraphicsDevice.Viewport;
				((Rectangle)(ref item))._002Ector(0, num3, ((Viewport)(ref viewport6)).Width, 1);
				list3.Add(item);
				if (list3.Count >= 255)
				{
					list2.Add(list3.ToArray());
					list3.Clear();
				}
				num += 2;
			}
			if (list3.Count > 0)
			{
				list2.Add(list3.ToArray());
				list3.Clear();
			}
			list.Add(list2.ToArray());
		}
		xpolFields = list.ToArray();
	}

	public void Dispose()
	{
		spriteBatch.Dispose();
		((RenderTarget)anagliphLeft).Dispose();
		((RenderTarget)anagliphRight).Dispose();
	}

	public void Draw(GameTime gameTime, XSISASContainer SASData)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDevice graphicsDevice = Game.GraphicsDevice;
		graphicsDevice.Clear(Color.Gray);
		graphicsDevice.RenderState.AlphaBlendEnable = false;
		graphicsDevice.RenderState.DepthBufferEnable = true;
		graphicsDevice.RenderState.DepthBufferWriteEnable = true;
		graphicsDevice.RenderState.CullMode = (CullMode)3;
		Action<GameTime>[] array = new Action<GameTime>[2] { DrawInitializeLeft, DrawInitializeRight };
		RenderTarget2D[] array2 = (RenderTarget2D[])(object)new RenderTarget2D[2] { anagliphLeft, anagliphRight };
		Action<GameTime>[] array3 = new Action<GameTime>[2] { DrawFinishedLeft, DrawFinishedRight };
		for (int i = 0; i < 2; i++)
		{
			if (array[i] != null)
			{
				array[i](gameTime);
			}
			if (DrawScene != null)
			{
				graphicsDevice.SetRenderTarget(0, array2[i]);
				graphicsDevice.Clear(Settings.BackColor);
				DrawScene(gameTime);
				int num = 0;
				while (Mode == DrawMode.LineByLine && num < xpolFields[i].Length)
				{
					Rectangle[] array4 = xpolFields[i][num];
					graphicsDevice.Clear((ClearOptions)1, Color.TransparentBlack, 1f, 0, array4);
					num++;
				}
				graphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
			}
			if (array3[i] != null)
			{
				array3[i](gameTime);
			}
		}
		DrawFinalize();
	}

	private void DrawFinalize()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Action[] array = new Action[4] { DrawFinalizeNormal, DrawFinalizeNormal, DrawFinalizeSideBySide, DrawFinalizeLineByLine };
		Game.GraphicsDevice.Clear(Color.Black);
		array[(int)Mode]();
	}

	private void DrawFinalizeNormal()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		_ = Game.GraphicsDevice;
		Texture2D[] array = (Texture2D[])(object)new Texture2D[2]
		{
			anagliphLeft.GetTexture(),
			anagliphRight.GetTexture()
		};
		Color[] array2 = (Color[])(object)new Color[2] { Settings.LeftColor, Settings.RightColor };
		spriteBatch.Begin((SpriteBlendMode)2);
		for (int i = 0; i < 2; i++)
		{
			spriteBatch.Draw(array[i], Vector2.Zero, array2[i]);
		}
		spriteBatch.End();
	}

	private void DrawFinalizeLineByLine()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Texture2D[] array = (Texture2D[])(object)new Texture2D[2]
		{
			anagliphLeft.GetTexture(),
			anagliphRight.GetTexture()
		};
		spriteBatch.Begin((SpriteBlendMode)2);
		for (int i = 0; i < array.Length; i++)
		{
			spriteBatch.Draw(array[i], Vector2.Zero, Color.White);
		}
		spriteBatch.End();
	}

	private void DrawFinalizeSideBySide()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		int backBufferWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
		int backBufferHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
		Texture2D[] array = (Texture2D[])(object)new Texture2D[2]
		{
			anagliphLeft.GetTexture(),
			anagliphRight.GetTexture()
		};
		Rectangle[] array2 = (Rectangle[])(object)new Rectangle[2]
		{
			new Rectangle(0, 0, backBufferWidth >> 1, backBufferHeight),
			new Rectangle(backBufferWidth >> 1, 0, backBufferWidth >> 1, backBufferHeight)
		};
		spriteBatch.Begin((SpriteBlendMode)2);
		for (int i = 0; i < array.Length; i++)
		{
			spriteBatch.Draw(array[i], array2[i], Color.White);
		}
		spriteBatch.End();
	}
}
