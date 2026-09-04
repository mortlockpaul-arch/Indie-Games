using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaLibrary;
using XnaLibrary.Graphics;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class StageView : GameScene
{
	private BasicModel stage;

	private BasicModel player;

	private FreeCameraComponent freeCamera;

	private Matrix view;

	private Matrix projection;

	private float animationPosition;

	private Matrix stageAnimation;

	private RenderTarget2D anagliphLeft;

	private RenderTarget2D anagliphRight;

	private Vector3 cameraPosition;

	private Vector3 leftEyePosition;

	private Vector3 rightEyePosition;

	private Vector3 lookAtPosition;

	private Vector3 playerVector;

	private readonly Color AnaglyphCyan;

	private readonly Color AnaglyphMagenta;

	public StageView(Game game)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		cameraPosition = new Vector3(0f, 0f, 30f);
		leftEyePosition = new Vector3(-0.5f, 0f, 0f);
		rightEyePosition = new Vector3(0.5f, 0f, 0f);
		lookAtPosition = new Vector3(0f, 0f, -60f);
		playerVector = Vector3.Zero;
		AnaglyphCyan = new Color((byte)0, (byte)128, byte.MaxValue, byte.MaxValue);
		AnaglyphMagenta = new Color(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue);
		base._002Ector(game);
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		stage = new BasicModel(base.Content.Load<Model>("Models/model/stage01"));
		player = new BasicModel(base.Content.Load<Model>("Models/model/player"));
		freeCamera = new FreeCameraComponent(base.Game);
		((Collection<IGameComponent>)(object)base.Game.Components).Add((IGameComponent)(object)freeCamera);
		freeCamera.InitializeCamera(Vector3.Zero, Vector3.Forward);
		Viewport viewport = base.GraphicsDevice.Viewport;
		float aspectRatio = ((Viewport)(ref viewport)).AspectRatio;
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, aspectRatio, 1f, 10000f);
		GraphicsDevice graphicsDevice = base.GraphicsDevice;
		Viewport viewport2 = base.GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport2)).Width;
		Viewport viewport3 = base.GraphicsDevice.Viewport;
		int height = ((Viewport)(ref viewport3)).Height;
		DisplayMode displayMode = base.GraphicsDevice.DisplayMode;
		anagliphLeft = new RenderTarget2D(graphicsDevice, width, height, 1, ((DisplayMode)(ref displayMode)).Format);
		GraphicsDevice graphicsDevice2 = base.GraphicsDevice;
		Viewport viewport4 = base.GraphicsDevice.Viewport;
		int width2 = ((Viewport)(ref viewport4)).Width;
		Viewport viewport5 = base.GraphicsDevice.Viewport;
		int height2 = ((Viewport)(ref viewport5)).Height;
		DisplayMode displayMode2 = base.GraphicsDevice.DisplayMode;
		anagliphRight = new RenderTarget2D(graphicsDevice2, width2, height2, 1, ((DisplayMode)(ref displayMode2)).Format);
		base.Initialize();
	}

	public override void Dispose()
	{
		base.Content.Unload();
		((GameComponent)freeCamera).Dispose();
		base.Dispose();
	}

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (fadePhase != FadePhase.In)
		{
			if (fadePhase == FadePhase.Main)
			{
				UpdateMain(gameTime);
			}
			else
			{
				_ = fadePhase;
				_ = 2;
			}
		}
		view = freeCamera.GetViewMatrix();
		animationPosition = (float)gameTime.TotalGameTime.TotalSeconds;
		float num = animationPosition;
		Curve[] array = (Curve[])stage.Model.Tag;
		stageAnimation = new Matrix(array[0].Evaluate(num), array[1].Evaluate(num), array[2].Evaluate(num), array[3].Evaluate(num), array[4].Evaluate(num), array[5].Evaluate(num), array[6].Evaluate(num), array[7].Evaluate(num), array[8].Evaluate(num), array[9].Evaluate(num), array[10].Evaluate(num), array[11].Evaluate(num), array[12].Evaluate(num), array[13].Evaluate(num), array[14].Evaluate(num), array[15].Evaluate(num));
	}

	private void UpdateMain(GameTime gameTime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[(PlayerIndex)0];
		_ = virtualPadState.Buttons;
		_ = virtualPadState.ThumbSticks.Left;
		_ = virtualPadState.DPad;
		ref Vector3 position = ref stage.Position;
		position.Z += 0.5f;
		GamePadState val = base.Input.GamePadStates[0];
		ref Vector3 reference = ref playerVector;
		float x = reference.X;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref val)).ThumbSticks;
		reference.X = x + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 0.1f;
		ref Vector3 reference2 = ref playerVector;
		float y = reference2.Y;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref val)).ThumbSticks;
		reference2.Y = y + ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y * 0.1f;
		player.Position = Vector3.Clamp(player.Position + playerVector, new Vector3(-17.5f, -8.5f, 0f), new Vector3(17.5f, 8.5f, 0f));
		playerVector *= 0.9f;
	}

	private void SceneDraw1(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.GraphicsDevice.Clear(Color.LightSteelBlue);
		base.GraphicsDevice.RenderState.DepthBufferEnable = true;
		stage.Draw(view, projection, stageAnimation);
		player.Draw(view, projection);
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		base.GraphicsDevice.RenderState.DepthBufferEnable = true;
		Matrix val = Matrix.CreateLookAt(cameraPosition + leftEyePosition, lookAtPosition, Vector3.Up);
		base.GraphicsDevice.SetRenderTarget(0, anagliphLeft);
		base.GraphicsDevice.Clear(Color.LightSteelBlue);
		stage.Draw(val, projection, stageAnimation);
		player.Draw(val, projection);
		base.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		Matrix val2 = Matrix.CreateLookAt(cameraPosition + rightEyePosition, lookAtPosition, Vector3.Up);
		base.GraphicsDevice.SetRenderTarget(0, anagliphRight);
		base.GraphicsDevice.Clear(Color.LightSteelBlue);
		stage.Draw(val2, projection, stageAnimation);
		player.Draw(val2, projection);
		base.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		base.GraphicsDevice.Clear(Color.Black);
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(640f, 360f);
		Vector2 val4 = val3 - new Vector2(val3.X * 0.5f, 0f) - new Vector2(0f, val3.Y * 0.5f);
		Vector2 val5 = val3 + new Vector2(val3.X * 0.5f, 0f) - new Vector2(0f, val3.Y * 0.5f);
		Vector2 val6 = val3 + new Vector2(0f, val3.Y * 0.5f);
		spriteBatch.Begin();
		spriteBatch.Draw(anagliphLeft.GetTexture(), val4, (Rectangle?)null, Color.White, 0f, val3, 0.5f, (SpriteEffects)0, 0f);
		spriteBatch.Draw(anagliphRight.GetTexture(), val5, (Rectangle?)null, Color.White, 0f, val3, 0.5f, (SpriteEffects)0, 0f);
		spriteBatch.End();
		spriteBatch.Begin((SpriteBlendMode)2);
		spriteBatch.Draw(anagliphLeft.GetTexture(), val6, (Rectangle?)null, AnaglyphCyan, 0f, val3, 0.5f, (SpriteEffects)0, 0f);
		spriteBatch.Draw(anagliphRight.GetTexture(), val6, (Rectangle?)null, AnaglyphMagenta, 0f, val3, 0.5f, (SpriteEffects)0, 0f);
		spriteBatch.End();
	}
}
