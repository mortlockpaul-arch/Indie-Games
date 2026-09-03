using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct SGSMeshData(bool reset)
{
	public Texture2D m_texture = null;

	public Texture2D m_normalmap = null;

	public Texture2D m_specularmap = null;

	public float m_specular_power = 50f;

	public Vector4 m_specular_color = new Vector4(0.25f, 0.25f, 0.25f, 1f);

	public CullMode m_cullmode = CullMode.CullCounterClockwiseFace;

	public void Clear()
	{
		m_texture = null;
		m_normalmap = null;
		m_specularmap = null;
	}
}
