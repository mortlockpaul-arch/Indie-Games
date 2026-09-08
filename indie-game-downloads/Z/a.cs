using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace z
{
	internal class a : IDisposable
	{
		internal _7 a5h;

		internal _6 a5b;

		internal IndexBuffer a56;

		internal int a5a;

		internal int a57;

		internal VertexBuffer a5_0006;

		private a(_6 P_0, _7 P_1)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			a5b = P_0;
			a5h = P_1;
			List<VertexPositionNormalTexture> list = new List<VertexPositionNormalTexture>();
			List<uint> list2 = new List<uint>();
			P_1.GetMeshData(list, list2);
			VertexPositionNormalTexture[] array = new VertexPositionNormalTexture[list.Count];
			uint[] array2 = new uint[list2.Count];
			list.CopyTo(array);
			list2.CopyTo(array2);
			a5_0006 = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), array.Length, BufferUsage.WriteOnly);
			a5_0006.SetData(array);
			a56 = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, array2.Length, BufferUsage.WriteOnly);
			a56.SetData(array2);
			a5a = array.Length;
			a57 = array2.Length;
		}

		internal static a b_0004(_6 P_0, _7 P_1)
		{
			return new a(P_0, P_1);
		}

		public void Dispose()
		{
			a56.Dispose();
			a5_0006.Dispose();
		}

		internal void bi(GraphicsDevice P_0, BasicEffect P_1, EffectPass P_2, RasterizerState P_3, RasterizerState P_4)
		{
			P_1.World = a5h.WorldTransform;
			P_0.SetVertexBuffer(a5_0006);
			P_0.Indices = a56;
			P_2.Apply();
			P_0.RasterizerState = P_3;
			P_0.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, a5a, 0, a57 / 3);
			P_0.RasterizerState = P_4;
			P_0.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, a5a, 0, a57 / 3);
		}
	}
}
namespace Z
{
	internal class a
	{
		internal a a5h;

		internal bool a5b = true;

		internal bool a56 = true;

		internal int a5a;

		internal int a57;

		private Action<_0006> a5_0006;

		private Action<_0006> a5v;

		private Action<_0006> a5B;

		internal a Parent
		{
			get
			{
				if (a5h != this)
				{
					return a5h.Parent;
				}
				return this;
			}
		}

		public bool IsActive
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

		public a()
		{
			a5_0006 = _6_0004;
			a5v = _6i;
			a5B = _6_0010;
			bV();
		}

		private void _6_0004(_0006 P_0)
		{
			Activate();
		}

		private void _6i(_0006 P_0)
		{
			Interlocked.Increment(ref a57);
		}

		private void _6_0010(_0006 P_0)
		{
			Interlocked.Decrement(ref a57);
		}

		public void Activate()
		{
			if (!a56)
			{
				a56 = true;
			}
		}

		public bool TryToDeactivate()
		{
			if (a5b)
			{
				if (a56 && a57 == a5a)
				{
					a56 = false;
					return true;
				}
				return false;
			}
			a5b = true;
			return false;
		}

		public void Add(_0006 member)
		{
			if (member.IsDynamic && member.a5y == null)
			{
				member.a5y = this;
				a5a++;
				member.Activated += a5_0006;
				member.BecameDeactivationCandidate += a5v;
				member.BecameNonDeactivationCandidate += a5B;
				if (member.IsDeactivationCandidate)
				{
					a57++;
				}
				return;
			}
			throw new Exception("Member either is not dynamic or already has a simulation island; cannot add.");
		}

		public void Remove(_0006 member)
		{
			if (member.a5y == this)
			{
				a5a--;
				member.a5y = null;
				member.Activated -= a5_0006;
				member.BecameDeactivationCandidate -= a5v;
				member.BecameNonDeactivationCandidate -= a5B;
				if (member.IsDeactivationCandidate)
				{
					a57--;
				}
				return;
			}
			throw new Exception("Member does not belong to island; cannot remove.");
		}

		internal void bV()
		{
			a56 = true;
			a57 = 0;
			a5a = 0;
			a5h = this;
		}
	}
}
