using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class Updates
{
	public class CollisionList
	{
		public class Item
		{
			public enum TypeID
			{
				Mobile,
				Bande,
				Skip
			}

			public float Time;

			public TypeID Type;

			public Point MobileBallIds;

			public int BandeBallId;

			public BandeObject Bande;

			public Item(float time, int ball0, int ball1)
			{
				Time = time;
				MobileBallIds = new Point(ball0, ball1);
				BandeBallId = -1;
				Type = TypeID.Mobile;
			}

			public Item(float time, int ball, BandeObject bande)
			{
				Time = time;
				MobileBallIds = new Point(-1, -1);
				BandeBallId = ball;
				Bande = bande;
				Type = TypeID.Bande;
			}

			public void DecrementTime(float timeIncr)
			{
				Time -= timeIncr;
			}
		}

		private static List<Item> data = new List<Item>();

		private static bool[] ballsRepositioned = new bool[28];

		private static float[] ballsRepoTime = new float[28];

		public static List<Item> Data => data;

		public static bool AlreadyRepositioned(int ballId)
		{
			return ballsRepositioned[ballId];
		}

		public static bool AlreadyRepositioned(Point ballIds)
		{
			if (!ballsRepositioned[ballIds.X])
			{
				return ballsRepositioned[ballIds.Y];
			}
			return true;
		}

		public static float RepositionTime(int ballId)
		{
			return ballsRepoTime[ballId];
		}

		public static void SetIsRepositioned(int ballId, float t, Item.TypeID ty)
		{
			ballsRepositioned[ballId] = true;
			ballsRepoTime[ballId] = t;
		}

		public static void Reset()
		{
			for (int i = 0; i < 28; i++)
			{
				ballsRepositioned[i] = false;
				ballsRepoTime[i] = 1f;
				Statics.balls[i].AppliedFullVelo = false;
			}
			data.Clear();
		}

		public static void Add(Item i)
		{
			int j;
			for (j = 0; j < data.Count && data[j].Time < i.Time; j++)
			{
			}
			data.Insert(j, i);
		}

		public static void Validate(Item i)
		{
			List<Ball> list = new List<Ball>();
			if (i.Type == Item.TypeID.Bande)
			{
				list.Add(Statics.balls[i.BandeBallId]);
			}
			else if (i.Type == Item.TypeID.Mobile)
			{
				list.Add(Statics.balls[i.MobileBallIds.X]);
				list.Add(Statics.balls[i.MobileBallIds.Y]);
			}
			foreach (Ball item in list)
			{
				SetIsRepositioned(item.Number, i.Time, i.Type);
			}
		}

		public static void DiscardCollInvolving(int num)
		{
			List<Item> list = new List<Item>();
			foreach (Item datum in Data)
			{
				if (datum.BandeBallId == num || datum.MobileBallIds.X == num || datum.MobileBallIds.Y == num)
				{
					list.Add(datum);
				}
			}
			foreach (Item item in list)
			{
				Data.Remove(item);
			}
		}
	}

	private static bool PositionsCollisions(GameTime gameTime)
	{
		bool result = false;
		CollisionList.Reset();
		foreach (Ball ball2 in Statics.balls)
		{
			ball2.CollisionBande_PreCompute();
		}
		CollisionMobile.PreCompute();
		DebugChecks(beforeUpdate: true);
		float num = 0f;
		List<Ball> list = new List<Ball>();
		while (CollisionList.Data.Count > 0)
		{
			result = true;
			list.Clear();
			CollisionList.Item item = CollisionList.Data[0];
			List<int> list2 = new List<int>();
			if (item.Type == CollisionList.Item.TypeID.Bande)
			{
				list2.Add(item.BandeBallId);
				Ball ball = Statics.balls[item.BandeBallId];
				if (ball.previsionCollisionFixe.tester.type == CollisionBande.Tester.Type.COLLISION_TROU)
				{
					Audio.PlaySFX(Audio.SFXID.BallTrou);
					ball.Kill(ball.previsionCollisionFixe.trou.pos);
				}
				else if (ball.previsionCollisionFixe.tester.type == CollisionBande.Tester.Type.COLLISION_BANDE)
				{
					Audio.PlaySFX(Audio.SFXID.BallBande, Utils.clampRatio((ball.Velo.Len + 1f) / 3f));
					ball.Pos.Set(ball.previsionCollisionFixe.positionBallCollision);
					ball.createAndAddCollisionFromBande_V8(ball.Pos.Value, item.Bande);
				}
			}
			else if (item.Type == CollisionList.Item.TypeID.Mobile)
			{
				if (item.MobileBallIds.X == 0 || item.MobileBallIds.Y == 0)
				{
					GameModeRules.Register_WhiteBall_ObjectBall_Collision((item.MobileBallIds.X == 0) ? item.MobileBallIds.Y : item.MobileBallIds.X);
				}
				Audio.PlaySFX(Audio.SFXID.BallBall, Utils.clampRatio((Statics.balls[item.MobileBallIds.X].Velo.Len + Statics.balls[item.MobileBallIds.Y].Velo.Len + 1f) / 3f));
				list2.Add(item.MobileBallIds.X);
				list2.Add(item.MobileBallIds.Y);
				CollisionMobile.Reposition_And_Impulses(new CollisionMobile.Info(item.Time, item.MobileBallIds));
				GameModeRules.MobileCollHasOccured();
			}
			foreach (Ball ball3 in Statics.balls)
			{
				if (ball3.Alive && !list2.Contains(ball3.Number))
				{
					ball3.ApplyVelo(item.Time);
					list.Add(ball3);
					DebugIntegrity(list);
				}
			}
			foreach (Ball ball4 in Statics.balls)
			{
				ball4.Update(item.Time);
			}
			CollisionList.Data.RemoveAt(0);
			foreach (int item2 in list2)
			{
				CollisionList.DiscardCollInvolving(item2);
			}
			num += item.Time;
			DebugCurrentTime(num);
			foreach (CollisionList.Item datum in CollisionList.Data)
			{
				datum.DecrementTime(item.Time);
			}
			foreach (int item3 in list2)
			{
				Statics.balls[item3].CollisionBande_PreCompute(1f - num);
			}
			CollisionMobile.PreCompute(list2, 1f - num);
		}
		list.Clear();
		DebugCurrentTime(num);
		foreach (Ball ball5 in Statics.balls)
		{
			if (ball5.Alive)
			{
				if (ball5.Velo.Len != 0f)
				{
					result = true;
				}
				ball5.ApplyVelo(1f - num);
				list.Add(ball5);
				DebugIntegrity(list);
			}
			else if (ball5.state == Ball.State.DYING)
			{
				ball5.UpdateDying();
			}
		}
		foreach (Ball ball6 in Statics.balls)
		{
			ball6.Update(1f - num);
		}
		foreach (Ball ball7 in Statics.balls)
		{
			ball7.createFriction_V2(gameTime);
		}
		DebugChecks(beforeUpdate: false);
		return result;
	}

	public static void Update(GameTime gameTime)
	{
		if (Statics.callbacks.DeferredEnableMenusTime >= 0.0 && Statics.callbacks.DeferredEnableMenusTime < gameTime.TotalGameTime.TotalSeconds)
		{
			Statics.callbacks.DeferredEnableMenusTime = -2.0;
			Statics.menus.Enable();
		}
		Statics.input.Update(gameTime);
		ChoosePower.Update(gameTime);
		Statics.cam.Update(gameTime);
		Cue.Update(gameTime);
		Statics.menus.Update(gameTime);
		InfoDisplay.Update();
		Bot.Update(gameTime);
		Audio.Update();
		GameModeRules.Update(gameTime);
		bool flag = PositionsCollisions(gameTime);
		switch (GameState.Current)
		{
		case GameState.Type.TRANSITION:
			if (!Statics.cam.Transitioning)
			{
				GameState.EndTransition(gameTime);
			}
			break;
		case GameState.Type.WATCHING_MOVE:
			if (!flag && !GameState.AnyBallDying && !GameModeRules.deferredInitiateNextTurn)
			{
				GameModeRules.NewTurn(gameTime);
			}
			break;
		}
	}

	public static void DebugIntegrity(List<Ball> repositionedBalls)
	{
	}

	public static void DebugCurrentTime(float currentTime)
	{
	}

	private static void DebugChecks(bool beforeUpdate)
	{
	}
}
