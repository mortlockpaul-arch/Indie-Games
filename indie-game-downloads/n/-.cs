using Microsoft.Xna.Framework;
using N;

namespace n
{
	internal class _0006
	{
		internal _6 a5h;

		public Vector3 Position
		{
			get
			{
				if (_6j())
				{
					return a5h.BufferedStatesManager.InterpolatedStates.GetState(a5h.a5h).Position;
				}
				return a5h.Entity.Position;
			}
		}

		public Quaternion Orientation
		{
			get
			{
				if (_6j())
				{
					return a5h.BufferedStatesManager.InterpolatedStates.GetState(a5h.a5h).Orientation;
				}
				return a5h.Entity.Orientation;
			}
		}

		public N._7 OrientationMatrix
		{
			get
			{
				N._7 result;
				if (_6j())
				{
					Quaternion quaternion = a5h.BufferedStatesManager.InterpolatedStates.GetState(a5h.a5h).Orientation;
					N._7.CreateFromQuaternion(ref quaternion, out result);
				}
				else
				{
					N._7.CreateFromQuaternion(ref a5h.Entity.a5b, out result);
				}
				return result;
			}
		}

		public Matrix WorldTransform
		{
			get
			{
				if (_6j())
				{
					return a5h.BufferedStatesManager.InterpolatedStates.GetState(a5h.a5h).Matrix;
				}
				return a5h.Entity.WorldTransform;
			}
		}

		public N._0006 RigidTransform
		{
			get
			{
				if (_6j())
				{
					return a5h.BufferedStatesManager.InterpolatedStates.GetState(a5h.a5h);
				}
				return new N._0006
				{
					Position = a5h.Entity.a5h,
					Orientation = a5h.Entity.a5b
				};
			}
		}

		public _0006(_6 bufferedStates)
		{
			a5h = bufferedStates;
		}

		private bool _6j()
		{
			if (a5h.BufferedStatesManager != null && a5h.BufferedStatesManager.Enabled)
			{
				return a5h.BufferedStatesManager.InterpolatedStates.Enabled;
			}
			return false;
		}
	}
}
namespace N
{
	internal struct _0006
	{
		public Vector3 Position;

		public Quaternion Orientation;

		public Matrix OrientationMatrix
		{
			get
			{
				Matrix.CreateFromQuaternion(ref Orientation, out var result);
				return result;
			}
		}

		public Matrix Matrix
		{
			get
			{
				Matrix.CreateFromQuaternion(ref Orientation, out var result);
				result.Translation = Position;
				return result;
			}
		}

		public static _0006 Identity => new _0006
		{
			Orientation = Quaternion.Identity,
			Position = default(Vector3)
		};

		public _0006(Vector3 position, Quaternion orienation)
		{
			Position = position;
			Orientation = orienation;
		}

		public _0006(Vector3 position)
		{
			Position = position;
			Orientation = Quaternion.Identity;
		}

		public _0006(Quaternion orienation)
		{
			Position = default(Vector3);
			Orientation = orienation;
		}

		public static void Invert(ref _0006 transform, out _0006 inverse)
		{
			Quaternion.Conjugate(ref transform.Orientation, out inverse.Orientation);
			Vector3.Transform(ref transform.Position, ref inverse.Orientation, out inverse.Position);
			Vector3.Negate(ref inverse.Position, out inverse.Position);
		}

		public static void Transform(ref _0006 a, ref _0006 b, out _0006 combined)
		{
			Vector3.Transform(ref a.Position, ref b.Orientation, out var result);
			Vector3.Add(ref result, ref b.Position, out combined.Position);
			Quaternion.Concatenate(ref a.Orientation, ref b.Orientation, out combined.Orientation);
		}

		public static void TransformByInverse(ref _0006 a, ref _0006 b, out _0006 combinedTransform)
		{
			Invert(ref b, out combinedTransform);
			Transform(ref a, ref combinedTransform, out combinedTransform);
		}

		public static void Transform(ref Vector3 position, ref _0006 transform, out Vector3 result)
		{
			Vector3.Transform(ref position, ref transform.Orientation, out var result2);
			Vector3.Add(ref result2, ref transform.Position, out result);
		}

		public static void TransformByInverse(ref Vector3 position, ref _0006 transform, out Vector3 result)
		{
			Vector3.Subtract(ref position, ref transform.Position, out var result2);
			Quaternion.Conjugate(ref transform.Orientation, out var result3);
			Vector3.Transform(ref result2, ref result3, out result);
		}
	}
}
