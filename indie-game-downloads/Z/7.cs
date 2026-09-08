using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using l;

namespace z
{
	internal abstract class _7
	{
		internal bool a5h;

		internal bool a5b;

		protected static Random Random = new Random();

		[CompilerGenerated]
		private Matrix a56;

		[CompilerGenerated]
		private h a5a;

		[CompilerGenerated]
		private b a57;

		[CompilerGenerated]
		private int a5_0006;

		public Matrix WorldTransform
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

		public h BatchInformation
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			private set
			{
				a5a = h2;
			}
		}

		public b Drawer
		{
			[CompilerGenerated]
			get
			{
				return a57;
			}
			[CompilerGenerated]
			private set
			{
				a57 = b2;
			}
		}

		public int TextureIndex
		{
			[CompilerGenerated]
			get
			{
				return a5_0006;
			}
			[CompilerGenerated]
			set
			{
				a5_0006 = value;
			}
		}

		protected _7(b P_0)
		{
			Drawer = P_0;
			BatchInformation = new h();
			TextureIndex = Random.Next(8);
		}

		public void ClearBatchReferences()
		{
			BatchInformation.Batch = null;
			BatchInformation.BaseVertexBufferIndex = 0;
			BatchInformation.BaseIndexBufferIndex = 0;
			BatchInformation.BatchListIndex = 0;
			BatchInformation.VertexCount = 0;
			BatchInformation.IndexCount = 0;
		}

		public void GetVertexData(List<VertexPositionNormalTexture> vertices, List<uint> indices, _0001 batch, ushort baseVertexBufferIndex, int baseIndexBufferIndex, int batchListIndex)
		{
			BatchInformation.Batch = batch;
			BatchInformation.BaseVertexBufferIndex = baseVertexBufferIndex;
			BatchInformation.BaseIndexBufferIndex = baseIndexBufferIndex;
			BatchInformation.BatchListIndex = batchListIndex;
			GetMeshData(vertices, indices);
			for (int i = 0; i < indices.Count; i++)
			{
				indices[i] += baseVertexBufferIndex;
			}
			BatchInformation.VertexCount = vertices.Count;
			BatchInformation.IndexCount = indices.Count;
		}

		public abstract int GetTriangleCountEstimate();

		public abstract void GetMeshData(List<VertexPositionNormalTexture> vertices, List<uint> indices);

		public abstract void Update();
	}
}
namespace Z
{
	internal class _7
	{
		internal l._7<_0006> a5h = new l._7<_0006>(2);

		[CompilerGenerated]
		private h a5b;

		[CompilerGenerated]
		private bool a56;

		[CompilerGenerated]
		private _6 a5a;

		public l._7<_0006> Members => a5h;

		public h Owner
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

		public bool SlatedForRemoval
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			internal set
			{
				a56 = flag;
			}
		}

		public _6 DeactivationManager
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			internal set
			{
				a5a = obj;
			}
		}

		public void AddReferencesToConnectedMembers()
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				a5h.Elements[i]._6D(this);
			}
		}

		public void RemoveReferencesFromConnectedMembers()
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				a5h.Elements[i]._6J(this);
			}
		}

		internal void bV()
		{
			SlatedForRemoval = false;
			a5h.Clear();
			Owner = null;
			DeactivationManager = null;
		}
	}
}
