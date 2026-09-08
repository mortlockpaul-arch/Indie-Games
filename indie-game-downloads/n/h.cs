using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using N;

namespace n
{
	internal class h
	{
		internal _6 a5h;

		[CompilerGenerated]
		private _7 a5b;

		public _7 WriteBuffer
		{
			[CompilerGenerated]
			get
			{
				return a5b;
			}
			[CompilerGenerated]
			set
			{
				a5b = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).Position;
				}
				return a5h.Entity.Position;
			}
			set
			{
				if (_6_0014())
				{
					WriteBuffer.EnqueuePosition(a5h.Entity, ref value);
				}
				else
				{
					a5h.Entity.Position = value;
				}
			}
		}

		public Quaternion Orientation
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).Orientation;
				}
				return a5h.Entity.Orientation;
			}
			set
			{
				if (_6_0014())
				{
					WriteBuffer.EnqueueOrientation(a5h.Entity, ref value);
				}
				else
				{
					a5h.Entity.Orientation = value;
				}
			}
		}

		public N._7 OrientationMatrix
		{
			get
			{
				N._7 result;
				if (_6p())
				{
					Quaternion quaternion = a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).Orientation;
					N._7.CreateFromQuaternion(ref quaternion, out result);
				}
				else
				{
					N._7.CreateFromQuaternion(ref a5h.Entity.a5b, out result);
				}
				return result;
			}
			set
			{
				if (_6_0014())
				{
					Quaternion newOrientationQuaternion = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(N._7.ToMatrix4X4(value)));
					WriteBuffer.EnqueueOrientation(a5h.Entity, ref newOrientationQuaternion);
				}
				else
				{
					a5h.Entity.OrientationMatrix = value;
				}
			}
		}

		public Vector3 LinearVelocity
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).LinearVelocity;
				}
				return a5h.Entity.LinearVelocity;
			}
			set
			{
				if (_6_0014())
				{
					WriteBuffer.EnqueueLinearVelocity(a5h.Entity, ref value);
				}
				else
				{
					a5h.Entity.LinearVelocity = value;
				}
			}
		}

		public Vector3 AngularVelocity
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).AngularVelocity;
				}
				return a5h.Entity.AngularVelocity;
			}
			set
			{
				if (_6_0014())
				{
					WriteBuffer.EnqueueAngularVelocity(a5h.Entity, ref value);
				}
				else
				{
					a5h.Entity.AngularVelocity = value;
				}
			}
		}

		public Matrix WorldTransform
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h).WorldTransform;
				}
				return a5h.Entity.WorldTransform;
			}
			set
			{
				if (_6_0014())
				{
					Vector3 newPosition = value.Translation;
					Quaternion.CreateFromRotationMatrix(ref value, out var result);
					result.Normalize();
					WriteBuffer.EnqueueOrientation(a5h.Entity, ref result);
					WriteBuffer.EnqueuePosition(a5h.Entity, ref newPosition);
				}
				else
				{
					a5h.Entity.WorldTransform = value;
				}
			}
		}

		public B MotionState
		{
			get
			{
				if (_6p())
				{
					return a5h.BufferedStatesManager.ReadBuffers.GetState(a5h.a5h);
				}
				return a5h.Entity.MotionState;
			}
			set
			{
				if (_6_0014())
				{
					WriteBuffer.EnqueueLinearVelocity(a5h.Entity, ref value.LinearVelocity);
					WriteBuffer.EnqueueAngularVelocity(a5h.Entity, ref value.AngularVelocity);
					WriteBuffer.EnqueueOrientation(a5h.Entity, ref value.Orientation);
					WriteBuffer.EnqueuePosition(a5h.Entity, ref value.Position);
				}
				else
				{
					a5h.Entity.MotionState = value;
				}
			}
		}

		public h(_6 bufferedStates)
		{
			a5h = bufferedStates;
		}

		private bool _6p()
		{
			if (a5h.BufferedStatesManager != null && a5h.BufferedStatesManager.Enabled)
			{
				return a5h.BufferedStatesManager.ReadBuffers.Enabled;
			}
			return false;
		}

		private bool _6_0014()
		{
			if (WriteBuffer != null)
			{
				return WriteBuffer.Enabled;
			}
			return false;
		}
	}
}
namespace N
{
	internal struct h
	{
		public Vector3 Translation;

		public _7 LinearTransform;

		public Matrix Matrix
		{
			get
			{
				_7.ToMatrix4X4(ref LinearTransform, out var result);
				result.Translation = Translation;
				return result;
			}
			set
			{
				_7.CreateFromMatrix(ref value, out LinearTransform);
				Translation = value.Translation;
			}
		}

		public static h Identity => new h
		{
			LinearTransform = _7.Identity,
			Translation = default(Vector3)
		};

		public h(Vector3 translation)
		{
			LinearTransform = _7.Identity;
			Translation = translation;
		}

		public h(Quaternion orientation, Vector3 translation)
		{
			_7.CreateFromQuaternion(ref orientation, out LinearTransform);
			Translation = translation;
		}

		public h(Vector3 scaling, Quaternion orientation, Vector3 translation)
		{
			_7.CreateScale(ref scaling, out LinearTransform);
			_7.CreateFromQuaternion(ref orientation, out var result);
			_7.Multiply(ref LinearTransform, ref result, out LinearTransform);
			Translation = translation;
		}

		public h(_7 linearTransform, Vector3 translation)
		{
			LinearTransform = linearTransform;
			Translation = translation;
		}

		public static void Transform(ref Vector3 position, ref h transform, out Vector3 transformed)
		{
			_7.Transform(ref position, ref transform.LinearTransform, out transformed);
			Vector3.Add(ref transformed, ref transform.Translation, out transformed);
		}

		public static void TransformInverse(ref Vector3 position, ref h transform, out Vector3 transformed)
		{
			Vector3.Subtract(ref position, ref transform.Translation, out transformed);
			_7.Invert(ref transform.LinearTransform, out var result);
			_7.TransformTranspose(ref transformed, ref result, out transformed);
		}

		public static void Invert(ref h transform, out h inverse)
		{
			_7.Invert(ref transform.LinearTransform, out inverse.LinearTransform);
			_7.Transform(ref transform.Translation, ref inverse.LinearTransform, out inverse.Translation);
			Vector3.Negate(ref inverse.Translation, out inverse.Translation);
		}

		public static void Multiply(ref h a, ref h b, out h transform)
		{
			_7.Multiply(ref a.LinearTransform, ref b.LinearTransform, out var result);
			_7.Transform(ref a.Translation, ref b.LinearTransform, out var result2);
			Vector3.Add(ref result2, ref b.Translation, out transform.Translation);
			transform.LinearTransform = result;
		}

		public static void Multiply(ref _0006 a, ref h b, out h transform)
		{
			_7.CreateFromQuaternion(ref a.Orientation, out var result);
			_7.Multiply(ref result, ref b.LinearTransform, out result);
			_7.Transform(ref a.Position, ref b.LinearTransform, out var result2);
			Vector3.Add(ref result2, ref b.Translation, out transform.Translation);
			transform.LinearTransform = result;
		}

		public static Vector3 Transform(Vector3 position, h affineTransform)
		{
			Transform(ref position, ref affineTransform, out var transformed);
			return transformed;
		}

		public static void CreateFromRigidTransform(ref _0006 rigid, out h affine)
		{
			affine.Translation = rigid.Position;
			_7.CreateFromQuaternion(ref rigid.Orientation, out affine.LinearTransform);
		}

		public static h CreateFromRigidTransform(_0006 rigid)
		{
			h result = default(h);
			result.Translation = rigid.Position;
			_7.CreateFromQuaternion(ref rigid.Orientation, out result.LinearTransform);
			return result;
		}
	}
}
