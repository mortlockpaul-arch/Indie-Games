using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Colony
{
	public Vector2 position;

	public Texture2D texture;

	public float health = 1f;

	public float healthTarget;

	public float MaximunHealth;

	public float energy;

	public float energyTarget;

	public float maximunEnergy;

	public float growH = 1f;

	public float growE = 1f;

	public bool Active;

	public bool damaged = false;

	private ushort phase;

	public int exploding;

	public int Width => texture.Width / 5;

	public int Height => texture.Height;

	public void Initialize(Texture2D texture, Vector2 position)
	{
		this.position = position;
		this.texture = texture;
		MaximunHealth = 100f;
		healthTarget = 100f;
		health = MaximunHealth / 2f;
		maximunEnergy = 10f;
		energyTarget = 0f;
		energy = maximunEnergy / 2f;
		damaged = false;
		phase = 0;
		Active = true;
		exploding = 0;
	}

	public void Update(GameTime gameTime)
	{
		healthTarget = MathHelper.Clamp(healthTarget, 0f, MaximunHealth);
		energyTarget = MathHelper.Clamp(energyTarget, 0f, maximunEnergy);
		health = MathHelper.Lerp(health, healthTarget, 0.2f);
		energy = MathHelper.Lerp(energy, energyTarget, 0.2f);
		health = MathHelper.Clamp(health, 0f, MaximunHealth);
		energy = MathHelper.Clamp(energy, 0f, maximunEnergy);
		phase = (ushort)((1f - health / MaximunHealth) * 4f);
		if (MathHelper.Distance(healthTarget, health) > 0.1f)
		{
			growH = MathHelper.Lerp(growH, 10f, 0.05f);
		}
		else
		{
			growH = MathHelper.Lerp(growH, 1f, 0.1f);
		}
		if (MathHelper.Distance(energyTarget, energy) > 0.1f)
		{
			growE = MathHelper.Lerp(growE, 10f, 0.05f);
		}
		else
		{
			growE = MathHelper.Lerp(growE, 1f, 0.1f);
		}
		if (healthTarget <= 0f || health <= 0f)
		{
			exploding++;
		}
	}

	public bool isMouseOver(Vector2 mouse)
	{
		int num = 100;
		Rectangle rectangle = new Rectangle((int)(position.X - (float)(num / 2)), (int)(position.Y - (float)(num / 2)), num, num);
		Rectangle value = new Rectangle((int)(mouse.X - 2f), (int)(mouse.Y - 2f), 4, 4);
		return rectangle.Intersects(value);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(sourceRectangle: new Rectangle(phase * Width, 0, Width, Height), texture: texture, position: position, color: Color.White, rotation: 0f, origin: new Vector2(Width / 2, Height / 2), scale: 1f, effects: SpriteEffects.None, layerDepth: 0.19f);
	}
}
