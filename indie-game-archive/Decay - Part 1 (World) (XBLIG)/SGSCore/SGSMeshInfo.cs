using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSMeshInfo
{
	public int m_texture;

	public int m_normalmap;

	public int m_specularmap;

	public float m_specular_power;

	public Vector4 m_specular_color;

	public CullMode m_cullmode;

	public SGSMeshInfo()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		m_texture = -1;
		m_normalmap = -1;
		m_specularmap = -1;
		m_specular_power = 50f;
		m_specular_color = new Vector4(0.25f, 0.25f, 0.25f, 1f);
		base._002Ector();
	}
}
