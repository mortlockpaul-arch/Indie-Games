using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace XnaToFna.ProxyForms;

[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
public struct Message
{
	public IntPtr HWnd { get; set; }

	public int Msg { get; set; }

	public IntPtr WParam { get; set; }

	public IntPtr LParam { get; set; }

	public IntPtr Result { get; set; }

	public object GetLParam(Type cls)
	{
		return Marshal.PtrToStructure(LParam, cls);
	}

	public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
	{
		return new Message
		{
			HWnd = hWnd,
			Msg = msg,
			WParam = wparam,
			LParam = lparam,
			Result = IntPtr.Zero
		};
	}

	public override bool Equals(object o)
	{
		if (!(o is Message message))
		{
			return false;
		}
		if (HWnd == message.HWnd && Msg == message.Msg && WParam == message.WParam && LParam == message.LParam)
		{
			return Result == message.Result;
		}
		return false;
	}

	public static bool operator !=(Message a, Message b)
	{
		return !a.Equals(b);
	}

	public static bool operator ==(Message a, Message b)
	{
		return a.Equals(b);
	}

	public override int GetHashCode()
	{
		return ((int)HWnd << 4) | Msg;
	}

	public override string ToString()
	{
		bool flag = false;
		try
		{
			ProxyMessageHelper.UnmanagedCode.Demand();
			flag = true;
		}
		catch (SecurityException)
		{
		}
		if (!flag)
		{
			return base.ToString();
		}
		return ProxyMessageHelper.ToString(this);
	}
}
