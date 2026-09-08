using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.Threading;

public sealed class Thread : CriticalFinalizerObject
{
	private static class DirectOnThreadLocalData
	{
		[ThreadStatic]
		public static nint pNativeThread;
	}

	private sealed class StartHelper
	{
		internal int _maxStackSize;

		internal Delegate _start;

		internal object _startArg;

		internal CultureInfo _culture;

		internal CultureInfo _uiCulture;

		internal ExecutionContext _executionContext;

		internal static readonly ContextCallback s_threadStartContextCallback = Callback;

		internal StartHelper(Delegate start)
		{
			_start = start;
		}

		private static void Callback(object state)
		{
			((StartHelper)state).RunWorker();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Run()
		{
			if (_executionContext != null && !_executionContext.IsDefault)
			{
				System.Threading.ExecutionContext.RunInternal(_executionContext, s_threadStartContextCallback, this);
			}
			else
			{
				RunWorker();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RunWorker()
		{
			InitializeCulture();
			Delegate start = _start;
			_start = null;
			try
			{
				if (start is ThreadStart threadStart)
				{
					threadStart();
					return;
				}
				ParameterizedThreadStart obj = (ParameterizedThreadStart)start;
				object startArg = _startArg;
				_startArg = null;
				obj(startArg);
			}
			catch (Exception ex) when (ExceptionHandling.IsHandledByGlobalHandler(ex))
			{
			}
		}

		private void InitializeCulture()
		{
			if (_culture != null)
			{
				CultureInfo.CurrentCulture = _culture;
				_culture = null;
			}
			if (_uiCulture != null)
			{
				CultureInfo.CurrentUICulture = _uiCulture;
				_uiCulture = null;
			}
		}
	}

	private static class LocalDataStore
	{
		private static Dictionary<string, LocalDataStoreSlot> s_nameToSlotMap;

		public static LocalDataStoreSlot AllocateSlot()
		{
			return new LocalDataStoreSlot(new ThreadLocal<object>());
		}

		private static Dictionary<string, LocalDataStoreSlot> EnsureNameToSlotMap()
		{
			Dictionary<string, LocalDataStoreSlot> dictionary = s_nameToSlotMap;
			if (dictionary != null)
			{
				return dictionary;
			}
			dictionary = new Dictionary<string, LocalDataStoreSlot>();
			return Interlocked.CompareExchange(ref s_nameToSlotMap, dictionary, null) ?? dictionary;
		}

		public static LocalDataStoreSlot AllocateNamedSlot(string name)
		{
			LocalDataStoreSlot localDataStoreSlot = AllocateSlot();
			Dictionary<string, LocalDataStoreSlot> dictionary = EnsureNameToSlotMap();
			lock (dictionary)
			{
				dictionary.Add(name, localDataStoreSlot);
				return localDataStoreSlot;
			}
		}

		public static LocalDataStoreSlot GetNamedSlot(string name)
		{
			Dictionary<string, LocalDataStoreSlot> dictionary = EnsureNameToSlotMap();
			lock (dictionary)
			{
				if (!dictionary.TryGetValue(name, out var value))
				{
					value = (dictionary[name] = AllocateSlot());
				}
				return value;
			}
		}

		public static void FreeNamedSlot(string name)
		{
			Dictionary<string, LocalDataStoreSlot> dictionary = EnsureNameToSlotMap();
			lock (dictionary)
			{
				dictionary.Remove(name);
			}
		}

		private static ThreadLocal<object> GetThreadLocal(LocalDataStoreSlot slot)
		{
			ArgumentNullException.ThrowIfNull(slot, "slot");
			return slot.Data;
		}

		public static object GetData(LocalDataStoreSlot slot)
		{
			return GetThreadLocal(slot).Value;
		}

		public static void SetData(LocalDataStoreSlot slot, object value)
		{
			GetThreadLocal(slot).Value = value;
		}
	}

	internal readonly ref struct CurrentUserSecurityDescriptorInfo
	{
		private readonly SafeTokenHandle _token;

		private readonly SafeLocalAllocHandle _tokenUser;

		private readonly SafeLocalAllocHandle _dacl;

		private readonly SafeLocalAllocHandle _sacl;

		private readonly SafeLocalAllocHandle _securityDescriptor;

		public SafeLocalAllocHandle TokenUser => _tokenUser;

		public nint SecurityDescriptor => _securityDescriptor.DangerousGetHandle();

		public CurrentUserSecurityDescriptorInfo(int accessMask)
		{
			this = default(CurrentUserSecurityDescriptorInfo);
			_token = OpenCurrentToken();
			try
			{
				_tokenUser = GetTokenUser(_token);
				nint tokenUserSid = GetTokenUserSid(_tokenUser);
				_dacl = CreateDacl(tokenUserSid, accessMask);
				_sacl = CreateMandatoryLabelAceSacl(_token);
				_securityDescriptor = CreateSecurityDescriptor(tokenUserSid, _dacl, _sacl);
			}
			catch
			{
				_securityDescriptor?.Dispose();
				_sacl?.Dispose();
				_dacl?.Dispose();
				_tokenUser?.Dispose();
				_token.Dispose();
				throw;
			}
		}

		public void Dispose()
		{
			if (_securityDescriptor != null)
			{
				_securityDescriptor.Dispose();
				_sacl.Dispose();
				_dacl.Dispose();
				_tokenUser.Dispose();
				_token.Dispose();
			}
		}

		private static SafeTokenHandle OpenCurrentToken()
		{
			nint currentThread = Interop.Kernel32.GetCurrentThread();
			if (Interop.Advapi32.OpenThreadToken(currentThread, 8, OpenAsSelf: true, out var TokenHandle))
			{
				return TokenHandle;
			}
			TokenHandle.Dispose();
			if (Interop.Advapi32.OpenThreadToken(currentThread, 8, OpenAsSelf: false, out TokenHandle))
			{
				return TokenHandle;
			}
			TokenHandle.Dispose();
			if (!Interop.Advapi32.OpenProcessToken(Interop.Kernel32.GetCurrentProcess(), 8, out TokenHandle))
			{
				int lastPInvokeError = Marshal.GetLastPInvokeError();
				TokenHandle.Dispose();
				ThrowExceptionForError(lastPInvokeError);
			}
			return TokenHandle;
		}

		private static SafeLocalAllocHandle GetTokenUser(SafeTokenHandle token)
		{
			Interop.Advapi32.GetTokenInformation(token.DangerousGetHandle(), 1u, 0, 0u, out var ReturnLength);
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (lastPInvokeError != 122)
			{
				ThrowExceptionForError(lastPInvokeError);
			}
			SafeLocalAllocHandle safeLocalAllocHandle = SafeLocalAllocHandle.LocalAlloc((int)ReturnLength);
			try
			{
				if (!Interop.Advapi32.GetTokenInformation(token.DangerousGetHandle(), 1u, safeLocalAllocHandle.DangerousGetHandle(), ReturnLength, out ReturnLength))
				{
					ThrowExceptionForLastError();
				}
				return safeLocalAllocHandle;
			}
			catch
			{
				safeLocalAllocHandle.Dispose();
				throw;
			}
		}

		private static nint GetTokenUserSid(SafeLocalAllocHandle tokenUser)
		{
			return ((SafeBuffer)tokenUser).Read<nint>(0uL);
		}

		private unsafe static SafeLocalAllocHandle CreateDacl(nint tokenUserSid, int userAccessMask)
		{
			uint resultSidLength = 68u;
			byte* ptr = stackalloc byte[(int)resultSidLength];
			if (!Interop.Advapi32.CreateWellKnownSid(26, 0, (nint)ptr, ref resultSidLength))
			{
				ThrowExceptionForLastError();
			}
			Interop.Advapi32.EXPLICIT_ACCESS* ptr2 = stackalloc Interop.Advapi32.EXPLICIT_ACCESS[2]
			{
				default(Interop.Advapi32.EXPLICIT_ACCESS),
				default(Interop.Advapi32.EXPLICIT_ACCESS)
			};
			ptr2->grfAccessPermissions = userAccessMask;
			ptr2->grfAccessMode = Interop.Advapi32.ACCESS_MODE.SET_ACCESS;
			ptr2->grfInheritance = 0;
			ptr2->Trustee.TrusteeForm = Interop.Advapi32.TRUSTEE_FORM.TRUSTEE_IS_SID;
			ptr2->Trustee.TrusteeType = Interop.Advapi32.TRUSTEE_TYPE.TRUSTEE_IS_USER;
			ptr2->Trustee.ptstrName = tokenUserSid;
			ptr2[1].grfAccessPermissions = 131072;
			ptr2[1].grfAccessMode = Interop.Advapi32.ACCESS_MODE.SET_ACCESS;
			ptr2[1].grfInheritance = 0;
			ptr2[1].Trustee.TrusteeForm = Interop.Advapi32.TRUSTEE_FORM.TRUSTEE_IS_SID;
			ptr2[1].Trustee.TrusteeType = Interop.Advapi32.TRUSTEE_TYPE.TRUSTEE_IS_GROUP;
			ptr2[1].Trustee.ptstrName = (nint)ptr;
			int num = Interop.Advapi32.SetEntriesInAcl(2, ptr2, 0, out var NewAcl);
			if (num != 0)
			{
				NewAcl.Dispose();
				if (num == 127)
				{
					num = 5;
				}
				ThrowExceptionForError(num);
			}
			return NewAcl;
		}

		private unsafe static SafeLocalAllocHandle CreateMandatoryLabelAceSacl(SafeTokenHandle token)
		{
			using SafeLocalAllocHandle safeLocalAllocHandle = GetTokenMandatoryLabel(token);
			nint num = ((SafeBuffer)safeLocalAllocHandle).Read<nint>(0uL);
			uint integrityLevel = GetIntegrityLevel(num);
			byte* ptr = stackalloc byte[68];
			uint resultSidLength;
			nint num2;
			if (integrityLevel >= 8192)
			{
				resultSidLength = 68u;
				if (!Interop.Advapi32.CreateWellKnownSid(67, 0, (nint)ptr, ref resultSidLength))
				{
					ThrowExceptionForLastError();
				}
				num2 = (nint)ptr;
			}
			else
			{
				num2 = num;
				resultSidLength = (uint)Interop.Advapi32.GetLengthSid(num2);
			}
			int num3 = sizeof(Interop.Advapi32.ACL) + sizeof(Interop.Advapi32.ACE) - 4 + (int)resultSidLength;
			num3 = (num3 + 3) & -4;
			SafeLocalAllocHandle safeLocalAllocHandle2 = SafeLocalAllocHandle.LocalAlloc(num3);
			try
			{
				if (!Interop.Advapi32.InitializeAcl(safeLocalAllocHandle2.DangerousGetHandle(), num3, 2))
				{
					ThrowExceptionForLastError();
				}
				if (!Interop.Advapi32.AddMandatoryAce(safeLocalAllocHandle2.DangerousGetHandle(), 2, 0, 3, num2))
				{
					ThrowExceptionForLastError();
				}
				return safeLocalAllocHandle2;
			}
			catch
			{
				safeLocalAllocHandle2.Dispose();
				throw;
			}
		}

		private static SafeLocalAllocHandle GetTokenMandatoryLabel(SafeTokenHandle token)
		{
			Interop.Advapi32.GetTokenInformation(token.DangerousGetHandle(), 25u, 0, 0u, out var ReturnLength);
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			if (lastPInvokeError != 122)
			{
				ThrowExceptionForError(lastPInvokeError);
			}
			SafeLocalAllocHandle safeLocalAllocHandle = SafeLocalAllocHandle.LocalAlloc((int)ReturnLength);
			try
			{
				if (!Interop.Advapi32.GetTokenInformation(token.DangerousGetHandle(), 25u, safeLocalAllocHandle.DangerousGetHandle(), ReturnLength, out ReturnLength))
				{
					ThrowExceptionForLastError();
				}
				return safeLocalAllocHandle;
			}
			catch
			{
				safeLocalAllocHandle.Dispose();
				throw;
			}
		}

		private unsafe static uint GetIntegrityLevel(nint integrityLevelSid)
		{
			byte b = Unsafe.Read<byte>((void*)Interop.Advapi32.GetSidSubAuthorityCount(integrityLevelSid));
			return Unsafe.Read<uint>((void*)Interop.Advapi32.GetSidSubAuthority(integrityLevelSid, b - 1));
		}

		private static SafeLocalAllocHandle CreateSecurityDescriptor(nint tokenUserSid, SafeLocalAllocHandle dacl, SafeLocalAllocHandle sacl)
		{
			SafeLocalAllocHandle safeLocalAllocHandle = SafeLocalAllocHandle.LocalAlloc(40);
			try
			{
				if (!Interop.Advapi32.InitializeSecurityDescriptor(safeLocalAllocHandle.DangerousGetHandle(), 1))
				{
					ThrowExceptionForLastError();
				}
				if (!Interop.Advapi32.SetSecurityDescriptorOwner(safeLocalAllocHandle.DangerousGetHandle(), tokenUserSid, bOwnerDefaulted: false))
				{
					ThrowExceptionForLastError();
				}
				if (!Interop.Advapi32.SetSecurityDescriptorGroup(safeLocalAllocHandle.DangerousGetHandle(), tokenUserSid, bGroupDefaulted: false))
				{
					ThrowExceptionForLastError();
				}
				if (!Interop.Advapi32.SetSecurityDescriptorDacl(safeLocalAllocHandle.DangerousGetHandle(), bDaclPresent: true, dacl.DangerousGetHandle(), bDaclDefaulted: false))
				{
					ThrowExceptionForLastError();
				}
				if (sacl != null && !Interop.Advapi32.SetSecurityDescriptorSacl(safeLocalAllocHandle.DangerousGetHandle(), bSaclPresent: true, sacl.DangerousGetHandle(), bSaclDefaulted: false))
				{
					ThrowExceptionForLastError();
				}
				return safeLocalAllocHandle;
			}
			catch
			{
				safeLocalAllocHandle.Dispose();
				throw;
			}
		}

		public static bool IsValidSecurityDescriptor(SafeWaitHandle objectHandle, int modifyStateAccessMask)
		{
			using SafeTokenHandle token = OpenCurrentToken();
			using SafeLocalAllocHandle tokenUser = GetTokenUser(token);
			return IsSecurityDescriptorCompatible(tokenUser, objectHandle, modifyStateAccessMask);
		}

		public unsafe static bool IsSecurityDescriptorCompatible(SafeLocalAllocHandle tokenUser, SafeWaitHandle objectHandle, int modifyStateAccessMask)
		{
			nint tokenUserSid = GetTokenUserSid(tokenUser);
			using SafeLocalAllocHandle safeLocalAllocHandle = new SafeLocalAllocHandle();
			nint pSid = 0;
			Interop.Advapi32.ACL* ptr = null;
			nint handle = 0;
			int securityInfoByHandle = (int)Interop.Advapi32.GetSecurityInfoByHandle(objectHandle, 6u, 5u, &pSid, null, (nint*)(&ptr), null, &handle);
			if (securityInfoByHandle != 0)
			{
				ThrowExceptionForError(securityInfoByHandle);
			}
			safeLocalAllocHandle.SetHandle(handle);
			if (!Interop.Advapi32.EqualSid(pSid, tokenUserSid))
			{
				return false;
			}
			int num = 0x1C0000 | modifyStateAccessMask;
			int i = 0;
			for (int aceCount = ptr->AceCount; i < aceCount; i++)
			{
				if (!Interop.Advapi32.GetAce(ptr, i, out var pAce))
				{
					ThrowExceptionForError(securityInfoByHandle);
				}
				if (pAce->Header.AceType == 0 && (pAce->Mask & num) != 0L && !Interop.Advapi32.EqualSid((nint)(&pAce->SidStart), tokenUserSid))
				{
					return false;
				}
			}
			return true;
		}

		[DoesNotReturn]
		private static void ThrowExceptionForLastError()
		{
			ThrowExceptionForError(Marshal.GetLastPInvokeError());
		}

		[DoesNotReturn]
		private static void ThrowExceptionForError(int error)
		{
			throw Win32Marshal.GetExceptionForWin32Error(error);
		}
	}

	internal ExecutionContext _executionContext;

	internal SynchronizationContext _synchronizationContext;

	private string _name;

	private StartHelper _startHelper;

	private nint _DONT_USE_InternalThread;

	private int _priority;

	private int _managedThreadId;

	private bool _mayNeedResetForThreadPool;

	private bool _isDead;

	private bool _isThreadPool;

	private const int SpinWaitCoopThreshold = 1024;

	private static AsyncLocal<IPrincipal> s_asyncLocalPrincipal;

	[ThreadStatic]
	private static Thread t_currentThread;

	public int ManagedThreadId
	{
		[Intrinsic]
		get
		{
			return _managedThreadId;
		}
	}

	public bool IsAlive => (ThreadState & (ThreadState.Unstarted | ThreadState.Stopped | ThreadState.Aborted)) == 0;

	public bool IsBackground
	{
		get
		{
			if (_isDead)
			{
				throw new ThreadStateException(SR.ThreadState_Dead_State);
			}
			Interop.BOOL isBackground = GetIsBackground(GetNativeHandle());
			GC.KeepAlive(this);
			return isBackground != Interop.BOOL.FALSE;
		}
		set
		{
			if (_isDead)
			{
				throw new ThreadStateException(SR.ThreadState_Dead_State);
			}
			SetIsBackground(GetNativeHandle(), value ? Interop.BOOL.TRUE : Interop.BOOL.FALSE);
			GC.KeepAlive(this);
			if (!value)
			{
				_mayNeedResetForThreadPool = true;
			}
		}
	}

	public bool IsThreadPoolThread
	{
		get
		{
			if (_isDead)
			{
				throw new ThreadStateException(SR.ThreadState_Dead_State);
			}
			return _isThreadPool;
		}
		internal set
		{
			_isThreadPool = value;
		}
	}

	public ThreadPriority Priority
	{
		get
		{
			if (_isDead)
			{
				throw new ThreadStateException(SR.ThreadState_Dead_Priority);
			}
			return (ThreadPriority)_priority;
		}
		set
		{
			Thread o = this;
			SetPriority(ObjectHandleOnStack.Create(ref o), (int)value);
			_mayNeedResetForThreadPool = true;
		}
	}

	public ThreadState ThreadState
	{
		get
		{
			int threadState = GetThreadState(GetNativeHandle());
			GC.KeepAlive(this);
			return (ThreadState)threadState;
		}
	}

	internal static extern int OptimalMaxSpinWaitsPerSpinIteration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[CompilerGenerated]
		get;
	}

	public CultureInfo CurrentCulture
	{
		get
		{
			RequireCurrentThread();
			return CultureInfo.CurrentCulture;
		}
		set
		{
			if (this != CurrentThread)
			{
				SetCultureOnUnstartedThread(value, uiCulture: false);
			}
			else
			{
				CultureInfo.CurrentCulture = value;
			}
		}
	}

	public CultureInfo CurrentUICulture
	{
		get
		{
			RequireCurrentThread();
			return CultureInfo.CurrentUICulture;
		}
		set
		{
			if (this != CurrentThread)
			{
				SetCultureOnUnstartedThread(value, uiCulture: true);
			}
			else
			{
				CultureInfo.CurrentUICulture = value;
			}
		}
	}

	public static IPrincipal? CurrentPrincipal
	{
		get
		{
			IPrincipal principal = s_asyncLocalPrincipal?.Value;
			if (principal == null)
			{
				principal = (CurrentPrincipal = AppDomain.CurrentDomain.GetThreadPrincipal());
			}
			return principal;
		}
		set
		{
			if (s_asyncLocalPrincipal == null)
			{
				if (value == null)
				{
					return;
				}
				Interlocked.CompareExchange(ref s_asyncLocalPrincipal, new AsyncLocal<IPrincipal>(), null);
			}
			s_asyncLocalPrincipal.Value = value;
		}
	}

	public static Thread CurrentThread
	{
		[Intrinsic]
		get
		{
			return t_currentThread ?? InitializeCurrentThread();
		}
	}

	internal static ulong CurrentOSThreadId => GetCurrentOSThreadId();

	internal static Thread CurrentThreadAssumedInitialized => t_currentThread;

	public ExecutionContext? ExecutionContext => System.Threading.ExecutionContext.Capture();

	public string? Name
	{
		get
		{
			return _name;
		}
		set
		{
			lock (this)
			{
				if (_name != value)
				{
					_name = value;
					ThreadNameChanged(value);
					_mayNeedResetForThreadPool = true;
				}
			}
		}
	}

	[Obsolete("The ApartmentState property has been deprecated. Use GetApartmentState, SetApartmentState or TrySetApartmentState instead.")]
	public ApartmentState ApartmentState
	{
		get
		{
			return GetApartmentState();
		}
		set
		{
			TrySetApartmentState(value);
		}
	}

	internal ThreadHandle GetNativeHandle()
	{
		nint dONT_USE_InternalThread = _DONT_USE_InternalThread;
		if (dONT_USE_InternalThread == IntPtr.Zero)
		{
			throw new ThreadStateException(SR.Argument_InvalidHandle);
		}
		return new ThreadHandle(dONT_USE_InternalThread);
	}

	private unsafe void StartCore()
	{
		lock (this)
		{
			fixed (char* name = _name)
			{
				StartInternal(GetNativeHandle(), _startHelper?._maxStackSize ?? 0, _priority, _isThreadPool ? Interop.BOOL.TRUE : Interop.BOOL.FALSE, name);
			}
		}
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_Start", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_Start")]
	private unsafe static extern void StartInternal(ThreadHandle t, int stackSize, int priority, Interop.BOOL isThreadPool, char* pThreadName);

	private void StartCallback()
	{
		StartHelper startHelper = _startHelper;
		_startHelper = null;
		startHelper.Run();
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_Sleep", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_Sleep")]
	private static extern void SleepInternal(int millisecondsTimeout);

	[DllImport("QCall", EntryPoint = "ThreadNative_SpinWait", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_SpinWait")]
	[SuppressGCTransition]
	private static extern void SpinWaitInternal(int iterations);

	[DllImport("QCall", EntryPoint = "ThreadNative_SpinWait", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_SpinWait")]
	private static extern void LongSpinWaitInternal(int iterations);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void LongSpinWait(int iterations)
	{
		LongSpinWaitInternal(iterations);
	}

	public static void SpinWait(int iterations)
	{
		if (iterations < 1024)
		{
			SpinWaitInternal(iterations);
		}
		else
		{
			LongSpinWait(iterations);
		}
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_YieldThread", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_YieldThread")]
	private static extern Interop.BOOL YieldInternal();

	public static bool Yield()
	{
		return YieldInternal() != Interop.BOOL.FALSE;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static Thread InitializeCurrentThread()
	{
		Thread o = null;
		GetCurrentThread(ObjectHandleOnStack.Create(ref o));
		return t_currentThread = o;
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_GetCurrentThread", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_GetCurrentThread")]
	private static extern void GetCurrentThread(ObjectHandleOnStack thread);

	private void Initialize()
	{
		Thread o = this;
		Initialize(ObjectHandleOnStack.Create(ref o));
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_Initialize", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_Initialize")]
	private static extern void Initialize(ObjectHandleOnStack thread);

	~Thread()
	{
		InternalFinalize();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void InternalFinalize();

	private void ThreadNameChanged(string value)
	{
		InformThreadNameChange(GetNativeHandle(), value, value?.Length ?? 0);
		GC.KeepAlive(this);
	}

	[LibraryImport("QCall", EntryPoint = "ThreadNative_InformThreadNameChange", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void InformThreadNameChange(ThreadHandle t, string name, int len)
	{
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(name))
		{
			void* _name_native = ptr;
			__PInvoke(t, (ushort*)_name_native, len);
		}
		[DllImport("QCall", EntryPoint = "ThreadNative_InformThreadNameChange", ExactSpelling = true)]
		unsafe static extern void __PInvoke(ThreadHandle __t_native, ushort* __name_native, int __len_native);
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_GetIsBackground", ExactSpelling = true)]
	[SuppressGCTransition]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_GetIsBackground")]
	private static extern Interop.BOOL GetIsBackground(ThreadHandle t);

	[DllImport("QCall", EntryPoint = "ThreadNative_SetIsBackground", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_SetIsBackground")]
	private static extern void SetIsBackground(ThreadHandle t, Interop.BOOL value);

	[DllImport("QCall", EntryPoint = "ThreadNative_SetPriority", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_SetPriority")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern void SetPriority(ObjectHandleOnStack thread, int priority);

	[DllImport("QCall", EntryPoint = "ThreadNative_GetCurrentOSThreadId", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_GetCurrentOSThreadId")]
	private static extern ulong GetCurrentOSThreadId();

	[DllImport("QCall", EntryPoint = "ThreadNative_GetThreadState", ExactSpelling = true)]
	[SuppressGCTransition]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_GetThreadState")]
	private static extern int GetThreadState(ThreadHandle t);

	[DllImport("QCall", EntryPoint = "ThreadNative_GetApartmentState", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_GetApartmentState")]
	private static extern int GetApartmentState(ObjectHandleOnStack t);

	[DllImport("QCall", EntryPoint = "ThreadNative_SetApartmentState", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_SetApartmentState")]
	private static extern int SetApartmentState(ObjectHandleOnStack t, int state);

	public ApartmentState GetApartmentState()
	{
		Thread o = this;
		return (ApartmentState)GetApartmentState(ObjectHandleOnStack.Create(ref o));
	}

	private bool SetApartmentStateUnchecked(ApartmentState state, bool throwOnError)
	{
		ApartmentState apartmentState;
		lock (this)
		{
			Thread o = this;
			apartmentState = (ApartmentState)SetApartmentState(ObjectHandleOnStack.Create(ref o), (int)state);
		}
		if (state == ApartmentState.Unknown && apartmentState == ApartmentState.MTA)
		{
			return true;
		}
		if (apartmentState != state)
		{
			if (throwOnError)
			{
				throw new InvalidOperationException(SR.Format(SR.Thread_ApartmentState_ChangeFailed, apartmentState));
			}
			return false;
		}
		return true;
	}

	public void DisableComObjectEagerCleanup()
	{
		DisableComObjectEagerCleanup(GetNativeHandle());
		GC.KeepAlive(this);
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_DisableComObjectEagerCleanup", ExactSpelling = true)]
	[SuppressGCTransition]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_DisableComObjectEagerCleanup")]
	private static extern void DisableComObjectEagerCleanup(ThreadHandle t);

	public void Interrupt()
	{
		Interrupt(GetNativeHandle());
		GC.KeepAlive(this);
	}

	[DllImport("QCall", EntryPoint = "ThreadNative_Interrupt", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_Interrupt")]
	private static extern void Interrupt(ThreadHandle t);

	[LibraryImport("QCall", EntryPoint = "ThreadNative_Join")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool Join(ObjectHandleOnStack thread, int millisecondsTimeout)
	{
		return __PInvoke(thread, millisecondsTimeout) != 0;
		[DllImport("QCall", EntryPoint = "ThreadNative_Join", ExactSpelling = true)]
		static extern int __PInvoke(ObjectHandleOnStack __thread_native, int __millisecondsTimeout_native);
	}

	public bool Join(int millisecondsTimeout)
	{
		if (millisecondsTimeout < 0 && millisecondsTimeout != -1)
		{
			throw new ArgumentOutOfRangeException("millisecondsTimeout", SR.ArgumentOutOfRange_NeedNonNegOrNegative1);
		}
		Thread o = this;
		return Join(ObjectHandleOnStack.Create(ref o), millisecondsTimeout);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[BypassReadyToRun]
	[DebuggerHidden]
	[DebuggerStepThrough]
	internal unsafe static StaticsHelpers.ThreadLocalData* GetThreadStaticsBase()
	{
		return (StaticsHelpers.ThreadLocalData*)Unsafe.AsPointer(in DirectOnThreadLocalData.pNativeThread) - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ResetFinalizerThread()
	{
		if (_mayNeedResetForThreadPool)
		{
			ResetFinalizerThreadSlow();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ResetFinalizerThreadSlow()
	{
		_mayNeedResetForThreadPool = false;
		if (Name != ".NET Finalizer")
		{
			Name = ".NET Finalizer";
		}
		if (!IsBackground)
		{
			IsBackground = true;
		}
		if (Priority != ThreadPriority.Highest)
		{
			Priority = ThreadPriority.Highest;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CatchAtSafePoint();

	[DllImport("QCall", EntryPoint = "ThreadNative_PollGC", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ThreadNative_PollGC")]
	private static extern void PollGCInternal();

	private static void PollGC()
	{
		if (CatchAtSafePoint())
		{
			PollGCWorker();
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		static void PollGCWorker()
		{
			PollGCInternal();
		}
	}

	public Thread(ThreadStart start)
	{
		ArgumentNullException.ThrowIfNull(start, "start");
		_startHelper = new StartHelper(start);
		Initialize();
	}

	public Thread(ThreadStart start, int maxStackSize)
	{
		ArgumentNullException.ThrowIfNull(start, "start");
		ArgumentOutOfRangeException.ThrowIfNegative(maxStackSize, "maxStackSize");
		_startHelper = new StartHelper(start)
		{
			_maxStackSize = maxStackSize
		};
		Initialize();
	}

	public Thread(ParameterizedThreadStart start)
	{
		ArgumentNullException.ThrowIfNull(start, "start");
		_startHelper = new StartHelper(start);
		Initialize();
	}

	public Thread(ParameterizedThreadStart start, int maxStackSize)
	{
		ArgumentNullException.ThrowIfNull(start, "start");
		ArgumentOutOfRangeException.ThrowIfNegative(maxStackSize, "maxStackSize");
		_startHelper = new StartHelper(start)
		{
			_maxStackSize = maxStackSize
		};
		Initialize();
	}

	internal static void ThrowIfNoThreadStart()
	{
		_ = 1;
	}

	[UnsupportedOSPlatform("browser")]
	public void Start(object? parameter)
	{
		Start(parameter, captureContext: true);
	}

	[UnsupportedOSPlatform("browser")]
	public void UnsafeStart(object? parameter)
	{
		Start(parameter, captureContext: false);
	}

	private void Start(object parameter, bool captureContext)
	{
		ThrowIfNoThreadStart();
		StartHelper startHelper = _startHelper;
		if (startHelper != null)
		{
			if (startHelper._start is ThreadStart)
			{
				throw new InvalidOperationException(SR.InvalidOperation_ThreadWrongThreadStart);
			}
			startHelper._startArg = parameter;
			startHelper._executionContext = (captureContext ? System.Threading.ExecutionContext.Capture() : null);
		}
		StartCore();
	}

	[UnsupportedOSPlatform("browser")]
	public void Start()
	{
		Start(captureContext: true);
	}

	[UnsupportedOSPlatform("browser")]
	public void UnsafeStart()
	{
		Start(captureContext: false);
	}

	private void Start(bool captureContext)
	{
		ThrowIfNoThreadStart();
		StartHelper startHelper = _startHelper;
		if (startHelper != null)
		{
			startHelper._startArg = null;
			startHelper._executionContext = (captureContext ? System.Threading.ExecutionContext.Capture() : null);
		}
		StartCore();
	}

	private void RequireCurrentThread()
	{
		if (this != CurrentThread)
		{
			throw new InvalidOperationException(SR.Thread_Operation_RequiresCurrentThread);
		}
	}

	private void SetCultureOnUnstartedThread(CultureInfo value, bool uiCulture)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		StartHelper startHelper = _startHelper;
		if ((ThreadState & ThreadState.Unstarted) == 0)
		{
			throw new InvalidOperationException(SR.Thread_Operation_RequiresCurrentThread);
		}
		if (uiCulture)
		{
			startHelper._uiCulture = value;
		}
		else
		{
			startHelper._culture = value;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void Sleep(int millisecondsTimeout)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(millisecondsTimeout, -1, "millisecondsTimeout");
		SleepInternal(millisecondsTimeout);
	}

	[Intrinsic]
	internal static void FastPollGC()
	{
		FastPollGC();
	}

	internal void SetThreadPoolWorkerThreadName()
	{
		lock (this)
		{
			_name = ".NET TP Worker";
			ThreadNameChanged(".NET TP Worker");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ResetThreadPoolThread()
	{
		if (_mayNeedResetForThreadPool)
		{
			ResetThreadPoolThreadSlow();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void ResetThreadPoolThreadSlow()
	{
		_mayNeedResetForThreadPool = false;
		if (_name != ".NET TP Worker")
		{
			SetThreadPoolWorkerThreadName();
		}
		if (!IsBackground)
		{
			IsBackground = true;
		}
		if (Priority != ThreadPriority.Normal)
		{
			Priority = ThreadPriority.Normal;
		}
	}

	[Obsolete("Thread.Abort is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Abort()
	{
		throw new PlatformNotSupportedException(SR.PlatformNotSupported_ThreadAbort);
	}

	[Obsolete("Thread.Abort is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void Abort(object? stateInfo)
	{
		throw new PlatformNotSupportedException(SR.PlatformNotSupported_ThreadAbort);
	}

	[Obsolete("Thread.ResetAbort is not supported and throws PlatformNotSupportedException.", DiagnosticId = "SYSLIB0006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void ResetAbort()
	{
		throw new PlatformNotSupportedException(SR.PlatformNotSupported_ThreadAbort);
	}

	[Obsolete("Thread.Suspend has been deprecated. Use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.")]
	public void Suspend()
	{
		throw new PlatformNotSupportedException(SR.PlatformNotSupported_ThreadSuspend);
	}

	[Obsolete("Thread.Resume has been deprecated. Use other classes in System.Threading, such as Monitor, Mutex, Event, and Semaphore, to synchronize Threads or protect resources.")]
	public void Resume()
	{
		throw new PlatformNotSupportedException(SR.PlatformNotSupported_ThreadSuspend);
	}

	public static void BeginCriticalRegion()
	{
	}

	public static void EndCriticalRegion()
	{
	}

	public static void BeginThreadAffinity()
	{
	}

	public static void EndThreadAffinity()
	{
	}

	public static LocalDataStoreSlot AllocateDataSlot()
	{
		return LocalDataStore.AllocateSlot();
	}

	public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
	{
		return LocalDataStore.AllocateNamedSlot(name);
	}

	public static LocalDataStoreSlot GetNamedDataSlot(string name)
	{
		return LocalDataStore.GetNamedSlot(name);
	}

	public static void FreeNamedDataSlot(string name)
	{
		LocalDataStore.FreeNamedSlot(name);
	}

	public static object? GetData(LocalDataStoreSlot slot)
	{
		return LocalDataStore.GetData(slot);
	}

	public static void SetData(LocalDataStoreSlot slot, object? data)
	{
		LocalDataStore.SetData(slot, data);
	}

	[SupportedOSPlatform("windows")]
	public void SetApartmentState(ApartmentState state)
	{
		SetApartmentState(state, throwOnError: true);
	}

	public bool TrySetApartmentState(ApartmentState state)
	{
		return SetApartmentState(state, throwOnError: false);
	}

	private bool SetApartmentState(ApartmentState state, bool throwOnError)
	{
		if ((uint)state > 2u)
		{
			throw new ArgumentOutOfRangeException("state", SR.ArgumentOutOfRange_Enum);
		}
		return SetApartmentStateUnchecked(state, throwOnError);
	}

	[Obsolete("Code Access Security is not supported or honored by the runtime.", DiagnosticId = "SYSLIB0003", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CompressedStack GetCompressedStack()
	{
		throw new InvalidOperationException(SR.Thread_GetSetCompressedStack_NotSupported);
	}

	[Obsolete("Code Access Security is not supported or honored by the runtime.", DiagnosticId = "SYSLIB0003", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void SetCompressedStack(CompressedStack stack)
	{
		throw new InvalidOperationException(SR.Thread_GetSetCompressedStack_NotSupported);
	}

	public static AppDomain GetDomain()
	{
		return AppDomain.CurrentDomain;
	}

	public static int GetDomainID()
	{
		return 1;
	}

	public override int GetHashCode()
	{
		return ManagedThreadId;
	}

	public void Join()
	{
		Join(-1);
	}

	public bool Join(TimeSpan timeout)
	{
		return Join(WaitHandle.ToTimeoutMilliseconds(timeout));
	}

	public static void MemoryBarrier()
	{
		Interlocked.MemoryBarrier();
	}

	public static void Sleep(TimeSpan timeout)
	{
		Sleep(WaitHandle.ToTimeoutMilliseconds(timeout));
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static byte VolatileRead(ref byte address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static double VolatileRead(ref double address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static short VolatileRead(ref short address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int VolatileRead(ref int address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static long VolatileRead(ref long address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint VolatileRead(ref nint address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[return: NotNullIfNotNull("address")]
	public static object? VolatileRead([NotNullIfNotNull("address")] ref object? address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static sbyte VolatileRead(ref sbyte address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static float VolatileRead(ref float address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static ushort VolatileRead(ref ushort address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static uint VolatileRead(ref uint address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static ulong VolatileRead(ref ulong address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static nuint VolatileRead(ref nuint address)
	{
		return Volatile.Read(in address);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref byte address, byte value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref double address, double value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref short address, short value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref int address, int value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref long address, long value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref nint address, nint value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite([NotNullIfNotNull("value")] ref object? address, object? value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref sbyte address, sbyte value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void VolatileWrite(ref float address, float value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref ushort address, ushort value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref uint address, uint value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref ulong address, ulong value)
	{
		Volatile.Write(ref address, value);
	}

	[Obsolete("Thread.VolatileRead and Thread.VolatileWrite are obsolete. Use Volatile.Read or Volatile.Write respectively instead.", DiagnosticId = "SYSLIB0054", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[CLSCompliant(false)]
	public static void VolatileWrite(ref nuint address, nuint value)
	{
		Volatile.Write(ref address, value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetCurrentProcessorId()
	{
		return ProcessorIdCache.GetCurrentProcessorId();
	}

	internal static void UninterruptibleSleep0()
	{
		Interop.Kernel32.Sleep(0u);
	}

	internal static int GetCurrentProcessorNumber()
	{
		Interop.Kernel32.GetCurrentProcessorNumberEx(out var ProcNumber);
		return (ProcNumber.Group << 6) | ProcNumber.Number;
	}
}
