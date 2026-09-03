using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSMeshInfo
{
	public int m_texture = -1;

	public int m_normalmap = -1;

	public int m_specularmap = -1;

	public float m_specular_power = 50f;

	public Vector4 m_specular_color = new Vector4(0.25f, 0.25f, 0.25f, 1f);

	public CullMode m_cullmode;
}
