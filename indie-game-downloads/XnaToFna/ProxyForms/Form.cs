using System;
using System.Threading;

namespace XnaToFna.ProxyForms;

public class Form : Control
{
	public IntPtr WindowHookPtr;

	public Delegate WindowHook;

	public int ThreadId;

	public virtual FormBorderStyle FormBorderStyle { get; set; }

	public virtual FormWindowState WindowState { get; set; }

	public virtual FormStartPosition StartPosition { get; set; }

	public virtual bool KeyPreview { get; set; }

	public event FormClosingEventHandler FormClosing;

	public event FormClosedEventHandler FormClosed;

	public Form()
	{
		Form = this;
		ThreadId = Thread.CurrentThread.ManagedThreadId;
		StartPosition = FormStartPosition.WindowsDefaultLocation;
		KeyPreview = false;
	}

	protected virtual void OnFormClosing(FormClosingEventArgs e)
	{
	}

	protected virtual void OnFormClosed(FormClosedEventArgs e)
	{
	}

	protected virtual void _Close()
	{
	}

	public void Close()
	{
		FormClosingEventArgs e = new FormClosingEventArgs(CloseReason.None, cancel: false);
		OnFormClosing(e);
		FormClosing(this, e);
		_Close();
		FormClosedEventArgs e2 = new FormClosedEventArgs(CloseReason.None);
		OnFormClosed(e2);
		FormClosed(this, e2);
	}

	protected override void WndProc(ref Message msg)
	{
		msg.Result = (IntPtr)(WindowHook?.DynamicInvoke(msg.HWnd, msg.Msg, msg.WParam, msg.LParam));
	}
}
