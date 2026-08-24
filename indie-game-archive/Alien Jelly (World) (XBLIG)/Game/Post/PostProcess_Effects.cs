using GKEngine;
using GKEngine.Scenes;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Effects : PostProcess
{
	public struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter textureDepth;
	}

	private const string PATH = "Content/Effects/Post/Post_Effects";

	private const string PATH_BUILD = "Content/Effects/Post/Post_Effects_Build";

	public EffectData effectData;

	public PostProcess_Effects(EntityStack oEntityStack)
		: base(oEntityStack)
	{
	}

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>((scene is BuildScene) ? "Content/Effects/Post/Post_Effects_Build" : "Content/Effects/Post/Post_Effects");
		effectData.amount = effect.Parameters["amount"];
		effectData.textureDepth = effect.Parameters["TextureDepth"];
	}

	public override void Unload()
	{
		base.Unload();
		effect = null;
		effectData.textureDepth = null;
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
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}
}
