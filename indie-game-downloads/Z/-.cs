using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using E;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using N;
using d;
using l;
using s;
using y;

namespace z
{
	internal abstract class _0006<T> : _7
	{
		[CompilerGenerated]
		private new T a5h;

		public T DisplayedObject
		{
			[CompilerGenerated]
			get
			{
				return a5h;
			}
			[CompilerGenerated]
			private set
			{
				a5h = val;
			}
		}

		protected _0006(b P_0, T P_1)
			: base(P_0)
		{
			DisplayedObject = P_1;
		}
	}
	internal static class _0018
	{
		public static void GetShapeMeshData(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices)
		{
			if (!(collidable.Shape is global::y._6 obj))
			{
				throw new ArgumentException("Wrong shape type.");
			}
			Vector3[] corners = new BoundingBox(new Vector3(0f - obj.HalfWidth, 0f - obj.HalfHeight, 0f - obj.HalfLength), new Vector3(obj.HalfWidth, obj.HalfHeight, obj.HalfLength)).GetCorners();
			Vector2[] array = new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			vertices.Add(new VertexPositionNormalTexture(corners[0], Vector3.Backward, array[0]));
			vertices.Add(new VertexPositionNormalTexture(corners[1], Vector3.Backward, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[2], Vector3.Backward, array[2]));
			vertices.Add(new VertexPositionNormalTexture(corners[3], Vector3.Backward, array[3]));
			indices.Add(0u);
			indices.Add(1u);
			indices.Add(2u);
			indices.Add(0u);
			indices.Add(2u);
			indices.Add(3u);
			vertices.Add(new VertexPositionNormalTexture(corners[1], Vector3.Right, array[0]));
			vertices.Add(new VertexPositionNormalTexture(corners[2], Vector3.Right, array[3]));
			vertices.Add(new VertexPositionNormalTexture(corners[5], Vector3.Right, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[6], Vector3.Right, array[2]));
			indices.Add(4u);
			indices.Add(6u);
			indices.Add(7u);
			indices.Add(4u);
			indices.Add(7u);
			indices.Add(5u);
			vertices.Add(new VertexPositionNormalTexture(corners[4], Vector3.Forward, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[5], Vector3.Forward, array[0]));
			vertices.Add(new VertexPositionNormalTexture(corners[6], Vector3.Forward, array[3]));
			vertices.Add(new VertexPositionNormalTexture(corners[7], Vector3.Forward, array[2]));
			indices.Add(9u);
			indices.Add(8u);
			indices.Add(11u);
			indices.Add(9u);
			indices.Add(11u);
			indices.Add(10u);
			vertices.Add(new VertexPositionNormalTexture(corners[0], Vector3.Left, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[3], Vector3.Left, array[2]));
			vertices.Add(new VertexPositionNormalTexture(corners[4], Vector3.Left, array[0]));
			vertices.Add(new VertexPositionNormalTexture(corners[7], Vector3.Left, array[3]));
			indices.Add(14u);
			indices.Add(12u);
			indices.Add(13u);
			indices.Add(14u);
			indices.Add(13u);
			indices.Add(15u);
			vertices.Add(new VertexPositionNormalTexture(corners[0], Vector3.Up, array[2]));
			vertices.Add(new VertexPositionNormalTexture(corners[1], Vector3.Up, array[3]));
			vertices.Add(new VertexPositionNormalTexture(corners[4], Vector3.Up, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[5], Vector3.Up, array[0]));
			indices.Add(16u);
			indices.Add(19u);
			indices.Add(17u);
			indices.Add(16u);
			indices.Add(18u);
			indices.Add(19u);
			vertices.Add(new VertexPositionNormalTexture(corners[2], Vector3.Down, array[1]));
			vertices.Add(new VertexPositionNormalTexture(corners[3], Vector3.Down, array[0]));
			vertices.Add(new VertexPositionNormalTexture(corners[6], Vector3.Down, array[2]));
			vertices.Add(new VertexPositionNormalTexture(corners[7], Vector3.Down, array[3]));
			indices.Add(21u);
			indices.Add(20u);
			indices.Add(22u);
			indices.Add(21u);
			indices.Add(22u);
			indices.Add(23u);
		}
	}
	internal static class _0002
	{
		public static int NumSides = 24;

		public static void GetShapeMeshData(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices)
		{
			if (!(collidable.Shape is global::y.b b2))
			{
				throw new ArgumentException("Wrong shape type.");
			}
			float num = b2.Height / 2f;
			float num2 = (float)Math.PI * 2f / (float)NumSides;
			float radius = b2.Radius;
			for (int i = 0; i < NumSides; i++)
			{
				float num3 = (float)i * num2;
				float num4 = (float)Math.Cos(num3) * radius;
				float z = (float)Math.Sin(num3) * radius;
				vertices.Add(new VertexPositionNormalTexture(new Vector3(num4, num, z), Vector3.Up, Vector2.Zero));
				vertices.Add(new VertexPositionNormalTexture(new Vector3(num4, num, z), new Vector3(num4, 0f, z), Vector2.Zero));
				vertices.Add(new VertexPositionNormalTexture(new Vector3(num4, 0f - num, z), new Vector3(num4, 0f, z), Vector2.Zero));
				vertices.Add(new VertexPositionNormalTexture(new Vector3(num4, 0f - num, z), Vector3.Down, Vector2.Zero));
			}
			for (uint num5 = 0u; num5 < vertices.Count; num5 += 4)
			{
				uint num6 = (uint)((num5 + 4) % vertices.Count);
				if (num6 != 0)
				{
					indices.Add(num5);
					indices.Add(num6);
					indices.Add(0u);
				}
				num6 = (uint)((num5 + 5) % vertices.Count);
				indices.Add(num5 + 1);
				indices.Add(num5 + 2);
				indices.Add(num6);
				indices.Add(num6);
				indices.Add(num5 + 2);
				indices.Add((uint)((num5 + 6) % vertices.Count));
				num6 = (uint)((num5 + 7) % vertices.Count);
				if (num6 != 3)
				{
					indices.Add(num5 + 3);
					indices.Add(3u);
					indices.Add(num6);
				}
			}
		}
	}
	internal class _000E : z._0006<E.h>
	{
		public _000E(b drawer, E.h entity)
			: base(drawer, entity)
		{
		}

		public override int GetTriangleCountEstimate()
		{
			return 100;
		}

		public override void GetMeshData(List<VertexPositionNormalTexture> vertices, List<uint> indices)
		{
			b.ShapeMeshGetters[base.DisplayedObject.CollisionInformation.GetType()](base.DisplayedObject.CollisionInformation, vertices, indices);
		}

		public override void Update()
		{
			Vector3 translation = N._7.Transform(base.DisplayedObject.CollisionInformation.LocalPosition, base.DisplayedObject.BufferedStates.InterpolatedStates.OrientationMatrix);
			translation += base.DisplayedObject.BufferedStates.InterpolatedStates.Position;
			Matrix worldTransform = N._7.ToMatrix4X4(base.DisplayedObject.BufferedStates.InterpolatedStates.OrientationMatrix);
			worldTransform.Translation = translation;
			base.WorldTransform = worldTransform;
		}
	}
	internal class _0001
	{
		public const int MaximumObjectsPerBatch = 61;

		public static int MaximumPrimitiveCountPerBatch = 100000;

		private readonly GraphicsDevice a5h;

		private readonly List<uint> a5b = new List<uint>();

		private readonly List<_7> a56 = new List<_7>();

		private readonly ReadOnlyCollection<_7> a5a;

		private readonly int[] a57 = new int[61];

		private readonly List<VertexPositionNormalTexture> a5_0006 = new List<VertexPositionNormalTexture>();

		private readonly Matrix[] a5v = new Matrix[61];

		private IndexBuffer a5B;

		private uint[] a5X = new uint[0];

		private VertexBuffer a5_0018;

		private float[] a5W = new float[0];

		private VertexBuffer a5_0002;

		private VertexPositionNormalTexture[] a5_000E = new VertexPositionNormalTexture[0];

		private VertexBufferBinding[] a5y;

		public ReadOnlyCollection<_7> DisplayObjects => a5a;

		public _0001(GraphicsDevice graphicsDevice)
		{
			a5h = graphicsDevice;
			a5a = new ReadOnlyCollection<_7>(a56);
		}

		public void Update()
		{
			for (int i = 0; i < a56.Count; i++)
			{
				a56[i].Update();
				ref Matrix reference = ref a5v[i];
				reference = a56[i].WorldTransform;
				a57[i] = a56[i].TextureIndex;
			}
		}

		public void Draw(Effect effect, EffectParameter worldTransformsParameter, EffectParameter textureIndicesParameter, EffectPass pass)
		{
			if (a5_000E.Length > 0)
			{
				a5h.SetVertexBuffers(a5y);
				a5h.Indices = a5B;
				worldTransformsParameter.SetValue(a5v);
				textureIndicesParameter.SetValue(a57);
				pass.Apply();
				a5h.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, a5_000E.Length, 0, a5X.Length / 3);
			}
		}
	}
}
namespace Z
{
	internal class _0006
	{
		private E.h a5h;

		private float a5b;

		internal float a56;

		internal bool a5a;

		internal l._7<_7> a57 = new l._7<_7>(8);

		private bool a5_0006;

		internal d.v a5v = new d.v();

		private bool a5B = true;

		internal bool a5X = true;

		private Action<_0006> a5_0018;

		private Action<_0006> a5W;

		private Action<_0006> a5_0002;

		private Action<_0006> a5_000E;

		internal a a5y;

		internal v a5r;

		[CompilerGenerated]
		private bool a5_0001;

		[CompilerGenerated]
		private _6 a5_000F;

		public E.h Owner => a5h;

		public l.X<_7> Connections => new l.X<_7>(a57);

		public bool IsDeactivationCandidate
		{
			get
			{
				return a5_0006;
			}
			private set
			{
				if (flag && !a5_0006)
				{
					a5_0006 = true;
					OnBecameDeactivationCandidate();
				}
				else if (!flag && a5_0006)
				{
					a5_0006 = false;
					OnBecameNonDeactivationCandidate();
				}
				if (!flag)
				{
					a56 = 0f;
				}
			}
		}

		public bool IsActive
		{
			get
			{
				a simulationIsland = SimulationIsland;
				if (simulationIsland != null)
				{
					return simulationIsland.a56;
				}
				if (!(a5b > 0f))
				{
					return a56 <= 0f;
				}
				return true;
			}
		}

		public bool IsAlwaysActive
		{
			[CompilerGenerated]
			get
			{
				return a5_0001;
			}
			[CompilerGenerated]
			set
			{
				a5_0001 = value;
			}
		}

		public bool AllowStabilization
		{
			get
			{
				return a5X;
			}
			set
			{
				a5X = value;
			}
		}

		public a SimulationIsland
		{
			get
			{
				if (a5y == null)
				{
					return null;
				}
				return a5y.Parent;
			}
			internal set
			{
				a5y = a2;
			}
		}

		public _6 DeactivationManager
		{
			[CompilerGenerated]
			get
			{
				return a5_000F;
			}
			[CompilerGenerated]
			internal set
			{
				a5_000F = obj;
			}
		}

		public bool IsDynamic => a5h.a5B;

		public event Action<_0006> Activated
		{
			add
			{
				Action<_0006> action = a5_0018;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5_0018, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0006> action = a5_0018;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5_0018, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		public event Action<_0006> BecameDeactivationCandidate
		{
			add
			{
				Action<_0006> action = a5W;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5W, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0006> action = a5W;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5W, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		public event Action<_0006> BecameNonDeactivationCandidate
		{
			add
			{
				Action<_0006> action = a5_0002;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5_0002, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0006> action = a5_0002;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5_0002, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		public event Action<_0006> Deactivated
		{
			add
			{
				Action<_0006> action = a5_000E;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5_000E, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0006> action = a5_000E;
				Action<_0006> action2;
				do
				{
					action2 = action;
					Action<_0006> value2 = (Action<_0006>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5_000E, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		internal _0006(E.h P_0)
		{
			a5h = P_0;
		}

		public void UpdateDeactivationCandidacy(float dt)
		{
			float num = a5h.a5a.LengthSquared() + a5h.a5_0006.LengthSquared();
			bool isActive = IsActive;
			if (isActive)
			{
				_6n();
				a5a = num <= a5b;
				if (IsDynamic)
				{
					if (num < DeactivationManager.a56)
					{
						a56 += dt;
					}
					else
					{
						a56 = 0f;
					}
					if (!IsAlwaysActive)
					{
						if (!a5_0006)
						{
							if (a56 > DeactivationManager.a5a && a5a)
							{
								IsDeactivationCandidate = true;
							}
						}
						else if (a56 <= DeactivationManager.a5a)
						{
							IsDeactivationCandidate = false;
						}
					}
					else
					{
						IsDeactivationCandidate = false;
					}
				}
				else
				{
					if (a56 == 0f)
					{
						a56 = 1f;
					}
					else if (a56 < 0f)
					{
						a56 = 0f;
					}
					if (num == 0f)
					{
						IsDeactivationCandidate = true;
					}
					else
					{
						IsDeactivationCandidate = false;
						for (int i = 0; i < a57.a5h; i++)
						{
							l._7<_0006> obj = a57.Elements[i].a5h;
							for (int num2 = obj.a5h - 1; num2 >= 0; num2--)
							{
								obj.Elements[num2].a5v.Enter();
								a simulationIsland = obj.Elements[num2].SimulationIsland;
								if (simulationIsland != null)
								{
									simulationIsland.Activate();
									simulationIsland.a5b = false;
								}
								obj.Elements[num2].a5v.Exit();
							}
						}
					}
				}
			}
			a5b = num;
			if (a5B && !isActive)
			{
				OnDeactivated();
			}
			else if (!a5B && isActive)
			{
				OnActivated();
			}
			a5B = isActive;
		}

		private void _6n()
		{
			a a2 = a5y;
			if (a2 != null && a2.a5h != a2)
			{
				a5v.Enter();
				lock (a2)
				{
					a2.Remove(this);
				}
				a2 = a2.Parent;
				lock (a2)
				{
					a2.Add(this);
				}
				a5v.Exit();
			}
		}

		public void Activate()
		{
			IsDeactivationCandidate = false;
			a simulationIsland = SimulationIsland;
			if (simulationIsland != null)
			{
				simulationIsland.IsActive = true;
			}
			else
			{
				a56 = -1f;
			}
		}

		protected internal void OnActivated()
		{
			if (a5_0018 != null)
			{
				a5_0018(this);
			}
		}

		protected internal void OnBecameDeactivationCandidate()
		{
			if (a5W != null)
			{
				a5W(this);
			}
		}

		protected internal void OnBecameNonDeactivationCandidate()
		{
			if (a5_0002 != null)
			{
				a5_0002(this);
			}
		}

		protected internal void OnDeactivated()
		{
			if (a5_000E != null)
			{
				a5_000E(this);
			}
		}

		internal void _6J(_7 P_0)
		{
			a57.FastRemove(P_0);
		}

		internal void _6D(_7 P_0)
		{
			a57.Add(P_0);
		}
	}
}
