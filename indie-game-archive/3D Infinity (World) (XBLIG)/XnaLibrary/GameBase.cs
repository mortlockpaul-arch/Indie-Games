#define TRACE
using System;
using System.Collections.ObjectModel;
using DebugSample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaLibrary.Audio;
using XnaLibrary.Blade;
using XnaLibrary.Diagnostics;
using XnaLibrary.Graphics;
using XnaLibrary.Input;

namespace XnaLibrary;

public class GameBase : Game
{
	protected GraphicsDeviceManager graphics;

	protected TimeRuler timerRuler;

	public GameBase()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		((Game)this)._002Ector();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		GameServiceContainer services = ((Game)this).Services;
		Type typeFromHandle = typeof(InputComponent);
		InputComponent inputComponent = new InputComponent((Game)(object)this);
		((GameComponent)inputComponent).UpdateOrder = 0;
		services.AddService(typeFromHandle, (object)inputComponent);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(InputComponent)((Game)this).Services.GetService(typeof(InputComponent)));
		GameServiceContainer services2 = ((Game)this).Services;
		Type typeFromHandle2 = typeof(SceneManagerComponent);
		SceneManagerComponent sceneManagerComponent = new SceneManagerComponent((Game)(object)this);
		((GameComponent)sceneManagerComponent).UpdateOrder = 100;
		((DrawableGameComponent)sceneManagerComponent).DrawOrder = 100;
		services2.AddService(typeFromHandle2, (object)sceneManagerComponent);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(SceneManagerComponent)((Game)this).Services.GetService(typeof(SceneManagerComponent)));
		GameServiceContainer services3 = ((Game)this).Services;
		Type typeFromHandle3 = typeof(FadeComponent);
		FadeComponent fadeComponent = new FadeComponent((Game)(object)this);
		((GameComponent)fadeComponent).UpdateOrder = 200;
		((DrawableGameComponent)fadeComponent).DrawOrder = 200;
		services3.AddService(typeFromHandle3, (object)fadeComponent);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(FadeComponent)((Game)this).Services.GetService(typeof(FadeComponent)));
		((Game)this).Services.AddService(typeof(SoundComponent), (object)new SoundComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(SoundComponent)((Game)this).Services.GetService(typeof(SoundComponent)));
		((Game)this).Services.AddService(typeof(StorageComponent), (object)new StorageComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(StorageComponent)((Game)this).Services.GetService(typeof(StorageComponent)));
		((Game)this).Services.AddService(typeof(KeybordInputComponent), (object)new KeybordInputComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(KeybordInputComponent)((Game)this).Services.GetService(typeof(KeybordInputComponent)));
		((Game)this).Services.AddService(typeof(NetworkComponent), (object)new NetworkComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(NetworkComponent)((Game)this).Services.GetService(typeof(NetworkComponent)));
		((Game)this).Services.AddService(typeof(DrawHelperComponent), (object)new DrawHelperComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(DrawHelperComponent)((Game)this).Services.GetService(typeof(DrawHelperComponent)));
		GameServiceContainer services4 = ((Game)this).Services;
		Type typeFromHandle4 = typeof(SafeAreaComponent);
		SafeAreaComponent safeAreaComponent = new SafeAreaComponent((Game)(object)this);
		((GameComponent)safeAreaComponent).UpdateOrder = 1000;
		((DrawableGameComponent)safeAreaComponent).DrawOrder = 1000;
		((GameComponent)safeAreaComponent).Enabled = false;
		((DrawableGameComponent)safeAreaComponent).Visible = false;
		services4.AddService(typeFromHandle4, (object)safeAreaComponent);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(SafeAreaComponent)((Game)this).Services.GetService(typeof(SafeAreaComponent)));
		GameServiceContainer services5 = ((Game)this).Services;
		Type typeFromHandle5 = typeof(VariableDisplayComponent);
		VariableDisplayComponent variableDisplayComponent = new VariableDisplayComponent((Game)(object)this);
		((GameComponent)variableDisplayComponent).UpdateOrder = 1000;
		((DrawableGameComponent)variableDisplayComponent).DrawOrder = 1000;
		((GameComponent)variableDisplayComponent).Enabled = false;
		((DrawableGameComponent)variableDisplayComponent).Visible = false;
		services5.AddService(typeFromHandle5, (object)variableDisplayComponent);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(VariableDisplayComponent)((Game)this).Services.GetService(typeof(VariableDisplayComponent)));
		new DebugManager((Game)(object)this);
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(DebugManager)((Game)this).Services.GetService(typeof(DebugManager)));
		TimeRuler timeRuler = new TimeRuler((Game)(object)this);
		((DrawableGameComponent)timeRuler).DrawOrder = 1000;
		((GameComponent)timeRuler).Enabled = false;
		((DrawableGameComponent)timeRuler).Visible = false;
		timerRuler = timeRuler;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)timerRuler);
		((Game)this).Content.RootDirectory = "Content";
	}

	private DepthFormat SelectStencilMode()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GraphicsAdapter defaultAdapter = GraphicsAdapter.DefaultAdapter;
		DisplayMode currentDisplayMode = defaultAdapter.CurrentDisplayMode;
		SurfaceFormat format = ((DisplayMode)(ref currentDisplayMode)).Format;
		if (defaultAdapter.CheckDepthStencilMatch((DeviceType)1, format, format, (DepthFormat)48))
		{
			return (DepthFormat)48;
		}
		if (defaultAdapter.CheckDepthStencilMatch((DeviceType)1, format, format, (DepthFormat)49))
		{
			return (DepthFormat)49;
		}
		if (defaultAdapter.CheckDepthStencilMatch((DeviceType)1, format, format, (DepthFormat)50))
		{
			return (DepthFormat)50;
		}
		if (defaultAdapter.CheckDepthStencilMatch((DeviceType)1, format, format, (DepthFormat)56))
		{
			return (DepthFormat)56;
		}
		throw new ApplicationException("Could Not Find Stencil Buffer for Default Adapter");
	}

	private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		PresentationParameters presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;
		presentationParameters.MultiSampleQuality = 1;
		presentationParameters.MultiSampleType = (MultiSampleType)4;
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		timerRuler.StartFrame();
		timerRuler.BeginMark("Update", Color.Blue);
		if (((SceneManagerComponent)((Game)this).Services.GetService(typeof(SceneManagerComponent))).Count == 0)
		{
			((Game)this).Exit();
		}
		GamePadState state = GamePad.GetState((PlayerIndex)0);
		_ = ((GamePadState)(ref state)).Buttons;
		Keyboard.GetState();
		((Game)this).Update(gameTime);
		timerRuler.EndMark("Update");
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		timerRuler.BeginMark("Draw", Color.Yellow);
		((Game)this).Draw(gameTime);
		timerRuler.EndMark("Draw");
	}
}
