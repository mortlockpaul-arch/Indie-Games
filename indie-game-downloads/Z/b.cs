using System;
using System.Collections.Generic;
using _000E;
using E;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P;
using SynapseGaming.LightingSystem.Core;
using l;
using r;
using s;
using y;

namespace z
{
	internal abstract class b
	{
		public delegate void _00065h(s.b collidable, List<VertexPositionNormalTexture> vertices, List<uint> indices);

		private readonly Dictionary<object, _7> a5h = new Dictionary<object, _7>();

		private readonly RasterizerState a5b = new RasterizerState
		{
			CullMode = CullMode.None,
			DepthBias = -1E-06f,
			FillMode = FillMode.Solid,
			SlopeScaleDepthBias = -0.1f
		};

		private readonly RasterizerState a56 = new RasterizerState
		{
			CullMode = CullMode.None,
			DepthBias = -1E-06f,
			FillMode = FillMode.WireFrame,
			SlopeScaleDepthBias = -0.1f
		};

		private static readonly Dictionary<Type, Type> a5a;

		private static readonly Dictionary<Type, _00065h> a57;

		private static readonly List<object> a5_0006;

		public static Dictionary<Type, Type> DisplayTypes => a5a;

		public static Dictionary<Type, _00065h> ShapeMeshGetters => a57;

		static b()
		{
			a5a = new Dictionary<Type, Type>();
			a57 = new Dictionary<Type, _00065h>();
			a5_0006 = new List<object>();
			a5a.Add(typeof(P._0006), typeof(B));
			a5a.Add(typeof(l.y), typeof(X));
			a5a.Add(typeof(P._7), typeof(v));
			a57.Add(typeof(s.B<global::y._6>), _0018.GetShapeMeshData);
			a57.Add(typeof(s.B<global::y._7>), y.GetShapeMeshData);
			a57.Add(typeof(s.B<global::y.b>), _0002.GetShapeMeshData);
			a57.Add(typeof(s.B<global::y.v>), r.GetShapeMeshData);
			a57.Add(typeof(s._6), W.GetShapeMeshData);
		}

		internal _7 bI(object P_0)
		{
			if (!a5h.ContainsKey(P_0))
			{
				if (a5a.TryGetValue(P_0.GetType(), out var value))
				{
					return (_7)value.GetConstructor(new Type[2]
					{
						typeof(b),
						P_0.GetType()
					}).Invoke(new object[2] { this, P_0 });
				}
				if (P_0 is E.h entity)
				{
					return new _000E(this, entity);
				}
			}
			return null;
		}

		public _7 Add(object objectToDisplay)
		{
			_7 obj = bI(objectToDisplay);
			if (obj != null)
			{
				Add(obj);
				a5h.Add(objectToDisplay, obj);
				return obj;
			}
			return null;
		}

		internal void b_0017(global::r.h P_0, bool P_1)
		{
			if (!a5h.TryGetValue(P_0, out var value))
			{
				value = Add(P_0);
			}
			value.a5b = P_1;
			value.a5h = true;
		}

		protected abstract void Add(_7 displayObject);

		public void Remove(object objectToRemove)
		{
			Remove(a5h[objectToRemove]);
			a5h.Remove(objectToRemove);
		}

		protected abstract void Remove(_7 displayObject);

		public void Clear()
		{
			a5h.Clear();
			ClearManagedModels();
		}

		public abstract void Unload();

		protected abstract void ClearManagedModels();

		public bool Contains(object displayedObject)
		{
			return a5h.ContainsKey(displayedObject);
		}

		public void Update()
		{
			UpdateManagedModels();
			a5_0006.Clear();
			foreach (KeyValuePair<object, _7> item in a5h)
			{
				_7 value = item.Value;
				if (value.a5h)
				{
					value.a5h = false;
				}
				else
				{
					a5_0006.Add(item.Key);
				}
			}
			foreach (object item2 in a5_0006)
			{
				Remove(item2);
			}
		}

		protected abstract void UpdateManagedModels();

		public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			graphicsDevice.RasterizerState = a5b;
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
			DrawManagedModels(viewMatrix, projectionMatrix, a5b, a56);
		}

		protected abstract void DrawManagedModels(Matrix viewMatrix, Matrix projectionMatrix, RasterizerState fillmode, RasterizerState wireframemode);
	}
	internal class B : z._0006<P._0006>
	{
		public B(b drawer, P._0006 displayedObject)
			: base(drawer, displayedObject)
		{
		}

		public override int GetTriangleCountEstimate()
		{
			return base.DisplayedObject.Shape.Heights.Length * 2;
		}

		public override void GetMeshData(List<VertexPositionNormalTexture> vertices, List<uint> indices)
		{
			int length = base.DisplayedObject.Shape.Heights.GetLength(0);
			int length2 = base.DisplayedObject.Shape.Heights.GetLength(1);
			global::_000E.v shape = base.DisplayedObject.Shape;
			base.DisplayedObject.GetPosition(0, 0, out var position);
			base.DisplayedObject.GetPosition(1, 0, out var position2);
			base.DisplayedObject.GetPosition(0, 1, out var position3);
			Vector3 vector = Vector3.Cross(position3 - position, position2 - position);
			Vector3 vector2 = new Vector3(base.DisplayedObject.WorldTransform.LinearTransform.M21, base.DisplayedObject.WorldTransform.LinearTransform.M22, base.DisplayedObject.WorldTransform.LinearTransform.M23);
			Vector3.Dot(ref vector, ref vector2, out var result);
			bool flag = result < 0f;
			VertexPositionNormalTexture item = default(VertexPositionNormalTexture);
			for (int i = 0; i < length2; i++)
			{
				for (int j = 0; j < length; j++)
				{
					base.DisplayedObject.GetPosition(j, i, out item.Position);
					base.DisplayedObject.GetNormal(j, i, out item.Normal);
					if (flag)
					{
						Vector3.Negate(ref item.Normal, out item.Normal);
					}
					item.TextureCoordinate = new Vector2(j, i);
					vertices.Add(item);
					if (j >= length - 1 || i >= length2 - 1)
					{
						continue;
					}
					if (shape.QuadTriangleOrganization == global::_000E.B.BottomLeftUpperRight)
					{
						indices.Add((uint)(length * i + j));
						if (flag)
						{
							indices.Add((uint)(length * (i + 1) + j));
							indices.Add((uint)(length * i + j + 1));
						}
						else
						{
							indices.Add((uint)(length * i + j + 1));
							indices.Add((uint)(length * (i + 1) + j));
						}
						indices.Add((uint)(length * i + j + 1));
						if (flag)
						{
							indices.Add((uint)(length * (i + 1) + j));
							indices.Add((uint)(length * (i + 1) + j + 1));
						}
						else
						{
							indices.Add((uint)(length * (i + 1) + j + 1));
							indices.Add((uint)(length * (i + 1) + j));
						}
					}
					else if (shape.QuadTriangleOrganization == global::_000E.B.BottomRightUpperLeft)
					{
						indices.Add((uint)(length * i + j));
						if (flag)
						{
							indices.Add((uint)(length * i + j + 1));
							indices.Add((uint)(length * (i + 1) + j + 1));
						}
						else
						{
							indices.Add((uint)(length * (i + 1) + j + 1));
							indices.Add((uint)(length * i + j + 1));
						}
						indices.Add((uint)(length * i + j));
						if (flag)
						{
							indices.Add((uint)(length * (i + 1) + j));
							indices.Add((uint)(length * (i + 1) + j + 1));
						}
						else
						{
							indices.Add((uint)(length * (i + 1) + j + 1));
							indices.Add((uint)(length * (i + 1) + j));
						}
					}
				}
			}
		}

		public override void Update()
		{
			base.WorldTransform = Matrix.Identity;
		}
	}
}
namespace Z
{
	internal interface b
	{
		_0006 ActivityInformation { get; }
	}
}
