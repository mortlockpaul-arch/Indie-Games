using GKEngine;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Dialog_Title(EntityStack oEntityStack) : PostProcess(oEntityStack)
{
	protected struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter time;
	}

	private const string PATH = "Content/Effects/Post/Post_Dialog_Title";

	protected EffectData effectData;

	private float time;

	private float timeTotal = 20000f;

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_Dialog_Title").Clone();
		effectData.amount = effect.Parameters["amount"];
		effectData.time = effect.Parameters["time"];
		effect.Parameters["TextureAlpha"].SetValue(GameEngine.SceneContent.Load<Texture2D>("Content/Materials/Post/TextureRadial_0"));
	}

	public override void Unload()
	{
		if (effect != null)
		{
			effect.Dispose();
		}
		effect = null;
		base.Unload();
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
		time += oGameTime.ElapsedGameTime.Milliseconds;
		time %= timeTotal;
		effectData.amount.SetValue(amount);
		effectData.time.SetValue(time / timeTotal);
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, effect);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}
}
