using System;
using System.Diagnostics;
using _0013;
using E;
using L;
using Microsoft.Xna.Framework;
using l;

namespace h
{
	internal class h
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static uint a5h;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static bool a5b = false;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static a a56;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static string a5a = "";

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static bool a57 = false;

		[DebuggerHidden]
		private static void h()
		{
			if (a56 == null)
			{
				a5h = global::h.b._0006();
				a56 = new a();
			}
		}

		[DebuggerHidden]
		internal static void b(byte[] P_0)
		{
			h();
			if (!a56._0002(P_0))
			{
				throw new Exception("Product not activated, please run activation tool.");
			}
		}

		[DebuggerHidden]
		internal static void _6()
		{
			h();
			if (!a57)
			{
				a57 = true;
				a5b = false;
				a5b = a56.y(global::h._6.a5b[0].FileName, global::h._6.a5b[0].DRMProductName, a5h);
			}
			if (!a5b)
			{
				throw new Exception("Product not activated, please run activation tool.");
			}
		}

		[DebuggerHidden]
		internal static void a()
		{
			h();
			if (!a57)
			{
				a57 = true;
				a5b = false;
				a5b = a56.y(global::h._6.a5b[0].FileName, global::h._6.a5b[0].DRMProductName, a5h);
			}
			if (!a5b)
			{
				throw new Exception("Product not activated, please run activation tool.");
			}
		}

		[DebuggerHidden]
		internal static string _7()
		{
			return global::h._6.ActivationPath + global::h._6.a5b[0].FileName;
		}
	}
}
namespace H
{
	internal abstract class h : L.h
	{
		public static readonly E.h WorldEntity = new _0013._6(Vector3.Zero, 0f);

		protected internal E.h connectionA;

		protected internal E.h connectionB;

		public E.h ConnectionA
		{
			get
			{
				return connectionA;
			}
			set
			{
				connectionA = value ?? WorldEntity;
				OnInvolvedEntitiesChanged();
			}
		}

		public E.h ConnectionB
		{
			get
			{
				return connectionB;
			}
			set
			{
				connectionB = value ?? WorldEntity;
				OnInvolvedEntitiesChanged();
			}
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (connectionA != null && connectionA != WorldEntity)
			{
				outputInvolvedEntities.Add(connectionA);
			}
			if (connectionB != null && connectionB != WorldEntity)
			{
				outputInvolvedEntities.Add(connectionB);
			}
		}
	}
}
