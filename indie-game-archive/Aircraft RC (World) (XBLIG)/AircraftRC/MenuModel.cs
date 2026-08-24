using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace AircraftRC;

public class MenuModel
{
	private enum MenuState
	{
		transD,
		Cavion,
		Cvolume,
		Cmanette,
		transF
	}

	public enum AvionChoix
	{
		A1,
		A2,
		A3,
		A4,
		A5,
		A6
	}

	private Model ModelMenu;

	private Model ModelMenu1;

	private Model ModelMenu2;

	private Model ModelMenuCompo;

	private Model ModelMenuCompo1;

	private Model ModelMenuCompo2;

	private SceneObject Menu;

	private SceneObject Menu1;

	private SceneObject Menu2;

	private SceneObject Rouleau1;

	private SceneObject Rouleau2;

	private SceneObject Rouleau21;

	private SceneObject Rouleau22;

	private SceneObject Volume;

	private SceneObject Inter1;

	private SceneObject Inter2;

	private SceneObject Inter3;

	private SceneObject Diode1;

	private SceneObject Diode2;

	private SceneObject Diode3;

	private float InterRotation1;

	private float InterRotation2;

	private float InterRotation3;

	public float RouleauRotation1;

	public float rot;

	public float VolumePosition2;

	public float RouleauRotation3;

	private float DiodeRotation1;

	private float DiodeRotation2;

	private float DiodeRotation3;

	public float volume;

	private float x = -20f;

	private float y = 15f;

	private float z = 100f;

	public bool Sortir;

	private float temploading;

	private MenuState menuState = MenuState.Cavion;

	public AvionChoix avionChoix;

	private Song Musique;

	private InputAction Starta;

	private InputAction sortir;

	private InputAction menuavant;

	private InputAction menuarriere;

	private InputAction menuUp;

	private InputAction menuDown;

	private InputAction menuCARight;

	private InputAction menuCALeft;

	private InputAction menuCARightI;

	private InputAction menuCALeftI;

	private InputState input;

	public SceneState sceneStateMenu;

	private SunBurnCoreSystem sunBurnCoreSystemMenu;

	private FrameBuffers frameBuffersMenu;

	public SceneInterface sceneInterfaceMenu;

	private SceneEnvironment environmentMenu;

	private Scene scenemenu;

	private Cue inter;

	private Cue interh;

	private AudioEmitter emittermenu = new AudioEmitter();

	private AudioListener listenermenu = new AudioListener();

	public PlayerIndex player;

	private PlayerIndex? controllingPlayer;

	public PlayerIndex? ControllingPlayer
	{
		get
		{
			return controllingPlayer;
		}
		internal set
		{
			controllingPlayer = value;
		}
	}

	public MenuModel(CustomPhysicsGame game)
	{
		sunBurnCoreSystemMenu = new SunBurnCoreSystem(game.Services, game.Content);
		sceneStateMenu = new SceneState();
		sceneInterfaceMenu = new SceneInterface();
		sceneInterfaceMenu.CreateDefaultManagers(RenderingSystemType.Forward, includeautoloadedplugins: true);
		frameBuffersMenu = new FrameBuffers(DetailPreference.High, DetailPreference.High);
		sceneInterfaceMenu.ResourceManager.AssignOwnership(frameBuffersMenu);
		sceneInterfaceMenu.ShowConsole = false;
		MediaPlayer.Volume = 0.5f;
		volume = 0.5f;
		VolumePosition2 = -0.06f;
		RouleauRotation1 = 0f;
		RouleauRotation3 = 0f;
		DiodeRotation1 = 0f;
		DiodeRotation2 = 180f;
		DiodeRotation3 = 180f;
		Starta = new InputAction(new Buttons[1] { Buttons.Start }, newPressOnly: true);
		sortir = new InputAction(new Buttons[1] { Buttons.Back }, newPressOnly: true);
		menuavant = new InputAction(new Buttons[1] { Buttons.A }, newPressOnly: true);
		menuarriere = new InputAction(new Buttons[1] { Buttons.B }, newPressOnly: true);
		menuUp = new InputAction(new Buttons[2]
		{
			Buttons.DPadUp,
			Buttons.LeftThumbstickUp
		}, newPressOnly: true);
		menuDown = new InputAction(new Buttons[2]
		{
			Buttons.DPadDown,
			Buttons.LeftThumbstickDown
		}, newPressOnly: true);
		menuCARight = new InputAction(new Buttons[2]
		{
			Buttons.DPadRight,
			Buttons.RightThumbstickRight
		}, newPressOnly: true);
		menuCALeft = new InputAction(new Buttons[2]
		{
			Buttons.DPadLeft,
			Buttons.RightThumbstickLeft
		}, newPressOnly: true);
		menuCARightI = new InputAction(new Buttons[2]
		{
			Buttons.DPadRight,
			Buttons.RightThumbstickRight
		}, newPressOnly: false);
		menuCALeftI = new InputAction(new Buttons[2]
		{
			Buttons.DPadLeft,
			Buttons.RightThumbstickLeft
		}, newPressOnly: false);
		MediaPlayer.IsRepeating = true;
		input = new InputState();
	}

	public void Load(CustomPhysicsGame game)
	{
		Musique = game.Content.Load<Song>("Audio/intro");
		MediaPlayer.Play(Musique);
		scenemenu = game.Content.Load<Scene>("Scenes/SceneMenu");
		environmentMenu = game.Content.Load<SceneEnvironment>("Environment/Environmentmenu");
		sceneInterfaceMenu.Submit(scenemenu);
		sceneInterfaceMenu.ApplyPreferences(game.preferences);
	}

	public void MMenu(CustomPhysicsGame game)
	{
		ModelMenuCompo = game.Content.Load<Model>("Models/menuCom");
		ModelMenu = game.Content.Load<Model>("Models/menu");
		ModelMesh mesh = ModelMenuCompo.Meshes["Rouleau2"];
		Menu = new SceneObject(ModelMenu);
		Rouleau2 = new SceneObject(mesh);
		sceneInterfaceMenu.ObjectManager.Submit(Menu);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau2);
		ModelMesh mesh2 = ModelMenuCompo.Meshes["Rouleau1"];
		ModelMesh mesh3 = ModelMenuCompo.Meshes["volume"];
		ModelMesh mesh4 = ModelMenuCompo.Meshes["inter1"];
		ModelMesh mesh5 = ModelMenuCompo.Meshes["diode1"];
		Rouleau1 = new SceneObject(mesh2);
		Volume = new SceneObject(mesh3);
		Inter1 = new SceneObject(mesh4);
		Inter2 = new SceneObject(mesh4);
		Inter3 = new SceneObject(mesh4);
		Diode1 = new SceneObject(mesh5);
		Diode2 = new SceneObject(mesh5);
		Diode3 = new SceneObject(mesh5);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau1);
		sceneInterfaceMenu.ObjectManager.Submit(Volume);
		sceneInterfaceMenu.ObjectManager.Submit(Inter1);
		sceneInterfaceMenu.ObjectManager.Submit(Inter2);
		sceneInterfaceMenu.ObjectManager.Submit(Inter3);
		sceneInterfaceMenu.ObjectManager.Submit(Diode1);
		sceneInterfaceMenu.ObjectManager.Submit(Diode2);
		sceneInterfaceMenu.ObjectManager.Submit(Diode3);
	}

	public void MMenuF(CustomPhysicsGame game)
	{
		ModelMenuCompo1 = game.Content.Load<Model>("Models/menuCom1");
		ModelMenu1 = game.Content.Load<Model>("Models/menu1");
		ModelMesh mesh = ModelMenuCompo1.Meshes["Rouleau2"];
		Menu1 = new SceneObject(ModelMenu1);
		Rouleau21 = new SceneObject(mesh);
		sceneInterfaceMenu.ObjectManager.Submit(Menu1);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau21);
		ModelMesh mesh2 = ModelMenuCompo1.Meshes["Rouleau1"];
		ModelMesh mesh3 = ModelMenuCompo1.Meshes["volume"];
		ModelMesh mesh4 = ModelMenuCompo1.Meshes["inter1"];
		ModelMesh mesh5 = ModelMenuCompo1.Meshes["diode1"];
		Rouleau1 = new SceneObject(mesh2);
		Volume = new SceneObject(mesh3);
		Inter1 = new SceneObject(mesh4);
		Inter2 = new SceneObject(mesh4);
		Inter3 = new SceneObject(mesh4);
		Diode1 = new SceneObject(mesh5);
		Diode2 = new SceneObject(mesh5);
		Diode3 = new SceneObject(mesh5);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau1);
		sceneInterfaceMenu.ObjectManager.Submit(Volume);
		sceneInterfaceMenu.ObjectManager.Submit(Inter1);
		sceneInterfaceMenu.ObjectManager.Submit(Inter2);
		sceneInterfaceMenu.ObjectManager.Submit(Inter3);
		sceneInterfaceMenu.ObjectManager.Submit(Diode1);
		sceneInterfaceMenu.ObjectManager.Submit(Diode2);
		sceneInterfaceMenu.ObjectManager.Submit(Diode3);
	}

	public void MMenuE(CustomPhysicsGame game)
	{
		ModelMenuCompo2 = game.Content.Load<Model>("Models/menuCom2");
		ModelMenu2 = game.Content.Load<Model>("Models/menu2");
		ModelMesh mesh = ModelMenuCompo2.Meshes["Rouleau2"];
		Menu2 = new SceneObject(ModelMenu2);
		Rouleau22 = new SceneObject(mesh);
		sceneInterfaceMenu.ObjectManager.Submit(Menu2);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau22);
		ModelMesh mesh2 = ModelMenuCompo2.Meshes["Rouleau1"];
		ModelMesh mesh3 = ModelMenuCompo2.Meshes["volume"];
		ModelMesh mesh4 = ModelMenuCompo2.Meshes["inter1"];
		ModelMesh mesh5 = ModelMenuCompo2.Meshes["diode1"];
		Rouleau1 = new SceneObject(mesh2);
		Volume = new SceneObject(mesh3);
		Inter1 = new SceneObject(mesh4);
		Inter2 = new SceneObject(mesh4);
		Inter3 = new SceneObject(mesh4);
		Diode1 = new SceneObject(mesh5);
		Diode2 = new SceneObject(mesh5);
		Diode3 = new SceneObject(mesh5);
		sceneInterfaceMenu.ObjectManager.Submit(Rouleau1);
		sceneInterfaceMenu.ObjectManager.Submit(Volume);
		sceneInterfaceMenu.ObjectManager.Submit(Inter1);
		sceneInterfaceMenu.ObjectManager.Submit(Inter2);
		sceneInterfaceMenu.ObjectManager.Submit(Inter3);
		sceneInterfaceMenu.ObjectManager.Submit(Diode1);
		sceneInterfaceMenu.ObjectManager.Submit(Diode2);
		sceneInterfaceMenu.ObjectManager.Submit(Diode3);
	}

	public void Inter(CustomPhysicsGame game)
	{
		inter = game.soundBank.GetCue("Interrupteur12");
		inter.Apply3D(listenermenu, emittermenu);
		inter.Play();
	}

	public void InterH(CustomPhysicsGame game)
	{
		interh = game.soundBank.GetCue("beep");
		interh.Apply3D(listenermenu, emittermenu);
		interh.Play();
	}

	public void Update(GameTime gameTime, CustomPhysicsGame game)
	{
		Matrix matrix = Matrix.CreateTranslation(0f, -1.843f, -1.98f);
		Matrix matrix2 = Matrix.CreateTranslation(0f, -1.848f, 2.185f);
		Matrix matrix3 = Matrix.CreateTranslation(0f, 0.155f, 0.152f);
		Matrix matrix4 = Matrix.CreateTranslation(0f, 0.07f, -2.056f);
		Matrix matrix5 = Matrix.CreateTranslation(0f, 0.07f, 0.153f);
		Matrix matrix6 = Matrix.CreateTranslation(0f, 0.07f, 2.215f);
		Matrix matrix7 = Matrix.CreateTranslation(0f, 0.159f, -2.056f);
		Matrix matrix8 = Matrix.CreateTranslation(0f, 0.159f, 0.153f);
		Matrix matrix9 = Matrix.CreateTranslation(0f, 0.159f, 2.215f);
		Rouleau1.World = Matrix.CreateRotationX(MathHelper.ToRadians(RouleauRotation1)) * matrix;
		if (game.string1 == "1")
		{
			Rouleau2.World = Matrix.CreateRotationX(MathHelper.ToRadians(RouleauRotation3)) * matrix2;
		}
		if (game.string1 == "0")
		{
			Rouleau21.World = Matrix.CreateRotationX(MathHelper.ToRadians(RouleauRotation3)) * matrix2;
		}
		if (game.string1 == "2")
		{
			Rouleau22.World = Matrix.CreateRotationX(MathHelper.ToRadians(RouleauRotation3)) * matrix2;
		}
		Volume.World = Matrix.CreateTranslation(0f, VolumePosition2, 0f) * matrix3;
		Inter1.World = Matrix.CreateRotationZ(MathHelper.ToRadians(InterRotation1)) * matrix4;
		Inter2.World = Matrix.CreateRotationZ(MathHelper.ToRadians(InterRotation2)) * matrix5;
		Inter3.World = Matrix.CreateRotationZ(MathHelper.ToRadians(InterRotation3)) * matrix6;
		Diode1.World = Matrix.CreateRotationX(MathHelper.ToRadians(DiodeRotation1)) * matrix7;
		Diode2.World = Matrix.CreateRotationX(MathHelper.ToRadians(DiodeRotation2)) * matrix8;
		Diode3.World = Matrix.CreateRotationX(MathHelper.ToRadians(DiodeRotation3)) * matrix9;
		InterRotation1 = 0f;
		InterRotation2 = 0f;
		InterRotation3 = 0f;
		sceneInterfaceMenu.Update(gameTime);
		game.camera.position = new Vector3(x, y, z);
		if (game.gameState == CustomPhysicsGame.GameState.Debut)
		{
			game.camera.target = new Vector3(0f, 2f, 0f);
			x -= 0.2f;
			y = 15f;
			z += 0.9f;
			if (x <= -20f)
			{
				x = -20f;
			}
			if (z >= 100f)
			{
				z = 100f;
			}
			game.camera.fov = 47f;
		}
		if (game.gameState == CustomPhysicsGame.GameState.Menu || game.gameState == CustomPhysicsGame.GameState.Loading)
		{
			game.camera.fov = 45f;
			game.camera.target = new Vector3(0f, 2f, 0f);
			x += 0.2f;
			y = 15f;
			z -= 0.9f;
			if (x >= 0f)
			{
				x = 0f;
			}
			if (z <= 10f)
			{
				z = 10f;
			}
		}
		if (RouleauRotation1 == 0f || RouleauRotation1 == 360f)
		{
			avionChoix = AvionChoix.A1;
		}
		if (RouleauRotation1 == 60f || RouleauRotation1 == -300f)
		{
			avionChoix = AvionChoix.A2;
		}
		if (RouleauRotation1 == 120f || RouleauRotation1 == -240f)
		{
			avionChoix = AvionChoix.A3;
		}
		if (RouleauRotation1 == 180f || RouleauRotation1 == -180f)
		{
			avionChoix = AvionChoix.A4;
		}
		if (RouleauRotation1 == 240f || RouleauRotation1 == -120f)
		{
			avionChoix = AvionChoix.A5;
		}
		if (RouleauRotation1 == 300f || RouleauRotation1 == -60f)
		{
			avionChoix = AvionChoix.A6;
		}
		PlayerIndex playerIndex;
		if (game.gameState == CustomPhysicsGame.GameState.pressA)
		{
			if (sortir.Evaluate(input, ControllingPlayer, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
			{
				Sortir = true;
			}
			if (menuarriere.Evaluate(input, ControllingPlayer, out playerIndex) && Sortir)
			{
				Sortir = false;
			}
			if (menuavant.Evaluate(input, ControllingPlayer, out playerIndex) && Sortir)
			{
				game.Exit();
			}
			if (Starta.Evaluate(input, ControllingPlayer, out playerIndex))
			{
				player = playerIndex;
				SignedInGamer signedInGamer = Gamer.SignedInGamers[player];
				if (signedInGamer == null && !Guide.IsVisible)
				{
					Guide.ShowSignIn(1, onlineOnly: false);
				}
				if (signedInGamer != null && !Guide.IsVisible)
				{
					game.gameState = CustomPhysicsGame.GameState.Debut;
					game.Lire();
				}
			}
		}
		checked
		{
			if (game.gameState == CustomPhysicsGame.GameState.Debut)
			{
				if (sortir.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Sortir = true;
				}
				if (menuarriere.Evaluate(input, ControllingPlayer, out playerIndex) && Sortir)
				{
					Sortir = false;
				}
				if (menuavant.Evaluate(input, ControllingPlayer, out playerIndex) && Sortir)
				{
					game.Exit();
				}
				if (menuavant.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu && !Sortir)
				{
					game.gameState++;
				}
			}
			if (Guide.IsTrialMode && avionChoix == AvionChoix.A1 && game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f) && menuavant.Evaluate(input, player, out playerIndex))
			{
				game.gameState = CustomPhysicsGame.GameState.Loading;
			}
			if (game.gameState == CustomPhysicsGame.GameState.Loading && Guide.IsTrialMode && avionChoix == AvionChoix.A1)
			{
				temploading++;
				if (temploading >= 3f)
				{
					game.gameState = CustomPhysicsGame.GameState.Partie;
				}
			}
			if (!Guide.IsTrialMode && game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f) && game.scoreP == CustomPhysicsGame.ScoreP.vu && menuavant.Evaluate(input, player, out playerIndex))
			{
				game.gameState = CustomPhysicsGame.GameState.Loading;
			}
			if (game.gameState == CustomPhysicsGame.GameState.Loading && !Guide.IsTrialMode)
			{
				temploading++;
				if (temploading >= 3f)
				{
					game.gameState = CustomPhysicsGame.GameState.Partie;
				}
			}
			if (game.gameState == CustomPhysicsGame.GameState.Menu && menuarriere.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
			{
				game.gameState = CustomPhysicsGame.GameState.Debut;
			}
			if (game.gameState == CustomPhysicsGame.GameState.Partie)
			{
				temploading = 0f;
				if (avionChoix == AvionChoix.A1)
				{
					game.LoadAvion1();
					game.pause = false;
				}
				if (avionChoix == AvionChoix.A3)
				{
					game.LoadAvion2();
					game.pause = false;
				}
				if (avionChoix == AvionChoix.A5)
				{
					game.LoadAvion3();
					game.pause = false;
				}
				if (avionChoix == AvionChoix.A2)
				{
					game.LoadAvion4();
					game.pause = false;
				}
				if (avionChoix == AvionChoix.A4)
				{
					game.LoadAvion5();
					game.pause = false;
				}
				if (avionChoix == AvionChoix.A6)
				{
					game.LoadAvion6();
					game.pause = false;
				}
			}
			if (game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f))
			{
				if (menuUp.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterH(game);
					menuState--;
					if (menuState == MenuState.transD)
					{
						menuState = MenuState.Cmanette;
					}
				}
				if (menuDown.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterH(game);
					menuState++;
					if (menuState == MenuState.transF)
					{
						menuState = MenuState.Cavion;
					}
				}
			}
			if (game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f) && menuState == MenuState.Cavion)
			{
				DiodeRotation1 = 0f;
				DiodeRotation2 = 180f;
				DiodeRotation3 = 180f;
				if (menuCARightI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation1 = -30f;
				}
				if (menuCARight.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
					RouleauRotation1 += 60f;
					if (RouleauRotation1 == 360f)
					{
						RouleauRotation1 = 0f;
					}
				}
				if (menuCALeftI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation1 = 30f;
				}
				if (menuCALeft.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
					RouleauRotation1 -= 60f;
					if (RouleauRotation1 == -360f)
					{
						RouleauRotation1 = 0f;
					}
				}
			}
			MediaPlayer.Volume = volume;
			if (volume >= 1f)
			{
				volume = 1f;
			}
			if (volume <= 0f)
			{
				volume = 0f;
			}
			if (game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f) && menuState == MenuState.Cvolume)
			{
				DiodeRotation1 = 180f;
				DiodeRotation2 = 0f;
				DiodeRotation3 = 180f;
				if (menuCARight.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
				}
				if (menuCARightI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation2 = -30f;
					VolumePosition2 += 0.0006f;
					volume += 0.009f;
					if (VolumePosition2 >= -0.028f)
					{
						VolumePosition2 = -0.028f;
					}
				}
				if (menuCALeft.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
				}
				if (menuCALeftI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation2 = 30f;
					VolumePosition2 -= 0.0006f;
					volume -= 0.009f;
					if (VolumePosition2 <= -0.092f)
					{
						VolumePosition2 = -0.092f;
					}
				}
			}
			if (game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f) && menuState == MenuState.Cmanette)
			{
				DiodeRotation1 = 180f;
				DiodeRotation2 = 180f;
				DiodeRotation3 = 0f;
				if (menuCARightI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation3 = -30f;
				}
				if (menuCARight.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
					RouleauRotation3 += 60f;
					if (RouleauRotation3 == 360f)
					{
						RouleauRotation3 = 0f;
					}
				}
				if (menuCALeftI.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					InterRotation3 = 30f;
				}
				if (menuCALeft.Evaluate(input, player, out playerIndex) && game.scoreP == CustomPhysicsGame.ScoreP.vu)
				{
					Inter(game);
					RouleauRotation3 -= 60f;
					if (RouleauRotation3 == -360f)
					{
						RouleauRotation3 = 0f;
					}
				}
				if (RouleauRotation3 == 0f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M1;
				}
				if (RouleauRotation3 == 60f || RouleauRotation3 == -300f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M2;
				}
				if (RouleauRotation3 == 120f || RouleauRotation3 == 240f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M3;
				}
				if (RouleauRotation3 == 180f || RouleauRotation3 == -180f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M4;
				}
				if (RouleauRotation3 == 240f || RouleauRotation3 == -120f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M5;
				}
				if (RouleauRotation3 == 300f || RouleauRotation3 == -60f)
				{
					game.manetteChoix = CustomPhysicsGame.ManetteChoix.M6;
				}
			}
			input.Update();
		}
	}

	public void Draw(CustomPhysicsGame game, GameTime gameTime)
	{
		sceneStateMenu.BeginFrameRendering(game.camera.View, game.camera.Projection, gameTime, environmentMenu, frameBuffersMenu, renderingtoscreen: true);
		sceneInterfaceMenu.BeginFrameRendering(sceneStateMenu);
		sceneInterfaceMenu.RenderManager.Render();
		sceneInterfaceMenu.EndFrameRendering();
		sceneStateMenu.EndFrameRendering();
	}
}
