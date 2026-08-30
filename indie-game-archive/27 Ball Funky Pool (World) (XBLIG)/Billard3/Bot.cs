using System;
using System.Collections.Generic;
using System.Linq;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Bot
{
	public enum Level
	{
		Easy,
		Medium,
		Hard
	}

	private class Trajet
	{
		public bool alive;

		public List<Vector2> wpos;

		public List<Vector2> cpos;

		public int cnum;

		public string trouname;

		public float Distance;

		public float wdistance;

		public float cdistance;

		public int NbRebonds;

		public Vector2 wdirection;

		public Vector2 cdirection;

		public float angleColl;

		public Trajet()
		{
			alive = true;
			wpos = new List<Vector2>();
			cpos = new List<Vector2>();
			cnum = -1;
			trouname = "";
			wdistance = 0f;
			cdistance = 0f;
			Distance = 0f;
			angleColl = 0f;
			NbRebonds = 0;
			wdirection = Vector2.Zero;
			cdirection = Vector2.Zero;
		}
	}

	private static Level level;

	private static BasicEffect effect;

	private static double startTime;

	public static float PowerRatio = 1f;

	private static bool computed;

	private static Vector2 repoStart;

	private static Vector2 repoEnd;

	private static List<Trajet> trajets = new List<Trajet>();

	private static List<Trajet> trajetsPossible = new List<Trajet>();

	private static double chosenAngle;

	private static double startAngle;

	public static void SetLevel(Level value)
	{
		level = value;
	}

	public static void Initialize()
	{
		effect = new BasicEffect(Statics.draw2D.Device);
		effect.VertexColorEnabled = true;
		effect.World = Matrix.Identity;
		effect.Alpha = 0.7f;
	}

	public static void StartMyTurn(GameTime gameTime)
	{
		startTime = gameTime.TotalGameTime.TotalSeconds;
		computed = false;
	}

	private static void ComputeMyTurn(GameTime gameTime)
	{
		computed = true;
		Vector2 zero = Vector2.Zero;
		if (!GameModeRules.BreakDone)
		{
			Ball ball = Statics.balls[0];
			if (ball.Pos.Value2D != Rack.InitialPositions[0])
			{
				zero = Vector2.Normalize(Rack.InitialPositions[1] - ball.Pos.Value2D);
			}
			else
			{
				zero = Vector2.Normalize(Rack.InitialPositions[1] - Statics.balls[0].Pos.Value2D);
				Vector2 vector = MyMath.Vector2Orthogonal(zero) * MathHelper.Lerp(-0.19f, 0.19f, Utils.RandomRatio);
				zero = Vector2.Normalize(zero + vector);
			}
			PowerRatio = 1f;
		}
		else
		{
			GameModeRules.SolidOrStrips currentColor = GameModeRules.CurrentColor;
			List<Ball> list = new List<Ball>();
			trajetsPossible.Clear();
			Ball ball2 = Statics.balls[0];
			if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball)
			{
				list.Add(Statics.balls[GameModeRules.LowestNumericalObjectBall]);
			}
			else
			{
				foreach (Ball ball4 in Statics.balls)
				{
					if (ball4.Number == 0 || !ball4.Alive)
					{
						continue;
					}
					bool num;
					if (GameModeRules.TeamCurrent.Pocketed.Count() >= 13)
					{
						num = ball4.Number == 9;
					}
					else
					{
						if (ball4.Number == 9)
						{
							continue;
						}
						if (currentColor == (GameModeRules.SolidOrStrips)(-1))
						{
							goto IL_0192;
						}
						num = currentColor == GameModeRules.BallColor(ball4.Number);
					}
					if (!num)
					{
						continue;
					}
					goto IL_0192;
					IL_0192:
					list.Add(ball4);
				}
			}
			foreach (Ball item in list)
			{
				foreach (Trou listTrou in CollisionBande.listTrous)
				{
					Vector2 pos = listTrou.pos;
					if (listTrou.name.Contains("CENTRAL"))
					{
						pos -= Vector2.Normalize(listTrou.pos) * 0.833333f * 1f;
					}
					else
					{
						Vector2 vector2 = Vector2.Normalize(Vector2.UnitX * ((!(listTrou.pos.X > 0f)) ? 1 : (-1)) + Vector2.UnitY * ((!(listTrou.pos.Y > 0f)) ? 1 : (-1)));
						vector2 *= 2.499999f;
						if (Vector2.Distance(pos + vector2, item.Pos.Value2D) > 1.666666f)
						{
							pos += vector2;
						}
					}
					if (MovePossible(item.Number, item.Pos.Value2D, pos))
					{
						Trajet trajet = new Trajet();
						trajet.trouname = listTrou.name;
						trajet.cnum = item.Number;
						trajet.cpos.Add(item.Pos.Value2D);
						trajet.cpos.Add(pos);
						trajet.cdistance = Vector2.Distance(pos, item.Pos.Value2D);
						trajet.cdirection = Vector2.Normalize(pos - item.Pos.Value2D);
						trajetsPossible.Add(trajet);
					}
					List<bool> list2 = new List<bool>();
					list2.Add(item: true);
					list2.Add(item: false);
					foreach (bool item2 in list2)
					{
						if ((item2 && pos.Y < 0f) || (!item2 && pos.Y > 0f))
						{
							Vector2 end = pos;
							end.Y += 2f * ((Bandes.bandeOrthoX_ZP.p1.Y - 0.833333f) * (float)(item2 ? 1 : (-1)) - end.Y);
							Trajet trajet2 = MoveColorBallRebondPossible(item.Pos.Value2D, end, pos, listTrou.name, item.Number);
							if (trajet2.alive)
							{
								trajetsPossible.Add(trajet2);
							}
						}
					}
					List<bool> list3 = new List<bool>();
					list3.Add(item: true);
					list3.Add(item: false);
					foreach (bool item3 in list3)
					{
						if ((item3 && pos.X < 0f) || (!item3 && pos.X > 0f))
						{
							Vector2 end2 = pos;
							end2.X += 2f * ((Bandes.bandeOrthoZ_XP.p1.X - 0.833333f) * (float)(item3 ? 1 : (-1)) - end2.X);
							Trajet trajet3 = MoveColorBallRebondPossible(item.Pos.Value2D, end2, pos, listTrou.name, item.Number);
							if (trajet3.alive)
							{
								trajetsPossible.Add(trajet3);
							}
						}
					}
				}
			}
			List<Trajet> list4 = new List<Trajet>();
			foreach (Trajet item4 in trajetsPossible)
			{
				Vector2 vector3 = item4.cpos[0] - item4.cdirection * 0.833333f * 2f;
				if (MovePossible(ball2.Number, ball2.Pos.Value2D, vector3))
				{
					item4.wpos.Add(ball2.Pos.Value2D);
					item4.wpos.Add(vector3);
					item4.wdistance = Vector2.Distance(item4.wpos[0], vector3);
					item4.wdirection = Vector2.Normalize(vector3 - item4.wpos[0]);
				}
				else
				{
					item4.alive = false;
					CheckMoveWhiteBallRebondPossible(ball2.Pos.Value2D, vector3, item4, list4);
				}
			}
			foreach (Trajet item5 in list4)
			{
				trajetsPossible.Add(item5);
			}
			trajets.Clear();
			foreach (Trajet item6 in trajetsPossible)
			{
				if (!item6.alive)
				{
					continue;
				}
				bool flag = true;
				foreach (Vector2 wpo in item6.wpos)
				{
					if (flag && !GameState.IsPositionValid(wpo))
					{
						flag = false;
					}
					if (flag && item6.trouname.Contains("CENTRAL"))
					{
						Ball ball3 = new Ball(-1);
						ball3.Reset(isAlive: true, item6.cpos[item6.cpos.Count - 2]);
						ball3.updateChangementDeDirection(ball3.Pos.Value2D, item6.cpos[item6.cpos.Count - 1] - ball3.Pos.Value2D);
						if (ball3.previsionCollisionFixe.bande.type == BandeObject.Type.ORTHO_X)
						{
							flag = false;
						}
					}
					if (flag)
					{
						Vector2 dir = item6.wpos[item6.wpos.Count - 1] - item6.wpos[item6.wpos.Count - 2];
						Vector2 dir2 = MyMath.Vector2Orthogonal(item6.wpos[item6.wpos.Count - 1] - item6.cpos[0]);
						double num2 = Aiming.DirectionToAngle(dir);
						double num3 = Aiming.DirectionToAngle(dir2);
						float num4;
						for (num4 = MathHelper.ToDegrees((float)Math.Abs(num3 - num2)); num4 > 90f; num4 -= 180f)
						{
						}
						item6.angleColl = num4;
						if (item6.angleColl < 15f)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					item6.Distance = item6.wdistance + item6.cdistance;
					item6.NbRebonds = ((item6.wpos.Count > 2) ? 1 : 0) + ((item6.cpos.Count > 2) ? 1 : 0);
					trajets.Add(item6);
				}
				else
				{
					item6.alive = false;
				}
			}
			trajets.Sort(compareTrajet);
			PowerRatio = 1f;
			if (trajets.Count > 0)
			{
				int num5 = trajets.Count / 3;
				int num6 = (int)level * num5;
				num6 += (int)((float)num5 * Utils.RandomRatio);
				num6 = (int)MathHelper.Clamp(num6, 0f, trajets.Count);
				zero = trajets[num6].wdirection;
				float num7 = Math.Max(0.1f, trajets[num6].angleColl);
				float num8 = trajets[num6].wdistance + trajets[num6].cdistance * 90f / num7;
				PowerRatio = Utils.clampRatio(num8 / 70f);
			}
			else if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball)
			{
				zero = Statics.balls[GameModeRules.LowestNumericalObjectBall].Pos.Value2D - ball2.Pos.Value2D;
				PowerRatio = Utils.clampRatio(zero.Length() / 20f);
			}
			else
			{
				bool flag2 = false;
				Vector2 vector4 = Vector2.Zero;
				foreach (Ball ball5 in Statics.balls)
				{
					if (ball5.Alive && ball5.Number % 8 != 0 && GameModeRules.BallColor(ball5.Number) == currentColor)
					{
						Vector2 vector5 = ball5.Pos.Value2D - ball2.Pos.Value2D;
						if (!flag2 || vector5.Length() < vector4.Length())
						{
							vector4 = vector5;
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					zero = vector4;
					PowerRatio = Utils.clampRatio(zero.Length() / (float)((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? 9 : 20));
				}
				else
				{
					zero = new VectorBillard(Aiming.AimVectorStatic((double)Utils.RandomRatio * Math.PI * 2.0)).Value2D;
					PowerRatio = MathHelper.Lerp(0.2f, 0.6f, Utils.RandomRatio);
				}
			}
		}
		float num9 = 0f;
		bool flag3 = false;
		float num10 = MathHelper.ToRadians(5f);
		switch (level)
		{
		case Level.Easy:
			flag3 = !Utils.OneChanceOutOf(4);
			break;
		case Level.Medium:
			flag3 = Utils.OneChanceOutOf(2);
			break;
		}
		if (flag3)
		{
			num9 = MathHelper.Lerp(0f - num10, num10, Utils.RandomRatio);
			chosenAngle += num9;
		}
		chosenAngle = Aiming.DirectionToAngle(zero) + (double)num9;
		startAngle = Aiming.AngleRad;
		if (chosenAngle - startAngle > Math.PI)
		{
			chosenAngle -= Math.PI * 2.0;
		}
		else if (startAngle - chosenAngle > Math.PI)
		{
			chosenAngle += Math.PI * 2.0;
		}
	}

	private static int compareTrajet(Trajet t1, Trajet t2)
	{
		if (t1.NbRebonds < t2.NbRebonds)
		{
			return -1;
		}
		if (t1.NbRebonds > t2.NbRebonds)
		{
			return 1;
		}
		return t1.Distance.CompareTo(t2.Distance);
	}

	private static void CheckMoveWhiteBallRebondPossible(Vector2 start, Vector2 end, Trajet trajetPreComputed, List<Trajet> trajetsFound)
	{
		List<bool> list = new List<bool>();
		list.Add(item: true);
		list.Add(item: false);
		foreach (bool item in list)
		{
			if ((item && end.Y < 0f) || (!item && end.Y > 0f))
			{
				Vector2 targetSym = end;
				float num = (Bandes.bandeOrthoX_ZP.p1.Y - 0.833333f) * (float)(item ? 1 : (-1));
				targetSym.Y += 2f * (num - targetSym.Y);
				Trajet trajet = MovePossibleWBall(start, targetSym, end, trajetPreComputed);
				if (trajet.alive)
				{
					trajetsFound.Add(trajet);
				}
			}
		}
		List<bool> list2 = new List<bool>();
		list2.Add(item: true);
		list2.Add(item: false);
		foreach (bool item2 in list2)
		{
			if ((item2 && end.X < 0f) || (!item2 && end.X > 0f))
			{
				Vector2 targetSym2 = end;
				targetSym2.X += 2f * ((Bandes.bandeOrthoZ_XP.p1.X - 0.833333f) * (float)(item2 ? 1 : (-1)) - targetSym2.X);
				Trajet trajet2 = MovePossibleWBall(start, targetSym2, end, trajetPreComputed);
				if (trajet2.alive)
				{
					trajetsFound.Add(trajet2);
				}
			}
		}
	}

	private static Trajet MovePossibleWBall(Vector2 start, Vector2 targetSym, Vector2 end, Trajet trajetPreComputed)
	{
		Trajet trajet = new Trajet();
		trajet.cpos = trajetPreComputed.cpos;
		trajet.cnum = trajetPreComputed.cnum;
		trajet.cdistance = trajetPreComputed.cdistance;
		trajet.cdirection = trajetPreComputed.cdirection;
		Ball ball = new Ball(-1);
		Vector2 vel = Vector2.Normalize(targetSym - start);
		ball.Reset(isAlive: true, start);
		ball.updateChangementDeDirection(start, vel);
		if (ball.previsionCollisionFixe.bande.type == BandeObject.Type.ORTHO_X || ball.previsionCollisionFixe.bande.type == BandeObject.Type.ORTHO_Z)
		{
			Vector3 positionBallCollision = ball.previsionCollisionFixe.positionBallCollision;
			Vector2 vector = new Vector2(positionBallCollision.X, positionBallCollision.Z);
			if (MovePossible(0, start, vector) && MovePossible(0, vector, end))
			{
				trajet.wpos.Add(start);
				trajet.wpos.Add(vector);
				trajet.wpos.Add(end);
				trajet.wdirection = Vector2.Normalize(vector - start);
				trajet.wdistance = Vector2.Distance(start, vector) + Vector2.Distance(vector, end);
			}
			else
			{
				trajet.alive = false;
			}
		}
		else
		{
			trajet.alive = false;
		}
		return trajet;
	}

	private static Trajet MoveColorBallRebondPossible(Vector2 start, Vector2 end, Vector2 realTarget, string trouname, int bNumber)
	{
		Trajet trajet = new Trajet();
		Vector2 vector = Vector2.Normalize(end - start);
		Ball ball = new Ball(-1);
		ball.Reset(isAlive: true, start);
		ball.updateChangementDeDirection(ball.Pos.Value2D, vector);
		if (ball.previsionCollisionFixe.bande.type == BandeObject.Type.ORTHO_X || ball.previsionCollisionFixe.bande.type == BandeObject.Type.ORTHO_Z)
		{
			Vector3 positionBallCollision = ball.previsionCollisionFixe.positionBallCollision;
			trajet.trouname = trouname;
			trajet.cnum = bNumber;
			trajet.cpos.Add(start);
			trajet.cpos.Add(new Vector2(positionBallCollision.X, positionBallCollision.Z));
			trajet.cpos.Add(realTarget);
			trajet.cdistance = Vector2.Distance(trajet.cpos[0], trajet.cpos[1]) + Vector2.Distance(trajet.cpos[1], trajet.cpos[2]);
			trajet.cdirection = vector;
			trajet.alive = MovePossible(bNumber, trajet.cpos[0], trajet.cpos[1]) && MovePossible(bNumber, trajet.cpos[1], trajet.cpos[2]);
		}
		else
		{
			trajet.alive = false;
		}
		return trajet;
	}

	private static bool MovePossible(int num, Vector2 start, Vector2 end)
	{
		bool flag = true;
		foreach (Ball ball in Statics.balls)
		{
			if (flag && ball.Alive && num != ball.Number)
			{
				float num2 = CollisionMobile.TimeOfClosestApproach(start, ball.Pos.Value2D, end - start, Vector2.Zero, 0.833333f, 0.833333f, out var collision);
				if (collision && num2 < 0.999f)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	public static void ComputeRepositionWball(GameTime gameTime)
	{
		startTime = gameTime.TotalGameTime.TotalSeconds;
		Vector2 zero = Vector2.Zero;
		do
		{
			zero = new Vector2(MathHelper.Lerp(-30f, 30f, Utils.RandomRatio), MathHelper.Lerp(-30f, 30f, Utils.RandomRatio));
		}
		while (!GameState.IsRepoWballValid(zero) || !GameState.IsPositionAvailable(0, zero));
		repoStart = Statics.balls[0].Pos.Value2D;
		repoEnd = zero;
	}

	public static void Draw()
	{
	}

	public static void Update(GameTime gameTime)
	{
		if (GameState.Current == GameState.Type.AIMING && GameModeRules.CurrentPlayer != GameModeRules.IndexCPU)
		{
			computed = false;
		}
		if (GameState.CpuAiming)
		{
			if (!computed)
			{
				if (Timer.Ratio(gameTime, startTime, 1.0) >= 1f)
				{
					ComputeMyTurn(gameTime);
				}
				return;
			}
			float num = Timer.Ratio(gameTime, startTime + 2.0, 2.0);
			Aiming.Set(MathHelper.SmoothStep((float)startAngle, (float)chosenAngle, num));
			Statics.cam.Update_Aiming_Normal(Aiming.AimVector);
			Cue.Update_Aiming();
			if (num >= 1f && !Statics.menus.Paused)
			{
				GameState.ChangeWithTransition(GameState.Type.WATCHING_MOVE, gameTime);
			}
		}
		if (GameState.Current == GameState.Type.REPOSITION_WBALL && GameModeRules.CurrentPlayer == GameModeRules.IndexCPU)
		{
			float num2 = Timer.Ratio(gameTime, startTime, 2.0);
			Ball ball = Statics.balls[0];
			ball.Pos.Set(Utils.LerpVector2(repoStart, repoEnd, num2));
			ball.UpdateDisplayMatrix();
			if (num2 >= 1f)
			{
				GameState.ChangeWithTransition(GameState.Type.AIMING, gameTime);
			}
		}
	}
}
