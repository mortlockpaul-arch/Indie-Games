using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace System.Net;

[EventSource(Name = "Private.InternalDiagnostics.System.Net.WebSockets")]
internal sealed class NetEventSource : EventSource
{
	public static class Keywords
	{
		public const EventKeywords Default = (EventKeywords)1L;

		public const EventKeywords Debug = (EventKeywords)2L;
	}

	public static readonly System.Net.NetEventSource Log = new System.Net.NetEventSource();

	[Event(5, Keywords = (EventKeywords)2L, Level = EventLevel.Informational)]
	private void KeepAliveSent(string objName, string opcode, long payload)
	{
		WriteEvent(5, objName, opcode, payload);
	}

	[Event(6, Keywords = (EventKeywords)2L, Level = EventLevel.Informational)]
	private void KeepAliveAcked(string objName, long payload)
	{
		WriteEvent(6, objName, payload);
	}

	[NonEvent]
	public static void KeepAlivePingSent(object obj, long payload)
	{
		Log.KeepAliveSent(IdOf(obj), "Ping", payload);
	}

	[NonEvent]
	public static void PongResponseReceived(object obj, long payload)
	{
		Log.KeepAliveAcked(IdOf(obj), payload);
	}

	[Event(7, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void WsTrace(string objName, string memberName, string message)
	{
		WriteEvent(7, objName, memberName, message);
	}

	[NonEvent]
	public static void TraceErrorMsg(object obj, Exception exception, [CallerMemberName] string memberName = null)
	{
		Trace(obj, exception.GetType().Name + ": " + exception.Message, memberName);
	}

	[NonEvent]
	public static void TraceException(object obj, Exception exception, [CallerMemberName] string memberName = null)
	{
		Trace(obj, exception.ToString(), memberName);
	}

	[NonEvent]
	public static void Trace(object obj, string message = null, [CallerMemberName] string memberName = null)
	{
		Log.WsTrace(IdOf(obj), memberName ?? "(?)", message ?? memberName ?? string.Empty);
	}

	[Event(8, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void CloseStart(string objName, string memberName)
	{
		WriteEvent(8, objName, memberName);
	}

	[Event(9, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void CloseStop(string objName, string memberName)
	{
		WriteEvent(9, objName, memberName);
	}

	[NonEvent]
	public static void CloseAsyncPrivateStarted(object obj, [CallerMemberName] string memberName = null)
	{
		Log.CloseStart(IdOf(obj), memberName ?? "(?)");
	}

	[NonEvent]
	public static void CloseAsyncPrivateCompleted(object obj, [CallerMemberName] string memberName = null)
	{
		Log.CloseStop(IdOf(obj), memberName ?? "(?)");
	}

	[Event(10, Keywords = (EventKeywords)2L, Level = EventLevel.Informational)]
	private void ReceiveStart(string objName, string memberName, int bufferLength)
	{
		WriteEvent(10, objName, memberName, bufferLength);
	}

	[Event(11, Keywords = (EventKeywords)2L, Level = EventLevel.Informational)]
	private void ReceiveStop(string objName, string memberName)
	{
		WriteEvent(11, objName, memberName);
	}

	[NonEvent]
	public static void ReceiveAsyncPrivateStarted(object obj, int bufferLength, [CallerMemberName] string memberName = null)
	{
		Log.ReceiveStart(IdOf(obj), memberName ?? "(?)", bufferLength);
	}

	[NonEvent]
	public static void ReceiveAsyncPrivateCompleted(object obj, [CallerMemberName] string memberName = null)
	{
		Log.ReceiveStop(IdOf(obj), memberName ?? "(?)");
	}

	[Event(12, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void SendStart(string objName, string memberName, string opcode, int bufferLength)
	{
		WriteEvent(12, objName, memberName, opcode, bufferLength);
	}

	[Event(13, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void SendStop(string objName, string memberName)
	{
		WriteEvent(13, objName, memberName);
	}

	[NonEvent]
	public static void SendFrameAsyncStarted(object obj, string opcode, int bufferLength, [CallerMemberName] string memberName = null)
	{
		Log.SendStart(IdOf(obj), memberName ?? "(?)", opcode, bufferLength);
	}

	[NonEvent]
	public static void SendFrameAsyncCompleted(object obj, [CallerMemberName] string memberName = null)
	{
		Log.SendStop(IdOf(obj), memberName ?? "(?)");
	}

	[Event(14, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void MutexEnter(string objName, string memberName)
	{
		WriteEvent(14, objName, memberName);
	}

	[Event(15, Keywords = (EventKeywords)2L, Level = EventLevel.Verbose)]
	private void MutexExit(string objName, string memberName)
	{
		WriteEvent(15, objName, memberName);
	}

	[NonEvent]
	public static void MutexEntered(object obj, [CallerMemberName] string memberName = null)
	{
		Log.MutexEnter(IdOf(obj), memberName ?? "(?)");
	}

	[NonEvent]
	public static void MutexExited(object obj, [CallerMemberName] string memberName = null)
	{
		Log.MutexExit(IdOf(obj), memberName ?? "(?)");
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, string arg1, string arg2, long arg3)
	{
		//The blocks IL_0021 are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		fixed (char* dataPointer = arg1)
		{
			char* intPtr;
			EventData* intPtr2;
			nint num;
			nint num2;
			if (arg2 == null)
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = null);
				EventData* ptr = stackalloc EventData[3];
				intPtr2 = ptr;
				*intPtr2 = new EventData
				{
					DataPointer = (nint)dataPointer,
					Size = (arg1.Length + 1) * 2
				};
				num = (nint)(ptr + 1);
				*(EventData*)num = new EventData
				{
					DataPointer = (nint)dataPointer2,
					Size = (arg2.Length + 1) * 2
				};
				num2 = (nint)(ptr + 2);
				*(EventData*)num2 = new EventData
				{
					DataPointer = (nint)(&arg3),
					Size = 8
				};
				WriteEventCore(eventId, 3, ptr);
				return;
			}
			fixed (char* ptr2 = &arg2.GetPinnableReference())
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = ptr2);
				EventData* ptr = stackalloc EventData[3];
				intPtr2 = ptr;
				*intPtr2 = new EventData
				{
					DataPointer = (nint)dataPointer,
					Size = (arg1.Length + 1) * 2
				};
				num = (nint)(ptr + 1);
				*(EventData*)num = new EventData
				{
					DataPointer = (nint)dataPointer2,
					Size = (arg2.Length + 1) * 2
				};
				num2 = (nint)(ptr + 2);
				*(EventData*)num2 = new EventData
				{
					DataPointer = (nint)(&arg3),
					Size = 8
				};
				WriteEventCore(eventId, 3, ptr);
			}
		}
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3, int arg4)
	{
		//The blocks IL_0021, IL_0024, IL_0036, IL_010f are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		fixed (char* dataPointer = arg1)
		{
			char* intPtr;
			EventData* intPtr2;
			nint num;
			nint num2;
			nint num3;
			if (arg2 == null)
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = null);
				fixed (char* ptr = arg3)
				{
					char* dataPointer3 = ptr;
					EventData* ptr2 = stackalloc EventData[4];
					intPtr2 = ptr2;
					*intPtr2 = new EventData
					{
						DataPointer = (nint)dataPointer,
						Size = (arg1.Length + 1) * 2
					};
					num = (nint)(ptr2 + 1);
					*(EventData*)num = new EventData
					{
						DataPointer = (nint)dataPointer2,
						Size = (arg2.Length + 1) * 2
					};
					num2 = (nint)(ptr2 + 2);
					*(EventData*)num2 = new EventData
					{
						DataPointer = (nint)dataPointer3,
						Size = (arg3.Length + 1) * 2
					};
					num3 = (nint)(ptr2 + 3);
					*(EventData*)num3 = new EventData
					{
						DataPointer = (nint)(&arg4),
						Size = 4
					};
					WriteEventCore(eventId, 4, ptr2);
				}
				return;
			}
			fixed (char* ptr3 = &arg2.GetPinnableReference())
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = ptr3);
				fixed (char* ptr = arg3)
				{
					char* dataPointer3 = ptr;
					EventData* ptr2 = stackalloc EventData[4];
					intPtr2 = ptr2;
					*intPtr2 = new EventData
					{
						DataPointer = (nint)dataPointer,
						Size = (arg1.Length + 1) * 2
					};
					num = (nint)(ptr2 + 1);
					*(EventData*)num = new EventData
					{
						DataPointer = (nint)dataPointer2,
						Size = (arg2.Length + 1) * 2
					};
					num2 = (nint)(ptr2 + 2);
					*(EventData*)num2 = new EventData
					{
						DataPointer = (nint)dataPointer3,
						Size = (arg3.Length + 1) * 2
					};
					num3 = (nint)(ptr2 + 3);
					*(EventData*)num3 = new EventData
					{
						DataPointer = (nint)(&arg4),
						Size = 4
					};
					WriteEventCore(eventId, 4, ptr2);
				}
			}
		}
	}

	[NonEvent]
	public static string IdOf(object value)
	{
		if (value == null)
		{
			return "(null)";
		}
		return value.GetType().Name + "#" + GetHashCode(value);
	}

	[NonEvent]
	public static int GetHashCode(object value)
	{
		return value?.GetHashCode() ?? 0;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, string arg1, string arg2, int arg3)
	{
		//The blocks IL_0035 are reachable both inside and outside the pinned region starting at IL_0032. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (arg1 == null)
		{
			arg1 = "";
		}
		if (arg2 == null)
		{
			arg2 = "";
		}
		fixed (char* dataPointer = arg1)
		{
			char* intPtr;
			EventData* intPtr2;
			nint num;
			nint num2;
			if (arg2 == null)
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = null);
				EventData* ptr = stackalloc EventData[3];
				intPtr2 = ptr;
				*intPtr2 = new EventData
				{
					DataPointer = (nint)dataPointer,
					Size = (arg1.Length + 1) * 2
				};
				num = (nint)(ptr + 1);
				*(EventData*)num = new EventData
				{
					DataPointer = (nint)dataPointer2,
					Size = (arg2.Length + 1) * 2
				};
				num2 = (nint)(ptr + 2);
				*(EventData*)num2 = new EventData
				{
					DataPointer = (nint)(&arg3),
					Size = 4
				};
				WriteEventCore(eventId, 3, ptr);
				return;
			}
			fixed (char* ptr2 = &arg2.GetPinnableReference())
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = ptr2);
				EventData* ptr = stackalloc EventData[3];
				intPtr2 = ptr;
				*intPtr2 = new EventData
				{
					DataPointer = (nint)dataPointer,
					Size = (arg1.Length + 1) * 2
				};
				num = (nint)(ptr + 1);
				*(EventData*)num = new EventData
				{
					DataPointer = (nint)dataPointer2,
					Size = (arg2.Length + 1) * 2
				};
				num2 = (nint)(ptr + 2);
				*(EventData*)num2 = new EventData
				{
					DataPointer = (nint)(&arg3),
					Size = 4
				};
				WriteEventCore(eventId, 3, ptr);
			}
		}
	}

	[NonEvent]
	public static void Associate(object first, object second, [CallerMemberName] string memberName = null)
	{
		Associate(first, first, second, memberName);
	}

	[NonEvent]
	public static void Associate(object thisOrContextObject, object first, object second, [CallerMemberName] string memberName = null)
	{
		Log.Associate(IdOf(thisOrContextObject), memberName, IdOf(first), IdOf(second));
	}

	[Event(3, Level = EventLevel.Informational, Keywords = (EventKeywords)1L, Message = "[{2}]<-->[{3}]")]
	private void Associate(string thisOrContextObject, string memberName, string first, string second)
	{
		WriteEvent(3, thisOrContextObject, memberName ?? "(?)", first, second);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Parameters to this method are primitive and are trimmer safe")]
	[NonEvent]
	private unsafe void WriteEvent(int eventId, string arg1, string arg2, string arg3, string arg4)
	{
		//The blocks IL_004b, IL_004e, IL_0060, IL_0066, IL_006a, IL_0075, IL_0076, IL_0158, IL_015c are reachable both inside and outside the pinned region starting at IL_0048. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0076 are reachable both inside and outside the pinned region starting at IL_0071. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0076 are reachable both inside and outside the pinned region starting at IL_0071. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (arg1 == null)
		{
			arg1 = "";
		}
		if (arg2 == null)
		{
			arg2 = "";
		}
		if (arg3 == null)
		{
			arg3 = "";
		}
		if (arg4 == null)
		{
			arg4 = "";
		}
		fixed (char* dataPointer = arg1)
		{
			char* intPtr;
			char* intPtr2;
			EventData* intPtr3;
			nint num;
			nint num2;
			nint num3;
			if (arg2 == null)
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = null);
				fixed (char* ptr = arg3)
				{
					char* dataPointer3 = ptr;
					if (arg4 == null)
					{
						char* dataPointer4;
						intPtr2 = (dataPointer4 = null);
						EventData* ptr2 = stackalloc EventData[4];
						intPtr3 = ptr2;
						*intPtr3 = new EventData
						{
							DataPointer = (nint)dataPointer,
							Size = (arg1.Length + 1) * 2
						};
						num = (nint)(ptr2 + 1);
						*(EventData*)num = new EventData
						{
							DataPointer = (nint)dataPointer2,
							Size = (arg2.Length + 1) * 2
						};
						num2 = (nint)(ptr2 + 2);
						*(EventData*)num2 = new EventData
						{
							DataPointer = (nint)dataPointer3,
							Size = (arg3.Length + 1) * 2
						};
						num3 = (nint)(ptr2 + 3);
						*(EventData*)num3 = new EventData
						{
							DataPointer = (nint)dataPointer4,
							Size = (arg4.Length + 1) * 2
						};
						WriteEventCore(eventId, 4, ptr2);
						return;
					}
					fixed (char* ptr3 = &arg4.GetPinnableReference())
					{
						char* dataPointer4;
						intPtr2 = (dataPointer4 = ptr3);
						EventData* ptr2 = stackalloc EventData[4];
						intPtr3 = ptr2;
						*intPtr3 = new EventData
						{
							DataPointer = (nint)dataPointer,
							Size = (arg1.Length + 1) * 2
						};
						num = (nint)(ptr2 + 1);
						*(EventData*)num = new EventData
						{
							DataPointer = (nint)dataPointer2,
							Size = (arg2.Length + 1) * 2
						};
						num2 = (nint)(ptr2 + 2);
						*(EventData*)num2 = new EventData
						{
							DataPointer = (nint)dataPointer3,
							Size = (arg3.Length + 1) * 2
						};
						num3 = (nint)(ptr2 + 3);
						*(EventData*)num3 = new EventData
						{
							DataPointer = (nint)dataPointer4,
							Size = (arg4.Length + 1) * 2
						};
						WriteEventCore(eventId, 4, ptr2);
					}
				}
				return;
			}
			fixed (char* ptr4 = &arg2.GetPinnableReference())
			{
				char* dataPointer2;
				intPtr = (dataPointer2 = ptr4);
				fixed (char* ptr = arg3)
				{
					char* dataPointer3 = ptr;
					if (arg4 == null)
					{
						char* dataPointer4;
						intPtr2 = (dataPointer4 = null);
						EventData* ptr2 = stackalloc EventData[4];
						intPtr3 = ptr2;
						*intPtr3 = new EventData
						{
							DataPointer = (nint)dataPointer,
							Size = (arg1.Length + 1) * 2
						};
						num = (nint)(ptr2 + 1);
						*(EventData*)num = new EventData
						{
							DataPointer = (nint)dataPointer2,
							Size = (arg2.Length + 1) * 2
						};
						num2 = (nint)(ptr2 + 2);
						*(EventData*)num2 = new EventData
						{
							DataPointer = (nint)dataPointer3,
							Size = (arg3.Length + 1) * 2
						};
						num3 = (nint)(ptr2 + 3);
						*(EventData*)num3 = new EventData
						{
							DataPointer = (nint)dataPointer4,
							Size = (arg4.Length + 1) * 2
						};
						WriteEventCore(eventId, 4, ptr2);
						return;
					}
					fixed (char* ptr3 = &arg4.GetPinnableReference())
					{
						char* dataPointer4;
						intPtr2 = (dataPointer4 = ptr3);
						EventData* ptr2 = stackalloc EventData[4];
						intPtr3 = ptr2;
						*intPtr3 = new EventData
						{
							DataPointer = (nint)dataPointer,
							Size = (arg1.Length + 1) * 2
						};
						num = (nint)(ptr2 + 1);
						*(EventData*)num = new EventData
						{
							DataPointer = (nint)dataPointer2,
							Size = (arg2.Length + 1) * 2
						};
						num2 = (nint)(ptr2 + 2);
						*(EventData*)num2 = new EventData
						{
							DataPointer = (nint)dataPointer3,
							Size = (arg3.Length + 1) * 2
						};
						num3 = (nint)(ptr2 + 3);
						*(EventData*)num3 = new EventData
						{
							DataPointer = (nint)dataPointer4,
							Size = (arg4.Length + 1) * 2
						};
						WriteEventCore(eventId, 4, ptr2);
					}
				}
			}
		}
	}
}
