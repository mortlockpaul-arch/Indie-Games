using System;
using System.Collections.Generic;
using System.Linq;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class LigneVisee
{
	public enum Level
	{
		L0,
		L1,
		L2,
		LCPU
	}

	private enum RebondType
	{
		Bande,
		Boule
	}

	private class VertexRebond
	{
		public Vector3 p0;

		public Color color;

		public VertexRebond(Vector3 pp0, Color c)
		{
			p0 = pp0;
			color = c;
		}
	}

	public const int LevelCount = 3;

	private static Obj obj;

	private static float distanceMax = 80f;

	private static List<VertexPositionColor> pointList = new List<VertexPositionColor>();

	private static List<int> indicesList = new List<int>();

	private static List<Vector3> wballPositions = new List<Vector3>();

	private static List<Vector3> aimingColorBallPos = new List<Vector3>();

	private static int aimingColorBallID;

	private static List<VertexRebond> rebondsList = new List<VertexRebond>();

	private static bool hasCollisionBouleOccured;

	private static int nbSegmentsAvantCollisionBoule;

	private static Ball aimingBall = new Ball(0);

	public static Level[] Levels = new Level[4];

	private static float totalLen = 0f;

	public static string LevelName(int level)
	{
		return "Level " + (level + 1);
	}

	public static void Initialise(ContentManager Content)
	{
		obj = new Obj((Obj.IDenum)(-1), Content.Load<Model>("Models/laser"));
		obj.Alpha = 0.66f;
		aimingBall.Reset(isAlive: true);
		for (int i = 0; i < 4; i++)
		{
			Levels[i] = Level.L0;
		}
	}

	public static void Compute()
	{
		ComputeStatic((GameModeRules.CurrentPlayer == GameModeRules.IndexCPU) ? Level.LCPU : Levels[(int)GameModeRules.CurrentPlayer], Aiming.AimVector, Statics.balls[0], Statics.balls, pointList, indicesList, wballPositions, out aimingColorBallID, aimingColorBallPos);
	}

	public static void ComputeStatic(Level currentLevel, Vector3 aimVector, Ball wball, List<Ball> listBalls, List<VertexPositionColor> outPointList, List<int> outIndicesList, List<Vector3> outWballPositions, out int outAimingColorBallID, List<Vector3> outAimingColorBallPos)
	{
		bool flag = false;
		outWballPositions.Clear();
		rebondsList.Clear();
		outAimingColorBallID = -1;
		outAimingColorBallPos.Clear();
		totalLen = 0f;
		outPointList.Clear();
		outIndicesList.Clear();
		if (currentLevel == Level.LCPU || !wball.previsionCollisionFixe.alive)
		{
			return;
		}
		CollisionBande collisionBande = new CollisionBande(wball.previsionCollisionFixe);
		bool flag2 = false;
		VectorBillard vectorBillard = wball.Pos;
		rebondsList.Add(new VertexRebond(vectorBillard.Value + Vector3.Normalize(aimVector) * 0.833333f, Color.White));
		int num = 1;
		VectorBillard vectorBillard2 = new VectorBillard(collisionBande.positionBallCollision);
		VectorBillard vectorBillard3 = new VectorBillard(Vector3.Normalize(vectorBillard2.Value - vectorBillard.Value));
		int colorBallID = -1;
		nbSegmentsAvantCollisionBoule = 0;
		hasCollisionBouleOccured = false;
		while (!flag2)
		{
			if (!CheckSegmentAgainsCollisionColorBall(listBalls, -1, vectorBillard.Value2D, vectorBillard2.Value2D, out var wballPos, out var colorBallPos, out colorBallID))
			{
				outWballPositions.Add(new Vector3(vectorBillard2.Value2D.X, 0.833333f, vectorBillard2.Value2D.Y));
				float num2 = Vector3.Distance(vectorBillard.Value, vectorBillard2.Value);
				if (totalLen + num2 > distanceMax)
				{
					flag2 = true;
				}
				rebondsList.Add(new VertexRebond(vectorBillard2.Value - Vector3.Normalize(vectorBillard3.Value) * 0.833333f, Color.White));
				nbSegmentsAvantCollisionBoule++;
				num++;
				totalLen += num2;
				if (collisionBande.tester.type == CollisionBande.Tester.Type.COLLISION_TROU || currentLevel == Level.L2)
				{
					flag2 = true;
				}
				else
				{
					vectorBillard = vectorBillard2;
					vectorBillard3 = new VectorBillard(ComputeVeloAfterRebondBande(vectorBillard.Value, vectorBillard3.Value, collisionBande.bande));
					rebondsList.Add(new VertexRebond(vectorBillard2.Value + Vector3.Normalize(vectorBillard3.Value) * 0.833333f, Color.White));
					num++;
					collisionBande.initialise(vectorBillard.Value2D, vectorBillard3.Value2D);
					vectorBillard2 = new VectorBillard(collisionBande.positionBallCollision);
				}
			}
			else
			{
				outWballPositions.Add(new Vector3(wballPos.Value2D.X, 0.833333f, wballPos.Value2D.Y));
				flag2 = true;
				hasCollisionBouleOccured = true;
				rebondsList.Add(new VertexRebond(wballPos.Value - Vector3.Normalize(vectorBillard3.Value) * 0.833333f, Color.White));
				nbSegmentsAvantCollisionBoule++;
				num++;
				totalLen += Vector3.Distance(wballPos.Value, vectorBillard.Value);
				if (currentLevel == Level.L0)
				{
					VectorBillard vectorBillard4 = NewDirectionColorBall(vectorBillard.Value2D, wballPos.Value2D, colorBallPos.Value2D);
					Color c = ColorOfBall(colorBallID);
					rebondsList.Add(new VertexRebond(colorBallPos.Value + Vector3.Normalize(vectorBillard4.Value) * 0.833333f, c));
					num++;
					collisionBande.initialise(colorBallPos.Value2D, vectorBillard4.Value2D);
					if (!CheckSegmentAgainsCollisionColorBall(listBalls, colorBallID, colorBallPos.Value2D, new VectorBillard(collisionBande.positionBallCollision).Value2D, out colorBallPos, out var _, out var _))
					{
						flag = true;
						rebondsList.Add(new VertexRebond(collisionBande.positionBallCollision - Vector3.Normalize(vectorBillard4.Value) * 0.833333f, c));
						num++;
						outAimingColorBallPos.Add(collisionBande.positionBallCollision);
						vectorBillard = new VectorBillard(collisionBande.positionBallCollision);
						vectorBillard3 = new VectorBillard(ComputeVeloAfterRebondBande(vectorBillard.Value, vectorBillard4.Value, collisionBande.bande));
						rebondsList.Add(new VertexRebond(collisionBande.positionBallCollision + Vector3.Normalize(vectorBillard3.Value) * 0.833333f, c));
						num++;
						collisionBande.initialise(vectorBillard.Value2D, vectorBillard3.Value2D);
						vectorBillard2 = new VectorBillard(collisionBande.positionBallCollision);
						List<Ball> list = new List<Ball>();
						foreach (Ball listBall in listBalls)
						{
							if (listBall.Number != colorBallID)
							{
								list.Add(listBall);
							}
						}
						if (CheckSegmentAgainsCollisionColorBall(list, -1, vectorBillard.Value2D, vectorBillard2.Value2D, out var wballPos2, out colorBallPos, out var _))
						{
							vectorBillard2 = wballPos2;
						}
						outAimingColorBallPos.Add(vectorBillard2.Value);
						rebondsList.Add(new VertexRebond(vectorBillard2.Value - Vector3.Normalize(vectorBillard3.Value) * 0.833333f, c));
						num++;
					}
					else
					{
						flag = false;
						rebondsList.Add(new VertexRebond(colorBallPos.Value - Vector3.Normalize(vectorBillard4.Value) * 0.833333f, c));
						outAimingColorBallPos.Add(colorBallPos.Value);
						num++;
					}
					outAimingColorBallID = colorBallID;
				}
			}
			if (currentLevel == Level.L2)
			{
				outWballPositions.Clear();
				VertexRebond vertexRebond = rebondsList[rebondsList.Count - 1];
				VertexRebond vertexRebond2 = rebondsList[0];
				if (Vector3.Distance(vertexRebond.p0, vertexRebond2.p0) > 8.33333f)
				{
					rebondsList.RemoveAt(rebondsList.Count - 1);
					rebondsList.Add(new VertexRebond(vertexRebond2.p0 + Vector3.Normalize(vertexRebond.p0 - vertexRebond2.p0) * 8.33333f, vertexRebond.color));
				}
			}
		}
		outPointList.Clear();
		for (int i = 0; i < num; i++)
		{
			outPointList.Add(new VertexPositionColor(rebondsList[i].p0, rebondsList[i].color));
		}
		outIndicesList.Clear();
		for (int j = 0; j < nbSegmentsAvantCollisionBoule; j++)
		{
			outIndicesList.Add(2 * j);
			outIndicesList.Add(2 * j + 1);
		}
		if (currentLevel == Level.L0 && hasCollisionBouleOccured)
		{
			outIndicesList.Add(num - 2);
			outIndicesList.Add(num - 1);
			if (flag)
			{
				outIndicesList.Add(num - 4);
				outIndicesList.Add(num - 3);
			}
		}
	}

	private static VectorBillard NewDirectionColorBall(Vector2 posWBallBefore, Vector2 posWBallAfter, Vector2 posColorBall)
	{
		_ = Vector3.Zero;
		Vector2.Distance(posWBallAfter, posColorBall);
		Vector2 vector = Vector2.Normalize(posWBallAfter - posColorBall);
		Vector2 value = Vector2.Normalize(posWBallAfter - posWBallBefore);
		_ = Vector2.Zero;
		Vector2 vector2 = Vector2.Dot(value, -1f * vector) * (-1f * vector);
		return new VectorBillard(Vector3.Normalize(new Vector3(vector2.X, 0f, vector2.Y)));
	}

	private static bool CheckSegmentAgainsCollisionColorBall(List<Ball> listBalls, int myColorBallId, Vector2 start, Vector2 end, out VectorBillard wballPos, out VectorBillard colorBallPos, out int colorBallID)
	{
		bool flag = false;
		float num = -1f;
		Vector2 vector = end - start;
		wballPos = new VectorBillard(Vector2.Zero);
		colorBallPos = new VectorBillard(Vector2.Zero);
		colorBallID = -1;
		foreach (Ball listBall in listBalls)
		{
			if (!listBall.Alive || listBall.Number == 0 || listBall.Number == myColorBallId)
			{
				continue;
			}
			float num2 = CollisionMobile.TimeOfClosestApproach(start, listBall.Pos.Value2D, end - start, Vector2.Zero, 0.833333f, 0.833333f, out var collision);
			if (!collision || !(num2 > 0f) || !(num2 < 1f))
			{
				continue;
			}
			float num3 = vector.Length() * num2;
			if (!flag || num3 < num)
			{
				flag = true;
				num = num3;
				wballPos = new VectorBillard(start + vector * num2);
				if (wballPos.Value.Y == 0f)
				{
					wballPos.Set(wballPos.Value + Vector3.UnitY * 0.833333f);
				}
				colorBallPos = new VectorBillard(listBall.Pos.Value);
				colorBallID = listBall.Number;
			}
		}
		return flag;
	}

	public static Color ColorOfBall(int id)
	{
		return (id % 8) switch
		{
			0 => Color.Black, 
			1 => Color.Yellow, 
			2 => Color.Blue, 
			3 => Color.Red, 
			4 => Color.Purple, 
			5 => new Color(Color.Orange.ToVector3() * 0.5f), 
			6 => Color.Green, 
			7 => new Color(Color.Brown.ToVector3() * 0.3f), 
			_ => Color.White, 
		};
	}

	private static Vector3 ComputeVeloAfterRebondBande(Vector3 pos, Vector3 vel, BandeObject bande)
	{
		Vector2 value2D = new VectorBillard(vel).Value2D;
		float angleVelocityBande = angle_0_90_BetweenBandeAndVelocity(bande.getVectorSameYSignAs(value2D), value2D);
		return vel + Ball.createForceBande(pos, vel.Length(), bande, angleVelocityBande).Vector;
	}

	private static float angle_0_90_BetweenBandeAndVelocity(Vector2 v1, Vector2 v2)
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

	public static void Draw()
	{
		Draw(normalMode: true, Statics.cam.ViewMatrix, Statics.cam.ProjMatrix, pointList.ToArray(), indicesList.ToArray(), wballPositions, aimingColorBallID, aimingColorBallPos);
	}

	public static void Draw(bool normalMode, Matrix viewMat, Matrix projMat, VertexPositionColor[] PointListArray, int[] IndicesListArray, List<Vector3> paramWballPositions, int paramAimingColorBallID, List<Vector3> paramAimingColorBallPos)
	{
		if (normalMode && GameState.Current != GameState.Type.AIMING)
		{
			return;
		}
		foreach (Vector3 paramWballPosition in paramWballPositions)
		{
			aimingBall.Pos.Set(paramWballPosition);
			aimingBall.UpdateDisplayMatrix(1f);
			aimingBall.Draw(0.33f, -2, viewMat, projMat, deadMode: false);
		}
		foreach (Vector3 paramAimingColorBallPo in paramAimingColorBallPos)
		{
			aimingBall.Pos.Set(paramAimingColorBallPo);
			aimingBall.UpdateDisplayMatrix(1f);
			aimingBall.displayMatrixRotation = Statics.balls[paramAimingColorBallID].displayMatrixRotation;
			aimingBall.Draw(0.33f, paramAimingColorBallID, viewMat, projMat, deadMode: false);
		}
		for (int i = 0; i < IndicesListArray.Count(); i += 2)
		{
			VertexPositionColor vertexPositionColor = PointListArray[IndicesListArray[i]];
			VertexPositionColor vertexPositionColor2 = PointListArray[IndicesListArray[i + 1]];
			Color color = vertexPositionColor.Color;
			VectorBillard vectorBillard = new VectorBillard(vertexPositionColor.Position);
			VectorBillard vectorBillard2 = new VectorBillard(vertexPositionColor2.Position);
			float xScale = Vector2.Distance(vectorBillard.Value2D, vectorBillard2.Value2D);
			double num = Aiming.DirectionToAngle(vectorBillard2.Value2D - vectorBillard.Value2D) * -1.0;
			float num2 = 1f;
			if (color == Color.White)
			{
				num2 = ((!CameraBillard.BoxShot) ? (num2 * 0.75f) : (num2 * 1.2f));
			}
			Drawing3D.DrawModel(obj, Matrix.CreateScale(xScale, num2, num2) * Matrix.CreateRotationY((float)num) * Matrix.CreateTranslation(vectorBillard.Value), hasCustomLighting: true, isColorCustom: true, color, viewMat, projMat, Vector3.Up * -1f, hasCustomAmbientColor: true, color);
		}
	}
}
