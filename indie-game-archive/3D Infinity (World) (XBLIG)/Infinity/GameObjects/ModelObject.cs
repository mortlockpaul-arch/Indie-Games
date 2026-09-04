using System;
using Microsoft.Xna.Framework;

namespace Infinity.GameObjects;

public abstract class ModelObject : IDisposable
{
	public Action<int> Destruction;

	protected Game game;

	public Vector3 Position;

	public XSIModel model;

	public XSIModel collision;

	public bool Use { get; set; }

	public bool Enable { get; set; }

	public bool Visible { get; set; }

	public int Vitality { get; set; }

	public BoundingSphere[] BoundingSpheres
	{
		get
		{
			if (collision == null)
			{
				return null;
			}
			return collision.Spheres;
		}
	}

	public event Action<string> SoundPlay;

	public ModelObject(Game game)
	{
		this.game = game;
	}

	public abstract void Initialize();

	public virtual void Dispose()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Use = false;
		Enable = false;
		Visible = false;
		Vitality = 0;
		Position = Vector3.Zero;
	}

	public void Update(TimeSpan elapsedGameTime)
	{
		if (Enable)
		{
			UpdateMain(elapsedGameTime);
		}
	}

	public abstract void UpdateMain(TimeSpan elapsedGameTime);

	public abstract void Draw(GameTime gameTime);

	public abstract bool Damage(int damage);

	public abstract Matrix GetWorld();

	public abstract Vector3 GetPosition();

	protected void PlaySE(string cue)
	{
		if (SoundPlay != null)
		{
			SoundPlay(cue);
		}
	}
}
