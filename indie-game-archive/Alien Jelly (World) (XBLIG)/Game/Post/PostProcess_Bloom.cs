using System;
using GKEngine;
using GKEngine.Edit;
using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Post;

public class PostProcess_Bloom : PostProcess, IEditable
{
	protected struct EffectData
	{
		public EffectParameter baseTexture;

		public EffectParameter sampleOffsets;

		public EffectParameter sampleWeights;

		public EffectParameter amount;

		public EffectParameter add;

		public EffectParameter multi;

		public EffectParameter composit;
	}

	private const string PATH = "Content/Effects/Post/Post_DOF";

	private EffectPass _pass;

	public float add;

	public float multi = 1.8f;

	protected EffectData effectData;

	private float blurAmount = 4f;

	public RenderTarget2D target;

	public Editable _editable;

	public Editable editable
	{
		get
		{
			return _editable;
		}
		set
		{
			_editable = value;
		}
	}

	public GUID editguid
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public PostProcess_Bloom(EntityStack oEntityStack)
		: base(oEntityStack)
	{
	}

	public override void Load()
	{
		base.Load();
		effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/Post/Post_DOF");
		effectData.amount = effect.Parameters["gAmount"];
		effectData.add = effect.Parameters["gAdd"];
		effectData.multi = effect.Parameters["gMulti"];
		effectData.composit = effect.Parameters["gComposit"];
		effectData.sampleOffsets = effect.Parameters["SampleOffsets"];
		effectData.sampleWeights = effect.Parameters["SampleWeights"];
		effectData.baseTexture = effect.Parameters["TextureBase"];
		target = new RenderTarget2D(GameEngine.Graphics.GraphicsDevice, GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
		SetBlurEffectParameters(1f / (float)GameEngine.Graphics.GraphicsDevice.Viewport.Width, 0f);
	}

	public override void Unload()
	{
		base.Unload();
		target.Dispose();
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
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(target);
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(null);
		effectData.baseTexture.SetValue(target);
		targetIndex = GameEngine.instance.renderer.targetIndex;
		GameEngine.instance.renderer.ToggleTarget();
		_pass = effect.CurrentTechnique.Passes[1];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		_pass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
		SetBlurEffectParameters(2f / (float)GameEngine.Graphics.GraphicsDevice.Viewport.Width, 0f);
		effectData.amount.SetValue(amount);
		effectData.add.SetValue(add);
		effectData.multi.SetValue(multi);
		effectData.composit.SetValue(0);
		targetIndex = GameEngine.instance.renderer.targetIndex;
		GameEngine.instance.renderer.ToggleTarget();
		_pass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		_pass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
		targetIndex = GameEngine.instance.renderer.targetIndex;
		GameEngine.instance.renderer.ToggleTarget();
		SetBlurEffectParameters(0f, 2f / (float)GameEngine.Graphics.GraphicsDevice.Viewport.Height);
		effectData.composit.SetValue(1);
		_pass = effect.CurrentTechnique.Passes[0];
		GameEngine.instance.renderer.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		_pass.Apply();
		GameEngine.instance.renderer.spriteBatch.Draw(GameEngine.instance.renderer.target[targetIndex], position, Color.White);
		GameEngine.instance.renderer.spriteBatch.End();
	}

	private void SetBlurEffectParameters(float dx, float dy)
	{
		int count = effectData.sampleWeights.Elements.Count;
		float[] array = new float[count];
		Vector2[] array2 = new Vector2[count];
		array[0] = ComputeGaussian(0f);
		ref Vector2 reference = ref array2[0];
		reference = new Vector2(0f);
		float num = array[0];
		for (int i = 0; i < count / 2; i++)
		{
			num += (array[i * 2 + 2] = (array[i * 2 + 1] = ComputeGaussian(i + 1))) * 2f;
			float num2 = (float)(i * 2) + 1.5f;
			Vector2 vector = new Vector2(dx, dy) * num2;
			array2[i * 2 + 1] = vector;
			ref Vector2 reference2 = ref array2[i * 2 + 2];
			reference2 = -vector;
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j] /= num;
		}
		effectData.sampleWeights.SetValue(array);
		effectData.sampleOffsets.SetValue(array2);
	}

	private float ComputeGaussian(float n)
	{
		float num = blurAmount;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)num) * Math.Exp((0f - n * n) / (2f * num * num)));
	}

	public void Edit_Event_Activate()
	{
		active = false;
	}

	public void Edit_Event_Deactivate()
	{
		active = true;
	}
}
