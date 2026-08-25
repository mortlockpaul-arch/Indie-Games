using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Weapons;

internal class EMPRound : WeaponRound
{
	private const float constEMPPulseDuration = 1.5f;

	private Vector3 m_Position;

	private float m_Radius;

	private double m_StartTime;

	private double m_Expires;

	private static Texture2D s_TexEMP;

	private static SpriteBatch s_SpriteBatch;

	public EMPRound(Vector3 position)
	{
		m_StartTime = TimeManager.TotalSeconds;
		m_Expires = m_StartTime + 1.5;
		m_Position = position;
		m_Radius = 0f;
	}

	public static void SetupStatics()
	{
		if (s_TexEMP == null)
		{
			s_TexEMP = MainGame.ContentMan.Load<Texture2D>("Textures/EMPRing");
			s_SpriteBatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		}
	}

	public override bool Update()
	{
		if (TimeManager.TotalSeconds > m_Expires)
		{
			return false;
		}
		return true;
	}

	public override void Draw()
	{
		double num = TimeManager.TotalSeconds - m_StartTime;
		float num2 = (float)num / 1.5f;
		float num3 = num2 * 10f;
		m_Radius = num3 * 128f * MainGame.ScreenToWorld;
		float a = MathHelper.Clamp(1f - num2, 0f, 1f);
		Color color = new Color(0f, 1f, 0f, a);
		s_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
		Vector3 vector = MainGame.Instance.GraphicsDevice.Viewport.Project(m_Position, MainGame.ProjectionMatrix, MainGame.ViewMatrix, Matrix.Identity);
		Vector2 position = new Vector2
		{
			X = vector.X - (float)MainGame.Instance.GraphicsDevice.Viewport.X,
			Y = vector.Y
		};
		s_SpriteBatch.Draw(s_TexEMP, position, null, color, 0f, new Vector2(128f, 128f), num3, SpriteEffects.None, 0f);
		s_SpriteBatch.End();
	}

	public override BoundingSphere GetBoundingSphere()
	{
		return new BoundingSphere(m_Position, m_Radius);
	}

	public override int GetHitDamage()
	{
		return 0;
	}

	public float GetPowerCutDuration()
	{
		float num = 1280f * MainGame.ScreenToWorld;
		float num2 = (num - m_Radius) / num;
		return MathHelper.Clamp(num2 * 10f, 0f, 10f);
	}
}
