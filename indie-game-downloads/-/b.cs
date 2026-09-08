using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using _0004;
using _0014;
using B;
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
using T;
using W;
using Y;
using l;
using n;
using q;
using r;
using s;
using u;
using v;
using y;

namespace _0006
{
	internal sealed class b
	{
		private static byte[] a5h = new byte[20]
		{
			218, 57, 163, 238, 94, 107, 75, 13, 50, 85,
			191, 239, 149, 96, 24, 144, 175, 216, 7, 9
		};

		private b()
		{
		}

		private static bool l(byte[] P_0, byte[] P_1)
		{
			bool flag = P_0.Length == P_1.Length;
			if (flag)
			{
				for (int i = 0; i < P_0.Length; i++)
				{
					if (P_0[i] != P_1[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		private static byte[] n(byte[] P_0, byte[] P_1)
		{
			byte[] array = new byte[P_0.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(P_0[i] ^ P_1[i]);
			}
			return array;
		}

		private static byte[] N(v.X P_0)
		{
			if (P_0 is v.u)
			{
				return a5h;
			}
			return P_0.ComputeHash((byte[])null);
		}

		public static byte[] I2OSP(int x, int size)
		{
			byte[] bytes = BitConverter.GetBytes(x);
			return I2OSP(bytes, size);
		}

		public static byte[] I2OSP(byte[] x, int size)
		{
			byte[] array = new byte[size];
			Buffer.BlockCopy(x, 0, array, array.Length - x.Length, x.Length);
			return array;
		}

		public static byte[] OS2IP(byte[] x)
		{
			int num = 0;
			while (x[num++] == 0 && num < x.Length)
			{
			}
			num--;
			if (num > 0)
			{
				byte[] array = new byte[x.Length - num];
				Buffer.BlockCopy(x, num, array, 0, array.Length);
				return array;
			}
			return x;
		}

		public static byte[] RSAEP(v.b rsa, byte[] m)
		{
			return rsa.EncryptValue(m);
		}

		public static byte[] RSADP(v.b rsa, byte[] c)
		{
			return rsa.DecryptValue(c);
		}

		public static byte[] RSASP1(v.b rsa, byte[] m)
		{
			return rsa.DecryptValue(m);
		}

		public static byte[] RSAVP1(v.b rsa, byte[] s)
		{
			return rsa.EncryptValue(s);
		}

		public static byte[] Encrypt_OAEP(v.b rsa, v.X hash, v.W rng, byte[] M)
		{
			int num = rsa.KeySize / 8;
			int num2 = hash.HashSize / 8;
			if (M.Length > num - 2 * num2 - 2)
			{
				throw new v._0006("message too long");
			}
			byte[] array = N(hash);
			int num3 = num - M.Length - 2 * num2 - 2;
			byte[] array2 = new byte[array.Length + num3 + 1 + M.Length];
			Buffer.BlockCopy(array, 0, array2, 0, array.Length);
			array2[array.Length + num3] = 1;
			Buffer.BlockCopy(M, 0, array2, array2.Length - M.Length, M.Length);
			byte[] array3 = new byte[num2];
			rng.GetBytes(array3);
			byte[] array4 = MGF1(hash, array3, num - num2 - 1);
			byte[] array5 = n(array2, array4);
			byte[] array6 = MGF1(hash, array5, num2);
			byte[] array7 = n(array3, array6);
			byte[] dst = new byte[array7.Length + array5.Length + 1];
			Buffer.BlockCopy(array7, 0, dst, 1, array7.Length);
			Buffer.BlockCopy(array5, 0, dst, array7.Length + 1, array5.Length);
			byte[] array8 = OS2IP(dst);
			byte[] array9 = RSAEP(rsa, array8);
			return I2OSP(array9, num);
		}

		public static byte[] Decrypt_OAEP(v.b rsa, v.X hash, byte[] C)
		{
			int num = rsa.KeySize / 8;
			int num2 = hash.HashSize / 8;
			if (num < 2 * num2 + 2 || C.Length != num)
			{
				throw new v._0006("decryption error");
			}
			byte[] array = OS2IP(C);
			byte[] array2 = RSADP(rsa, array);
			byte[] array3 = I2OSP(array2, num);
			byte[] array4 = new byte[num2];
			Buffer.BlockCopy(array3, 1, array4, 0, array4.Length);
			byte[] array5 = new byte[num - num2 - 1];
			Buffer.BlockCopy(array3, array3.Length - array5.Length, array5, 0, array5.Length);
			byte[] array6 = MGF1(hash, array5, num2);
			byte[] mgfSeed = n(array4, array6);
			byte[] array7 = MGF1(hash, mgfSeed, num - num2 - 1);
			byte[] array8 = n(array5, array7);
			byte[] array9 = N(hash);
			byte[] array10 = new byte[array9.Length];
			Buffer.BlockCopy(array8, 0, array10, 0, array10.Length);
			bool flag = l(array9, array10);
			int i;
			for (i = array9.Length; array8[i] == 0; i++)
			{
			}
			int num3 = array8.Length - i - 1;
			byte[] array11 = new byte[num3];
			Buffer.BlockCopy(array8, i + 1, array11, 0, num3);
			if (array3[0] != 0 || !flag || array8[i] != 1)
			{
				return null;
			}
			return array11;
		}

		public static byte[] Encrypt_v15(v.b rsa, v.W rng, byte[] M)
		{
			int num = rsa.KeySize / 8;
			if (M.Length > num - 11)
			{
				throw new v._0006("message too long");
			}
			int num2 = Math.Max(8, num - M.Length - 3);
			byte[] array = new byte[num2];
			rng.GetNonZeroBytes(array);
			byte[] array2 = new byte[num];
			array2[1] = 2;
			Buffer.BlockCopy(array, 0, array2, 2, num2);
			Buffer.BlockCopy(M, 0, array2, num - M.Length, M.Length);
			byte[] array3 = OS2IP(array2);
			byte[] array4 = RSAEP(rsa, array3);
			return I2OSP(array4, num);
		}

		public static byte[] Decrypt_v15(v.b rsa, byte[] C)
		{
			int num = rsa.KeySize >> 3;
			if (num < 11 || C.Length > num)
			{
				throw new v._0006("decryption error");
			}
			byte[] array = OS2IP(C);
			byte[] array2 = RSADP(rsa, array);
			byte[] array3 = I2OSP(array2, num);
			if (array3[0] != 0 || array3[1] != 2)
			{
				return null;
			}
			int i;
			for (i = 10; array3[i] != 0 && i < array3.Length; i++)
			{
			}
			if (array3[i] != 0)
			{
				return null;
			}
			i++;
			byte[] array4 = new byte[array3.Length - i];
			Buffer.BlockCopy(array3, i, array4, 0, array4.Length);
			return array4;
		}

		public static byte[] Sign_v15(v.b rsa, v.X hash, byte[] hashValue)
		{
			int num = rsa.KeySize >> 3;
			byte[] array = Encode_v15(hash, hashValue, num);
			byte[] array2 = OS2IP(array);
			byte[] array3 = RSASP1(rsa, array2);
			return I2OSP(array3, num);
		}

		public static bool Verify_v15(v.b rsa, v.X hash, byte[] hashValue, byte[] signature)
		{
			return Verify_v15(rsa, hash, hashValue, signature, tryNonStandardEncoding: false);
		}

		public static bool Verify_v15(v.b rsa, v.X hash, byte[] hashValue, byte[] signature, bool tryNonStandardEncoding)
		{
			int num = rsa.KeySize >> 3;
			byte[] s = OS2IP(signature);
			byte[] array = RSAVP1(rsa, s);
			byte[] array2 = I2OSP(array, num);
			byte[] array3 = Encode_v15(hash, hashValue, num);
			bool flag = l(array3, array2);
			if (flag || !tryNonStandardEncoding)
			{
				return flag;
			}
			if (array2[0] != 0 || array2[1] != 1)
			{
				return false;
			}
			int i;
			for (i = 2; i < array2.Length - hashValue.Length - 1; i++)
			{
				if (array2[i] != byte.MaxValue)
				{
					return false;
				}
			}
			if (array2[i++] != 0)
			{
				return false;
			}
			byte[] array4 = new byte[hashValue.Length];
			Buffer.BlockCopy(array2, i, array4, 0, array4.Length);
			return l(array4, hashValue);
		}

		public static byte[] Encode_v15(v.X hash, byte[] hashValue, int emLength)
		{
			if (hashValue.Length != hash.HashSize >> 3)
			{
				throw new v._0006("bad hash length for " + hash.ToString());
			}
			byte[] array = null;
			string text = v._7.MapNameToOID(hash.ToString());
			if (text != null)
			{
				B.h h2 = new B.h(48);
				h2.Add(new B.h(v._7.EncodeOID(text)));
				h2.Add(new B.h(5));
				B.h asn = new B.h(4, hashValue);
				B.h h3 = new B.h(48);
				h3.Add(h2);
				h3.Add(asn);
				array = h3.GetBytes();
			}
			else
			{
				array = hashValue;
			}
			Buffer.BlockCopy(hashValue, 0, array, array.Length - hashValue.Length, hashValue.Length);
			int num = Math.Max(8, emLength - array.Length - 3);
			byte[] array2 = new byte[num + array.Length + 3];
			array2[1] = 1;
			for (int i = 2; i < num + 2; i++)
			{
				array2[i] = byte.MaxValue;
			}
			Buffer.BlockCopy(array, 0, array2, num + 3, array.Length);
			return array2;
		}

		public static byte[] MGF1(v.X hash, byte[] mgfSeed, int maskLen)
		{
			if (maskLen < 0)
			{
				throw new OverflowException();
			}
			int num = mgfSeed.Length;
			int num2 = hash.HashSize >> 3;
			int num3 = maskLen / num2;
			if (maskLen % num2 != 0)
			{
				num3++;
			}
			byte[] array = new byte[num3 * num2];
			byte[] array2 = new byte[num + 4];
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				byte[] src = I2OSP(i, 4);
				Buffer.BlockCopy(mgfSeed, 0, array2, 0, num);
				Buffer.BlockCopy(src, 0, array2, num, 4);
				byte[] src2 = hash.ComputeHash(array2);
				Buffer.BlockCopy(src2, 0, array, num4, num2);
				num4 += num;
			}
			byte[] array3 = new byte[maskLen];
			Buffer.BlockCopy(array, 0, array3, 0, maskLen);
			return array3;
		}
	}
}
namespace _0018
{
	[Serializable]
	internal sealed class b : IComparer
	{
		public static readonly b Default = new b();

		internal static readonly b a5h = new b(CultureInfo.InvariantCulture);

		private CompareInfo a5b;

		private b()
		{
		}

		internal b(CultureInfo P_0)
		{
			if (P_0 == null)
			{
				throw new ArgumentNullException("culture");
			}
			a5b = P_0.CompareInfo;
		}

		public int Compare(object a, object b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a == null)
			{
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			if (a5b != null)
			{
				string text = a as string;
				string text2 = b as string;
				if (text != null && text2 != null)
				{
					return a5b.Compare(text, text2);
				}
			}
			if (a is IComparable)
			{
				return (a as IComparable).CompareTo(b);
			}
			if (b is IComparable)
			{
				return -(b as IComparable).CompareTo(a);
			}
			throw new ArgumentException("Neither 'a' nor 'b' implements IComparable.");
		}
	}
}
namespace _0002
{
	internal class b<T> : global::W.h, h where T : Y.a
	{
		protected internal T owner;

		private global::W._7 a5h;

		private bool a5b;

		public T Owner => owner;

		global::W._7 global::W.h.DeferredEventDispatcher
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
			}
		}

		bool global::W.h.IsActive
		{
			get
			{
				return a5b;
			}
			set
			{
				if (!a5b && value)
				{
					a5b = true;
					if (a5h != null)
					{
						a5h.CreatorActivityChanged(this);
					}
				}
				else if (a5b && !value)
				{
					a5b = false;
					if (a5h != null)
					{
						a5h.CreatorActivityChanged(this);
					}
				}
			}
		}

		public b(T owner)
		{
			this.owner = owner;
		}

		protected void VerifyEventStatus()
		{
			if (EventsAreInactive())
			{
				((global::W.h)this).IsActive = false;
			}
		}

		protected virtual bool EventsAreInactive()
		{
			return true;
		}

		protected void AddToEventfuls()
		{
			((global::W.h)this).IsActive = true;
		}

		void global::W.h.DispatchEvents()
		{
			DispatchEvents();
		}

		protected virtual void DispatchEvents()
		{
		}

		public void OnPairCreated(Y.a other, D.h collisionPair)
		{
		}

		public void OnPairRemoved(Y.a other)
		{
		}

		public void OnPairUpdated(Y.a other, D.h collisionPair)
		{
		}

		public virtual void RemoveAllEvents()
		{
			VerifyEventStatus();
		}
	}
}
namespace _000E
{
	internal abstract class b : h
	{
		public virtual float ComputeVolume()
		{
			ComputeDistributionInformation(out var shapeInfo);
			return shapeInfo.Volume;
		}

		public virtual N._7 ComputeVolumeDistribution(out float volume)
		{
			ComputeDistributionInformation(out var shapeInfo);
			volume = shapeInfo.Volume;
			return shapeInfo.VolumeDistribution;
		}

		public virtual N._7 ComputeVolumeDistribution()
		{
			ComputeDistributionInformation(out var shapeInfo);
			return shapeInfo.VolumeDistribution;
		}

		public virtual Vector3 ComputeCenter()
		{
			ComputeDistributionInformation(out var shapeInfo);
			return shapeInfo.Center;
		}

		public virtual Vector3 ComputeCenter(out float volume)
		{
			ComputeDistributionInformation(out var shapeInfo);
			volume = shapeInfo.Volume;
			return shapeInfo.Center;
		}

		public abstract void ComputeDistributionInformation(out _7 shapeInfo);

		public abstract s.b GetCollidableInstance();
	}
}
namespace _0001
{
	internal abstract class b : h, global::r.h
	{
		private bool a5h = true;

		private List<X> a5b = new List<X>();

		private global::r.a a56;

		[CompilerGenerated]
		private bool a5a;

		[CompilerGenerated]
		private object a57;

		List<X> h.Managers => a5b;

		public bool IsUpdatedSequentially
		{
			get
			{
				return a5h;
			}
			set
			{
				bool flag = a5h;
				a5h = value;
				if (value != flag)
				{
					for (int i = 0; i < a5b.Count; i++)
					{
						a5b[i].SequentialUpdatingStateChanged(this);
					}
				}
			}
		}

		public bool IsUpdating
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			set
			{
				a5a = value;
			}
		}

		global::r.a global::r.h.Space
		{
			get
			{
				return a56;
			}
			set
			{
				a56 = value;
			}
		}

		public global::r.a Space => a56;

		public object Tag
		{
			[CompilerGenerated]
			get
			{
				return a57;
			}
			[CompilerGenerated]
			set
			{
				a57 = value;
			}
		}

		protected b()
		{
			IsUpdating = true;
		}

		public virtual void OnAdditionToSpace(global::r.a newSpace)
		{
		}

		public virtual void OnRemovalFromSpace(global::r.a oldSpace)
		{
		}
	}
}
namespace _000F
{
	internal class b : L.h
	{
		private new h a5h;

		private y a5b;

		private Vector2 a56;

		private float a5a = 8f;

		private float a57 = 3f;

		private float a5_0006 = 6f;

		private float a5v = 1f;

		private float a5B = 1000f;

		private float a5X = 50f;

		private float a5_0018 = 250f;

		private float a5W = 1f;

		private float a5_0002;

		private float a5_000E;

		private N.b a5y;

		private E.h a5r;

		private Vector3 a5_0001;

		private Vector3 a5_000F;

		private Vector3 a5Z;

		private Vector3 a5u;

		private Vector3 a5L;

		private Vector3 a5z;

		private Vector2 a5Y;

		private Vector2 a5m;

		private Vector2 a5T;

		private Vector3 a5q;

		private bool a5E;

		private bool a5_0013;

		private E.h a5x;

		private float a5Q;

		private float a5_0012;

		[CompilerGenerated]
		private _6 a5g;

		public y SupportData
		{
			get
			{
				return a5b;
			}
			set
			{
				P.h supportObject = a5b.SupportObject;
				a5b = value;
				if (supportObject != a5b.SupportObject)
				{
					OnInvolvedEntitiesChanged();
					if (a5b.SupportObject is s.b b2)
					{
						a5r = b2.Entity;
					}
					else
					{
						a5r = null;
					}
				}
			}
		}

		public Vector2 MovementDirection
		{
			get
			{
				return a56;
			}
			set
			{
				float num = value.LengthSquared();
				if (num > 1E-07f)
				{
					a5h.Body.ActivityInformation.Activate();
					Vector2.Divide(ref value, (float)Math.Sqrt(num), out a56);
				}
				else
				{
					a5h.Body.ActivityInformation.Activate();
					a56 = default(Vector2);
				}
			}
		}

		public float Speed
		{
			get
			{
				return a5a;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5a = value;
			}
		}

		public float CrouchingSpeed
		{
			get
			{
				return a57;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a57 = value;
			}
		}

		public float SlidingSpeed
		{
			get
			{
				return a5_0006;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5_0006 = value;
			}
		}

		public float AirSpeed
		{
			get
			{
				return a5v;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5v = value;
			}
		}

		public float MaximumForce
		{
			get
			{
				return a5B;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5B = value;
			}
		}

		public float MaximumSlidingForce
		{
			get
			{
				return a5X;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5X = value;
			}
		}

		public float MaximumAirForce
		{
			get
			{
				return a5_0018;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5_0018 = value;
			}
		}

		public float SupportForceFactor
		{
			get
			{
				return a5W;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5W = value;
			}
		}

		public _6 MovementMode
		{
			[CompilerGenerated]
			get
			{
				return a5g;
			}
			[CompilerGenerated]
			private set
			{
				a5g = obj;
			}
		}

		public Vector2 RelativeVelocity
		{
			get
			{
				Vector2 result = default(Vector2);
				Vector3 vector = a5h.Body.LinearVelocity;
				Vector3.Dot(ref a5_0001, ref vector, out result.X);
				Vector3.Dot(ref a5_000F, ref vector, out result.Y);
				if (a5r != null)
				{
					Vector3 vector2 = a5r.LinearVelocity;
					Vector3 vector3 = a5r.AngularVelocity;
					Vector3.Dot(ref a5Z, ref vector2, out var result2);
					Vector3.Dot(ref a5u, ref vector2, out var result3);
					result.X += result2;
					result.Y += result3;
					Vector3.Dot(ref a5L, ref vector3, out result2);
					Vector3.Dot(ref a5z, ref vector3, out result3);
					result.X += result2;
					result.Y += result3;
				}
				return result;
			}
		}

		public Vector3 RelativeWorldVelocity
		{
			get
			{
				Vector3 linearVelocity = a5h.Body.LinearVelocity;
				if (a5r != null)
				{
					return linearVelocity - global::r.X.GetVelocityOfPoint(a5b.Position, a5r);
				}
				return linearVelocity;
			}
		}

		public b(h characterController)
		{
			a5h = characterController;
			CollectInvolvedEntities();
			a5Q = a5a / (a5B * a5h.Body.InverseMass);
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5b.SupportObject is s.b b2)
			{
				outputInvolvedEntities.Add(b2.Entity);
			}
			outputInvolvedEntities.Add(a5h.Body);
		}

		public override void Update(float dt)
		{
			bool flag = a56.LengthSquared() > 0f;
			if (a5b.SupportObject != null)
			{
				if (a5b.HasTraction)
				{
					MovementMode = _6.Traction;
					if (a5h.StanceManager.CurrentStance == v.Standing)
					{
						a5_0002 = a5a;
					}
					else
					{
						a5_0002 = a57;
					}
					a5_000E = a5B;
				}
				else
				{
					MovementMode = _6.Sliding;
					a5_0002 = a5_0006;
					a5_000E = a5X;
				}
			}
			else
			{
				MovementMode = _6.Floating;
				a5_0002 = a5v;
				a5_000E = a5_0018;
				a5r = null;
			}
			if (!flag)
			{
				a5_0002 = 0f;
			}
			a5_000E *= dt;
			Vector3 value = a5h.Body.OrientationMatrix.Down;
			if (MovementMode != _6.Floating)
			{
				if (flag)
				{
					Vector3 value2 = new Vector3(a56.X, 0f, a56.Y);
					Vector3.Add(ref value2, ref value, out var result);
					Plane p = new Plane(a5h.SupportFinder.HasTraction ? a5b.Normal : a5b.Normal, 0f);
					global::r.X.GetLinePlaneIntersection(ref value2, ref result, ref p, out var _, out var vector);
					vector.Normalize();
					Vector3.Cross(ref vector, ref a5b.Normal, out var result2);
					a5_0001 = vector;
					a5_000F = result2;
					a5Z = -vector;
					a5u = -result2;
				}
				else
				{
					Vector3.Dot(ref a5_0001, ref a5b.Normal, out var result3);
					Vector3.Multiply(ref a5b.Normal, result3, out var result4);
					Vector3.Subtract(ref a5_0001, ref result4, out a5_0001);
					float num2 = a5_0001.LengthSquared();
					if (num2 < 1E-07f)
					{
						Vector3.Cross(ref global::r.X.RightVector, ref a5b.Normal, out a5_0001);
						num2 = a5_0001.LengthSquared();
						if (num2 < 1E-07f)
						{
							Vector3.Cross(ref global::r.X.ForwardVector, ref a5b.Normal, out a5_0001);
							num2 = a5_0001.LengthSquared();
						}
					}
					Vector3.Divide(ref a5_0001, (float)Math.Sqrt(num2), out a5_0001);
					Vector3.Cross(ref a5_0001, ref a5b.Normal, out a5_000F);
					a5Z = -a5_0001;
					a5u = -a5_000F;
				}
				if (a5r != null)
				{
					Vector3 vector2 = a5b.Position - a5r.Position;
					Vector3.Cross(ref a5_0001, ref vector2, out a5L);
					Vector3.Cross(ref a5_000F, ref vector2, out a5z);
				}
				else
				{
					a5L = default(Vector3);
					a5z = default(Vector3);
				}
			}
			else
			{
				a5_0001 = new Vector3(a56.X, 0f, a56.Y);
				a5_000F = new Vector3(a56.Y, 0f, 0f - a56.X);
			}
			a5m.X = a5_0002;
			a5m.Y = 0f;
			if (a5r != null && a5r.IsDynamic)
			{
				float num3 = 0f;
				float inverseMass = a5h.Body.InverseMass;
				float num4 = inverseMass;
				float num5 = inverseMass;
				N._7 matrix = a5r.InertiaTensorInverse;
				N._7.Multiply(ref matrix, a5W, out matrix);
				inverseMass = a5W * a5r.InverseMass;
				N._7.Transform(ref a5L, ref matrix, out var result5);
				Vector3.Dot(ref result5, ref a5L, out var result6);
				num4 += inverseMass + result6;
				Vector3.Dot(ref result5, ref a5z, out result6);
				num3 += result6;
				N._7.Transform(ref a5z, ref matrix, out result5);
				Vector3.Dot(ref result5, ref a5z, out result6);
				num5 += inverseMass + result6;
				a5y.M11 = num4;
				a5y.M12 = num3;
				a5y.M21 = num3;
				a5y.M22 = num5;
				N.b.Invert(ref a5y, out a5y);
			}
			else
			{
				N.b.CreateScale(a5h.Body.Mass, out a5y);
			}
			if (a5r != null && ((a5E && !flag) || (!a5_0013 && a5b.HasTraction) || a5r != a5x))
			{
				a5_0012 = 0f;
			}
			if (!flag && a5b.HasTraction && a5r != null)
			{
				if (a5_0012 >= 0f && a5_0012 < a5Q)
				{
					a5_0012 += dt;
				}
				if (a5_0012 >= a5Q)
				{
					Vector3.Multiply(ref value, a5h.Body.Height * 0.5f, out a5q);
					a5q = a5q + a5h.Body.Position - a5r.Position;
					a5q = N._7.TransformTranspose(a5q, a5r.OrientationMatrix);
					a5_0012 = -1f;
				}
				if (a5_0012 < 0f)
				{
					Vector3.Multiply(ref value, a5h.Body.Height * 0.5f, out var result7);
					result7 += a5h.Body.Position;
					Vector3 value3 = N._7.Transform(a5q, a5r.OrientationMatrix) + a5r.Position;
					Vector3.Subtract(ref result7, ref value3, out var result8);
					if (result8.LengthSquared() > 0.0225f)
					{
						Vector3.Multiply(ref value, a5h.Body.Height * 0.5f, out a5q);
						a5q = a5q + a5h.Body.Position - a5r.Position;
						a5q = N._7.TransformTranspose(a5q, a5r.OrientationMatrix);
						a5T = default(Vector2);
					}
					else
					{
						Vector3.Dot(ref result8, ref a5_0001, out a5T.X);
						Vector3.Dot(ref result8, ref a5_000F, out a5T.Y);
						Vector2.Multiply(ref a5T, 0.2f / dt, out a5T);
					}
				}
			}
			else
			{
				a5_0012 = 0f;
				a5T = default(Vector2);
			}
			a5E = flag;
			a5_0013 = a5b.HasTraction;
			a5x = a5r;
		}

		public override void ExclusiveUpdate()
		{
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			float num = a5Y.X;
			float num2 = a5Y.Y;
			impulse.X = a5_0001.X * num + a5_000F.X * num2;
			impulse.Y = a5_0001.Y * num + a5_000F.Y * num2;
			impulse.Z = a5_0001.Z * num + a5_000F.Z * num2;
			a5h.Body.ApplyLinearImpulse(ref impulse);
			if (a5r != null && a5r.IsDynamic)
			{
				Vector3.Multiply(ref impulse, 0f - a5W, out impulse);
				num *= a5W;
				num2 *= a5W;
				impulse2.X = num * a5L.X + num2 * a5z.X;
				impulse2.Y = num * a5L.Y + num2 * a5z.Y;
				impulse2.Z = num * a5L.Z + num2 * a5z.Z;
				a5r.ApplyLinearImpulse(ref impulse);
				a5r.ApplyAngularImpulse(ref impulse2);
			}
		}

		public override float SolveIteration()
		{
			Vector2 value = RelativeVelocity;
			Vector2.Add(ref value, ref a5T, out value);
			Vector2.Subtract(ref a5m, ref value, out var result);
			N.b.Transform(ref result, ref a5y, out result);
			Vector2 value2 = a5Y;
			if (MovementMode == _6.Floating)
			{
				a5Y.X = MathHelper.Clamp(a5Y.X + result.X, 0f, a5_000E);
				a5Y.Y = 0f;
			}
			else
			{
				Vector2.Add(ref result, ref a5Y, out a5Y);
				float num = a5Y.LengthSquared();
				if (num > a5_000E * a5_000E)
				{
					Vector2.Multiply(ref a5Y, a5_000E / (float)Math.Sqrt(num), out a5Y);
				}
			}
			Vector2.Subtract(ref a5Y, ref value2, out result);
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			float num2 = result.X;
			float num3 = result.Y;
			impulse.X = a5_0001.X * num2 + a5_000F.X * num3;
			impulse.Y = a5_0001.Y * num2 + a5_000F.Y * num3;
			impulse.Z = a5_0001.Z * num2 + a5_000F.Z * num3;
			a5h.Body.ApplyLinearImpulse(ref impulse);
			if (a5r != null && a5r.IsDynamic)
			{
				Vector3.Multiply(ref impulse, 0f - a5W, out impulse);
				num2 *= a5W;
				num3 *= a5W;
				impulse2.X = num2 * a5L.X + num3 * a5z.X;
				impulse2.Y = num2 * a5L.Y + num3 * a5z.Y;
				impulse2.Z = num2 * a5L.Z + num3 * a5z.Z;
				a5r.ApplyLinearImpulse(ref impulse);
				a5r.ApplyAngularImpulse(ref impulse2);
			}
			return Math.Abs(result.X) + Math.Abs(result.Y);
		}
	}
	internal class B
	{
		private h a5h;

		private float a5b = 1f;

		private float a56 = 0.1f;

		private float a5a;

		private l._7<_0004.b> a57 = new l._7<_0004.b>();

		private float a5_0006 = 0.1f;

		public float MaximumStepHeight
		{
			get
			{
				return a5b;
			}
			set
			{
				if (a5b < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5b = value;
			}
		}

		public float MinimumDownStepHeight
		{
			get
			{
				return a56;
			}
			set
			{
				if (a56 < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a56 = value;
			}
		}

		public B(h character)
		{
			a5h = character;
			a5a = _0014.h.AllowedPenetration * 1.1f;
		}

		private bool bL(l._7<_0004.b> P_0)
		{
			for (int i = 0; i < P_0.Count; i++)
			{
				if (bz(ref P_0.Elements[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool bz(ref _0004.b P_0)
		{
			if (a5h.SupportFinder.SideContacts.Count == 0 && P_0.PenetrationDepth > _0014.h.AllowedPenetration)
			{
				return true;
			}
			foreach (_0002 sideContact in a5h.SupportFinder.SideContacts)
			{
				float num = Vector3.Dot(P_0.Normal, sideContact.Contact.Normal);
				float num2 = num * sideContact.Contact.PenetrationDepth;
				if (num2 > sideContact.Contact.PenetrationDepth)
				{
					return true;
				}
			}
			return false;
		}

		public bool TryToStepDown(out Vector3 newPosition)
		{
			if (a5h.SupportFinder.a5b.Count == 0 && a5h.SupportFinder.SupportRayData.HasValue && a5h.SupportFinder.SupportRayData.Value.HasTraction && a5h.SupportFinder.SupportRayData.Value.HitData.T - a5h.SupportFinder.RayLengthToBottom > a56)
			{
				Vector3 normal = a5h.SupportFinder.SupportRayData.Value.HitData.Normal;
				Vector3 down = a5h.Body.OrientationMatrix.Down;
				N._0006 shapeTransform = a5h.Body.CollisionInformation.WorldTransform;
				Ray ray = default(Ray);
				a5h.Body.CollisionInformation.Shape.GetExtremePoint(normal, ref shapeTransform, out ray.Position);
				ray.Direction = down;
				Plane p = new Plane(normal, Vector3.Dot(a5h.SupportFinder.SupportRayData.Value.HitData.Location, normal));
				float num = 0f;
				float num2 = a5h.Body.CollisionInformation.Shape.CollisionMargin + a5h.SupportFinder.SupportRayData.Value.HitData.T - a5h.SupportFinder.RayLengthToBottom;
				float num3 = num2;
				float num5;
				if (global::r.X.GetRayPlaneIntersection(ref ray, ref p, out var num4, out var _))
				{
					num3 = num4 + _0014.h.AllowedPenetration;
					Vector3 vector2 = a5h.Body.Position + down * num3;
					switch (bY(ref vector2, out num5))
					{
					case _7.Accepted:
						num3 += num5;
						if (num3 > a56 && num3 < a5b)
						{
							newPosition = a5h.Body.Position + num3 * down;
							return true;
						}
						newPosition = default(Vector3);
						return false;
					case _7.NoHit:
						num = num3 + num5;
						num3 = (num2 + num3) * 0.5f;
						break;
					case _7.Obstructed:
						num2 = num3;
						num3 = (num + num3) * 0.5f;
						break;
					case _7.TooDeep:
						num3 += num5;
						num2 = num3;
						break;
					}
				}
				int num6 = 0;
				while (num6++ < 5 && num2 - num > 1E-05f)
				{
					Vector3 vector2 = a5h.Body.Position + num3 * down;
					switch (bY(ref vector2, out num5))
					{
					case _7.Accepted:
						num3 += num5;
						if (num3 > a56 && num3 < a5b)
						{
							newPosition = a5h.Body.Position + num3 * down;
							return true;
						}
						newPosition = default(Vector3);
						return false;
					case _7.NoHit:
						num = num3 + num5;
						num3 = (num2 + num) * 0.5f;
						break;
					case _7.Obstructed:
						num2 = num3;
						num3 = (num + num2) * 0.5f;
						break;
					case _7.TooDeep:
						num3 += num5;
						num2 = num3;
						break;
					}
				}
				newPosition = default(Vector3);
				return false;
			}
			newPosition = default(Vector3);
			return false;
		}

		private _7 bY(ref Vector3 P_0, out float P_1)
		{
			P_1 = 0f;
			a5h.QueryManager.QueryContacts(P_0);
			bool flag = bL(a5h.QueryManager.SideContacts);
			if (a5h.QueryManager.br(out var _, out var obj, out var b2) && !flag)
			{
				switch (obj)
				{
				case _7.Accepted:
					P_1 = (0f - Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down)) * b2.PenetrationDepth;
					return _7.Accepted;
				case _7.TooDeep:
					P_1 = Math.Min(0f, 0.001f - Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * b2.PenetrationDepth);
					return _7.TooDeep;
				default:
					P_1 = -0.001f - Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * b2.PenetrationDepth;
					return _7.NoHit;
				}
			}
			if (flag)
			{
				return _7.Obstructed;
			}
			return _7.NoHit;
		}

		public bool TryToStepUp(out Vector3 newPosition)
		{
			if (a5h.SupportFinder.HasTraction)
			{
				a57.Clear();
				bm(a57);
				for (int i = 0; i < a57.Count; i++)
				{
					if (bT(ref a57.Elements[i], out newPosition))
					{
						return true;
					}
				}
			}
			newPosition = default(Vector3);
			return false;
		}

		private void bm(l._7<_0004.b> P_0)
		{
			foreach (_0002 item in a5h.SupportFinder.a56)
			{
				Vector3 vector = new Vector3
				{
					X = a5h.HorizontalMotionConstraint.MovementDirection.X,
					Z = a5h.HorizontalMotionConstraint.MovementDirection.Y
				};
				_0004.b contact = item.Contact;
				Vector3.Dot(ref contact.Normal, ref vector, out var result);
				if (!(result > 0f))
				{
					continue;
				}
				result = Vector3.Dot(a5h.Body.OrientationMatrix.Down, item.Contact.Position - a5h.Body.Position);
				if (!(result < a5h.Body.Height * 0.5f) || !(result > a5h.Body.Height * 0.5f - a5b - a5_0006))
				{
					continue;
				}
				bool flag = true;
				for (int i = 0; i < P_0.Count; i++)
				{
					Vector3.Dot(ref P_0.Elements[i].Normal, ref contact.Normal, out result);
					if (result > 0.99f)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					P_0.Add(contact);
				}
			}
		}

		private bool bT(ref _0004.b P_0, out Vector3 P_1)
		{
			Vector3 vector = a5h.Body.OrientationMatrix.Down;
			Vector3 value = a5h.Body.Position;
			Vector3 vector2 = P_0.Normal;
			Vector3.Dot(ref vector2, ref vector, out var result);
			Vector3.Multiply(ref vector, result, out var result2);
			Vector3.Subtract(ref vector2, ref result2, out vector2);
			vector2.Normalize();
			float height = a5h.Body.Height;
			Ray ray = default(Ray);
			Vector3.Multiply(ref vector, a5h.Body.Height * 0.5f - height, out ray.Position);
			Vector3.Add(ref ray.Position, ref value, out ray.Position);
			ray.Direction = vector2;
			float collisionMargin = a5h.Body.CollisionInformation.Shape.CollisionMargin;
			float num = a5h.Body.Radius + collisionMargin;
			if (a5h.QueryManager.RayCastHitAnything(ray, num))
			{
				P_1 = default(Vector3);
				return false;
			}
			Vector3.Multiply(ref vector2, num, out var result3);
			Vector3.Add(ref ray.Position, ref result3, out ray.Position);
			ray.Direction = vector;
			global::r._0006 earliestHit = default(global::r._0006);
			if (!a5h.QueryManager.RayCast(ray, height, out earliestHit) || earliestHit.T <= 0f || earliestHit.T - height > 0f - a5a || earliestHit.T - height < 0f - a5b - a5_0006)
			{
				P_1 = default(Vector3);
				return false;
			}
			Vector3.Normalize(ref earliestHit.Normal, out var result4);
			Vector3.Dot(ref result4, ref vector, out result);
			if (result < 0f)
			{
				Vector3.Negate(ref result4, out result4);
				result = 0f - result;
			}
			if (result < a5h.SupportFinder.a5B)
			{
				P_1 = default(Vector3);
				return false;
			}
			Vector3.Negate(ref vector, out ray.Direction);
			float length = a5h.Body.Height - earliestHit.T;
			if (a5h.QueryManager.RayCastHitAnything(ray, length))
			{
				P_1 = default(Vector3);
				return false;
			}
			N._0006 shapeTransform = a5h.Body.CollisionInformation.WorldTransform;
			Vector3.Multiply(ref vector2, collisionMargin, out result3);
			Vector3.Add(ref shapeTransform.Position, ref result3, out shapeTransform.Position);
			Vector3.Multiply(ref vector, 0f - height, out var result5);
			Vector3.Add(ref shapeTransform.Position, ref result5, out shapeTransform.Position);
			Ray ray2 = default(Ray);
			a5h.Body.CollisionInformation.Shape.GetExtremePoint(result4, ref shapeTransform, out ray2.Position);
			ray2.Direction = vector;
			Vector3.Dot(ref earliestHit.Location, ref result4, out result);
			Plane p = new Plane(result4, result);
			float num2 = 0f - a5b;
			float num3 = a5h.Body.CollisionInformation.Shape.CollisionMargin - height + earliestHit.T;
			float num4 = num3;
			float num6;
			if (global::r.X.GetRayPlaneIntersection(ref ray2, ref p, out var num5, out var _))
			{
				num5 = 0f - height + num5 + _0014.h.AllowedPenetration;
				if (num5 < num2)
				{
					num5 = num2;
				}
				num4 = num5;
				if (num4 > num3)
				{
					num3 = num4;
				}
				Vector3 vector4 = a5h.Body.Position + vector * num4 + result3;
				switch (bq(ref vector2, ref vector4, out num6))
				{
				case _7.Accepted:
					num4 += num6;
					if (num4 < 0f && num4 > 0f - a5b - _0014.h.AllowedPenetration)
					{
						P_1 = a5h.Body.Position + Math.Max(0f - a5b, num4) * vector + result3;
						return true;
					}
					P_1 = default(Vector3);
					return false;
				case _7.Rejected:
					P_1 = default(Vector3);
					return false;
				case _7.NoHit:
					num2 = num4 + num6;
					num4 = (num3 + num4) * 0.5f;
					break;
				case _7.Obstructed:
					num3 = num4;
					num4 = (num2 + num4) * 0.5f;
					break;
				case _7.HeadObstructed:
					num2 = num4 + num6;
					num4 = (num3 + num4) * 0.5f;
					break;
				case _7.TooDeep:
					num4 += num6;
					num3 = num4;
					break;
				}
			}
			int num7 = 0;
			while (num7++ < 5 && num3 - num2 > 1E-05f)
			{
				Vector3 vector4 = a5h.Body.Position + num4 * vector + result3;
				switch (bq(ref vector2, ref vector4, out num6))
				{
				case _7.Accepted:
					num4 += num6;
					if (num4 < 0f && num4 > 0f - a5b - _0014.h.AllowedPenetration)
					{
						P_1 = a5h.Body.Position + Math.Max(0f - a5b, num4) * vector + result3;
						return true;
					}
					P_1 = default(Vector3);
					return false;
				case _7.Rejected:
					P_1 = default(Vector3);
					return false;
				case _7.NoHit:
					num2 = num4 + num6;
					num4 = (num3 + num2) * 0.5f;
					break;
				case _7.Obstructed:
					num3 = num4;
					num4 = (num2 + num3) * 0.5f;
					break;
				case _7.HeadObstructed:
					num2 = num4 + num6;
					num4 = (num3 + num4) * 0.5f;
					break;
				case _7.TooDeep:
					num4 += num6;
					num3 = num4;
					break;
				}
			}
			P_1 = default(Vector3);
			return false;
		}

		private _7 bq(ref Vector3 P_0, ref Vector3 P_1, out float P_2)
		{
			P_2 = 0f;
			a5h.QueryManager.QueryContacts(P_1);
			if (a5h.QueryManager.HeadContacts.Count > 0)
			{
				Vector3 vector = a5h.Body.OrientationMatrix.Up;
				Vector3.Dot(ref vector, ref a5h.QueryManager.HeadContacts.Elements[0].Normal, out var result);
				P_2 = result * a5h.QueryManager.HeadContacts.Elements[0].PenetrationDepth;
				for (int i = 1; i < a5h.QueryManager.HeadContacts.Count; i++)
				{
					Vector3.Dot(ref vector, ref a5h.QueryManager.HeadContacts.Elements[i].Normal, out result);
					result *= a5h.QueryManager.HeadContacts.Elements[i].PenetrationDepth;
					if (result > P_2)
					{
						P_2 = result;
					}
				}
				return _7.HeadObstructed;
			}
			bool flag = bE(ref P_0, a5h.QueryManager.SideContacts, a5h.QueryManager.HeadContacts);
			if (a5h.QueryManager.br(out var flag2, out var obj, out var b2) && !flag)
			{
				switch (obj)
				{
				case _7.Accepted:
				{
					if (flag2)
					{
						P_2 = Math.Min(0f, Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * (_0014.h.AllowedPenetration * 0.5f - b2.PenetrationDepth));
						return _7.Accepted;
					}
					Vector3 vector2 = a5h.Body.OrientationMatrix.Down;
					Ray ray = default(Ray);
					ray.Position = b2.Position + P_0 * 0.1f * a5h.Body.Radius;
					float num = Vector3.Dot(ray.Position - P_1, vector2);
					num = a5h.Body.Height * 0.5f + num;
					ray.Position -= num * vector2;
					ray.Direction = vector2;
					Ray ray2 = default(Ray);
					ray2.Position = P_1 + a5h.Body.OrientationMatrix.Up * (a5h.Body.Height * 0.5f);
					ray2.Direction = ray.Position - ray2.Position;
					if (!a5h.QueryManager.RayCastHitAnything(ray2, 1f) && a5h.QueryManager.RayCast(ray, a5h.Body.Height, out var earliestHit) && a5h.Body.Height - a5b < earliestHit.T)
					{
						earliestHit.Normal.Normalize();
						Vector3.Dot(ref earliestHit.Normal, ref vector2, out var result2);
						if (Math.Abs(result2) > a5h.SupportFinder.a5B)
						{
							P_2 = Math.Min(0f, Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * (_0014.h.AllowedPenetration * 0.5f - b2.PenetrationDepth));
							ray.Position = P_1;
							if (a5h.QueryManager.RayCast(ray, a5h.Body.Height * 0.75f + a5b, out earliestHit))
							{
								earliestHit.Normal.Normalize();
								Vector3.Dot(ref earliestHit.Normal, ref vector2, out result2);
								if (Math.Abs(result2) > a5h.SupportFinder.a5B)
								{
									return _7.Accepted;
								}
							}
						}
					}
					return _7.Rejected;
				}
				case _7.TooDeep:
					P_2 = Math.Min(0f, Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * (_0014.h.AllowedPenetration * 0.5f - b2.PenetrationDepth));
					return _7.TooDeep;
				default:
					P_2 = -0.001f - Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * b2.PenetrationDepth;
					return _7.NoHit;
				}
			}
			if (flag)
			{
				return _7.Obstructed;
			}
			return _7.NoHit;
		}

		private bool bE(ref Vector3 P_0, l._7<_0004.b> P_1, l._7<_0004.b> P_2)
		{
			for (int i = 0; i < P_1.Count; i++)
			{
				if (b_0013(ref P_0, ref P_1.Elements[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool b_0013(ref Vector3 P_0, ref _0004.b P_1)
		{
			Vector3.Dot(ref P_1.Normal, ref P_0, out var result);
			if (result * P_1.PenetrationDepth > _0014.h.AllowedPenetration)
			{
				return true;
			}
			foreach (_0002 sideContact in a5h.SupportFinder.SideContacts)
			{
				result = Vector3.Dot(P_1.Normal, sideContact.Contact.Normal);
				float num = result * sideContact.Contact.PenetrationDepth;
				if (num > Math.Max(sideContact.Contact.PenetrationDepth, _0014.h.AllowedPenetration))
				{
					return true;
				}
			}
			return false;
		}
	}
}
namespace _0012
{
	internal class b
	{
		internal static void b9(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			if (!(P_0.CollisionMove is CollisionMove { WorldBoundingBox: var box } collisionMove))
			{
				return;
			}
			BoundingSphere worldBoundingSphere = P_1.WorldBoundingSphere;
			float closestPointOnBoxAndDistance = CoreHelper.GetClosestPointOnBoxAndDistance(ref box, ref worldBoundingSphere.Center, out var _);
			if (closestPointOnBoxAndDistance > worldBoundingSphere.Radius * worldBoundingSphere.Radius)
			{
				return;
			}
			Ray ray = new Ray(worldBoundingSphere.Center, -collisionMove.Normal);
			BoundingBox box2 = P_0.WorldBoundingBox;
			box2.Max.X += worldBoundingSphere.Radius;
			box2.Max.Y += worldBoundingSphere.Radius;
			box2.Max.Z += worldBoundingSphere.Radius;
			box2.Min.X -= worldBoundingSphere.Radius;
			box2.Min.Y -= worldBoundingSphere.Radius;
			box2.Min.Z -= worldBoundingSphere.Radius;
			ray.Intersects(ref box2, out var result);
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
					BoundingBox worldBoundingBox = P_0.WorldBoundingBox;
					Vector3 vector = (worldBoundingBox.Max + worldBoundingBox.Min) * 0.5f;
					Vector3 vector2 = vector + collisionMove.Normal * (collisionMove.Distance - value);
					Vector3 surfaceNormal = Vector3.Normalize(vector2 - worldBoundingSphere.Center);
					P_2.ContactTime = MathHelper.Clamp(num, 0f, 1f);
					P_2.ContactPoint = vector2;
					P_2.SurfaceNormal = surfaceNormal;
					P_2.ContactObject = P_1;
					P_2.Material = P_1.DefaultCollisionMaterial;
				}
			}
		}

		internal static void b1(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			if (!(P_0.CollisionMove is CollisionMove { WorldBoundingBox: var worldBoundingBox } collisionMove))
			{
				return;
			}
			BoundingBox box = P_1.WorldBoundingBox;
			worldBoundingBox.Contains(ref box, out var result);
			if (result == ContainmentType.Disjoint)
			{
				return;
			}
			BoundingBox worldBoundingBox2 = P_0.WorldBoundingBox;
			Vector3 vector = (worldBoundingBox2.Max - worldBoundingBox2.Min) * 0.5f;
			box.Max.X += vector.X;
			box.Max.Y += vector.Y;
			box.Max.Z += vector.Z;
			box.Min.X -= vector.X;
			box.Min.Y -= vector.Y;
			box.Min.Z -= vector.Z;
			Vector3 position = (worldBoundingBox2.Max + worldBoundingBox2.Min) * 0.5f;
			new Ray(position, collisionMove.Normal).Intersects(ref box, out var result2);
			if (!result2.HasValue)
			{
				return;
			}
			float value = result2.Value;
			if (!(value < 0f) && !h.bc(P_0, P_1, P_2))
			{
				float num = value / collisionMove.Distance;
				if (!(num >= P_2.ContactTime))
				{
					CoreHelper.GetClosestPointOnBoxAndDistanceWithNormal(ref box, ref position, out var closestpoint, out var surfacenormal);
					P_2.ContactTime = MathHelper.Clamp(num, 0f, 1f);
					P_2.ContactPoint = closestpoint;
					P_2.SurfaceNormal = surfacenormal;
					P_2.ContactObject = P_1;
					P_2.Material = P_1.DefaultCollisionMaterial;
				}
			}
		}

		internal static void bf(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			h.a5h.AccumulationValue++;
			CollisionMove collisionMove = P_0.CollisionMove as CollisionMove;
			CollisionMove collisionMove2 = P_1.CollisionMove as CollisionMove;
			if (collisionMove == null || collisionMove2 == null)
			{
				return;
			}
			BoundingBox worldSweepBoundingBox = collisionMove.WorldSweepBoundingBox;
			CollisionMesh worldCollisionMesh = collisionMove2.WorldCollisionMesh;
			h.a5B.Clear();
			worldCollisionMesh.a5a._0012(ref worldSweepBoundingBox, true, h.a5B);
			if (h.a5B.Count <= 0)
			{
				return;
			}
			BoundingBox worldBoundingBox = collisionMove.WorldBoundingBox;
			BoundingBox box = P_0.WorldBoundingBox;
			BoundingSphere worldBoundingSphere = collisionMove.WorldBoundingSphere;
			float distance = collisionMove.Distance;
			float radius = worldBoundingSphere.Radius;
			float num = 0f - (radius + distance);
			Vector3 vector = (box.Max + box.Min) * 0.5f;
			Vector3 center = worldBoundingSphere.Center;
			Ray ray = new Ray
			{
				Direction = collisionMove.Normal
			};
			foreach (CollisionMesh.CollisionSurface item in h.a5B)
			{
				h.a5b.AccumulationValue++;
				float num2 = center.X * item.Surface.Normal.X + center.Y * item.Surface.Normal.Y + center.Z * item.Surface.Normal.Z + item.Surface.D;
				if (num2 > radius || num2 < num)
				{
					continue;
				}
				worldBoundingBox.Intersects(ref item.Surface, out var result);
				if (result == PlaneIntersectionType.Front)
				{
					continue;
				}
				Vector3.Dot(ref ray.Direction, ref item.Surface.Normal, out var result2);
				result2 *= -1f;
				if (result2 <= 0f || !item.bp(worldCollisionMesh, ref worldBoundingBox))
				{
					continue;
				}
				Vector3 position = vector - item.Surface.Normal * 100000f;
				CoreHelper.GetClosestPointOnBoxAndDistance(ref box, ref position, out ray.Position);
				float? result3 = null;
				ray.Intersects(ref item.Surface, out result3);
				if (result3.HasValue)
				{
					if (h.bc(P_0, P_1, P_2))
					{
						break;
					}
					float value = result3.Value;
					float num3 = value / distance;
					if (!(num3 >= P_2.ContactTime))
					{
						P_2.ContactTime = MathHelper.Clamp(num3, 0f, 1f);
						P_2.ContactPoint = ray.Position + ray.Direction * value;
						P_2.SurfaceNormal = item.Surface.Normal;
						P_2.ContactObject = P_1;
						P_2.Material = item.Material;
					}
				}
			}
		}
	}
}
namespace _0002
{
	internal struct B
	{
		internal D.b a5h;

		internal P.h a5b;

		internal B(P.h P_0, D.b P_1)
		{
			a5h = P_1;
			a5b = P_0;
		}
	}
}
namespace _000E
{
	internal enum B
	{
		BottomLeftUpperRight,
		BottomRightUpperLeft
	}
}
namespace _0017
{
	internal enum b : byte
	{
		Empty,
		Point,
		Segment,
		Triangle,
		Tetrahedron
	}
}
namespace _0004
{
	internal struct b : IEquatable<b>
	{
		public float PenetrationDepth;

		public int Id;

		public Vector3 Normal;

		public Vector3 Position;

		public override string ToString()
		{
			return string.Concat(Position, ", ", Normal);
		}

		public bool Equals(b other)
		{
			if (other.PenetrationDepth == PenetrationDepth && other.Id == Id && other.Normal == Normal)
			{
				return other.Position == Position;
			}
			return false;
		}
	}
}
namespace _0010
{
	internal abstract class b : G.h
	{
		internal new T.b a5h;

		protected E.h entityA;

		protected E.h entityB;

		protected internal D.b pair;

		public T.b MaterialInteraction
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
			}
		}

		public E.h EntityA => entityA;

		public E.h EntityB => entityB;

		public D.b Pair => pair;

		protected internal override void OnInvolvedEntitiesChanged()
		{
			CollectInvolvedEntities();
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

		public abstract void AddContact(_0004.h contact);

		public abstract void RemoveContact(_0004.h contact);

		public virtual void Initialize(E.h a, E.h b, D.b newPair)
		{
			entityA = a;
			entityB = b;
			pair = newPair;
			OnInvolvedEntitiesChanged();
		}

		public abstract void CleanUp();

		protected internal void CleanUpReferences()
		{
			entityA = null;
			entityB = null;
			OnInvolvedEntitiesChanged();
		}

		public override void OnRemovalFromSolver(u.b oldSolver)
		{
			if (pair == null)
			{
				CleanUpReferences();
			}
		}

		public override void UpdateSolverActivity()
		{
			if (isActive)
			{
				isActiveInSolver = pair.BroadPhaseOverlap.a56 < q.a.NoSolver && ((entityA != null && entityA.a5L.IsActive) || (entityB != null && entityB.a5L.IsActive));
				for (int i = 0; i < base.a5h.a5h; i++)
				{
					base.a5h.Elements[i].isActiveInSolver = base.a5h.Elements[i].isActive && isActiveInSolver;
				}
			}
			else
			{
				isActiveInSolver = false;
			}
		}

		public void UpdateMaterialProperties(T._6 materialA, T._6 materialB)
		{
			if (materialA != null && materialB != null)
			{
				T.a.GetInteractionProperties(materialA, materialB, out a5h);
			}
			else if (materialA == null)
			{
				a5h.KineticFriction = materialB.a5h;
				a5h.StaticFriction = materialB.a5b;
				a5h.Bounciness = materialB.a56;
			}
			else if (materialB == null)
			{
				a5h.KineticFriction = materialA.a5h;
				a5h.StaticFriction = materialA.a5b;
				a5h.Bounciness = materialA.a56;
			}
			else
			{
				a5h.KineticFriction = 0f;
				a5h.StaticFriction = 0f;
				a5h.Bounciness = 0f;
			}
		}
	}
	internal class B : L.h
	{
		private new readonly float[] a5h = new float[4];

		private _7 a5b;

		internal float a56;

		private float a5a;

		private float a57;

		private float a5_0006;

		private int a5v;

		private float a5B;

		private E.h a5X;

		private E.h a5_0018;

		private bool a5W;

		private bool a5_0002;

		private float a5_000E;

		public _7 ContactManifoldConstraint => a5b;

		public float TotalTorque => a56;

		public float RelativeVelocity
		{
			get
			{
				float num = 0f;
				if (a5X != null)
				{
					num = a5X.a5_0006.X * a5a + a5X.a5_0006.Y * a57 + a5X.a5_0006.Z * a5_0006;
				}
				if (a5_0018 != null)
				{
					num -= a5_0018.a5_0006.X * a5a + a5_0018.a5_0006.Y * a57 + a5_0018.a5_0006.Z * a5_0006;
				}
				return num;
			}
		}

		public B()
		{
			isActive = false;
		}

		public override float SolveIteration()
		{
			float relativeVelocity = RelativeVelocity;
			relativeVelocity *= a5_000E;
			float num = a56;
			float num2 = 0f;
			for (int i = 0; i < a5v; i++)
			{
				num2 += a5h[i] * a5b.a56.Elements[i].a5b;
			}
			num2 *= a5B;
			a56 = MathHelper.Clamp(a56 + relativeVelocity, 0f - num2, num2);
			relativeVelocity = a56 - num;
			Vector3 impulse = new Vector3
			{
				X = relativeVelocity * a5a,
				Y = relativeVelocity * a57,
				Z = relativeVelocity * a5_0006
			};
			if (a5W)
			{
				a5X.ApplyAngularImpulse(ref impulse);
			}
			if (a5_0002)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				a5_0018.ApplyAngularImpulse(ref impulse);
			}
			return Math.Abs(relativeVelocity);
		}

		public override void Update(float dt)
		{
			Vector3 normal = a5b.a56.Elements[0].a5h.Normal;
			a5a = normal.X;
			a57 = normal.Y;
			a5_0006 = normal.Z;
			float num4;
			if (a5W)
			{
				float num = a5a * a5X.a5_0018.M11 + a57 * a5X.a5_0018.M21 + a5_0006 * a5X.a5_0018.M31;
				float num2 = a5a * a5X.a5_0018.M12 + a57 * a5X.a5_0018.M22 + a5_0006 * a5X.a5_0018.M32;
				float num3 = a5a * a5X.a5_0018.M13 + a57 * a5X.a5_0018.M23 + a5_0006 * a5X.a5_0018.M33;
				num4 = num * a5a + num2 * a57 + num3 * a5_0006 + a5X.a5r;
			}
			else
			{
				num4 = 0f;
			}
			float num5;
			if (a5_0002)
			{
				float num = a5a * a5_0018.a5_0018.M11 + a57 * a5_0018.a5_0018.M21 + a5_0006 * a5_0018.a5_0018.M31;
				float num2 = a5a * a5_0018.a5_0018.M12 + a57 * a5_0018.a5_0018.M22 + a5_0006 * a5_0018.a5_0018.M32;
				float num3 = a5a * a5_0018.a5_0018.M13 + a57 * a5_0018.a5_0018.M23 + a5_0006 * a5_0018.a5_0018.M33;
				num5 = num * a5a + num2 * a57 + num3 * a5_0006 + a5_0018.a5r;
			}
			else
			{
				num5 = 0f;
			}
			a5_000E = -1f / (num4 + num5);
			float relativeVelocity = RelativeVelocity;
			Vector3 vector = a5b.SlidingFriction.a5_0001;
			a5B = ((Math.Abs(relativeVelocity) > _0014.b.StaticFrictionVelocityThreshold || Math.Abs(vector.X) + Math.Abs(vector.Y) + Math.Abs(vector.Z) > _0014.b.StaticFrictionVelocityThreshold) ? ((b)a5b).a5h.KineticFriction : ((b)a5b).a5h.StaticFriction);
			a5B *= _0014.b.TwistFrictionFactor;
			a5v = a5b.a56.a5h;
			for (int i = 0; i < a5v; i++)
			{
				Vector3.Subtract(ref a5b.a56.Elements[i].a5h.Position, ref a5b.SlidingFriction.a5r, out var result);
				a5h[i] = result.Length();
			}
		}

		public override void ExclusiveUpdate()
		{
			Vector3 impulse = new Vector3
			{
				X = a56 * a5a,
				Y = a56 * a57,
				Z = a56 * a5_0006
			};
			if (a5W)
			{
				a5X.ApplyAngularImpulse(ref impulse);
			}
			if (a5_0002)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				a5_0018.ApplyAngularImpulse(ref impulse);
			}
		}

		internal void _6E(_7 P_0)
		{
			a5b = P_0;
			isActive = true;
			a5X = P_0.EntityA;
			a5_0018 = P_0.EntityB;
			a5W = a5X != null && a5X.a5B;
			a5_0002 = a5_0018 != null && a5_0018.a5B;
		}

		internal void bV()
		{
			a56 = 0f;
			a5b = null;
			a5X = null;
			a5_0018 = null;
			isActive = false;
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5X != null)
			{
				outputInvolvedEntities.Add(a5X);
			}
			if (a5_0018 != null)
			{
				outputInvolvedEntities.Add(a5_0018);
			}
		}
	}
}
namespace _0013
{
	internal class b : E.b<s.B<y._6>>
	{
		public float HalfWidth
		{
			get
			{
				return base.CollisionInformation.Shape.HalfWidth;
			}
			set
			{
				base.CollisionInformation.Shape.HalfWidth = value;
			}
		}

		public float HalfHeight
		{
			get
			{
				return base.CollisionInformation.Shape.HalfHeight;
			}
			set
			{
				base.CollisionInformation.Shape.HalfHeight = value;
			}
		}

		public float HalfLength
		{
			get
			{
				return base.CollisionInformation.Shape.HalfLength;
			}
			set
			{
				base.CollisionInformation.Shape.HalfLength = value;
			}
		}

		public float Width
		{
			get
			{
				return base.CollisionInformation.Shape.Width;
			}
			set
			{
				base.CollisionInformation.Shape.Width = value;
			}
		}

		public float Height
		{
			get
			{
				return base.CollisionInformation.Shape.Height;
			}
			set
			{
				base.CollisionInformation.Shape.Height = value;
			}
		}

		public float Length
		{
			get
			{
				return base.CollisionInformation.Shape.Length;
			}
			set
			{
				base.CollisionInformation.Shape.Length = value;
			}
		}

		private b(float P_0, float P_1, float P_2)
			: base(new s.B<y._6>(new y._6(P_0, P_1, P_2)))
		{
		}

		private b(float P_0, float P_1, float P_2, float P_3)
			: base(new s.B<y._6>(new y._6(P_0, P_1, P_2)), P_3)
		{
		}

		public b(Vector3 pos, float width, float height, float length, float mass)
			: this(width, height, length, mass)
		{
			base.Position = pos;
		}

		public b(Vector3 pos, float width, float height, float length)
			: this(width, height, length)
		{
			base.Position = pos;
		}

		public b(n.B motionState, float width, float height, float length, float mass)
			: this(width, height, length, mass)
		{
			base.MotionState = motionState;
		}

		public b(n.B motionState, float width, float height, float length)
			: this(width, height, length)
		{
			base.MotionState = motionState;
		}
	}
}
namespace _0014
{
	internal static class b
	{
		public static float BouncinessVelocityThreshold = 1f;

		public static float MaximumPositionCorrectionSpeed = 2f;

		public static float PenetrationRecoveryStiffness = 0.2f;

		public static float StaticFrictionVelocityThreshold = 0.2f;

		public static float TwistFrictionFactor = 1f;
	}
}
namespace _0001
{
	internal interface B : h, global::r.h
	{
		void Update(float dt);
	}
}
