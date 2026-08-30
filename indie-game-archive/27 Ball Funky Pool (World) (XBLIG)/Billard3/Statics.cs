using System;
using System.Collections.Generic;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class Statics
{
	public class Constants
	{
		public const float CoefficientFrictionHigh = 1f / 53f;

		public const float CoefficientFrictionLow = 1f / 90f;

		public const float VitesseChangementFriction = 0.8f;

		public const float veloRatio_GUF_MPH = 9.204545f;
	}

	public const string GameName = "8 Ball Champion";

	public const string ContentDirModels = "Models/";

	public const string ContentDirTex = "tex/";

	public const int NbBalls = 28;

	public static Drawing2D draw2D;

	public static List<Obj> objects;

	public static CameraBillard cam;

	public static Input input;

	public static Callbacks callbacks;

	public static List<Ball> balls;

	public static GameMenus menus;

	public static Lobby lobby;

	public static CheatPrompt cheatPrompt;

	public static Table table;

	public static Diamonds diamonds;

	public static Obj floor;

	public static Texture2D clothAlternate = null;

	public static Texture2D trouCentralAlternate = null;

	public static double ContentLoadedTime = -2.0;

	public static void LoadContent(MaximinusGame game, ContentManager Content)
	{
		Rack.InitializeRack();
		CollisionBande.Initialize();
		Ball.Textures.LoadContent(Content);
		Draws.Initialize(Content);
		cam = new CameraBillard();
		input = new Input();
		callbacks = new Callbacks(game);
		objects = new List<Obj>();
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball)
		{
			clothAlternate = Content.Load<Texture2D>("tex/plan-table-ligne-9ball");
			trouCentralAlternate = Content.Load<Texture2D>("tex/trou-central-9ball");
		}
		else if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			clothAlternate = Content.Load<Texture2D>("tex/plan-table-ligne-funky");
			trouCentralAlternate = Content.Load<Texture2D>("tex/trou-central-funky");
		}
		table = new Table();
		table.LoadContent(Content);
		foreach (Obj item in table.obj)
		{
			objects.Add(item);
		}
		Ball.LoadContent(Content);
		balls = new List<Ball>();
		for (int i = 0; i < 28; i++)
		{
			balls.Add(new Ball(i));
		}
		foreach (Ball ball in balls)
		{
			objects.Add(ball.obj);
		}
		RepositionWBall.LoadContent(Content);
		objects.Add(RepositionWBall.obj);
		Cue.LoadContent(Content);
		objects.Add(Cue.obj);
		LigneVisee.Initialise(Content);
		RoundedRectangle.Initialize(Content.Load<Texture2D>("tex/boxLines"));
		InfoDisplay.Initialize(draw2D.ScreenSizePoint);
		Menus.Initialize(draw2D);
		menus = new GameMenus(Content);
		ChoosePower.Initialize(draw2D.ScreenSizePoint, Content);
		lobby = new Lobby(game);
		cheatPrompt = new CheatPrompt(game);
		BoolColor.TrueColor = Color.Gold;
		Bot.Initialize();
		diamonds = new Diamonds(game);
		floor = new Obj(Obj.IDenum.Floor, Content.Load<Model>("Models/floor"));
		Draws.FloorEffect = Content.Load<Effect>("effects/spotlight");
		Audio.LoadContent(Content);
		foreach (Ball ball2 in balls)
		{
			ball2.Reset(isAlive: true);
		}
		ContentLoadedTime = -1.0;
	}

	public static Ball GetBall(Obj.IDenum i)
	{
		if (!Obj.IsBall(i))
		{
			throw new Exception("bug");
		}
		int index = (int)(i - 4);
		return balls[index];
	}
}
