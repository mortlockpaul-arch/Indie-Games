using GKEngine;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_SpectralSeperation(EntityStack oEntityStack) : PostProcess(oEntityStack)
{
	protected struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter seperation;
	}

	private const string PATH = "Content/Effects/Post/Post_SpectralSeperation";

	protected EffectData effectData;

	protected Vector2 seperation = new Vector2(0.004f, 0.004f);

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_SpectralSeperation");
		effectData.amount = effect.Parameters["gAmount"];
		effectData.seperation = effect.Parameters["seperation"];
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
		effectData.amount.SetValue(amount);
		effectData.seperation.SetValue(seperation);
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}
}
