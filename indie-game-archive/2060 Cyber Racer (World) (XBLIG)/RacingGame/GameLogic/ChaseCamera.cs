using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RacingGame.Graphics;
using RacingGame.Helpers;

namespace RacingGame.GameLogic;

public class ChaseCamera : CarPhysics
{
	public enum CameraMode
	{
		Default,
		FreeCamera
	}

	private const int MaxCameraWobbelTimeoutMs = 700;

	protected Vector3 cameraPos;

	private float cameraDistance;

	private Vector3 cameraLookVector;

	private CameraMode cameraMode;

	private Matrix rotMatrix;

	private static float cameraWobbelTimeoutMs = 0f;

	private static float cameraWobbelFactor = 1f;

	private Vector3 wannaCameraLookVector;

	private float wannaCameraDistance;

	private Vector3 freeCameraRot;

	private Vector3 wannaHaveCameraRotation;

	private Vector3 lastCameraWobble;

	public Matrix RotationMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return rotMatrix;
		}
	}

	public Vector3 CameraPosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return cameraPos;
		}
	}

	public static Vector3 XAxis
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(BaseGame.ViewMatrix.M11, BaseGame.ViewMatrix.M21, BaseGame.ViewMatrix.M31);
		}
	}

	public static Vector3 YAxis
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(BaseGame.ViewMatrix.M12, BaseGame.ViewMatrix.M22, BaseGame.ViewMatrix.M32);
		}
	}

	public static Vector3 ZAxis
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(BaseGame.ViewMatrix.M13, BaseGame.ViewMatrix.M23, BaseGame.ViewMatrix.M33);
		}
	}

	public bool FreeCamera
	{
		get
		{
			return cameraMode == CameraMode.FreeCamera;
		}
		set
		{
			if (value)
			{
				cameraMode = CameraMode.FreeCamera;
			}
			else
			{
				cameraMode = CameraMode.Default;
			}
		}
	}

	public static void WobbelCamera(float wobbelFactor)
	{
		cameraWobbelTimeoutMs = 700f;
		cameraWobbelFactor = wobbelFactor;
	}

	public ChaseCamera(Vector3 setCarPosition, Vector3 setDirection, Vector3 setUp, Vector3 setCameraPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		rotMatrix = Matrix.Identity;
		wannaCameraLookVector = Vector3.Zero;
		freeCameraRot = new Vector3((float)Math.PI, 0f, -(float)Math.PI / 2f);
		wannaHaveCameraRotation = Vector3.Zero;
		lastCameraWobble = Vector3.Zero;
		base._002Ector(setCarPosition, setDirection, setUp);
		SetCameraPosition(setCameraPos);
	}

	public ChaseCamera(Vector3 setCarPosition, Vector3 setCameraPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		rotMatrix = Matrix.Identity;
		wannaCameraLookVector = Vector3.Zero;
		freeCameraRot = new Vector3((float)Math.PI, 0f, -(float)Math.PI / 2f);
		wannaHaveCameraRotation = Vector3.Zero;
		lastCameraWobble = Vector3.Zero;
		base._002Ector(setCarPosition);
		SetCameraPosition(setCameraPos);
	}

	public ChaseCamera(Vector3 setCarPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		rotMatrix = Matrix.Identity;
		wannaCameraLookVector = Vector3.Zero;
		freeCameraRot = new Vector3((float)Math.PI, 0f, -(float)Math.PI / 2f);
		wannaHaveCameraRotation = Vector3.Zero;
		lastCameraWobble = Vector3.Zero;
		base._002Ector(setCarPosition);
		SetCameraPosition(setCarPosition + new Vector3(0f, 10f, 25f));
	}

	public void SetCameraPosition(Vector3 setCameraPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		cameraPos = setCameraPos;
		cameraDistance = Vector3.Distance(base.LookAtPos, cameraPos);
		cameraLookVector = base.LookAtPos - cameraPos;
		wannaCameraDistance = cameraDistance;
		wannaCameraLookVector = cameraLookVector;
		rotMatrix = Matrix.CreateLookAt(cameraPos, base.LookAtPos, base.CarUpVector);
	}

	public void InterpolateCameraPosition(Vector3 setInterpolatedCameraPos)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!FreeCamera)
		{
			if (wannaCameraDistance == 0f)
			{
				SetCameraPosition(setInterpolatedCameraPos);
			}
			wannaCameraDistance = Vector3.Distance(base.LookAtPos, setInterpolatedCameraPos);
			wannaCameraLookVector = base.LookAtPos - setInterpolatedCameraPos;
		}
	}

	private void HandleFreeCamera()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		if (cameraMode != CameraMode.FreeCamera)
		{
			return;
		}
		float num = 0.0075f;
		float num2 = 5f * BaseGame.MoveFactorPerSecond;
		cameraDistance = ((Vector3)(ref cameraLookVector)).Length();
		if (((Vector3)(ref wannaHaveCameraRotation)).Equals(Vector3.Zero))
		{
			wannaHaveCameraRotation = freeCameraRot;
		}
		Vector3 val = wannaHaveCameraRotation;
		float num3 = (0f - Input.MouseXMovement) * num;
		GamePadState gamePad = Input.GamePad;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamePad)).ThumbSticks;
		float num4 = num3 + ((GamePadThumbSticks)(ref thumbSticks)).Left.X * num2;
		if (num4 == 0f)
		{
			if (Input.GamePadLeftPressed || Input.KeyboardLeftPressed)
			{
				num4 = 0f - num2;
			}
			if (Input.GamePadRightPressed || Input.KeyboardRightPressed)
			{
				num4 = num2;
			}
		}
		float num5 = (0f - Input.MouseYMovement) * num;
		GamePadState gamePad2 = Input.GamePad;
		GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamePad2)).ThumbSticks;
		float num6 = num5 + ((GamePadThumbSticks)(ref thumbSticks2)).Left.Y * num2;
		if (num6 == 0f)
		{
			if (Input.GamePadUpPressed || Input.KeyboardUpPressed)
			{
				num6 = 0f - num2;
			}
			if (Input.GamePadDownPressed || Input.KeyboardDownPressed)
			{
				num6 = num2;
			}
		}
		wannaHaveCameraRotation = new Vector3(val.X, val.Y + num6, val.Z + num4);
		freeCameraRot = Vector3.Lerp(freeCameraRot, wannaHaveCameraRotation, 0.5f);
		float num7 = 1E-06f;
		float num8 = 3.1415918f;
		if (freeCameraRot.X < num7)
		{
			freeCameraRot.X = num7;
		}
		else if (freeCameraRot.X > num8)
		{
			freeCameraRot.X = num8;
		}
		cameraLookVector = new Vector3(0f, 0f, cameraDistance);
		cameraLookVector = Vector3.TransformNormal(cameraLookVector, Matrix.CreateRotationX(freeCameraRot.X) * Matrix.CreateRotationY(freeCameraRot.Y) * Matrix.CreateRotationZ(freeCameraRot.Z));
		KeyboardState keyboard = Input.Keyboard;
		float num9 = (((KeyboardState)(ref keyboard)).IsKeyDown((Keys)160) ? 20f : 40f) * BaseGame.MoveFactorPerSecond;
		float num10 = num9 / 4f;
		float num11 = 0f;
		KeyboardState keyboard2 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard2)).IsKeyDown((Keys)33))
		{
			num11 += num9 * 0.05f;
		}
		KeyboardState keyboard3 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard3)).IsKeyDown((Keys)34))
		{
			num11 -= num9 * 0.05f;
		}
		KeyboardState keyboard4 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard4)).IsKeyDown((Keys)36))
		{
			num11 += num10 * 0.05f;
		}
		KeyboardState keyboard5 = Input.Keyboard;
		if (((KeyboardState)(ref keyboard5)).IsKeyDown((Keys)35))
		{
			num11 -= num10 * 0.05f;
		}
		if (Input.MouseWheelDelta != 0)
		{
			num11 = (float)Input.MouseWheelDelta * BaseGame.MoveFactorPerSecond / 16f;
		}
		GamePadState gamePad3 = Input.GamePad;
		GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref gamePad3)).ThumbSticks;
		if (((GamePadThumbSticks)(ref thumbSticks3)).Right.Y != 0f)
		{
			GamePadState gamePad4 = Input.GamePad;
			GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref gamePad4)).ThumbSticks;
			num11 = ((GamePadThumbSticks)(ref thumbSticks4)).Right.Y * BaseGame.MoveFactorPerSecond;
		}
		if (num11 != 0f)
		{
			KeyboardState keyboard6 = Input.Keyboard;
			if (((KeyboardState)(ref keyboard6)).IsKeyDown((Keys)160))
			{
				num11 /= 2f;
			}
			cameraDistance *= 1f - num11;
			if (cameraDistance < 1f)
			{
				cameraDistance = 1f;
			}
			cameraLookVector = Vector3.TransformNormal(new Vector3(0f, 0f, cameraDistance), Matrix.CreateRotationX(freeCameraRot.X) * Matrix.CreateRotationY(freeCameraRot.Y) * Matrix.CreateRotationZ(freeCameraRot.Z));
		}
		wannaCameraDistance = cameraDistance;
		wannaCameraLookVector = cameraLookVector;
	}

	private void UpdateViewMatrix()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		cameraDistance = cameraDistance * 0.9f + wannaCameraDistance * 0.1f;
		cameraLookVector = cameraLookVector * 0.9f + wannaCameraLookVector * 0.1f;
		cameraPos = base.LookAtPos + cameraLookVector;
		rotMatrix = Matrix.CreateLookAt(cameraPos, base.LookAtPos, base.CarUpVector);
		if (cameraWobbelTimeoutMs > 0f)
		{
			cameraWobbelTimeoutMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
			if (cameraWobbelTimeoutMs < 0f)
			{
				cameraWobbelTimeoutMs = 0f;
			}
		}
		if (cameraWobbelTimeoutMs > 0f && base.ZoomInTime <= 5000f)
		{
			float num = 1.5f * cameraWobbelFactor * (cameraWobbelTimeoutMs / 700f);
			lastCameraWobble = lastCameraWobble * 0.9f + RandomHelper.GetRandomVector3(0f - num, num) * 0.1f;
			rotMatrix *= Matrix.CreateTranslation(lastCameraWobble);
		}
		BaseGame.ViewMatrix = rotMatrix;
	}

	public override void Reset()
	{
		base.Reset();
		cameraWobbelFactor = 0f;
	}

	public override void ClearVariablesForGameOver()
	{
		base.ClearVariablesForGameOver();
		cameraWobbelFactor = 0f;
	}

	public override void Update()
	{
		base.Update();
		HandleFreeCamera();
		UpdateViewMatrix();
	}
}
