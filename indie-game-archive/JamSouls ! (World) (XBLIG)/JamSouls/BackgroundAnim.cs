using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class BackgroundAnim : ScenaricEntitie
{
	public AnimatedSprite m_Anim;

	public Color m_TextureColor;

	private float m_StartOffset;

	public BackgroundAnim(SpriteBatch LocalBatch, Texture2D Sprite, int FrameCount, int x, int y, int Width, int Height, float Speed, int Loop, string name, float startoffset)
	{
		m_Anim = new AnimatedSprite(LocalBatch, Sprite, FrameCount, Width, Height, Speed, Loop);
		m_Anim.m_FixedPos = new Vector2(x, y);
		TypeId = SCENARIC.TYPE_ANIM;
		Name = name;
		m_TextureColor = Color.White;
		m_StartOffset = startoffset;
		InitEntity();
	}

	public override void SetPosition(Vector2 pos)
	{
		m_Anim.m_FixedPos = pos;
	}

	public void SetTextureColor(Color color)
	{
		m_TextureColor = color;
	}

	public override Vector2 GetPosition()
	{
		return m_Anim.m_FixedPos;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_StartOffset <= 0f)
		{
			m_Anim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		}
		else
		{
			m_StartOffset -= gameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			m_Anim.DrawFixed(m_SpriteEffect, m_TextureColor, m_zOrder);
		}
	}
}
