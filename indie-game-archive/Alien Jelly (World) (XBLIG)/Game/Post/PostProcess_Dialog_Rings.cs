using GKEngine;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Dialog_Rings(EntityStack oEntityStack) : PostProcess(oEntityStack)
{
	protected struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter time;
	}

	private const string PATH = "Content/Effects/Post/Post_Dialog_Rings";

	protected EffectData effectData;

	private float time;

	private float timeTotal = 2000f;

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_Dialog_Rings");
		effectData.amount = effect.Parameters["gAmount"];
		effectData.time = effect.Parameters["time"];
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
		time += oGameTime.ElapsedGameTime.Milliseconds;
		time %= timeTotal;
		effectData.amount.SetValue(amount);
		effectData.time.SetValue(time / timeTotal);
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}
}
