using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System;

[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class Exception : ISerializable
{
	internal enum ExceptionMessageKind
	{
		ThreadAbort = 1,
		ThreadInterrupted,
		OutOfMemory
	}

	internal readonly struct DispatchState(object stackTrace, string remoteStackTrace, nuint ipForWatsonBuckets, byte[] watsonBuckets)
	{
		public readonly object StackTrace = stackTrace;

		public readonly string RemoteStackTrace = remoteStackTrace;

		public readonly nuint IpForWatsonBuckets = ipForWatsonBuckets;

		public readonly byte[] WatsonBuckets = watsonBuckets;
	}

	private MethodBase _exceptionMethod;

	internal string _message;

	private IDictionary _data;

	private readonly Exception _innerException;

	private string _helpURL;

	private object _stackTrace;

	private byte[] _watsonBuckets;

	private string _stackTraceString;

	private string _remoteStackTraceString;

	private string _source;

	private nuint _ipForWatsonBuckets;

	private readonly nint _xptrs;

	private readonly int _xcode = -532462766;

	private int _HResult;

	private const int EXCEPTION_COMPLUS = -532462766;

	private protected const string InnerExceptionPrefix = " ---> ";

	public MethodBase? TargetSite
	{
		[RequiresUnreferencedCode("Metadata for the method might be incomplete or removed")]
		get
		{
			return _exceptionMethod ?? (_exceptionMethod = GetExceptionMethodFromStackTrace());
		}
	}

	private bool HasBeenThrown => _stackTrace != null;

	private object? SerializationWatsonBuckets => _watsonBuckets;

	public virtual string Message => _message ?? SR.Format(SR.Exception_WasThrown, GetClassName());

	public virtual IDictionary Data => _data ?? (_data = CreateDataContainer());

	public Exception? InnerException => _innerException;

	public virtual string? HelpLink
	{
		get
		{
			return _helpURL;
		}
		set
		{
			_helpURL = value;
		}
	}

	public virtual string? Source
	{
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The API will return <unknown> if the metadata for current method cannot be established.")]
		get
		{
			return _source ?? (_source = ((!HasBeenThrown) ? null : (TargetSite?.Module.Assembly.GetName().Name ?? "<unknown>")));
		}
		set
		{
			_source = value;
		}
	}

	public int HResult
	{
		get
		{
			return _HResult;
		}
		set
		{
			_HResult = value;
		}
	}

	public virtual string? StackTrace
	{
		get
		{
			string stackTraceString = _stackTraceString;
			string remoteStackTraceString = _remoteStackTraceString;
			if (stackTraceString != null)
			{
				return remoteStackTraceString + stackTraceString;
			}
			if (!HasBeenThrown)
			{
				return remoteStackTraceString;
			}
			return remoteStackTraceString + GetStackTrace();
		}
	}

	private string? SerializationStackTraceString
	{
		get
		{
			string text = _stackTraceString;
			if (text == null && HasBeenThrown)
			{
				text = GetStackTrace();
			}
			return text;
		}
	}

	[Obsolete("BinaryFormatter serialization is obsolete and should not be used. See https://aka.ms/binaryformatter for more information.", DiagnosticId = "SYSLIB0011", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected event EventHandler<SafeSerializationEventArgs>? SerializeObjectState
	{
		add
		{
			throw new PlatformNotSupportedException(SR.PlatformNotSupported_SecureBinarySerialization);
		}
		remove
		{
			throw new PlatformNotSupportedException(SR.PlatformNotSupported_SecureBinarySerialization);
		}
	}

	private IDictionary CreateDataContainer()
	{
		if (IsImmutableAgileException(this))
		{
			return new EmptyReadOnlyDictionaryInternal();
		}
		return new ListDictionaryInternal();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsImmutableAgileException(Exception e);

	[DllImport("QCall", EntryPoint = "ExceptionNative_GetMethodFromStackTrace", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_GetMethodFromStackTrace")]
	private static extern void GetMethodFromStackTrace(ObjectHandleOnStack stackTrace, ObjectHandleOnStack method);

	private MethodBase GetExceptionMethodFromStackTrace()
	{
		object o = _stackTrace;
		if (o == null)
		{
			return null;
		}
		IRuntimeMethodInfo o2 = null;
		GetMethodFromStackTrace(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref o2));
		return RuntimeType.GetMethodBase(o2);
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		_stackTrace = null;
		_ipForWatsonBuckets = UIntPtr.Zero;
	}

	internal void InternalPreserveStackTrace()
	{
		_ = Source;
		string stackTrace = StackTrace;
		if (!string.IsNullOrEmpty(stackTrace))
		{
			_remoteStackTraceString = stackTrace + "\r\n";
		}
		_stackTrace = null;
		_stackTraceString = null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PrepareForForeignExceptionRaise();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern uint GetExceptionCount();

	internal void RestoreDispatchState(in DispatchState dispatchState)
	{
		if (!IsImmutableAgileException(this))
		{
			_watsonBuckets = dispatchState.WatsonBuckets;
			_ipForWatsonBuckets = dispatchState.IpForWatsonBuckets;
			_remoteStackTraceString = dispatchState.RemoteStackTrace;
			_stackTrace = dispatchState.StackTrace;
			_stackTraceString = null;
			PrepareForForeignExceptionRaise();
		}
	}

	internal static string GetMessageFromNativeResources(ExceptionMessageKind kind)
	{
		string s = null;
		GetMessageFromNativeResources(kind, new StringHandleOnStack(ref s));
		return s;
	}

	[DllImport("QCall", EntryPoint = "ExceptionNative_GetMessageFromNativeResources", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_GetMessageFromNativeResources")]
	private static extern void GetMessageFromNativeResources(ExceptionMessageKind kind, StringHandleOnStack retMesg);

	[DllImport("QCall", EntryPoint = "ExceptionNative_GetFrozenStackTrace", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ExceptionNative_GetFrozenStackTrace")]
	private static extern void GetFrozenStackTrace(ObjectHandleOnStack exception, ObjectHandleOnStack stackTrace);

	internal DispatchState CaptureDispatchState()
	{
		Exception o = this;
		object o2 = null;
		GetFrozenStackTrace(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref o2));
		return new DispatchState(o2, _remoteStackTraceString, _ipForWatsonBuckets, _watsonBuckets);
	}

	private bool CanSetRemoteStackTrace()
	{
		if (IsImmutableAgileException(this))
		{
			return false;
		}
		if (_stackTrace != null || _stackTraceString != null || _remoteStackTraceString != null)
		{
			ThrowHelper.ThrowInvalidOperationException();
		}
		return true;
	}

	internal string GetHelpContext(out uint helpContext)
	{
		helpContext = 0u;
		string text = HelpLink;
		int num;
		if (text == null || (num = text.LastIndexOf('#')) == -1)
		{
			return text;
		}
		int i;
		for (i = num + 1; i < text.Length && !char.IsWhiteSpace(text[i]); i++)
		{
		}
		if (uint.TryParse(text.AsSpan(num + 1, i - num - 1), out helpContext))
		{
			text = text.Substring(0, num);
		}
		return text;
	}

	public Exception()
	{
		_HResult = -2146233088;
	}

	public Exception(string? message)
		: this()
	{
		_message = message;
	}

	public Exception(string? message, Exception? innerException)
		: this()
	{
		_message = message;
		_innerException = innerException;
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected Exception(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		_message = info.GetString("Message");
		_data = (IDictionary)info.GetValueNoThrow("Data", typeof(IDictionary));
		_innerException = (Exception)info.GetValue("InnerException", typeof(Exception));
		_helpURL = info.GetString("HelpURL");
		_stackTraceString = info.GetString("StackTraceString");
		_remoteStackTraceString = info.GetString("RemoteStackTraceString");
		_HResult = info.GetInt32("HResult");
		_source = info.GetString("Source");
		RestoreRemoteStackTrace(info, context);
	}

	private string GetClassName()
	{
		return GetType().ToString();
	}

	public virtual Exception GetBaseException()
	{
		Exception innerException = InnerException;
		Exception result = this;
		while (innerException != null)
		{
			result = innerException;
			innerException = innerException.InnerException;
		}
		return result;
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		if (_source == null)
		{
			_source = Source;
		}
		info.AddValue("ClassName", GetClassName(), typeof(string));
		info.AddValue("Message", _message, typeof(string));
		info.AddValue("Data", _data, typeof(IDictionary));
		info.AddValue("InnerException", _innerException, typeof(Exception));
		info.AddValue("HelpURL", _helpURL, typeof(string));
		info.AddValue("StackTraceString", SerializationStackTraceString, typeof(string));
		info.AddValue("RemoteStackTraceString", _remoteStackTraceString, typeof(string));
		info.AddValue("RemoteStackIndex", 0, typeof(int));
		info.AddValue("ExceptionMethod", null, typeof(string));
		info.AddValue("HResult", _HResult);
		info.AddValue("Source", _source, typeof(string));
		info.AddValue("WatsonBuckets", SerializationWatsonBuckets, typeof(byte[]));
	}

	public override string ToString()
	{
		string className = GetClassName();
		string message = Message;
		string text = _innerException?.ToString() ?? "";
		string exception_EndOfInnerExceptionStack = SR.Exception_EndOfInnerExceptionStack;
		string stackTrace = StackTrace;
		int num = className.Length;
		checked
		{
			if (!string.IsNullOrEmpty(message))
			{
				num += 2 + message.Length;
			}
			if (_innerException != null)
			{
				num += "\r\n".Length + " ---> ".Length + text.Length + "\r\n".Length + 3 + exception_EndOfInnerExceptionStack.Length;
			}
			if (stackTrace != null)
			{
				num += "\r\n".Length + stackTrace.Length;
			}
			string text2 = string.FastAllocateString(num);
			Span<char> dest = new Span<char>(ref text2.GetRawStringData(), text2.Length);
			Write(className, ref dest);
			if (!string.IsNullOrEmpty(message))
			{
				Write(": ", ref dest);
				Write(message, ref dest);
			}
			if (_innerException != null)
			{
				Write("\r\n", ref dest);
				Write(" ---> ", ref dest);
				Write(text, ref dest);
				Write("\r\n", ref dest);
				Write("   ", ref dest);
				Write(exception_EndOfInnerExceptionStack, ref dest);
			}
			if (stackTrace != null)
			{
				Write("\r\n", ref dest);
				Write(stackTrace, ref dest);
			}
			return text2;
		}
		static void Write(string source, ref Span<char> reference)
		{
			source.CopyTo(reference);
			reference = reference.Slice(source.Length);
		}
	}

	public new Type GetType()
	{
		return base.GetType();
	}

	private void RestoreRemoteStackTrace(SerializationInfo info, StreamingContext context)
	{
		_watsonBuckets = (byte[])info.GetValueNoThrow("WatsonBuckets", typeof(byte[]));
		if (context.State == StreamingContextStates.CrossAppDomain)
		{
			_remoteStackTraceString += _stackTraceString;
			_stackTraceString = null;
		}
	}

	private string GetStackTrace()
	{
		return new StackTrace(this, fNeedFileInfo: true).ToString(System.Diagnostics.StackTrace.TraceFormat.Normal);
	}

	[StackTraceHidden]
	internal void SetCurrentStackTrace()
	{
		if (CanSetRemoteStackTrace())
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			new StackTrace(fNeedFileInfo: true).ToString(System.Diagnostics.StackTrace.TraceFormat.TrailingNewLine, stringBuilder);
			stringBuilder.AppendLine(SR.Exception_EndStackTraceFromPreviousThrow);
			_remoteStackTraceString = stringBuilder.ToString();
		}
	}

	internal void SetRemoteStackTrace(string stackTrace)
	{
		if (CanSetRemoteStackTrace())
		{
			_remoteStackTraceString = stackTrace + "\r\n" + SR.Exception_EndStackTraceFromPreviousThrow + "\r\n";
		}
	}
}
