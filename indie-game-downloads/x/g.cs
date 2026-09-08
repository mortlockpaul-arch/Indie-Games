using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace X;

internal class g : IDisposable
{
	private BoundingBox _3A_0018 = default(BoundingBox);

	private int _3AL;

	private IndexBuffer _3A_0019;

	private VertexBuffer _3A3;

	internal BoundingBox ObjectBoundingBox => _3A_0018;

	internal int VertexCount => _3AL;

	internal IndexBuffer IndexBuffer => _3A_0019;

	internal VertexBuffer VertexBuffer => _3A3;

	internal unsafe void _67(GraphicsDevice P_0, c[] P_1, ushort[] P_2, int P_3)
	{
		if (_3A3 == null)
		{
			_3A3 = new VertexBuffer(P_0, typeof(c), 4096, BufferUsage.None);
			_3A_0019 = new IndexBuffer(P_0, typeof(ushort), 6144, BufferUsage.None);
		}
		fixed (c* ptr = P_1)
		{
			fixed (Vector3* max = &_3A_0018.Max)
			{
				fixed (Vector3* min = &_3A_0018.Min)
				{
					int num = P_3 * 4;
					c* ptr2 = ptr;
					min->X = (max->X = ptr2->Position.X);
					min->Y = (max->Y = ptr2->Position.Y);
					max->Z = 1f;
					min->Z = 0f;
					ptr2++;
					for (int i = 1; i < num; i++)
					{
						if (ptr2->Position.X > max->X)
						{
							max->X = ptr2->Position.X;
						}
						else if (ptr2->Position.X < min->X)
						{
							min->X = ptr2->Position.X;
						}
						if (ptr2->Position.Y > max->Y)
						{
							max->Y = ptr2->Position.Y;
						}
						else if (ptr2->Position.Y < min->Y)
						{
							min->Y = ptr2->Position.Y;
						}
						ptr2++;
					}
				}
			}
		}
		P_0.Indices = null;
		P_0.SetVertexBuffer(null);
		_3AL = P_3 * 4;
		_3A3.SetData(P_1, 0, _3AL);
		_3A_0019.SetData(P_2, 0, P_3 * 6);
		P_3 = 0;
	}

	public void Dispose()
	{
		if (_3A_0019 != null)
		{
			_3A_0019.Dispose();
			_3A_0019 = null;
		}
		if (_3A3 != null)
		{
			_3A3.Dispose();
			_3A3 = null;
		}
	}
}
