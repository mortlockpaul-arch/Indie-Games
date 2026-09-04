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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_StartTime = TimeManager.TotalSeconds;
		m_Expires = m_StartTime + 1.5;
		m_Position = position;
		m_Radius = 0f;
	}

	public static void SetupStatics()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		if (s_TexEMP == null)
		{
			s_TexEMP = MainGame.ContentMan.Load<Texture2D>("Textures/EMPRing");
			s_SpriteBatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		double num = TimeManager.TotalSeconds - m_StartTime;
		float num2 = (float)num / 1.5f;
		float num3 = num2 * 10f;
		m_Radius = num3 * 128f * MainGame.ScreenToWorld;
		float num4 = MathHelper.Clamp(1f - num2, 0f, 1f);
		Color val = default(Color);
		((Color)(ref val))._002Ector(0f, 1f, 0f, num4);
		s_SpriteBatch.Begin((SpriteBlendMode)1);
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		Vector3 val2 = ((Viewport)(ref viewport)).Project(m_Position, MainGame.ProjectionMatrix, MainGame.ViewMatrix, Matrix.Identity);
		Vector2 val3 = default(Vector2);
		float x = val2.X;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		val3.X = x - (float)((Viewport)(ref viewport2)).X;
		val3.Y = val2.Y;
		s_SpriteBatch.Draw(s_TexEMP, val3, (Rectangle?)null, val, 0f, new Vector2(128f, 128f), num3, (SpriteEffects)0, 0f);
		s_SpriteBatch.End();
	}

	public override BoundingSphere GetBoundingSphere()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
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
