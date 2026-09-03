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
		m_pos = pos;
		m_view_matrix = Matrix.Identity;
		m_proj_matrix = Matrix.Identity;
		m_rot_matrix = Matrix.Identity;
		m_aspect_ratio = device.Viewport.AspectRatio;
		Matrix.CreatePerspective(device.Viewport.AspectRatio, 1f, near, far, out m_proj_matrix);
	}

	public void LookAt(Vector3 pos, Vector3 up)
	{
		m_view_matrix = Matrix.CreateLookAt(m_pos, pos, up);
		m_pos = m_view_matrix.Translation;
	}

	public void CreateYawPitchRoll(float yaw, float pitch, float roll)
	{
		m_rot_matrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
		m_view_matrix = Matrix.Identity;
		m_view_matrix *= m_rot_matrix;
		m_view_matrix.Translation = m_pos;
	}

	public void MoveLeft(float move)
	{
		m_pos.X -= move * m_rot_matrix.Forward.Z;
		m_pos.Z -= move * m_rot_matrix.Forward.X;
		m_update = true;
	}

	public void MoveRight(float move)
	{
		m_pos.X += move * m_rot_matrix.Forward.Z;
		m_pos.Z += move * m_rot_matrix.Forward.X;
		m_update = true;
	}

	public void MoveForward(float move)
	{
		m_pos.X += move * m_rot_matrix.Right.Z;
		m_pos.Z += move * m_rot_matrix.Right.X;
		m_pos.Y += move * m_rot_matrix.Up.Z;
		m_update = true;
	}

	public void MoveBackward(float move)
	{
		m_pos.X -= move * m_rot_matrix.Right.Z;
		m_pos.Z -= move * m_rot_matrix.Right.X;
		m_pos.Y -= move * m_rot_matrix.Up.Z;
		m_update = true;
	}

	public void MoveUp(float move)
	{
		m_pos.Y -= move;
		m_update = true;
	}

	public void MoveDown(float move)
	{
		m_pos.Y += move;
		m_update = true;
	}

	public void RotateLeft(float rotate)
	{
		m_rotation.Y -= rotate;
		m_update = true;
	}

	public void RotateRight(float rotate)
	{
		m_rotation.Y += rotate;
		m_update = true;
	}

	public void TiltForward(float rotate)
	{
		m_rotation.X += rotate;
		m_update = true;
	}

	public void TiltBackward(float rotate)
	{
		m_rotation.X -= rotate;
		m_update = true;
	}

	public void Update(TimeSpan elapsed)
	{
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
