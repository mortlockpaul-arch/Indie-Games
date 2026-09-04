using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity.GameObjects;

public class HormingMissile : ModelObject
{
	private const float AngleTurnSpeed = 0.025f;

	public EnemyData Target;

	private Vector3 latestTargetPosition;

	public Vector3 Velocity;

	public Vector3 Normal;

	private float angleY;

	private float angleX;

	private readonly float DefaultAngle = MathHelper.ToRadians(-90f);

	private readonly float AngleOffset = MathHelper.ToRadians(270f);

	public int Limit { get; set; }

	public float Reduction { get; set; }

	public event Action<Vector3> Explosion;

	public HormingMissile(Game game)
		: base(game)
	{
		Initialize();
	}

	public override void Initialize()
	{
		ContentManager content = game.Content;
		model = new XSIModel("Models/Models/player/player_shot", content);
		collision = new XSIModel("Models/Models/player/player_shot", content);
		base.Use = false;
		base.Enable = false;
		base.Visible = false;
	}

	public override void Dispose()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Target = null;
		latestTargetPosition = Vector3.Zero;
		Velocity = Vector3.Zero;
		Normal = Vector3.Zero;
		Reduction = 0f;
		Limit = 0;
		angleY = DefaultAngle;
		angleX = DefaultAngle;
		base.Dispose();
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Limit++;
		if (Target != null && !Target.Use)
		{
			Target = null;
			Velocity *= 2f;
			if (Explosion != null)
			{
				Explosion(GetPosition());
			}
			Dispose();
		}
		else if (Target != null)
		{
			latestTargetPosition = Target.GetPosition();
			float num = 0f;
			float num2 = 0f;
			Vector3 val = latestTargetPosition - Position;
			Vector3 val2 = Vector3.Cross(Vector3.Up, val);
			Vector3 val3 = Vector3.Cross(val2, Vector3.Up);
			Matrix val4 = Matrix.CreateFromAxisAngle(val2, num);
			Matrix val5 = Matrix.CreateFromAxisAngle(Vector3.Up, num2);
			Vector3 val6 = Vector3.TransformNormal(val, val4 * val5);
			if (Vector3.Dot(val6, val3) > 0.001f)
			{
				val = Vector3.Normalize(val6);
			}
			Velocity += val * Reduction;
			float num3 = ((Velocity.Z > 0f) ? 0.001f : 0.001f);
			Reduction = Math.Min(Reduction + num3, 1f);
			angleX = TurnToFaceX(GetPosition(), latestTargetPosition, angleX, 0.025f);
			angleY = TurnToFaceY(GetPosition(), latestTargetPosition, angleY, 0.025f);
		}
		float num4 = Vector3.Distance(Vector3.Zero, Velocity) * 5f;
		float num5 = ((Target != null) ? Vector3.Distance(Position, latestTargetPosition) : 0f);
		if (Target != null && num4 > num5)
		{
			Position = Target.GetPosition();
		}
		else
		{
			Position += Velocity;
		}
		collision.Update(elapsedGameTime);
		collision.UpdateBoundingSphere(GetWorld());
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_ = game.GraphicsDevice;
		Matrix world = GetWorld();
		model.Draw(Global.SASData, world);
	}

	public override Matrix GetWorld()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateRotationX(AngleOffset - angleX) * Matrix.CreateRotationY(AngleOffset - angleY) * Matrix.CreateTranslation(Position);
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}

	public override bool Damage(int damage)
	{
		Dispose();
		return true;
	}

	private static float TurnToFaceX(Vector3 position, Vector3 faceThis, float currentAngle, float turnSpeed)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return TurnToFace(new Vector2(0f - position.Y, position.Z), new Vector2(0f - faceThis.Y, faceThis.Z), currentAngle, turnSpeed);
	}

	private static float TurnToFaceY(Vector3 position, Vector3 faceThis, float currentAngle, float turnSpeed)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return TurnToFace(new Vector2(position.X, position.Z), new Vector2(faceThis.X, faceThis.Z), currentAngle, turnSpeed);
	}

	private static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
	{
		float num = faceThis.X - position.X;
		float num2 = faceThis.Y - position.Y;
		float num3 = (float)Math.Atan2(num2, num);
		float num4 = WrapAngle(num3 - currentAngle);
		num4 = MathHelper.Clamp(num4, 0f - turnSpeed, turnSpeed);
		return WrapAngle(currentAngle + num4);
	}

	private static float WrapAngle(float radians)
	{
		while (radians < -(float)Math.PI)
		{
			radians += (float)Math.PI * 2f;
		}
		while (radians > (float)Math.PI)
		{
			radians -= (float)Math.PI * 2f;
		}
		return radians;
	}
}
