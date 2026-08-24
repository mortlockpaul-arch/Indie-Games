using System;
using GKEngine;
using GKEngine.Animation;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Splat(EntityStack oEntityStack) : PostProcess(oEntityStack)
{
	protected struct EffectData
	{
		public EffectParameter amount;

		public EffectParameter distort;
	}

	private const string PATH_EFFECT = "Content/Effects/Post/Post_Splat";

	private const string PATH_TEXTURE_NORMAL = "Content/Materials/Post/Splat";

	private const float TEXTURE_WIDTH = 1024f;

	private const float TEXTURE_HEIGHT = 720f;

	private const int ANIM_IN_TIME = 100;

	private const int ANIM_OUT_TIME = 5000;

	protected EffectData effectData;

	private Vector2 textureScale;

	private float time;

	private Range animScaleX = new Range();

	private Range animScaleY = new Range();

	private Range animOffsetX = new Range();

	private Range animOffsetY = new Range();

	public override void Load()
	{
		base.Load();
		textureScale = new Vector2(1024f / (float)GameEngine.Graphics.GraphicsDevice.Viewport.Width, 720f / (float)GameEngine.Graphics.GraphicsDevice.Viewport.Height);
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_Splat").Clone();
		effectData.amount = effect.Parameters["amount"];
		effectData.distort = effect.Parameters["distort"];
		effect.Parameters["TextureDepth"].SetValue(GameEngine.SceneContent.Load<Texture2D>("Content/Materials/Post/Splat"));
		effect.Parameters["offset"].SetValue(new Vector2((1f - textureScale.X) * 0.5f, (1f - textureScale.Y) * 0.4f));
		effect.Parameters["scale"].SetValue(new Vector2(1f / textureScale.X, 1f / textureScale.Y));
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
		EffectPass effectPass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		effectPass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}

	public void SetTint(Color oTint)
	{
		effect.Parameters["tint"].SetValue(oTint.ToVector4());
	}

	public void Anim_In()
	{
		time = 0f;
		effectData.distort.SetValue(0);
		animOffsetX.from = 0.5f;
		animOffsetX.to = (1f - textureScale.X) * 0.5f;
		animOffsetY.from = 0.5f;
		animOffsetY.to = (1f - textureScale.Y) * 0.4f;
		active = true;
		GameEngine.instance.updateStack.Add(Anim_In_Update);
	}

	private bool Anim_In_Update(GameTime oGameTime)
	{
		bool result = false;
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= 100f)
		{
			Anim_In_Lerp(100f);
			(scene as PlayScene).audio.EventCues_Trigger("Sound_Splat");
			Anim_Out();
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
		float num = (amount = Tween.EaseIn_Exp(xTime, 0f, 1f, 100f));
		effect.Parameters["offset"].SetValue(new Vector2(animOffsetX.Lerp(num), animOffsetY.Lerp(num)));
		effect.Parameters["scale"].SetValue(new Vector2(1f / (textureScale.X * Math.Max(num, 1E-09f)), 1f / (textureScale.Y * Math.Max(num, 1E-09f))));
	}

	public void Anim_Out()
	{
		time = 0f;
		effectData.distort.SetValue(0);
		animOffsetY.from = (1f - textureScale.Y) * 0.4f;
		animOffsetY.to = 1f;
		active = true;
		GameEngine.instance.updateStack.Add(Anim_Out_Update);
	}

	private bool Anim_Out_Update(GameTime oGameTime)
	{
		bool result = false;
		time += oGameTime.ElapsedGameTime.Milliseconds;
		if (time >= 5000f)
		{
			Anim_Out_Lerp(5000f);
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
		float num = xTime / 5000f;
		amount = 1f - num;
		effectData.distort.SetValue(num);
		effect.Parameters["scale"].SetValue(new Vector2(1f / textureScale.X, 1f / (textureScale.Y * (1f + num * 2f))));
	}
}
