using Renderer;

namespace PlayObjects;

public class OutfitPiece
{
	private SpriteInstance m_attachTo;

	private SpriteInstance m_sprite;

	private int m_iSlotIndex;

	public int Slot => m_iSlotIndex;

	public SpriteInstance AttachedTo
	{
		get
		{
			return m_attachTo;
		}
		set
		{
			m_attachTo = value;
		}
	}

	public OutfitPiece(SpriteInstance spr, SpriteInstance attachedTo, int slot)
	{
		m_attachTo = attachedTo;
		m_sprite = spr;
		m_iSlotIndex = slot;
	}

	public void Update(TimeTracker gameTime)
	{
		m_sprite.Position = m_attachTo.Position;
		m_sprite.Rotation = m_attachTo.Rotation;
		m_sprite.Depth = m_attachTo.Depth + 1E-05f;
	}

	public void Draw(TimeTracker gameTime)
	{
		m_sprite.Draw(gameTime);
	}
}
