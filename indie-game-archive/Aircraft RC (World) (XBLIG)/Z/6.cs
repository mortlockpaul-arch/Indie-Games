using System;
using System.Text;

namespace Z
{
	internal class _6<T> : global::Z.y<T> where T : IDisposable, new()
	{
		public override void Clear()
		{
			FreeAllTracked();
			foreach (T item in _UnusedObjectPool)
			{
				item.Dispose();
			}
			_UnusedObjectPool.Clear();
			if (_LostObjectCount > 0)
			{
				throw new Exception("Some tracked pool objects were not disposed.");
			}
		}

		public void Unload()
		{
			Clear();
		}
	}
}
namespace z
{
	internal sealed class _6 : y
	{
		private static object HCB;

		private IntPtr HC_0002;

		private static Random HC_0012;

		static _6()
		{
			HC_0012 = new Random();
			if (H9())
			{
				HCB = new object();
			}
		}

		public _6()
		{
			HC_0002 = H_0004(null);
			HE();
		}

		public _6(byte[] rgb)
		{
			HC_0002 = H_0004(rgb);
			HE();
		}

		public _6(string str)
		{
			if (str == null)
			{
				HC_0002 = H_0004(null);
			}
			else
			{
				HC_0002 = H_0004(Encoding.UTF8.GetBytes(str));
			}
			HE();
		}

		private void HE()
		{
			if (HC_0002 == IntPtr.Zero)
			{
				throw new _7("Couldn't access random source.");
			}
		}

		private static bool H9()
		{
			return true;
		}

		private static IntPtr H_0004(byte[] P_0)
		{
			if (P_0 == null || P_0.Length < 1)
			{
				return new IntPtr(1);
			}
			HC_0012 = new Random(P_0[0]);
			return new IntPtr(1);
		}

		private static IntPtr HM(IntPtr P_0, byte[] P_1)
		{
			HC_0012.NextBytes(P_1);
			return new IntPtr(1);
		}

		private static void HP(IntPtr P_0)
		{
		}

		public override void GetBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (HCB == null)
			{
				HC_0002 = HM(HC_0002, data);
			}
			else
			{
				lock (HCB)
				{
					HC_0002 = HM(HC_0002, data);
				}
			}
			HE();
		}

		public override void GetNonZeroBytes(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			byte[] array = new byte[data.Length * 2];
			int num = 0;
			while (num < data.Length)
			{
				HC_0002 = HM(HC_0002, array);
				HE();
				for (int i = 0; i < array.Length; i++)
				{
					if (num == data.Length)
					{
						break;
					}
					if (array[i] != 0)
					{
						data[num++] = array[i];
					}
				}
			}
		}

		~_6()
		{
			if (HC_0002 != IntPtr.Zero)
			{
				HP(HC_0002);
				HC_0002 = IntPtr.Zero;
			}
		}
	}
}
