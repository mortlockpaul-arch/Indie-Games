using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Threading;

namespace System.Reflection;

internal struct TypeNameResolver
{
	private Func<AssemblyName, Assembly> _assemblyResolver;

	private Func<Assembly, string, bool, Type> _typeResolver;

	private bool _throwOnError;

	private bool _ignoreCase;

	private bool _extensibleParser;

	private bool _requireAssemblyQualifiedName;

	private bool _suppressContextualReflectionContext;

	private nint _unsafeAccessorMethod;

	private Assembly _requestingAssembly;

	private Assembly _topLevelAssembly;

	private bool SupportsUnboundGenerics => _unsafeAccessorMethod != IntPtr.Zero;

	[RequiresUnreferencedCode("The type might be removed")]
	internal static Type GetType(string typeName, Assembly requestingAssembly, bool throwOnError = false, bool ignoreCase = false)
	{
		return GetType(typeName, null, null, requestingAssembly, throwOnError, ignoreCase, extensibleParser: false);
	}

	[RequiresUnreferencedCode("The type might be removed")]
	internal static Type GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, Assembly requestingAssembly, bool throwOnError = false, bool ignoreCase = false, bool extensibleParser = true)
	{
		ArgumentNullException.ThrowIfNull(typeName, "typeName");
		if (typeName.Length == 0)
		{
			if (throwOnError)
			{
				throw new TypeLoadException(SR.Arg_TypeLoadNullStr);
			}
			return null;
		}
		TypeName typeName2 = TypeNameParser.Parse(typeName.AsSpan(), throwOnError);
		if (typeName2 == null)
		{
			return null;
		}
		return new TypeNameResolver
		{
			_assemblyResolver = assemblyResolver,
			_typeResolver = typeResolver,
			_throwOnError = throwOnError,
			_ignoreCase = ignoreCase,
			_extensibleParser = extensibleParser,
			_requestingAssembly = requestingAssembly
		}.Resolve(typeName2);
	}

	[RequiresUnreferencedCode("The type might be removed")]
	internal static Type GetType(string typeName, bool throwOnError, bool ignoreCase, Assembly topLevelAssembly)
	{
		TypeName typeName2 = TypeNameParser.Parse(typeName.AsSpan(), throwOnError, new TypeNameParseOptions
		{
			IsAssemblyGetType = true
		});
		if (typeName2 == null)
		{
			return null;
		}
		if (typeName2.AssemblyName != null)
		{
			if (!throwOnError)
			{
				return null;
			}
			throw new ArgumentException(SR.Argument_AssemblyGetTypeCannotSpecifyAssembly);
		}
		return new TypeNameResolver
		{
			_throwOnError = throwOnError,
			_ignoreCase = ignoreCase,
			_topLevelAssembly = topLevelAssembly,
			_requestingAssembly = topLevelAssembly
		}.Resolve(typeName2);
	}

	internal static RuntimeType GetTypeReferencedByCustomAttribute(string typeName, RuntimeModule scope)
	{
		ArgumentException.ThrowIfNullOrEmpty(typeName, "typeName");
		RuntimeAssembly runtimeAssembly = scope.GetRuntimeAssembly();
		TypeName typeName2 = TypeName.Parse(typeName.AsSpan());
		TypeNameResolver typeNameResolver = new TypeNameResolver
		{
			_throwOnError = true,
			_suppressContextualReflectionContext = true,
			_requestingAssembly = runtimeAssembly
		};
		RuntimeType obj = (RuntimeType)typeNameResolver.Resolve(typeName2);
		RuntimeTypeHandle.RegisterCollectibleTypeDependency(obj, runtimeAssembly);
		return obj;
	}

	internal unsafe static RuntimeType GetTypeHelper(char* pTypeName, RuntimeAssembly requestingAssembly, bool throwOnError, bool requireAssemblyQualifiedName, nint unsafeAccessorMethod)
	{
		return GetTypeHelper(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pTypeName), requestingAssembly, throwOnError, requireAssemblyQualifiedName, unsafeAccessorMethod);
	}

	internal static RuntimeType GetTypeHelper(ReadOnlySpan<char> typeName, RuntimeAssembly requestingAssembly, bool throwOnError, bool requireAssemblyQualifiedName, nint unsafeAccessorMethod = 0)
	{
		if (typeName.Length == 0)
		{
			if (throwOnError)
			{
				throw new TypeLoadException(SR.Arg_TypeLoadNullStr);
			}
			return null;
		}
		TypeName typeName2 = TypeNameParser.Parse(typeName, throwOnError);
		if (typeName2 == null)
		{
			return null;
		}
		RuntimeType runtimeType = (RuntimeType)new TypeNameResolver
		{
			_requestingAssembly = requestingAssembly,
			_throwOnError = throwOnError,
			_suppressContextualReflectionContext = true,
			_requireAssemblyQualifiedName = requireAssemblyQualifiedName,
			_unsafeAccessorMethod = unsafeAccessorMethod
		}.Resolve(typeName2);
		if (runtimeType != null)
		{
			RuntimeTypeHandle.RegisterCollectibleTypeDependency(runtimeType, requestingAssembly);
		}
		return runtimeType;
	}

	private Assembly ResolveAssembly(AssemblyName assemblyName)
	{
		Assembly assembly;
		if (_assemblyResolver != null)
		{
			assembly = _assemblyResolver(assemblyName);
			if ((object)assembly == null && _throwOnError)
			{
				throw new FileNotFoundException(SR.Format(SR.FileNotFound_ResolveAssembly, assemblyName));
			}
		}
		else
		{
			assembly = RuntimeAssembly.InternalLoad(assemblyName, ref Unsafe.NullRef<StackCrawlMark>(), _suppressContextualReflectionContext ? null : AssemblyLoadContext.CurrentContextualReflectionContext, (RuntimeAssembly)_requestingAssembly, _throwOnError);
		}
		return assembly;
	}

	[LibraryImport("QCall", EntryPoint = "UnsafeAccessors_ResolveGenericParamToTypeHandle")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private static nint ResolveGenericParamToTypeHandle(nint unsafeAccessorMethod, [MarshalAs(UnmanagedType.Bool)] bool isMethodParam, uint paramIndex)
	{
		int _isMethodParam_native = (isMethodParam ? 1 : 0);
		return __PInvoke(unsafeAccessorMethod, _isMethodParam_native, paramIndex);
		[DllImport("QCall", EntryPoint = "UnsafeAccessors_ResolveGenericParamToTypeHandle", ExactSpelling = true)]
		static extern nint __PInvoke(nint __unsafeAccessorMethod_native, int __isMethodParam_native, uint __paramIndex_native);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "TypeNameResolver.GetType is marked as RequiresUnreferencedCode.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "TypeNameResolver.GetType is marked as RequiresUnreferencedCode.")]
	private Type GetType(string escapedTypeName, ReadOnlySpan<string> nestedTypeNames, TypeName parsedName)
	{
		Assembly assembly;
		if (parsedName.AssemblyName != null)
		{
			assembly = ResolveAssembly(parsedName.AssemblyName.ToAssemblyName());
			if ((object)assembly == null)
			{
				return null;
			}
		}
		else
		{
			assembly = _topLevelAssembly;
		}
		Type type;
		if (_typeResolver != null)
		{
			type = _typeResolver(assembly, escapedTypeName, _ignoreCase);
			if ((object)type == null)
			{
				if (_throwOnError)
				{
					throw new TypeLoadException(((object)assembly == null) ? SR.Format(SR.TypeLoad_ResolveType, escapedTypeName) : SR.Format(SR.TypeLoad_ResolveTypeFromAssembly, escapedTypeName, assembly.FullName), escapedTypeName);
				}
				return null;
			}
		}
		else
		{
			if ((object)assembly == null)
			{
				if (SupportsUnboundGenerics && !string.IsNullOrEmpty(escapedTypeName) && escapedTypeName[0] == '!')
				{
					if (escapedTypeName.Length == 1)
					{
						throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveType, escapedTypeName), escapedTypeName);
					}
					bool flag = escapedTypeName[1] == '!';
					if (!uint.TryParse(flag ? escapedTypeName.AsSpan(2) : escapedTypeName.AsSpan(1), NumberStyles.None, null, out var result))
					{
						throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveType, escapedTypeName), escapedTypeName);
					}
					nint num = ResolveGenericParamToTypeHandle(_unsafeAccessorMethod, flag, result);
					if (num == IntPtr.Zero)
					{
						throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveType, escapedTypeName), escapedTypeName);
					}
					return RuntimeTypeHandle.GetRuntimeTypeFromHandle(num);
				}
				if (_requireAssemblyQualifiedName)
				{
					if (_throwOnError)
					{
						throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveType, escapedTypeName), escapedTypeName);
					}
					return null;
				}
				return GetTypeFromDefaultAssemblies(TypeName.Unescape(escapedTypeName), nestedTypeNames, parsedName);
			}
			if (assembly is RuntimeAssembly runtimeAssembly)
			{
				bool flag2 = _extensibleParser && _ignoreCase;
				type = runtimeAssembly.GetTypeCore(TypeName.Unescape(escapedTypeName), flag2 ? default(ReadOnlySpan<string>) : nestedTypeNames, _throwOnError, _ignoreCase);
				if ((object)type == null)
				{
					if (_throwOnError)
					{
						throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveTypeFromAssembly, parsedName.FullName, runtimeAssembly.FullName), parsedName.FullName);
					}
					return null;
				}
				if (!flag2)
				{
					return type;
				}
			}
			else
			{
				type = assembly.GetType(escapedTypeName, _throwOnError, _ignoreCase);
			}
			if ((object)type == null)
			{
				return null;
			}
		}
		for (int i = 0; i < nestedTypeNames.Length; i++)
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			if (_ignoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			type = type.GetNestedType(nestedTypeNames[i], bindingFlags);
			if ((object)type == null)
			{
				if (_throwOnError)
				{
					throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveNestedType, nestedTypeNames[i], (i > 0) ? nestedTypeNames[i - 1] : TypeName.Unescape(escapedTypeName)), parsedName.FullName);
				}
				return null;
			}
		}
		return type;
	}

	private Type GetTypeFromDefaultAssemblies(string typeName, ReadOnlySpan<string> nestedTypeNames, TypeName parsedName)
	{
		RuntimeAssembly runtimeAssembly = (RuntimeAssembly)_requestingAssembly;
		if ((object)runtimeAssembly != null)
		{
			Type typeCore = runtimeAssembly.GetTypeCore(typeName, nestedTypeNames, throwOnFileNotFound: false, _ignoreCase);
			if ((object)typeCore != null)
			{
				return typeCore;
			}
		}
		RuntimeAssembly runtimeAssembly2 = (RuntimeAssembly)typeof(object).Assembly;
		if (runtimeAssembly != runtimeAssembly2)
		{
			Type typeCore2 = runtimeAssembly2.GetTypeCore(typeName, nestedTypeNames, throwOnFileNotFound: false, _ignoreCase);
			if ((object)typeCore2 != null)
			{
				return typeCore2;
			}
		}
		RuntimeAssembly runtimeAssembly3 = AssemblyLoadContext.OnTypeResolve(runtimeAssembly, parsedName.FullName);
		if ((object)runtimeAssembly3 != null)
		{
			Type typeCore3 = runtimeAssembly3.GetTypeCore(typeName, nestedTypeNames, throwOnFileNotFound: false, _ignoreCase);
			if ((object)typeCore3 != null)
			{
				return typeCore3;
			}
		}
		if (_throwOnError)
		{
			throw new TypeLoadException(SR.Format(SR.TypeLoad_ResolveTypeFromAssembly, parsedName.FullName, (runtimeAssembly ?? runtimeAssembly2).FullName), parsedName.FullName);
		}
		return null;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresDynamicCode", Justification = "Used to implement resolving types from strings.")]
	private Type Resolve(TypeName typeName)
	{
		if (typeName.IsSimple)
		{
			return GetSimpleType(typeName);
		}
		if (typeName.IsConstructedGenericType)
		{
			return GetGenericType(typeName);
		}
		if (typeName.IsArray || typeName.IsPointer || typeName.IsByRef)
		{
			Type type = Resolve(typeName.GetElementType());
			if ((object)type == null)
			{
				return null;
			}
			if (typeName.IsArray)
			{
				if (!typeName.IsSZArray)
				{
					return type.MakeArrayType(typeName.GetArrayRank());
				}
				return type.MakeArrayType();
			}
			if (typeName.IsByRef)
			{
				return type.MakeByRefType();
			}
			if (typeName.IsPointer)
			{
				return type.MakePointerType();
			}
		}
		return null;
	}

	private Type GetSimpleType(TypeName typeName)
	{
		if (typeName.IsNested)
		{
			TypeName typeName2 = typeName;
			int num = 0;
			do
			{
				num++;
				typeName2 = typeName2.DeclaringType;
			}
			while (typeName2.IsNested);
			string[] array = new string[num];
			typeName2 = typeName;
			while (typeName2.IsNested)
			{
				array[--num] = TypeName.Unescape(typeName2.Name);
				typeName2 = typeName2.DeclaringType;
			}
			return GetType(typeName2.FullName, array, typeName);
		}
		return GetType(typeName.FullName, default(ReadOnlySpan<string>), typeName);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2055:UnrecognizedReflectionPattern", Justification = "Used to implement resolving types from strings.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresDynamicCode", Justification = "Used to implement resolving types from strings.")]
	private Type GetGenericType(TypeName typeName)
	{
		Type type = Resolve(typeName.GetGenericTypeDefinition());
		if ((object)type == null)
		{
			return null;
		}
		ReadOnlySpan<TypeName> genericArguments = typeName.GetGenericArguments();
		Type[] array = new Type[genericArguments.Length];
		for (int i = 0; i < genericArguments.Length; i++)
		{
			Type type2 = Resolve(genericArguments[i]);
			if ((object)type2 == null)
			{
				return null;
			}
			array[i] = type2;
		}
		return type.MakeGenericType(array);
	}
}
