using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Win32.SafeHandles;

internal static class Interop
{
	internal enum BOOL
	{
		FALSE,
		TRUE
	}

	internal static class Kernel32
	{
		internal struct SECURITY_ATTRIBUTES
		{
			internal uint nLength;

			internal unsafe void* lpSecurityDescriptor;

			internal BOOL bInheritHandle;
		}

		[LibraryImport("kernel32.dll", EntryPoint = "CreateEventExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle CreateEventEx(nint lpSecurityAttributes, string name, uint flags, uint desiredAccess)
		{
			bool flag = false;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(lpSecurityAttributes, (ushort*)_name_native, flags, desiredAccess);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "CreateEventExW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(nint __lpSecurityAttributes_native, ushort* __name_native, uint __flags_native, uint __desiredAccess_native);
		}

		[LibraryImport("kernel32.dll", EntryPoint = "OpenEventW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle OpenEvent(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, string name)
		{
			bool flag = false;
			int num = 0;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				num = (inheritHandle ? 1 : 0);
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(desiredAccess, num, (ushort*)_name_native);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "OpenEventW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(uint __desiredAccess_native, int __inheritHandle_native, ushort* __name_native);
		}

		[LibraryImport("kernel32.dll", EntryPoint = "OpenMutexW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle OpenMutex(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, string name)
		{
			bool flag = false;
			int num = 0;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				num = (inheritHandle ? 1 : 0);
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(desiredAccess, num, (ushort*)_name_native);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "OpenMutexW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(uint __desiredAccess_native, int __inheritHandle_native, ushort* __name_native);
		}

		[LibraryImport("kernel32.dll", EntryPoint = "CreateMutexExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle CreateMutexEx(nint lpMutexAttributes, string name, uint flags, uint desiredAccess)
		{
			bool flag = false;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(lpMutexAttributes, (ushort*)_name_native, flags, desiredAccess);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "CreateMutexExW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(nint __lpMutexAttributes_native, ushort* __name_native, uint __flags_native, uint __desiredAccess_native);
		}

		[LibraryImport("kernel32.dll", EntryPoint = "OpenSemaphoreW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle OpenSemaphore(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, string name)
		{
			bool flag = false;
			int num = 0;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				num = (inheritHandle ? 1 : 0);
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(desiredAccess, num, (ushort*)_name_native);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "OpenSemaphoreW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(uint __desiredAccess_native, int __inheritHandle_native, ushort* __name_native);
		}

		[LibraryImport("kernel32.dll", EntryPoint = "CreateSemaphoreExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
		[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
		internal unsafe static SafeWaitHandle CreateSemaphoreEx(nint lpSecurityAttributes, int initialCount, int maximumCount, string name, uint flags, uint desiredAccess)
		{
			bool flag = false;
			SafeWaitHandle result = null;
			nint value = 0;
			SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut managedToUnmanagedOut = new SafeHandleMarshaller<SafeWaitHandle>.ManagedToUnmanagedOut();
			int lastSystemError;
			try
			{
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
				{
					void* _name_native = ptr;
					Marshal.SetLastSystemError(0);
					value = __PInvoke(lpSecurityAttributes, initialCount, maximumCount, (ushort*)_name_native, flags, desiredAccess);
					lastSystemError = Marshal.GetLastSystemError();
				}
				flag = true;
				managedToUnmanagedOut.FromUnmanaged(value);
				result = managedToUnmanagedOut.ToManaged();
			}
			finally
			{
				if (flag)
				{
					managedToUnmanagedOut.Free();
				}
			}
			Marshal.SetLastPInvokeError(lastSystemError);
			return result;
			[DllImport("kernel32.dll", EntryPoint = "CreateSemaphoreExW", ExactSpelling = true)]
			unsafe static extern nint __PInvoke(nint __lpSecurityAttributes_native, int __initialCount_native, int __maximumCount_native, ushort* __name_native, uint __flags_native, uint __desiredAccess_native);
		}
	}
}
