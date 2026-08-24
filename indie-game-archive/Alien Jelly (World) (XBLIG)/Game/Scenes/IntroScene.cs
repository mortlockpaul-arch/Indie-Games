using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Environment;
using Game.Post;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes;

public class IntroScene : Scene
{
	private const float SKY_RADIUS = 3000f;

	private const float SKY_SPEED = 0.5f;

	public static IntroScene instance;

	public DialogManager dialogs;

	public PostProcess_Dialog postDialog;

	public PostProcess_Dialog_Title postDialogTitle;

	public PostProcess_Gamma postGamma;

	private SpriteManager spriteManager;

	private Sprite spriteOverlay;

	private Sky sky;

	private bool skyActive;

	private Vector3 skyFocus;

	private Vector3 skyPositionFrom;

	private Vector3 skyPositionTo;

	private Quaternion skyRotation;

	private float skyTime;

	private float skyTimeTotal;

	private Vector3 skyAxis = new Vector3(0f, 1f, 0f);

	public GameAudio audio;

	public Base3D audioPoint;

	public bool errors;

	public bool ready;

	public bool showTitle = true;

	public IntroScene()
		: base("Intro")
	{
		instance = this;
		renderStacks.Add(new EntityStack(this, Material.State.Solid, GameMain.RENDERSTACK_SOLID, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Alpha, GameMain.RENDERSTACK_ALPHA_HARD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.Add, GameMain.RENDERSTACK_ADD, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_DIALOGS, xSort: false));
	}

	public override void Load()
	{
		library.FileLoad("Content/Data/Library_Intro.xml");
		base.Load();
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_DISTORT] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_DISTORT] = library.texture2Ds["TextureDistort"];
		Init();
		ready = true;
	}

	public override void Init()
	{
		base.Init();
		audioPoint = new Base3D();
		audio = new GameAudio(this, audioPoint);
		Sprites_Init();
		Post_Init();
		Dialogs_Init();
		dialogs.Open("Loading");
		audio.music.Set(0);
		if (!errors)
		{
			Init_Sky();
			dialogs.Close(delegate
			{
				if (showTitle)
				{
					showTitle = false;
					dialogs.Show("CM");
					(dialogs.dialog as DialogCM).__opened = delegate
					{
						dialogs.Close(delegate
						{
							dialogs.Show("Start");
							(dialogs.dialog as DialogStart).__completed = delegate
							{
								dialogs.Close(delegate
								{
									DataManager.PlayerData_Load(delegate
									{
										dialogs.Show("Loading", 1000f);
										(dialogs.dialog as DialogLoading).__opened = delegate
										{
											GameMain.instance.Scene_Swap(GameMain.instance.sceneMenu);
										};
									}, delegate
									{
										dialogs.Show("Error_LoadPlayerData");
									});
								});
							};
						});
					};
				}
				else
				{
					dialogs.Show("MainMenu");
				}
			});
		}
		else
		{
			dialogs.Close(delegate
			{
				dialogs.Show("Error_Load");
			});
		}
	}

	private void Init_Sky()
	{
		sky = new Sky(this);
		sky.FromName("Alpha Prime", 1);
		Sky_Start();
	}

	public override void Update(GameTime pGameTime)
	{
		base.Update(pGameTime);
		if (ready)
		{
			dialogs.Update(pGameTime);
			audio.Update(pGameTime);
			if (skyActive)
			{
				Sky_Update(pGameTime);
			}
		}
	}

	public override void Unload()
	{
		dialogs.Dispose();
		Sprites_Dispose();
		if (sky != null)
		{
			sky.Dispose();
		}
		audio.Dispose();
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		base.Unload();
	}

	private void Post_Init()
	{
		postDialog = new PostProcess_Dialog(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialog.Load();
		postDialog.amount = 0f;
		postDialog.active = false;
		postDialogTitle = new PostProcess_Dialog_Title(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postDialogTitle.Load();
		postDialogTitle.amount = 0f;
		postDialogTitle.active = false;
	}

	private void Post_Dispose()
	{
		postDialog.Unload();
		postDialogTitle.Unload();
		postDialog = null;
		postDialogTitle = null;
	}

	private void Dialogs_Init()
	{
		dialogs = new DialogManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS), delegate
		{
		}, new PostProcess[2] { postDialog, postDialogTitle }, audio);
		DialogCatalog.Make_Loading(dialogs);
		DialogCatalog.Make_CM(dialogs);
		DialogCatalog.Make_Start(dialogs);
		DialogCatalog.Make_Error_Loading(dialogs);
		DialogCatalog.Make_Error_LoadingPlayerData(dialogs);
	}

	private void Sprites_Init()
	{
		spriteManager = new SpriteManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		spriteOverlay = new Sprite(spriteManager);
		spriteOverlay.texture = GameMain.instance.GetSolidColorTexture(Color.Black);
		spriteOverlay.size = new Vector2(GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
		spriteOverlay.visible = true;
	}

	private void Sprites_Dispose()
	{
		spriteOverlay.Dispose();
		spriteManager.Dispose();
		spriteOverlay = null;
		spriteManager = null;
	}

	public void Sprites_HideOverlay()
	{
		spriteOverlay.visible = false;
	}

	private void Sky_Start()
	{
		skyFocus = cameras.camera._position;
		Sky_Set();
	}

	private void Sky_Set()
	{
		skyPositionFrom = skyFocus;
		skyPositionTo = GameMain.instance.GetRandUnitVecor() * 3000f;
		skyTime = 0f;
		skyTimeTotal = Vector3.Distance(skyPositionFrom, skyPositionTo) / 0.5f;
		skyActive = true;
	}

	private void Sky_Update(GameTime oGameTime)
	{
		if (!errors)
		{
			skyTime += oGameTime.ElapsedGameTime.Milliseconds;
			sky.Update(oGameTime);
			if (skyTime >= skyTimeTotal)
			{
				Sky_Set();
			}
			else
			{
				Sky_Lerp(skyTime / skyTimeTotal, oGameTime.ElapsedGameTime.Milliseconds);
			}
		}
	}

	private void Sky_Lerp(float xRatio, float oElapsed)
	{
		skyFocus = Vector3.Lerp(skyPositionFrom, skyPositionTo, xRatio);
		float amount = Math.Min(oElapsed / 10000f, 1f);
		cameras.camera.position = Vector3.Lerp(cameras.camera.position, skyFocus, amount);
		skyRotation = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(cameras.camera.position, skyFocus, Vector3.Up, Vector3.Forward)));
		cameras.camera.rotation = Quaternion.Lerp(cameras.camera.rotation, skyRotation, amount);
	}

	public override void Input_Update(GameTime oGameTime)
	{
		if (dialogs != null)
		{
			dialogs.Input_Update(oGameTime);
		}
		base.Input_Update(oGameTime);
	}
}
