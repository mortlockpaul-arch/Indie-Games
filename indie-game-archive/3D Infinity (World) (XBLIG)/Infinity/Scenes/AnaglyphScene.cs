using System;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using XSIXNARuntime;
using XnaLibrary;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class AnaglyphScene : GameScene
{
	protected Vector3 cameraPosition;

	protected Vector3 cameraInterest;

	protected Matrix LeftView;

	protected Matrix RightView;

	private Vector2 shake;

	private Vector2 shakeValue;

	protected float shakerReduction;

	protected AnaglyphRender anaglyphRender;

	protected Random random = new Random();

	public XSISASContainer SASData => Global.SASData;

	public AnaglyphSettings AnaglyphSettings => Global.SaveData.AnaglyphSettings;

	public CameraSettings Camera => AnaglyphSettings.CameraSettings[Global.SaveData.DrawModeIndex];

	public PadVibrationComponent PadVibration => (PadVibrationComponent)base.Game.Services.GetService(typeof(PadVibrationComponent));

	public AnaglyphScene(Game game)
		: base(game)
	{
		base.update += Update;
	}

	public override void Initialize()
	{
		InitializeCamera();
		shakerReduction = 0.9f;
		anaglyphRender = new AnaglyphRender(base.Game, Global.SaveData.AnaglyphSettings);
		anaglyphRender.DrawInitializeLeft += DrawInitializeLeft;
		anaglyphRender.DrawFinishedLeft += DrawFinishedLeft;
		anaglyphRender.DrawInitializeRight += DrawInitializeRight;
		anaglyphRender.DrawFinishedRight += DrawFinishedRight;
		anaglyphRender.DrawScene += DrawScene;
		SetDrawMode(Global.SaveData.DrawModeIndex);
		base.Initialize();
	}

	public void InitializeCamera()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		cameraPosition = Camera.Position;
		cameraInterest = Camera.Interest;
	}

	public override void Dispose()
	{
		anaglyphRender.Dispose();
		base.Dispose();
	}

	private void Update(object sender, GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (InputState.IsPush(base.Input[Global.CurrentPlayer].Buttons.RightStick))
		{
			Global.SaveData.DrawModeIndex = (Global.SaveData.DrawModeIndex + 1) % 3;
			SetDrawMode(Global.SaveData.DrawModeIndex);
		}
	}

	protected void UpdateShaker(GameTime gameTime)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		shakeValue.X = (float)random.NextDouble() * shake.X;
		shakeValue.Y = (float)random.NextDouble() * shake.Y;
		shake *= shakerReduction;
	}

	protected virtual void DrawInitializeLeft(GameTime gameTime)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		float distance = Camera.Distance;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(distance + shakeValue.X, shakeValue.Y);
		SetAnaglyphCamera(val.X, val.Y);
		LeftView = SASData.View;
	}

	protected virtual void DrawFinishedLeft(GameTime gameTime)
	{
		float distance = Camera.Distance;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(distance + shakeValue.X, shakeValue.Y);
		XSISASCamera.Pan(0f - val.X, 0f - val.Y, ref cameraPosition, ref cameraInterest);
	}

	protected virtual void DrawInitializeRight(GameTime gameTime)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		float distance = Camera.Distance;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(0f - distance + shakeValue.X, shakeValue.Y);
		SetAnaglyphCamera(val.X, val.Y);
		RightView = SASData.View;
	}

	protected virtual void DrawFinishedRight(GameTime gameTime)
	{
		float distance = Camera.Distance;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(0f - distance + shakeValue.X, shakeValue.Y);
		XSISASCamera.Pan(0f - val.X, 0f - val.Y, ref cameraPosition, ref cameraInterest);
	}

	protected virtual void DrawScene(GameTime gameTime)
	{
	}

	protected virtual void SetAnaglyphCamera(float panX, float panY)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		XSISASCamera.Pan(panX, panY, ref cameraPosition, ref cameraInterest);
		SASData.Camera.Position.X = cameraPosition.X;
		SASData.Camera.Position.Y = cameraPosition.Y;
		SASData.Camera.Position.Z = cameraPosition.Z;
		SASData.View = Matrix.CreateLookAt(cameraPosition, Camera.Interest, Vector3.Up);
		SASData.ComputeViewAndProjection();
	}

	protected void SetShaker(float x, float y)
	{
		shake.X = x;
		shake.Y = y;
	}

	protected void SetShaker()
	{
		SetShaker(0.1f, 0.1f);
	}

	protected void SetDrawMode(int mode)
	{
		if (mode >= 0 && mode < 3)
		{
			SetDrawMode((DrawMode)mode);
		}
	}

	protected void SetDrawMode(DrawMode mode)
	{
		anaglyphRender.Mode = mode;
		InitializeCamera();
	}
}
