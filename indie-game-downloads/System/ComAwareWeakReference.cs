using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System;

internal sealed class ComAwareWeakReference
{
	internal sealed class ComInfo
	{
		private nint _pComWeakRef;

		private readonly object _context;

		internal object ResolveTarget()
		{
			return ComWeakRefToObject(_pComWeakRef, _context);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ComInfo FromObject(object target)
		{
			if (target == null || !PossiblyComObject(target))
			{
				return null;
			}
			return FromObjectSlow(target);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ComInfo FromObjectSlow(object target)
		{
			nint num = ObjectToComWeakRef(target, out var context);
			if (num == 0)
			{
				return null;
			}
			try
			{
				return new ComInfo(num, context);
			}
			catch (OutOfMemoryException)
			{
				Marshal.Release(num);
				throw;
			}
		}

		private ComInfo(nint pComWeakRef, object context)
		{
			_pComWeakRef = pComWeakRef;
			_context = context;
		}

		~ComInfo()
		{
			Marshal.Release(_pComWeakRef);
			_pComWeakRef = 0;
		}
	}

	private nint _weakHandle;

	private ComInfo _comInfo;

	private unsafe static delegate*<nint, object, object> s_comWeakRefToObjectCallback;

	private unsafe static delegate*<object, bool> s_possiblyComObjectCallback;

	private unsafe static delegate*<object, out object, nint> s_objectToComWeakRefCallback;

	internal object Target => GCHandle.InternalGet(_weakHandle);

	internal nint WeakHandle => _weakHandle;

	[DllImport("QCall", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ComWeakRefToObject")]
	private static extern void ComWeakRefToObject(nint pComWeakRef, ObjectHandleOnStack retRcw);

	internal static object ComWeakRefToObject(nint pComWeakRef, object context)
	{
		if (context == null)
		{
			object o = null;
			ComWeakRefToObject(pComWeakRef, ObjectHandleOnStack.Create(ref o));
			return o;
		}
		return ComWeakRefToComWrappersObject(pComWeakRef, context);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool PossiblyComObject(object target)
	{
		if (!(target is __ComObject))
		{
			return PossiblyComWrappersObject(target);
		}
		return true;
	}

	[DllImport("QCall", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ObjectToComWeakRef")]
	private static extern nint ObjectToComWeakRef(ObjectHandleOnStack retRcw);

	internal static nint ObjectToComWeakRef(object target, out object context)
	{
		if (target is __ComObject)
		{
			context = null;
			return ObjectToComWeakRef(ObjectHandleOnStack.Create(ref target));
		}
		if (PossiblyComWrappersObject(target))
		{
			return ComWrappersObjectToComWeakRef(target, out context);
		}
		context = null;
		return IntPtr.Zero;
	}

	private ComAwareWeakReference(nint weakHandle)
	{
		_weakHandle = weakHandle;
	}

	~ComAwareWeakReference()
	{
		GCHandle.InternalFree(_weakHandle);
		_weakHandle = 0;
	}

	private void SetTarget(object target, ComInfo comInfo)
	{
		lock (this)
		{
			GCHandle.InternalSet(_weakHandle, target);
			_comInfo = comInfo;
		}
	}

	internal T RehydrateTarget<T>() where T : class
	{
		T val = null;
		lock (this)
		{
			if (_comInfo != null)
			{
				val = Unsafe.As<T>(GCHandle.InternalGet(_weakHandle));
				if (val == null)
				{
					val = (T)_comInfo.ResolveTarget();
					if (val != null)
					{
						GCHandle.InternalSet(_weakHandle, val);
					}
				}
			}
		}
		return val;
	}

	private static ComAwareWeakReference EnsureComAwareReference(ref nint taggedHandle)
	{
		nint num = taggedHandle;
		if ((num & 2) == 0)
		{
			ComAwareWeakReference comAwareWeakReference = new ComAwareWeakReference(taggedHandle & ~(nint)3);
			nint num2 = GCHandle.InternalAlloc(comAwareWeakReference, GCHandleType.Normal);
			nint value = num2 | 2 | (taggedHandle & 1);
			if (Interlocked.CompareExchange(ref taggedHandle, value, num) == num)
			{
				return comAwareWeakReference;
			}
			GCHandle.InternalFree(num2);
			GC.SuppressFinalize(comAwareWeakReference);
		}
		return Unsafe.As<ComAwareWeakReference>(GCHandle.InternalGet(taggedHandle & ~(nint)3));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ComAwareWeakReference GetFromTaggedReference(nint taggedHandle)
	{
		return Unsafe.As<ComAwareWeakReference>(GCHandle.InternalGet(taggedHandle & ~(nint)3));
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void SetTarget(ref nint taggedHandle, object target, ComInfo comInfo)
	{
		((comInfo != null) ? EnsureComAwareReference(ref taggedHandle) : Unsafe.As<ComAwareWeakReference>(GCHandle.InternalGet(taggedHandle & ~(nint)3))).SetTarget(target, comInfo);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void SetComInfoInConstructor(ref nint taggedHandle, ComInfo comInfo)
	{
		ComAwareWeakReference comAwareWeakReference = new ComAwareWeakReference(taggedHandle & ~(nint)3);
		nint num = GCHandle.InternalAlloc(comAwareWeakReference, GCHandleType.Normal);
		taggedHandle = num | 2 | (taggedHandle & 1);
		comAwareWeakReference._comInfo = comInfo;
	}

	internal unsafe static void InitializeCallbacks(delegate*<nint, object, object> comWeakRefToObject, delegate*<object, bool> possiblyComObject, delegate*<object, out object, nint> objectToComWeakRef)
	{
		s_comWeakRefToObjectCallback = comWeakRefToObject;
		s_objectToComWeakRefCallback = objectToComWeakRef;
		s_possiblyComObjectCallback = possiblyComObject;
	}

	internal unsafe static object ComWeakRefToComWrappersObject(nint pComWeakRef, object context)
	{
		if (s_comWeakRefToObjectCallback == (delegate*<nint, object, object>)null)
		{
			return null;
		}
		return s_comWeakRefToObjectCallback(pComWeakRef, context);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static bool PossiblyComWrappersObject(object target)
	{
		if (s_possiblyComObjectCallback == (delegate*<object, bool>)null)
		{
			return false;
		}
		return s_possiblyComObjectCallback(target);
	}

	internal unsafe static nint ComWrappersObjectToComWeakRef(object target, out object context)
	{
		context = null;
		if (s_objectToComWeakRefCallback == (delegate*<object, out object, nint>)null)
		{
			return IntPtr.Zero;
		}
		return s_objectToComWeakRefCallback(target, out context);
	}
}
