using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Ball
{
	public enum State
	{
		ALIVE,
		DYING,
		DEAD
	}

	public class Textures
	{
		private static List<Texture2D> Data = new List<Texture2D>();

		public static Texture2D ForBallNumber(int number)
		{
			return Data[number % 28];
		}

		public static void LoadContent(ContentManager Content)
		{
			Data.Add(Content.Load<Texture2D>("Models/white"));
			if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
			{
				for (int i = 1; i < 28; i++)
				{
					Data.Add(Content.Load<Texture2D>("Models/texballs-funky/" + i.ToString("00")));
				}
				modelFunkyBall = new Drawing3D.ModelAlpha(Content.Load<Model>("Models/ballFunkyUVTex"));
			}
			else
			{
				for (int j = 1; j <= 15; j++)
				{
					Data.Add(Content.Load<Texture2D>("Models/texballs/" + j.ToString("00")));
				}
			}
		}
	}

	public const float Ray = 0.833333f;

	public const float Circonference = 5.2359858f;

	public const float stopSpeed = 0.001f;

	public CollisionBande previsionCollisionFixe;

	private Vector3 deathAnimationWayPoint;

	public Matrix displayMatrixRotation;

	public Matrix displayMatrixTranslation;

	public float debugDistanceTrou;

	public bool AppliedFullVelo;

	private static Drawing3D.ModelAlpha modelFunkyBall;

	public State state;

	private static Model model;

	private static Drawing3D.ModelAlpha shadow;

	private int id;

	public Obj obj;

	public VectorBillard Pos = new VectorBillard();

	public VectorBillard Velo = new VectorBillard();

	public int Number => id;

	public bool Moving => Velo.Value != Vector3.Zero;

	public bool Alive => state == State.ALIVE;

	public float SpeedMPH => OldMath.velocityGameUnitsPerFrame_To_MPH(Velo.Len);

	public void Kill()
	{
		state = State.DEAD;
	}

	public void Stop()
	{
		Velo.Set(Vector3.Zero);
	}

	public void Kill(Vector2 posTrou)
	{
		deathAnimationWayPoint = new Vector3(posTrou.X, -1f, posTrou.Y);
		Vector3 value = deathAnimationWayPoint;
		value.Y = 0f;
		value = ((!(Math.Abs(value.X) < 1f)) ? (Vector3.Normalize(Vector3.UnitX * ((value.X > 0f) ? 1 : (-1)) + Vector3.UnitZ * ((value.Z > 0f) ? 1 : (-1))) * 0.833333f * 2f) : (Vector3.Normalize(value) * 0.833333f * 4f));
		if (Velo.Len < 0.1f)
		{
			Velo.Set(Vector3.Normalize(Velo.Value) * 0.1f);
		}
		Vector3 p = Vector3.Normalize(deathAnimationWayPoint + value - Pos.Value) * Velo.Len * ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 1f : 2f);
		Velo.Set(p);
		state = State.DYING;
	}

	public static void LoadContent(ContentManager Content)
	{
		model = Content.Load<Model>("Models/ball");
		shadow = new Drawing3D.ModelAlpha(Content.Load<Model>("Models/shadowobj"));
	}

	public Ball(int number)
	{
		id = number;
		obj = new Obj(BallID(number), model);
		previsionCollisionFixe = new CollisionBande();
		if (number != -1)
		{
			Reset(isAlive: false);
		}
		displayMatrixRotation = Matrix.CreateRotationX((float)Math.PI * 2f * Utils.RandomRatio) * Matrix.CreateRotationY((float)Math.PI * 2f * Utils.RandomRatio) * Matrix.CreateRotationZ((float)Math.PI * 2f * Utils.RandomRatio);
	}

	public Obj.IDenum BallID(int number)
	{
		return (Obj.IDenum)(4 + number);
	}

	public void Reset(bool isAlive)
	{
		Reset(isAlive, usePosParam: false, Vector2.Zero);
	}

	public void Reset(bool isAlive, Vector2 posParam)
	{
		Reset(isAlive, usePosParam: true, posParam);
	}

	private void Reset(bool isAlive, bool usePosParam, Vector2 posParam)
	{
		debugDistanceTrou = 0f;
		Pos.Set(Vector3.Up * 0.833333f);
		state = ((!isAlive) ? State.DEAD : State.ALIVE);
		Velo.Set(Vector3.Zero);
		Vector2 zero = Vector2.Zero;
		zero = ((!usePosParam) ? Rack.InitialPositions[Number] : posParam);
		Pos.Set(zero);
		UpdateDisplayMatrix(1f);
	}

	public void UpdateDying()
	{
		if (Pos.Value.Y > -1f && Velo.Len < 0.8f && Velo.Len != 0f && Vector3.Distance(Pos.Value, deathAnimationWayPoint) > Velo.Len && (double)(Math.Abs(Pos.Value.X) + 0.833333f) < 32.833 && (double)(Math.Abs(Pos.Value.Z) + 0.833333f) < 32.833)
		{
			Pos.Set(Pos.Value + Velo.Value + Vector3.UnitY * -1f * 9.81f * 1f / 60f * 0.057f * 20f);
			UpdateDisplayMatrix(1f);
		}
		else
		{
			GameModeRules.PocketedThisTurn(Number);
			state = State.DEAD;
		}
	}

	public void Update(float fractionTime)
	{
		if (Alive && Moving)
		{
			UpdateDisplayMatrix(fractionTime);
			updateDistanceTrou();
		}
	}

	public void CollisionBande_PreCompute()
	{
		CollisionBande_PreCompute(1f);
	}

	public void CollisionBande_PreCompute(float maxTime)
	{
		if (Alive && Moving && previsionCollisionFixe != null && previsionCollisionFixe.alive && previsionCollisionFixe.applyTest(Pos.Value2D, Velo.Value2D, out var time) && time < maxTime)
		{
			Updates.CollisionList.Add(new Updates.CollisionList.Item(time, Number, previsionCollisionFixe.bande));
		}
	}

	private void updateDistanceTrou()
	{
		float num = 120f;
		foreach (Trou listTrou in CollisionBande.listTrous)
		{
			float num2 = Vector2.Distance(Pos.Value2D, listTrou.pos) - listTrou.rayon;
			if (num2 < num)
			{
				num = num2;
			}
		}
		debugDistanceTrou = num;
	}

	public void ResetMatrixRotation()
	{
		displayMatrixRotation = Matrix.CreateRotationZ((float)Math.PI * 2f * Utils.RandomRatio);
	}

	public void UpdateDisplayMatrix()
	{
		UpdateDisplayMatrix(1f);
	}

	public void UpdateDisplayMatrix(float repositionTime)
	{
		if (Velo.Len > 0f)
		{
			Vector3 zero = Vector3.Zero;
			float degrees = Velo.Len * 360f / 5.2359858f * -1f * repositionTime;
			Vector3 vector = Vector3.Normalize(Velo.Value);
			zero.Y = 0f;
			zero.X = vector.Z * -1f;
			zero.Z = vector.X;
			displayMatrixRotation *= Matrix.CreateFromAxisAngle(Vector3.Normalize(zero), MathHelper.ToRadians(degrees));
		}
		displayMatrixTranslation = Matrix.CreateTranslation(Pos.Value);
	}

	public void createFriction_V2(GameTime gameTime)
	{
		if (Alive)
		{
			float num = ((Velo.Len > 0.8f) ? (1f / 90f) : ((!((double)Velo.Len > 0.01)) ? 0.1f : (1f / 53f)));
			if (!testMinimumVelocityAndStop())
			{
				Velo.Set(Velo.Value * (1f - num));
			}
		}
	}

	private bool testMinimumVelocityAndStop()
	{
		bool result = false;
		if (Velo.Len <= 0.001f)
		{
			Velo.Set(Vector3.Zero);
			result = true;
		}
		return result;
	}

	public static Force createForceBande(Vector3 pos3AsParam, float velLenAsParam, BandeObject bande, float angleVelocityBande)
	{
		Force force = new Force();
		force.Name = bande.name;
		force.Kind = ForceKind.CollisionBande;
		force.Position = pos3AsParam;
		Vector2 vectorUnitaireRejet = bande.getVectorUnitaireRejet();
		vectorUnitaireRejet *= (float)Math.Sin(MathHelper.ToRadians(angleVelocityBande));
		vectorUnitaireRejet *= velLenAsParam;
		vectorUnitaireRejet *= 2f;
		force.Vector = new Vector3(vectorUnitaireRejet.X, 0f, vectorUnitaireRejet.Y);
		if (float.IsInfinity(force.Vector.X) || float.IsInfinity(force.Vector.Y))
		{
			Utils.DebugOut("BAD force vector infinity " + force.Vector);
			Debugger.Break();
		}
		return force;
	}

	public static float angle_0_90_BetweenBandeAndVelocity(Vector2 v1, Vector2 v2)
	{
		Vector2 vector = v1;
		Vector2 vector2 = v2;
		if (v1.Length() == 0f || v2.Length() == 0f)
		{
			return 0f;
		}
		vector.Normalize();
		vector2.Normalize();
		float radians = (float)Math.Acos(vector.X);
		float radians2 = (float)Math.Acos(vector2.X);
		radians = MathHelper.ToDegrees(radians);
		radians2 = MathHelper.ToDegrees(radians2);
		radians %= 180f;
		radians2 %= 180f;
		float num = radians - radians2;
		if (num < 0f)
		{
			num *= -1f;
		}
		if (num > 90f)
		{
			num = 180f - num;
		}
		return num;
	}

	public void applyMiscForce(Force f)
	{
		_ = f.Kind;
		_ = 3;
		Velo.Set(Velo.Value + f.Vector);
		if (Velo.Len != 0f && f.Kind != ForceKind.Friction)
		{
			updateChangementDeDirection(Pos.Value2D, Velo.Value2D);
		}
	}

	public void ChangeVeloCollisionMobile(Vector2 newVelo)
	{
		Velo.Set(newVelo);
		updateChangementDeDirection(Pos.Value2D, Velo.Value2D);
	}

	public void createAndAddCollisionFromBande_V8(Vector3 pos3AsParam, BandeObject bande)
	{
		float angleVelocityBande = angle_0_90_BetweenBandeAndVelocity(bande.getVectorSameYSignAs(Velo.Value2D), Velo.Value2D);
		Force f = createForceBande(pos3AsParam, Velo.Len, bande, angleVelocityBande);
		applyMiscForce(f);
	}

	public void ApplyVelo(float fraction)
	{
		if (Velo.Len > 0f)
		{
			Pos.Set(Pos.Value + Velo.Value * fraction);
		}
	}

	public void updateChangementDeDirection(Vector2 pos2, Vector2 vel2)
	{
		if (vel2.Length() != 0f && !previsionCollisionFixe.initialise(pos2, Vector2.Normalize(vel2)))
		{
			Debugger.Break();
			Kill();
		}
	}

	public void Draw()
	{
		Draw(1f, -1);
	}

	public void Draw(float alpha, int IDoverride)
	{
		Draw(alpha, IDoverride, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, deadMode: false);
	}

	public void DrawDead()
	{
		Draw(1f, -1, InfoDisplay.ViewMat, InfoDisplay.ProjMat, deadMode: true);
	}

	public void Draw(float alpha, int IDoverride, Matrix viewMat, Matrix projMat, bool deadMode)
	{
		int num = ((IDoverride != -1) ? IDoverride : id);
		ModelMesh modelMesh = ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool && num != 23 && num != 27 && num % 2 == 1) ? modelFunkyBall.model.Meshes[0] : model.Meshes[0]);
		BasicEffect basicEffect = (BasicEffect)modelMesh.Effects[0];
		basicEffect.DiffuseColor = Vector3.One;
		basicEffect.SpecularColor = Vector3.One;
		basicEffect.LightingEnabled = true;
		basicEffect.World = (deadMode ? (Matrix.CreateRotationY((float)Math.PI / 2f) * displayMatrixRotation) : (Draws.defaultMat * displayMatrixRotation)) * displayMatrixTranslation;
		basicEffect.Projection = projMat;
		basicEffect.View = viewMat;
		basicEffect.Alpha = alpha;
		if (IDoverride == -2)
		{
			IDoverride = 0;
			basicEffect.Texture = GameMenus.Textures.black;
			basicEffect.Alpha = 0.5f;
		}
		basicEffect.Texture = Textures.ForBallNumber((IDoverride != -1) ? IDoverride : id);
		basicEffect.EnableDefaultLighting();
		basicEffect.SpecularPower = 120f;
		if (deadMode)
		{
			basicEffect.DirectionalLight0.Direction = Vector3.UnitZ * -1f;
			basicEffect.DirectionalLight1.Enabled = false;
			basicEffect.DirectionalLight2.Enabled = false;
		}
		else
		{
			basicEffect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.Down + Aiming.AimVectorStatic(Math.PI / 2.0) * 0.5f) * 3f / 5f;
			basicEffect.DirectionalLight1.Direction = Vector3.Normalize(Vector3.Down + Aiming.AimVectorStatic(3.665191429188092) * 0.5f) * 3f / 5f;
			basicEffect.DirectionalLight2.Direction = Vector3.Normalize(Vector3.Down + Aiming.AimVectorStatic(5.759586531581287) * 0.5f) * 3f / 5f;
		}
		basicEffect.PreferPerPixelLighting = true;
		modelMesh.Draw();
		if (!deadMode && alpha == 1f)
		{
			DrawShadow(viewMat, projMat);
		}
	}

	private void DrawShadow(Matrix viewMat, Matrix projMat)
	{
		if (state != State.ALIVE)
		{
			return;
		}
		List<double> list = new List<double>();
		list.Add(0.0);
		foreach (double item in list)
		{
			double num = item;
			Vector3 zero = Vector3.Zero;
			Drawing3D.DrawModel(shadow, Matrix.CreateScale(0.8f) * Draws.defaultMat * Matrix.CreateTranslation(zero + new Vector3(Pos.Value2D.X, 0.001f * (float)(Number + 2) + ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 0.01f : 0.0001f) * (float)(num + 1.0), Pos.Value2D.Y)), hasCustomLighting: true, isColorCustom: true, Color.Black, viewMat, projMat, Vector3.Down, hasCustomAmbientColor: true, Color.Black);
		}
	}
}
