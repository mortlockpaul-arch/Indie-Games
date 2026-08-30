using System;
using System.Collections.Generic;
using System.Linq;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class CheatPrompt : GameComponent
{
	private class Value
	{
		public int level;

		public int levelPrev;

		public float TransitionRatio;

		public bool ready;

		public bool updateReadyNeeded;

		private Point[] positions = new Point[3];

		public Point Position
		{
			get
			{
				if (TransitionRatio == 1f)
				{
					return positions[level];
				}
				return Utils.LerpPoint(positions[levelPrev], positions[level], TransitionRatio);
			}
		}

		public Value(Point ScreenSize, int number)
		{
			TransitionRatio = 1f;
			level = 1;
			ready = false;
			updateReadyNeeded = false;
			if (number == -1)
			{
				for (int i = 0; i < 3; i++)
				{
					ref Point reference = ref positions[i];
					reference = new Point((int)(posX_Ratio[i] * (float)ScreenSize.X), (int)(0.84999996f * (float)ScreenSize.Y));
				}
			}
			else
			{
				for (int j = 0; j < 3; j++)
				{
					ref Point reference2 = ref positions[j];
					reference2 = new Point((int)(posX_Ratio[j] * (float)ScreenSize.X + (float)(ScreenSize.X / -10) + (float)(ScreenSize.X * number / 15)), (int)(0.84999996f * (float)ScreenSize.Y));
				}
			}
		}

		public void SetLevel(int newLevel)
		{
			level = newLevel;
		}

		public void Ready(bool v)
		{
			ready = v;
			updateReadyNeeded = true;
		}

		public void ChangeLevel(int change)
		{
			levelPrev = level;
			level += change;
			level = Math.Max(0, level);
			level = Math.Min(2, level);
			if (level != levelPrev)
			{
				Audio.PlaySFX(Audio.SFXID.Menu);
				TransitionRatio = 0f;
			}
		}
	}

	public class AimInfo
	{
		public List<VertexPositionColor> PointList;

		public List<int> IndicesList;

		public List<Vector3> wballPositions;

		public int aimingColorBallID;

		public List<Vector3> aimingColorBallPos;

		public AimInfo()
		{
			PointList = new List<VertexPositionColor>();
			IndicesList = new List<int>();
			wballPositions = new List<Vector3>();
			aimingColorBallPos = new List<Vector3>();
			Reset();
		}

		public void Reset()
		{
			PointList.Clear();
			IndicesList.Clear();
			wballPositions.Clear();
			aimingColorBallID = -1;
			aimingColorBallPos.Clear();
		}
	}

	private const float posY_SPRatio = 0.84999996f;

	private Menus.ScreenInfo[] mpScreens = new Menus.ScreenInfo[4];

	private Menus.ScreenInfo title;

	private Menus.ScreenInfo spScreen;

	private Menus.ScreenInfo[] main = new Menus.ScreenInfo[3];

	private Texture2D playerReadyBlankTex;

	private bool disable;

	private int texSeparator;

	private int texSize;

	private GameModeRules.Type gameType;

	private Menus.MenuEntryValue<Texture2D> entryNotReady;

	private Menus.MenuEntryValue<Texture2D>[] entryReady = new Menus.MenuEntryValue<Texture2D>[4];

	private static readonly float[] posX_Ratio = new float[3] { 0.2f, 0.5f, 0.8f };

	private RenderTarget2D[] renders = new RenderTarget2D[3];

	private bool firstTime = true;

	private Value[] valueMP = new Value[4];

	private Value valueSP;

	private AimInfo[] aimInfos = new AimInfo[3];

	private static readonly Vector3 camPos = new Vector3(30f, 0f, 30f) - (Vector3.UnitX + Vector3.UnitZ) * 0.833333f * 8f + Vector3.UnitY * 22f;

	private static readonly Matrix viewMat = Matrix.CreateLookAt(camPos, camPos + Vector3.UnitY * -1f, Vector3.UnitZ * 1f);

	private static readonly Matrix projMat = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), 1f, 0.5f, 65f);

	private Ball wball = new Ball(0);

	private Ball cball = new Ball(3);

	private List<Ball> colorBalls = new List<Ball>();

	private double angle;

	private RoundedRectangle[] outlines = new RoundedRectangle[3];

	private Vector2[] posAnims = new Vector2[3];

	public CheatPrompt(Game game)
		: base(game)
	{
		game.Components.Add(this);
		texSeparator = Statics.draw2D.ScreenSizePoint.X / 15;
		texSize = (int)((float)Statics.draw2D.ScreenSizePoint.X / 4.5f);
		int borderOverride = Menus.ManagerV2.DefaultBorder / 2;
		playerReadyBlankTex = new Texture2D(Statics.draw2D.Device, GameMenus.Textures.checkMark.Width, GameMenus.Textures.checkMark.Height);
		entryNotReady = new Menus.MenuEntryValue<Texture2D>(0, playerReadyBlankTex, Statics.draw2D.Font, 0);
		for (int i = 0; i < mpScreens.Count(); i++)
		{
			entryReady[i] = new Menus.MenuEntryValue<Texture2D>(0, GameMenus.Textures.checkMark, Statics.draw2D.Font, 0);
			mpScreens[i] = new Menus.ScreenInfo(9, "", Vector2.One, borderOverride, 0.9f);
			mpScreens[i].AddNonSelectableEntry(GameModeRules.Team.NameOf((PlayerIndex)i), overrideSelectionTransition: true);
			mpScreens[i].AddEntryValue(entryNotReady);
			Statics.menus.AddScreenInfo(mpScreens[i]);
			valueMP[i] = new Value(Statics.draw2D.ScreenSizePoint, i);
		}
		spScreen = new Menus.ScreenInfo(8, "", Vector2.One, borderOverride, 0.9f);
		spScreen.AddNonSelectableEntry("    Your Choice    ", overrideSelectionTransition: true);
		valueSP = new Value(Statics.draw2D.ScreenSizePoint, -1);
		Statics.menus.AddScreenInfo(spScreen);
		title = new Menus.ScreenInfo(7, "", new Vector2(0.5f, 0.125f), Menus.ManagerV2.DefaultBorder * 2 / 3, 1f);
		title.AddNonSelectableEntry("      CHEAT-O-LASER LEVEL      ", overrideSelectionTransition: true);
		Statics.menus.AddScreenInfo(title);
		Texture2D tex = new Texture2D(Statics.draw2D.Device, texSize, texSize * 4 / 3);
		for (int j = 0; j < 3; j++)
		{
			main[j] = new Menus.ScreenInfo(7, "", new Vector2(posX_Ratio[j], 0.6f), 1f);
			main[j].AddNonSelectableEntry(LigneVisee.LevelName(j), overrideSelectionTransition: true);
			main[j].AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(tex, ""), Statics.draw2D.Font, 0));
			Statics.menus.AddScreenInfo(main[j]);
		}
		for (int k = 0; k < renders.Count(); k++)
		{
			renders[k] = new RenderTarget2D(Statics.draw2D.Device, texSize, texSize, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth16, 4, RenderTargetUsage.DiscardContents);
			aimInfos[k] = new AimInfo();
			ref Vector2 reference = ref posAnims[k];
			reference = new Vector2(main[k].Overlay.Center.X, main[k].Overlay.Center.Y - main[k].Overlay.Height / 20);
			outlines[k] = new RoundedRectangle(new Rectangle((int)(posAnims[k].X - (float)(texSize / 2)), (int)(posAnims[k].Y - (float)(texSize / 2)), texSize, texSize));
			outlines[k].TexWidth = Statics.menus.DecoWidth;
		}
		cball.Reset(isAlive: true);
		wball.Reset(isAlive: true);
		wball.Pos.Set(camPos + Vector3.UnitY * (0.833333f + camPos.Y * -1f) + (Vector3.UnitX + Vector3.UnitZ) * 0.833333f * -5f);
		wball.UpdateDisplayMatrix(1f);
		cball.Pos.Set(wball.Pos.Value + (Vector3.UnitX + Vector3.UnitZ) * 0.833333f * 7f);
		cball.UpdateDisplayMatrix(1f);
		colorBalls.Add(cball);
		disable = false;
	}

	public void Enable(GameModeRules.Type gameType, GameTime gameTime)
	{
		if (firstTime)
		{
			firstTime = false;
			if (Trial.IsTrial)
			{
				valueSP.SetLevel(0);
			}
		}
		disable = false;
		this.gameType = gameType;
		if (gameType == GameModeRules.Type.SinglePlayer)
		{
			valueSP.Ready(v: false);
		}
		else
		{
			Value[] array = valueMP;
			foreach (Value value in array)
			{
				value.Ready(v: false);
			}
		}
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, 7, value: true);
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, (gameType == GameModeRules.Type.SinglePlayer) ? 8 : 9, value: true);
	}

	private void Disable(GameTime gameTime)
	{
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, 7, value: false);
		Statics.menus.SwitchAllScreenInfoWithID(gameTime, (gameType == GameModeRules.Type.SinglePlayer) ? 8 : 9, value: false);
		disable = true;
	}

	public override void Update(GameTime gameTime)
	{
		if (GameState.Current != GameState.Type.CHEAT_PROMPT)
		{
			if (title.state == Menus.Screen.State.Active)
			{
				Disable(gameTime);
			}
			return;
		}
		if (disable && title.state == Menus.Screen.State.Hidden)
		{
			disable = false;
			GameState.Change(GameState.Type.MENUS, gameTime);
			Statics.menus.Enable();
		}
		if (title.state == Menus.Screen.State.Hidden)
		{
			return;
		}
		if (gameType == GameModeRules.Type.MultiPlayer)
		{
			for (int i = 0; i < 4; i++)
			{
				if (Statics.lobby.HasJoined(i))
				{
					UpdatePlayer(valueMP[i], mpScreens[i], entryReady[i]);
				}
				else
				{
					mpScreens[i].OverridePositions(new Point(-1000, -1000));
				}
			}
		}
		else
		{
			UpdatePlayer(valueSP, spScreen, entryReady[0]);
		}
		if (gameType == GameModeRules.Type.SinglePlayer)
		{
			if (valueSP.ready)
			{
				GameModeRules.InitializeSinglePlayer(gameTime, Statics.input.PlayerIndex);
			}
		}
		else
		{
			bool flag = true;
			for (int j = 0; j < 4; j++)
			{
				flag &= !Statics.lobby.HasJoined(j) || valueMP[j].ready;
			}
			if (flag)
			{
				GameModeRules.InitializeMultiPlayer(gameTime, Statics.lobby.teamA, Statics.lobby.teamB, promptForAddCpu: true);
			}
		}
		angle = Math.PI / 4.0 + Math.Cos(gameTime.TotalGameTime.TotalSeconds * 1.149999976158142) * Math.PI / 9.5;
		for (int k = 0; k < 3; k++)
		{
			LigneVisee.ComputeStatic((LigneVisee.Level)k, Aiming.AimVectorStatic(angle), wball, colorBalls, aimInfos[k].PointList, aimInfos[k].IndicesList, aimInfos[k].wballPositions, out aimInfos[k].aimingColorBallID, aimInfos[k].aimingColorBallPos);
		}
	}

	private void UpdatePlayer(Value value, Menus.ScreenInfo screen, Menus.MenuEntryValue<Texture2D> entryReadyValue)
	{
		if (gameType == GameModeRules.Type.MultiPlayer && value.updateReadyNeeded)
		{
			value.updateReadyNeeded = false;
			screen.entries.RemoveAt(screen.entries.Count - 1);
			screen.AddEntryValue(value.ready ? entryReadyValue : entryNotReady);
		}
		if (value.TransitionRatio < 1f)
		{
			value.TransitionRatio = Utils.incrementRatio(value.TransitionRatio, 15);
		}
		screen.OverridePositions(value.Position);
	}

	public void HandleInput(GameTime gameTime, PlayerIndex pInd, Utils.Input.ActionMenu action)
	{
		if (gameType == GameModeRules.Type.SinglePlayer && pInd != Statics.input.PlayerIndex)
		{
			return;
		}
		Value value;
		if (gameType == GameModeRules.Type.SinglePlayer)
		{
			value = valueSP;
		}
		else
		{
			value = valueMP[(int)pInd];
			_ = mpScreens[(int)pInd];
		}
		switch (action)
		{
		case Utils.Input.ActionMenu.MENU_ACTIVATE:
			if (!value.ready)
			{
				Audio.PlaySFX(Audio.SFXID.Menu);
				value.Ready(v: true);
				LigneVisee.Levels[(int)pInd] = (LigneVisee.Level)value.level;
			}
			break;
		case Utils.Input.ActionMenu.MENU_BACK:
			if (value.ready)
			{
				value.Ready(v: false);
			}
			else if (pInd == Statics.input.PlayerIndex)
			{
				Disable(gameTime);
			}
			break;
		}
		if (Utils.Input.IsActionLeftRight(action) && !value.ready && !(value.TransitionRatio < 1f))
		{
			value.ChangeLevel((action == Utils.Input.ActionMenu.MENU_RIGHT) ? 1 : (-1));
		}
	}

	public void DrawToTexture(GameTime gameTime)
	{
		if (GameState.Current == GameState.Type.CHEAT_PROMPT)
		{
			Statics.draw2D.PrepareFor3D();
			for (int i = 0; i < 3; i++)
			{
				RenderTarget2D r = renders[i];
				Drawing2D.SetAndClearRender(Statics.draw2D.Device, r, Color.Black);
				Drawing3D.DrawParams drawParams = new Drawing3D.DrawParams();
				drawParams.Reset(Draws.defaultMat);
				Draws.SetDrawParamsTable(drawParams);
				Drawing3D.DrawModel(Statics.table.obj[0], drawParams.transforms, drawParams.hasLighting, drawParams.isCustomColor, drawParams.customColor, viewMat, projMat, drawParams.lightingDir, drawParams.hasCustomAmbientColor, drawParams.customAmbientColor);
				wball.Draw(1f, 0, viewMat, projMat, deadMode: false);
				cball.Draw(1f, 3 - i, viewMat, projMat, deadMode: false);
				wball.updateChangementDeDirection(wball.Pos.Value2D, new VectorBillard(Aiming.AimVectorStatic(angle)).Value2D);
				drawParams.Reset(Draws.defaultMat);
				drawParams.transforms *= Cue.ComputeTransform(wball.Pos.Value, (float)angle, Vector3.Zero);
				Drawing3D.DrawModel(Cue.objNoAlpha, drawParams.transforms, drawParams.hasLighting, drawParams.isCustomColor, drawParams.customColor, viewMat, projMat, drawParams.lightingDir, drawParams.hasCustomAmbientColor, drawParams.customAmbientColor);
				LigneVisee.Draw(normalMode: false, viewMat, projMat, aimInfos[i].PointList.ToArray(), aimInfos[i].IndicesList.ToArray(), aimInfos[i].wballPositions, aimInfos[i].aimingColorBallID, aimInfos[i].aimingColorBallPos);
			}
			Statics.draw2D.Device.SetRenderTarget(null);
		}
	}

	public void Draw2D()
	{
		if (GameState.Current == GameState.Type.CHEAT_PROMPT)
		{
			float a = MathHelper.Lerp(0f, 1f, title.TransitionPosition);
			Color color = Utils.ColorWithAlpha(Color.White, a);
			for (int i = 0; i < 3; i++)
			{
				Statics.draw2D.SpriteBatch.Draw(renders[i], posAnims[i], null, color, 0f, new Vector2(texSize, texSize) * 0.5f, 1f, SpriteEffects.None, 0.5f);
				outlines[i].Color = Utils.ColorWithAlpha(GameMenus.ColorOutline, title.TransitionPosition * (float)(int)GameMenus.ColorOutline.A / 255f);
				outlines[i].Draw(Statics.draw2D.SpriteBatch);
			}
		}
	}
}
