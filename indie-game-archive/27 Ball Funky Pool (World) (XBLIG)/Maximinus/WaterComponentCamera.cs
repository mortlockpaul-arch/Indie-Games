using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public sealed class WaterComponentCamera
{
	private Matrix mView;

	private Matrix mProj;

	private Matrix mViewProj;

	private float mFieldOfView;

	private float mAspect;

	private float mNearZ;

	private float mFarZ;

	private Vector3 mPosition;

	private Vector3 mRight;

	private Vector3 mUp;

	private Vector3 mLook;

	private Plane[] mFrustumPlanes;

	private BoundingFrustum mBoundingFrustum;

	private float mDeltaX;

	private float mDeltaY;

	private float mLastMouseX;

	private float mLastMouseY;

	public Matrix View => mView;

	public Matrix Projection
	{
		get
		{
			return mProj;
		}
		set
		{
			mProj = value;
		}
	}

	public Matrix ViewProj => mViewProj;

	public float FOV => mFieldOfView;

	public float Aspect => mAspect;

	public float NearZ => mNearZ;

	public float FarZ => mFarZ;

	public Vector3 Position
	{
		get
		{
			return mPosition;
		}
		set
		{
			mPosition = value;
		}
	}

	public Vector3 Look
	{
		get
		{
			return mLook;
		}
		set
		{
			mLook = value;
		}
	}

	public Vector3 Right
	{
		get
		{
			return mRight;
		}
		set
		{
			mRight = value;
		}
	}

	public Vector3 Up
	{
		get
		{
			return mUp;
		}
		set
		{
			mUp = value;
		}
	}

	public WaterComponentCamera()
	{
		mView = Matrix.Identity;
		mProj = Matrix.Identity;
		mViewProj = Matrix.Identity;
		mFieldOfView = 0f;
		mAspect = 0f;
		mNearZ = 0f;
		mFarZ = 0f;
		mPosition = new Vector3(0f, 0f, 0f);
		mRight = new Vector3(1f, 0f, 0f);
		mUp = new Vector3(0f, 1f, 0f);
		mLook = new Vector3(0f, 0f, 1f);
		mFrustumPlanes = new Plane[6];
		mDeltaX = 0f;
		mDeltaY = 0f;
		mLastMouseX = 0f;
		mLastMouseY = 0f;
	}

	public WaterComponentCamera(WaterComponentCamera camera)
	{
		mView = camera.View;
		mProj = camera.Projection;
		mViewProj = camera.ViewProj;
		mFieldOfView = camera.FOV;
		mAspect = camera.Aspect;
		mNearZ = camera.NearZ;
		mFarZ = camera.FarZ;
		mPosition = camera.Position;
		mRight = camera.Right;
		mUp = camera.Up;
		mLook = camera.Look;
	}

	public void LookAt(Vector3 pos, Vector3 target, Vector3 up)
	{
		Vector3 value = pos - target;
		value = Vector3.Normalize(value);
		Vector3 vector = Vector3.Cross(up, value);
		Vector3 vector2 = Vector3.Cross(value, vector);
		mPosition = pos;
		mRight = vector;
		mUp = vector2;
		mLook = value;
	}

	public void LookAt(Vector3 pos, Vector3 target)
	{
		Vector3 value = pos - target;
		value = Vector3.Normalize(value);
		Vector3 vector;
		Vector3 vector2;
		if (Math.Abs(Vector3.Dot(mUp, value)) < 0.5f)
		{
			vector = Vector3.Cross(mUp, value);
			vector2 = Vector3.Cross(value, vector);
		}
		else
		{
			vector2 = Vector3.Cross(value, mRight);
			vector = Vector3.Cross(vector2, value);
		}
		mPosition = pos;
		mRight = vector;
		mUp = vector2;
		mLook = value;
	}

	public void SetLens(float fov, float aspect, float nearZ, float farZ)
	{
		mFieldOfView = fov;
		mAspect = aspect;
		mNearZ = nearZ;
		mFarZ = farZ;
		mProj = Matrix.CreatePerspectiveFieldOfView(fov, aspect, nearZ, farZ);
	}

	public void Place(Vector3 pos, Vector3 look, Vector3 up)
	{
		mLook.Normalize();
		Vector3 vector = up;
		vector.Normalize();
		Vector3 value = Vector3.Cross(vector, look);
		value.Normalize();
		value = Vector3.Multiply(value, -1f);
		mPosition = pos;
		mRight = value;
		mUp = vector;
		mLook = look;
	}

	public void Walk(float units)
	{
		mPosition += mLook * units;
	}

	public void Fly(float units)
	{
		mPosition += mUp * units;
	}

	public void Strafe(float units)
	{
		mPosition += new Vector3(mRight.X, 0f, mRight.Z) * units;
	}

	public void Pitch(float angle)
	{
		Matrix matrix = Matrix.CreateFromAxisAngle(mRight, angle);
		mUp = Vector3.Transform(mUp, matrix);
		mLook = Vector3.Transform(mLook, matrix);
	}

	public void Yaw(float angle)
	{
		Matrix matrix = Matrix.CreateRotationY(angle);
		mRight = Vector3.Transform(mRight, matrix);
		mLook = Vector3.Transform(mLook, matrix);
	}

	public void UpdateMouse(MouseState state, float units)
	{
		mDeltaX = mLastMouseX - state.X;
		mDeltaY = mLastMouseY - state.Y;
		mLastMouseX = state.X;
		mLastMouseY = state.Y;
		Yaw(mDeltaX * units);
		Pitch(mDeltaY * units);
	}

	public void BuildView()
	{
		mLook.Normalize();
		mUp = Vector3.Cross(mLook, mRight);
		mUp.Normalize();
		mRight = Vector3.Cross(mUp, mLook);
		mRight.Normalize();
		float m = 0f - Vector3.Dot(mRight, mPosition);
		float m2 = 0f - Vector3.Dot(mUp, mPosition);
		float m3 = 0f - Vector3.Dot(mLook, mPosition);
		mView.M11 = mRight.X;
		mView.M21 = mRight.Y;
		mView.M31 = mRight.Z;
		mView.M41 = m;
		mView.M12 = mUp.X;
		mView.M22 = mUp.Y;
		mView.M32 = mUp.Z;
		mView.M42 = m2;
		mView.M13 = mLook.X;
		mView.M23 = mLook.Y;
		mView.M33 = mLook.Z;
		mView.M43 = m3;
		mView.M14 = 0f;
		mView.M24 = 0f;
		mView.M34 = 0f;
		mView.M44 = 1f;
		mViewProj = mView * mProj;
		mBoundingFrustum = new BoundingFrustum(mViewProj);
	}
}
