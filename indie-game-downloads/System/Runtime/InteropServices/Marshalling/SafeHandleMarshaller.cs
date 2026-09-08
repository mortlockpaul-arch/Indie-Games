using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.InteropServices.Marshalling;

[CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder), MarshalMode.ManagedToUnmanagedIn, typeof(SafeHandleMarshaller<>.ManagedToUnmanagedIn))]
[CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder), MarshalMode.ManagedToUnmanagedRef, typeof(SafeHandleMarshaller<>.ManagedToUnmanagedRef))]
[CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder), MarshalMode.ManagedToUnmanagedOut, typeof(SafeHandleMarshaller<>.ManagedToUnmanagedOut))]
public static class SafeHandleMarshaller<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> where T : SafeHandle
{
	public struct ManagedToUnmanagedIn
	{
		private bool _addRefd;

		private T _handle;

		public void FromManaged(T handle)
		{
			_handle = handle;
			handle.DangerousAddRef(ref _addRefd);
		}

		public nint ToUnmanaged()
		{
			return _handle.DangerousGetHandle();
		}

		public void Free()
		{
			if (_addRefd)
			{
				_handle.DangerousRelease();
			}
		}
	}

	public struct ManagedToUnmanagedRef
	{
		private bool _addRefd = false;

		private bool _callInvoked = false;

		private T _handle = null;

		private nint _originalHandleValue = 0;

		private T _newHandle = Activator.CreateInstance<T>();

		private T _handleToReturn = null;

		public ManagedToUnmanagedRef()
		{
		}

		public void FromManaged(T handle)
		{
			_handle = handle;
			handle.DangerousAddRef(ref _addRefd);
			_originalHandleValue = handle.DangerousGetHandle();
		}

		public nint ToUnmanaged()
		{
			return _originalHandleValue;
		}

		public void FromUnmanaged(nint value)
		{
			if (value == _originalHandleValue)
			{
				_handleToReturn = _handle;
				return;
			}
			Marshal.InitHandle(_newHandle, value);
			_handleToReturn = _newHandle;
		}

		public void OnInvoked()
		{
			_callInvoked = true;
		}

		public T ToManagedFinally()
		{
			return _handleToReturn;
		}

		public void Free()
		{
			if (_addRefd)
			{
				_handle.DangerousRelease();
			}
			if (!_callInvoked)
			{
				_newHandle.Dispose();
			}
		}
	}

	public struct ManagedToUnmanagedOut
	{
		private bool _initialized = false;

		private T _newHandle = Activator.CreateInstance<T>();

		public ManagedToUnmanagedOut()
		{
		}

		public void FromUnmanaged(nint value)
		{
			_initialized = true;
			Marshal.InitHandle(_newHandle, value);
		}

		public T ToManaged()
		{
			return _newHandle;
		}

		public void Free()
		{
			if (!_initialized)
			{
				_newHandle.Dispose();
			}
		}
	}
}
