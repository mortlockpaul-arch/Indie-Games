using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _0004;
using _0014;
using D;
using Microsoft.Xna.Framework;
using N;
using P;
using Y;
using d;
using l;
using r;

namespace _000F
{
	internal class _0006
	{
		private h a5h;

		[CompilerGenerated]
		private float a5b;

		[CompilerGenerated]
		private float a56;

		[CompilerGenerated]
		private v a5a;

		[CompilerGenerated]
		private v a57;

		public float StandingHeight
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

		public float CrouchingHeight
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			set
			{
				a56 = value;
			}
		}

		public v CurrentStance
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			private set
			{
				a5a = v2;
			}
		}

		public v DesiredStance
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

		public _0006(h character)
		{
			a5h = character;
			StandingHeight = character.Body.Height;
			CrouchingHeight = StandingHeight * 0.7f;
		}

		public bool UpdateStance(out Vector3 newPosition)
		{
			newPosition = default(Vector3);
			if (CurrentStance != DesiredStance)
			{
				if (CurrentStance == v.Standing && DesiredStance == v.Crouching)
				{
					if (a5h.SupportFinder.HasSupport)
					{
						newPosition = a5h.Body.Position + a5h.Body.OrientationMatrix.Down * ((StandingHeight - CrouchingHeight) * 0.5f);
						a5h.Body.Height = CrouchingHeight;
						CurrentStance = v.Crouching;
					}
					else
					{
						newPosition = a5h.Body.Position;
						a5h.Body.Height = CrouchingHeight;
						CurrentStance = v.Crouching;
					}
					return true;
				}
				if (CurrentStance == v.Crouching && DesiredStance == v.Standing)
				{
					if (a5h.SupportFinder.HasSupport)
					{
						newPosition = a5h.Body.Position - a5h.Body.OrientationMatrix.Down * ((StandingHeight - CrouchingHeight) * 0.5f);
						a5h.QueryManager.QueryContacts(newPosition, v.Standing);
						if (b_000F(a5h.QueryManager.SideContacts, a5h.QueryManager.HeadContacts))
						{
							return false;
						}
						a5h.Body.Height = StandingHeight;
						CurrentStance = v.Standing;
						return true;
					}
					float num = 0f;
					float num2 = (StandingHeight - CrouchingHeight) * 0.5f;
					float num3 = num2;
					float num4 = num2;
					int num5 = 0;
					Vector3 down = a5h.Body.OrientationMatrix.Down;
					while (num5++ < 5 && num2 - num > 1E-05f)
					{
						Vector3 vector = a5h.Body.Position + num3 * down;
						float num6;
						switch (bu(ref vector, out num6))
						{
						case _7.Accepted:
							num3 += num6;
							if (num3 > 0f && num3 < num4)
							{
								newPosition = a5h.Body.Position + num3 * down;
								a5h.Body.Height = StandingHeight;
								CurrentStance = v.Standing;
								return true;
							}
							return false;
						case _7.NoHit:
							num = num3 + num6;
							num3 = (num2 + num) * 0.5f;
							break;
						case _7.Obstructed:
							num2 = num3;
							num3 = (num + num2) * 0.5f;
							break;
						case _7.TooDeep:
							num3 += num6;
							num2 = num3;
							break;
						}
					}
					newPosition = a5h.Body.Position;
					a5h.Body.Height = StandingHeight;
					CurrentStance = v.Standing;
					return true;
				}
			}
			return false;
		}

		private bool b_000F(l._7<_0004.b> P_0, l._7<_0004.b> P_1)
		{
			if (P_1.Count > 0)
			{
				return true;
			}
			for (int i = 0; i < P_0.Count; i++)
			{
				if (bZ(ref P_0.Elements[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool bZ(ref _0004.b P_0)
		{
			if (a5h.SupportFinder.SideContacts.Count == 0 && P_0.PenetrationDepth > _0014.h.AllowedPenetration)
			{
				return true;
			}
			foreach (_0002 sideContact in a5h.SupportFinder.SideContacts)
			{
				float num = Vector3.Dot(P_0.Normal, sideContact.Contact.Normal);
				float num2 = num * sideContact.Contact.PenetrationDepth;
				if (num2 > Math.Max(sideContact.Contact.PenetrationDepth, _0014.h.AllowedPenetration))
				{
					return true;
				}
			}
			return false;
		}

		private _7 bu(ref Vector3 P_0, out float P_1)
		{
			P_1 = 0f;
			a5h.QueryManager.QueryContacts(P_0, v.Standing);
			bool flag = b_000F(a5h.QueryManager.SideContacts, a5h.QueryManager.HeadContacts);
			if (a5h.QueryManager.br(out var _, out var obj, out var b2) && !flag)
			{
				switch (obj)
				{
				case _7.Accepted:
					P_1 = Math.Min(0f, Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * (_0014.h.AllowedPenetration * 0.5f - b2.PenetrationDepth));
					return _7.Accepted;
				case _7.TooDeep:
					P_1 = Math.Min(0f, Vector3.Dot(b2.Normal, a5h.Body.OrientationMatrix.Down) * (_0014.h.AllowedPenetration * 0.5f - b2.PenetrationDepth));
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
	}
	internal struct _0018(l._7<W> supports) : IEnumerable<_0004.b>, IEnumerable
	{
		public struct _00065h(l._7<W> supports) : IEnumerator<_0004.b>, IDisposable, IEnumerator
		{
			private int a5h = -1;

			private l._7<W> a5b = supports;

			public _0004.b Current => a5b.Elements[a5h].Contact;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				while (++a5h < a5b.Count)
				{
					if (a5b.Elements[a5h].HasTraction)
					{
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				a5h = -1;
			}
		}

		private l._7<W> a5h = supports;

		public _00065h GetEnumerator()
		{
			return new _00065h(a5h);
		}

		IEnumerator<_0004.b> IEnumerable<_0004.b>.GetEnumerator()
		{
			return new _00065h(a5h);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new _00065h(a5h);
		}
	}
	internal struct _0002
	{
		public _0004.b Contact;

		public P.h Collidable;
	}
	internal struct _000E
	{
		public global::r._0006 HitData;

		public P.h HitObject;

		public bool HasTraction;
	}
}
namespace _0002
{
	internal struct _0006
	{
		internal Y.a a5h;

		internal _0006(Y.a P_0)
		{
			a5h = P_0;
		}
	}
	internal struct _0018
	{
		internal D.b a5h;

		internal P.h a5b;

		internal _0018(P.h P_0, D.b P_1)
		{
			a5b = P_0;
			a5h = P_1;
		}
	}
	internal struct _0002
	{
		internal D.b a5h;

		internal P.h a5b;

		internal _0002(P.h P_0, D.b P_1)
		{
			a5b = P_0;
			a5h = P_1;
		}
	}
	internal interface _000E : h
	{
		void OnCollisionEnded(P.h other, D.b collisionPair);

		void OnPairTouching(P.h other, D.b collisionPair);

		void OnContactCreated(P.h other, D.b collisionPair, _0004.h contact);

		void OnContactRemoved(P.h other, D.b collisionPair, _0004.h contact);

		void OnInitialCollisionDetected(P.h other, D.b collisionPair);
	}
}
namespace _000E
{
	internal class _0006 : h
	{
		private l._0002 a5h;

		public l._0002 TriangleMeshData => a5h;

		public _0006(Vector3[] vertices, int[] indices, N.h worldTransform)
		{
			a5h = new l._0002(vertices, indices, worldTransform);
		}

		public _0006(Vector3[] vertices, int[] indices)
		{
			a5h = new l._0002(vertices, indices);
		}
	}
}
namespace _0017
{
	internal struct _0006
	{
		public Vector3 A;

		public Vector3 B;

		public Vector3 C;

		public Vector3 D;

		public b State;

		public bool GetPointClosestToOrigin(ref _0006 simplex, out Vector3 point)
		{
			switch (State)
			{
			case b.Point:
				point = A;
				break;
			case b.Segment:
				GetPointOnSegmentClosestToOrigin(ref simplex, out point);
				break;
			case b.Triangle:
				GetPointOnTriangleClosestToOrigin(ref simplex, out point);
				break;
			case b.Tetrahedron:
				return GetPointOnTetrahedronClosestToOrigin(ref simplex, out point);
			default:
				point = r.X.ZeroVector;
				break;
			}
			return false;
		}

		public void GetPointOnSegmentClosestToOrigin(ref _0006 simplex, out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Dot(ref result, ref A, out var result2);
			if (result2 > 0f)
			{
				simplex.State = b.Point;
				point = A;
				return;
			}
			Vector3.Dot(ref result, ref B, out var result3);
			if (result3 > 0f)
			{
				float scaleFactor = (0f - result2) / result.LengthSquared();
				Vector3.Multiply(ref result, scaleFactor, out point);
				Vector3.Add(ref point, ref A, out point);
			}
			else
			{
				simplex.A = simplex.B;
				simplex.State = b.Point;
				point = A;
			}
		}

		public void GetPointOnTriangleClosestToOrigin(ref _0006 simplex, out Vector3 point)
		{
			Vector3.Subtract(ref B, ref A, out var result);
			Vector3.Subtract(ref C, ref A, out var result2);
			Vector3.Dot(ref result, ref A, out var result3);
			Vector3.Dot(ref result2, ref A, out var result4);
			result3 = 0f - result3;
			result4 = 0f - result4;
			if (result4 <= 0f && result3 <= 0f)
			{
				simplex.State = b.Point;
				point = A;
				return;
			}
			Vector3.Dot(ref result, ref B, out var result5);
			Vector3.Dot(ref result2, ref B, out var result6);
			result5 = 0f - result5;
			result6 = 0f - result6;
			if (result5 >= 0f && result6 <= result5)
			{
				simplex.State = b.Point;
				simplex.A = simplex.B;
				point = B;
				return;
			}
			float num = result3 * result6 - result5 * result4;
			if (num <= 0f && result3 > 0f && result5 < 0f)
			{
				simplex.State = b.Segment;
				float scaleFactor = result3 / (result3 - result5);
				Vector3.Multiply(ref result, scaleFactor, out point);
				Vector3.Add(ref point, ref A, out point);
				return;
			}
			Vector3.Dot(ref result, ref C, out var result7);
			Vector3.Dot(ref result2, ref C, out var result8);
			result7 = 0f - result7;
			result8 = 0f - result8;
			if (result8 >= 0f && result7 <= result8)
			{
				simplex.State = b.Point;
				simplex.A = simplex.C;
				point = A;
				return;
			}
			float num2 = result7 * result4 - result3 * result8;
			if (num2 <= 0f && result4 > 0f && result8 < 0f)
			{
				simplex.State = b.Segment;
				simplex.B = simplex.C;
				float scaleFactor2 = result4 / (result4 - result8);
				Vector3.Multiply(ref result2, scaleFactor2, out point);
				Vector3.Add(ref point, ref A, out point);
				return;
			}
			float num3 = result5 * result8 - result7 * result6;
			float num4;
			float num5;
			if (num3 <= 0f && (num4 = result6 - result5) > 0f && (num5 = result7 - result8) > 0f)
			{
				simplex.State = b.Segment;
				simplex.A = simplex.C;
				float scaleFactor3 = num4 / (num4 + num5);
				Vector3.Subtract(ref C, ref B, out var result9);
				Vector3.Multiply(ref result9, scaleFactor3, out point);
				Vector3.Add(ref point, ref B, out point);
			}
			else
			{
				float num6 = 1f / (num3 + num2 + num);
				float scaleFactor4 = num2 * num6;
				float scaleFactor5 = num * num6;
				Vector3.Multiply(ref result, scaleFactor4, out point);
				Vector3.Multiply(ref result2, scaleFactor5, out var result10);
				Vector3.Add(ref A, ref point, out point);
				Vector3.Add(ref point, ref result10, out point);
			}
		}

		public bool GetPointOnTetrahedronClosestToOrigin(ref _0006 simplex, out Vector3 point)
		{
			_0006 obj = default(_0006);
			point = default(Vector3);
			float num = float.MaxValue;
			if (_6X(ref A, ref C, ref D, ref simplex.A, ref simplex.C, ref simplex.D, ref B, out var obj2, out var vector))
			{
				point = vector;
				obj = obj2;
				num = vector.LengthSquared();
			}
			float num2;
			if (_6X(ref C, ref B, ref D, ref simplex.C, ref simplex.B, ref simplex.D, ref A, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (_6X(ref B, ref A, ref D, ref simplex.B, ref simplex.A, ref simplex.D, ref C, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (_6X(ref A, ref B, ref C, ref simplex.A, ref simplex.B, ref simplex.C, ref D, out obj2, out vector) && (num2 = vector.LengthSquared()) < num)
			{
				point = vector;
				obj = obj2;
				num = num2;
			}
			if (num < float.MaxValue)
			{
				simplex = obj;
				return false;
			}
			return true;
		}

		private static bool _6X(ref Vector3 P_0, ref Vector3 P_1, ref Vector3 P_2, ref Vector3 P_3, ref Vector3 P_4, ref Vector3 P_5, ref Vector3 P_6, out _0006 P_7, out Vector3 P_8)
		{
			P_7 = default(_0006);
			P_8 = default(Vector3);
			Vector3.Subtract(ref P_1, ref P_0, out var result);
			Vector3.Subtract(ref P_2, ref P_0, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			Vector3.Subtract(ref P_6, ref P_0, out var result4);
			Vector3.Dot(ref P_0, ref result3, out var result5);
			Vector3.Dot(ref result4, ref result3, out var result6);
			if (result5 * result6 >= 0f)
			{
				Vector3.Dot(ref result, ref P_0, out var result7);
				Vector3.Dot(ref result2, ref P_0, out var result8);
				result7 = 0f - result7;
				result8 = 0f - result8;
				if (result8 <= 0f && result7 <= 0f)
				{
					P_7.State = b.Point;
					P_7.A = P_3;
					P_8 = P_0;
					return true;
				}
				Vector3.Dot(ref result, ref P_1, out var result9);
				Vector3.Dot(ref result2, ref P_1, out var result10);
				result9 = 0f - result9;
				result10 = 0f - result10;
				if (result9 >= 0f && result10 <= result9)
				{
					P_7.State = b.Point;
					P_7.A = P_4;
					P_8 = P_1;
					return true;
				}
				float num = result7 * result10 - result9 * result8;
				if (num <= 0f && result7 > 0f && result9 < 0f)
				{
					P_7.State = b.Segment;
					P_7.A = P_3;
					P_7.B = P_4;
					float scaleFactor = result7 / (result7 - result9);
					Vector3.Multiply(ref result, scaleFactor, out P_8);
					Vector3.Add(ref P_8, ref P_0, out P_8);
					return true;
				}
				Vector3.Dot(ref result, ref P_2, out var result11);
				Vector3.Dot(ref result2, ref P_2, out var result12);
				result11 = 0f - result11;
				result12 = 0f - result12;
				if (result12 >= 0f && result11 <= result12)
				{
					P_7.State = b.Point;
					P_7.A = P_5;
					P_8 = P_2;
					return true;
				}
				float num2 = result11 * result8 - result7 * result12;
				if (num2 <= 0f && result8 > 0f && result12 < 0f)
				{
					P_7.State = b.Segment;
					P_7.A = P_3;
					P_7.B = P_5;
					float scaleFactor2 = result8 / (result8 - result12);
					Vector3.Multiply(ref result2, scaleFactor2, out P_8);
					Vector3.Add(ref P_8, ref P_0, out P_8);
					return true;
				}
				float num3 = result9 * result12 - result11 * result10;
				float num4;
				float num5;
				if (num3 <= 0f && (num4 = result10 - result9) > 0f && (num5 = result11 - result12) > 0f)
				{
					P_7.State = b.Segment;
					P_7.A = P_4;
					P_7.B = P_5;
					float scaleFactor3 = num4 / (num4 + num5);
					Vector3.Subtract(ref P_2, ref P_1, out var result13);
					Vector3.Multiply(ref result13, scaleFactor3, out P_8);
					Vector3.Add(ref P_8, ref P_1, out P_8);
					return true;
				}
				P_7.State = b.Triangle;
				P_7.A = P_3;
				P_7.B = P_4;
				P_7.C = P_5;
				float num6 = 1f / (num3 + num2 + num);
				float scaleFactor4 = num * num6;
				float scaleFactor5 = num2 * num6;
				Vector3.Multiply(ref result, scaleFactor5, out P_8);
				Vector3.Multiply(ref result2, scaleFactor4, out var result14);
				Vector3.Add(ref P_0, ref P_8, out P_8);
				Vector3.Add(ref P_8, ref result14, out P_8);
				return true;
			}
			return false;
		}

		public void AddNewSimplexPoint(ref Vector3 point, ref Vector3 hitLocation, out _0006 shiftedSimplex)
		{
			shiftedSimplex = default(_0006);
			switch (State)
			{
			case b.Empty:
				State = b.Point;
				A = point;
				Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
				break;
			case b.Point:
				State = b.Segment;
				B = point;
				Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
				Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
				break;
			case b.Segment:
				State = b.Triangle;
				C = point;
				Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
				Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
				Vector3.Subtract(ref hitLocation, ref C, out shiftedSimplex.C);
				break;
			case b.Triangle:
				State = b.Tetrahedron;
				D = point;
				Vector3.Subtract(ref hitLocation, ref A, out shiftedSimplex.A);
				Vector3.Subtract(ref hitLocation, ref B, out shiftedSimplex.B);
				Vector3.Subtract(ref hitLocation, ref C, out shiftedSimplex.C);
				Vector3.Subtract(ref hitLocation, ref D, out shiftedSimplex.D);
				break;
			}
			shiftedSimplex.State = State;
		}

		public float GetErrorTolerance(ref Vector3 rayOrigin)
		{
			float result;
			float result2;
			float result3;
			switch (State)
			{
			case b.Point:
				Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
				return result;
			case b.Segment:
				Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
				Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
				return MathHelper.Max(result, result2);
			case b.Triangle:
				Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
				Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
				Vector3.DistanceSquared(ref C, ref rayOrigin, out result3);
				return MathHelper.Max(result, MathHelper.Max(result2, result3));
			case b.Tetrahedron:
			{
				Vector3.DistanceSquared(ref A, ref rayOrigin, out result);
				Vector3.DistanceSquared(ref B, ref rayOrigin, out result2);
				Vector3.DistanceSquared(ref C, ref rayOrigin, out result3);
				Vector3.DistanceSquared(ref D, ref rayOrigin, out var result4);
				return MathHelper.Max(result, MathHelper.Max(result2, MathHelper.Max(result3, result4)));
			}
			default:
				return 0f;
			}
		}
	}
}
namespace _0004
{
	internal enum _0006
	{
		A,
		B,
		C,
		AB,
		AC,
		BC,
		ABC
	}
}
namespace _0010
{
	internal class _0006 : b
	{
		internal new l._7<a> a5h;

		private Stack<a> a5b = new Stack<a>(4);

		internal l._7<h> a56;

		private Stack<h> a5a = new Stack<h>(4);

		public l.X<a> ContactPenetrationConstraints => new l.X<a>(a5h);

		public l.X<h> ContactFrictionConstraints => new l.X<h>(a56);

		public _0006()
		{
			a5h = new l._7<a>(4);
			a56 = new l._7<h>(4);
			for (int i = 0; i < 4; i++)
			{
				a a2 = new a();
				a5b.Push(a2);
				Add(a2);
				h h2 = new h();
				a5a.Push(h2);
				Add(h2);
			}
		}

		public override void CleanUp()
		{
			for (int num = a5h.a5h - 1; num >= 0; num--)
			{
				a a2 = a5h.Elements[num];
				a2.CleanUp();
				a5h.RemoveAt(num);
				a5b.Push(a2);
			}
			for (int num2 = a56.a5h - 1; num2 >= 0; num2--)
			{
				h h2 = a56.Elements[num2];
				h2.CleanUp();
				a56.RemoveAt(num2);
				a5a.Push(h2);
			}
		}

		public override void AddContact(_0004.h contact)
		{
			a a2 = a5b.Pop();
			a2.Setup(this, contact);
			a5h.Add(a2);
			h h2 = a5a.Pop();
			h2.Setup(this, a2);
			a56.Add(h2);
		}

		public override void RemoveContact(_0004.h contact)
		{
			a a2 = null;
			for (int i = 0; i < a5h.a5h; i++)
			{
				if ((a2 = a5h.Elements[i]).a5h == contact)
				{
					a2.CleanUp();
					a5h.RemoveAt(i);
					a5b.Push(a2);
					break;
				}
			}
			for (int num = a56.a5h - 1; num >= 0; num--)
			{
				h h2 = a56[num];
				if (h2.PenetrationConstraint == a2)
				{
					h2.CleanUp();
					a56.RemoveAt(num);
					a5a.Push(h2);
					break;
				}
			}
		}

		public sealed override void Update(float dt)
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				UpdateUpdateable(a5h.Elements[i], dt);
			}
			for (int j = 0; j < a56.a5h; j++)
			{
				UpdateUpdateable(a56.Elements[j], dt);
			}
		}

		public sealed override void ExclusiveUpdate()
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				ExclusiveUpdateUpdateable(a5h.Elements[i]);
			}
			for (int j = 0; j < a56.a5h; j++)
			{
				ExclusiveUpdateUpdateable(a56.Elements[j]);
			}
		}

		public sealed override float SolveIteration()
		{
			int activeConstraints = 0;
			for (int i = 0; i < a5h.a5h; i++)
			{
				SolveUpdateable(a5h.Elements[i], ref activeConstraints);
			}
			for (int j = 0; j < a56.a5h; j++)
			{
				SolveUpdateable(a56.Elements[j], ref activeConstraints);
			}
			isActiveInSolver = activeConstraints > 0;
			return solverSettings.a5a + 1f;
		}
	}
}
namespace _0001
{
	internal interface _0006 : h, global::r.h
	{
		void Update(float dt);
	}
	internal abstract class _0018<T> : X where T : class, h
	{
		protected List<T> sequentiallyUpdatedUpdateables = new List<T>();

		protected List<T> simultaneouslyUpdatedUpdateables = new List<T>();

		protected _0018(global::r.B P_0)
			: base(P_0)
		{
			multithreadedUpdateDelegate = MultithreadedUpdate;
		}

		protected _0018(global::r.B P_0, d.b P_1)
			: base(P_0, P_1)
		{
			multithreadedUpdateDelegate = MultithreadedUpdate;
		}

		protected abstract void MultithreadedUpdate(int i);

		protected abstract void SequentialUpdate(int i);

		public override void SequentialUpdatingStateChanged(h updateable)
		{
			if (updateable.Managers.Contains(this))
			{
				T item = updateable as T;
				if (updateable.IsUpdatedSequentially)
				{
					if (simultaneouslyUpdatedUpdateables.Remove(item))
					{
						sequentiallyUpdatedUpdateables.Add(item);
					}
				}
				else if (sequentiallyUpdatedUpdateables.Remove(item))
				{
					simultaneouslyUpdatedUpdateables.Add(item);
				}
				return;
			}
			throw new Exception("Updateable does not belong to this manager.");
		}

		public void Add(T updateable)
		{
			if (!updateable.Managers.Contains(this))
			{
				if (updateable.IsUpdatedSequentially)
				{
					sequentiallyUpdatedUpdateables.Add(updateable);
				}
				else
				{
					simultaneouslyUpdatedUpdateables.Add(updateable);
				}
				updateable.Managers.Add(this);
				return;
			}
			throw new Exception("Updateable already belongs to the manager, cannot re-add.");
		}

		public void Remove(T updateable)
		{
			if (updateable.Managers.Contains(this))
			{
				if (updateable.IsUpdatedSequentially)
				{
					sequentiallyUpdatedUpdateables.Remove(updateable);
				}
				else
				{
					simultaneouslyUpdatedUpdateables.Remove(updateable);
				}
				updateable.Managers.Remove(this);
				return;
			}
			throw new Exception("Updateable does not belong to this manager; cannot remove.");
		}

		protected override void UpdateMultithreaded()
		{
			for (int i = 0; i < sequentiallyUpdatedUpdateables.Count; i++)
			{
				SequentialUpdate(i);
			}
			base.ThreadManager.ForLoop(0, simultaneouslyUpdatedUpdateables.Count, multithreadedUpdateDelegate);
		}

		protected override void UpdateSingleThreaded()
		{
			for (int i = 0; i < sequentiallyUpdatedUpdateables.Count; i++)
			{
				SequentialUpdate(i);
			}
			for (int j = 0; j < simultaneouslyUpdatedUpdateables.Count; j++)
			{
				MultithreadedUpdate(j);
			}
		}
	}
	internal class _0002 : global::_0001._0018<_7>
	{
		public _0002(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public _0002(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
	internal class _000E : global::_0001._0018<_6>
	{
		public _000E(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public _000E(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
	internal class _0001 : global::_0001._0018<a>
	{
		public _0001(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public _0001(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
}
