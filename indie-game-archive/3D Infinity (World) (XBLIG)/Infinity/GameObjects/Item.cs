using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity.GameObjects;

public class Item : ModelObject
{
	public Action Effect;

	[CompilerGenerated]
	private Vector3 _003CVelocity_003Ek__BackingField;

	public string Name { get; set; }

	public Vector3 Velocity
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CVelocity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CVelocity_003Ek__BackingField = value;
		}
	}

	public TimeSpan UpdateTime { get; set; }

	public Item(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		ContentManager content = game.Content;
		if (!string.IsNullOrEmpty(Name))
		{
			string text = $"Models/Models/player/player_item_{Name}";
			model = new XSIModel(text, content);
			collision = new XSIModel(text + "_col", content);
		}
		if (model != null)
		{
			model.Play(isLoop: true);
		}
		if (collision != null)
		{
			collision.Play(isLoop: true);
		}
	}

	public override void Dispose()
	{
		model = null;
		collision = null;
		UpdateTime = TimeSpan.Zero;
		base.Dispose();
	}

	public override void UpdateMain(TimeSpan elapsedGameTime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		UpdateTime += elapsedGameTime;
		Position += Velocity * Global.GameSpeed;
		collision.UpdateBoundingSphere(GetWorld());
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDevice graphicsDevice = game.GraphicsDevice;
		if (model != null)
		{
			CustomParticleSystem.SetParticleRenderStates(graphicsDevice.RenderState, (SpriteBlendMode)2);
			model.FixedUpdate(UpdateTime);
			model.Draw(Global.SASData, GetWorld());
			CustomParticleSystem.SetParticleRenderStates(graphicsDevice.RenderState, (SpriteBlendMode)1);
		}
	}

	public override Matrix GetWorld()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(Position);
	}

	public override Vector3 GetPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = GetWorld();
		return ((Matrix)(ref world)).Translation;
	}

	public override bool Damage(int damage)
	{
		throw new NotImplementedException();
	}

	public void ActionEffect()
	{
		if (Effect != null)
		{
			Effect();
		}
	}
}
