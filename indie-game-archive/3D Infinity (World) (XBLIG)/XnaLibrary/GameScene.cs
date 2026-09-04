using System;
using DebugSample;
using Microsoft.Xna.Framework;
using XnaLibrary.Audio;
using XnaLibrary.Blade;
using XnaLibrary.Diagnostics;
using XnaLibrary.Graphics;
using XnaLibrary.Input;

namespace XnaLibrary;

public class GameScene : GameObject
{
	protected enum FadePhase
	{
		In,
		Main,
		Out
	}

	protected FadePhase fadePhase;

	public SceneManagerComponent SceneManager => (SceneManagerComponent)base.Game.Services.GetService(typeof(SceneManagerComponent));

	public VariableDisplayComponent VariableDisplay => (VariableDisplayComponent)base.Game.Services.GetService(typeof(VariableDisplayComponent));

	public FadeComponent Fade => (FadeComponent)base.Game.Services.GetService(typeof(FadeComponent));

	public InputComponent Input => (InputComponent)base.Game.Services.GetService(typeof(InputComponent));

	public SoundComponent Sound => (SoundComponent)base.Game.Services.GetService(typeof(SoundComponent));

	public StorageComponent Storage => (StorageComponent)base.Game.Services.GetService(typeof(StorageComponent));

	public NetworkComponent Network => (NetworkComponent)base.Game.Services.GetService(typeof(NetworkComponent));

	public DrawHelperComponent DrawHelper => (DrawHelperComponent)base.Game.Services.GetService(typeof(DrawHelperComponent));

	public TimeRuler TimeRuler => (TimeRuler)base.Game.Services.GetService(typeof(TimeRuler));

	public GameScene(Game game)
		: base(game)
	{
		base.Enabled = true;
		base.Visible = true;
	}

	public override void Initialize()
	{
		FadeIn();
		base.Initialize();
	}

	protected virtual void FadeIn()
	{
		fadePhase = FadePhase.In;
		Fade.FadeIn();
		Fade.FadeFinished += FadeInFinished;
	}

	protected virtual void FadeInFinished(object sender, EventArgs e)
	{
		fadePhase = FadePhase.Main;
		Fade.ClearEvents();
	}

	protected virtual void FadeOut()
	{
		fadePhase = FadePhase.Out;
		Fade.FadeOut();
		Fade.FadeFinished += FadeOutFinished;
	}

	protected virtual void FadeOutFinished(object sender, EventArgs e)
	{
		Fade.ClearEvents();
		Dispose();
	}
}
