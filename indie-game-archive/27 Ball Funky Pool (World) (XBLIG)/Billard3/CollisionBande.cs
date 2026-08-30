using System;
using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class CollisionBande
{
	public class Tester
	{
		public enum Type
		{
			COLLISION_BANDE,
			COLLISION_TROU
		}

		public Type type;

		public bool isTestOnX;

		public float testValue;

		public Tester(Type type, bool isTestOnX, float testValue)
		{
			this.type = type;
			this.isTestOnX = isTestOnX;
			this.testValue = testValue;
		}

		public Tester()
		{
		}

		private void set(Type type, bool isTestOnX, float testValue)
		{
			this.type = type;
			this.isTestOnX = isTestOnX;
			this.testValue = testValue;
		}

		public void setAutoChooseDimension(Type type, Vector2 stopPoint, Vector2 vel)
		{
			this.type = type;
			if (Math.Abs(vel.X) > Math.Abs(vel.Y))
			{
				isTestOnX = true;
				testValue = stopPoint.X;
			}
			else
			{
				isTestOnX = false;
				testValue = stopPoint.Y;
			}
		}
	}

	private class ZoneTrouCentral
	{
		private const float X_P0 = 1.239f;

		private const float Y_P0 = 49.166668f;

		private const float X_P1 = 0.7792364f;

		private const float Y_P1 = 29.305374f;

		private const float X_P2 = 0.47235602f;

		private const float Y_P2 = 29.674198f;

		private const float X_P3 = 0.3292066f;

		private const float Y_P3 = 30.01104f;

		private const float X_P4 = 0f;

		private const float Y_P4 = 29.948f;

		private const float X_P5 = -0.3292066f;

		private const float Y_P5 = 30.01104f;

		private const float X_P6 = -0.47235602f;

		private const float Y_P6 = 29.674198f;

		private const float X_P7 = -0.7792364f;

		private const float Y_P7 = 29.305374f;

		private const float X_P8 = -1.239f;

		private const float Y_P8 = 49.166668f;

		private const float X_PAngle = 1.239f;

		private const float Y_PAngle = 30f;

		public static Vector2 P0_ZP = new Vector2(1.239f, 49.166668f);

		public static Vector2 P1_ZP = new Vector2(0.7792364f, 29.305374f);

		public static Vector2 P2_ZP = new Vector2(0.47235602f, 29.674198f);

		public static Vector2 P3_ZP = new Vector2(0.3292066f, 30.01104f);

		public static Vector2 P4_ZP = new Vector2(0f, 29.948f);

		public static Vector2 P5_ZP = new Vector2(-0.3292066f, 30.01104f);

		public static Vector2 P6_ZP = new Vector2(-0.47235602f, 29.674198f);

		public static Vector2 P7_ZP = new Vector2(-0.7792364f, 29.305374f);

		public static Vector2 P8_ZP = new Vector2(-1.239f, 49.166668f);

		public static Vector2 P0_ZM = new Vector2(1.239f, -49.166668f);

		public static Vector2 P1_ZM = new Vector2(0.7792364f, -29.305374f);

		public static Vector2 P2_ZM = new Vector2(0.47235602f, -29.674198f);

		public static Vector2 P3_ZM = new Vector2(0.3292066f, -30.01104f);

		public static Vector2 P4_ZM = new Vector2(0f, -29.948f);

		public static Vector2 P5_ZM = new Vector2(-0.3292066f, -30.01104f);

		public static Vector2 P6_ZM = new Vector2(-0.47235602f, -29.674198f);

		public static Vector2 P7_ZM = new Vector2(-0.7792364f, -29.305374f);

		public static Vector2 P8_ZM = new Vector2(-1.239f, -49.166668f);

		public static Vector2 PAngle_XP_ZP = new Vector2(1.239f, 30f);

		public static Vector2 PAngle_XP_ZM = new Vector2(1.239f, -30f);

		public static Vector2 PAngle_XM_ZP = new Vector2(-1.239f, 30f);

		public static Vector2 PAngle_XM_ZM = new Vector2(-1.239f, -30f);
	}

	private class ZoneTrouCorner
	{
		public const float X_P0 = 28.271f;

		public const float Y_P0 = 49.166668f;

		public const float X_P1 = 28.62597f;

		public const float Y_P1 = 29.24642f;

		public const float X_P2 = 28.91326f;

		public const float Y_P2 = 29.469538f;

		public const float X_P3 = 29.23319f;

		public const float Y_P3 = 29.856903f;

		public const float X_P4 = 29.50589f;

		public const float Y_P4 = 29.50589f;

		public const float X_P5 = 29.856901f;

		public const float Y_P5 = 29.233189f;

		public const float X_P6 = 29.469538f;

		public const float Y_P6 = 28.913261f;

		public const float X_P7 = 29.24642f;

		public const float Y_P7 = 28.625969f;

		public const float X_P8 = 29.166668f;

		public const float Y_P8 = 28.271f;

		public const float X_PAngle0 = 28.271f;

		public const float Y_PAngle0 = 30f;

		public const float X_PAngle8 = 30f;

		public const float Y_PAngle8 = 28.271f;
	}

	private const float limitX_Ortho = 29.166668f;

	private const float limitZ_Ortho = 29.166668f;

	private const float xLimit = 30f;

	private const float yLimit = 30f;

	private const float cornerOffset = 1.729f;

	private const float centralOffset = 1.239f;

	public static List<Trou> listTrous = new List<Trou>();

	public static List<BandeObject> listBandes = new List<BandeObject>();

	public bool alive;

	public BandeObject bande;

	public Trou trou;

	public Vector3 positionBallCollision;

	public Tester tester;

	public static void Initialize()
	{
		listTrous.Clear();
		listTrous.Add(Trous.trouXMZP);
		listTrous.Add(Trous.trouXMZM);
		listTrous.Add(Trous.trouX0ZP);
		listTrous.Add(Trous.trouX0ZM);
		listTrous.Add(Trous.trouXPZP);
		listTrous.Add(Trous.trouXPZM);
		listBandes.Clear();
		listBandes.Add(Bandes.bandeCorner_XPZP_Largeur);
		listBandes.Add(Bandes.bandeCorner_XPZP_Longueur);
		listBandes.Add(Bandes.bandeCorner_XPZM_Largeur);
		listBandes.Add(Bandes.bandeCorner_XPZM_Longueur);
		listBandes.Add(Bandes.bandeCorner_XMZP_Largeur);
		listBandes.Add(Bandes.bandeCorner_XMZP_Longueur);
		listBandes.Add(Bandes.bandeCorner_XMZM_Largeur);
		listBandes.Add(Bandes.bandeCorner_XMZM_Longueur);
		listBandes.Add(Bandes.bandeTrouCentralXPZP);
		listBandes.Add(Bandes.bandeTrouCentralXPZM);
		listBandes.Add(Bandes.bandeTrouCentralXMZP);
		listBandes.Add(Bandes.bandeTrouCentralXMZM);
		listBandes.Add(Bandes.bandeOrthoZ_XP);
		listBandes.Add(Bandes.bandeOrthoZ_XM);
		listBandes.Add(Bandes.bandeOrthoX_ZP);
		listBandes.Add(Bandes.bandeOrthoX_ZM);
		FunkyBandes.Initialize();
	}

	public bool applyTest(Vector2 pos2, Vector2 vel2, out float time)
	{
		Vector2 vector = pos2 + vel2;
		if (!alive)
		{
			time = -1f;
			return false;
		}
		bool result;
		if (tester.isTestOnX)
		{
			result = ((!(vel2.X >= 0f)) ? (vector.X <= tester.testValue) : (vector.X >= tester.testValue));
			if (vector.X == tester.testValue)
			{
				time = 1f;
			}
			else
			{
				time = (tester.testValue - pos2.X) / vel2.X;
			}
		}
		else
		{
			result = ((!(vel2.Y >= 0f)) ? (vector.Y <= tester.testValue) : (vector.Y >= tester.testValue));
			if (vector.Y == tester.testValue)
			{
				time = 1f;
			}
			else
			{
				time = (tester.testValue - pos2.Y) / vel2.Y;
			}
		}
		time = MathHelper.Clamp(time, 0f, 1f);
		return result;
	}

	public CollisionBande()
	{
		bande = null;
		trou = null;
		alive = false;
	}

	public static bool TestColl_Trou(Vector2 pos, Vector2 vel, Trou trou, out float collTime, out float distance)
	{
		collTime = CollisionMobile.TimeOfClosestApproach(pos, trou.pos, vel, Vector2.Zero, 0f, trou.rayon, out var collision);
		distance = (collision ? (vel.Length() * collTime) : (-1f));
		return collision;
	}

	public static bool TestColl_BandeOrtho_Extremite(Vector2 pos, Vector2 vel, Vector2 posAngle, out float collTime, out float distance)
	{
		collTime = CollisionMobile.TimeOfClosestApproach(pos, posAngle, vel, Vector2.Zero, 0.833333f, 0f, out var collision);
		distance = (collision ? (vel.Length() * collTime) : (-1f));
		return collision;
	}

	public CollisionBande(CollisionBande clone)
	{
		bande = clone.bande;
		trou = clone.trou;
		alive = clone.alive;
		positionBallCollision = clone.positionBallCollision;
		tester = clone.tester;
	}

	public bool initialise(Vector2 pos, Vector2 vel)
	{
		alive = false;
		float num = 120f;
		BandeObject bandeObject = new BandeObject();
		Vector2 pos2 = default(Vector2);
		Vector2 IPout = default(Vector2);
		Tester tester = new Tester();
		if (testIntersectionEntreDemiDroiteEt6BandesOrthos(pos, vel, out var IPout2, out var distanceOut, out var bandeOut, out var testerOut) && distanceOut < num)
		{
			alive = true;
			num = distanceOut;
			pos2 = IPout2;
			bandeObject = bandeOut;
			tester = testerOut;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCentral(pos, vel, isTrouCentralZPos: true, out IPout, out var distanceOut2, out var bandeOut2, out var testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouX0ZP;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCentral(pos, vel, isTrouCentralZPos: false, out IPout, out distanceOut2, out bandeOut2, out testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouX0ZM;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCORNER(pos, vel, isZoneTrouXPos: true, isZoneTrouZPos: true, out IPout, out distanceOut2, out bandeOut2, out testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouXPZP;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCORNER(pos, vel, isZoneTrouXPos: true, isZoneTrouZPos: false, out IPout, out distanceOut2, out bandeOut2, out testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouXPZM;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCORNER(pos, vel, isZoneTrouXPos: false, isZoneTrouZPos: true, out IPout, out distanceOut2, out bandeOut2, out testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouXMZP;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (testIntersectionEntreDemiDroiteEtZoneTrouCORNER(pos, vel, isZoneTrouXPos: false, isZoneTrouZPos: false, out IPout, out distanceOut2, out bandeOut2, out testerOut2) && distanceOut2 < num)
		{
			if (testerOut2.type == Tester.Type.COLLISION_TROU)
			{
				trou = Trous.trouXMZM;
			}
			alive = true;
			num = distanceOut2;
			pos2 = IPout;
			bandeObject = bandeOut2;
			tester = testerOut2;
		}
		if (alive)
		{
			bande = bandeObject;
			positionBallCollision = new Vector3(pos2.X, 0.833333f, pos2.Y);
			GameState.IsPositionValid(pos2);
			this.tester = tester;
			return true;
		}
		return false;
	}

	private bool testIntersectionEntreDemiDroiteEt6BandesOrthos(Vector2 pos, Vector2 vel, out Vector2 IPout, out float distanceOut, out BandeObject bandeOut, out Tester testerOut)
	{
		bool flag = false;
		Vector2 intersectionPoint = default(Vector2);
		float num = 120f;
		BandeObject bandeObject = new BandeObject();
		Vector2 vector = default(Vector2);
		Tester tester = new Tester();
		IPout = vector;
		distanceOut = num;
		bandeOut = bandeObject;
		testerOut = tester;
		foreach (FunkyBandes.CollisionInfoFourBande item in FunkyBandes.listCollisionInfo)
		{
			FunkyBandes.CollisionInfoOneBande[] data = item.Data;
			foreach (FunkyBandes.CollisionInfoOneBande collisionInfoOneBande in data)
			{
				if (collisionInfoOneBande.Test(vel) && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, collisionInfoOneBande.P0, collisionInfoOneBande.P1, out intersectionPoint))
				{
					float num2 = Vector2.Distance(pos, intersectionPoint);
					if (num2 < num)
					{
						flag = true;
						alive = true;
						num = num2;
						vector = intersectionPoint;
						bandeObject = collisionInfoOneBande.Bande;
						tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
						collisionInfoOneBande.Hit = true;
					}
				}
			}
		}
		if (vel.Y > 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(28.271f, 29.166668f), new Vector2(1.239f, 29.166668f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoX_ZP;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (vel.Y > 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(-28.271f, 29.166668f), new Vector2(-1.239f, 29.166668f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoX_ZP;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (vel.Y < 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(28.271f, -29.166668f), new Vector2(1.239f, -29.166668f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoX_ZM;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (vel.Y < 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(-28.271f, -29.166668f), new Vector2(-1.239f, -29.166668f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoX_ZM;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (vel.X > 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(29.166668f, 28.271f), new Vector2(29.166668f, -28.271f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoZ_XP;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (vel.X < 0f && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, new Vector2(-29.166668f, 28.271f), new Vector2(-29.166668f, -28.271f), out intersectionPoint))
		{
			float num2 = Vector2.Distance(pos, intersectionPoint);
			if (num2 < num)
			{
				flag = true;
				alive = true;
				num = num2;
				vector = intersectionPoint;
				bandeObject = Bandes.bandeOrthoZ_XM;
				tester.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, intersectionPoint, vel);
			}
		}
		if (flag)
		{
			IPout = vector;
			distanceOut = num;
			bandeOut = bandeObject;
			testerOut = tester;
		}
		return flag;
	}

	private bool testIntersectionEntreDemiDroiteEtZoneTrouCORNER(Vector2 pos, Vector2 vel, bool isZoneTrouXPos, bool isZoneTrouZPos, out Vector2 IPout, out float distanceOut, out BandeObject bandeOut, out Tester testerOut)
	{
		bool flag = false;
		Vector2 intersectionPoint = default(Vector2);
		float num = 120f;
		testerOut = new Tester();
		IPout = Vector2.Zero;
		distanceOut = 0f;
		bandeOut = null;
		Vector2 p;
		Vector2 vector;
		Vector2 vector2;
		Vector2 segP;
		Vector2 segP2;
		Vector2 vector3;
		Vector2 vector4;
		Vector2 p2;
		Vector2 posAngle;
		Vector2 posAngle2;
		BandeObject bandeObject;
		BandeObject bandeObject2;
		if (isZoneTrouXPos)
		{
			if (isZoneTrouZPos)
			{
				p = new Vector2(28.271f, 49.166668f);
				vector = new Vector2(28.62597f, 29.24642f);
				vector2 = new Vector2(28.91326f, 29.469538f);
				segP = new Vector2(29.23319f, 29.856903f);
				new Vector2(29.50589f, 29.50589f);
				segP2 = new Vector2(29.856901f, 29.233189f);
				vector3 = new Vector2(29.469538f, 28.913261f);
				vector4 = new Vector2(29.24642f, 28.625969f);
				p2 = new Vector2(29.166668f, 28.271f);
				posAngle = new Vector2(28.271f, 30f);
				posAngle2 = new Vector2(30f, 28.271f);
				_ = Bandes.bandeOrthoX_ZP;
				_ = Bandes.bandeOrthoZ_XP;
				bandeObject = Bandes.bandeCorner_XPZP_Longueur;
				bandeObject2 = Bandes.bandeCorner_XPZP_Largeur;
			}
			else
			{
				p = new Vector2(28.271f, -49.166668f);
				vector = new Vector2(28.62597f, -29.24642f);
				vector2 = new Vector2(28.91326f, -29.469538f);
				segP = new Vector2(29.23319f, -29.856903f);
				new Vector2(29.50589f, -29.50589f);
				segP2 = new Vector2(29.856901f, -29.233189f);
				vector3 = new Vector2(29.469538f, -28.913261f);
				vector4 = new Vector2(29.24642f, -28.625969f);
				p2 = new Vector2(29.166668f, -28.271f);
				posAngle = new Vector2(28.271f, -30f);
				posAngle2 = new Vector2(30f, -28.271f);
				_ = Bandes.bandeOrthoX_ZM;
				_ = Bandes.bandeOrthoZ_XP;
				bandeObject = Bandes.bandeCorner_XPZM_Longueur;
				bandeObject2 = Bandes.bandeCorner_XPZM_Largeur;
			}
		}
		else if (isZoneTrouZPos)
		{
			p = new Vector2(-28.271f, 49.166668f);
			vector = new Vector2(-28.62597f, 29.24642f);
			vector2 = new Vector2(-28.91326f, 29.469538f);
			segP = new Vector2(-29.23319f, 29.856903f);
			new Vector2(-29.50589f, 29.50589f);
			segP2 = new Vector2(-29.856901f, 29.233189f);
			vector3 = new Vector2(-29.469538f, 28.913261f);
			vector4 = new Vector2(-29.24642f, 28.625969f);
			p2 = new Vector2(-29.166668f, 28.271f);
			posAngle = new Vector2(-28.271f, 30f);
			posAngle2 = new Vector2(-30f, 28.271f);
			_ = Bandes.bandeOrthoX_ZP;
			_ = Bandes.bandeOrthoZ_XM;
			bandeObject = Bandes.bandeCorner_XMZP_Longueur;
			bandeObject2 = Bandes.bandeCorner_XMZP_Largeur;
		}
		else
		{
			p = new Vector2(-28.271f, -49.166668f);
			vector = new Vector2(-28.62597f, -29.24642f);
			vector2 = new Vector2(-28.91326f, -29.469538f);
			segP = new Vector2(-29.23319f, -29.856903f);
			new Vector2(-29.50589f, -29.50589f);
			segP2 = new Vector2(-29.856901f, -29.233189f);
			vector3 = new Vector2(-29.469538f, -28.913261f);
			vector4 = new Vector2(-29.24642f, -28.625969f);
			p2 = new Vector2(-29.166668f, -28.271f);
			posAngle = new Vector2(-28.271f, -30f);
			posAngle2 = new Vector2(-30f, -28.271f);
			_ = Bandes.bandeOrthoX_ZM;
			_ = Bandes.bandeOrthoZ_XM;
			bandeObject = Bandes.bandeCorner_XMZM_Longueur;
			bandeObject2 = Bandes.bandeCorner_XMZM_Largeur;
		}
		Vector2 pointTrouUtilisePourSensVecteurRejet = (vector + vector4) * 0.5f;
		new BandeObject(bandeObject.id, bandeObject2.type, p, vector, pointTrouUtilisePourSensVecteurRejet, bandeObject2.name + " COLLISION ANGULAIRE P0 P1");
		new BandeObject(bandeObject.id, bandeObject2.type, vector, vector2, pointTrouUtilisePourSensVecteurRejet, bandeObject2.name + " COLLISION ANGULAIRE P1 P2");
		new BandeObject(bandeObject2.id, bandeObject2.type, vector3, vector4, pointTrouUtilisePourSensVecteurRejet, bandeObject2.name + " COLLISION ANGULAIRE P6 P7");
		new BandeObject(bandeObject2.id, bandeObject2.type, vector4, p2, pointTrouUtilisePourSensVecteurRejet, bandeObject2.name + " COLLISION ANGULAIRE P7 P8");
		if (TestColl_BandeOrtho_Extremite(pos, vel, posAngle, out var collTime, out var distance) && distance < num)
		{
			repositionCollisionAngulaire(pos, vel, collTime, posAngle, pointTrouUtilisePourSensVecteurRejet, out IPout, out bandeOut);
			flag = true;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
		}
		if (!OldMath.testBallProcheSegment(vector2, segP, pos) && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, vector2, segP, out intersectionPoint))
		{
			distance = Vector2.Distance(pos, intersectionPoint);
			if (distance < num)
			{
				flag = true;
				num = distance;
				distanceOut = distance;
				IPout = intersectionPoint;
				bandeOut = bandeObject;
				testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
			}
		}
		Trou trou = ((!isZoneTrouXPos) ? (isZoneTrouZPos ? Trous.trouXMZP : Trous.trouXMZM) : (isZoneTrouZPos ? Trous.trouXPZP : Trous.trouXPZM));
		if (TestColl_Trou(pos, vel, trou, out collTime, out distance) && distance < num)
		{
			flag = true;
			IPout = pos + vel * collTime;
			bandeOut = Bandes.specialCollisionTrou;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_TROU, IPout, vel);
		}
		if (!OldMath.testBallProcheSegment(segP2, vector3, pos) && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, segP2, vector3, out intersectionPoint))
		{
			distance = Vector2.Distance(pos, intersectionPoint);
			if (distance < num)
			{
				flag = true;
				num = distance;
				distanceOut = distance;
				IPout = intersectionPoint;
				bandeOut = bandeObject2;
				testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
			}
		}
		if (TestColl_BandeOrtho_Extremite(pos, vel, posAngle2, out collTime, out distance) && distance < num)
		{
			repositionCollisionAngulaire(pos, vel, collTime, posAngle2, pointTrouUtilisePourSensVecteurRejet, out IPout, out bandeOut);
			flag = true;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
		}
		if (flag)
		{
			distanceOut = num;
		}
		return flag;
	}

	private bool testIntersectionEntreDemiDroiteEtZoneTrouCentral(Vector2 pos, Vector2 vel, bool isTrouCentralZPos, out Vector2 IPout, out float distanceOut, out BandeObject bandeOut, out Tester testerOut)
	{
		bool flag = false;
		Vector2 intersectionPoint = default(Vector2);
		float num = 120f;
		testerOut = new Tester();
		IPout = Vector2.Zero;
		distanceOut = 0f;
		bandeOut = null;
		_ = Bandes.bandeOrthoX_ZP;
		BandeObject bandeObject = Bandes.bandeTrouCentralXPZP;
		BandeObject bandeObject2 = Bandes.bandeTrouCentralXMZP;
		Vector2 p = ZoneTrouCentral.P0_ZP;
		Vector2 vector = ZoneTrouCentral.P1_ZP;
		Vector2 vector2 = ZoneTrouCentral.P2_ZP;
		Vector2 segP = ZoneTrouCentral.P3_ZP;
		_ = ZoneTrouCentral.P4_ZP;
		Vector2 segP2 = ZoneTrouCentral.P5_ZP;
		Vector2 vector3 = ZoneTrouCentral.P6_ZP;
		Vector2 vector4 = ZoneTrouCentral.P7_ZP;
		Vector2 p2 = ZoneTrouCentral.P8_ZP;
		Vector2 posAngle = ZoneTrouCentral.PAngle_XP_ZP;
		Vector2 posAngle2 = ZoneTrouCentral.PAngle_XM_ZP;
		if (!isTrouCentralZPos)
		{
			_ = Bandes.bandeOrthoX_ZM;
			bandeObject = Bandes.bandeTrouCentralXPZM;
			bandeObject2 = Bandes.bandeTrouCentralXMZM;
			p = ZoneTrouCentral.P0_ZM;
			vector = ZoneTrouCentral.P1_ZM;
			vector2 = ZoneTrouCentral.P2_ZM;
			segP = ZoneTrouCentral.P3_ZM;
			_ = ZoneTrouCentral.P4_ZM;
			segP2 = ZoneTrouCentral.P5_ZM;
			vector3 = ZoneTrouCentral.P6_ZM;
			vector4 = ZoneTrouCentral.P7_ZM;
			p2 = ZoneTrouCentral.P8_ZM;
			posAngle = ZoneTrouCentral.PAngle_XP_ZM;
			posAngle2 = ZoneTrouCentral.PAngle_XM_ZM;
		}
		new BandeObject(bandeObject.id, bandeObject.type, p, vector, Vector2.Zero, bandeObject.name + " COLLISION ANGULAIRE P0 P1");
		new BandeObject(bandeObject.id, bandeObject.type, vector, vector2, Vector2.Zero, bandeObject.name + " COLLISION ANGULAIRE P1 P2");
		new BandeObject(bandeObject2.id, bandeObject2.type, vector3, vector4, Vector2.Zero, bandeObject2.name + " COLLISION ANGULAIRE P6 P7");
		new BandeObject(bandeObject2.id, bandeObject2.type, vector4, p2, Vector2.Zero, bandeObject2.name + " COLLISION ANGULAIRE P7 P8");
		if (TestColl_BandeOrtho_Extremite(pos, vel, posAngle, out var collTime, out var distance) && distance < num)
		{
			repositionCollisionAngulaire(pos, vel, collTime, posAngle, Vector2.Zero, out IPout, out bandeOut);
			flag = true;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
		}
		if (!OldMath.testBallProcheSegment(vector2, segP, pos) && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, vector2, segP, out intersectionPoint))
		{
			distance = Vector2.Distance(pos, intersectionPoint);
			if (distance < num)
			{
				flag = true;
				num = distance;
				distanceOut = distance;
				IPout = intersectionPoint;
				bandeOut = bandeObject;
				testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
			}
		}
		Trou trou = (isTrouCentralZPos ? Trous.trouX0ZP : Trous.trouX0ZM);
		if (TestColl_Trou(pos, vel, trou, out collTime, out distance) && distance < num)
		{
			flag = true;
			IPout = pos + vel * collTime;
			bandeOut = Bandes.specialCollisionTrou;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_TROU, IPout, vel);
		}
		if (!OldMath.testBallProcheSegment(segP2, vector3, pos) && OldMath.testIntersectionEntreDemiDroiteEtSegment(pos, vel, segP2, vector3, out intersectionPoint))
		{
			distance = Vector2.Distance(pos, intersectionPoint);
			if (distance < num)
			{
				flag = true;
				num = distance;
				distanceOut = distance;
				IPout = intersectionPoint;
				bandeOut = bandeObject2;
				testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
			}
		}
		if (TestColl_BandeOrtho_Extremite(pos, vel, posAngle2, out collTime, out distance) && distance < num)
		{
			repositionCollisionAngulaire(pos, vel, collTime, posAngle2, Vector2.Zero, out IPout, out bandeOut);
			flag = true;
			num = distance;
			testerOut.setAutoChooseDimension(Tester.Type.COLLISION_BANDE, IPout, vel);
		}
		if (flag)
		{
			distanceOut = num;
		}
		return flag;
	}

	private static void repositionCollisionAngulaire(Vector2 pos, Vector2 velo, float timeOfColl, Vector2 posAngle, Vector2 pointTrouUtilisePourSensVecteurRejet, out Vector2 collPoint, out BandeObject bande)
	{
		collPoint = pos + velo * timeOfColl;
		Vector2 vector = MyMath.Vector2Orthogonal(Vector2.Normalize(collPoint - posAngle));
		bande = new BandeObject(BandeObject.Id.CUSTOM, (BandeObject.Type)(-1), posAngle + vector, posAngle - vector, pointTrouUtilisePourSensVecteurRejet, "Custom Coll Angulaire " + posAngle);
	}

	public string ToString(Vector2 vel)
	{
		if (!alive)
		{
			return "False";
		}
		string text = "";
		switch (tester.type)
		{
		case Tester.Type.COLLISION_TROU:
			text += "Trou ";
			break;
		case Tester.Type.COLLISION_BANDE:
			text = text + bande.name + " ";
			break;
		}
		if (tester.isTestOnX)
		{
			text += "(COND X ";
			text = ((!(vel.X >= 0f)) ? (text + "<= ") : (text + ">= "));
		}
		else
		{
			text += "(COND Z ";
			text = ((!(vel.Y >= 0f)) ? (text + "<= ") : (text + ">= "));
		}
		return text + tester.testValue + " )";
	}
}
