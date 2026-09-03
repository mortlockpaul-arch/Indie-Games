using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSModelContent(string path) : SGSContent(path)
{
	public Model m_model;

	public Vector3 m_pos = Vector3.Zero;

	public Matrix m_rot_matrix = Matrix.Identity;

	public float m_light_energy = 1f;

	public List<SGSMeshData> m_mesh_data;

	public List<Texture2D> m_textures;

	public List<Texture2D> m_normalmaps;

	public List<Texture2D> m_specularmaps;

	public override void Clear()
	{
		m_model = null;
		if (m_mesh_data != null)
		{
			for (int i = 0; i < m_mesh_data.Count; i++)
			{
				m_mesh_data[i].Clear();
			}
			m_mesh_data.Clear();
			m_mesh_data = null;
		}
		if (m_textures != null)
		{
			for (int j = 0; j < m_textures.Count; j++)
			{
				m_textures[j] = null;
			}
			m_textures.Clear();
			m_textures = null;
		}
		if (m_normalmaps != null)
		{
			for (int k = 0; k < m_normalmaps.Count; k++)
			{
				m_normalmaps[k] = null;
			}
			m_normalmaps.Clear();
			m_normalmaps = null;
		}
		if (m_specularmaps != null)
		{
			for (int l = 0; l < m_specularmaps.Count; l++)
			{
				m_specularmaps[l] = null;
			}
			m_specularmaps.Clear();
			m_specularmaps = null;
		}
	}

	public void Load(ContentManager CM, string path)
	{
		Clear();
		if (CM == null)
		{
			return;
		}
		SGSXML sGSXML = CM.Load<SGSXML>(path + "ModelData");
		SGSXMLData data = sGSXML.GetData("Model");
		if (data == null)
		{
			return;
		}
		m_model = CM.Load<Model>(path + (string)data.GetField(0));
		m_pos = (Vector3)data.GetField(1);
		m_rot_matrix = (Matrix)data.GetField(2);
		if (data.GetField(4) != null)
		{
			m_light_energy = (float)data.GetField(4);
		}
		data = null;
		data = sGSXML.GetData("Textures");
		if (data == null)
		{
			return;
		}
		m_textures = new List<Texture2D>();
		for (int i = 0; i < data.FIELDS.Count; i++)
		{
			m_textures.Add(CM.Load<Texture2D>(path + (string)data.GetField(i)));
		}
		data = null;
		data = sGSXML.GetData("Normalmaps");
		if (data == null)
		{
			return;
		}
		m_normalmaps = new List<Texture2D>();
		for (int j = 0; j < data.FIELDS.Count; j++)
		{
			m_normalmaps.Add(CM.Load<Texture2D>(path + (string)data.GetField(j)));
		}
		data = null;
		data = sGSXML.GetData("Specularmaps");
		if (data == null)
		{
			return;
		}
		m_specularmaps = new List<Texture2D>();
		for (int k = 0; k < data.FIELDS.Count; k++)
		{
			m_specularmaps.Add(CM.Load<Texture2D>(path + (string)data.GetField(k)));
		}
		data = null;
		data = sGSXML.GetData("Meshes");
		if (data == null)
		{
			return;
		}
		int num = (int)data.GetField(0);
		data = null;
		if (m_mesh_data == null)
		{
			m_mesh_data = new List<SGSMeshData>();
		}
		for (int l = 0; l < m_model.Meshes.Count; l++)
		{
			m_mesh_data.Add(new SGSMeshData(reset: true));
		}
		for (int m = 0; m < num; m++)
		{
			data = sGSXML.GetData("MeshData" + m);
			if (data != null)
			{
				SGSMeshData value = m_mesh_data[m];
				int num2 = (int)data.GetField(0);
				if (num2 >= 0)
				{
					value.m_texture = m_textures[num2];
				}
				num2 = (int)data.GetField(1);
				if (num2 >= 0)
				{
					value.m_normalmap = m_normalmaps[num2];
				}
				num2 = (int)data.GetField(2);
				if (num2 >= 0)
				{
					value.m_specularmap = m_specularmaps[num2];
				}
				value.m_specular_power = (float)data.GetField(3);
				value.m_specular_color = (Vector4)data.GetField(4);
				m_mesh_data[m] = value;
			}
		}
	}
}
