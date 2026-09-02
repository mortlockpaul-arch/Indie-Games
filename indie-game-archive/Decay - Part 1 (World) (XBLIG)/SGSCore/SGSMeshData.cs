using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public struct SGSMeshData
{
	public Texture2D m_texture;

	public Texture2D m_normalmap;

	public Texture2D m_specularmap;

	public float m_specular_power;

	public Vector4 m_specular_color;

	public CullMode m_cullmode;

	public SGSMeshData(bool reset)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		m_texture = null;
		m_normalmap = null;
		m_specularmap = null;
		m_specular_power = 50f;
		m_specular_color = new Vector4(0.25f, 0.25f, 0.25f, 1f);
		m_cullmode = (CullMode)3;
	}

	public void Clear()
	{
		m_texture = null;
		m_normalmap = null;
		m_specularmap = null;
	}
}
