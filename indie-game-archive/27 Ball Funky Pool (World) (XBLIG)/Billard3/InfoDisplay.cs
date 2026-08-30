using System;
using System.Collections.Generic;
using System.Linq;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class InfoDisplay
{
	public const float Height = 0.07f;

	private static RoundedRectangle bg0;

	private static RoundedRectangle bg1;

	private static RoundedRectangle currentP;

	private static List<int>[] pocketed = new List<int>[2]
	{
		new List<int>(),
		new List<int>()
	};

	public static Matrix ViewMat;

	public static Matrix ProjMat;

	private static RenderTarget2D[] Renders = new RenderTarget2D[2];

	private static bool[] ComputedPos = new bool[28];

	private static Vector3[] FinalPos = new Vector3[28];

	private static readonly Vector3 finalPosOffset = 1.666666f * Vector3.UnitX;

	private static readonly Vector3 posZ = Vector3.UnitZ * 1f * 0.833333f * 2f;

	private static readonly Vector3 posZero = finalPosOffset * -3f + posZ;

	private static readonly Vector3 posStart = finalPosOffset * 4f + posZ;

	public static bool AnyBallPocketed
	{
		get
		{
			if (pocketed[0].Count <= 0)
			{
				return pocketed[1].Count > 0;
			}
			return true;
		}
	}

	public static RenderTarget2D Render(int ballNum)
	{
		return Renders[GameModeRules.TeamNumForBallNum(ballNum)];
	}

	public static void Initialize(Point ScreenSize)
	{
		int texWidth = ScreenSize.X / 80;
		bg0 = new RoundedRectangle(Rectangle.Empty);
		bg1 = new RoundedRectangle(Rectangle.Empty);
		bg0.TexWidth = texWidth;
		bg1.TexWidth = texWidth;
		bg0.Color = GameModeRules.Team.Colors[0];
		bg1.Color = GameModeRules.Team.Colors[1];
		Rectangle empty = Rectangle.Empty;
		empty.Height = ScreenSize.Y / 15;
		empty.Width = empty.Height / 2 * 12;
		if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			empty.Height *= 2;
		}
		empty.Y = (int)((float)ScreenSize.Y * 0.07f);
		empty.X = ScreenSize.X / 12;
		bg0.Rect = empty;
		empty.X = ScreenSize.X * 11 / 12 - empty.Width;
		bg1.Rect = empty;
		Point point = new Point((int)((float)ScreenSize.X * 0.21f), (int)((float)ScreenSize.Y * 0.055f));
		if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			point.Y *= 2;
		}
		Vector3 vector = Vector3.UnitZ * 0.833333f * 4f + Vector3.Zero;
		ViewMat = Matrix.CreateLookAt(vector, vector - Vector3.UnitZ, Vector3.Up);
		float num = 12.083328f;
		float height = num * (float)point.Y / (float)point.X;
		ProjMat = Matrix.CreateOrthographic(num, height, 0.5f, 10f);
		for (int i = 0; i < Renders.Count(); i++)
		{
			Renders[i] = new RenderTarget2D(Statics.draw2D.Device, point.X, point.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth16, 4, RenderTargetUsage.DiscardContents);
		}
		currentP = new RoundedRectangle(new Rectangle(0, bg0.Rect.Y + ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? (bg0.Rect.Height / 4) : 0), (int)((float)bg0.Rect.Height * 1.5f) / ((MaximinusGame.Id != MaximinusGame.ID.FunkyPool) ? 1 : 2), bg0.Rect.Height / ((MaximinusGame.Id != MaximinusGame.ID.FunkyPool) ? 1 : 2)));
		currentP.TexWidth = texWidth;
	}

	public static void DrawToTexture()
	{
		if (GameState.InMenu)
		{
			return;
		}
		GameModeRules.Team[] allTeams = GameModeRules.AllTeams;
		foreach (GameModeRules.Team team in allTeams)
		{
			if (team.Pocketed.Count <= 0)
			{
				continue;
			}
			Drawing2D.SetAndClearRender(Statics.draw2D.Device, Renders[team.number], Color.Black);
			Statics.draw2D.PrepareFor3D();
			for (int num = team.Pocketed.Count - 1; num >= 0; num--)
			{
				int index = team.Pocketed[num];
				Ball ball = Statics.balls[index];
				if (ball.state == Ball.State.DEAD && NewPosComputedFor(ball.Number))
				{
					ball.DrawDead();
				}
			}
		}
		Statics.draw2D.Device.SetRenderTarget(null);
	}

	public static void Draw(SpriteBatch sb)
	{
		if (GameState.InMenu || CameraBillard.BoxShot)
		{
			return;
		}
		bg0.Draw(sb);
		bg1.Draw(sb);
		int num = bg0.Rect.X + bg0.Rect.Width;
		int num2 = bg0.Rect.Y + bg0.Rect.Height / 2;
		SpriteFont font = Statics.draw2D.Font;
		int num3 = 0;
		for (int i = 0; i < pocketed.Count(); i++)
		{
			_ = pocketed[i];
			Color color = GameModeRules.Team.Colors[i];
			List<PlayerIndex> list = GameModeRules.PlayerIndexes(num3);
			for (int j = 0; j < list.Count; j++)
			{
				PlayerIndex playerIndex = list[j];
				string text = GameModeRules.Team.NameOf(playerIndex);
				Vector2 vector = font.MeasureString(text);
				Vector2 vector2 = new Vector2(num + ((num3 == 0) ? (currentP.Rect.Width * (1 + j)) : (currentP.Rect.Width * (1 + j) * -1)), num2);
				if (playerIndex == GameModeRules.CurrentPlayer)
				{
					Statics.draw2D.DrawStringWithSelectEffect(text, vector2 + vector * -0.5f, color, Utils.ColorWithAlpha(Color.Black, 0.66f), 1f, 1f);
					currentP.Rect = new Rectangle((int)(vector2.X - (float)(currentP.Rect.Width / 2)), currentP.Rect.Y, currentP.Rect.Width, currentP.Rect.Height);
					currentP.Color = color;
					currentP.Draw(sb);
					sb.Draw(Statics.draw2D.BlankTex, currentP.Rect, null, Utils.ColorWithAlpha(Color.White, 0.33f), 0f, Vector2.Zero, SpriteEffects.None, 1f);
				}
				else
				{
					sb.DrawString(font, text, vector2, color, 0f, vector * 0.5f, 1f, SpriteEffects.None, 0f);
				}
			}
			num = bg1.Rect.X;
			num3++;
		}
		_ = Statics.draw2D.ScreenSizePoint;
		Rectangle rect = bg0.Rect;
		RenderTarget2D[] renders = Renders;
		foreach (RenderTarget2D renderTarget2D in renders)
		{
			sb.Draw(Statics.draw2D.BlankTex, rect, null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, 1f);
			sb.Draw(renderTarget2D, new Vector2(rect.Center.X - renderTarget2D.Width / 2, rect.Center.Y - renderTarget2D.Height / 2), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
			rect = bg1.Rect;
		}
	}

	public static void Reset()
	{
		List<int>[] array = pocketed;
		foreach (List<int> list in array)
		{
			list.Clear();
		}
		for (int j = 0; j < ComputedPos.Count(); j++)
		{
			ComputedPos[j] = false;
		}
		RenderTarget2D[] renders = Renders;
		foreach (RenderTarget2D r in renders)
		{
			Drawing2D.SetAndClearRender(Statics.draw2D.Device, r, Color.Black);
		}
		Statics.draw2D.Device.SetRenderTarget(null);
	}

	public static void Update()
	{
		List<int>[] array = pocketed;
		foreach (List<int> list in array)
		{
			foreach (int item in list)
			{
				Ball ball = Statics.balls[item];
				if (ball.Pos.Value.X > FinalPos[item].X)
				{
					float x = Math.Max(FinalPos[item].X, ball.Pos.Value.X - 0.1f);
					ball.Pos.Set(new Vector3(x, ball.Pos.Value.Y, ball.Pos.Value.Z));
					ball.Velo.Set(Vector3.UnitX * -0.1f);
					ball.UpdateDisplayMatrix();
				}
			}
		}
	}

	public static void Pocketed(int ballNum, int teamNum)
	{
		pocketed[teamNum].Add(ballNum);
		List<int> list = pocketed[teamNum];
		Vector3 vector = posZero;
		foreach (int item in list)
		{
			if (item == ballNum)
			{
				ComputedPos[item] = true;
				if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool && list.Count > 7)
				{
					vector.X -= finalPosOffset.X * 13f / 2f;
				}
				FinalPos[item] = vector;
				Statics.balls[item].Pos.Set(posStart + finalPosOffset * 1f * (list.Count - 1) + ((MaximinusGame.Id != MaximinusGame.ID.FunkyPool) ? Vector3.Zero : ((list.Count <= 7) ? (Vector3.UnitY * 0.833333f * -1f) : (Vector3.UnitY * 0.833333f))));
				Statics.balls[item].ResetMatrixRotation();
			}
			vector += finalPosOffset;
		}
	}

	public static bool NewPosComputedFor(int ballNum)
	{
		return ComputedPos[ballNum];
	}
}
