using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XnaLibrary.Graphics;

public class FreeCameraComponent : GameComponent
{
	private const float TurnSpeed = 2f;

	private const float MoveSpeed = 5f;

	private Vector3 position;

	private Vector3 front;

	private Vector3 yawPitchRoll;

	private Vector3 defaultPosition;

	private Vector3 defaultFront;

	private Vector3 defaultYawPitchRoll;

	private bool isReverse;

	public Vector3 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	public Vector3 Front
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return front;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			front = value;
		}
	}

	public Vector3 YawPitchRoll
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return yawPitchRoll;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			yawPitchRoll = value;
		}
	}

	public float Yaw
	{
		get
		{
			return yawPitchRoll.X;
		}
		set
		{
			yawPitchRoll.X = value;
		}
	}

	public float Pitch
	{
		get
		{
			return yawPitchRoll.Y;
		}
		set
		{
			yawPitchRoll.Y = value;
		}
	}

	public float Roll
	{
		get
		{
			return yawPitchRoll.Z;
		}
		set
		{
			yawPitchRoll.Z = value;
		}
	}

	public bool IsReverse
	{
		get
		{
			return isReverse;
		}
		set
		{
			isReverse = value;
		}
	}

	public FreeCameraComponent(Game game)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		((GameComponent)this)._002Ector(game);
		defaultPosition = new Vector3(0f, 50f, 50f);
		defaultFront = new Vector3(0f, 0f, -1f);
		defaultYawPitchRoll = Vector3.Zero;
		InitializeCamera();
	}

	public void InitializeCamera(Vector3 position, Vector3 front, Vector3 yawPitchRoll)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		defaultPosition = position;
		defaultFront = front;
		defaultYawPitchRoll = yawPitchRoll;
		InitializeCamera();
	}

	public void InitializeCamera(Vector3 position, Vector3 front)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		InitializeCamera(position, front, Vector3.Zero);
	}

	public void InitializeCamera()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		position = defaultPosition;
		front = defaultFront;
		yawPitchRoll = defaultYawPitchRoll;
	}

	public override void Update(GameTime gameTime)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Invalid comparison between Unknown and I4
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState((PlayerIndex)0);
		_ = gameTime.ElapsedGameTime.TotalMilliseconds;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref state2)).ThumbSticks;
		float num = (0f - MathHelper.ToRadians(((GamePadThumbSticks)(ref thumbSticks)).Right.Y)) * 2f;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state2)).ThumbSticks;
		float num2 = (0f - MathHelper.ToRadians(((GamePadThumbSticks)(ref thumbSticks2)).Right.X)) * 2f;
		if (!IsReverse)
		{
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				num -= MathHelper.ToRadians(1f) * 2f;
			}
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				num += MathHelper.ToRadians(1f) * 2f;
			}
		}
		else
		{
			num = 0f - num;
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				num += MathHelper.ToRadians(1f) * 2f;
			}
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				num -= MathHelper.ToRadians(1f) * 2f;
			}
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)37))
		{
			num2 += MathHelper.ToRadians(1f) * 2f;
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)39))
		{
			num2 -= MathHelper.ToRadians(1f) * 2f;
		}
		Vector3 val = Vector3.Cross(Vector3.Up, front);
		Vector3 val2 = Vector3.Cross(val, Vector3.Up);
		Matrix val3 = Matrix.CreateFromAxisAngle(val, num);
		Matrix val4 = Matrix.CreateFromAxisAngle(Vector3.Up, num2);
		Vector3 val5 = Vector3.TransformNormal(front, val3 * val4);
		if (Vector3.Dot(val5, val2) > 0.001f)
		{
			front = Vector3.Normalize(val5);
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)87))
		{
			position += front * 5f;
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)83))
		{
			position -= front * 5f;
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)65))
		{
			position += val * 5f;
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)68))
		{
			position -= val * 5f;
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)81))
		{
			Roll -= MathHelper.ToRadians(5f);
		}
		if (((KeyboardState)(ref state)).IsKeyDown((Keys)69))
		{
			Roll += MathHelper.ToRadians(5f);
		}
		Vector3 val6 = position;
		Vector3 val7 = front;
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state2)).ThumbSticks;
		position = val6 + val7 * ((GamePadThumbSticks)(ref thumbSticks3)).Left.Y * 5f;
		Vector3 val8 = position;
		GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state2)).ThumbSticks;
		position = val8 - val * ((GamePadThumbSticks)(ref thumbSticks4)).Left.X * 5f;
		float roll = Roll;
		GamePadTriggers triggers = ((GamePadState)(ref state2)).Triggers;
		float left = ((GamePadTriggers)(ref triggers)).Left;
		GamePadTriggers triggers2 = ((GamePadState)(ref state2)).Triggers;
		Roll = roll + MathHelper.ToRadians((left - ((GamePadTriggers)(ref triggers2)).Right) * 5f);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).RightStick == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)82))
		{
			InitializeCamera();
		}
		((GameComponent)this).Update(gameTime);
	}

	public Matrix GetViewMatrix()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateLookAt(Position, Position + Front, Vector3.Up) * Matrix.CreateFromYawPitchRoll(YawPitchRoll.X, YawPitchRoll.Y, YawPitchRoll.Z);
	}
}
