using System.Collections.Generic;
using GKEngine;
using GKEngine.Animation;
using GKEngine.Entities;
using GKEngine.Input;
using GKEngine.Scenes;
using Game.Audio;
using Game.Data;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.Dialogs;

public class DialogManager
{
	public enum ExitEvent
	{
		None,
		A,
		B,
		X,
		Y
	}

	public enum Mode
	{
		None,
		In,
		Dialog,
		Out
	}

	public delegate void PauseDelegate(bool xPause);

	public Scene scene;

	public AudioManager audio;

	public SpriteManager spriteManager;

	public bool active;

	public Mode mode;

	public EntityStack renderStack;

	private PostProcess post;

	private PostProcess[] postStack;

	public ExitEvent exitEventType;

	public Dialog.DialogDelegate exitEvent;

	public Dialog dialog;

	public Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

	private float time;

	private float timeTotal;

	public PauseDelegate __pause;

	public Dialog.DialogDelegate ___oop;

	public int oopTick;

	public SpriteFont fontKH_15;

	public SpriteFont fontKA_20;

	public SpriteFont fontKA_25;

	public SpriteFont fontKA_30;

	public SpriteFont fontKA_40;

	public SpriteFont fontKA_60;

	public object data;

	public SpriteManager spriteMessageManager;

	public Sprite spriteSaving;

	public Sprite spriteSavingBackground;

	public Dialog.DialogDelegate __oop
	{
		get
		{
			return ___oop;
		}
		set
		{
			___oop = value;
			oopTick = 3;
		}
	}

	public DialogManager(Scene oScene, EntityStack oRenderStack, PauseDelegate oPause, PostProcess[] aPost, AudioManager oAudio)
	{
		scene = oScene;
		renderStack = oRenderStack;
		__pause = oPause;
		audio = oAudio;
		postStack = aPost;
		post = postStack[0];
		Init();
	}

	public void Init()
	{
		spriteManager = new SpriteManager(scene, renderStack);
		spriteManager.visible = false;
		spriteManager.effect = null;
		Message_Init();
		Fonts_Init();
		Input_Set();
	}

	public void Update(GameTime oGameTime)
	{
		if (dialog == null)
		{
			return;
		}
		if (__oop != null)
		{
			oopTick--;
			if (oopTick <= 0)
			{
				__oop(dialog);
				__oop = null;
			}
		}
		switch (mode)
		{
		case Mode.In:
			Dialog_In_Update(oGameTime);
			break;
		case Mode.Dialog:
			dialog.Update(oGameTime);
			break;
		case Mode.Out:
			Dialog_Out_Update(oGameTime);
			break;
		case Mode.None:
			break;
		}
	}

	public void Dispose()
	{
		active = false;
		mode = Mode.None;
		Message_Dispose();
		Fonts_Dispose();
		spriteManager.Dispose();
		Dialogs_Dispose();
		post = null;
		renderStack = null;
		exitEventType = ExitEvent.None;
		exitEvent = null;
		__pause = null;
	}

	private void Fonts_Init()
	{
		fontKA_60 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_60");
		fontKA_40 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_40");
		fontKA_30 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_30");
		fontKA_25 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_25");
		fontKA_20 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_20");
		fontKH_15 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KH_15");
	}

	private void Fonts_Dispose()
	{
		fontKA_60 = null;
		fontKA_40 = null;
		fontKA_30 = null;
		fontKA_25 = null;
		fontKA_20 = null;
		fontKH_15 = null;
	}

	public void Open(string xName)
	{
		__pause(xPause: true);
		spriteManager.visible = true;
		dialog = dialogs[xName];
		exitEventType = ExitEvent.None;
		exitEvent = null;
		dialog.Show();
		data = null;
		Post_Show();
		Post_Lerp(1f);
		dialog.Event_In_Start();
		dialog.Event_In_Lerp(1f);
		mode = Mode.Dialog;
		Input_Activate();
		dialog.Event_In_Done();
	}

	public void Show(string xName)
	{
		Show(xName, -1f);
	}

	public void Show(string xName, float xTime)
	{
		__pause(xPause: true);
		spriteManager.visible = true;
		dialog = dialogs[xName];
		Dialog_In(xTime);
	}

	public void Close(Dialog.DialogDelegate oEvent)
	{
		UniversalInput.FlushStates();
		Dialog_Out(ExitEvent.None, oEvent);
	}

	public void Dialog_In(float xTime)
	{
		exitEventType = ExitEvent.None;
		exitEvent = null;
		time = 0f;
		timeTotal = ((xTime >= 0f) ? xTime : dialog.timeIn);
		dialog.Show();
		data = null;
		dialog.Event_In_Start();
		dialog.Event_In_Lerp(0f);
		Post_Show();
		mode = Mode.In;
	}

	public void Dialog_In_Update(GameTime oGameTime)
	{
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= timeTotal)
		{
			Post_Lerp(1f);
			dialog.Event_In_Lerp(1f);
			mode = Mode.Dialog;
			Input_Activate();
			dialog.Event_In_Done();
		}
		else
		{
			float xRatio = time / timeTotal;
			float xRatio2 = Tween.EaseInOut(xRatio);
			dialog.Event_In_Lerp(xRatio2);
			Post_Lerp(xRatio2);
		}
	}

	public void Dialog_Out(ExitEvent xExitEvent, Dialog.DialogDelegate oEvent)
	{
		Input_Deactivate();
		exitEventType = xExitEvent;
		exitEvent = oEvent;
		time = 0f;
		timeTotal = dialog.timeOut;
		dialog.Event_Out_Start();
		dialog.Event_Out_Lerp(0f);
		Post_Show();
		Post_Lerp(1f);
		mode = Mode.Out;
	}

	public void Dialog_Out_Update(GameTime oGameTime)
	{
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= timeTotal)
		{
			this.dialog.Event_Out_Lerp(1f);
			this.dialog.Event_Out_Done();
			this.dialog.Hide();
			Post_Hide();
			Dialog dialog = this.dialog;
			this.dialog = null;
			mode = Mode.None;
			spriteManager.visible = false;
			__pause(xPause: false);
			if (exitEvent != null)
			{
				exitEvent(dialog);
			}
			exitEventType = ExitEvent.None;
			exitEvent = null;
		}
		else
		{
			float xRatio = time / timeTotal;
			float num = Tween.EaseInOut(xRatio);
			this.dialog.Event_Out_Lerp(num);
			Post_Lerp(1f - num);
		}
	}

	public void Dialogs_Dispose()
	{
		dialog = null;
		foreach (KeyValuePair<string, Dialog> dialog in dialogs)
		{
			dialog.Value.Dispose();
		}
		dialogs.Clear();
	}

	public void Utils_AudioUpdate()
	{
		if (dialog.manager.scene is PlayScene)
		{
			(dialog.manager.scene as PlayScene).audio.FromSettings();
		}
		else if (dialog.manager.scene is BuildScene)
		{
			(dialog.manager.scene as BuildScene).audio.FromSettings();
		}
		else if (dialog.manager.scene is MenuScene)
		{
			(dialog.manager.scene as MenuScene).audio.FromSettings();
		}
	}

	private void Post_Show()
	{
		if (dialog.postIndex >= 0)
		{
			post = postStack[dialog.postIndex];
		}
		if (post != null)
		{
			if (dialog.usePostShader)
			{
				post.amount = 0f;
				post.active = true;
			}
			else
			{
				post.active = false;
			}
		}
	}

	private void Post_Lerp(float xRatio)
	{
		if (post != null && dialog.usePostShader)
		{
			post.amount = xRatio;
		}
	}

	private void Post_Hide()
	{
		if (post != null && dialog.usePostShader)
		{
			post.active = false;
		}
	}

	private void Message_Init()
	{
		spriteMessageManager = new SpriteManager(scene, renderStack);
		spriteMessageManager.visible = false;
		spriteMessageManager.effect = null;
		spriteSavingBackground = new Sprite(spriteMessageManager);
		spriteSavingBackground.texture = GameEngine.instance.GetSolidColorTexture(new Color(0, 0, 0, 128));
		spriteSaving = new Sprite(spriteMessageManager);
		spriteSaving.texture = GameEngine.Content.Load<Texture2D>("Content/UI/Dialogs/Saving/Message");
		spriteSaving.visible = false;
		spriteSavingBackground.visible = false;
	}

	private void Message_Dispose()
	{
		spriteSaving.Dispose();
		spriteSavingBackground.Dispose();
		spriteSaving = null;
		spriteSavingBackground = null;
		spriteMessageManager.Dispose();
	}

	public void Message_Saving_Show()
	{
		spriteSaving.position.X = (float)DataManager.local.settings.screen.X + ((float)DataManager.local.settings.screen.Width - spriteSaving.size.X) * 0.5f;
		spriteSaving.position.Y = (float)DataManager.local.settings.screen.Y + ((float)DataManager.local.settings.screen.Height - spriteSaving.size.Y) * 0.5f;
		spriteSavingBackground.scale.X = GameEngine.Graphics.GraphicsDevice.Viewport.Width;
		spriteSavingBackground.scale.Y = GameEngine.Graphics.GraphicsDevice.Viewport.Height;
		spriteMessageManager.visible = true;
		spriteSaving.visible = true;
		spriteSavingBackground.visible = true;
		spriteMessageManager.Render(new GameTime());
	}

	public void Message_Saving_Hide()
	{
		spriteMessageManager.visible = false;
		spriteSaving.visible = false;
		spriteSavingBackground.visible = false;
	}

	public void Input_Set()
	{
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DialogStart", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogStart"].Add(new InputButton(GamePadButton.Start));
		UniversalInput.inputEntities["DialogStart"].Add(new InputButton(Keys.Enter));
		UniversalInput.inputEntities["DialogStart"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DialogA", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogA"].Add(new InputButton(GamePadButton.A));
		UniversalInput.inputEntities["DialogA"].Add(new InputButton(Keys.Z));
		UniversalInput.inputEntities["DialogA"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DialogB", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogB"].Add(new InputButton(GamePadButton.B));
		UniversalInput.inputEntities["DialogB"].Add(new InputButton(Keys.X));
		UniversalInput.inputEntities["DialogB"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DialogX", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogX"].Add(new InputButton(GamePadButton.X));
		UniversalInput.inputEntities["DialogX"].Add(new InputButton(Keys.C));
		UniversalInput.inputEntities["DialogX"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DialogY", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogY"].Add(new InputButton(GamePadButton.Y));
		UniversalInput.inputEntities["DialogY"].Add(new InputButton(Keys.V));
		UniversalInput.inputEntities["DialogY"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog2D, "DialogStick", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogStick"].Add(new InputAnalog2D(GamePadAnalog2D.Left));
		UniversalInput.inputEntities["DialogStick"].active = false;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog2D, "DialogStickRight", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DialogStickRight"].Add(new InputAnalog2D(GamePadAnalog2D.Right));
		UniversalInput.inputEntities["DialogStickRight"].active = true;
	}

	public void Input_Update(GameTime oGameTime)
	{
		if (dialog != null && mode == Mode.Dialog)
		{
			dialog.Input_Update(oGameTime);
		}
	}

	public void Input_Activate()
	{
		UniversalInput.inputEntities["DialogStart"].active = true;
		UniversalInput.inputEntities["DialogA"].active = true;
		UniversalInput.inputEntities["DialogB"].active = true;
		UniversalInput.inputEntities["DialogX"].active = true;
		UniversalInput.inputEntities["DialogY"].active = true;
		UniversalInput.inputEntities["DialogStick"].active = true;
	}

	public void Input_Deactivate()
	{
		UniversalInput.inputEntities["DialogStart"].active = false;
		UniversalInput.inputEntities["DialogA"].active = false;
		UniversalInput.inputEntities["DialogB"].active = false;
		UniversalInput.inputEntities["DialogX"].active = false;
		UniversalInput.inputEntities["DialogY"].active = false;
		UniversalInput.inputEntities["DialogStick"].active = false;
	}
}
