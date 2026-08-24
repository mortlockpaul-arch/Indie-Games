using GKEngine;
using GKEngine.Scenes;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Gamma : PostProcess
{
	protected struct EffectData
	{
		public EffectParameter gamma;
	}

	private const string PATH = "Content/Effects/Post/Post_Gamma";

	protected EffectData effectData;

	public PostProcess_Gamma(EntityStack oEntityStack)
		: base(oEntityStack)
	{
	}

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_Gamma");
		effectData.gamma = effect.Parameters["gamma"];
		UpdateGamma();
	}

	public override void Unload()
	{
		base.Unload();
		effect = null;
	}

	public override void Init()
	{
		base.Init();
	}

	public override void Execute(GraphicsDevice oDevice, GameTime oGameTime)
	{
		base.Execute(oDevice, oGameTime);
		int targetIndex = GameEngine.instance.renderer.targetIndex;
		GameEngine.instance.renderer.ToggleTarget();
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}

	public void UpdateGamma()
	{
		effectData.gamma.SetValue((float)DataManager.local.settings.gamma / 10f);
		if (DataManager.local.settings.gamma <= 0)
		{
			active = false;
		}
		else
		{
			active = true;
		}
	}
}
