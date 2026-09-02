using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSCamera
{
	public Vector3 m_pos;

	public Vector3 m_rotation;

	public Matrix m_rot_matrix;

	public Matrix m_trans_matrix;

	public Matrix m_view_matrix;

	public Matrix m_proj_matrix;

	public float m_aspect_ratio;

	public float m_fov;

	public bool m_update;

	public SGSCamera(GraphicsDevice device, Vector3 pos, float near, float far)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_pos = pos;
		m_view_matrix = Matrix.Identity;
		m_proj_matrix = Matrix.Identity;
		m_rot_matrix = Matrix.Identity;
		Viewport viewport = device.Viewport;
		m_aspect_ratio = ((Viewport)(ref viewport)).AspectRatio;
		Viewport viewport2 = device.Viewport;
		Matrix.CreatePerspective(((Viewport)(ref viewport2)).AspectRatio, 1f, near, far, ref m_proj_matrix);
	}

	public void LookAt(Vector3 pos, Vector3 up)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		m_view_matrix = Matrix.CreateLookAt(m_pos, pos, up);
		m_pos = ((Matrix)(ref m_view_matrix)).Translation;
	}

	public void CreateYawPitchRoll(float yaw, float pitch, float roll)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		m_rot_matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
		m_view_matrix = Matrix.Identity;
		m_view_matrix *= m_rot_matrix;
		((Matrix)(ref m_view_matrix)).Translation = m_pos;
	}

	public void MoveLeft(float move)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 pos = ref m_pos;
		pos.X -= move * ((Matrix)(ref m_rot_matrix)).Forward.Z;
		ref Vector3 pos2 = ref m_pos;
		pos2.Z -= move * ((Matrix)(ref m_rot_matrix)).Forward.X;
		m_update = true;
	}

	public void MoveRight(float move)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 pos = ref m_pos;
		pos.X += move * ((Matrix)(ref m_rot_matrix)).Forward.Z;
		ref Vector3 pos2 = ref m_pos;
		pos2.Z += move * ((Matrix)(ref m_rot_matrix)).Forward.X;
		m_update = true;
	}

	public void MoveForward(float move)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 pos = ref m_pos;
		pos.X += move * ((Matrix)(ref m_rot_matrix)).Right.Z;
		ref Vector3 pos2 = ref m_pos;
		pos2.Z += move * ((Matrix)(ref m_rot_matrix)).Right.X;
		ref Vector3 pos3 = ref m_pos;
		pos3.Y += move * ((Matrix)(ref m_rot_matrix)).Up.Z;
		m_update = true;
	}

	public void MoveBackward(float move)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 pos = ref m_pos;
		pos.X -= move * ((Matrix)(ref m_rot_matrix)).Right.Z;
		ref Vector3 pos2 = ref m_pos;
		pos2.Z -= move * ((Matrix)(ref m_rot_matrix)).Right.X;
		ref Vector3 pos3 = ref m_pos;
		pos3.Y -= move * ((Matrix)(ref m_rot_matrix)).Up.Z;
		m_update = true;
	}

	public void MoveUp(float move)
	{
		ref Vector3 pos = ref m_pos;
		pos.Y -= move;
		m_update = true;
	}

	public void MoveDown(float move)
	{
		ref Vector3 pos = ref m_pos;
		pos.Y += move;
		m_update = true;
	}

	public void RotateLeft(float rotate)
	{
		ref Vector3 rotation = ref m_rotation;
		rotation.Y -= rotate;
		m_update = true;
	}

	public void RotateRight(float rotate)
	{
		ref Vector3 rotation = ref m_rotation;
		rotation.Y += rotate;
		m_update = true;
	}

	public void TiltForward(float rotate)
	{
		ref Vector3 rotation = ref m_rotation;
		rotation.X += rotate;
		m_update = true;
	}

	public void TiltBackward(float rotate)
	{
		ref Vector3 rotation = ref m_rotation;
		rotation.X -= rotate;
		m_update = true;
	}

	public void Update(TimeSpan elapsed)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (m_update)
		{
			m_view_matrix = Matrix.Identity;
			m_trans_matrix = Matrix.CreateTranslation(m_pos);
			m_rot_matrix = Matrix.CreateRotationY(m_rotation.Y);
			m_rot_matrix *= Matrix.CreateRotationX(m_rotation.X);
			m_view_matrix = m_trans_matrix * m_rot_matrix;
			m_update = false;
		}
	}
}
