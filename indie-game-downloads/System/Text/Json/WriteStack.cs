using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json;

[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal struct WriteStack
{
	public WriteStackFrame Current;

	private WriteStackFrame[] _stack;

	private int _count;

	private int _continuationCount;

	private byte _indexOffset;

	public CancellationToken CancellationToken;

	public bool SuppressFlush;

	public Task PendingTask;

	public List<IAsyncDisposable> CompletedAsyncDisposables;

	public int FlushThreshold;

	public PipeWriter PipeWriter;

	public ReferenceResolver ReferenceResolver;

	public bool SupportContinuation;

	public bool SupportAsync;

	public string NewReferenceId;

	public object PolymorphicTypeDiscriminator;

	public PolymorphicTypeResolver PolymorphicTypeResolver;

	public readonly int CurrentDepth => _count;

	public readonly bool IsContinuation => _continuationCount != 0;

	public readonly bool CurrentContainsMetadata
	{
		get
		{
			if (NewReferenceId == null)
			{
				return PolymorphicTypeDiscriminator != null;
			}
			return true;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"Path = {PropertyPath()} Current = ConverterStrategy.{Current.JsonPropertyInfo?.EffectiveConverter.ConverterStrategy}, {Current.JsonTypeInfo?.Type.Name}";

	private void EnsurePushCapacity()
	{
		if (_stack == null)
		{
			_stack = new WriteStackFrame[4];
		}
		else if (_count - _indexOffset == _stack.Length)
		{
			Array.Resize(ref _stack, 2 * _stack.Length);
		}
	}

	internal void Initialize(JsonTypeInfo jsonTypeInfo, object rootValueBoxed = null, bool supportContinuation = false, bool supportAsync = false)
	{
		Current.JsonTypeInfo = jsonTypeInfo;
		Current.JsonPropertyInfo = jsonTypeInfo.PropertyInfoForTypeInfo;
		Current.NumberHandling = Current.JsonPropertyInfo.EffectiveNumberHandling;
		SupportContinuation = supportContinuation;
		SupportAsync = supportAsync;
		JsonSerializerOptions options = jsonTypeInfo.Options;
		if (options.ReferenceHandlingStrategy != JsonKnownReferenceHandler.Unspecified)
		{
			ReferenceResolver = options.ReferenceHandler.CreateResolver(writing: true);
			if (options.ReferenceHandlingStrategy == JsonKnownReferenceHandler.IgnoreCycles && rootValueBoxed != null && jsonTypeInfo.Type.IsValueType)
			{
				ReferenceResolver.PushReferenceForCycleDetection(rootValueBoxed);
			}
		}
	}

	public readonly JsonTypeInfo PeekNestedJsonTypeInfo()
	{
		if (_count != 0)
		{
			return Current.JsonPropertyInfo.JsonTypeInfo;
		}
		return Current.JsonTypeInfo;
	}

	public void Push()
	{
		if (_continuationCount == 0)
		{
			if (_count == 0 && Current.PolymorphicSerializationState == PolymorphicSerializationState.None)
			{
				_count = 1;
				_indexOffset = 1;
				return;
			}
			JsonTypeInfo nestedJsonTypeInfo = Current.GetNestedJsonTypeInfo();
			JsonNumberHandling? numberHandling = Current.NumberHandling;
			EnsurePushCapacity();
			_stack[_count - _indexOffset] = Current;
			Current = default(WriteStackFrame);
			_count++;
			Current.JsonTypeInfo = nestedJsonTypeInfo;
			Current.JsonPropertyInfo = nestedJsonTypeInfo.PropertyInfoForTypeInfo;
			Current.NumberHandling = numberHandling ?? Current.JsonPropertyInfo.EffectiveNumberHandling;
		}
		else
		{
			if (_count++ > 0 || _indexOffset == 0)
			{
				Current = _stack[_count - _indexOffset];
			}
			if (_continuationCount == _count)
			{
				_continuationCount = 0;
			}
		}
	}

	public void Pop(bool success)
	{
		if (!success)
		{
			if (_continuationCount == 0)
			{
				if (_count == 1 && _indexOffset > 0)
				{
					_continuationCount = 1;
					_count = 0;
					return;
				}
				EnsurePushCapacity();
				_continuationCount = _count--;
			}
			else if (--_count == 0 && _indexOffset > 0)
			{
				return;
			}
			int num = _count - _indexOffset;
			_stack[num + 1] = Current;
			Current = _stack[num];
		}
		else if (--_count > 0 || _indexOffset == 0)
		{
			Current = _stack[_count - _indexOffset];
		}
	}

	public void AddCompletedAsyncDisposable(IAsyncDisposable asyncDisposable)
	{
		(CompletedAsyncDisposables ?? (CompletedAsyncDisposables = new List<IAsyncDisposable>())).Add(asyncDisposable);
	}

	public readonly async ValueTask DisposeCompletedAsyncDisposables()
	{
		Exception exception = null;
		foreach (IAsyncDisposable completedAsyncDisposable in CompletedAsyncDisposables)
		{
			try
			{
				await completedAsyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				exception = ex;
			}
		}
		if (exception != null)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
		}
		CompletedAsyncDisposables.Clear();
	}

	public readonly void DisposePendingDisposablesOnException()
	{
		Exception exception = null;
		DisposeFrame(Current.CollectionEnumerator, ref exception);
		int num = Math.Max(_count, _continuationCount);
		for (int i = 0; i < num - 1; i++)
		{
			DisposeFrame(_stack[i].CollectionEnumerator, ref exception);
		}
		if (exception != null)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
		}
		static void DisposeFrame(IEnumerator collectionEnumerator, ref Exception reference)
		{
			try
			{
				if (collectionEnumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch (Exception ex)
			{
				reference = ex;
			}
		}
	}

	public readonly async ValueTask DisposePendingDisposablesOnExceptionAsync()
	{
		Exception exception = null;
		exception = await DisposeFrame(Current.CollectionEnumerator, Current.AsyncDisposable, exception).ConfigureAwait(continueOnCapturedContext: false);
		int stackSize = Math.Max(_count, _continuationCount);
		for (int i = 0; i < stackSize - 1; i++)
		{
			exception = await DisposeFrame(_stack[i].CollectionEnumerator, _stack[i].AsyncDisposable, exception).ConfigureAwait(continueOnCapturedContext: false);
		}
		if (exception != null)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
		}
		static async ValueTask<Exception> DisposeFrame(IEnumerator collectionEnumerator, IAsyncDisposable asyncDisposable, Exception result)
		{
			try
			{
				if (collectionEnumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
				else if (asyncDisposable != null)
				{
					await asyncDisposable.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				result = ex;
			}
			return result;
		}
	}

	public unsafe string PropertyPath()
	{
		StringBuilder stringBuilder = new StringBuilder("$");
		int continuationCount = _continuationCount;
		object obj = continuationCount switch
		{
			0 => (_count - 1, true), 
			1 => (0, true), 
			_ => (continuationCount, false), 
		};
		int item = ((ValueTuple<int, bool>*)(&obj))->Item1;
		bool item2 = ((ValueTuple<int, bool>*)(&obj))->Item2;
		for (int i = 1; i <= item; i++)
		{
			AppendStackFrame(stringBuilder, ref _stack[i - _indexOffset]);
		}
		if (item2)
		{
			AppendStackFrame(stringBuilder, ref Current);
		}
		return stringBuilder.ToString();
		static void AppendPropertyName(StringBuilder sb, string propertyName)
		{
			if (propertyName != null)
			{
				if (propertyName.AsSpan().ContainsSpecialCharacters())
				{
					sb.Append("['");
					sb.Append(propertyName);
					sb.Append("']");
				}
				else
				{
					sb.Append('.');
					sb.Append(propertyName);
				}
			}
		}
		static void AppendStackFrame(StringBuilder sb, ref WriteStackFrame frame)
		{
			string propertyName = frame.JsonPropertyInfo?.MemberName ?? frame.JsonPropertyNameAsString;
			AppendPropertyName(sb, propertyName);
		}
	}
}
