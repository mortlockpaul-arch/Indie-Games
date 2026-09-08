using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using Internal;

namespace System.Runtime.InteropServices;

[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
internal static class TypeMapLazyDictionary
{
	private ref struct CallbackContext
	{
		private RuntimeAssembly _currAssembly;

		private LazyExternalTypeDictionary _externalTypeMap;

		private LazyProxyTypeDictionary _proxyTypeMap;

		private ExceptionDispatchInfo _creationException;

		public RuntimeAssembly CurrentAssembly => _currAssembly;

		public LazyExternalTypeDictionary ExternalTypeMap
		{
			[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
			get
			{
				if (_externalTypeMap == null)
				{
					_externalTypeMap = new LazyExternalTypeDictionary();
				}
				return _externalTypeMap;
			}
		}

		public LazyProxyTypeDictionary ProxyTypeMap
		{
			[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
			get
			{
				if (_proxyTypeMap == null)
				{
					_proxyTypeMap = new LazyProxyTypeDictionary();
				}
				return _proxyTypeMap;
			}
		}

		public ExceptionDispatchInfo CreationException
		{
			get
			{
				return _creationException;
			}
			set
			{
				_creationException = value;
			}
		}
	}

	public struct ProcessAttributesCallbackArg
	{
		public unsafe void* Utf8String1;

		public unsafe void* Utf8String2;

		public int StringLen1;

		public int StringLen2;
	}

	public ref struct Utf16SharedBuffer
	{
		private char[] _backingArray;

		public ReadOnlySpan<char> Buffer { get; init; }

		public Utf16SharedBuffer()
		{
			_backingArray = null;
			Buffer = default(ReadOnlySpan<char>);
		}

		public Utf16SharedBuffer(char[] backingBuffer, int validLength)
		{
			_backingArray = backingBuffer;
			Buffer = new ReadOnlySpan<char>(backingBuffer, 0, validLength);
		}

		public void Dispose()
		{
			if (_backingArray != null)
			{
				ArrayPool<char>.Shared.Return(_backingArray);
			}
		}
	}

	private abstract class LazyTypeLoadDictionary<TKey> : IReadOnlyDictionary<TKey, Type>, IReadOnlyCollection<KeyValuePair<TKey, Type>>, IEnumerable<KeyValuePair<TKey, Type>>, IEnumerable
	{
		public Type this[TKey key]
		{
			get
			{
				if (!TryGetOrLoadType(key, out var type))
				{
					ThrowHelper.ThrowKeyNotFoundException(key);
				}
				return type;
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public IEnumerable<Type> Values
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected abstract bool TryGetOrLoadType(TKey key, [NotNullWhen(true)] out Type type);

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out Type value)
		{
			return TryGetOrLoadType(key, out value);
		}

		public bool ContainsKey(TKey key)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<TKey, Type>> GetEnumerator()
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}

	private struct TypeNameUtf8
	{
		public unsafe required void* Utf8TypeName { get; init; }

		public required int Utf8TypeNameLen { get; init; }
	}

	[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
	private sealed class DelayedType
	{
		private TypeNameUtf8 _typeNameUtf8;

		private RuntimeAssembly _fallbackAssembly;

		private Type _type;

		public DelayedType(TypeNameUtf8 typeNameUtf8, RuntimeAssembly fallbackAssembly)
		{
			_typeNameUtf8 = typeNameUtf8;
			_fallbackAssembly = fallbackAssembly;
			_type = null;
		}

		public unsafe Type GetOrLoadType()
		{
			if ((object)_type == null)
			{
				Utf16SharedBuffer utf16Buffer = new Utf16SharedBuffer();
				try
				{
					ConvertUtf8ToUtf16(new ReadOnlySpan<byte>(_typeNameUtf8.Utf8TypeName, _typeNameUtf8.Utf8TypeNameLen), out utf16Buffer);
					_type = TypeNameResolver.GetTypeHelper(utf16Buffer.Buffer, _fallbackAssembly, throwOnError: true, requireAssemblyQualifiedName: false);
				}
				finally
				{
					utf16Buffer.Dispose();
				}
			}
			return _type;
		}
	}

	[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
	private sealed class LazyExternalTypeDictionary : LazyTypeLoadDictionary<string>
	{
		private readonly Dictionary<string, DelayedType> _lazyData = new Dictionary<string, DelayedType>();

		protected override bool TryGetOrLoadType(string key, [NotNullWhen(true)] out Type type)
		{
			if (!_lazyData.TryGetValue(key, out var value))
			{
				type = null;
				return false;
			}
			type = value.GetOrLoadType();
			return true;
		}

		public void Add(string key, TypeNameUtf8 targetType, RuntimeAssembly fallbackAssembly)
		{
			if (_lazyData.ContainsKey(key))
			{
				ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
			}
			_lazyData.Add(key, new DelayedType(targetType, fallbackAssembly));
		}
	}

	[RequiresUnreferencedCode("Lazy TypeMap isn't supported for Trimmer scenarios")]
	private sealed class LazyProxyTypeDictionary : LazyTypeLoadDictionary<Type>
	{
		private struct SourceProxyPair
		{
			public required DelayedType Source { get; init; }

			public required DelayedType Proxy { get; init; }
		}

		private sealed class DelayedTypeCollection
		{
			public required SourceProxyPair First { get; init; }

			public List<SourceProxyPair> Others { get; private set; }

			public void Add(SourceProxyPair newEntryMaybe)
			{
				if (Others == null)
				{
					List<SourceProxyPair> list = (Others = new List<SourceProxyPair>());
				}
				Others.Add(newEntryMaybe);
			}
		}

		private readonly Dictionary<int, DelayedTypeCollection> _lazyData = new Dictionary<int, DelayedTypeCollection>();

		private static int ComputeHashCode(RuntimeType key)
		{
			return VersionResilientHashCode.TypeHashCode(key);
		}

		private static int ComputeHashCode(TypeName key)
		{
			return VersionResilientHashCode.TypeHashCode(key);
		}

		protected override bool TryGetOrLoadType(Type key, [NotNullWhen(true)] out Type type)
		{
			int key2 = ComputeHashCode((key as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "key"));
			if (_lazyData.TryGetValue(key2, out var value))
			{
				if (value.First.Source.GetOrLoadType() == key)
				{
					type = value.First.Proxy.GetOrLoadType();
					return true;
				}
				if (value.Others != null)
				{
					foreach (SourceProxyPair other in value.Others)
					{
						if (other.Source.GetOrLoadType() == key)
						{
							type = other.Proxy.GetOrLoadType();
							return true;
						}
					}
				}
			}
			type = null;
			return false;
		}

		public void Add(TypeName parsedSourceTypeName, TypeNameUtf8 sourceTypeName, TypeNameUtf8 proxyTypeName, RuntimeAssembly fallbackAssembly)
		{
			int key = ComputeHashCode(parsedSourceTypeName);
			SourceProxyPair sourceProxyPair = new SourceProxyPair
			{
				Source = new DelayedType(sourceTypeName, fallbackAssembly),
				Proxy = new DelayedType(proxyTypeName, fallbackAssembly)
			};
			if (!_lazyData.TryGetValue(key, out var value))
			{
				value = new DelayedTypeCollection
				{
					First = sourceProxyPair
				};
				_lazyData.Add(key, value);
			}
			else
			{
				value.Add(sourceProxyPair);
			}
		}
	}

	[DllImport("QCall", EntryPoint = "TypeMapLazyDictionary_ProcessAttributes", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "TypeMapLazyDictionary_ProcessAttributes")]
	private unsafe static extern void ProcessAttributes(QCallAssembly assembly, QCallTypeHandle groupType, delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL> newExternalTypeEntry, delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL> newProxyTypeEntry, CallbackContext* context);

	private static void ConvertUtf8ToUtf16(ReadOnlySpan<byte> utf8TypeName, out Utf16SharedBuffer utf16Buffer)
	{
		int minimumLength = ((utf8TypeName.Length < 1024) ? Encoding.UTF8.GetMaxCharCount(utf8TypeName.Length) : Encoding.UTF8.GetCharCount(utf8TypeName));
		char[] array = ArrayPool<char>.Shared.Rent(minimumLength);
		int chars = Encoding.UTF8.GetChars(utf8TypeName, array);
		utf16Buffer = new Utf16SharedBuffer(array, chars);
	}

	[UnmanagedCallersOnly]
	private unsafe static Interop.BOOL NewExternalTypeEntry(CallbackContext* context, ProcessAttributesCallbackArg* arg)
	{
		try
		{
			string key = new string((sbyte*)arg->Utf8String1, 0, arg->StringLen1, Encoding.UTF8);
			TypeNameUtf8 targetType = new TypeNameUtf8
			{
				Utf8TypeName = arg->Utf8String2,
				Utf8TypeNameLen = arg->StringLen2
			};
			context->ExternalTypeMap.Add(key, targetType, context->CurrentAssembly);
		}
		catch (Exception source)
		{
			context->CreationException = ExceptionDispatchInfo.Capture(source);
			return Interop.BOOL.FALSE;
		}
		return Interop.BOOL.TRUE;
	}

	[UnmanagedCallersOnly]
	private unsafe static Interop.BOOL NewProxyTypeEntry(CallbackContext* context, ProcessAttributesCallbackArg* arg)
	{
		Utf16SharedBuffer utf16Buffer = new Utf16SharedBuffer();
		try
		{
			ConvertUtf8ToUtf16(new ReadOnlySpan<byte>(arg->Utf8String1, arg->StringLen1), out utf16Buffer);
			TypeName parsedSourceTypeName = TypeNameParser.Parse(utf16Buffer.Buffer, throwOnError: true);
			TypeNameUtf8 sourceTypeName = new TypeNameUtf8
			{
				Utf8TypeName = arg->Utf8String1,
				Utf8TypeNameLen = arg->StringLen1
			};
			TypeNameUtf8 proxyTypeName = new TypeNameUtf8
			{
				Utf8TypeName = arg->Utf8String2,
				Utf8TypeNameLen = arg->StringLen2
			};
			context->ProxyTypeMap.Add(parsedSourceTypeName, sourceTypeName, proxyTypeName, context->CurrentAssembly);
		}
		catch (Exception source)
		{
			context->CreationException = ExceptionDispatchInfo.Capture(source);
			return Interop.BOOL.FALSE;
		}
		finally
		{
			utf16Buffer.Dispose();
		}
		return Interop.BOOL.TRUE;
	}

	private unsafe static CallbackContext CreateMaps(RuntimeType groupType, delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL> newExternalTypeEntry, delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL> newProxyTypeEntry)
	{
		RuntimeAssembly assembly = (RuntimeAssembly)Assembly.GetEntryAssembly();
		if ((object)assembly == null)
		{
			throw new InvalidOperationException(SR.InvalidOperation_TypeMapMissingEntryAssembly);
		}
		Unsafe.SkipInit(out CallbackContext result);
		ProcessAttributes(new QCallAssembly(ref assembly), new QCallTypeHandle(ref groupType), newExternalTypeEntry, newProxyTypeEntry, &result);
		result.CreationException?.Throw();
		return result;
	}

	public unsafe static IReadOnlyDictionary<string, Type> CreateExternalTypeMap(RuntimeType groupType)
	{
		return CreateMaps(groupType, (delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL>)(&NewExternalTypeEntry), (delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL>)null).ExternalTypeMap;
	}

	public unsafe static IReadOnlyDictionary<Type, Type> CreateProxyTypeMap(RuntimeType groupType)
	{
		return CreateMaps(groupType, (delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL>)null, (delegate* unmanaged<CallbackContext*, ProcessAttributesCallbackArg*, Interop.BOOL>)(&NewProxyTypeEntry)).ProxyTypeMap;
	}
}
