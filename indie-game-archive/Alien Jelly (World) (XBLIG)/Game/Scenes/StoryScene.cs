using System;
using GKEngine;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Audio;
using Game.Data;
using Game.Dialogs;
using Game.Post;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes;

public class StoryScene : Scene
{
	public enum StoryMode
	{
		Intro,
		Ending
	}

	public delegate void StoryDelegate();

	public static StoryScene instance;

	private static Vector2[] PAN_DIR = new Vector2[5]
	{
		new Vector2(0f, 0f),
		new Vector2(0f, 0f),
		new Vector2(-1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(0f, 1f)
	};

	public DialogManager dialogs;

	public GameAudio audio;

	public SpriteManager spriteManager;

	public SpriteManager spriteManagerBG;

	private bool exiting;

	private bool active;

	public StoryMode storyMode;

	private PostProcess_BloomSimple postBloom;

	private PostProcess_RewindStory postRewind;

	private Sprite black;

	private Sprite background;

	private Sprite[] text;

	private Sprite[] pannels;

	private StoryDelegate __textCompleted;

	private bool blackActive;

	private float blackTime;

	private float blackFrom;

	private float blackTo;

	private float blackTimeTotal;

	private StoryDelegate __blackCompleted;

	private bool waitActive;

	private float waitTime;

	private float waitTimeTotal;

	private StoryDelegate __waitCompleted;

	private Sprite pan;

	private bool panActive;

	private float panTime;

	private Vector2 panFromScale = default(Vector2);

	private Vector2 panToScale = default(Vector2);

	private Vector2 panFromPos = default(Vector2);

	private Vector2 panToPos = default(Vector2);

	private float panTimeTotal;

	private StoryDelegate __panCompleted;

	public InputEntity inputExit;

	public StoryScene()
		: base("Story")
	{
		instance = this;
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_SOLID, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_UI, xSort: false));
		renderStacks.Add(new EntityStack(this, Material.State.None, GameMain.RENDERSTACK_DIALOGS, xSort: false));
	}

	public override void Load()
	{
		library.FileLoad("Content/Data/Library_Story.xml");
		base.Load();
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_DISTORT] = SamplerState.AnisotropicWrap;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_DISTORT] = library.texture2Ds["TextureDistort"];
		Init();
	}

	public override void Init()
	{
		base.Init();
		audio = new GameAudio(this, new Base3D());
		Init_Dialogs();
		if (storyMode == StoryMode.Intro)
		{
			Story_Init();
		}
		else
		{
			Ending_Init();
		}
		exiting = false;
		active = false;
		dialogs.Open("Loading");
		dialogs.Close(delegate
		{
			active = true;
			if (storyMode == StoryMode.Intro)
			{
				Story_Start();
			}
			else
			{
				Ending_Start();
			}
		});
	}

	private void Init_Dialogs()
	{
		postBloom = new PostProcess_BloomSimple(RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS));
		postBloom.Load();
		postBloom.amount = 0f;
		postBloom.active = false;
		postRewind = new PostProcess_RewindStory(RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		postRewind.Load();
		postRewind.amount = 0f;
		postRewind.active = false;
		dialogs = new DialogManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_DIALOGS), delegate
		{
		}, new PostProcess[1] { postBloom }, audio);
		DialogCatalog.Make_Loading(dialogs);
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		Black_Update(oGameTime);
		Wait_Update(oGameTime);
		Pan_Update(oGameTime);
		dialogs.Update(oGameTime);
		audio.Update(oGameTime);
	}

	public override void Render(GameTime oGameTime)
	{
		base.Render(oGameTime);
	}

	public override void Unload()
	{
		background.Dispose();
		black.Dispose();
		for (int i = 0; i < text.Length; i++)
		{
			text[i].Dispose();
			text[i] = null;
		}
		for (int i = 0; i < pannels.Length; i++)
		{
			pannels[i].Dispose();
			pannels[i] = null;
		}
		spriteManager.Dispose();
		spriteManagerBG.Dispose();
		spriteManager = null;
		spriteManagerBG = null;
		dialogs.Dispose();
		postBloom.Unload();
		postRewind.Unload();
		postBloom = null;
		postRewind = null;
		audio.Dispose();
		audio = null;
		UniversalInput.InputEntity_Flush(InputEntity.Scope.Scene);
		dialogs = null;
		audio = null;
		base.Unload();
	}

	private void SwitchToPlay()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			DataManager.Levels_Load(0u, 0u, 0, delegate
			{
				GameMain.instance.Scene_Swap(GameMain.instance.scenePlay);
			}, delegate
			{
				Console.WriteLine("Loading Play Level Failed From Story Scene");
			});
		};
	}

	private void SwitchToMenu()
	{
		dialogs.Show("Loading");
		(dialogs.dialog as DialogLoading).__opened = delegate
		{
			GameMain.instance.Scene_Swap(GameMain.instance.sceneMenu);
		};
	}

	private void HideAll()
	{
		for (int i = 0; i < text.Length; i++)
		{
			text[i].visible = false;
		}
		for (int i = 0; i < pannels.Length; i++)
		{
			pannels[i].visible = false;
		}
	}

	private void Halt()
	{
		blackActive = false;
		waitActive = false;
		panActive = false;
		exiting = true;
		if (storyMode == StoryMode.Intro)
		{
			Black(1000f, black.alpha, 1f, delegate
			{
				audio.music.Stop();
				SwitchToPlay();
			});
		}
		else
		{
			Black(1000f, black.alpha, 1f, delegate
			{
				audio.music.Stop();
				SwitchToMenu();
			});
		}
	}

	private void Story_Init()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		_ = DataManager.local.settings.screen.X;
		_ = DataManager.local.settings.screen.Y;
		_ = DataManager.local.settings.screen.Width;
		_ = DataManager.local.settings.screen.Height;
		spriteManagerBG = new SpriteManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_SOLID));
		spriteManager = new SpriteManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		spriteManager.effect = null;
		spriteManagerBG.effect = null;
		background = new Sprite(spriteManagerBG);
		background.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Background");
		background.scale.X = (float)width / (float)background.texture.Width;
		background.scale.Y = (float)height / (float)background.texture.Height;
		text = new Sprite[6];
		Sprite sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_0");
		text[0] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_1");
		text[1] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_2");
		text[2] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_3");
		text[3] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_4");
		text[4] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_5");
		text[5] = sprite;
		pannels = new Sprite[4];
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Scene_0");
		pannels[0] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Scene_1");
		pannels[1] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Scene_2");
		pannels[2] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Scene_3");
		pannels[3] = sprite;
		black = new Sprite(spriteManager);
		black.texture = GameEngine.instance.GetSolidColorTexture(new Color(0, 0, 0, 255));
		black.scale.X = width;
		black.scale.Y = height;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < pannels.Length; i++)
		{
			num = (float)width / pannels[i].size.X;
			num2 = (float)height / pannels[i].size.Y;
			num3 = ((num > num2) ? num : num2);
			pannels[i].scale.X = num3;
			pannels[i].scale.Y = num3;
			pannels[i].position.X = ((float)width - pannels[i].size.X * pannels[i].scale.X) * 0.5f;
			pannels[i].position.Y = ((float)height - pannels[i].size.Y * pannels[i].scale.Y) * 0.5f;
		}
		for (int i = 0; i < text.Length; i++)
		{
			text[i].scale.X = num3;
			text[i].scale.Y = num3;
			text[i].position.X = ((float)width - text[i].size.X * num3) * 0.5f;
			text[i].position.Y = ((float)height - text[i].size.Y * num3) * 0.5f;
		}
		HideAll();
	}

	private void Story_Start()
	{
		audio.music.Set(1);
		Story_Text(0, 3000f, delegate
		{
			Story_Pannel(0, 8000f, delegate
			{
				Story_Text(1, 3000f, delegate
				{
					Story_Text(2, 3000f, delegate
					{
						Story_Pannel(1, 8000f, delegate
						{
							Story_Text(3, 3000f, delegate
							{
								Story_Pannel(2, 8000f, delegate
								{
									Story_Text(4, 3000f, delegate
									{
										Story_Pannel(3, 8000f, delegate
										{
											Story_Text(5, 3000f, delegate
											{
												audio.music.Stop();
												SwitchToPlay();
											});
										});
									});
								});
							});
						});
					});
				});
			});
		});
	}

	private void Story_Text(int xIndex, float xTime, StoryDelegate oCompleted)
	{
		__textCompleted = oCompleted;
		HideAll();
		text[xIndex].visible = true;
		postRewind.active = true;
		postRewind.amount = 0.4f;
		Black(1000f, 1f, 0f, delegate
		{
			Wait(xTime, delegate
			{
				Black(1000f, 0f, 1f, delegate
				{
					postRewind.active = false;
					Wait(500f, __textCompleted);
				});
			});
		});
	}

	private void Story_Pannel(int xIndex, float xTime, StoryDelegate oCompleted)
	{
		__textCompleted = oCompleted;
		HideAll();
		pannels[xIndex].visible = true;
		Pan(xIndex, xTime, delegate
		{
			Black(1000f, 0f, 1f, delegate
			{
				Wait(500f, __textCompleted);
			});
		});
		Pan_Start();
		Black(1000f, 1f, 0f, delegate
		{
		});
	}

	private void Ending_Init()
	{
		int width = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		int height = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		_ = DataManager.local.settings.screen.X;
		_ = DataManager.local.settings.screen.Y;
		_ = DataManager.local.settings.screen.Width;
		_ = DataManager.local.settings.screen.Height;
		spriteManagerBG = new SpriteManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_SOLID));
		spriteManager = new SpriteManager(this, RenderStacks_FromName(GameMain.RENDERSTACK_UI));
		spriteManager.effect = null;
		spriteManagerBG.effect = null;
		background = new Sprite(spriteManagerBG);
		background.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Background");
		background.scale.X = (float)width / (float)background.texture.Width;
		background.scale.Y = (float)height / (float)background.texture.Height;
		text = new Sprite[2];
		Sprite sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_End_0");
		text[0] = sprite;
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Text_End_1");
		text[1] = sprite;
		pannels = new Sprite[1];
		sprite = new Sprite(spriteManager);
		sprite.texture = GameEngine.SceneContent.Load<Texture2D>("Content/UI/Story/Scene_End_0");
		pannels[0] = sprite;
		black = new Sprite(spriteManager);
		black.texture = GameEngine.instance.GetSolidColorTexture(new Color(0, 0, 0, 255));
		black.scale.X = width;
		black.scale.Y = height;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < pannels.Length; i++)
		{
			num = (float)width / pannels[i].size.X;
			num2 = (float)height / pannels[i].size.Y;
			num3 = ((num > num2) ? num : num2);
			pannels[i].scale.X = num3;
			pannels[i].scale.Y = num3;
			pannels[i].position.X = ((float)width - pannels[i].size.X * pannels[i].scale.X) * 0.5f;
			pannels[i].position.Y = ((float)height - pannels[i].size.Y * pannels[i].scale.Y) * 0.5f;
		}
		for (int i = 0; i < text.Length; i++)
		{
			text[i].scale.X = num3;
			text[i].scale.Y = num3;
			text[i].position.X = ((float)width - text[i].size.X * num3) * 0.5f;
			text[i].position.Y = ((float)height - text[i].size.Y * num3) * 0.5f;
		}
		HideAll();
	}

	private void Ending_Start()
	{
		audio.music.Set(0);
		Story_Text(0, 3000f, delegate
		{
			Story_Text(1, 3000f, delegate
			{
				Story_Pannel(0, 15000f, delegate
				{
					audio.music.Stop();
					SwitchToMenu();
				});
			});
		});
	}

	private void Black(float xTime, float xFrom, float xTo, StoryDelegate oCompleted)
	{
		blackTimeTotal = xTime;
		blackFrom = xFrom;
		blackTo = xTo;
		__blackCompleted = oCompleted;
		blackTime = 0f;
		Black_Lerp(0f);
		blackActive = true;
	}

	private void Black_Update(GameTime elapsed)
	{
		if (!blackActive)
		{
			return;
		}
		blackTime += (float)elapsed.ElapsedGameTime.TotalMilliseconds;
		if (blackTime >= blackTimeTotal)
		{
			blackActive = false;
			Black_Lerp(1f);
			if (__blackCompleted != null)
			{
				__blackCompleted();
			}
		}
		else
		{
			Black_Lerp(blackTime / blackTimeTotal);
		}
	}

	private void Black_Lerp(float xRatio)
	{
		float alpha = blackFrom + (blackTo - blackFrom) * xRatio;
		black.alpha = alpha;
	}

	private void Wait(float xTime, StoryDelegate oCompleted)
	{
		waitTimeTotal = xTime;
		__waitCompleted = oCompleted;
		waitTime = 0f;
		waitActive = true;
	}

	private void Wait_Update(GameTime elapsed)
	{
		if (!waitActive)
		{
			return;
		}
		waitTime += (float)elapsed.ElapsedGameTime.TotalMilliseconds;
		if (waitTime >= waitTimeTotal)
		{
			waitActive = false;
			if (__waitCompleted != null)
			{
				__waitCompleted();
			}
		}
	}

	private void Pan(int xIndex, float xTime, StoryDelegate oCompleted)
	{
		pan = pannels[xIndex];
		panTimeTotal = xTime;
		__panCompleted = oCompleted;
		if (storyMode == StoryMode.Intro)
		{
			panFromPos.X = pan.position.X;
			panFromPos.Y = pan.position.Y;
			panFromScale.X = pan.scale.X;
			panFromScale.Y = pan.scale.Y;
			panToScale.X = pan.scale.X * 1.2f;
			panToScale.Y = pan.scale.Y * 1.2f;
			panToPos.X = pan.position.X - (panToScale.X - panFromScale.X) * pan.size.X * 0.5f;
			panToPos.Y = pan.position.Y - (panToScale.Y - panFromScale.Y) * pan.size.Y * 0.5f;
			panToPos.X += (panToPos.X - panFromPos.X) * PAN_DIR[xIndex].X;
			panToPos.Y += (panToPos.Y - panFromPos.Y) * PAN_DIR[xIndex].Y;
		}
		else
		{
			panToPos.X = pan.position.X;
			panToPos.Y = pan.position.Y;
			panToScale.X = pan.scale.X;
			panToScale.Y = pan.scale.Y;
			panFromScale.X = pan.scale.X * 4f;
			panFromScale.Y = pan.scale.Y * 4f;
			panFromPos.X = pan.position.X - (panFromScale.X - panToScale.X) * pan.size.X * 0.5f;
			panFromPos.Y = pan.position.Y - (panFromScale.Y - panToScale.Y) * pan.size.Y * 0.5f;
			panFromPos.Y += (panFromPos.Y - panToPos.Y) * 0.9f;
		}
		panTime = 0f;
		Pan_Lerp(0f);
	}

	private void Pan_Start()
	{
		panActive = true;
	}

	private void Pan_Update(GameTime elapsed)
	{
		if (!panActive)
		{
			return;
		}
		panTime += (float)elapsed.ElapsedGameTime.TotalMilliseconds;
		if (panTime >= panTimeTotal)
		{
			panActive = false;
			Pan_Lerp(1f);
			if (__panCompleted != null)
			{
				__panCompleted();
			}
		}
		else
		{
			Pan_Lerp(panTime / panTimeTotal);
		}
	}

	private void Pan_Lerp(float xRatio)
	{
		float amount = xRatio;
		if (storyMode == StoryMode.Ending)
		{
			xRatio = Math.Max(Math.Min(xRatio * 1.8f - 0.3f, 1f), 0f);
			amount = (float)(Math.Cos(Math.PI + (double)xRatio * Math.PI) + 1.0) * 0.5f;
		}
		pan.scale = Vector2.Lerp(panFromScale, panToScale, amount);
		pan.position = Vector2.Lerp(panFromPos, panToPos, amount);
	}

	public override void Input_Set()
	{
		base.Input_Set();
		inputExit = new InputEntity(InputEntity.Type.Button, "ExitStory", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputExit);
		inputExit.Add(new InputButton(GamePadButton.A));
		inputExit.Add(new InputButton(GamePadButton.B));
		inputExit.Add(new InputButton(GamePadButton.Y));
		inputExit.Add(new InputButton(GamePadButton.X));
		inputExit.active = true;
	}

	private void Input_Clear()
	{
	}

	public override void Input_Update(GameTime oGameTime)
	{
		dialogs.Input_Update(oGameTime);
		if (inputExit.pressed && !exiting && active)
		{
			exiting = true;
			Halt();
		}
		base.Input_Update(oGameTime);
	}
}
