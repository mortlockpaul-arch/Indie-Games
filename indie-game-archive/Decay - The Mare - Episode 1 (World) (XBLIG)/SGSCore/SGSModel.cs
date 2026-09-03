using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSModel
{
	public Model m_model;

	public Effect m_effect;

	public List<Texture2D> m_textures;

	public List<Texture2D> m_normalmaps;

	public List<Texture2D> m_specularmaps;

	public List<SGSMeshData> m_mesh_data = new List<SGSMeshData>();

	public Matrix m_rot_matrix = Matrix.Identity;

	public Matrix m_world_matrix = Matrix.Identity;

	public Matrix[] m_bones;

	public Vector3 m_pos = Vector3.Zero;

	protected Vector3 m_rot_delta = Vector3.Zero;

	protected bool m_update_transform;

	public AnimationPlayer m_animation_player;

	private SkinningData m_skinningData;

	public string[] m_animation_clips;

	public List<SGSModel> m_attachments = new List<SGSModel>();

	public int m_attached_to = -1;

	public float m_light_energy = 1f;

	public Vector4 m_light_dir = new Vector4(0f, 0f, -1f, 1f);

	protected RasterizerState m_RS_counter_clockwise;

	protected RasterizerState m_RS_clockwise;

	protected RasterizerState m_RS_none;

	public SGSModel()
	{
	}

	public SGSModel(SGSModelContent MC, Effect effect)
	{
		if (MC == null || MC.m_model == null)
		{
			return;
		}
		m_model = MC.m_model;
		m_bones = new Matrix[m_model.Bones.Count];
		m_model.CopyAbsoluteBoneTransformsTo(m_bones);
		if (m_RS_counter_clockwise == null)
		{
			m_RS_counter_clockwise = new RasterizerState();
		}
		if (m_RS_clockwise == null)
		{
			m_RS_clockwise = new RasterizerState();
			m_RS_clockwise.CullMode = CullMode.CullClockwiseFace;
		}
		if (m_RS_none == null)
		{
			m_RS_none = new RasterizerState();
			m_RS_none.CullMode = CullMode.None;
		}
		if (m_model != null)
		{
			if (m_mesh_data == null)
			{
				m_mesh_data = new List<SGSMeshData>();
			}
			for (int i = 0; i < m_model.Meshes.Count; i++)
			{
				m_mesh_data.Add(new SGSMeshData(reset: true));
			}
		}
		m_skinningData = m_model.Tag as SkinningData;
		if (m_skinningData != null)
		{
			m_animation_player = new AnimationPlayer(m_skinningData);
			m_animation_clips = new string[m_skinningData.AnimationClips.Keys.Count];
			for (int j = 0; j < m_skinningData.AnimationClips.Keys.Count; j++)
			{
				m_animation_clips[j] = m_skinningData.AnimationClips.Keys.ElementAt(j);
			}
			if (m_animation_clips.Length > 0)
			{
				m_animation_player.StartClip(m_skinningData.AnimationClips[m_animation_clips[0]]);
				m_animation_player.Stop();
			}
		}
		m_RS_counter_clockwise = new RasterizerState();
		m_RS_clockwise = new RasterizerState();
		m_RS_clockwise.CullMode = CullMode.CullClockwiseFace;
		m_RS_none = new RasterizerState();
		m_RS_none.CullMode = CullMode.None;
		m_effect = effect;
		m_pos = MC.m_pos;
		m_rot_matrix = MC.m_rot_matrix;
		m_light_energy = MC.m_light_energy;
		m_textures = MC.m_textures;
		m_normalmaps = MC.m_normalmaps;
		m_specularmaps = MC.m_specularmaps;
		m_mesh_data = MC.m_mesh_data;
		m_update_transform = true;
	}

	public virtual void Clear()
	{
		if (m_RS_counter_clockwise != null)
		{
			m_RS_counter_clockwise.Dispose();
			m_RS_counter_clockwise = null;
		}
		if (m_RS_clockwise != null)
		{
			m_RS_clockwise.Dispose();
			m_RS_clockwise = null;
		}
		if (m_RS_none != null)
		{
			m_RS_none.Dispose();
			m_RS_none = null;
		}
		m_model = null;
		m_effect = null;
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
		m_world_matrix = Matrix.Identity;
		m_bones = null;
		m_animation_player = null;
		m_skinningData = null;
		m_animation_clips = null;
		m_attached_to = -1;
		foreach (SGSModel attachment in m_attachments)
		{
			attachment.Clear();
		}
		m_attachments.Clear();
	}

	public Vector3 GetPosition()
	{
		return m_pos;
	}

	public void SetPosition(Vector3 pos)
	{
		m_pos = pos;
		m_update_transform = true;
	}

	public void MoveX(float delta)
	{
		m_pos.X += delta;
		m_update_transform = true;
	}

	public void MoveY(float delta)
	{
		m_pos.Y += delta;
		m_update_transform = true;
	}

	public void MoveZ(float delta)
	{
		m_pos.Z += delta;
		m_update_transform = true;
	}

	public void RotateX(float delta)
	{
		m_rot_delta.X += delta;
		m_update_transform = true;
	}

	public void RotateY(float delta)
	{
		m_rot_delta.Y += delta;
		m_update_transform = true;
	}

	public void RotateZ(float delta)
	{
		m_rot_delta.Z += delta;
		m_update_transform = true;
	}

	public void UpdateTransform()
	{
		m_update_transform = true;
	}

	public void Load(ContentManager CM, string path, SGSXML modeldata, Effect effect)
	{
		Clear();
		if (CM == null || modeldata == null)
		{
			return;
		}
		m_RS_counter_clockwise = new RasterizerState();
		m_RS_clockwise = new RasterizerState();
		m_RS_clockwise.CullMode = CullMode.CullClockwiseFace;
		m_RS_none = new RasterizerState();
		m_RS_none.CullMode = CullMode.None;
		m_effect = effect;
		SGSXMLData data = modeldata.GetData("Model");
		if (data == null)
		{
			return;
		}
		SetModel(CM.Load<Model>(path + (string)data.GetField(0)));
		m_pos = (Vector3)data.GetField(1);
		m_rot_matrix = (Matrix)data.GetField(2);
		if (data.GetField(4) != null)
		{
			m_light_energy = (float)data.GetField(4);
		}
		data = null;
		data = modeldata.GetData("Textures");
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
		data = modeldata.GetData("Normalmaps");
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
		data = modeldata.GetData("Specularmaps");
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
		data = modeldata.GetData("Meshes");
		if (data == null)
		{
			return;
		}
		int num = (int)data.GetField(0);
		data = null;
		for (int l = 0; l < num; l++)
		{
			data = modeldata.GetData("MeshData" + l);
			if (data != null)
			{
				SGSMeshData value = m_mesh_data[l];
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
				m_mesh_data[l] = value;
			}
		}
		m_update_transform = true;
	}

	public void SetModel(Model model)
	{
		m_model = model;
		m_bones = new Matrix[m_model.Bones.Count];
		m_model.CopyAbsoluteBoneTransformsTo(m_bones);
		if (m_RS_counter_clockwise == null)
		{
			m_RS_counter_clockwise = new RasterizerState();
		}
		if (m_RS_clockwise == null)
		{
			m_RS_clockwise = new RasterizerState();
			m_RS_clockwise.CullMode = CullMode.CullClockwiseFace;
		}
		if (m_RS_none == null)
		{
			m_RS_none = new RasterizerState();
			m_RS_none.CullMode = CullMode.None;
		}
		if (m_model != null)
		{
			if (m_mesh_data == null)
			{
				m_mesh_data = new List<SGSMeshData>();
			}
			for (int i = 0; i < m_model.Meshes.Count; i++)
			{
				m_mesh_data.Add(new SGSMeshData(reset: true));
			}
		}
		m_skinningData = m_model.Tag as SkinningData;
		if (m_skinningData != null)
		{
			m_animation_player = new AnimationPlayer(m_skinningData);
			m_animation_clips = new string[m_skinningData.AnimationClips.Keys.Count];
			for (int j = 0; j < m_skinningData.AnimationClips.Keys.Count; j++)
			{
				m_animation_clips[j] = m_skinningData.AnimationClips.Keys.ElementAt(j);
			}
			if (m_animation_clips.Length > 0)
			{
				m_animation_player.StartClip(m_skinningData.AnimationClips[m_animation_clips[0]]);
				m_animation_player.Stop();
			}
		}
	}

	public void SetTexture(int mesh, Texture2D texture)
	{
		if (mesh >= 0 && mesh < m_mesh_data.Count)
		{
			SGSMeshData value = m_mesh_data[mesh];
			value.m_texture = texture;
			m_mesh_data[mesh] = value;
		}
	}

	public void SetNormalmap(int mesh, Texture2D normalmap)
	{
		if (mesh >= 0 && mesh < m_mesh_data.Count)
		{
			SGSMeshData value = m_mesh_data[mesh];
			value.m_normalmap = normalmap;
			m_mesh_data[mesh] = value;
		}
	}

	public void SetSpecularmap(int mesh, Texture2D specularmap)
	{
		if (mesh >= 0 && mesh < m_mesh_data.Count)
		{
			SGSMeshData value = m_mesh_data[mesh];
			value.m_specularmap = specularmap;
			m_mesh_data[mesh] = value;
		}
	}

	public SGSModel AddAttachment(int bone)
	{
		SGSModel sGSModel = new SGSModel();
		sGSModel.m_attached_to = bone;
		if (sGSModel.m_attached_to < 0)
		{
			sGSModel.m_attached_to = 0;
		}
		m_attachments.Add(sGSModel);
		return sGSModel;
	}

	public void PlayAnimation(string anim)
	{
		PlayAnimation(anim, loop: false);
	}

	public void PlayAnimation(string anim, bool loop)
	{
		if (m_skinningData.AnimationClips[anim] != null)
		{
			m_animation_player.m_loop = loop;
			m_animation_player.StartClip(m_skinningData.AnimationClips[anim]);
		}
	}

	public void StopAnimation()
	{
		if (m_animation_player != null)
		{
			m_animation_player.Stop();
		}
	}

	private void DrawModel(GraphicsDevice device, SGSCamera camera)
	{
		Matrix[] array = null;
		if (m_animation_player != null)
		{
			m_bones = m_animation_player.GetWorldTransforms();
			array = m_animation_player.GetSkinTransforms();
		}
		foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
		{
			_ = pass;
			for (int num = m_model.Meshes.Count - 1; num >= 0; num--)
			{
				ModelMesh modelMesh = m_model.Meshes[num];
				if (m_skinningData == null)
				{
					if (m_mesh_data[num].m_normalmap == null && m_mesh_data[num].m_specularmap == null)
					{
						m_effect.CurrentTechnique = m_effect.Techniques["TextureMapping"];
					}
					else if (m_mesh_data[num].m_normalmap != null && m_mesh_data[num].m_specularmap == null)
					{
						m_effect.CurrentTechnique = m_effect.Techniques["NormalMapping"];
					}
					else if (m_mesh_data[num].m_normalmap != null && m_mesh_data[num].m_specularmap != null)
					{
						m_effect.CurrentTechnique = m_effect.Techniques["NormalSpecularMapping"];
					}
				}
				else if (m_mesh_data[num].m_normalmap == null && m_mesh_data[num].m_specularmap == null)
				{
					m_effect.CurrentTechnique = m_effect.Techniques["SkinTextureMapping"];
				}
				else if (m_mesh_data[num].m_normalmap != null && m_mesh_data[num].m_specularmap == null)
				{
					m_effect.CurrentTechnique = m_effect.Techniques["SkinNormalMapping"];
				}
				else if (m_mesh_data[num].m_normalmap != null && m_mesh_data[num].m_specularmap != null)
				{
					m_effect.CurrentTechnique = m_effect.Techniques["SkinNormalSpecularMapping"];
				}
				foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
				{
					Vector4 value = new Vector4(camera.m_pos.X, camera.m_pos.Y, camera.m_pos.Z, 0f);
					Vector4 light_dir = m_light_dir;
					light_dir = -light_dir;
					meshPart.Effect = m_effect;
					if (array != null)
					{
						meshPart.Effect.Parameters["Bones"].SetValue(array);
					}
					if (m_mesh_data[num].m_normalmap == null)
					{
						Matrix matrix = Matrix.Invert(m_rot_matrix);
						light_dir = Vector4.Transform(light_dir, matrix);
					}
					light_dir.Normalize();
					meshPart.Effect.Parameters["matWorldViewProj"].SetValue(m_world_matrix * camera.m_view_matrix * camera.m_proj_matrix);
					meshPart.Effect.Parameters["matWorld"].SetValue(m_world_matrix);
					meshPart.Effect.Parameters["vecEye"].SetValue(value);
					meshPart.Effect.Parameters["vecLightDir"].SetValue(light_dir);
					meshPart.Effect.Parameters["Texture"].SetValue(m_mesh_data[num].m_texture);
					meshPart.Effect.Parameters["NormalMap"].SetValue(m_mesh_data[num].m_normalmap);
					meshPart.Effect.Parameters["SpecularMap"].SetValue(m_mesh_data[num].m_specularmap);
					meshPart.Effect.Parameters["specularColor"].SetValue(m_mesh_data[num].m_specular_color);
					meshPart.Effect.Parameters["specularPower"].SetValue(m_mesh_data[num].m_specular_power);
					meshPart.Effect.Parameters["lightEnergy"].SetValue(m_light_energy);
				}
				if (m_mesh_data[num].m_cullmode == CullMode.CullCounterClockwiseFace)
				{
					device.RasterizerState = m_RS_counter_clockwise;
				}
				else if (m_mesh_data[num].m_cullmode == CullMode.CullClockwiseFace)
				{
					device.RasterizerState = m_RS_clockwise;
				}
				else if (m_mesh_data[num].m_cullmode == CullMode.None)
				{
					device.RasterizerState = m_RS_none;
				}
				modelMesh.Draw();
				device.RasterizerState = RasterizerState.CullCounterClockwise;
				modelMesh = null;
			}
		}
	}

	public void Update(TimeSpan time)
	{
		if (m_update_transform)
		{
			m_update_transform = false;
			Matrix matrix = Matrix.CreateRotationZ(m_rot_delta.Z) * Matrix.CreateRotationY(m_rot_delta.Y) * Matrix.CreateRotationX(m_rot_delta.X);
			m_rot_matrix *= matrix;
			m_world_matrix = m_rot_matrix * Matrix.CreateTranslation(m_pos);
			m_rot_delta.X = 0f;
			m_rot_delta.Y = 0f;
			m_rot_delta.Z = 0f;
		}
		if (m_animation_player != null)
		{
			m_animation_player.Update(time, relativeToCurrentTime: true, Matrix.Identity);
		}
		foreach (SGSModel attachment in m_attachments)
		{
			attachment.Update(time);
		}
	}

	public void Draw(GraphicsDevice device, SGSCamera camera)
	{
		if (m_model == null)
		{
			return;
		}
		DrawModel(device, camera);
		foreach (SGSModel attachment in m_attachments)
		{
			if (attachment.m_attached_to > 0 && attachment.m_attached_to <= m_bones.Length)
			{
				attachment.m_world_matrix = m_bones[attachment.m_attached_to - 1] * m_world_matrix;
				attachment.Draw(device, camera);
			}
		}
	}
}
