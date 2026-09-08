using System;
using System.Text;

namespace T;

internal sealed class F : t
{
	private static object _3A_0018;

	private IntPtr _3AL;

	private static Random _3A_0019;

	static F()
	{
		_3A_0019 = new Random();
		if (_3d())
		{
			_3A_0018 = new object();
		}
	}

	public F()
	{
		_3AL = _3_0006(null);
		_3V();
	}

	public F(byte[] rgb)
	{
		_3AL = _3_0006(rgb);
		_3V();
	}

	public F(string str)
	{
		if (str == null)
		{
			_3AL = _3_0006(null);
		}
		else
		{
			_3AL = _3_0006(Encoding.UTF8.GetBytes(str));
		}
		_3V();
	}

	private void _3V()
	{
		if (_3AL == IntPtr.Zero)
		{
			throw new _6("Couldn't access random source.");
		}
	}

	private static bool _3d()
	{
		return true;
	}

	private static IntPtr _3_0006(byte[] P_0)
	{
		if (P_0 == null || P_0.Length < 1)
		{
			return new IntPtr(1);
		}
		_3A_0019 = new Random(P_0[0]);
		return new IntPtr(1);
	}

	private static IntPtr _3i(IntPtr P_0, byte[] P_1)
	{
		_3A_0019.NextBytes(P_1);
		return new IntPtr(1);
	}

	private static void _3K(IntPtr P_0)
	{
	}

	public override void GetBytes(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		if (_3A_0018 == null)
		{
			_3AL = _3i(_3AL, data);
		}
		else
		{
			lock (_3A_0018)
			{
				_3AL = _3i(_3AL, data);
			}
		}
		_3V();
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
			_3AL = _3i(_3AL, array);
			_3V();
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

	~F()
	{
		if (_3AL != IntPtr.Zero)
		{
			_3K(_3AL);
			_3AL = IntPtr.Zero;
		}
	}
}
