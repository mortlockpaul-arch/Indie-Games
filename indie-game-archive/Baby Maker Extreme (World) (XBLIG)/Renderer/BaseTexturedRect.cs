using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class BaseTexturedRect : DrawableComponent
{
	private Texture2D m_Texture;

	private Rectangle m_Rect;

	private float m_rotation;

	public Texture2D Texture => m_Texture;

	public override float Rotation => m_rotation;

	public BaseTexturedRect(Texture2D tex, Rectangle pos, float rotation, float depth)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(depth);
		m_Texture = tex;
		m_Rect = pos;
		m_vPosition = new Vector2((float)((Rectangle)(ref pos)).Left + (float)pos.Width / 2f, (float)((Rectangle)(ref pos)).Top + (float)pos.Height / 2f);
		m_vSurfaceScale = new Vector2((float)pos.Width, (float)pos.Height);
		m_rotation = rotation;
	}
}
