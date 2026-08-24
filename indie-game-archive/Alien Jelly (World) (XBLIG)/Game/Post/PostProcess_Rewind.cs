using GKEngine;
using GKEngine.Animation;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Rewind : PostProcess
{
	protected struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter greenOffset;

		public EffectParameter blueOffset;

		public EffectParameter time;
	}

	private const string PATH = "Content/Effects/Post/Post_Rewind";

	private const int ANIM_IN_TIME = 1000;

	private const int ANIM_OUT_TIME = 1000;

	protected EffectData effectData;

	private float time;

	public PostProcess_Rewind(EntityStack oEntityStack)
		: base(oEntityStack)
	{
	}

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_Rewind");
		effectData.amount = effect.Parameters["gAmount"];
		effectData.greenOffset = effect.Parameters["greenOffset"];
		effectData.blueOffset = effect.Parameters["blueOffset"];
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
		effectData.amount.SetValue(amount);
		if (scene.gameTime != null)
		{
			effectData.time.SetValue((float)scene.gameTime.TotalGameTime.TotalMilliseconds);
		}
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}

	public void Anim_In()
	{
		GameEngine.instance.updateStack.stack.Remove(Anim_In_Update);
		GameEngine.instance.updateStack.stack.Remove(Anim_Out_Update);
		time = 0f;
		active = true;
		GameEngine.instance.updateStack.Add(Anim_In_Update);
	}

	private bool Anim_In_Update(GameTime oGameTime)
	{
		bool result = false;
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= 1000f)
		{
			Anim_In_Lerp(1000f);
			result = true;
		}
		else
		{
			Anim_In_Lerp(time);
		}
		return result;
	}

	private void Anim_In_Lerp(float xTime)
	{
		float num = Tween.EaseOut_Exp(xTime, 0f, 1f, 1000f);
		amount = num;
	}

	public void Anim_Out()
	{
		GameEngine.instance.updateStack.stack.Remove(Anim_In_Update);
		GameEngine.instance.updateStack.stack.Remove(Anim_Out_Update);
		time = 0f;
		active = true;
		GameEngine.instance.updateStack.Add(Anim_Out_Update);
	}

	private bool Anim_Out_Update(GameTime oGameTime)
	{
		bool result = false;
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= 1000f)
		{
			Anim_Out_Lerp(1000f);
			active = false;
			result = true;
		}
		else
		{
			Anim_Out_Lerp(time);
		}
		return result;
	}

	private void Anim_Out_Lerp(float xTime)
	{
		float num = Tween.EaseOut_Exp(xTime, 1f, -1f, 1000f);
		amount = num;
	}
}
