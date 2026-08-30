using System;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class CameraBillard
{
	public enum Type
	{
		NORMAL,
		ALT
	}

	private class TransitionInfo
	{
		public const double TransitionTimeFast = 0.85;

		public const double TransitionTimeSlow = 1.0;

		public bool enabled;

		public double startTimeSec;

		public Vector3 startPos;

		public Vector3 startLookAt;

		public Vector3 startUp;

		public Vector3 endPos;

		public Vector3 endLookAt;

		public Vector3 endUp;

		private bool upVectorTransitionAtBeginning;

		private double TransitionTimeSec;

		private float ratio;

		public float ElapsedTime => ratio;

		public TransitionInfo()
		{
			enabled = false;
		}

		public void Initiate(GameTime gameTime, Vector3 startPos, Vector3 startLookAt, Vector3 endPos, Vector3 endLookAt)
		{
			Vector3 zero = Vector3.Zero;
			Vector3 zero2 = Vector3.Zero;
			Initiate(gameTime, startPos, startLookAt, zero, endPos, endLookAt, zero2);
		}

		public void Initiate(GameTime gameTime, Vector3 startPos, Vector3 startLookAt, Vector3 startUp, Vector3 endPos, Vector3 endLookAt, Vector3 endUp)
		{
			Initiate(gameTime, startPos, startLookAt, startUp, endPos, endLookAt, endUp, upVectorTransitionAtBeginning: true, 1.0);
		}

		public void Initiate(GameTime gameTime, Vector3 startPos, Vector3 startLookAt, Vector3 startUp, Vector3 endPos, Vector3 endLookAt, Vector3 endUp, bool upVectorTransitionAtBeginning, double transitionTime)
		{
			enabled = true;
			startTimeSec = gameTime.TotalGameTime.TotalSeconds;
			this.upVectorTransitionAtBeginning = upVectorTransitionAtBeginning;
			TransitionTimeSec = transitionTime;
			this.startPos = startPos;
			this.startLookAt = startLookAt;
			this.startUp = startUp;
			this.endPos = endPos;
			this.endLookAt = endLookAt;
			this.endUp = endUp;
			ratio = 0f;
		}

		public void Update(GameTime gameTime, out Vector3 outPos, out Vector3 outLookAt)
		{
			Update(gameTime, out outPos, out outLookAt, out var _);
		}

		public void UpdateEndAttributes(Vector3 endPos, Vector3 endLookAt)
		{
			this.endPos = endPos;
			this.endLookAt = endLookAt;
		}

		public void Update(GameTime gameTime, out Vector3 outPos, out Vector3 outLookAt, out Vector3 outUp)
		{
			ratio = (float)((gameTime.TotalGameTime.TotalSeconds - startTimeSec) / TransitionTimeSec);
			float old = Utils.SmoothStep(ratio);
			old = Utils.clampRatio(old);
			outPos = Utils.LerpVector3(startPos, endPos, old);
			outLookAt = Utils.LerpVector3(startLookAt, endLookAt, old);
			float amount = (upVectorTransitionAtBeginning ? Utils.PowerCurveInverse(ratio, 3) : Utils.PowerCurve(ratio, 3f));
			outUp = Utils.LerpVector3(startUp, endUp, amount);
			if (old >= 1f)
			{
				enabled = false;
			}
		}
	}

	private const float customHeightMin = 12f;

	private const float customHeightMax = 25f;

	private const float distanceToBall = 25f;

	public Type type;

	public static bool BoxShot = false;

	private Vector3 pos;

	private Vector3 lookAt;

	private Vector3 upVec;

	private float customHeightRatio;

	public static readonly Vector3 AltPositionFinal = new Vector3(0f, 110f, 0f);

	public static readonly Vector3 AltLookAtFinal = new Vector3(AltPositionFinal.X, 0f, AltPositionFinal.Z);

	private static readonly Vector3 AltPositionFinalOffsetZ = Vector3.UnitZ * -6.5f;

	private Vector3 AltPosition;

	private Vector3 AltLookAt;

	private Vector3 AltUp;

	private TransitionInfo transitionMenu = new TransitionInfo();

	private Vector3 menuPos;

	private Vector3 menuLookAt;

	private Vector3 menuUp;

	private TransitionInfo transition = new TransitionInfo();

	private TransitionInfo transitionALT = new TransitionInfo();

	private readonly Vector3 posInit = new Vector3(36f, 18f, 0f);

	private readonly Vector3 lookAtInit = Vector3.Zero;

	private Matrix proj;

	private Matrix view;

	private float menuTurnRatio = (float)Math.PI / 4f;

	private float CustomHeight => MathHelper.Lerp(12f, 25f, customHeightRatio);

	public bool Transitioning => transition.enabled;

	public bool TransitioningALT => transitionALT.enabled;

	public bool TransitioningMenu => transitionMenu.enabled;

	public bool AltCamUpVectorSens => transitionALT.endUp.Z > 0f;

	public float TransitionRatio => transition.ElapsedTime;

	public Matrix ViewMatrix => view;

	public Matrix ProjMatrix => proj;

	public void Switch(GameTime gameTime)
	{
		if (type == Type.NORMAL)
		{
			type = Type.ALT;
			bool flag = lookAt.Z - pos.Z > 0f;
			Vector3 endUp = (flag ? Vector3.UnitZ : (Vector3.UnitZ * -1f));
			Vector3 altPosition = pos;
			Vector3 altLookAt = lookAt;
			Vector3 startUp = Vector3.Up;
			if (transitionALT.enabled)
			{
				altPosition = AltPosition;
				altLookAt = AltLookAt;
				startUp = AltUp;
				_ = transitionALT.ElapsedTime;
			}
			transitionALT.Initiate(gameTime, altPosition, altLookAt, startUp, AltPositionFinal + AltPositionFinalOffsetZ * ((!flag) ? 1 : (-1)), AltLookAtFinal + AltPositionFinalOffsetZ * ((!flag) ? 1 : (-1)), endUp, upVectorTransitionAtBeginning: false, 0.85);
		}
		else
		{
			if (type != Type.ALT)
			{
				throw new Exception("not supported : " + type);
			}
			type = Type.NORMAL;
			transitionALT.Initiate(gameTime, AltPosition, AltLookAt, AltUp, pos, lookAt, Vector3.Up, !transitionALT.enabled, 0.85 * (double)(transitionALT.enabled ? transitionALT.ElapsedTime : 1f));
		}
	}

	public void InitiateTransitionMenu(GameTime gameTime)
	{
		Vector3 altPosition = pos;
		Vector3 altLookAt = lookAt;
		Vector3 startUp = Vector3.Up;
		float num = 1f;
		if (type == Type.ALT || transitionALT.enabled)
		{
			altPosition = AltPosition;
			altLookAt = AltLookAt;
			startUp = AltUp;
			if (transitionALT.enabled)
			{
				transitionALT.enabled = false;
			}
		}
		type = Type.NORMAL;
		UpdateMenus(gameTime);
		transitionMenu.Initiate(gameTime, altPosition, altLookAt, startUp, menuPos, menuLookAt, menuUp, upVectorTransitionAtBeginning: true, (double)num * 1.0);
	}

	public void InitiateTransitionNormal(GameTime gameTime, Vector3 finalPos, Vector3 finalLookAt)
	{
		transition.Initiate(gameTime, pos, lookAt, upVec, finalPos, finalLookAt, Vector3.Up);
	}

	public CameraBillard()
	{
		lookAt = lookAtInit;
		pos = posInit;
		upVec = Vector3.Up;
		proj = Matrix.CreatePerspectiveFieldOfView(BoxShot ? MathHelper.ToRadians(90f) : MathHelper.ToRadians(45f), Statics.draw2D.Device.Viewport.AspectRatio, 1f, 10000f);
		customHeightRatio = ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 0.5f : 1f);
		type = Type.NORMAL;
		UpdateMatrix();
	}

	public void DebugSet(Vector3 p, Vector3 la)
	{
		pos = p;
		lookAt = la;
		UpdateMatrix();
	}

	public void Quake(Vector2 stick1, Vector2 stick2, float throttle, bool RBpressed, bool LBpressed)
	{
		Vector3 vector = Vector3.Normalize(lookAt - pos);
		if (RBpressed)
		{
			pos.Y += 0.1f;
		}
		if (LBpressed)
		{
			pos.Y -= 0.1f;
		}
		if (throttle != 0f)
		{
			Vector3 vector2 = vector * throttle * 0.5f;
			pos += vector2;
			lookAt += vector2;
		}
		Vector2 vector3 = OldMath.vector2Normal(Vector2.Normalize(new Vector2(vector.X, vector.Z)));
		Vector3 vector4 = new Vector3(vector3.X, 0f, vector3.Y);
		if (stick1 != Vector2.Zero)
		{
			lookAt -= pos;
			lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(stick1.X * -0.03f));
			lookAt = Vector3.Transform(lookAt, Matrix.CreateFromAxisAngle(vector4, stick1.Y * 0.03f));
			lookAt += pos;
		}
		if (stick2 != Vector2.Zero)
		{
			stick2 *= 0.1f;
			Vector3 vector5 = vector4 * stick2.X;
			vector5.Y = stick2.Y;
			pos += vector5;
			lookAt += vector5;
		}
		UpdateMatrix();
	}

	private void UpdateMatrix()
	{
		if (type == Type.NORMAL && !transitionALT.enabled)
		{
			view = Matrix.CreateLookAt(pos, lookAt, upVec);
		}
		else
		{
			view = Matrix.CreateLookAt(AltPosition, AltLookAt, AltUp);
		}
	}

	private void UpdateMenus(GameTime gameTime)
	{
		menuTurnRatio += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 4000f;
		menuTurnRatio %= (float)Math.PI * 2f;
		float num = ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 54f : 36f);
		menuPos = new Vector3((float)Math.Sin(menuTurnRatio) * num, 18f, (float)Math.Cos(menuTurnRatio) * num);
		menuLookAt = Vector3.Zero;
		menuUp = Vector3.Up;
	}

	public void Update(GameTime gameTime)
	{
		if (GameState.CameraMenu)
		{
			UpdateMenus(gameTime);
			if (transitionMenu.enabled)
			{
				transitionMenu.UpdateEndAttributes(menuPos, menuLookAt);
				transitionMenu.Update(gameTime, out pos, out lookAt, out upVec);
			}
			else
			{
				pos = menuPos;
				lookAt = menuLookAt;
			}
			UpdateMatrix();
			return;
		}
		transitionMenu.enabled = false;
		if (transitionALT.enabled)
		{
			if (type == Type.NORMAL)
			{
				transitionALT.UpdateEndAttributes(pos, lookAt);
			}
			transitionALT.Update(gameTime, out AltPosition, out AltLookAt, out AltUp);
			UpdateMatrix();
		}
		if (transition.enabled)
		{
			transition.Update(gameTime, out pos, out lookAt, out upVec);
			UpdateMatrix();
			return;
		}
		GameState.Type current = GameState.Current;
		if (current == GameState.Type.REPOSITION_WBALL)
		{
			Update_Aiming_Normal(RepositionWBall.LookAtDirection);
		}
	}

	public void Update_Aiming_Normal(Vector3 vector)
	{
		ComputePosLookAt(Statics.balls[0].Pos.Value, vector, out pos, out lookAt);
		UpdateMatrix();
	}

	public void ComputePosLookAt(Vector3 wballPos, Vector3 vector, out Vector3 outPos, out Vector3 outLookAt)
	{
		outLookAt = wballPos + vector * 0.833333f * 10f;
		outPos = wballPos + vector * -1f * 25f;
		outPos.Y = CustomHeight;
	}

	public void ChangeHeight(float ratioOffset)
	{
		if (type == Type.NORMAL)
		{
			customHeightRatio = Utils.clampRatio(customHeightRatio + ratioOffset);
			pos.Y = CustomHeight;
			UpdateMatrix();
		}
	}
}
