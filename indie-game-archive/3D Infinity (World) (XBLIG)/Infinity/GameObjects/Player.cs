using System;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaLibrary.Input;

namespace Infinity.GameObjects;

public class Player : ModelObject
{
	private const float ShieldAlpha = 1f;

	private const float ShieldAlphaSub = 1f / 120f;

	private readonly string[] BarnerBoneNames = new string[2] { "burner_null1", "burner_null2" };

	private readonly float AvoidRotation = MathHelper.ToRadians(360f);

	private XSIModel sight;

	private XSIModel shield;

	private Vector3 MoveVelocity;

	private Vector3 Avoid;

	private float avoidRotation;

	private float avoidAmount;

	private GameSettings gameSettings;

	public bool IsHandling { get; set; }

	public bool IsDamage { get; set; }

	public float ShiledAlpha { get; set; }

	public float Thrust { get; set; }

	public Vector3[] BarnerPositions { get; private set; }

	public InputComponent Input => (InputComponent)game.Services.GetService(typeof(InputComponent));

	public event Action Vulcan;

	public event Action Missile;

	public event Action Crush;

	public Player(Game game)
		: base(game)
	{
		Initialize();
	}

	public override void Initialize()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		ContentManager content = game.Content;
		gameSettings = content.Load<GameSettings>("GameSettings");
		model = new XSIModel("Models/Models/player/player", content);
		collision = new XSIModel("Models/Models/player/player_col", content);
		shield = new XSIModel("Models/Models/player/player_shield", content);
		sight = new XSIModel("Models/Models/player/player_sight", content);
		Position = new Vector3(gameSettings.PlayerPosition.X, gameSettings.PlayerPosition.Y, 0f);
		shield.Alpha = 0f;
		shield.Play();
		base.Use = true;
		base.Enable = true;
		base.Visible = true;
		IsDamage = true;
		base.Vitality = 100;
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (IsHandling)
		{
			HandleInput();
		}
		shield.Alpha = MathHelper.Max(shield.Alpha.Value - 1f / 120f, 0f);
		shield.Update(elapsedGameTime);
		collision.Update(elapsedGameTime);
		collision.UpdateBoundingSphere(GetWorld());
		avoidAmount = Math.Min(avoidAmount + 0.05f, 1f);
		if (model != null)
		{
			if (BarnerPositions == null)
			{
				BarnerPositions = (Vector3[])(object)new Vector3[BarnerBoneNames.Length];
			}
			Matrix world = GetWorld();
			for (int i = 0; i < BarnerBoneNames.Length; i++)
			{
				string text = BarnerBoneNames[i];
				Matrix transform = model.CrosswalkModel.Bones[text].Transform;
				Matrix val = transform * world;
				ref Vector3 reference = ref BarnerPositions[i];
				reference = ((Matrix)(ref val)).Translation;
			}
		}
	}

	public void HandleInput()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected I4, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected I4, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = Input[Global.CurrentPlayer];
		VirtualPadButtons buttons = virtualPadState.Buttons;
		_ = virtualPadState.ThumbSticks.Left;
		_ = virtualPadState.DPad;
		GamePadState val = Input.GamePadStates[(int)Global.CurrentPlayer];
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref val)).ThumbSticks;
		Vector2 left = ((GamePadThumbSticks)(ref thumbSticks)).Left;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref val)).ThumbSticks;
		_ = ((GamePadThumbSticks)(ref thumbSticks2)).Right;
		KeyboardState val2 = Input.KeyboardStates[(int)Global.CurrentPlayer];
		if (((KeyboardState)(ref val2)).IsKeyDown((Keys)37))
		{
			left.X = MathHelper.Clamp(left.X - 1f, -1f, 1f);
		}
		if (((KeyboardState)(ref val2)).IsKeyDown((Keys)39))
		{
			left.X = MathHelper.Clamp(left.X + 1f, -1f, 1f);
		}
		if (((KeyboardState)(ref val2)).IsKeyDown((Keys)38))
		{
			left.Y = MathHelper.Clamp(left.Y + 1f, -1f, 1f);
		}
		if (((KeyboardState)(ref val2)).IsKeyDown((Keys)40))
		{
			left.Y = MathHelper.Clamp(left.Y - 1f, -1f, 1f);
		}
		MoveVelocity *= gameSettings.PlayerMoveSpeedReduction;
		MoveVelocity.X = MathHelper.Clamp(MoveVelocity.X + left.X * gameSettings.PlayerMoveSensitivity.X, gameSettings.PlayerMoveSpeedMin.X, gameSettings.PlayerMoveSpeedMax.X);
		MoveVelocity.Y = MathHelper.Clamp(MoveVelocity.Y + left.Y * gameSettings.PlayerMoveSensitivity.Y, gameSettings.PlayerMoveSpeedMin.Y, gameSettings.PlayerMoveSpeedMax.Y);
		Avoid *= 0.95f;
		Vector3 val3 = Position + MoveVelocity + Avoid;
		Position = Vector3.Clamp(val3, gameSettings.PlayerMoveRangeMin, gameSettings.PlayerMoveRangeMax);
		ref Vector3 position = ref Position;
		position.Z += Thrust;
		if ((virtualPadState.Triggers.Right[VirtualKeyState.Press] || buttons.B[VirtualKeyState.Press]) && Vulcan != null)
		{
			Vulcan();
		}
		if (InputState.IsPush(buttons.A) && Missile != null)
		{
			Missile();
		}
		if (InputState.IsPush(buttons.LeftShoulder))
		{
			MoveVelocity.X = 0f - gameSettings.PlayerMoveSpeedMax.X;
			Avoid.X = 0f - gameSettings.PlayerMoveSpeedMax.X;
			avoidAmount = 0f;
			avoidRotation = 0f - AvoidRotation;
			PlaySE("SE19");
		}
		if (InputState.IsPush(buttons.RightShoulder))
		{
			MoveVelocity.X = gameSettings.PlayerMoveSpeedMax.X;
			Avoid.X = gameSettings.PlayerMoveSpeedMax.X;
			avoidAmount = 0f;
			avoidRotation = AvoidRotation;
			PlaySE("SE18");
		}
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDevice graphicsDevice = game.GraphicsDevice;
		Matrix world = GetWorld();
		model.Draw(Global.SASData, world);
		sight.Draw(Global.SASData, GetSightWorld());
		CustomParticleSystem.SetParticleRenderStates(graphicsDevice.RenderState, (SpriteBlendMode)2);
		shield.Draw(Global.SASData, world);
		CustomParticleSystem.SetParticleRenderStates(graphicsDevice.RenderState, (SpriteBlendMode)1);
	}

	public override bool Damage(int damage)
	{
		if (IsDamage)
		{
			float? alpha = shield.Alpha;
			if (alpha.GetValueOrDefault() <= 0f && alpha.HasValue && base.Vitality > 0)
			{
				if (Crush != null)
				{
					Crush();
				}
				shield.Alpha = 1f;
				shield.Play(isLoop: false);
				base.Vitality = Math.Max(base.Vitality - damage, 0);
				if (base.Vitality == 0 && Destruction != null)
				{
					Destruction(0);
				}
			}
		}
		return false;
	}

	public bool Damage(int damage, float rate)
	{
		return Damage((int)((float)damage * rate));
	}

	public override Matrix GetWorld()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		float num = MathHelper.Lerp(avoidRotation, 0f, avoidAmount);
		float num2 = 0f - MoveVelocity.X + num;
		float y = MoveVelocity.Y;
		Vector3 position = Position;
		position.Z += Thrust;
		return Matrix.CreateRotationZ(num2) * Matrix.CreateRotationX(y) * Matrix.CreateTranslation(position);
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}

	public Matrix GetSightWorld()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = GetPosition();
		position.Z += gameSettings.SightDistance;
		return Matrix.CreateTranslation(position);
	}

	public void Restore(int restore)
	{
		base.Vitality = Math.Min(base.Vitality + restore, 100);
	}
}
