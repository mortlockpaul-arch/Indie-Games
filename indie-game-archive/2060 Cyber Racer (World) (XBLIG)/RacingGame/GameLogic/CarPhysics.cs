using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using RacingGame.GameLogic.Physics;
using RacingGame.Graphics;
using RacingGame.Helpers;
using RacingGame.Properties;
using RacingGame.Sounds;

namespace RacingGame.GameLogic;

public class CarPhysics : BasePlayer
{
	public const float DefaultCarMass = 1000f;

	private const float Gravity = 9.81f;

	public const float DefaultMaxSpeed = 47.465855f;

	public const float MaxPossibleSpeed = 50.0549f;

	public const float DefaultMaxAccelerationPerSec = 2.5f;

	public const float MaxAcceleration = 5.75f;

	public const float MinAcceleration = -3.25f;

	private const float CarFrictionOnRoad = 17.523457f;

	private const float AirFrictionPerSpeed = 0.66f;

	private const float MaxAirFriction = 132f;

	private const float BrakeSlowdown = 1f;

	public const float MeterPerSecToMph = 5.793638f;

	public const float MphToMeterPerSec = 0.17260312f;

	public const float MaxRotationPerSec = 1.3f;

	public const float MinSensitivity = 0.5f;

	protected const float CarHeight = 2f;

	private const float MinViewDistance = 0.4f;

	private const float MaxViewDistance = 1.8f;

	private const float WheelMovementSpeed = 1f;

	protected static float maxSpeed = 49.839146f;

	protected static float carMass = 1015f;

	protected static float maxAccelerationPerSec = 2.125f;

	private Vector3 carPos;

	private Vector3 carDir;

	private float speed;

	private Vector3 carUp;

	private Vector3 carForce;

	private static SpringPhysicsObject carPitchPhysics = new SpringPhysicsObject(1000f, 1.5f, 120f, 0f);

	private float viewDistance;

	private float wheelPos;

	private float rotateCarAfterCollision;

	protected bool isCarOnGround;

	private int trackSegmentNumber;

	private float trackSegmentPercent;

	private Matrix carRenderMatrix;

	private float lastAccelerationResult;

	private int lastGear;

	private float virtualRotationAmount;

	private float rotationChange;

	private float gravitySpeed;

	protected Vector3 groundPlanePos;

	protected Vector3 groundPlaneNormal;

	protected Vector3 guardrailLeft;

	protected Vector3 nextGuardrailLeft;

	protected Vector3 guardrailRight;

	protected Vector3 nextGuardrailRight;

	public Vector3 CarUpVector
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return carUp;
		}
	}

	public Vector3 CarPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return carPos;
		}
	}

	public float Speed => speed;

	public float Acceleration
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			lastAccelerationResult += Vector3.Dot(carForce, carDir) * 0.01f * BaseGame.MoveFactorPerSecond;
			if (lastAccelerationResult < -0.25f)
			{
				lastAccelerationResult = -0.25f;
			}
			if (lastAccelerationResult > 1f)
			{
				lastAccelerationResult = 1f;
			}
			int num = 1 + (int)(5f * Speed / 50.0549f);
			if (num != lastGear)
			{
				lastAccelerationResult = 0f;
				lastGear = num;
			}
			return lastAccelerationResult;
		}
	}

	public Vector3 LookAtPos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return carPos + carUp * 2f;
		}
	}

	public Vector3 CarDirection
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return carDir;
		}
	}

	public float CarWheelPos => wheelPos;

	public Vector3 CarRight
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Cross(carDir, carUp);
		}
	}

	public Matrix CarRenderMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return carRenderMatrix;
		}
	}

	public static void SetCarVariablesForCarType(float setMaxCarSpeed, float setCarMass, float setMaxAccelerationPerSec)
	{
		maxSpeed = setMaxCarSpeed;
		carMass = setCarMass;
		maxAccelerationPerSec = setMaxAccelerationPerSec;
		carPitchPhysics = new SpringPhysicsObject(carMass, 1.5f, 120f, 0f);
	}

	public CarPhysics(Vector3 setCarPosition)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		viewDistance = 1f;
		carRenderMatrix = Matrix.Identity;
		base._002Ector();
		SetCarPosition(setCarPosition, new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f));
	}

	public CarPhysics(Vector3 setCarPosition, Vector3 setDirection, Vector3 setUp)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		viewDistance = 1f;
		carRenderMatrix = Matrix.Identity;
		base._002Ector();
		SetCarPosition(setCarPosition, setDirection, setUp);
	}

	public void SetCarPosition(Vector3 setNewCarPosition, Vector3 setDirection, Vector3 setUp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		carPos = setNewCarPosition;
		carDir = setDirection;
		carUp = setUp;
	}

	public override void Reset()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.Reset();
		speed = 0f;
		carForce = Vector3.Zero;
		trackSegmentNumber = 0;
		trackSegmentPercent = 0f;
	}

	public override void ClearVariablesForGameOver()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.ClearVariablesForGameOver();
		speed = 0f;
		carForce = Vector3.Zero;
		trackSegmentNumber = 0;
		trackSegmentPercent = 0f;
	}

	public override void Update()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Invalid comparison between Unknown and I4
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Invalid comparison between Unknown and I4
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Invalid comparison between Unknown and I4
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Invalid comparison between Unknown and I4
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Invalid comparison between Unknown and I4
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		base.Update();
		if (!Input.IsGamePadConnected || Guide.IsVisible || game_paused || RacingGameManager.Player.FreeCamera)
		{
			return;
		}
		if (base.ZoomInTime > 0f)
		{
			isCarOnGround = false;
		}
		wheelPos += BaseGame.MoveFactorPerSecond * speed / 1f;
		float num = BaseGame.MoveFactorPerSecond;
		if (num < 0.001f)
		{
			num = 0.001f;
		}
		if (num > 0.5f)
		{
			num = 0.5f;
		}
		float num2 = 0.5f + GameSettings.Default.ControllerSensitivity;
		rotationChange *= 0.95f;
		if (!Input.KeyboardLeftPressed)
		{
			KeyboardState keyboard = Input.Keyboard;
			if (!((KeyboardState)(ref keyboard)).IsKeyDown((Keys)65))
			{
				if (!Input.KeyboardRightPressed)
				{
					KeyboardState keyboard2 = Input.Keyboard;
					if (!((KeyboardState)(ref keyboard2)).IsKeyDown((Keys)68))
					{
						KeyboardState keyboard3 = Input.Keyboard;
						if (!((KeyboardState)(ref keyboard3)).IsKeyDown((Keys)69))
						{
							rotationChange = 0f;
							goto IL_012d;
						}
					}
				}
				rotationChange -= num2 * 1.3f * num / 2.5f;
				goto IL_012d;
			}
		}
		rotationChange += num2 * 1.3f * num / 2.5f;
		goto IL_012d;
		IL_0792:
		float num4;
		float num3 = speed - num4;
		bool flag;
		if ((speed > 0.5f && speed < 7.5f && num3 > 5.5f * num) || (speed > 0.75f && num3 < 10f * num && flag))
		{
			Sound.Sounds breakSoundType = Sound.GetBreakSoundType(speed, num3, rotationChange);
			if (breakSoundType == Sound.Sounds.BrakeCurveMajor || breakSoundType == Sound.Sounds.BrakeMajor)
			{
				RacingGameManager.Landscape.AddBrakeTrack(this);
			}
			Sound.PlayBrakeSound(breakSoundType);
		}
		if (num3 < -8f * num)
		{
			num3 = -8f * num;
		}
		if (num3 > 8f * num)
		{
			num3 = 8f * num;
		}
		carPitchPhysics.ChangePos(num3);
		goto IL_0843;
		IL_0843:
		if (speed > maxSpeed)
		{
			speed = maxSpeed;
		}
		if (speed < 0f - maxSpeed)
		{
			speed = 0f - maxSpeed;
		}
		carPos += speed * carDir * num * 1.75f;
		carPitchPhysics.Simulate(num);
		int num5 = trackSegmentNumber;
		RacingGameManager.Landscape.UpdateCarTrackPosition(carPos, ref trackSegmentNumber, ref trackSegmentPercent);
		if (trackSegmentNumber != num5 && RacingGameManager.InGame && !base.GameOver)
		{
			if (trackSegmentNumber == 0 && RacingGameManager.Landscape.NewReplay.CheckpointTimes.Count >= RacingGameManager.Landscape.CheckpointSegmentPositions.Count - 1)
			{
				BaseGame.UI.AddTimeFadeupEffect((int)base.GameTimeMilliseconds, UIRenderer.TimeFadeupMode.Normal);
				StartNewLap();
			}
			else
			{
				int count = RacingGameManager.Landscape.NewReplay.CheckpointTimes.Count;
				if (base.ZoomInTime <= 0f && count < RacingGameManager.Landscape.CheckpointSegmentPositions.Count && RacingGameManager.Landscape.CheckpointSegmentPositions[count] > num5 && RacingGameManager.Landscape.CheckpointSegmentPositions[count] <= trackSegmentNumber)
				{
					int num6 = RacingGameManager.Landscape.CompareCheckpointTime(count);
					if (num6 < 0)
					{
						Sound.Play(Sound.Sounds.CheckpointBetter);
					}
					else
					{
						Sound.Play(Sound.Sounds.CheckpointWorse);
					}
					BaseGame.UI.AddTimeFadeupEffect(Math.Abs(num6), (num6 < 0) ? UIRenderer.TimeFadeupMode.Minus : UIRenderer.TimeFadeupMode.Plus);
					RacingGameManager.Landscape.NewReplay.CheckpointTimes.Add(RacingGameManager.Player.GameTimeMilliseconds / 1000f);
				}
			}
		}
		Matrix trackPositionMatrix = RacingGameManager.Landscape.GetTrackPositionMatrix(trackSegmentNumber, trackSegmentPercent, out var roadWidth, out var nextRoadWidth);
		Vector3 carRight = CarRight;
		carUp = ((Matrix)(ref trackPositionMatrix)).Up;
		carDir = Vector3.Cross(carUp, carRight);
		Vector3 translation = ((Matrix)(ref trackPositionMatrix)).Translation;
		RacingGameManager.Player.SetGroundPlaneAndGuardRails(translation, ((Matrix)(ref trackPositionMatrix)).Up, translation - ((Matrix)(ref trackPositionMatrix)).Right * (roadWidth / 2f - 0.25f), translation - ((Matrix)(ref trackPositionMatrix)).Right * (roadWidth / 2f - 0.25f) + ((Matrix)(ref trackPositionMatrix)).Forward, translation + ((Matrix)(ref trackPositionMatrix)).Right * (nextRoadWidth / 2f - 0.25f), translation + ((Matrix)(ref trackPositionMatrix)).Right * (nextRoadWidth / 2f - 0.25f) + ((Matrix)(ref trackPositionMatrix)).Forward);
		carRenderMatrix = RacingGameManager.Player.UpdateCarMatrixAndCamera();
		ApplyGravityAndCheckForCollisions();
		return;
		IL_0518:
		float num7;
		if (speed > 0f && num7 > 5.75f)
		{
			num7 = 5.75f;
		}
		if (num7 < -3.25f)
		{
			num7 = -3.25f;
		}
		if (isCarOnGround)
		{
			carForce += carDir * num7 * (num * 85f);
		}
		num4 = speed;
		Vector3 val = carForce / carMass;
		if (isCarOnGround && ((Vector3)(ref val)).Length() > 0f)
		{
			float num8 = Vector3.Dot(Vector3.Normalize(val), carDir);
			if (num8 > 1f)
			{
				num8 = 1f;
			}
			speed += ((Vector3)(ref val)).Length() * num8;
		}
		float num9 = 0.66f * Math.Abs(speed);
		if (num9 > 132f)
		{
			num9 = 132f;
		}
		float num10 = 17.523457f;
		if (!isCarOnGround)
		{
			num10 = 0f;
		}
		carForce *= 1f - 0.0011687501f * (num10 + num9);
		float num11 = speed;
		speed *= 1f - 2.125E-05f * (num10 + num9);
		if (speed < num11 - 1f)
		{
			speed = num11 - 1f;
		}
		if (isCarOnGround)
		{
			int num12;
			if (!Input.MouseRightButtonPressed && !Input.KeyboardDownPressed)
			{
				GamePadState gamePad = Input.GamePad;
				GamePadDPad dPad = ((GamePadState)(ref gamePad)).DPad;
				num12 = (((int)((GamePadDPad)(ref dPad)).Down == 1) ? 1 : 0);
			}
			else
			{
				num12 = 1;
			}
			flag = (byte)num12 != 0;
			KeyboardState keyboard4 = Input.Keyboard;
			if (!((KeyboardState)(ref keyboard4)).IsKeyDown((Keys)32) && !Input.MouseMiddleButtonPressed)
			{
				GamePadState gamePad2 = Input.GamePad;
				GamePadTriggers triggers = ((GamePadState)(ref gamePad2)).Triggers;
				if (!(((GamePadTriggers)(ref triggers)).Left > 0.5f) && !Input.GamePadBPressed && !flag)
				{
					goto IL_0792;
				}
			}
			float val2 = 1f - num * (flag ? 0.5f : 1f) * ((speed < 0f) ? 0.33f : 1f);
			speed *= Math.Max(0f, val2);
			if (speed > num4 + 100f * num)
			{
				speed = num4 + 100f * num;
			}
			if (speed < num4 - 100f * num)
			{
				speed = num4 - 100f * num;
			}
			flag = true;
			goto IL_0792;
		}
		goto IL_0843;
		IL_012d:
		if (Input.MouseXMovement != 0f)
		{
			rotationChange -= num2 * (Input.MouseXMovement / 15f) * 1.3f * num;
		}
		if (Input.IsGamePadConnected)
		{
			float num13 = rotationChange;
			GamePadState gamePad3 = Input.GamePad;
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePad3)).ThumbSticks;
			rotationChange = num13 - num2 * ((GamePadThumbSticks)(ref thumbSticks)).Left.X * 1.3f * num / 1.12345f;
			GamePadState gamePad4 = Input.GamePad;
			GamePadDPad dPad2 = ((GamePadState)(ref gamePad4)).DPad;
			if ((int)((GamePadDPad)(ref dPad2)).Left == 1)
			{
				rotationChange += num2 * 1.3f * num / 1.5f;
			}
			else
			{
				GamePadState gamePad5 = Input.GamePad;
				GamePadDPad dPad3 = ((GamePadState)(ref gamePad5)).DPad;
				if ((int)((GamePadDPad)(ref dPad3)).Right == 1)
				{
					rotationChange -= num2 * 1.3f * num / 1.5f;
				}
			}
		}
		float num14 = 1.3f * num * 1.25f;
		if (rotateCarAfterCollision != 0f)
		{
			if (rotateCarAfterCollision > num14)
			{
				rotationChange += num14;
				rotateCarAfterCollision -= num14;
			}
			else if (rotateCarAfterCollision < 0f - num14)
			{
				rotationChange -= num14;
				rotateCarAfterCollision += num14;
			}
			else
			{
				rotationChange += rotateCarAfterCollision;
				rotateCarAfterCollision = 0f;
			}
		}
		else if (speed < 10f)
		{
			rotationChange *= 0.67f + 0.33f * speed / 10f;
		}
		else
		{
			rotationChange *= 1f + (speed - 10f) / 100f;
		}
		if (rotationChange > num14)
		{
			rotationChange = num14;
		}
		if (rotationChange < 0f - num14)
		{
			rotationChange = 0f - num14;
		}
		virtualRotationAmount += rotationChange;
		float num15 = (rotationChange + virtualRotationAmount) * num / 0.225f;
		virtualRotationAmount -= num15;
		if (isCarOnGround)
		{
			carDir = Vector3.TransformNormal(carDir, Matrix.CreateFromAxisAngle(carUp, num15));
		}
		KeyboardState keyboard5 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard5)).IsKeyDown((Keys)33) || Input.GamePadXPressed)
		{
			viewDistance -= num * 2f;
		}
		KeyboardState keyboard6 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard6)).IsKeyDown((Keys)34) || Input.GamePadYPressed)
		{
			viewDistance += num * 2f;
		}
		if (Input.MouseWheelDelta != 0)
		{
			viewDistance -= (float)Input.MouseWheelDelta / 500f;
		}
		if (base.ZoomInTime <= 0f)
		{
			viewDistance = MathHelper.Clamp(viewDistance, 0.4f, 1.8f);
		}
		else
		{
			viewDistance = Math.Max(viewDistance, 0.4f);
		}
		num7 = 0f;
		if (!Input.KeyboardUpPressed)
		{
			KeyboardState keyboard7 = Input.Keyboard;
			if (!((KeyboardState)(ref keyboard7)).IsKeyDown((Keys)87) && !Input.MouseLeftButtonPressed && !Input.GamePadAPressed)
			{
				if (!Input.KeyboardDownPressed)
				{
					KeyboardState keyboard8 = Input.Keyboard;
					if (!((KeyboardState)(ref keyboard8)).IsKeyDown((Keys)83))
					{
						KeyboardState keyboard9 = Input.Keyboard;
						if (!((KeyboardState)(ref keyboard9)).IsKeyDown((Keys)79) && !Input.MouseRightButtonPressed)
						{
							if (Input.IsGamePadConnected)
							{
								float num16 = num7;
								GamePadState gamePad6 = Input.GamePad;
								GamePadTriggers triggers2 = ((GamePadState)(ref gamePad6)).Triggers;
								num7 = num16 + ((GamePadTriggers)(ref triggers2)).Right * maxAccelerationPerSec;
								GamePadState gamePad7 = Input.GamePad;
								GamePadDPad dPad4 = ((GamePadState)(ref gamePad7)).DPad;
								if ((int)((GamePadDPad)(ref dPad4)).Up == 1)
								{
									num7 += maxAccelerationPerSec;
								}
								else
								{
									GamePadState gamePad8 = Input.GamePad;
									GamePadDPad dPad5 = ((GamePadState)(ref gamePad8)).DPad;
									if ((int)((GamePadDPad)(ref dPad5)).Down == 1)
									{
										num7 -= maxAccelerationPerSec;
									}
								}
							}
							goto IL_0518;
						}
					}
				}
				num7 -= maxAccelerationPerSec;
				goto IL_0518;
			}
		}
		num7 += maxAccelerationPerSec;
		goto IL_0518;
	}

	public void ApplyGravityAndCheckForCollisions()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		if (RacingGameManager.InMenu)
		{
			return;
		}
		Vector3 val = Vector3.Normalize(nextGuardrailLeft - guardrailLeft);
		Vector3 val2 = Vector3.Normalize(nextGuardrailRight - guardrailRight);
		Vector3 val3 = Vector3.Cross(val, groundPlaneNormal);
		Vector3 val4 = Vector3.Cross(groundPlaneNormal, val2);
		Vector3 val5 = guardrailLeft - guardrailRight;
		float num = ((Vector3)(ref val5)).Length();
		float moveFactorPerSecond = BaseGame.MoveFactorPerSecond;
		Vector3 val6 = carPos;
		Vector3 val7 = Vector3.Cross(carDir, carUp);
		Vector3 vec = -val7;
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			val6 + carDir * 5.6f / 2f - val7 * 2.6f / 2f,
			val6 + carDir * 5.6f / 2f + val7 * 2.6f / 2f,
			val6 - carDir * 5.6f / 2f + val7 * 2.6f / 2f,
			val6 - carDir * 5.6f / 2f - val7 * 2.6f / 2f
		};
		float num2 = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Z > groundPlanePos.Z)
			{
				num2 += 2.4525f;
			}
			float num3 = Vector3Helper.DistanceToLine(array[i], guardrailLeft, nextGuardrailLeft);
			float num4 = Vector3Helper.DistanceToLine(array[i], guardrailRight, nextGuardrailRight);
			if (num3 < 0.1f || num4 > num)
			{
				float num5 = Vector3Helper.GetAngleBetweenVectors(val7, val3);
				if (num5 > (float)Math.PI / 2f)
				{
					num5 -= (float)Math.PI;
				}
				if (Math.Abs(num5) < (float)Math.PI / 4f)
				{
					Sound.PlayCrashSound(totalCrash: false);
					if (i < 2)
					{
						rotateCarAfterCollision = (0f - num5) / 1.5f;
						speed *= 0.93f;
						if (viewDistance > 0.75f)
						{
							viewDistance -= 0.1f;
						}
					}
					else
					{
						rotateCarAfterCollision = (0f - num5) / 2.5f;
						speed *= 0.96f;
						if (viewDistance > 0.75f)
						{
							viewDistance -= 0.05f;
						}
					}
					ChaseCamera.WobbelCamera(0.00075f * speed);
				}
				else if (Math.Abs(num5) < (float)Math.PI * 3f / 4f)
				{
					if (Math.Abs(num5) < (float)Math.PI / 3f)
					{
						rotateCarAfterCollision = num5 / 3f;
					}
					Sound.PlayCrashSound(totalCrash: true);
					ChaseCamera.WobbelCamera(0.005f * speed);
					speed = 0f;
				}
				carForce = Vector3.Zero;
				float num6 = speed * Math.Abs(Vector3.Dot(carDir, val3));
				if (num3 > 0f)
				{
					float num7 = num3 + 0.01f + 0.1f * num6 * moveFactorPerSecond;
					carPos += num7 * val3;
				}
			}
			if (!(num4 < 0.1f) && !(num3 > num))
			{
				continue;
			}
			float num8 = Vector3Helper.GetAngleBetweenVectors(vec, val4);
			if (num8 > (float)Math.PI / 2f)
			{
				num8 -= (float)Math.PI;
			}
			if (Math.Abs(num8) < (float)Math.PI / 4f)
			{
				Sound.PlayCrashSound(totalCrash: false);
				if (i < 2)
				{
					rotateCarAfterCollision = num8 / 1.5f;
					speed *= 0.935f;
					if (viewDistance > 0.75f)
					{
						viewDistance -= 0.1f;
					}
				}
				else
				{
					rotateCarAfterCollision = num8 / 2.5f;
					speed *= 0.96f;
					if (viewDistance > 0.75f)
					{
						viewDistance -= 0.05f;
					}
				}
				ChaseCamera.WobbelCamera(0.00075f * speed);
			}
			else if (Math.Abs(num8) < (float)Math.PI * 3f / 4f)
			{
				if (Math.Abs(num8) < (float)Math.PI / 3f)
				{
					rotateCarAfterCollision = num8 / 3f;
				}
				Sound.PlayCrashSound(totalCrash: true);
				ChaseCamera.WobbelCamera(0.005f * speed);
				speed = 0f;
			}
			carForce = Vector3.Zero;
			float num9 = speed * Math.Abs(Vector3.Dot(carDir, val3));
			if (num4 > 0f)
			{
				float num10 = num4 + 0.01f + 0.1f * num9 * moveFactorPerSecond;
				carPos += num10 * val4;
			}
		}
		ApplyGravity();
	}

	private void ApplyGravity()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		float moveFactorPerSecond = BaseGame.MoveFactorPerSecond;
		float num = Vector3Helper.SignedDistanceToPlane(carPos, groundPlanePos - new Vector3(0f, 0f, 0.15f), groundPlaneNormal);
		isCarOnGround = num > -0.5f;
		float num2 = 9.81f * moveFactorPerSecond;
		float num3 = -9.81f * moveFactorPerSecond;
		if (num > num2)
		{
			num = num2;
			gravitySpeed = 0f;
		}
		if (num < num3)
		{
			num = num3;
			gravitySpeed -= num;
		}
		ref Vector3 reference = ref carPos;
		reference.Z += num;
		bool flag = carUp.Z < 0.05f;
		bool flag2 = carDir.Z > 0.65f;
		bool flag3 = carDir.Z < -0.65f;
		if (flag || flag2 || flag3)
		{
			carPos.Z = groundPlanePos.Z;
		}
	}

	public void SetGroundPlaneAndGuardRails(Vector3 setGroundPlanePos, Vector3 setGroundPlaneNormal, Vector3 setGuardrailLeft, Vector3 setNextGuardrailLeft, Vector3 setGuardrailRight, Vector3 setNextGuardrailRight)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		groundPlanePos = setGroundPlanePos;
		groundPlaneNormal = setGroundPlaneNormal;
		guardrailLeft = setGuardrailLeft;
		nextGuardrailLeft = setNextGuardrailLeft;
		guardrailRight = setGuardrailRight;
		nextGuardrailRight = setNextGuardrailRight;
	}

	public Matrix UpdateCarMatrixAndCamera()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		((Matrix)(ref val)).Right = CarRight;
		((Matrix)(ref val)).Up = carUp;
		((Matrix)(ref val)).Forward = carDir;
		((Matrix)(ref val)).Translation = carPos;
		float num = (4.25f + 9.75f * speed / maxSpeed) * viewDistance;
		if (!RacingGameManager.InMenu && base.ZoomInTime > 1500f)
		{
			Vector3 val2 = carPos + carUp * 2f + ((Matrix)(ref val)).Forward * (num + MathHelper.Max(base.ZoomInTime - 3000f, 0f) / 5000f * 250f) - ((Matrix)(ref val)).Up * (0.6f + MathHelper.Max(base.ZoomInTime - 3000f, 0f) / 5000f * 200f);
			if (base.ZoomInTime - BaseGame.ElapsedTimeThisFrameInMilliseconds >= 3000f)
			{
				RacingGameManager.Player.SetCameraPosition(val2);
			}
			else
			{
				RacingGameManager.Player.InterpolateCameraPosition(val2);
			}
		}
		else if (RacingGameManager.Player.FreeCamera)
		{
			RacingGameManager.Player.InterpolateCameraPosition(carPos + carUp * 2f + ((Matrix)(ref val)).Forward * num - ((Matrix)(ref val)).Up * num / (viewDistance + 6f) - ((Matrix)(ref val)).Up * 1f);
		}
		else if (RacingGameManager.InMenu && BaseGame.TotalTimeMilliseconds < 100f)
		{
			RacingGameManager.Player.SetCameraPosition(carPos + carUp * 2f + ((Matrix)(ref val)).Forward * num - ((Matrix)(ref val)).Up * 0.6f);
		}
		else
		{
			RacingGameManager.Player.InterpolateCameraPosition(carPos + ((Matrix)(ref val)).Up * 2f + ((Matrix)(ref val)).Forward * num / 1.125f - ((Matrix)(ref val)).Up * 0.8f);
		}
		if (RacingGameManager.Player.GameTimeMilliseconds > (float)RacingGameManager.Landscape.NewReplay.NumberOfTrackMatrices * 0.2f * 1000f)
		{
			RacingGameManager.Landscape.NewReplay.AddCarMatrix(val);
		}
		val = Matrix.CreateRotationX((float)Math.PI / 2f - carPitchPhysics.pos / 60f) * Matrix.CreateRotationZ((float)Math.PI) * val;
		return val;
	}
}
