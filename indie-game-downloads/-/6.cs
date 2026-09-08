using System;
using _7;
using D;
using E;
using G;
using L;
using Microsoft.Xna.Framework;
using N;
using P;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Collision.Legacy;
using SynapseGaming.LightingSystem.Core;
using l;
using m;
using n;
using r;
using s;
using v;
using y;

namespace _0006
{
	internal class _6 : v.b
	{
		public delegate void _00065h(object sender, EventArgs e);

		private const int a5h = 1024;

		private bool a5b;

		private bool a56 = true;

		private bool a5a;

		private bool a57;

		private _7.h a5_0006;

		private _7.h a5v;

		private _7.h a5B;

		private _7.h a5X;

		private _7.h a5_0018;

		private _7.h a5W;

		private _7.h a5_0002;

		private _7.h a5_000E;

		public override int KeySize
		{
			get
			{
				if (a5a)
				{
					int num = a5_0002.BitCount();
					if ((num & 7) != 0)
					{
						num += 8 - (num & 7);
					}
					return num;
				}
				return base.KeySize;
			}
		}

		public override string KeyExchangeAlgorithm => "RSA-PKCS1-KeyEx";

		public bool PublicOnly
		{
			get
			{
				if (a5a)
				{
					if (!(a5_0006 == null))
					{
						return a5_0002 == null;
					}
					return true;
				}
				return false;
			}
		}

		public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		internal bool UseKeyBlinding
		{
			get
			{
				return a56;
			}
			set
			{
				a56 = flag;
			}
		}

		internal bool IsCrtPossible
		{
			get
			{
				if (a5a)
				{
					return a5b;
				}
				return true;
			}
		}

		public _6()
			: this(1024)
		{
		}

		public _6(int keySize)
		{
			LegalKeySizesValue = new v._0018[1];
			LegalKeySizesValue[0] = new v._0018(384, 16384, 8);
			base.KeySize = keySize;
		}

		~_6()
		{
			Dispose(disposing: false);
		}

		public override byte[] DecryptValue(byte[] rgb)
		{
			if (a57)
			{
				throw new ObjectDisposedException("private key");
			}
			if (!a5a)
			{
				throw new Exception("not supported");
			}
			_7.h h2 = new _7.h(rgb);
			_7.h h3 = null;
			if (a56)
			{
				h3 = _7.h.GenerateRandom(a5_0002.BitCount());
				h2 = h3.ModPow(a5_000E, a5_0002) * h2 % a5_0002;
			}
			_7.h h7;
			if (a5b)
			{
				_7.h h4 = h2.ModPow(a5X, a5v);
				_7.h h5 = h2.ModPow(a5_0018, a5B);
				if (h5 > h4)
				{
					_7.h h6 = a5v - (h5 - h4) * a5W % a5v;
					h7 = h5 + a5B * h6;
				}
				else
				{
					_7.h h6 = (h4 - h5) * a5W % a5v;
					h7 = h5 + a5B * h6;
				}
			}
			else
			{
				if (PublicOnly)
				{
					throw new v._0006("Missing private key to decrypt value.");
				}
				h7 = h2.ModPow(a5_0006, a5_0002);
			}
			if (a56)
			{
				h7 = h7 * h3.ModInverse(a5_0002) % a5_0002;
				h3.Clear();
			}
			byte[] result = d(h7, KeySize >> 3);
			h2.Clear();
			h7.Clear();
			return result;
		}

		public override byte[] EncryptValue(byte[] rgb)
		{
			if (a57)
			{
				throw new ObjectDisposedException("public key");
			}
			if (!a5a)
			{
				throw new Exception("not supported");
			}
			_7.h h2 = new _7.h(rgb);
			_7.h h3 = h2.ModPow(a5_000E, a5_0002);
			byte[] result = d(h3, KeySize >> 3);
			h2.Clear();
			h3.Clear();
			return result;
		}

		public override v._0001 ExportParameters(bool includePrivateParameters)
		{
			if (a57)
			{
				throw new ObjectDisposedException("");
			}
			if (!a5a)
			{
				throw new Exception("not supported");
			}
			v._0001 result = new v._0001
			{
				Exponent = a5_000E.GetBytes(),
				Modulus = a5_0002.GetBytes()
			};
			if (includePrivateParameters)
			{
				if (a5_0006 == null)
				{
					throw new v._0006("Missing private key");
				}
				result.D = a5_0006.GetBytes();
				if (result.D.Length != result.Modulus.Length)
				{
					byte[] array = new byte[result.Modulus.Length];
					Buffer.BlockCopy(result.D, 0, array, array.Length - result.D.Length, result.D.Length);
					result.D = array;
				}
				if (a5v != null && a5B != null && a5X != null && a5_0018 != null && a5W != null)
				{
					int num = KeySize >> 4;
					result.P = d(a5v, num);
					result.Q = d(a5B, num);
					result.DP = d(a5X, num);
					result.DQ = d(a5_0018, num);
					result.InverseQ = d(a5W, num);
				}
			}
			return result;
		}

		public override void ImportParameters(v._0001 parameters)
		{
			if (a57)
			{
				throw new ObjectDisposedException("");
			}
			if (parameters.Exponent == null)
			{
				throw new v._0006("Missing Exponent");
			}
			if (parameters.Modulus == null)
			{
				throw new v._0006("Missing Modulus");
			}
			a5_000E = new _7.h(parameters.Exponent);
			a5_0002 = new _7.h(parameters.Modulus);
			if (parameters.D != null)
			{
				a5_0006 = new _7.h(parameters.D);
			}
			if (parameters.DP != null)
			{
				a5X = new _7.h(parameters.DP);
			}
			if (parameters.DQ != null)
			{
				a5_0018 = new _7.h(parameters.DQ);
			}
			if (parameters.InverseQ != null)
			{
				a5W = new _7.h(parameters.InverseQ);
			}
			if (parameters.P != null)
			{
				a5v = new _7.h(parameters.P);
			}
			if (parameters.Q != null)
			{
				a5B = new _7.h(parameters.Q);
			}
			a5a = true;
			a5b = a5v != null && a5B != null && a5X != null && a5_0018 != null && a5W != null;
		}

		protected override void Dispose(bool disposing)
		{
			if (!a57)
			{
				if (a5_0006 != null)
				{
					a5_0006.Clear();
					a5_0006 = null;
				}
				if (a5v != null)
				{
					a5v.Clear();
					a5v = null;
				}
				if (a5B != null)
				{
					a5B.Clear();
					a5B = null;
				}
				if (a5X != null)
				{
					a5X.Clear();
					a5X = null;
				}
				if (a5_0018 != null)
				{
					a5_0018.Clear();
					a5_0018 = null;
				}
				if (a5W != null)
				{
					a5W.Clear();
					a5W = null;
				}
				if (disposing)
				{
					if (a5_000E != null)
					{
						a5_000E.Clear();
						a5_000E = null;
					}
					if (a5_0002 != null)
					{
						a5_0002.Clear();
						a5_0002 = null;
					}
				}
			}
			a57 = true;
		}

		private byte[] d(_7.h P_0, int P_1)
		{
			byte[] bytes = P_0.GetBytes();
			if (bytes.Length >= P_1)
			{
				return bytes;
			}
			byte[] array = new byte[P_1];
			Buffer.BlockCopy(bytes, 0, array, P_1 - bytes.Length, bytes.Length);
			Array.Clear(bytes, 0, bytes.Length);
			return array;
		}
	}
}
namespace _0001
{
	internal interface _6 : h, global::r.h
	{
		void Update(float dt);
	}
}
namespace _0002
{
	internal interface _6
	{
		void OnDetectingInitialCollision(P.h sender, P.h other, D.b pair);
	}
}
namespace _000F
{
	internal enum _6
	{
		Traction,
		Sliding,
		Floating
	}
}
namespace _0012
{
	internal class _6
	{
		internal static void b_0011(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			if (!(P_0.CollisionMove is CollisionMove { WorldBoundingSphere: var worldBoundingSphere } collisionMove))
			{
				return;
			}
			BoundingBox box = P_1.WorldBoundingBox;
			float closestPointOnBoxAndDistance = CoreHelper.GetClosestPointOnBoxAndDistance(ref box, ref worldBoundingSphere.Center, out var _);
			if (closestPointOnBoxAndDistance > worldBoundingSphere.Radius * worldBoundingSphere.Radius)
			{
				return;
			}
			Vector3 position = P_0.WorldBoundingSphere.Center;
			Ray ray = new Ray(position, collisionMove.Normal);
			box.Max.X += worldBoundingSphere.Radius;
			box.Max.Y += worldBoundingSphere.Radius;
			box.Max.Z += worldBoundingSphere.Radius;
			box.Min.X -= worldBoundingSphere.Radius;
			box.Min.Y -= worldBoundingSphere.Radius;
			box.Min.Z -= worldBoundingSphere.Radius;
			ray.Intersects(ref box, out var result);
			if (!result.HasValue)
			{
				return;
			}
			float value = result.Value;
			if (!(value < 0f) && !h.bc(P_0, P_1, P_2))
			{
				float num = value / collisionMove.Distance;
				if (!(num >= P_2.ContactTime))
				{
					CoreHelper.GetClosestPointOnBoxAndDistanceWithNormal(ref box, ref position, out var closestpoint2, out var surfacenormal);
					P_2.ContactTime = MathHelper.Clamp(num, 0f, 1f);
					P_2.ContactPoint = closestpoint2;
					P_2.SurfaceNormal = surfacenormal;
					P_2.ContactObject = P_1;
					P_2.Material = P_1.DefaultCollisionMaterial;
				}
			}
		}

		internal static void bw(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			if (!(P_0.CollisionMove is CollisionMove { WorldBoundingSphere: var worldBoundingSphere } collisionMove))
			{
				return;
			}
			BoundingSphere sphere = P_1.WorldBoundingSphere;
			worldBoundingSphere.Contains(ref sphere, out var result);
			if (result == ContainmentType.Disjoint)
			{
				return;
			}
			Vector3 center = P_0.WorldBoundingSphere.Center;
			Ray ray = new Ray(center, collisionMove.Normal);
			Plane plane = default(Plane);
			plane.Normal = -ray.Direction;
			plane.D = 0f - Vector3.Dot(plane.Normal, sphere.Center);
			ray.Intersects(ref plane, out var result2);
			if (!result2.HasValue)
			{
				return;
			}
			float value = result2.Value;
			if (value < 0f || h.bc(P_0, P_1, P_2))
			{
				return;
			}
			Vector3 value2 = ray.Position + ray.Direction * value;
			Vector3.DistanceSquared(ref value2, ref sphere.Center, out var result3);
			float num = worldBoundingSphere.Radius + sphere.Radius;
			num *= num;
			float num2 = num - result3;
			if (!(num2 < 0f))
			{
				num2 = (float)Math.Sqrt(num2);
				value -= num2;
				float num3 = value / collisionMove.Distance;
				if (!(num3 >= P_2.ContactTime))
				{
					Vector3 vector = ray.Position + ray.Direction * value;
					Vector3 vector2 = Vector3.Normalize(vector - sphere.Center);
					P_2.ContactTime = MathHelper.Clamp(num3, 0f, 1f);
					P_2.ContactPoint = vector - vector2 * worldBoundingSphere.Radius;
					P_2.SurfaceNormal = vector2;
					P_2.ContactObject = P_1;
					P_2.Material = P_1.DefaultCollisionMaterial;
				}
			}
		}

		internal static void bS(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			h.a5h.AccumulationValue++;
			CollisionMove collisionMove = P_0.CollisionMove as CollisionMove;
			CollisionMove collisionMove2 = P_1.CollisionMove as CollisionMove;
			if (collisionMove == null || collisionMove2 == null)
			{
				return;
			}
			BoundingSphere worldBoundingSphere = collisionMove.WorldBoundingSphere;
			BoundingBox boundingBox = BoundingBox.CreateMerged(P_0.WorldBoundingBox, BoundingBox.CreateFromSphere(worldBoundingSphere));
			CollisionMesh worldCollisionMesh = collisionMove2.WorldCollisionMesh;
			h.a5B.Clear();
			worldCollisionMesh.a5a._0012(ref boundingBox, true, h.a5B);
			if (h.a5B.Count <= 0)
			{
				return;
			}
			float distance = collisionMove.Distance;
			float radius = worldBoundingSphere.Radius;
			float num = 0f - radius;
			float num2 = 0f - (radius + distance);
			Vector3 vector = worldBoundingSphere.Center;
			Vector3 vector2 = vector;
			Vector3 center = P_0.WorldBoundingSphere.Center;
			Ray ray = new Ray(center, collisionMove.Normal);
			bool flag = false;
			foreach (CollisionMesh.CollisionSurface item in h.a5B)
			{
				h.a5b.AccumulationValue++;
				if (flag)
				{
					vector = vector2;
					worldBoundingSphere.Center = vector2;
					flag = false;
				}
				float num3 = vector.X * item.Surface.Normal.X + vector.Y * item.Surface.Normal.Y + vector.Z * item.Surface.Normal.Z + item.Surface.D;
				if (num3 > radius || num3 < num2)
				{
					continue;
				}
				float? result = null;
				if (num3 < num)
				{
					h.a56.AccumulationValue++;
					float num4 = center.X * item.Surface.Normal.X + center.Y * item.Surface.Normal.Y + center.Z * item.Surface.Normal.Z + item.Surface.D;
					if (num4 < num)
					{
						continue;
					}
					ray.Intersects(ref item.Surface, out result);
					h.a5a.AccumulationValue++;
					if (!result.HasValue)
					{
						continue;
					}
					float value = result.Value;
					if (value < 0f || value > distance)
					{
						continue;
					}
					vector = (worldBoundingSphere.Center = ray.Position + ray.Direction * value);
					h.a57.AccumulationValue++;
					flag = true;
				}
				if (!item.bp(worldCollisionMesh, ref vector, ref worldBoundingSphere, num))
				{
					continue;
				}
				Vector3.Dot(ref ray.Direction, ref item.Surface.Normal, out var result2);
				result2 *= -1f;
				if (result2 <= 0f)
				{
					continue;
				}
				if (!flag)
				{
					ray.Intersects(ref item.Surface, out result);
				}
				if (result.HasValue)
				{
					if (h.bc(P_0, P_1, P_2))
					{
						break;
					}
					float value2 = result.Value;
					float num5 = value2 - worldBoundingSphere.Radius / result2;
					float num6 = num5 / distance;
					if (!(num6 >= P_2.ContactTime))
					{
						Vector3 vector3 = ray.Position + ray.Direction * num5;
						P_2.ContactTime = MathHelper.Clamp(num6, 0f, 1f);
						P_2.ContactPoint = vector3 - item.Surface.Normal * worldBoundingSphere.Radius;
						P_2.SurfaceNormal = item.Surface.Normal;
						P_2.ContactObject = P_1;
						P_2.Material = item.Material;
					}
				}
			}
		}
	}
}
namespace _000E
{
	internal struct _6
	{
		public N._0006 LocalTransform;

		public b Shape;

		public float Weight;

		public _6(b shape, N._0006 localTransform, float weight)
		{
			LocalTransform = localTransform;
			Shape = shape;
			Weight = weight;
		}

		public _6(b shape, Vector3 position, float weight)
		{
			LocalTransform = new N._0006(position);
			Shape = shape;
			Weight = weight;
		}

		public _6(b shape, Quaternion orientation, float weight)
		{
			LocalTransform = new N._0006(orientation);
			Shape = shape;
			Weight = weight;
		}

		public _6(b shape, float weight)
		{
			LocalTransform = N._0006.Identity;
			Shape = shape;
			Weight = weight;
		}

		public _6(b shape, N._0006 localTransform)
		{
			LocalTransform = localTransform;
			Shape = shape;
			Weight = shape.ComputeVolume();
		}

		public _6(b shape, Vector3 position)
		{
			LocalTransform = new N._0006(position);
			Shape = shape;
			Weight = shape.ComputeVolume();
		}

		public _6(b shape, Quaternion orientation)
		{
			LocalTransform = new N._0006(orientation);
			Shape = shape;
			Weight = shape.ComputeVolume();
		}

		public _6(b shape)
		{
			LocalTransform = N._0006.Identity;
			Shape = shape;
			Weight = shape.ComputeVolume();
		}
	}
}
namespace _0017
{
	internal struct _6
	{
		public a LocalSimplexA;

		public a LocalSimplexB;

		public b State;
	}
}
namespace _0004
{
	internal static class _6
	{
		public static void ReduceContacts(l._7<h> contacts, l._0006<b> contactCandidates, l._7<int> contactsToRemove, l._0006<b> toAdd)
		{
			float num = float.MinValue;
			int num2 = -1;
			Vector3 value = r.X.ZeroVector;
			for (int i = 0; i < contacts.a5h; i++)
			{
				Vector3.Add(ref value, ref contacts.Elements[i].Normal, out value);
				if (contacts.Elements[i].PenetrationDepth > num)
				{
					num2 = i;
					num = contacts.Elements[i].PenetrationDepth;
				}
			}
			for (int j = 0; j < contactCandidates.a5h; j++)
			{
				Vector3.Add(ref value, ref contactCandidates.Elements[j].Normal, out value);
				if (contactCandidates.Elements[j].PenetrationDepth > num)
				{
					num2 = contacts.a5h + j;
					num = contactCandidates.Elements[j].PenetrationDepth;
				}
			}
			if (value.LengthSquared() < 1E-07f)
			{
				if (contacts.a5h > 0)
				{
					value = contacts.Elements[0].Normal;
				}
				else
				{
					if (contactCandidates.a5h <= 0)
					{
						throw new ArgumentException("Cannot reduce an empty contact set.");
					}
					value = contactCandidates.Elements[0].Normal;
				}
			}
			Vector3 value2 = ((num2 >= contacts.a5h) ? contactCandidates.Elements[num2 - contacts.a5h].Position : contacts.Elements[num2].Position);
			float num3 = 0f;
			int num4 = -1;
			float result;
			for (int k = 0; k < contacts.a5h; k++)
			{
				Vector3.DistanceSquared(ref contacts.Elements[k].Position, ref value2, out result);
				if (result > num3)
				{
					num3 = result;
					num4 = k;
				}
			}
			for (int n = 0; n < contactCandidates.a5h; n++)
			{
				Vector3.DistanceSquared(ref contactCandidates.Elements[n].Position, ref value2, out result);
				if (result > num3)
				{
					num3 = result;
					num4 = contacts.a5h + n;
				}
			}
			if (num4 == -1)
			{
				if (contacts.a5h > 0)
				{
					for (int num5 = 1; num5 < contacts.a5h; num5++)
					{
						contactsToRemove.Add(num5);
					}
					return;
				}
				if (contactCandidates.a5h > 0)
				{
					toAdd.Add(ref contactCandidates.Elements[0]);
					return;
				}
				throw new ArgumentException("Cannot reduce an empty contact set.");
			}
			Vector3 value3 = ((num4 >= contacts.a5h) ? contactCandidates.Elements[num4 - contacts.a5h].Position : contacts.Elements[num4].Position);
			Vector3.Subtract(ref value2, ref value3, out var result2);
			Vector3.Cross(ref result2, ref value, out var result3);
			float num6 = float.MaxValue;
			float num7 = float.MinValue;
			int num8 = -1;
			int num9 = -1;
			for (int num10 = 0; num10 < contacts.a5h; num10++)
			{
				Vector3.Dot(ref contacts.Elements[num10].Position, ref result3, out var result4);
				if (result4 < num6)
				{
					num8 = num10;
					num6 = result4;
				}
				if (result4 > num7)
				{
					num9 = num10;
					num7 = result4;
				}
			}
			for (int num11 = 0; num11 < contactCandidates.a5h; num11++)
			{
				Vector3.Dot(ref contactCandidates.Elements[num11].Position, ref result3, out var result5);
				if (result5 < num6)
				{
					num8 = num11 + contacts.a5h;
					num6 = result5;
				}
				if (result5 > num7)
				{
					num9 = num11 + contacts.a5h;
					num7 = result5;
				}
			}
			for (int num12 = 0; num12 < contactCandidates.a5h; num12++)
			{
				int num13 = num12 + contacts.a5h;
				if (num13 == num2 || num13 == num4 || num13 == num8 || num13 == num9)
				{
					toAdd.Add(ref contactCandidates.Elements[num12]);
				}
			}
			for (int num14 = 0; num14 < contacts.a5h; num14++)
			{
				if (num14 != num2 && num14 != num4 && num14 != num8 && num14 != num9)
				{
					contactsToRemove.Add(num14);
				}
			}
		}

		public static void ReduceContacts(l._7<h> contacts, ref b contactCandidate, l._7<int> toRemove, out bool addCandidate)
		{
			if (contacts.a5h != 4)
			{
				throw new ArgumentException("Can only use this method to reduce contact lists with four contacts and a contact candidate.");
			}
			float num = float.MinValue;
			int num2 = -1;
			for (int i = 0; i < 4; i++)
			{
				if (contacts.Elements[i].PenetrationDepth > num)
				{
					num2 = i;
					num = contacts.Elements[i].PenetrationDepth;
				}
			}
			if (contactCandidate.PenetrationDepth > num)
			{
				num2 = 4;
			}
			Vector3 value = ((num2 >= 4) ? contactCandidate.Position : contacts.Elements[num2].Position);
			float num3 = 0f;
			int num4 = -1;
			float result;
			for (int j = 0; j < 4; j++)
			{
				Vector3.DistanceSquared(ref contacts.Elements[j].Position, ref value, out result);
				if (result > num3)
				{
					num3 = result;
					num4 = j;
				}
			}
			Vector3.DistanceSquared(ref contactCandidate.Position, ref value, out result);
			if (result > num3)
			{
				num4 = 4;
			}
			Vector3 value2 = ((num4 >= contacts.a5h) ? contactCandidate.Position : contacts.Elements[num4].Position);
			Vector3.Subtract(ref value, ref value2, out var result2);
			Vector3.Cross(ref result2, ref contacts.Elements[0].Normal, out var result3);
			float num5 = float.MaxValue;
			float num6 = float.MinValue;
			int num7 = -1;
			int num8 = -1;
			float result4;
			for (int k = 0; k < 4; k++)
			{
				Vector3.Dot(ref contacts.Elements[k].Position, ref result3, out result4);
				if (result4 < num5)
				{
					num7 = k;
					num5 = result4;
				}
				if (result4 > num6)
				{
					num8 = k;
					num6 = result4;
				}
			}
			Vector3.Dot(ref contactCandidate.Position, ref result3, out result4);
			if (result4 < num5)
			{
				num7 = 4;
			}
			if (result4 > num6)
			{
				num8 = 4;
			}
			if (4 == num2 || 4 == num4 || 4 == num7 || 4 == num8)
			{
				addCandidate = true;
				for (int n = 0; n < 4; n++)
				{
					if (n != num2 && n != num4 && n != num7 && n != num8)
					{
						toRemove.Add(n);
						break;
					}
				}
			}
			else
			{
				addCandidate = false;
			}
		}
	}
}
namespace _0010
{
	internal class _6 : G.h
	{
		protected E.h entityA;

		protected E.h entityB;

		public E.h EntityA => entityA;

		public E.h EntityB => entityB;

		public new void Add(L.h manifoldConstraint)
		{
			if (manifoldConstraint.solver == null)
			{
				if (manifoldConstraint.SolverGroup == null)
				{
					a5h.Add(manifoldConstraint);
					manifoldConstraint.SolverGroup = this;
					manifoldConstraint.Solver = solver;
					return;
				}
				throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a SolverGroup.");
			}
			throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a solver.");
		}

		public new void Remove(L.h manifoldConstraint)
		{
			if (manifoldConstraint.SolverGroup == this)
			{
				a5h.Remove(manifoldConstraint);
				manifoldConstraint.SolverGroup = null;
				manifoldConstraint.Solver = null;
				return;
			}
			throw new InvalidOperationException("Cannot remove SolverUpdateable from SolverGroup; it doesn't belong to this SolverGroup.");
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (entityA != null)
			{
				outputInvolvedEntities.Add(entityA);
			}
			if (entityB != null)
			{
				outputInvolvedEntities.Add(entityB);
			}
		}

		protected internal override void OnInvolvedEntitiesChanged()
		{
			CollectInvolvedEntities();
		}

		public virtual void Initialize(E.h a, E.h b)
		{
			entityA = a;
			entityB = b;
			OnInvolvedEntitiesChanged();
		}

		public virtual void CleanUp()
		{
			entityA = null;
			entityB = null;
			OnInvolvedEntitiesChanged();
		}
	}
}
namespace _0013
{
	internal class _6 : E.b<s.B<y._7>>
	{
		public float Radius
		{
			get
			{
				return base.CollisionInformation.Shape.Radius;
			}
			set
			{
				base.CollisionInformation.Shape.Radius = value;
			}
		}

		private _6(float P_0)
			: base(new s.B<y._7>(new y._7(P_0)))
		{
		}

		private _6(float P_0, float P_1)
			: base(new s.B<y._7>(new y._7(P_0)), P_1)
		{
		}

		public _6(Vector3 position, float radius, float mass)
			: this(radius, mass)
		{
			base.Position = position;
		}

		public _6(Vector3 position, float radius)
			: this(radius)
		{
			base.Position = position;
		}

		public _6(n.B motionState, float radius, float mass)
			: this(radius, mass)
		{
			base.MotionState = motionState;
		}

		public _6(n.B motionState, float radius)
			: this(radius)
		{
			base.MotionState = motionState;
		}
	}
}
namespace _0014
{
	internal static class _6
	{
		public static bool UseRk4AngularIntegration;

		public static bool ConserveAngularMomentum;

		private static float a5h = 0.8f;

		public static m._7 DefaultPositionUpdateMode = m._7.Discrete;

		public static bool UseExtraExpansionForContinuousBoundingBoxes;

		public static bool UseCCDForNoSolverPairs;

		public static float CoreShapeScaling
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = MathHelper.Clamp(value, 0f, 0.99f);
			}
		}
	}
}
