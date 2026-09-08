using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System;

internal sealed class RuntimeType : TypeInfo, ICloneable
{
	internal sealed class ActivatorCache : IGenericCacheEntry<ActivatorCache>, IGenericCacheEntry
	{
		private unsafe readonly delegate*<void*, object> _pfnAllocator;

		private unsafe readonly void* _allocatorFirstArg;

		private unsafe readonly delegate*<object, void> _pfnRefCtor;

		private unsafe readonly delegate*<ref byte, void> _pfnValueCtor;

		private readonly bool _ctorIsPublic;

		internal bool CtorIsPublic => _ctorIsPublic;

		public static ActivatorCache Create(RuntimeType type)
		{
			return new ActivatorCache(type);
		}

		public void InitializeCompositeCache(CompositeCacheEntry compositeEntry)
		{
			compositeEntry._activatorCache = this;
		}

		public static ref ActivatorCache GetStorageRef(CompositeCacheEntry compositeEntry)
		{
			return ref compositeEntry._activatorCache;
		}

		private unsafe ActivatorCache(RuntimeType rt)
		{
			rt.CreateInstanceCheckThis();
			try
			{
				RuntimeTypeHandle.GetActivationInfo(rt, out _pfnAllocator, out _allocatorFirstArg, out _pfnRefCtor, out _pfnValueCtor, out _ctorIsPublic);
			}
			catch (Exception ex)
			{
				string message = SR.Format(SR.Activator_CannotCreateInstance, rt, ex.Message);
				if (!(ex is ArgumentException))
				{
					if (!(ex is PlatformNotSupportedException))
					{
						if (!(ex is NotSupportedException))
						{
							if (!(ex is MethodAccessException))
							{
								if (!(ex is MissingMethodException))
								{
									if (ex is MemberAccessException)
									{
										throw new MemberAccessException(message);
									}
									throw;
								}
								throw new MissingMethodException(message);
							}
							throw new MethodAccessException(message);
						}
						throw new NotSupportedException(message);
					}
					throw new PlatformNotSupportedException(message);
				}
				throw new ArgumentException(message);
			}
			if (_pfnAllocator == (delegate*<void*, object>)null)
			{
				_pfnAllocator = &ReturnNull;
			}
			if (_pfnRefCtor == (delegate*<object, void>)null)
			{
				_pfnRefCtor = &RefCtorNoopStub;
			}
			if (rt.IsValueType && _pfnValueCtor == (delegate*<ref byte, void>)null)
			{
				_pfnValueCtor = &ValueRefCtorNoopStub;
			}
			static void RefCtorNoopStub(object uninitializedObject)
			{
			}
			unsafe static object ReturnNull(void* _)
			{
				return null;
			}
			static void ValueRefCtorNoopStub(ref byte uninitializedObject)
			{
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe object CreateUninitializedObject(RuntimeType rt)
		{
			object result = _pfnAllocator(_allocatorFirstArg);
			GC.KeepAlive(rt);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void CallRefConstructor(object uninitializedObject)
		{
			_pfnRefCtor(uninitializedObject);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void CallValueConstructor(ref byte uninitializedObject)
		{
			_pfnValueCtor(ref uninitializedObject);
		}
	}

	internal enum MemberListType
	{
		All,
		CaseSensitive,
		CaseInsensitive,
		HandleToInfo
	}

	internal struct ListBuilder<T>(int capacity) where T : class
	{
		private T[] _items = null;

		private T _item = null;

		private int _count = 0;

		private int _capacity = capacity;

		public T this[int index]
		{
			get
			{
				if (_items == null)
				{
					return _item;
				}
				return _items[index];
			}
		}

		public int Count => _count;

		public T[] ToArray()
		{
			if (_count == 0)
			{
				return Array.Empty<T>();
			}
			if (_count == 1)
			{
				return new T[1] { _item };
			}
			Array.Resize(ref _items, _count);
			_capacity = _count;
			return _items;
		}

		public void CopyTo(object[] array, int index)
		{
			if (_count != 0)
			{
				if (_count == 1)
				{
					array[index] = _item;
				}
				else
				{
					Array.Copy(_items, 0, array, index, _count);
				}
			}
		}

		public void Add(T item)
		{
			if (_count == 0)
			{
				_item = item;
			}
			else
			{
				if (_count == 1)
				{
					if (_capacity < 2)
					{
						_capacity = 4;
					}
					_items = new T[_capacity];
					_items[0] = _item;
				}
				else if (_capacity == _count)
				{
					int num = 2 * _capacity;
					Array.Resize(ref _items, num);
					_capacity = num;
				}
				_items[_count] = item;
			}
			_count++;
		}
	}

	internal sealed class RuntimeTypeCache
	{
		internal enum CacheType
		{
			Method,
			Constructor,
			Field,
			Property,
			Event,
			Interface,
			NestedType
		}

		private unsafe readonly struct Filter(byte* pUtf8Name, int cUtf8Name, MemberListType listType)
		{
			private unsafe readonly MdUtf8String m_name = new MdUtf8String(pUtf8Name, cUtf8Name);

			private readonly MemberListType m_listType = listType;

			public bool Match(MdUtf8String name)
			{
				bool result = true;
				if (m_listType == MemberListType.CaseSensitive)
				{
					result = m_name.Equals(name);
				}
				else if (m_listType == MemberListType.CaseInsensitive)
				{
					result = m_name.EqualsCaseInsensitive(name);
				}
				return result;
			}

			public bool RequiresStringComparison()
			{
				if (m_listType != MemberListType.CaseSensitive)
				{
					return m_listType == MemberListType.CaseInsensitive;
				}
				return true;
			}

			public bool CaseSensitive()
			{
				return m_listType == MemberListType.CaseSensitive;
			}
		}

		private sealed class MemberInfoCache<T> where T : MemberInfo
		{
			private CerHashtable<string, T[]> m_csMemberInfos;

			private CerHashtable<string, T[]> m_cisMemberInfos;

			private T[] m_allMembers;

			private bool m_cacheComplete;

			private readonly RuntimeTypeCache m_runtimeTypeCache;

			internal RuntimeType ReflectedType => m_runtimeTypeCache.GetRuntimeType();

			internal MemberInfoCache(RuntimeTypeCache runtimeTypeCache)
			{
				m_runtimeTypeCache = runtimeTypeCache;
			}

			internal MethodBase AddMethod(RuntimeType declaringType, RuntimeMethodHandleInternal method, CacheType cacheType)
			{
				T[] allMembers = m_allMembers;
				if (allMembers != null)
				{
					switch (cacheType)
					{
					case CacheType.Method:
					{
						T[] array = allMembers;
						foreach (T val2 in array)
						{
							if ((object)val2 == null)
							{
								break;
							}
							if (val2 is RuntimeMethodInfo { MethodHandle: var methodHandle2 } runtimeMethodInfo && methodHandle2.Value == method.Value)
							{
								return runtimeMethodInfo;
							}
						}
						break;
					}
					case CacheType.Constructor:
					{
						T[] array = allMembers;
						foreach (T val in array)
						{
							if ((object)val == null)
							{
								break;
							}
							if (val is RuntimeConstructorInfo { MethodHandle: var methodHandle } runtimeConstructorInfo && methodHandle.Value == method.Value)
							{
								return runtimeConstructorInfo;
							}
						}
						break;
					}
					}
				}
				T[] list = null;
				MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(method);
				bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
				bool isStatic = (attributes & MethodAttributes.Static) != 0;
				bool isInherited = declaringType != ReflectedType;
				BindingFlags bindingFlags = FilterPreCalculate(isPublic, isInherited, isStatic);
				switch (cacheType)
				{
				case CacheType.Method:
					list = (T[])(object)new RuntimeMethodInfo[1]
					{
						new RuntimeMethodInfo(method, declaringType, m_runtimeTypeCache, attributes, bindingFlags, null)
					};
					break;
				case CacheType.Constructor:
					list = (T[])(object)new RuntimeConstructorInfo[1]
					{
						new RuntimeConstructorInfo(method, declaringType, m_runtimeTypeCache, attributes, bindingFlags)
					};
					break;
				}
				Insert(ref list, null, MemberListType.HandleToInfo);
				return (MethodBase)(object)list[0];
			}

			internal FieldInfo AddField(RuntimeFieldHandleInternal field)
			{
				T[] allMembers = m_allMembers;
				if (allMembers != null)
				{
					T[] array = allMembers;
					foreach (T val in array)
					{
						if ((object)val == null)
						{
							break;
						}
						if (val is RtFieldInfo rtFieldInfo && rtFieldInfo.GetFieldDesc() == field.Value)
						{
							return rtFieldInfo;
						}
					}
				}
				FieldAttributes attributes = RuntimeFieldHandle.GetAttributes(field);
				bool isPublic = (attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
				bool isStatic = (attributes & FieldAttributes.Static) != 0;
				RuntimeType approxDeclaringType = RuntimeFieldHandle.GetApproxDeclaringType(field);
				bool isInherited = (RuntimeFieldHandle.AcquiresContextFromThis(field) ? (!RuntimeTypeHandle.CompareCanonicalHandles(approxDeclaringType, ReflectedType)) : (approxDeclaringType != ReflectedType));
				BindingFlags bindingFlags = FilterPreCalculate(isPublic, isInherited, isStatic);
				T[] list = (T[])(object)new RuntimeFieldInfo[1]
				{
					new RtFieldInfo(field, ReflectedType, m_runtimeTypeCache, bindingFlags)
				};
				Insert(ref list, null, MemberListType.HandleToInfo);
				return (FieldInfo)(object)list[0];
			}

			private T[] Populate(string name, MemberListType listType, CacheType cacheType)
			{
				T[] list;
				if (string.IsNullOrEmpty(name) || (cacheType == CacheType.Constructor && name[0] != '.' && name[0] != '*'))
				{
					list = GetListByName(string.Empty, Span<byte>.Empty, listType, cacheType);
				}
				else
				{
					int byteCount = Encoding.UTF8.GetByteCount(name);
					Span<byte> span = (((uint)byteCount <= 1024u) ? stackalloc byte[byteCount] : ((Span<byte>)new byte[byteCount]));
					Span<byte> utf8Name = span;
					list = GetListByName(name, utf8Name, listType, cacheType);
				}
				Insert(ref list, name, listType);
				return list;
			}

			private unsafe T[] GetListByName(string name, Span<byte> utf8Name, MemberListType listType, CacheType cacheType)
			{
				if (name.Length != 0)
				{
					Encoding.UTF8.GetBytes(name.AsSpan(), utf8Name);
				}
				fixed (byte* pUtf8Name = utf8Name)
				{
					Filter filter = new Filter(pUtf8Name, utf8Name.Length, listType);
					object obj = null;
					switch (cacheType)
					{
					case CacheType.Method:
						obj = PopulateMethods(filter);
						break;
					case CacheType.Field:
						obj = PopulateFields(filter);
						break;
					case CacheType.Constructor:
						obj = PopulateConstructors(filter);
						break;
					case CacheType.Property:
						obj = PopulateProperties(filter);
						break;
					case CacheType.Event:
						obj = PopulateEvents(filter);
						break;
					case CacheType.NestedType:
						obj = PopulateNestedClasses(filter);
						break;
					case CacheType.Interface:
						obj = PopulateInterfaces(filter);
						break;
					}
					return (T[])obj;
				}
			}

			internal void Insert(ref T[] list, string name, MemberListType listType)
			{
				bool lockTaken = false;
				try
				{
					Monitor.Enter(this, ref lockTaken);
					switch (listType)
					{
					case MemberListType.CaseSensitive:
					{
						T[] array = m_csMemberInfos[name];
						if (array == null)
						{
							MergeWithGlobalList(list);
							m_csMemberInfos[name] = list;
						}
						else
						{
							list = array;
						}
						break;
					}
					case MemberListType.CaseInsensitive:
					{
						T[] array2 = m_cisMemberInfos[name];
						if (array2 == null)
						{
							MergeWithGlobalList(list);
							m_cisMemberInfos[name] = list;
						}
						else
						{
							list = array2;
						}
						break;
					}
					case MemberListType.All:
						if (!m_cacheComplete)
						{
							MergeWithGlobalListInOrder(list);
							int num = m_allMembers.Length;
							while (num > 0 && !(m_allMembers[num - 1] != null))
							{
								num--;
							}
							Array.Resize(ref m_allMembers, num);
							Volatile.Write(ref m_cacheComplete, value: true);
						}
						list = m_allMembers;
						break;
					default:
						MergeWithGlobalList(list);
						break;
					}
				}
				finally
				{
					if (lockTaken)
					{
						Monitor.Exit(this);
					}
				}
			}

			private void MergeWithGlobalListInOrder(T[] list)
			{
				T[] allMembers = m_allMembers;
				if (allMembers == null)
				{
					m_allMembers = list;
					return;
				}
				T[] array = allMembers;
				foreach (T val in array)
				{
					if (val == null)
					{
						break;
					}
					for (int j = 0; j < list.Length; j++)
					{
						if (list[j].CacheEquals(val))
						{
							list[j] = val;
							break;
						}
					}
				}
				m_allMembers = list;
			}

			private void MergeWithGlobalList(T[] list)
			{
				T[] array = m_allMembers;
				if (array == null)
				{
					m_allMembers = list;
					return;
				}
				int num = array.Length;
				int num2 = 0;
				for (int i = 0; i < list.Length; i++)
				{
					T val = list[i];
					bool flag = false;
					int j;
					for (j = 0; j < num; j++)
					{
						T val2 = array[j];
						if (val2 == null)
						{
							break;
						}
						if (val.CacheEquals(val2))
						{
							list[i] = val2;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						if (num2 == 0)
						{
							num2 = j;
						}
						if (num2 >= array.Length)
						{
							int newSize = ((!m_cacheComplete) ? Math.Max(Math.Max(4, 2 * array.Length), list.Length) : (array.Length + 1));
							T[] array2 = array;
							Array.Resize(ref array2, newSize);
							array = array2;
						}
						Volatile.Write(ref array[num2], val);
						num2++;
					}
				}
				m_allMembers = array;
			}

			private RuntimeMethodInfo[] PopulateMethods(Filter filter)
			{
				ListBuilder<RuntimeMethodInfo> listBuilder = default(ListBuilder<RuntimeMethodInfo>);
				RuntimeType runtimeType = ReflectedType;
				if (runtimeType.IsActualInterface)
				{
					foreach (RuntimeMethodHandleInternal introducedMethod in RuntimeTypeHandle.GetIntroducedMethods(runtimeType))
					{
						if (!filter.RequiresStringComparison() || filter.Match(RuntimeMethodHandle.GetUtf8Name(introducedMethod)))
						{
							MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(introducedMethod);
							if ((attributes & MethodAttributes.RTSpecialName) == 0 && (!MetadataUpdater.IsSupported || !RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers || !RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(runtimeType.GetRuntimeModule(), RuntimeMethodHandle.GetMethodDef(introducedMethod))))
							{
								bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
								bool isStatic = (attributes & MethodAttributes.Static) != 0;
								BindingFlags bindingFlags = FilterPreCalculate(isPublic, isInherited: false, isStatic);
								RuntimeMethodInfo item = new RuntimeMethodInfo(RuntimeMethodHandle.GetStubIfNeeded(introducedMethod, runtimeType, null), runtimeType, m_runtimeTypeCache, attributes, bindingFlags, null);
								listBuilder.Add(item);
							}
						}
					}
				}
				else
				{
					while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
					{
						runtimeType = runtimeType.GetBaseType();
					}
					int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(runtimeType);
					Span<bool> span = (((uint)numVirtuals <= 512u) ? stackalloc bool[numVirtuals] : ((Span<bool>)new bool[numVirtuals]));
					Span<bool> span2 = span;
					span2.Clear();
					bool isActualValueType = runtimeType.IsActualValueType;
					do
					{
						int numVirtuals2 = RuntimeTypeHandle.GetNumVirtuals(runtimeType);
						foreach (RuntimeMethodHandleInternal introducedMethod2 in RuntimeTypeHandle.GetIntroducedMethods(runtimeType))
						{
							if (filter.RequiresStringComparison() && !filter.Match(RuntimeMethodHandle.GetUtf8Name(introducedMethod2)))
							{
								continue;
							}
							MethodAttributes attributes2 = RuntimeMethodHandle.GetAttributes(introducedMethod2);
							MethodAttributes methodAttributes = attributes2 & MethodAttributes.MemberAccessMask;
							if ((attributes2 & MethodAttributes.RTSpecialName) != MethodAttributes.PrivateScope)
							{
								continue;
							}
							bool flag = false;
							int num = 0;
							if ((attributes2 & MethodAttributes.Virtual) != MethodAttributes.PrivateScope)
							{
								num = RuntimeMethodHandle.GetSlot(introducedMethod2);
								flag = num < numVirtuals2;
							}
							bool flag2 = runtimeType != ReflectedType;
							bool flag3 = methodAttributes == MethodAttributes.Private;
							if (((flag2 & flag3) && !flag) || (MetadataUpdater.IsSupported && RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers && RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(runtimeType.GetRuntimeModule(), RuntimeMethodHandle.GetMethodDef(introducedMethod2))))
							{
								continue;
							}
							if (flag)
							{
								if (span2[num])
								{
									continue;
								}
								span2[num] = true;
							}
							else if (isActualValueType && (attributes2 & (MethodAttributes.Virtual | MethodAttributes.Abstract)) != MethodAttributes.PrivateScope)
							{
								continue;
							}
							bool isPublic2 = methodAttributes == MethodAttributes.Public;
							bool isStatic2 = (attributes2 & MethodAttributes.Static) != 0;
							BindingFlags bindingFlags2 = FilterPreCalculate(isPublic2, flag2, isStatic2);
							RuntimeMethodInfo item2 = new RuntimeMethodInfo(RuntimeMethodHandle.GetStubIfNeeded(introducedMethod2, runtimeType, null), runtimeType, m_runtimeTypeCache, attributes2, bindingFlags2, null);
							listBuilder.Add(item2);
						}
						runtimeType = runtimeType.GetParentType();
					}
					while (runtimeType != null);
				}
				return listBuilder.ToArray();
			}

			private RuntimeConstructorInfo[] PopulateConstructors(Filter filter)
			{
				if (ReflectedType.IsGenericParameter)
				{
					return Array.Empty<RuntimeConstructorInfo>();
				}
				ListBuilder<RuntimeConstructorInfo> listBuilder = default(ListBuilder<RuntimeConstructorInfo>);
				RuntimeType reflectedType = ReflectedType;
				foreach (RuntimeMethodHandleInternal introducedMethod in RuntimeTypeHandle.GetIntroducedMethods(reflectedType))
				{
					if (!filter.RequiresStringComparison() || filter.Match(RuntimeMethodHandle.GetUtf8Name(introducedMethod)))
					{
						MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(introducedMethod);
						if ((attributes & MethodAttributes.RTSpecialName) != MethodAttributes.PrivateScope && (!MetadataUpdater.IsSupported || !RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers || !RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(reflectedType.GetRuntimeModule(), RuntimeMethodHandle.GetMethodDef(introducedMethod))))
						{
							bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
							bool isStatic = (attributes & MethodAttributes.Static) != 0;
							BindingFlags bindingFlags = FilterPreCalculate(isPublic, isInherited: false, isStatic);
							RuntimeConstructorInfo item = new RuntimeConstructorInfo(RuntimeMethodHandle.GetStubIfNeeded(introducedMethod, reflectedType, null), ReflectedType, m_runtimeTypeCache, attributes, bindingFlags);
							listBuilder.Add(item);
						}
					}
				}
				return listBuilder.ToArray();
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075:UnrecognizedReflectionPattern", Justification = "Calls to GetInterfaces technically require all interfaces on ReflectedTypeBut this is not a public API to enumerate reflection items, all the public APIs which do thatshould be annotated accordingly.")]
			private RuntimeFieldInfo[] PopulateFields(Filter filter)
			{
				ListBuilder<RuntimeFieldInfo> list = default(ListBuilder<RuntimeFieldInfo>);
				RuntimeType runtimeType = ReflectedType;
				while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
				{
					runtimeType = runtimeType.GetBaseType();
				}
				RuntimeType runtimeType2 = runtimeType;
				while (runtimeType2 != null)
				{
					PopulateRtFields(filter, runtimeType2, ref list);
					PopulateLiteralFields(filter, runtimeType2, ref list);
					runtimeType2 = runtimeType2.GetParentType();
				}
				Type[] array = ((!ReflectedType.IsGenericParameter) ? RuntimeTypeHandle.GetInterfaces(ReflectedType) : ReflectedType.BaseType.GetInterfaces());
				Type[] array2 = array;
				foreach (Type type in array2)
				{
					PopulateLiteralFields(filter, (RuntimeType)type, ref list);
					PopulateRtFields(filter, (RuntimeType)type, ref list);
				}
				return list.ToArray();
			}

			private unsafe void PopulateRtFields(Filter filter, RuntimeType declaringType, ref ListBuilder<RuntimeFieldInfo> list)
			{
				Span<nint> buffer = new Span<nint>(stackalloc byte[(int)checked((nuint)64u * (nuint)8u)], 64);
				int count;
				while (!RuntimeTypeHandle.GetFields(declaringType, buffer, out count))
				{
					buffer = new nint[count];
				}
				if (count > 0)
				{
					PopulateRtFields(filter, buffer.Slice(0, count), declaringType, ref list);
				}
			}

			private void PopulateRtFields(Filter filter, ReadOnlySpan<nint> fieldHandles, RuntimeType declaringType, ref ListBuilder<RuntimeFieldInfo> list)
			{
				bool flag = declaringType.IsGenericType && !RuntimeTypeHandle.ContainsGenericVariables(declaringType);
				bool flag2 = declaringType != ReflectedType;
				ReadOnlySpan<nint> readOnlySpan = fieldHandles;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					nint num = readOnlySpan[i];
					RuntimeFieldHandleInternal runtimeFieldHandleInternal = new RuntimeFieldHandleInternal(num);
					if (filter.RequiresStringComparison() && !filter.Match(RuntimeFieldHandle.GetUtf8Name(runtimeFieldHandleInternal)))
					{
						continue;
					}
					FieldAttributes attributes = RuntimeFieldHandle.GetAttributes(runtimeFieldHandleInternal);
					FieldAttributes fieldAttributes = attributes & FieldAttributes.FieldAccessMask;
					if ((!flag2 || fieldAttributes != FieldAttributes.Private) && (!MetadataUpdater.IsSupported || !RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers || !RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(declaringType.GetRuntimeModule(), RuntimeFieldHandle.GetToken(num))))
					{
						bool isPublic = fieldAttributes == FieldAttributes.Public;
						bool flag3 = (attributes & FieldAttributes.Static) != 0;
						BindingFlags bindingFlags = FilterPreCalculate(isPublic, flag2, flag3);
						if (flag & flag3)
						{
							runtimeFieldHandleInternal = RuntimeFieldHandle.GetStaticFieldForGenericType(runtimeFieldHandleInternal, declaringType);
						}
						RtFieldInfo item = new RtFieldInfo(runtimeFieldHandleInternal, declaringType, m_runtimeTypeCache, bindingFlags);
						list.Add(item);
					}
				}
			}

			private void PopulateLiteralFields(Filter filter, RuntimeType declaringType, ref ListBuilder<RuntimeFieldInfo> list)
			{
				int token = RuntimeTypeHandle.GetToken(declaringType);
				if (System.Reflection.MetadataToken.IsNullToken(token))
				{
					return;
				}
				RuntimeModule runtimeModule = declaringType.GetRuntimeModule();
				MetadataImport metadataImport = runtimeModule.MetadataImport;
				metadataImport.EnumFields(token, out var result);
				for (int i = 0; i < result.Length; i++)
				{
					int num = result[i];
					metadataImport.GetFieldDefProps(num, out var fieldAttributes);
					FieldAttributes fieldAttributes2 = fieldAttributes & FieldAttributes.FieldAccessMask;
					if ((fieldAttributes & FieldAttributes.Literal) == 0)
					{
						continue;
					}
					bool flag = declaringType != ReflectedType;
					if (flag && fieldAttributes2 == FieldAttributes.Private)
					{
						continue;
					}
					if (filter.RequiresStringComparison())
					{
						MdUtf8String name = metadataImport.GetName(num);
						if (!filter.Match(name))
						{
							continue;
						}
					}
					if (!MetadataUpdater.IsSupported || !RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers || !RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(runtimeModule, num))
					{
						bool isPublic = fieldAttributes2 == FieldAttributes.Public;
						bool isStatic = (fieldAttributes & FieldAttributes.Static) != 0;
						BindingFlags bindingFlags = FilterPreCalculate(isPublic, flag, isStatic);
						RuntimeFieldInfo item = new MdFieldInfo(num, fieldAttributes, declaringType.TypeHandle, m_runtimeTypeCache, bindingFlags);
						list.Add(item);
					}
				}
				GC.KeepAlive(runtimeModule);
			}

			private void AddSpecialInterface(ref ListBuilder<RuntimeType> list, Filter filter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] RuntimeType iList, bool addSubInterface)
			{
				if (!iList.IsAssignableFrom(ReflectedType))
				{
					return;
				}
				if (filter.Match(RuntimeTypeHandle.GetUtf8Name(iList)))
				{
					list.Add(iList);
				}
				if (!addSubInterface)
				{
					return;
				}
				Type[] interfaces = iList.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					RuntimeType runtimeType = (RuntimeType)interfaces[i];
					if (runtimeType.IsGenericType && filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType)))
					{
						list.Add(runtimeType);
					}
				}
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2065:UnrecognizedReflectionPattern", Justification = "Calls to GetInterfaces technically require all interfaces on ReflectedTypeBut this is not a public API to enumerate reflection items, all the public APIs which do thatshould be annotated accordingly.")]
			private RuntimeType[] PopulateInterfaces(Filter filter)
			{
				ListBuilder<RuntimeType> list = default(ListBuilder<RuntimeType>);
				RuntimeType reflectedType = ReflectedType;
				if (!RuntimeTypeHandle.IsGenericVariable(reflectedType))
				{
					Type[] interfaces = RuntimeTypeHandle.GetInterfaces(reflectedType);
					for (int i = 0; i < interfaces.Length; i++)
					{
						RuntimeType runtimeType = (RuntimeType)interfaces[i];
						if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType)))
						{
							list.Add(runtimeType);
						}
					}
					if (ReflectedType.IsSZArray)
					{
						RuntimeType runtimeType2 = (RuntimeType)ReflectedType.GetElementType();
						if (!runtimeType2.IsPointer)
						{
							AddSpecialInterface(ref list, filter, (RuntimeType)typeof(IList<>).MakeGenericType(runtimeType2), addSubInterface: true);
							AddSpecialInterface(ref list, filter, (RuntimeType)typeof(IReadOnlyList<>).MakeGenericType(runtimeType2), addSubInterface: false);
							AddSpecialInterface(ref list, filter, (RuntimeType)typeof(IReadOnlyCollection<>).MakeGenericType(runtimeType2), addSubInterface: false);
						}
					}
				}
				else
				{
					HashSet<RuntimeType> hashSet = new HashSet<RuntimeType>();
					Type[] genericParameterConstraints = reflectedType.GetGenericParameterConstraints();
					for (int j = 0; j < genericParameterConstraints.Length; j++)
					{
						RuntimeType runtimeType3 = (RuntimeType)genericParameterConstraints[j];
						if (runtimeType3.IsActualInterface)
						{
							hashSet.Add(runtimeType3);
						}
						Type[] interfaces2 = runtimeType3.GetInterfaces();
						for (int k = 0; k < interfaces2.Length; k++)
						{
							hashSet.Add((RuntimeType)interfaces2[k]);
						}
					}
					foreach (RuntimeType item in hashSet)
					{
						if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(item)))
						{
							list.Add(item);
						}
					}
				}
				return list.ToArray();
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "Calls to ResolveTypeHandle technically require all types to be kept But this is not a public API to enumerate reflection items, all the public APIs which do that should be annotated accordingly.")]
			private RuntimeType[] PopulateNestedClasses(Filter filter)
			{
				RuntimeType runtimeType = ReflectedType;
				while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
				{
					runtimeType = runtimeType.GetBaseType();
				}
				int token = RuntimeTypeHandle.GetToken(runtimeType);
				if (System.Reflection.MetadataToken.IsNullToken(token))
				{
					return Array.Empty<RuntimeType>();
				}
				ListBuilder<RuntimeType> listBuilder = default(ListBuilder<RuntimeType>);
				ModuleHandle moduleHandle = new ModuleHandle(RuntimeTypeHandle.GetModule(runtimeType));
				moduleHandle.GetRuntimeModule().MetadataImport.EnumNestedTypes(token, out var result);
				for (int i = 0; i < result.Length; i++)
				{
					RuntimeType runtimeType2;
					try
					{
						runtimeType2 = moduleHandle.ResolveTypeHandle(result[i]).GetRuntimeType();
					}
					catch (TypeLoadException)
					{
						continue;
					}
					if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType2)))
					{
						listBuilder.Add(runtimeType2);
					}
				}
				return listBuilder.ToArray();
			}

			private RuntimeEventInfo[] PopulateEvents(Filter filter)
			{
				Dictionary<string, RuntimeEventInfo> csEventInfos = (filter.CaseSensitive() ? null : new Dictionary<string, RuntimeEventInfo>());
				RuntimeType runtimeType = ReflectedType;
				ListBuilder<RuntimeEventInfo> list = default(ListBuilder<RuntimeEventInfo>);
				if (!runtimeType.IsActualInterface)
				{
					while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
					{
						runtimeType = runtimeType.GetBaseType();
					}
					RuntimeType runtimeType2 = runtimeType;
					while (runtimeType2 != null)
					{
						PopulateEvents(filter, runtimeType2, csEventInfos, ref list);
						runtimeType2 = runtimeType2.GetParentType();
					}
				}
				else
				{
					PopulateEvents(filter, runtimeType, csEventInfos, ref list);
				}
				return list.ToArray();
			}

			private void PopulateEvents(Filter filter, RuntimeType declaringType, Dictionary<string, RuntimeEventInfo> csEventInfos, ref ListBuilder<RuntimeEventInfo> list)
			{
				int token = RuntimeTypeHandle.GetToken(declaringType);
				if (System.Reflection.MetadataToken.IsNullToken(token))
				{
					return;
				}
				RuntimeModule runtimeModule = declaringType.GetRuntimeModule();
				MetadataImport metadataImport = runtimeModule.MetadataImport;
				metadataImport.EnumEvents(token, out var result);
				for (int i = 0; i < result.Length; i++)
				{
					int num = result[i];
					if (filter.RequiresStringComparison())
					{
						MdUtf8String name = metadataImport.GetName(num);
						if (!filter.Match(name))
						{
							continue;
						}
					}
					if (MetadataUpdater.IsSupported && RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers && RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(runtimeModule, num))
					{
						continue;
					}
					RuntimeEventInfo runtimeEventInfo = new RuntimeEventInfo(num, declaringType, m_runtimeTypeCache, out var isPrivate);
					if ((declaringType != m_runtimeTypeCache.GetRuntimeType()) & isPrivate)
					{
						continue;
					}
					if (csEventInfos != null)
					{
						string name2 = runtimeEventInfo.Name;
						if (csEventInfos.ContainsKey(name2))
						{
							continue;
						}
						csEventInfos[name2] = runtimeEventInfo;
					}
					else if (list.Count > 0)
					{
						break;
					}
					list.Add(runtimeEventInfo);
				}
				GC.KeepAlive(runtimeModule);
			}

			private RuntimePropertyInfo[] PopulateProperties(Filter filter)
			{
				RuntimeType runtimeType = ReflectedType;
				ListBuilder<RuntimePropertyInfo> list = default(ListBuilder<RuntimePropertyInfo>);
				if (!runtimeType.IsActualInterface)
				{
					while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
					{
						runtimeType = runtimeType.GetBaseType();
					}
					Dictionary<string, List<RuntimePropertyInfo>> csPropertyInfos = (filter.CaseSensitive() ? null : new Dictionary<string, List<RuntimePropertyInfo>>());
					int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(runtimeType);
					Span<bool> span = (((uint)numVirtuals <= 128u) ? stackalloc bool[numVirtuals] : ((Span<bool>)new bool[numVirtuals]));
					Span<bool> usedSlots = span;
					usedSlots.Clear();
					RuntimeType runtimeType2 = runtimeType;
					while (runtimeType2 != null)
					{
						PopulateProperties(filter, runtimeType2, csPropertyInfos, usedSlots, isInterface: false, ref list);
						runtimeType2 = runtimeType2.GetParentType();
					}
				}
				else
				{
					PopulateProperties(filter, runtimeType, null, default(Span<bool>), isInterface: true, ref list);
				}
				return list.ToArray();
			}

			private void PopulateProperties(Filter filter, RuntimeType declaringType, Dictionary<string, List<RuntimePropertyInfo>> csPropertyInfos, Span<bool> usedSlots, bool isInterface, ref ListBuilder<RuntimePropertyInfo> list)
			{
				int token = RuntimeTypeHandle.GetToken(declaringType);
				if (System.Reflection.MetadataToken.IsNullToken(token))
				{
					return;
				}
				RuntimeModule runtimeModule = declaringType.GetRuntimeModule();
				MetadataImport metadataImport = runtimeModule.MetadataImport;
				metadataImport.EnumProperties(token, out var result);
				int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(declaringType);
				for (int i = 0; i < result.Length; i++)
				{
					int num = result[i];
					if (filter.RequiresStringComparison())
					{
						MdUtf8String name = metadataImport.GetName(num);
						if (!filter.Match(name))
						{
							continue;
						}
					}
					if (MetadataUpdater.IsSupported && RuntimeTypeMetadataUpdateHandler.FilterDeletedMembers && RuntimeTypeMetadataUpdateHandler.IsMetadataUpdateDeleted(runtimeModule, num))
					{
						continue;
					}
					RuntimePropertyInfo runtimePropertyInfo = new RuntimePropertyInfo(num, declaringType, m_runtimeTypeCache, out var isPrivate);
					if (!isInterface)
					{
						if ((declaringType != ReflectedType) & isPrivate)
						{
							continue;
						}
						MethodInfo methodInfo = runtimePropertyInfo.GetGetMethod() ?? runtimePropertyInfo.GetSetMethod();
						if (methodInfo != null)
						{
							int slot = RuntimeMethodHandle.GetSlot((RuntimeMethodInfo)methodInfo);
							if (slot < numVirtuals)
							{
								if (usedSlots[slot])
								{
									continue;
								}
								usedSlots[slot] = true;
							}
						}
						if (csPropertyInfos != null)
						{
							string name2 = runtimePropertyInfo.Name;
							if (!csPropertyInfos.TryGetValue(name2, out var value))
							{
								value = (csPropertyInfos[name2] = new List<RuntimePropertyInfo>(1));
							}
							for (int j = 0; j < value.Count; j++)
							{
								if (runtimePropertyInfo.EqualsSig(value[j]))
								{
									value = null;
									break;
								}
							}
							if (value == null)
							{
								continue;
							}
							value.Add(runtimePropertyInfo);
						}
						else
						{
							bool flag = false;
							for (int k = 0; k < list.Count; k++)
							{
								if (runtimePropertyInfo.EqualsSig(list[k]))
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								continue;
							}
						}
					}
					list.Add(runtimePropertyInfo);
				}
				GC.KeepAlive(runtimeModule);
			}

			internal T[] GetMemberList(MemberListType listType, string name, CacheType cacheType)
			{
				switch (listType)
				{
				case MemberListType.CaseSensitive:
					return m_csMemberInfos[name] ?? Populate(name, listType, cacheType);
				case MemberListType.CaseInsensitive:
					return m_cisMemberInfos[name] ?? Populate(name, listType, cacheType);
				default:
					if (Volatile.Read(in m_cacheComplete))
					{
						return m_allMembers;
					}
					return Populate(null, listType, cacheType);
				}
			}
		}

		internal sealed class FunctionPointerCache : IGenericCacheEntry<FunctionPointerCache>, IGenericCacheEntry
		{
			public Type[] FunctionPointerReturnAndParameterTypes { get; }

			private FunctionPointerCache(Type[] functionPointerReturnAndParameterTypes)
			{
				FunctionPointerReturnAndParameterTypes = functionPointerReturnAndParameterTypes;
			}

			public static FunctionPointerCache Create(RuntimeType type)
			{
				return new FunctionPointerCache(RuntimeTypeHandle.GetArgumentTypesFromFunctionPointer(type));
			}

			public void InitializeCompositeCache(CompositeCacheEntry compositeEntry)
			{
				compositeEntry._functionPointerCache = this;
			}

			public static ref FunctionPointerCache GetStorageRef(CompositeCacheEntry compositeEntry)
			{
				return ref compositeEntry._functionPointerCache;
			}
		}

		private readonly RuntimeType m_runtimeType;

		private RuntimeType m_enclosingType;

		private TypeCode m_typeCode;

		private string m_name;

		private string m_fullName;

		private string m_assemblyQualifiedName;

		private string m_toString;

		private string m_namespace;

		private readonly bool m_isGlobal;

		private MemberInfoCache<RuntimeMethodInfo> m_methodInfoCache;

		private MemberInfoCache<RuntimeConstructorInfo> m_constructorInfoCache;

		private MemberInfoCache<RuntimeFieldInfo> m_fieldInfoCache;

		private MemberInfoCache<RuntimeType> m_interfaceCache;

		private MemberInfoCache<RuntimeType> m_nestedClassesCache;

		private MemberInfoCache<RuntimePropertyInfo> m_propertyInfoCache;

		private MemberInfoCache<RuntimeEventInfo> m_eventInfoCache;

		private static CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo> s_methodInstantiations;

		private static object s_methodInstantiationsLock;

		private string m_defaultMemberName;

		private IGenericCacheEntry m_genericCache;

		private object[] _emptyArray;

		private RuntimeType _genericTypeDefinition;

		internal ref IGenericCacheEntry GenericCache => ref m_genericCache;

		internal Type[] FunctionPointerReturnAndParameterTypes => m_runtimeType.GetOrCreateCacheEntry<FunctionPointerCache>().FunctionPointerReturnAndParameterTypes;

		internal TypeCode TypeCode
		{
			get
			{
				return m_typeCode;
			}
			set
			{
				m_typeCode = value;
			}
		}

		internal bool IsGlobal => m_isGlobal;

		internal RuntimeTypeCache(RuntimeType runtimeType)
		{
			m_typeCode = TypeCode.Empty;
			m_runtimeType = runtimeType;
			m_isGlobal = RuntimeTypeHandle.GetModule(runtimeType).RuntimeType == runtimeType;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private string ConstructName(ref string name, TypeNameFormatFlags formatFlags)
		{
			return name = new RuntimeTypeHandle(m_runtimeType).ConstructName(formatFlags);
		}

		private T[] GetMemberList<T>(ref MemberInfoCache<T> m_cache, MemberListType listType, string name, CacheType cacheType) where T : MemberInfo
		{
			return GetMemberCache(ref m_cache).GetMemberList(listType, name, cacheType);
		}

		private MemberInfoCache<T> GetMemberCache<T>(ref MemberInfoCache<T> m_cache) where T : MemberInfo
		{
			MemberInfoCache<T> memberInfoCache = m_cache;
			if (memberInfoCache == null)
			{
				MemberInfoCache<T> memberInfoCache2 = new MemberInfoCache<T>(this);
				memberInfoCache = Interlocked.CompareExchange(ref m_cache, memberInfoCache2, null);
				if (memberInfoCache == null)
				{
					memberInfoCache = memberInfoCache2;
				}
			}
			return memberInfoCache;
		}

		internal string GetName()
		{
			return m_name ?? ConstructName(ref m_name, TypeNameFormatFlags.FormatBasic);
		}

		internal string GetFullName()
		{
			string text = m_fullName;
			if (text == null)
			{
				if (!IsFullNameRoundtripCompatible(m_runtimeType))
				{
					return null;
				}
				text = ConstructName(ref m_fullName, (TypeNameFormatFlags)3);
			}
			return text;
		}

		internal string GetToString()
		{
			return m_toString ?? ConstructName(ref m_toString, TypeNameFormatFlags.FormatNamespace);
		}

		internal string GetAssemblyQualifiedName()
		{
			string text = m_assemblyQualifiedName;
			if (text == null)
			{
				if (!IsFullNameRoundtripCompatible(m_runtimeType))
				{
					return null;
				}
				text = ConstructName(ref m_assemblyQualifiedName, (TypeNameFormatFlags)7);
			}
			return text;
		}

		internal string GetNamespace()
		{
			if (m_namespace == null)
			{
				Type type = m_runtimeType.GetRootElementType();
				if (type.IsFunctionPointer)
				{
					return null;
				}
				while (type.IsNested)
				{
					type = type.DeclaringType;
				}
				RuntimeModule runtimeModule = ((RuntimeType)type).GetRuntimeModule();
				m_namespace = runtimeModule.MetadataImport.GetNamespace(type.MetadataToken).ToString();
				GC.KeepAlive(runtimeModule);
			}
			return m_namespace;
		}

		internal RuntimeType GetEnclosingType()
		{
			if (m_enclosingType == null)
			{
				RuntimeType declaringType = RuntimeTypeHandle.GetDeclaringType(GetRuntimeType());
				m_enclosingType = declaringType ?? ((RuntimeType)typeof(void));
			}
			if (!(m_enclosingType == typeof(void)))
			{
				return m_enclosingType;
			}
			return null;
		}

		internal RuntimeType GetRuntimeType()
		{
			return m_runtimeType;
		}

		internal void InvalidateCachedNestedType()
		{
			m_nestedClassesCache = null;
		}

		internal string GetDefaultMemberName()
		{
			if (m_defaultMemberName == null)
			{
				CustomAttributeData customAttributeData = null;
				Type typeFromHandle = typeof(DefaultMemberAttribute);
				RuntimeType runtimeType = m_runtimeType;
				while (runtimeType != null)
				{
					IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(runtimeType);
					for (int i = 0; i < customAttributes.Count; i++)
					{
						if ((object)customAttributes[i].Constructor.DeclaringType == typeFromHandle)
						{
							customAttributeData = customAttributes[i];
							break;
						}
					}
					if (customAttributeData != null)
					{
						m_defaultMemberName = customAttributeData.ConstructorArguments[0].Value as string;
						break;
					}
					runtimeType = runtimeType.GetBaseType();
				}
			}
			return m_defaultMemberName;
		}

		internal object[] GetEmptyArray()
		{
			return _emptyArray ?? (_emptyArray = (object[])Array.CreateInstance(m_runtimeType, 0));
		}

		internal RuntimeType GetGenericTypeDefinition()
		{
			return _genericTypeDefinition ?? CacheGenericDefinition();
			[MethodImpl(MethodImplOptions.NoInlining)]
			RuntimeType CacheGenericDefinition()
			{
				RuntimeType o = null;
				if (m_runtimeType.IsGenericTypeDefinition)
				{
					o = m_runtimeType;
				}
				else
				{
					RuntimeType type = m_runtimeType;
					RuntimeTypeHandle.GetGenericTypeDefinition(new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o));
				}
				return _genericTypeDefinition = o;
			}
		}

		internal MethodInfo GetGenericMethodInfo(RuntimeMethodHandleInternal genericMethod)
		{
			LoaderAllocator loaderAllocator = RuntimeMethodHandle.GetLoaderAllocator(genericMethod);
			RuntimeMethodInfo runtimeMethodInfo = new RuntimeMethodInfo(genericMethod, RuntimeMethodHandle.GetDeclaringType(genericMethod), this, RuntimeMethodHandle.GetAttributes(genericMethod), (BindingFlags)(-1), loaderAllocator);
			RuntimeMethodInfo runtimeMethodInfo2 = ((loaderAllocator == null) ? s_methodInstantiations[runtimeMethodInfo] : loaderAllocator.m_methodInstantiations[runtimeMethodInfo]);
			if (runtimeMethodInfo2 != null)
			{
				return runtimeMethodInfo2;
			}
			if (s_methodInstantiationsLock == null)
			{
				Interlocked.CompareExchange(ref s_methodInstantiationsLock, new object(), null);
			}
			bool lockTaken = false;
			try
			{
				Monitor.Enter(s_methodInstantiationsLock, ref lockTaken);
				if (loaderAllocator != null)
				{
					runtimeMethodInfo2 = loaderAllocator.m_methodInstantiations[runtimeMethodInfo];
					if (runtimeMethodInfo2 != null)
					{
						return runtimeMethodInfo2;
					}
					loaderAllocator.m_methodInstantiations[runtimeMethodInfo] = runtimeMethodInfo;
				}
				else
				{
					runtimeMethodInfo2 = s_methodInstantiations[runtimeMethodInfo];
					if (runtimeMethodInfo2 != null)
					{
						return runtimeMethodInfo2;
					}
					s_methodInstantiations[runtimeMethodInfo] = runtimeMethodInfo;
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(s_methodInstantiationsLock);
				}
			}
			return runtimeMethodInfo;
		}

		internal RuntimeMethodInfo[] GetMethodList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_methodInfoCache, listType, name, CacheType.Method);
		}

		internal RuntimeConstructorInfo[] GetConstructorList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_constructorInfoCache, listType, name, CacheType.Constructor);
		}

		internal RuntimePropertyInfo[] GetPropertyList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_propertyInfoCache, listType, name, CacheType.Property);
		}

		internal RuntimeEventInfo[] GetEventList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_eventInfoCache, listType, name, CacheType.Event);
		}

		internal RuntimeFieldInfo[] GetFieldList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_fieldInfoCache, listType, name, CacheType.Field);
		}

		internal RuntimeType[] GetInterfaceList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_interfaceCache, listType, name, CacheType.Interface);
		}

		internal RuntimeType[] GetNestedTypeList(MemberListType listType, string name)
		{
			return GetMemberList(ref m_nestedClassesCache, listType, name, CacheType.NestedType);
		}

		internal MethodBase GetMethod(RuntimeType declaringType, RuntimeMethodHandleInternal method)
		{
			GetMemberCache(ref m_methodInfoCache);
			return m_methodInfoCache.AddMethod(declaringType, method, CacheType.Method);
		}

		internal MethodBase GetConstructor(RuntimeType declaringType, RuntimeMethodHandleInternal constructor)
		{
			GetMemberCache(ref m_constructorInfoCache);
			return m_constructorInfoCache.AddMethod(declaringType, constructor, CacheType.Constructor);
		}

		internal FieldInfo GetField(RuntimeFieldHandleInternal field)
		{
			GetMemberCache(ref m_fieldInfoCache);
			return m_fieldInfoCache.AddField(field);
		}
	}

	[Flags]
	private enum DispatchWrapperType
	{
		Unknown = 1,
		Dispatch = 2,
		Error = 8,
		Currency = 0x10,
		BStr = 0x20,
		SafeArray = 0x10000
	}

	internal sealed class BoxCache : IGenericCacheEntry<BoxCache>, IGenericCacheEntry
	{
		private unsafe readonly delegate*<void*, object> _pfnAllocator;

		private unsafe readonly void* _allocatorFirstArg;

		private readonly int _nullableValueOffset;

		private readonly uint _valueTypeSize;

		private unsafe readonly MethodTable* _pMT;

		public static BoxCache Create(RuntimeType type)
		{
			return new BoxCache(type);
		}

		public void InitializeCompositeCache(CompositeCacheEntry compositeEntry)
		{
			compositeEntry._boxCache = this;
		}

		public static ref BoxCache GetStorageRef(CompositeCacheEntry compositeEntry)
		{
			return ref compositeEntry._boxCache;
		}

		private unsafe BoxCache(RuntimeType rt)
		{
			TypeHandle nativeTypeHandle = rt.GetNativeTypeHandle();
			if (nativeTypeHandle.IsTypeDesc)
			{
				throw new ArgumentException(SR.Arg_TypeNotSupported);
			}
			_pMT = nativeTypeHandle.AsMethodTable();
			if (_pMT->ContainsGenericVariables)
			{
				throw new ArgumentException(SR.Arg_TypeNotSupported);
			}
			if (_pMT->IsValueType)
			{
				GetBoxInfo(rt, out _pfnAllocator, out _allocatorFirstArg, out _nullableValueOffset, out _valueTypeSize);
			}
		}

		internal unsafe object Box(RuntimeType rt, ref byte data)
		{
			if (_pfnAllocator == (delegate*<void*, object>)null)
			{
				return Unsafe.As<byte, object>(ref data);
			}
			ref byte reference = ref data;
			byte b = Unsafe.ReadUnaligned<byte>(in reference);
			if (_nullableValueOffset != 0)
			{
				if (b == 0)
				{
					return null;
				}
				reference = ref Unsafe.Add(ref reference, _nullableValueOffset);
			}
			object obj = _pfnAllocator(_allocatorFirstArg);
			GC.KeepAlive(rt);
			if (_pMT->ContainsGCPointers)
			{
				Buffer.BulkMoveWithWriteBarrier(ref obj.GetRawData(), ref reference, _valueTypeSize);
			}
			else
			{
				SpanHelpers.Memmove(ref obj.GetRawData(), ref reference, _valueTypeSize);
			}
			return obj;
		}

		private unsafe static void GetBoxInfo(RuntimeType rt, out delegate*<void*, object> pfnAllocator, out void* vAllocatorFirstArg, out int nullableValueOffset, out uint valueTypeSize)
		{
			delegate*<void*, object> obj = default(delegate*<void*, object>);
			void* ptr = default(void*);
			int num = 0;
			uint num2 = 0u;
			GetBoxInfo(new QCallTypeHandle(ref rt), &obj, &ptr, &num, &num2);
			Unsafe.As<delegate*<void*, object>, IntPtr>(ref pfnAllocator) = (nint)obj;
			vAllocatorFirstArg = ptr;
			nullableValueOffset = num;
			valueTypeSize = num2;
		}

		[DllImport("QCall", EntryPoint = "ReflectionInvocation_GetBoxInfo", ExactSpelling = true)]
		[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_GetBoxInfo")]
		private unsafe static extern void GetBoxInfo(QCallTypeHandle type, delegate*<void*, object>* ppfnAllocator, void** pvAllocatorFirstArg, int* pNullableValueOffset, uint* pValueTypeSize);
	}

	internal sealed class CreateUninitializedCache : IGenericCacheEntry<CreateUninitializedCache>, IGenericCacheEntry
	{
		private unsafe readonly delegate*<void*, object> _pfnAllocator;

		private unsafe readonly void* _allocatorFirstArg;

		public static CreateUninitializedCache Create(RuntimeType type)
		{
			return new CreateUninitializedCache(type);
		}

		public void InitializeCompositeCache(CompositeCacheEntry compositeEntry)
		{
			compositeEntry._createUninitializedCache = this;
		}

		public static ref CreateUninitializedCache GetStorageRef(CompositeCacheEntry compositeEntry)
		{
			return ref compositeEntry._createUninitializedCache;
		}

		private unsafe CreateUninitializedCache(RuntimeType rt)
		{
			GetCreateUninitializedInfo(rt, out _pfnAllocator, out _allocatorFirstArg);
		}

		internal unsafe object CreateUninitializedObject(RuntimeType rt)
		{
			object result = _pfnAllocator(_allocatorFirstArg);
			GC.KeepAlive(rt);
			return result;
		}

		private unsafe static void GetCreateUninitializedInfo(RuntimeType rt, out delegate*<void*, object> pfnAllocator, out void* vAllocatorFirstArg)
		{
			delegate*<void*, object> obj = default(delegate*<void*, object>);
			void* ptr = default(void*);
			GetCreateUninitializedInfo(new QCallTypeHandle(ref rt), &obj, &ptr);
			Unsafe.As<delegate*<void*, object>, IntPtr>(ref pfnAllocator) = (nint)obj;
			vAllocatorFirstArg = ptr;
		}

		[DllImport("QCall", EntryPoint = "ReflectionSerialization_GetCreateUninitializedObjectInfo", ExactSpelling = true)]
		[LibraryImport("QCall", EntryPoint = "ReflectionSerialization_GetCreateUninitializedObjectInfo")]
		private unsafe static extern void GetCreateUninitializedInfo(QCallTypeHandle type, delegate*<void*, object>* ppfnAllocator, void** pvAllocatorFirstArg);
	}

	internal sealed class CompositeCacheEntry : IGenericCacheEntry
	{
		internal ActivatorCache _activatorCache;

		internal CreateUninitializedCache _createUninitializedCache;

		internal RuntimeTypeCache.FunctionPointerCache _functionPointerCache;

		internal Array.ArrayInitializeCache _arrayInitializeCache;

		internal IGenericCacheEntry _enumInfo;

		internal BoxCache _boxCache;

		void IGenericCacheEntry.InitializeCompositeCache(CompositeCacheEntry compositeEntry)
		{
			throw new UnreachableException();
		}
	}

	internal interface IGenericCacheEntry
	{
		void InitializeCompositeCache(CompositeCacheEntry compositeEntry);
	}

	internal interface IGenericCacheEntry<TCache> : IGenericCacheEntry where TCache : class, IGenericCacheEntry<TCache>
	{
		static abstract TCache Create(RuntimeType type);

		static abstract ref TCache GetStorageRef(CompositeCacheEntry compositeEntry);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TCache GetOrCreate(RuntimeType type)
		{
			IGenericCacheEntry genericCache = type.Cache.GenericCache;
			if (genericCache != null)
			{
				if (genericCache is TCache result)
				{
					return result;
				}
				if (genericCache is CompositeCacheEntry compositeEntry)
				{
					TCache storageRef = GetStorageRef(compositeEntry);
					if (storageRef != null)
					{
						return storageRef;
					}
				}
			}
			return CreateAndCache(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TCache Find(RuntimeType type)
		{
			IGenericCacheEntry genericCache = type.Cache.GenericCache;
			if (genericCache != null)
			{
				if (genericCache is TCache result)
				{
					return result;
				}
				if (genericCache is CompositeCacheEntry compositeEntry)
				{
					return GetStorageRef(compositeEntry);
				}
			}
			return null;
		}

		static TCache Replace(RuntimeType type, TCache newEntry)
		{
			ref IGenericCacheEntry genericCache = ref type.Cache.GenericCache;
			while (true)
			{
				IGenericCacheEntry genericCacheEntry = genericCache;
				if ((genericCacheEntry != null && !(genericCacheEntry is TCache)) || 1 == 0)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref genericCache, newEntry, genericCacheEntry) == genericCacheEntry)
				{
					return newEntry;
				}
			}
			CompositeCacheEntry compositeCacheEntry;
			TCache storageRef;
			do
			{
				IL_0035:
				IGenericCacheEntry genericCacheEntry2 = genericCache;
				compositeCacheEntry = genericCacheEntry2 as CompositeCacheEntry;
				if (compositeCacheEntry == null)
				{
					compositeCacheEntry = new CompositeCacheEntry();
					genericCacheEntry2.InitializeCompositeCache(compositeCacheEntry);
					if (Interlocked.CompareExchange(ref genericCache, compositeCacheEntry, genericCacheEntry2) != genericCacheEntry2)
					{
						goto IL_0035;
					}
				}
				storageRef = GetStorageRef(compositeCacheEntry);
			}
			while (Interlocked.CompareExchange(ref GetStorageRef(compositeCacheEntry), newEntry, storageRef) != storageRef);
			return newEntry;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static TCache CreateAndCache(RuntimeType type)
		{
			CompositeCacheEntry compositeCacheEntry;
			while (true)
			{
				ref IGenericCacheEntry genericCache = ref type.Cache.GenericCache;
				IGenericCacheEntry genericCacheEntry = genericCache;
				if (genericCacheEntry == null)
				{
					TCache val = Create(type);
					if (Interlocked.CompareExchange(ref genericCache, val, null) == null)
					{
						return val;
					}
					continue;
				}
				if (genericCacheEntry is TCache result)
				{
					return result;
				}
				compositeCacheEntry = genericCacheEntry as CompositeCacheEntry;
				if (compositeCacheEntry != null)
				{
					break;
				}
				compositeCacheEntry = new CompositeCacheEntry();
				genericCacheEntry.InitializeCompositeCache(compositeCacheEntry);
				if (Interlocked.CompareExchange(ref genericCache, compositeCacheEntry, genericCacheEntry) == genericCacheEntry)
				{
					break;
				}
			}
			TCache val2 = Create(type);
			return Interlocked.CompareExchange(ref GetStorageRef(compositeCacheEntry), val2, null) ?? val2;
		}
	}

	private enum CheckValueStatus
	{
		Success,
		ArgumentException,
		NotSupported_ByRefLike
	}

	private readonly object m_keepalive;

	private nint m_cache;

	internal nint m_handle;

	internal static readonly RuntimeType ValueType = (RuntimeType)typeof(ValueType);

	private static readonly RuntimeType ObjectType = (RuntimeType)typeof(object);

	private static readonly RuntimeType StringType = (RuntimeType)typeof(string);

	private const int GenericParameterCountAny = -1;

	private static OleAutBinder s_ForwardCallBinder;

	private RuntimeTypeCache Cache
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (m_cache != IntPtr.Zero)
			{
				object obj = GCHandle.InternalGet(m_cache);
				if (obj != null)
				{
					return Unsafe.As<RuntimeTypeCache>(obj);
				}
			}
			return InitializeCache();
		}
	}

	public sealed override bool IsCollectible
	{
		get
		{
			RuntimeType type = this;
			return RuntimeTypeHandle.IsCollectible(new QCallTypeHandle(ref type)) != Interop.BOOL.FALSE;
		}
	}

	public override MethodBase DeclaringMethod
	{
		get
		{
			if (!IsGenericParameter)
			{
				throw new InvalidOperationException(SR.Arg_NotGenericParameter);
			}
			IRuntimeMethodInfo declaringMethodForGenericParameter = RuntimeTypeHandle.GetDeclaringMethodForGenericParameter(this);
			if (declaringMethodForGenericParameter == null)
			{
				return null;
			}
			return GetMethodBase(RuntimeMethodHandle.GetDeclaringType(declaringMethodForGenericParameter), declaringMethodForGenericParameter);
		}
	}

	public override string FullName => Cache.GetFullName();

	public override string AssemblyQualifiedName => Cache.GetAssemblyQualifiedName();

	public override string Namespace
	{
		get
		{
			string text = Cache.GetNamespace();
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return text;
		}
	}

	public unsafe override Guid GUID
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			if (nativeTypeHandle.IsTypeDesc || nativeTypeHandle.AsMethodTable()->IsArray)
			{
				return Guid.Empty;
			}
			Unsafe.SkipInit(out Guid result);
			if (System.Runtime.CompilerServices.TypeHandle.AreSameType(nativeTypeHandle, System.Runtime.CompilerServices.TypeHandle.TypeHandleOf<__ComObject>()))
			{
				GetComObjectGuidWorker(this, &result);
			}
			else
			{
				GetGuid(nativeTypeHandle.AsMethodTable(), &result);
			}
			GC.KeepAlive(this);
			return result;
		}
	}

	internal unsafe bool IsActualValueType
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsValueType;
			GC.KeepAlive(this);
			return result;
		}
	}

	public unsafe override bool IsEnum
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			if (nativeTypeHandle.IsTypeDesc)
			{
				return IsSubclassOf(typeof(Enum));
			}
			bool result = nativeTypeHandle.AsMethodTable()->ParentMethodTable == System.Runtime.CompilerServices.TypeHandle.TypeHandleOf<Enum>().AsMethodTable();
			GC.KeepAlive(this);
			return result;
		}
	}

	internal unsafe bool IsActualEnum
	{
		[Intrinsic]
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->ParentMethodTable == System.Runtime.CompilerServices.TypeHandle.TypeHandleOf<Enum>().AsMethodTable();
			GC.KeepAlive(this);
			return result;
		}
	}

	internal unsafe bool IsActualInterface
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsInterface;
			GC.KeepAlive(this);
			return result;
		}
	}

	public unsafe override bool IsByRefLike
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsByRefLike;
			GC.KeepAlive(this);
			return result;
		}
	}

	public unsafe override bool IsConstructedGenericType
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsConstructedGenericType;
			GC.KeepAlive(this);
			return result;
		}
	}

	public unsafe override bool IsGenericType
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->HasInstantiation;
			GC.KeepAlive(this);
			return result;
		}
	}

	public unsafe override bool IsGenericTypeDefinition
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsGenericTypeDefinition;
			GC.KeepAlive(this);
			return result;
		}
	}

	public override GenericParameterAttributes GenericParameterAttributes
	{
		get
		{
			if (!IsGenericParameter)
			{
				throw new InvalidOperationException(SR.Arg_NotGenericParameter);
			}
			RuntimeModule runtimeModule = GetRuntimeModule();
			runtimeModule.MetadataImport.GetGenericParamProps(MetadataToken, out var attributes);
			GC.KeepAlive(runtimeModule);
			return attributes;
		}
	}

	public sealed override bool IsSZArray => RuntimeTypeHandle.IsSZArray(this);

	public override int GenericParameterPosition
	{
		get
		{
			if (!IsGenericParameter)
			{
				throw new InvalidOperationException(SR.Arg_NotGenericParameter);
			}
			return new RuntimeTypeHandle(this).GetGenericVariableIndex();
		}
	}

	public override bool ContainsGenericParameters => GetRootElementType().TypeHandle.ContainsGenericVariables();

	internal unsafe bool IsNullableOfT
	{
		get
		{
			TypeHandle nativeTypeHandle = GetNativeTypeHandle();
			bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->IsNullable;
			GC.KeepAlive(this);
			return result;
		}
	}

	public override StructLayoutAttribute StructLayoutAttribute => PseudoCustomAttribute.GetStructLayoutCustomAttribute(this);

	public override bool IsFunctionPointer => RuntimeTypeHandle.IsFunctionPointer(this);

	public override bool IsUnmanagedFunctionPointer => RuntimeTypeHandle.IsUnmanagedFunctionPointer(this);

	public override string Name => Cache.GetName();

	public override Type DeclaringType => Cache.GetEnclosingType();

	private static OleAutBinder ForwardCallBinder => s_ForwardCallBinder ?? (s_ForwardCallBinder = new OleAutBinder());

	public override Assembly Assembly => RuntimeTypeHandle.GetAssembly(this);

	public override Type BaseType => GetBaseType();

	public override bool IsGenericParameter => RuntimeTypeHandle.IsGenericVariable(this);

	public override bool IsTypeDefinition => RuntimeTypeHandle.IsTypeDefinition(this);

	public override bool IsSecurityCritical => true;

	public override bool IsSecuritySafeCritical => false;

	public override bool IsSecurityTransparent => false;

	public override MemberTypes MemberType
	{
		get
		{
			if (!base.IsPublic && !base.IsNotPublic)
			{
				return MemberTypes.NestedType;
			}
			return MemberTypes.TypeInfo;
		}
	}

	public override int MetadataToken => RuntimeTypeHandle.GetToken(this);

	public override Module Module => GetRuntimeModule();

	public override Type ReflectedType => DeclaringType;

	public override RuntimeTypeHandle TypeHandle
	{
		[Intrinsic]
		get
		{
			return new RuntimeTypeHandle(this);
		}
	}

	public override Type UnderlyingSystemType => this;

	internal unsafe RuntimeType GetParentType()
	{
		TypeHandle nativeTypeHandle = GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			return null;
		}
		MethodTable* parentMethodTable = nativeTypeHandle.AsMethodTable()->ParentMethodTable;
		if (parentMethodTable == null)
		{
			return null;
		}
		RuntimeType runtimeType = RuntimeTypeHandle.GetRuntimeType(parentMethodTable);
		GC.KeepAlive(this);
		return runtimeType;
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	internal static MethodBase GetMethodBase(RuntimeModule scope, int typeMetadataToken)
	{
		return GetMethodBase(new ModuleHandle(scope).ResolveMethodHandle(typeMetadataToken).GetMethodInfo());
	}

	internal static MethodBase GetMethodBase(IRuntimeMethodInfo methodHandle)
	{
		return GetMethodBase(null, methodHandle);
	}

	internal static MethodBase GetMethodBase(RuntimeType reflectedType, IRuntimeMethodInfo methodHandle)
	{
		MethodBase methodBase = GetMethodBase(reflectedType, methodHandle.Value);
		GC.KeepAlive(methodHandle);
		return methodBase;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "The code in this method looks up the method by name, but it always starts with a method handle.To get here something somewhere had to get the method handle and thus the method must exist.")]
	internal static MethodBase GetMethodBase(RuntimeType reflectedType, RuntimeMethodHandleInternal methodHandle)
	{
		if (RuntimeMethodHandle.IsDynamicMethod(methodHandle))
		{
			return RuntimeMethodHandle.GetResolver(methodHandle)?.GetDynamicMethod();
		}
		RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(methodHandle);
		RuntimeType[] array = null;
		if ((object)reflectedType == null)
		{
			reflectedType = runtimeType;
		}
		if (reflectedType != runtimeType && !reflectedType.IsSubclassOf(runtimeType))
		{
			if (reflectedType.IsArray)
			{
				MethodBase[] array2 = reflectedType.GetMember(RuntimeMethodHandle.GetName(methodHandle), MemberTypes.Constructor | MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) as MethodBase[];
				bool flag = false;
				for (int i = 0; i < array2.Length; i++)
				{
					if (((IRuntimeMethodInfo)array2[i]).Value.Value == methodHandle.Value)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					throw new ArgumentException(SR.Format(SR.Argument_ResolveMethodHandle, reflectedType, runtimeType));
				}
			}
			else if (runtimeType.IsGenericType)
			{
				RuntimeType runtimeType2 = (RuntimeType)runtimeType.GetGenericTypeDefinition();
				RuntimeType runtimeType3 = reflectedType;
				while (runtimeType3 != null)
				{
					RuntimeType runtimeType4 = runtimeType3;
					if (runtimeType4.IsGenericType && !runtimeType3.IsGenericTypeDefinition)
					{
						runtimeType4 = (RuntimeType)runtimeType4.GetGenericTypeDefinition();
					}
					if (runtimeType4 == runtimeType2)
					{
						break;
					}
					runtimeType3 = runtimeType3.GetBaseType();
				}
				if (runtimeType3 == null)
				{
					throw new ArgumentException(SR.Format(SR.Argument_ResolveMethodHandle, reflectedType, runtimeType));
				}
				runtimeType = runtimeType3;
				if (!RuntimeMethodHandle.IsGenericMethodDefinition(methodHandle))
				{
					array = RuntimeMethodHandle.GetMethodInstantiationInternal(methodHandle);
				}
				methodHandle = RuntimeMethodHandle.GetMethodFromCanonical(methodHandle, runtimeType);
			}
			else if (!runtimeType.IsAssignableFrom(reflectedType))
			{
				throw new ArgumentException(SR.Format(SR.Argument_ResolveMethodHandle, reflectedType.ToString(), runtimeType.ToString()));
			}
		}
		methodHandle = RuntimeMethodHandle.GetStubIfNeeded(methodHandle, runtimeType, array);
		MethodBase result = (RuntimeMethodHandle.IsConstructor(methodHandle) ? reflectedType.Cache.GetConstructor(runtimeType, methodHandle) : ((!RuntimeMethodHandle.HasMethodInstantiation(methodHandle) || RuntimeMethodHandle.IsGenericMethodDefinition(methodHandle)) ? reflectedType.Cache.GetMethod(runtimeType, methodHandle) : reflectedType.Cache.GetGenericMethodInfo(methodHandle)));
		GC.KeepAlive(array);
		return result;
	}

	internal T GetOrCreateCacheEntry<T>() where T : class, IGenericCacheEntry<T>
	{
		return IGenericCacheEntry<T>.GetOrCreate(this);
	}

	internal T FindCacheEntry<T>() where T : class, IGenericCacheEntry<T>
	{
		return IGenericCacheEntry<T>.Find(this);
	}

	internal T ReplaceCacheEntry<T>(T entry) where T : class, IGenericCacheEntry<T>
	{
		IGenericCacheEntry<T>.Replace(this, entry);
		return entry;
	}

	internal static FieldInfo GetFieldInfo(IRuntimeFieldInfo fieldHandle)
	{
		return GetFieldInfo(RuntimeFieldHandle.GetApproxDeclaringType(fieldHandle), fieldHandle);
	}

	internal static FieldInfo GetFieldInfo(RuntimeType reflectedType, IRuntimeFieldInfo field)
	{
		RuntimeFieldHandleInternal value = field.Value;
		if (reflectedType == null)
		{
			reflectedType = RuntimeFieldHandle.GetApproxDeclaringType(value);
		}
		else
		{
			RuntimeType approxDeclaringType = RuntimeFieldHandle.GetApproxDeclaringType(value);
			if (reflectedType != approxDeclaringType && (!RuntimeFieldHandle.AcquiresContextFromThis(value) || !RuntimeTypeHandle.CompareCanonicalHandles(approxDeclaringType, reflectedType)))
			{
				throw new ArgumentException(SR.Format(SR.Argument_ResolveFieldHandle, reflectedType, approxDeclaringType));
			}
		}
		FieldInfo field2 = reflectedType.Cache.GetField(value);
		GC.KeepAlive(field);
		return field2;
	}

	internal static void ValidateGenericArguments(MemberInfo definition, RuntimeType[] genericArguments, Exception e)
	{
		RuntimeMethodInfo runtimeMethodInfo = null;
		RuntimeType runtimeType;
		RuntimeType[] genericArgumentsInternal;
		if (definition is Type)
		{
			runtimeType = (RuntimeType)definition;
			genericArgumentsInternal = runtimeType.GetGenericArgumentsInternal();
		}
		else
		{
			runtimeMethodInfo = (RuntimeMethodInfo)definition;
			runtimeType = (RuntimeType)runtimeMethodInfo.DeclaringType;
			genericArgumentsInternal = runtimeMethodInfo.GetGenericArgumentsInternal();
		}
		for (int i = 0; i < genericArguments.Length; i++)
		{
			Type type = genericArguments[i];
			Type type2 = genericArgumentsInternal[i];
			if (!RuntimeTypeHandle.SatisfiesConstraints(type2.TypeHandle.GetRuntimeTypeChecked(), runtimeType, runtimeMethodInfo, type.TypeHandle.GetRuntimeTypeChecked()))
			{
				throw new ArgumentException(SR.Format(SR.Argument_GenConstraintViolation, i.ToString(), type, definition, type2), e);
			}
		}
	}

	private static void SplitName(string fullname, out string name, out string ns)
	{
		name = null;
		ns = null;
		if (fullname != null)
		{
			int num = fullname.LastIndexOf('.');
			if (num >= 0)
			{
				ns = fullname.Substring(0, num);
				name = fullname.Substring(num + 1);
			}
			else
			{
				name = fullname;
			}
		}
	}

	internal static BindingFlags FilterPreCalculate(bool isPublic, bool isInherited, bool isStatic)
	{
		BindingFlags bindingFlags = (isPublic ? BindingFlags.Public : BindingFlags.NonPublic);
		if (isInherited)
		{
			bindingFlags |= BindingFlags.DeclaredOnly;
			if (isStatic)
			{
				return bindingFlags | (BindingFlags.Static | BindingFlags.FlattenHierarchy);
			}
			return bindingFlags | BindingFlags.Instance;
		}
		if (isStatic)
		{
			return bindingFlags | BindingFlags.Static;
		}
		return bindingFlags | BindingFlags.Instance;
	}

	private static void FilterHelper(BindingFlags bindingFlags, ref string name, bool allowPrefixLookup, out bool prefixLookup, out bool ignoreCase, out MemberListType listType)
	{
		prefixLookup = false;
		ignoreCase = false;
		if (name != null)
		{
			if ((bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default)
			{
				name = name.ToLowerInvariant();
				ignoreCase = true;
				listType = MemberListType.CaseInsensitive;
			}
			else
			{
				listType = MemberListType.CaseSensitive;
			}
			if (allowPrefixLookup && name.EndsWith('*'))
			{
				string text = name;
				name = text.Substring(0, text.Length - 1);
				prefixLookup = true;
				listType = MemberListType.All;
			}
		}
		else
		{
			listType = MemberListType.All;
		}
	}

	private static void FilterHelper(BindingFlags bindingFlags, ref string name, out bool ignoreCase, out MemberListType listType)
	{
		FilterHelper(bindingFlags, ref name, allowPrefixLookup: false, out var _, out ignoreCase, out listType);
	}

	private static bool FilterApplyPrefixLookup(MemberInfo memberInfo, string name, bool ignoreCase)
	{
		if (ignoreCase)
		{
			if (!memberInfo.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		else if (!memberInfo.Name.StartsWith(name, StringComparison.Ordinal))
		{
			return false;
		}
		return true;
	}

	private static bool FilterApplyBase(MemberInfo memberInfo, BindingFlags bindingFlags, bool isPublic, bool isNonProtectedInternal, bool isStatic, string name, bool prefixLookup)
	{
		if (isPublic)
		{
			if ((bindingFlags & BindingFlags.Public) == 0)
			{
				return false;
			}
		}
		else if ((bindingFlags & BindingFlags.NonPublic) == 0)
		{
			return false;
		}
		bool flag = (object)memberInfo.DeclaringType != memberInfo.ReflectedType;
		if (((bindingFlags & BindingFlags.DeclaredOnly) != 0) & flag)
		{
			return false;
		}
		if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.MemberType != MemberTypes.NestedType)
		{
			if (isStatic)
			{
				if (((bindingFlags & BindingFlags.FlattenHierarchy) == 0) & flag)
				{
					return false;
				}
				if ((bindingFlags & BindingFlags.Static) == 0)
				{
					return false;
				}
			}
			else if ((bindingFlags & BindingFlags.Instance) == 0)
			{
				return false;
			}
		}
		if (prefixLookup && !FilterApplyPrefixLookup(memberInfo, name, (bindingFlags & BindingFlags.IgnoreCase) != 0))
		{
			return false;
		}
		if ((((bindingFlags & BindingFlags.DeclaredOnly) == 0) & flag & isNonProtectedInternal) && (bindingFlags & BindingFlags.NonPublic) != BindingFlags.Default && !isStatic && (bindingFlags & BindingFlags.Instance) != BindingFlags.Default)
		{
			MethodInfo methodInfo = memberInfo as MethodInfo;
			if (methodInfo == null)
			{
				return false;
			}
			if (!methodInfo.IsVirtual && !methodInfo.IsAbstract)
			{
				return false;
			}
		}
		return true;
	}

	private static bool FilterApplyType(Type type, BindingFlags bindingFlags, string name, bool prefixLookup, string ns)
	{
		bool isPublic = type.IsNestedPublic || type.IsPublic;
		if (!FilterApplyBase(type, bindingFlags, isPublic, type.IsNestedAssembly, isStatic: false, name, prefixLookup))
		{
			return false;
		}
		if (ns != null && ns != type.Namespace)
		{
			return false;
		}
		return true;
	}

	private static bool FilterApplyMethodInfo(RuntimeMethodInfo method, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
	{
		return FilterApplyMethodBase(method, method.BindingFlags, bindingFlags, callConv, argumentTypes);
	}

	private static bool FilterApplyConstructorInfo(RuntimeConstructorInfo constructor, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
	{
		return FilterApplyMethodBase(constructor, constructor.BindingFlags, bindingFlags, callConv, argumentTypes);
	}

	private static bool FilterApplyMethodBase(MethodBase methodBase, BindingFlags methodFlags, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
	{
		bindingFlags ^= BindingFlags.DeclaredOnly;
		if ((bindingFlags & methodFlags) != methodFlags)
		{
			return false;
		}
		if ((callConv & CallingConventions.Any) == 0)
		{
			if ((callConv & CallingConventions.VarArgs) != 0 && (methodBase.CallingConvention & CallingConventions.VarArgs) == 0)
			{
				return false;
			}
			if ((callConv & CallingConventions.Standard) != 0 && (methodBase.CallingConvention & CallingConventions.Standard) == 0)
			{
				return false;
			}
		}
		if (argumentTypes != null)
		{
			ReadOnlySpan<ParameterInfo> parametersAsSpan = methodBase.GetParametersAsSpan();
			if (argumentTypes.Length != parametersAsSpan.Length)
			{
				if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetProperty | BindingFlags.SetProperty)) == 0)
				{
					return false;
				}
				bool flag = false;
				if (argumentTypes.Length > parametersAsSpan.Length)
				{
					if ((methodBase.CallingConvention & CallingConventions.VarArgs) == 0)
					{
						flag = true;
					}
				}
				else if ((bindingFlags & BindingFlags.OptionalParamBinding) == 0)
				{
					flag = true;
				}
				else if (!parametersAsSpan[argumentTypes.Length].IsOptional)
				{
					flag = true;
				}
				if (flag)
				{
					if (parametersAsSpan.Length == 0)
					{
						return false;
					}
					if (argumentTypes.Length < parametersAsSpan.Length - 1)
					{
						return false;
					}
					ParameterInfo parameterInfo = parametersAsSpan[parametersAsSpan.Length - 1];
					if (!parameterInfo.ParameterType.IsArray)
					{
						return false;
					}
					if (!parameterInfo.IsDefined(typeof(ParamArrayAttribute), inherit: false))
					{
						return false;
					}
				}
			}
			else if ((bindingFlags & BindingFlags.ExactBinding) != BindingFlags.Default && (bindingFlags & BindingFlags.InvokeMethod) == 0)
			{
				for (int i = 0; i < parametersAsSpan.Length; i++)
				{
					Type type = argumentTypes[i];
					if ((object)type != null && !type.MatchesParameterTypeExactly(parametersAsSpan[i]))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	internal RuntimeType()
	{
		throw new NotSupportedException();
	}

	internal unsafe TypeHandle GetNativeTypeHandle()
	{
		return new TypeHandle((void*)m_handle);
	}

	internal nint GetUnderlyingNativeHandle()
	{
		return m_handle;
	}

	internal override bool CacheEquals(object o)
	{
		if (o is RuntimeType runtimeType)
		{
			return runtimeType.m_handle == m_handle;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private RuntimeTypeCache InitializeCache()
	{
		if (m_cache == IntPtr.Zero)
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(this);
			nint gCHandle = runtimeTypeHandle.GetGCHandle(GCHandleType.WeakTrackResurrection);
			if (Interlocked.CompareExchange(ref m_cache, gCHandle, IntPtr.Zero) != IntPtr.Zero)
			{
				runtimeTypeHandle.FreeGCHandle(gCHandle);
			}
		}
		RuntimeTypeCache runtimeTypeCache = (RuntimeTypeCache)GCHandle.InternalGet(m_cache);
		if (runtimeTypeCache == null)
		{
			runtimeTypeCache = new RuntimeTypeCache(this);
			RuntimeTypeCache runtimeTypeCache2 = (RuntimeTypeCache)GCHandle.InternalCompareExchange(m_cache, runtimeTypeCache, null);
			if (runtimeTypeCache2 != null)
			{
				runtimeTypeCache = runtimeTypeCache2;
			}
		}
		return runtimeTypeCache;
	}

	internal void ClearCache()
	{
		if (Volatile.Read(in m_cache) != IntPtr.Zero)
		{
			GCHandle.InternalSet(m_cache, null);
		}
	}

	private string GetDefaultMemberName()
	{
		return Cache.GetDefaultMemberName();
	}

	private ListBuilder<MethodInfo> GetMethodCandidates(string name, int genericParameterCount, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
	{
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var ignoreCase, out var listType);
		RuntimeMethodInfo[] methodList = Cache.GetMethodList(listType, name);
		ListBuilder<MethodInfo> result = new ListBuilder<MethodInfo>(methodList.Length);
		foreach (RuntimeMethodInfo runtimeMethodInfo in methodList)
		{
			if ((genericParameterCount == -1 || genericParameterCount == runtimeMethodInfo.GenericParameterCount) && FilterApplyMethodInfo(runtimeMethodInfo, bindingAttr, callConv, types) && (!prefixLookup || FilterApplyPrefixLookup(runtimeMethodInfo, name, ignoreCase)))
			{
				result.Add(runtimeMethodInfo);
			}
		}
		return result;
	}

	private ListBuilder<ConstructorInfo> GetConstructorCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
	{
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var ignoreCase, out var listType);
		RuntimeConstructorInfo[] constructorList = Cache.GetConstructorList(listType, name);
		ListBuilder<ConstructorInfo> result = new ListBuilder<ConstructorInfo>(constructorList.Length);
		foreach (RuntimeConstructorInfo runtimeConstructorInfo in constructorList)
		{
			if (FilterApplyConstructorInfo(runtimeConstructorInfo, bindingAttr, callConv, types) && (!prefixLookup || FilterApplyPrefixLookup(runtimeConstructorInfo, name, ignoreCase)))
			{
				result.Add(runtimeConstructorInfo);
			}
		}
		return result;
	}

	private ListBuilder<PropertyInfo> GetPropertyCandidates(string name, BindingFlags bindingAttr, Type[] types, bool allowPrefixLookup)
	{
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var ignoreCase, out var listType);
		RuntimePropertyInfo[] propertyList = Cache.GetPropertyList(listType, name);
		bindingAttr ^= BindingFlags.DeclaredOnly;
		ListBuilder<PropertyInfo> result = new ListBuilder<PropertyInfo>(propertyList.Length);
		foreach (RuntimePropertyInfo runtimePropertyInfo in propertyList)
		{
			if ((bindingAttr & runtimePropertyInfo.BindingFlags) == runtimePropertyInfo.BindingFlags && (!prefixLookup || FilterApplyPrefixLookup(runtimePropertyInfo, name, ignoreCase)) && (types == null || runtimePropertyInfo.GetIndexParameters().Length == types.Length))
			{
				result.Add(runtimePropertyInfo);
			}
		}
		return result;
	}

	private ListBuilder<EventInfo> GetEventCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
	{
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var ignoreCase, out var listType);
		RuntimeEventInfo[] eventList = Cache.GetEventList(listType, name);
		bindingAttr ^= BindingFlags.DeclaredOnly;
		ListBuilder<EventInfo> result = new ListBuilder<EventInfo>(eventList.Length);
		foreach (RuntimeEventInfo runtimeEventInfo in eventList)
		{
			if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags && (!prefixLookup || FilterApplyPrefixLookup(runtimeEventInfo, name, ignoreCase)))
			{
				result.Add(runtimeEventInfo);
			}
		}
		return result;
	}

	private ListBuilder<FieldInfo> GetFieldCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
	{
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var ignoreCase, out var listType);
		RuntimeFieldInfo[] fieldList = Cache.GetFieldList(listType, name);
		bindingAttr ^= BindingFlags.DeclaredOnly;
		ListBuilder<FieldInfo> result = new ListBuilder<FieldInfo>(fieldList.Length);
		foreach (RuntimeFieldInfo runtimeFieldInfo in fieldList)
		{
			if ((bindingAttr & runtimeFieldInfo.BindingFlags) == runtimeFieldInfo.BindingFlags && (!prefixLookup || FilterApplyPrefixLookup(runtimeFieldInfo, name, ignoreCase)))
			{
				result.Add(runtimeFieldInfo);
			}
		}
		return result;
	}

	private ListBuilder<Type> GetNestedTypeCandidates(string fullname, BindingFlags bindingAttr, bool allowPrefixLookup)
	{
		bindingAttr &= ~BindingFlags.Static;
		SplitName(fullname, out var name, out var ns);
		FilterHelper(bindingAttr, ref name, allowPrefixLookup, out var prefixLookup, out var _, out var listType);
		RuntimeType[] nestedTypeList = Cache.GetNestedTypeList(listType, name);
		ListBuilder<Type> result = new ListBuilder<Type>(nestedTypeList.Length);
		foreach (RuntimeType runtimeType in nestedTypeList)
		{
			if (FilterApplyType(runtimeType, bindingAttr, name, prefixLookup, ns))
			{
				result.Add(runtimeType);
			}
		}
		return result;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return GetMethodCandidates(null, -1, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return GetPropertyCandidates(null, bindingAttr, null, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return GetEventCandidates(null, bindingAttr, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return GetFieldCandidates(null, bindingAttr, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	public override Type[] GetInterfaces()
	{
		Type[] interfaceList = Cache.GetInterfaceList(MemberListType.All, null);
		return new ReadOnlySpan<Type>(interfaceList).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return GetNestedTypeCandidates(null, bindingAttr, allowPrefixLookup: false).ToArray();
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		ListBuilder<MethodInfo> methodCandidates = GetMethodCandidates(null, -1, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: false);
		ListBuilder<ConstructorInfo> constructorCandidates = GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: false);
		ListBuilder<PropertyInfo> propertyCandidates = GetPropertyCandidates(null, bindingAttr, null, allowPrefixLookup: false);
		ListBuilder<EventInfo> eventCandidates = GetEventCandidates(null, bindingAttr, allowPrefixLookup: false);
		ListBuilder<FieldInfo> fieldCandidates = GetFieldCandidates(null, bindingAttr, allowPrefixLookup: false);
		ListBuilder<Type> nestedTypeCandidates = GetNestedTypeCandidates(null, bindingAttr, allowPrefixLookup: false);
		MemberInfo[] array = new MemberInfo[methodCandidates.Count + constructorCandidates.Count + propertyCandidates.Count + eventCandidates.Count + fieldCandidates.Count + nestedTypeCandidates.Count];
		int num = 0;
		object[] array2 = array;
		methodCandidates.CopyTo(array2, num);
		num += methodCandidates.Count;
		array2 = array;
		constructorCandidates.CopyTo(array2, num);
		num += constructorCandidates.Count;
		array2 = array;
		propertyCandidates.CopyTo(array2, num);
		num += propertyCandidates.Count;
		array2 = array;
		eventCandidates.CopyTo(array2, num);
		num += eventCandidates.Count;
		array2 = array;
		fieldCandidates.CopyTo(array2, num);
		num += fieldCandidates.Count;
		array2 = array;
		nestedTypeCandidates.CopyTo(array2, num);
		num += nestedTypeCandidates.Count;
		return array;
	}

	public override InterfaceMapping GetInterfaceMap([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type interfaceType)
	{
		if (IsGenericParameter)
		{
			throw new InvalidOperationException(SR.Arg_GenericParameter);
		}
		ArgumentNullException.ThrowIfNull(interfaceType, "interfaceType");
		RuntimeType runtimeType = (interfaceType as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "interfaceType");
		RuntimeTypeHandle typeHandle = runtimeType.TypeHandle;
		TypeHandle.VerifyInterfaceIsImplemented(typeHandle);
		if (IsSZArray && interfaceType.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_ArrayGetInterfaceMap);
		}
		int numVirtualsAndStaticVirtuals = RuntimeTypeHandle.GetNumVirtualsAndStaticVirtuals(runtimeType);
		Unsafe.SkipInit(out InterfaceMapping result);
		result.InterfaceType = interfaceType;
		result.TargetType = this;
		result.InterfaceMethods = new MethodInfo[numVirtualsAndStaticVirtuals];
		result.TargetMethods = new MethodInfo[numVirtualsAndStaticVirtuals];
		int num = 0;
		for (int i = 0; i < numVirtualsAndStaticVirtuals; i++)
		{
			RuntimeMethodHandleInternal methodAt = RuntimeTypeHandle.GetMethodAt(runtimeType, i);
			if (methodAt.IsNullHandle())
			{
				continue;
			}
			MethodBase methodBase = GetMethodBase(runtimeType, methodAt);
			result.InterfaceMethods[num] = (MethodInfo)methodBase;
			RuntimeMethodHandleInternal interfaceMethodImplementation = TypeHandle.GetInterfaceMethodImplementation(typeHandle, methodAt);
			if (interfaceMethodImplementation.IsNullHandle())
			{
				num++;
				continue;
			}
			RuntimeType runtimeType2 = RuntimeMethodHandle.GetDeclaringType(interfaceMethodImplementation);
			if (!runtimeType2.IsActualInterface)
			{
				runtimeType2 = this;
			}
			RuntimeMethodInfo runtimeMethodInfo = (RuntimeMethodInfo)GetMethodBase(runtimeType2, interfaceMethodImplementation);
			result.TargetMethods[num++] = (((object)runtimeMethodInfo != null && runtimeMethodInfo.IsGenericMethod && !runtimeMethodInfo.IsGenericMethodDefinition) ? runtimeMethodInfo.GetGenericMethodDefinition() : runtimeMethodInfo);
		}
		if (num != numVirtualsAndStaticVirtuals)
		{
			Array.Resize(ref result.InterfaceMethods, num);
			Array.Resize(ref result.TargetMethods, num);
		}
		return result;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
	{
		return GetMethodImplCommon(name, -1, bindingAttr, binder, callConv, types, modifiers);
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
	protected override MethodInfo GetMethodImpl(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
	{
		return GetMethodImplCommon(name, genericParameterCount, bindingAttr, binder, callConv, types, modifiers);
	}

	private MethodInfo GetMethodImplCommon(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
	{
		ListBuilder<MethodInfo> methodCandidates = GetMethodCandidates(name, genericParameterCount, bindingAttr, callConv, types, allowPrefixLookup: false);
		if (methodCandidates.Count == 0)
		{
			return null;
		}
		MethodBase[] match;
		if (types == null || types.Length == 0)
		{
			MethodInfo methodInfo = methodCandidates[0];
			if (methodCandidates.Count == 1)
			{
				return methodInfo;
			}
			if (types == null)
			{
				for (int i = 1; i < methodCandidates.Count; i++)
				{
					if (!System.DefaultBinder.CompareMethodSig(methodCandidates[i], methodInfo))
					{
						throw ThrowHelper.GetAmbiguousMatchException(methodInfo);
					}
				}
				match = methodCandidates.ToArray();
				return System.DefaultBinder.FindMostDerivedNewSlotMeth(match, methodCandidates.Count) as MethodInfo;
			}
		}
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		Binder binder2 = binder;
		match = methodCandidates.ToArray();
		return binder2.SelectMethod(bindingAttr, match, types, modifiers) as MethodInfo;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		ListBuilder<ConstructorInfo> constructorCandidates = GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, types, allowPrefixLookup: false);
		if (constructorCandidates.Count == 0)
		{
			return null;
		}
		if (types.Length == 0 && constructorCandidates.Count == 1)
		{
			ConstructorInfo constructorInfo = constructorCandidates[0];
			if (constructorInfo.GetParametersAsSpan().IsEmpty)
			{
				return constructorInfo;
			}
		}
		MethodBase[] match;
		if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
		{
			match = constructorCandidates.ToArray();
			return System.DefaultBinder.ExactBinding(match, types) as ConstructorInfo;
		}
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		Binder binder2 = binder;
		match = constructorCandidates.ToArray();
		return binder2.SelectMethod(bindingAttr, match, types, modifiers) as ConstructorInfo;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ListBuilder<PropertyInfo> propertyCandidates = GetPropertyCandidates(name, bindingAttr, types, allowPrefixLookup: false);
		if (propertyCandidates.Count == 0)
		{
			return null;
		}
		if (types == null || types.Length == 0)
		{
			PropertyInfo propertyInfo = propertyCandidates[0];
			if (propertyCandidates.Count == 1)
			{
				if ((object)returnType != null && !returnType.IsEquivalentTo(propertyInfo.PropertyType))
				{
					return null;
				}
				return propertyInfo;
			}
			if ((object)returnType == null)
			{
				throw ThrowHelper.GetAmbiguousMatchException(propertyInfo);
			}
		}
		if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
		{
			return System.DefaultBinder.ExactPropertyBinding(propertyCandidates.ToArray(), returnType, types);
		}
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		return binder.SelectProperty(bindingAttr, propertyCandidates.ToArray(), returnType, types, modifiers);
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		FilterHelper(bindingAttr, ref name, out var _, out var listType);
		RuntimeEventInfo[] eventList = Cache.GetEventList(listType, name);
		EventInfo eventInfo = null;
		bindingAttr ^= BindingFlags.DeclaredOnly;
		foreach (RuntimeEventInfo runtimeEventInfo in eventList)
		{
			if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags)
			{
				if (eventInfo != null)
				{
					throw ThrowHelper.GetAmbiguousMatchException(eventInfo);
				}
				eventInfo = runtimeEventInfo;
			}
		}
		return eventInfo;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		FilterHelper(bindingAttr, ref name, out var _, out var listType);
		RuntimeFieldInfo[] fieldList = Cache.GetFieldList(listType, name);
		FieldInfo fieldInfo = null;
		bindingAttr ^= BindingFlags.DeclaredOnly;
		bool flag = false;
		foreach (RuntimeFieldInfo runtimeFieldInfo in fieldList)
		{
			if ((bindingAttr & runtimeFieldInfo.BindingFlags) != runtimeFieldInfo.BindingFlags)
			{
				continue;
			}
			if (fieldInfo != null)
			{
				if ((object)runtimeFieldInfo.DeclaringType == fieldInfo.DeclaringType)
				{
					throw ThrowHelper.GetAmbiguousMatchException(fieldInfo);
				}
				if (fieldInfo.DeclaringType.IsInterface && runtimeFieldInfo.DeclaringType.IsInterface)
				{
					flag = true;
				}
			}
			if (fieldInfo == null || runtimeFieldInfo.DeclaringType.IsSubclassOf(fieldInfo.DeclaringType) || fieldInfo.DeclaringType.IsInterface)
			{
				fieldInfo = runtimeFieldInfo;
			}
		}
		if (flag && fieldInfo.DeclaringType.IsInterface)
		{
			throw ThrowHelper.GetAmbiguousMatchException(fieldInfo);
		}
		return fieldInfo;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2063:UnrecognizedReflectionPattern", Justification = "Trimming makes sure that interfaces are fully preserved, so the Interfaces annotation is transitive.The cache doesn't carry the necessary annotation since it returns an array type,so the analysis complains that the returned value doesn't have the necessary annotation.")]
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
	public override Type GetInterface(string fullname, bool ignoreCase)
	{
		ArgumentNullException.ThrowIfNull(fullname, "fullname");
		BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
		bindingFlags &= ~BindingFlags.Static;
		if (ignoreCase)
		{
			bindingFlags |= BindingFlags.IgnoreCase;
		}
		SplitName(fullname, out var name, out var ns);
		FilterHelper(bindingFlags, ref name, out var _, out var listType);
		RuntimeType[] interfaceList = Cache.GetInterfaceList(listType, name);
		RuntimeType runtimeType = null;
		foreach (RuntimeType runtimeType2 in interfaceList)
		{
			if (FilterApplyType(runtimeType2, bindingFlags, name, prefixLookup: false, ns))
			{
				if (runtimeType != null)
				{
					throw ThrowHelper.GetAmbiguousMatchException(runtimeType);
				}
				runtimeType = runtimeType2;
			}
		}
		return runtimeType;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
	public override Type GetNestedType(string fullname, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(fullname, "fullname");
		bindingAttr &= ~BindingFlags.Static;
		SplitName(fullname, out var name, out var ns);
		FilterHelper(bindingAttr, ref name, out var _, out var listType);
		RuntimeType[] nestedTypeList = Cache.GetNestedTypeList(listType, name);
		RuntimeType runtimeType = null;
		foreach (RuntimeType runtimeType2 in nestedTypeList)
		{
			if (FilterApplyType(runtimeType2, bindingAttr, name, prefixLookup: false, ns))
			{
				if (runtimeType != null)
				{
					throw ThrowHelper.GetAmbiguousMatchException(runtimeType);
				}
				runtimeType = runtimeType2;
			}
		}
		return runtimeType;
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents)]
	public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ListBuilder<MethodInfo> listBuilder = default(ListBuilder<MethodInfo>);
		ListBuilder<ConstructorInfo> listBuilder2 = default(ListBuilder<ConstructorInfo>);
		ListBuilder<PropertyInfo> listBuilder3 = default(ListBuilder<PropertyInfo>);
		ListBuilder<EventInfo> listBuilder4 = default(ListBuilder<EventInfo>);
		ListBuilder<FieldInfo> listBuilder5 = default(ListBuilder<FieldInfo>);
		ListBuilder<Type> listBuilder6 = default(ListBuilder<Type>);
		int num = 0;
		if ((type & MemberTypes.Method) != 0)
		{
			listBuilder = GetMethodCandidates(name, -1, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: true);
			if (type == MemberTypes.Method)
			{
				return listBuilder.ToArray();
			}
			num += listBuilder.Count;
		}
		if ((type & MemberTypes.Constructor) != 0)
		{
			listBuilder2 = GetConstructorCandidates(name, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: true);
			if (type == MemberTypes.Constructor)
			{
				return listBuilder2.ToArray();
			}
			num += listBuilder2.Count;
		}
		if ((type & MemberTypes.Property) != 0)
		{
			listBuilder3 = GetPropertyCandidates(name, bindingAttr, null, allowPrefixLookup: true);
			if (type == MemberTypes.Property)
			{
				return listBuilder3.ToArray();
			}
			num += listBuilder3.Count;
		}
		if ((type & MemberTypes.Event) != 0)
		{
			listBuilder4 = GetEventCandidates(name, bindingAttr, allowPrefixLookup: true);
			if (type == MemberTypes.Event)
			{
				return listBuilder4.ToArray();
			}
			num += listBuilder4.Count;
		}
		if ((type & MemberTypes.Field) != 0)
		{
			listBuilder5 = GetFieldCandidates(name, bindingAttr, allowPrefixLookup: true);
			if (type == MemberTypes.Field)
			{
				return listBuilder5.ToArray();
			}
			num += listBuilder5.Count;
		}
		if ((type & (MemberTypes.TypeInfo | MemberTypes.NestedType)) != 0)
		{
			listBuilder6 = GetNestedTypeCandidates(name, bindingAttr, allowPrefixLookup: true);
			if (type == MemberTypes.NestedType || type == MemberTypes.TypeInfo)
			{
				return listBuilder6.ToArray();
			}
			num += listBuilder6.Count;
		}
		MemberInfo[] array;
		if (type != (MemberTypes.Constructor | MemberTypes.Method))
		{
			array = new MemberInfo[num];
		}
		else
		{
			MemberInfo[] array2 = new MethodBase[num];
			array = array2;
		}
		MemberInfo[] array3 = array;
		int num2 = 0;
		object[] array4 = array3;
		listBuilder.CopyTo(array4, num2);
		num2 += listBuilder.Count;
		array4 = array3;
		listBuilder2.CopyTo(array4, num2);
		num2 += listBuilder2.Count;
		array4 = array3;
		listBuilder3.CopyTo(array4, num2);
		num2 += listBuilder3.Count;
		array4 = array3;
		listBuilder4.CopyTo(array4, num2);
		num2 += listBuilder4.Count;
		array4 = array3;
		listBuilder5.CopyTo(array4, num2);
		num2 += listBuilder5.Count;
		array4 = array3;
		listBuilder6.CopyTo(array4, num2);
		num2 += listBuilder6.Count;
		return array3;
	}

	public override MemberInfo GetMemberWithSameMetadataDefinitionAs(MemberInfo member)
	{
		ArgumentNullException.ThrowIfNull(member, "member");
		RuntimeType runtimeType = this;
		while (runtimeType != null)
		{
			MemberInfo memberInfo = member.MemberType switch
			{
				MemberTypes.Method => GetMethodWithSameMetadataDefinitionAs(runtimeType, member), 
				MemberTypes.Constructor => GetConstructorWithSameMetadataDefinitionAs(runtimeType, member), 
				MemberTypes.Property => GetPropertyWithSameMetadataDefinitionAs(runtimeType, member), 
				MemberTypes.Field => GetFieldWithSameMetadataDefinitionAs(runtimeType, member), 
				MemberTypes.Event => GetEventWithSameMetadataDefinitionAs(runtimeType, member), 
				MemberTypes.NestedType => GetNestedTypeWithSameMetadataDefinitionAs(runtimeType, member), 
				_ => null, 
			};
			if (memberInfo != null)
			{
				return memberInfo;
			}
			runtimeType = runtimeType.GetBaseType();
		}
		throw Type.CreateGetMemberWithSameMetadataDefinitionAsNotFoundException(member);
	}

	private static RuntimeMethodInfo GetMethodWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo method)
	{
		RuntimeMethodInfo[] methodList = runtimeType.Cache.GetMethodList(MemberListType.CaseSensitive, method.Name);
		foreach (RuntimeMethodInfo runtimeMethodInfo in methodList)
		{
			if (runtimeMethodInfo.HasSameMetadataDefinitionAs(method))
			{
				return runtimeMethodInfo;
			}
		}
		return null;
	}

	private static RuntimeConstructorInfo GetConstructorWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo constructor)
	{
		RuntimeConstructorInfo[] constructorList = runtimeType.Cache.GetConstructorList(MemberListType.CaseSensitive, constructor.Name);
		foreach (RuntimeConstructorInfo runtimeConstructorInfo in constructorList)
		{
			if (runtimeConstructorInfo.HasSameMetadataDefinitionAs(constructor))
			{
				return runtimeConstructorInfo;
			}
		}
		return null;
	}

	private static RuntimePropertyInfo GetPropertyWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo property)
	{
		RuntimePropertyInfo[] propertyList = runtimeType.Cache.GetPropertyList(MemberListType.CaseSensitive, property.Name);
		foreach (RuntimePropertyInfo runtimePropertyInfo in propertyList)
		{
			if (runtimePropertyInfo.HasSameMetadataDefinitionAs(property))
			{
				return runtimePropertyInfo;
			}
		}
		return null;
	}

	private static RuntimeFieldInfo GetFieldWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo field)
	{
		RuntimeFieldInfo[] fieldList = runtimeType.Cache.GetFieldList(MemberListType.CaseSensitive, field.Name);
		foreach (RuntimeFieldInfo runtimeFieldInfo in fieldList)
		{
			if (runtimeFieldInfo.HasSameMetadataDefinitionAs(field))
			{
				return runtimeFieldInfo;
			}
		}
		return null;
	}

	private static RuntimeEventInfo GetEventWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo eventInfo)
	{
		RuntimeEventInfo[] eventList = runtimeType.Cache.GetEventList(MemberListType.CaseSensitive, eventInfo.Name);
		foreach (RuntimeEventInfo runtimeEventInfo in eventList)
		{
			if (runtimeEventInfo.HasSameMetadataDefinitionAs(eventInfo))
			{
				return runtimeEventInfo;
			}
		}
		return null;
	}

	private static RuntimeType GetNestedTypeWithSameMetadataDefinitionAs(RuntimeType runtimeType, MemberInfo nestedType)
	{
		RuntimeType[] nestedTypeList = runtimeType.Cache.GetNestedTypeList(MemberListType.CaseSensitive, nestedType.Name);
		foreach (RuntimeType runtimeType2 in nestedTypeList)
		{
			if (runtimeType2.HasSameMetadataDefinitionAs(nestedType))
			{
				return runtimeType2;
			}
		}
		return null;
	}

	public override bool IsSubclassOf(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		RuntimeType runtimeType = type as RuntimeType;
		if (runtimeType == null)
		{
			return false;
		}
		RuntimeType baseType = GetBaseType();
		while (baseType != null)
		{
			if (baseType == runtimeType)
			{
				return true;
			}
			baseType = baseType.GetBaseType();
		}
		if (runtimeType == ObjectType && runtimeType != this)
		{
			return true;
		}
		return false;
	}

	public unsafe override bool IsInstanceOfType([NotNullWhen(true)] object o)
	{
		bool result = CastHelpers.IsInstanceOfAny(((IntPtr)GetUnderlyingNativeHandle()).ToPointer(), o) != null;
		GC.KeepAlive(this);
		return result;
	}

	public override bool IsEquivalentTo([NotNullWhen(true)] Type other)
	{
		if (!(other is RuntimeType runtimeType))
		{
			return false;
		}
		if (runtimeType == this)
		{
			return true;
		}
		return RuntimeTypeHandle.IsEquivalentTo(this, runtimeType);
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_GetGuid", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_GetGuid")]
	private unsafe static extern void GetGuid(MethodTable* pMT, Guid* result);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private unsafe static void GetComObjectGuidWorker(RuntimeType type, Guid* result)
	{
		GetComObjectGuid(ObjectHandleOnStack.Create(ref type), result);
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_GetComObjectGuid", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_GetComObjectGuid")]
	private unsafe static extern void GetComObjectGuid(ObjectHandleOnStack type, Guid* result);

	protected unsafe override bool IsValueTypeImpl()
	{
		TypeHandle nativeTypeHandle = GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			return IsSubclassOf(typeof(ValueType));
		}
		bool isValueType = nativeTypeHandle.AsMethodTable()->IsValueType;
		GC.KeepAlive(this);
		return isValueType;
	}

	internal unsafe bool IsDelegate()
	{
		TypeHandle nativeTypeHandle = GetNativeTypeHandle();
		bool result = !nativeTypeHandle.IsTypeDesc && nativeTypeHandle.AsMethodTable()->ParentMethodTable == System.Runtime.CompilerServices.TypeHandle.TypeHandleOf<MulticastDelegate>().AsMethodTable();
		GC.KeepAlive(this);
		return result;
	}

	public override Type GetGenericTypeDefinition()
	{
		if (!IsGenericType)
		{
			throw new InvalidOperationException(SR.InvalidOperation_NotGenericType);
		}
		return Cache.GetGenericTypeDefinition();
	}

	internal object[] GetEmptyArray()
	{
		return Cache.GetEmptyArray();
	}

	internal RuntimeType[] GetGenericArgumentsInternal()
	{
		return GetRootElementType().TypeHandle.GetInstantiationInternal();
	}

	public override Type[] GetGenericArguments()
	{
		return GetRootElementType().TypeHandle.GetInstantiationPublic() ?? Type.EmptyTypes;
	}

	[RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
	public override Type MakeGenericType(params Type[] typeArguments)
	{
		ArgumentNullException.ThrowIfNull(typeArguments, "typeArguments");
		if (!IsGenericTypeDefinition)
		{
			throw new InvalidOperationException(SR.Format(SR.Arg_NotGenericTypeDefinition, this));
		}
		RuntimeType[] genericArgumentsInternal = GetGenericArgumentsInternal();
		if (genericArgumentsInternal.Length != typeArguments.Length)
		{
			throw new ArgumentException(SR.Argument_GenericArgsCount, "typeArguments");
		}
		if (typeArguments.Length == 1 && typeArguments[0] is RuntimeType runtimeType)
		{
			ThrowIfTypeNeverValidGenericArgument(runtimeType);
			try
			{
				return new RuntimeTypeHandle(this).Instantiate(runtimeType);
			}
			catch (TypeLoadException e)
			{
				ValidateGenericArguments(this, new RuntimeType[1] { runtimeType }, e);
				throw;
			}
		}
		RuntimeType[] array = new RuntimeType[typeArguments.Length];
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < typeArguments.Length; i++)
		{
			Type type = typeArguments[i] ?? throw new ArgumentNullException();
			RuntimeType runtimeType2 = type as RuntimeType;
			if (runtimeType2 == null)
			{
				flag2 = true;
				if (type.IsSignatureType)
				{
					flag = true;
				}
			}
			array[i] = runtimeType2;
		}
		if (flag2)
		{
			if (flag)
			{
				return new SignatureConstructedGenericType(this, typeArguments);
			}
			return TypeBuilderInstantiation.MakeGenericType(this, (Type[])typeArguments.Clone());
		}
		SanityCheckGenericArguments(array, genericArgumentsInternal);
		try
		{
			RuntimeTypeHandle runtimeTypeHandle = new RuntimeTypeHandle(this);
			Type[] inst = array;
			return runtimeTypeHandle.Instantiate(inst);
		}
		catch (TypeLoadException e2)
		{
			ValidateGenericArguments(this, array, e2);
			throw;
		}
	}

	public override Type[] GetGenericParameterConstraints()
	{
		if (!IsGenericParameter)
		{
			throw new InvalidOperationException(SR.Arg_NotGenericParameter);
		}
		return new RuntimeTypeHandle(this).GetConstraints() ?? Type.EmptyTypes;
	}

	internal CorElementType GetCorElementType()
	{
		byte result = (byte)GetNativeTypeHandle().GetCorElementType();
		GC.KeepAlive(this);
		return (CorElementType)result;
	}

	public sealed override bool HasSameMetadataDefinitionAs(MemberInfo other)
	{
		return HasSameMetadataDefinitionAsCore<RuntimeType>(other);
	}

	public override Type MakePointerType()
	{
		return new RuntimeTypeHandle(this).MakePointer();
	}

	public override Type MakeByRefType()
	{
		return new RuntimeTypeHandle(this).MakeByRef();
	}

	public override Type MakeArrayType()
	{
		return new RuntimeTypeHandle(this).MakeSZArray();
	}

	public override Type MakeArrayType(int rank)
	{
		if (rank <= 0)
		{
			throw new IndexOutOfRangeException();
		}
		return new RuntimeTypeHandle(this).MakeArray(rank);
	}

	private unsafe static bool CanValueSpecialCast(RuntimeType valueType, RuntimeType targetType)
	{
		if (targetType.IsPointer || targetType.IsFunctionPointer)
		{
			if (valueType == typeof(nint))
			{
				return true;
			}
			if (targetType == typeof(void*))
			{
				return true;
			}
			return valueType.IsAssignableTo(targetType);
		}
		CorElementType underlyingCorElementType = valueType.GetUnderlyingCorElementType();
		CorElementType underlyingCorElementType2 = targetType.GetUnderlyingCorElementType();
		if (underlyingCorElementType.IsPrimitiveType())
		{
			return RuntimeHelpers.CanPrimitiveWiden(underlyingCorElementType, underlyingCorElementType2);
		}
		return false;
	}

	private CheckValueStatus TryChangeTypeSpecial(ref object value)
	{
		Pointer pointer = value as Pointer;
		RuntimeType runtimeType = ((pointer != null) ? pointer.GetPointerType() : ((RuntimeType)value.GetType()));
		if (!CanValueSpecialCast(runtimeType, this))
		{
			return CheckValueStatus.ArgumentException;
		}
		if (pointer != null)
		{
			value = pointer.GetPointerValue();
		}
		else
		{
			CorElementType underlyingCorElementType = runtimeType.GetUnderlyingCorElementType();
			CorElementType underlyingCorElementType2 = GetUnderlyingCorElementType();
			if (underlyingCorElementType2 != underlyingCorElementType)
			{
				value = InvokeUtils.ConvertOrWiden(runtimeType, value, this, underlyingCorElementType2);
			}
		}
		return CheckValueStatus.Success;
	}

	public override Type[] GetFunctionPointerCallingConventions()
	{
		if (!IsFunctionPointer)
		{
			throw new InvalidOperationException(SR.InvalidOperation_NotFunctionPointer);
		}
		return Type.EmptyTypes;
	}

	public override Type[] GetFunctionPointerParameterTypes()
	{
		if (!IsFunctionPointer)
		{
			throw new InvalidOperationException(SR.InvalidOperation_NotFunctionPointer);
		}
		Type[] functionPointerReturnAndParameterTypes = Cache.FunctionPointerReturnAndParameterTypes;
		if (functionPointerReturnAndParameterTypes.Length == 1)
		{
			return Type.EmptyTypes;
		}
		return functionPointerReturnAndParameterTypes.AsSpan(1).ToArray();
	}

	public override Type GetFunctionPointerReturnType()
	{
		if (!IsFunctionPointer)
		{
			throw new InvalidOperationException(SR.InvalidOperation_NotFunctionPointer);
		}
		return Cache.FunctionPointerReturnAndParameterTypes[0];
	}

	public override string ToString()
	{
		return Cache.GetToString();
	}

	private void CreateInstanceCheckThis()
	{
		if (ContainsGenericParameters)
		{
			throw new ArgumentException(SR.Format(SR.Acc_CreateGenericEx, this));
		}
		Type rootElementType = GetRootElementType();
		if ((object)rootElementType == typeof(ArgIterator))
		{
			throw new NotSupportedException(SR.Acc_CreateArgIterator);
		}
		if ((object)rootElementType == typeof(void))
		{
			throw new NotSupportedException(SR.Acc_CreateVoid);
		}
	}

	internal object CreateInstanceImpl(BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture)
	{
		CreateInstanceCheckThis();
		if (args == null)
		{
			args = Array.Empty<object>();
		}
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		bool publicOnly = (bindingAttr & BindingFlags.NonPublic) == 0;
		bool wrapExceptions = (bindingAttr & BindingFlags.DoNotWrapExceptions) == 0;
		object result;
		if (args.Length == 0 && (bindingAttr & BindingFlags.Public) != BindingFlags.Default && (bindingAttr & BindingFlags.Instance) != BindingFlags.Default && (IsGenericCOMObjectImpl() || base.IsValueType))
		{
			result = CreateInstanceDefaultCtor(publicOnly, wrapExceptions);
		}
		else
		{
			ListBuilder<ConstructorInfo> constructorCandidates = GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, allowPrefixLookup: false);
			MethodBase[] array = new MethodBase[constructorCandidates.Count];
			int num = 0;
			Type[] array2 = ((args.Length != 0) ? new Type[args.Length] : Type.EmptyTypes);
			for (int i = 0; i < args.Length; i++)
			{
				object obj = args[i];
				if (obj != null)
				{
					array2[i] = obj.GetType();
				}
			}
			for (int j = 0; j < constructorCandidates.Count; j++)
			{
				if (FilterApplyConstructorInfo((RuntimeConstructorInfo)constructorCandidates[j], bindingAttr, CallingConventions.Any, array2))
				{
					array[num++] = constructorCandidates[j];
				}
			}
			if (num == 0)
			{
				throw new MissingMethodException(SR.Format(SR.MissingConstructor_Name, FullName));
			}
			if (num != array.Length)
			{
				Array.Resize(ref array, num);
			}
			MethodBase methodBase;
			object state;
			try
			{
				methodBase = binder.BindToMethod(bindingAttr, array, ref args, null, culture, null, out state);
			}
			catch (MissingMethodException inner)
			{
				throw new MissingMethodException(SR.Format(SR.MissingConstructor_Name, FullName), inner);
			}
			if ((object)methodBase == null)
			{
				throw new MissingMethodException(SR.Format(SR.MissingConstructor_Name, FullName));
			}
			if (methodBase.GetParametersAsSpan().Length == 0)
			{
				if (args.Length != 0)
				{
					throw new NotSupportedException(SR.NotSupported_CallToVarArg);
				}
				result = CreateInstanceLocal(wrapExceptions);
			}
			else
			{
				result = ((ConstructorInfo)methodBase).Invoke(bindingAttr, binder, args, culture);
				if (state != null)
				{
					binder.ReorderArgumentArray(ref args, state);
				}
			}
		}
		return result;
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2082:UnrecognizedReflectionPattern", Justification = "Implementation detail of Activator that linker intrinsically recognizes")]
		object CreateInstanceLocal(bool wrapExceptions2)
		{
			return Activator.CreateInstance(this, nonPublic: true, wrapExceptions2);
		}
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	internal object GetUninitializedObject()
	{
		return GetOrCreateCacheEntry<CreateUninitializedCache>().CreateUninitializedObject(this);
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	internal object CreateInstanceDefaultCtor(bool publicOnly, bool wrapExceptions)
	{
		ActivatorCache orCreateCacheEntry = GetOrCreateCacheEntry<ActivatorCache>();
		if (!orCreateCacheEntry.CtorIsPublic & publicOnly)
		{
			throw new MissingMethodException(SR.Format(SR.Arg_NoDefCTor, this));
		}
		if (IsByRefLike)
		{
			throw new NotSupportedException(SR.NotSupported_ByRefLike);
		}
		object obj = orCreateCacheEntry.CreateUninitializedObject(this);
		try
		{
			orCreateCacheEntry.CallRefConstructor(obj);
			return obj;
		}
		catch (Exception inner) when (wrapExceptions)
		{
			throw new TargetInvocationException(inner);
		}
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	internal object CreateInstanceOfT()
	{
		ActivatorCache orCreateCacheEntry = GetOrCreateCacheEntry<ActivatorCache>();
		if (!orCreateCacheEntry.CtorIsPublic)
		{
			throw new MissingMethodException(SR.Format(SR.Arg_NoDefCTor, this));
		}
		object obj = orCreateCacheEntry.CreateUninitializedObject(this);
		try
		{
			orCreateCacheEntry.CallRefConstructor(obj);
			return obj;
		}
		catch (Exception inner)
		{
			throw new TargetInvocationException(inner);
		}
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	internal void CallDefaultStructConstructor(ref byte data)
	{
		ActivatorCache orCreateCacheEntry = GetOrCreateCacheEntry<ActivatorCache>();
		if (!orCreateCacheEntry.CtorIsPublic)
		{
			throw new MissingMethodException(SR.Format(SR.Arg_NoDefCTor, this));
		}
		try
		{
			orCreateCacheEntry.CallValueConstructor(ref data);
		}
		catch (Exception inner)
		{
			throw new TargetInvocationException(inner);
		}
	}

	internal void InvalidateCachedNestedType()
	{
		Cache.InvalidateCachedNestedType();
	}

	protected override bool IsCOMObjectImpl()
	{
		return RuntimeTypeHandle.CanCastTo(this, (RuntimeType)typeof(__ComObject));
	}

	internal bool IsGenericCOMObjectImpl()
	{
		return TypeHandle.Value == typeof(__ComObject).TypeHandle.Value;
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_InvokeDispMethod", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_InvokeDispMethod")]
	private static extern void InvokeDispMethod(ObjectHandleOnStack type, ObjectHandleOnStack name, BindingFlags invokeAttr, ObjectHandleOnStack target, ObjectHandleOnStack args, ObjectHandleOnStack byrefModifiers, int lcid, ObjectHandleOnStack namedParameters, ObjectHandleOnStack result);

	private object InvokeDispMethod(string name, BindingFlags invokeAttr, object target, object[] args, bool[] byrefModifiers, int culture, string[] namedParameters)
	{
		RuntimeType o = this;
		object o2 = null;
		InvokeDispMethod(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref name), invokeAttr, ObjectHandleOnStack.Create(ref target), ObjectHandleOnStack.Create(ref args), ObjectHandleOnStack.Create(ref byrefModifiers), culture, ObjectHandleOnStack.Create(ref namedParameters), ObjectHandleOnStack.Create(ref o2));
		return o2;
	}

	[RequiresUnreferencedCode("The member might be removed")]
	private object ForwardCallToInvokeMember(string memberName, BindingFlags flags, object target, object[] aArgs, bool[] aArgsIsByRef, int[] aArgsWrapperTypes, Type[] aArgsTypes, Type retType)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		int num = aArgs.Length;
		ParameterModifier[] array = null;
		if (num > 0)
		{
			ParameterModifier parameterModifier = new ParameterModifier(num);
			for (int i = 0; i < num; i++)
			{
				parameterModifier[i] = aArgsIsByRef[i];
			}
			array = new ParameterModifier[1] { parameterModifier };
			if (aArgsWrapperTypes != null)
			{
				WrapArgsForInvokeCall(aArgs, aArgsWrapperTypes);
			}
		}
		flags |= BindingFlags.DoNotWrapExceptions;
		object obj = InvokeMember(memberName, flags, null, target, aArgs, array, null, null);
		for (int j = 0; j < num; j++)
		{
			if (array[0][j] && aArgs[j] != null)
			{
				Type type = aArgsTypes[j];
				if ((object)type != aArgs[j].GetType())
				{
					aArgs[j] = ForwardCallBinder.ChangeType(aArgs[j], type, null);
				}
			}
		}
		if (obj != null && (object)retType != obj.GetType())
		{
			obj = ForwardCallBinder.ChangeType(obj, retType, null);
		}
		return obj;
	}

	private static void WrapArgsForInvokeCall(object[] aArgs, int[] aArgsWrapperTypes)
	{
		int num = aArgs.Length;
		for (int i = 0; i < num; i++)
		{
			if (aArgsWrapperTypes[i] == 0)
			{
				continue;
			}
			if (((DispatchWrapperType)aArgsWrapperTypes[i]).HasFlag(DispatchWrapperType.SafeArray))
			{
				Type type = null;
				bool flag = false;
				switch ((DispatchWrapperType)(aArgsWrapperTypes[i] & -65537))
				{
				case DispatchWrapperType.Unknown:
					type = typeof(UnknownWrapper);
					break;
				case DispatchWrapperType.Dispatch:
					type = typeof(DispatchWrapper);
					break;
				case DispatchWrapperType.Error:
					type = typeof(ErrorWrapper);
					break;
				case DispatchWrapperType.Currency:
					type = typeof(CurrencyWrapper);
					break;
				case DispatchWrapperType.BStr:
					type = typeof(BStrWrapper);
					flag = true;
					break;
				}
				Array array = (Array)aArgs[i];
				int length = array.Length;
				object[] array2 = (object[])Array.CreateInstance(type, length);
				ConstructorInfo constructorInfo = ((!flag) ? type.GetConstructor(new Type[1] { typeof(object) }) : type.GetConstructor(new Type[1] { typeof(string) }));
				for (int j = 0; j < length; j++)
				{
					if (flag)
					{
						array2[j] = constructorInfo.Invoke(new object[1] { (string)array.GetValue(j) });
					}
					else
					{
						array2[j] = constructorInfo.Invoke(new object[1] { array.GetValue(j) });
					}
				}
				aArgs[i] = array2;
			}
			else
			{
				switch ((DispatchWrapperType)aArgsWrapperTypes[i])
				{
				case DispatchWrapperType.Unknown:
					aArgs[i] = new UnknownWrapper(aArgs[i]);
					break;
				case DispatchWrapperType.Dispatch:
					aArgs[i] = new DispatchWrapper(aArgs[i]);
					break;
				case DispatchWrapperType.Error:
					aArgs[i] = new ErrorWrapper(aArgs[i]);
					break;
				case DispatchWrapperType.Currency:
					aArgs[i] = new CurrencyWrapper(aArgs[i]);
					break;
				case DispatchWrapperType.BStr:
					aArgs[i] = new BStrWrapper((string)aArgs[i]);
					break;
				}
			}
		}
	}

	internal object Box(ref byte data)
	{
		return GetOrCreateCacheEntry<BoxCache>().Box(this, ref data);
	}

	public object Clone()
	{
		return this;
	}

	public override bool Equals(object obj)
	{
		return obj == this;
	}

	public override int GetArrayRank()
	{
		if (!IsArrayImpl())
		{
			throw new ArgumentException(SR.Argument_HasToBeArrayClass);
		}
		return RuntimeTypeHandle.GetArrayRank(this);
	}

	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return RuntimeTypeHandle.GetAttributes(this);
	}

	public override object[] GetCustomAttributes(bool inherit)
	{
		return CustomAttribute.GetCustomAttributes(this, ObjectType, inherit);
	}

	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		ArgumentNullException.ThrowIfNull(attributeType, "attributeType");
		if (!(attributeType.UnderlyingSystemType is RuntimeType caType))
		{
			throw new ArgumentException(SR.Arg_MustBeType, "attributeType");
		}
		return CustomAttribute.GetCustomAttributes(this, caType, inherit);
	}

	public override IList<CustomAttributeData> GetCustomAttributesData()
	{
		return RuntimeCustomAttributeData.GetCustomAttributesInternal(this);
	}

	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents)]
	public override MemberInfo[] GetDefaultMembers()
	{
		MemberInfo[] array = null;
		string defaultMemberName = GetDefaultMemberName();
		if (defaultMemberName != null)
		{
			array = GetMember(defaultMemberName);
		}
		return array ?? Array.Empty<MemberInfo>();
	}

	private static bool IsFullNameRoundtripCompatible(RuntimeType runtimeType)
	{
		Type rootElementType = runtimeType.GetRootElementType();
		if (!rootElementType.IsGenericTypeDefinition && rootElementType.ContainsGenericParameters)
		{
			return false;
		}
		if (rootElementType.IsFunctionPointer)
		{
			return false;
		}
		return true;
	}

	public override Type GetElementType()
	{
		return RuntimeTypeHandle.GetElementType(this);
	}

	public override string GetEnumName(object value)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		RuntimeType runtimeType = (RuntimeType)value.GetType();
		if (!runtimeType.IsActualEnum && !Type.IsIntegerType(runtimeType))
		{
			throw new ArgumentException(SR.Arg_MustBeEnumBaseTypeOrEnum, "value");
		}
		return Enum.GetName(this, Enum.ToUInt64(value));
	}

	private static void ThrowMustBeEnum()
	{
		throw new ArgumentException(SR.Arg_MustBeEnum, "enumType");
	}

	public override string[] GetEnumNames()
	{
		if (!IsActualEnum)
		{
			ThrowMustBeEnum();
		}
		return new ReadOnlySpan<string>(Enum.GetNamesNoCopy(this)).ToArray();
	}

	[RequiresDynamicCode("It might not be possible to create an array of the enum type at runtime. Use Enum.GetValues<T> or the GetEnumValuesAsUnderlyingType method instead.")]
	public override Array GetEnumValues()
	{
		if (!IsActualEnum)
		{
			ThrowMustBeEnum();
		}
		Array valuesAsUnderlyingTypeNoCopy = Enum.GetValuesAsUnderlyingTypeNoCopy(this);
		Array array = Array.CreateInstance(this, valuesAsUnderlyingTypeNoCopy.Length);
		Array.Copy(valuesAsUnderlyingTypeNoCopy, array, valuesAsUnderlyingTypeNoCopy.Length);
		return array;
	}

	public override Array GetEnumValuesAsUnderlyingType()
	{
		if (!IsActualEnum)
		{
			ThrowMustBeEnum();
		}
		return Enum.GetValuesAsUnderlyingType(this);
	}

	public override Type GetEnumUnderlyingType()
	{
		if (!IsActualEnum)
		{
			ThrowMustBeEnum();
		}
		return Enum.InternalGetUnderlyingType(this);
	}

	public override int GetHashCode()
	{
		return RuntimeHelpers.GetHashCode(this);
	}

	internal RuntimeModule GetRuntimeModule()
	{
		return RuntimeTypeHandle.GetModule(this);
	}

	protected override TypeCode GetTypeCodeImpl()
	{
		TypeCode typeCode = Cache.TypeCode;
		if (typeCode != TypeCode.Empty)
		{
			return typeCode;
		}
		typeCode = Type.GetRuntimeTypeCode(this);
		Cache.TypeCode = typeCode;
		return typeCode;
	}

	protected override bool HasElementTypeImpl()
	{
		return RuntimeTypeHandle.HasElementType(this);
	}

	protected override bool IsArrayImpl()
	{
		return RuntimeTypeHandle.IsArray(this);
	}

	protected override bool IsContextfulImpl()
	{
		return false;
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		ArgumentNullException.ThrowIfNull(attributeType, "attributeType");
		if (!(attributeType.UnderlyingSystemType is RuntimeType caType))
		{
			throw new ArgumentException(SR.Arg_MustBeType, "attributeType");
		}
		return CustomAttribute.IsDefined(this, caType, inherit);
	}

	public override bool IsEnumDefined(object value)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		if (!IsActualEnum)
		{
			ThrowMustBeEnum();
		}
		RuntimeType runtimeType = (RuntimeType)value.GetType();
		if (runtimeType.IsActualEnum)
		{
			if (!runtimeType.IsEquivalentTo(this))
			{
				throw new ArgumentException(SR.Format(SR.Arg_EnumAndObjectMustBeSameType, runtimeType, this));
			}
			runtimeType = (RuntimeType)runtimeType.GetEnumUnderlyingType();
		}
		if (runtimeType == StringType)
		{
			return Array.IndexOf(Enum.GetNamesNoCopy(this), (string)value) >= 0;
		}
		if (!Type.IsIntegerType(runtimeType))
		{
			throw new InvalidOperationException(SR.InvalidOperation_UnknownEnumType);
		}
		RuntimeType runtimeType2 = Enum.InternalGetUnderlyingType(this);
		if (runtimeType2 != runtimeType)
		{
			throw new ArgumentException(SR.Format(SR.Arg_EnumUnderlyingTypeAndObjectMustBeSameType, runtimeType, runtimeType2));
		}
		switch (Type.GetTypeCode(runtimeType2))
		{
		case TypeCode.SByte:
			return Enum.IsDefinedPrimitive(this, (byte)(sbyte)value);
		case TypeCode.Byte:
			return Enum.IsDefinedPrimitive(this, (byte)value);
		case TypeCode.Int16:
			return Enum.IsDefinedPrimitive(this, (ushort)(short)value);
		case TypeCode.UInt16:
			return Enum.IsDefinedPrimitive(this, (ushort)value);
		case TypeCode.Int32:
			return Enum.IsDefinedPrimitive(this, (uint)(int)value);
		case TypeCode.UInt32:
			return Enum.IsDefinedPrimitive(this, (uint)value);
		case TypeCode.Int64:
			return Enum.IsDefinedPrimitive(this, (ulong)(long)value);
		case TypeCode.UInt64:
			return Enum.IsDefinedPrimitive(this, (ulong)value);
		case TypeCode.Single:
			return Enum.IsDefinedPrimitive(this, (float)value);
		case TypeCode.Double:
			return Enum.IsDefinedPrimitive(this, (double)value);
		case TypeCode.Char:
			return Enum.IsDefinedPrimitive(this, (char)value);
		default:
		{
			bool result;
			if (!(runtimeType2 == typeof(nint)))
			{
				if (!(runtimeType2 == typeof(nuint)))
				{
					throw new InvalidOperationException(SR.InvalidOperation_UnknownEnumType);
				}
				result = Enum.IsDefinedPrimitive<nuint>(this, (nuint)value);
			}
			else
			{
				result = Enum.IsDefinedPrimitive<nuint>(this, (nuint)(nint)value);
			}
			return result;
		}
		}
	}

	protected override bool IsByRefImpl()
	{
		return RuntimeTypeHandle.IsByRef(this);
	}

	protected override bool IsPrimitiveImpl()
	{
		return RuntimeTypeHandle.IsPrimitive(this);
	}

	protected override bool IsPointerImpl()
	{
		return RuntimeTypeHandle.IsPointer(this);
	}

	public override bool IsAssignableFrom([NotNullWhen(true)] TypeInfo typeInfo)
	{
		if (typeInfo != null)
		{
			return IsAssignableFrom(typeInfo.AsType());
		}
		return false;
	}

	public override bool IsAssignableFrom([NotNullWhen(true)] Type c)
	{
		if ((object)c == null)
		{
			return false;
		}
		if ((object)c == this)
		{
			return true;
		}
		if (c.UnderlyingSystemType is RuntimeType type)
		{
			return RuntimeTypeHandle.CanCastTo(type, this);
		}
		if (c is TypeBuilder)
		{
			if (c.IsSubclassOf(this))
			{
				return true;
			}
			if (IsActualInterface)
			{
				return c.ImplementInterface(this);
			}
			if (IsGenericParameter)
			{
				Type[] genericParameterConstraints = GetGenericParameterConstraints();
				for (int i = 0; i < genericParameterConstraints.Length; i++)
				{
					if (!genericParameterConstraints[i].IsAssignableFrom(c))
					{
						return false;
					}
				}
				return true;
			}
		}
		return false;
	}

	[DebuggerStepThrough]
	[DebuggerHidden]
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
	public override object InvokeMember(string name, BindingFlags bindingFlags, Binder binder, object target, object[] providedArgs, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParams)
	{
		if (IsGenericParameter)
		{
			throw new InvalidOperationException(SR.Arg_GenericParameter);
		}
		if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == 0)
		{
			throw new ArgumentException(SR.Arg_NoAccessSpec, "bindingFlags");
		}
		if ((bindingFlags & (BindingFlags)0xFF) == 0)
		{
			bindingFlags |= BindingFlags.Instance | BindingFlags.Public;
			if ((bindingFlags & BindingFlags.CreateInstance) == 0)
			{
				bindingFlags |= BindingFlags.Static;
			}
		}
		if (namedParams != null)
		{
			if (providedArgs != null)
			{
				if (namedParams.Length > providedArgs.Length)
				{
					throw new ArgumentException(SR.Arg_NamedParamTooBig, "namedParams");
				}
			}
			else if (namedParams.Length != 0)
			{
				throw new ArgumentException(SR.Arg_NamedParamTooBig, "namedParams");
			}
		}
		if (target != null && target.GetType().IsCOMObject)
		{
			if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == 0)
			{
				throw new ArgumentException(SR.Arg_COMAccess, "bindingFlags");
			}
			if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_PropSetGet, "bindingFlags");
			}
			if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_PropSetInvoke, "bindingFlags");
			}
			if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.SetProperty) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_COMPropSetPut, "bindingFlags");
			}
			if ((bindingFlags & BindingFlags.PutDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutDispProperty) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_COMPropSetPut, "bindingFlags");
			}
			if ((bindingFlags & BindingFlags.PutRefDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutRefDispProperty) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_COMPropSetPut, "bindingFlags");
			}
			ArgumentNullException.ThrowIfNull(name, "name");
			bool[] byrefModifiers = modifiers?[0].IsByRefArray;
			int culture2 = culture?.LCID ?? 1033;
			bool flag = (bindingFlags & BindingFlags.DoNotWrapExceptions) != 0;
			try
			{
				return InvokeDispMethod(name, bindingFlags, target, providedArgs, byrefModifiers, culture2, namedParams);
			}
			catch (TargetInvocationException ex) when (flag)
			{
				throw ex.InnerException;
			}
		}
		if (namedParams != null && Array.IndexOf(namedParams, null) >= 0)
		{
			throw new ArgumentException(SR.Arg_NamedParamNull, "namedParams");
		}
		int num = ((providedArgs != null) ? providedArgs.Length : 0);
		if (binder == null)
		{
			binder = Type.DefaultBinder;
		}
		if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default)
		{
			if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty)) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_CreatInstAccess, "bindingFlags");
			}
			return Activator.CreateInstance(this, bindingFlags, binder, providedArgs, culture);
		}
		if ((bindingFlags & (BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) != BindingFlags.Default)
		{
			bindingFlags |= BindingFlags.SetProperty;
		}
		ArgumentNullException.ThrowIfNull(name, "name");
		if (name.Length == 0 || name.Equals("[DISPID=0]"))
		{
			name = GetDefaultMemberName() ?? "ToString";
		}
		bool flag2 = (bindingFlags & BindingFlags.GetField) != 0;
		bool flag3 = (bindingFlags & BindingFlags.SetField) != 0;
		if (flag2 | flag3)
		{
			if (flag2)
			{
				if (flag3)
				{
					throw new ArgumentException(SR.Arg_FldSetGet, "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(SR.Arg_FldGetPropSet, "bindingFlags");
				}
			}
			else
			{
				ArgumentNullException.ThrowIfNull(providedArgs, "providedArgs");
				if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(SR.Arg_FldSetPropGet, "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
				{
					throw new ArgumentException(SR.Arg_FldSetInvoke, "bindingFlags");
				}
			}
			FieldInfo fieldInfo = null;
			FieldInfo[] array = GetFields(this, name, bindingFlags);
			if (array.Length == 1)
			{
				fieldInfo = array[0];
			}
			else if (array.Length != 0)
			{
				fieldInfo = binder.BindToField(bindingFlags, array, flag2 ? Empty.Value : providedArgs[0], culture);
			}
			if (fieldInfo != null)
			{
				if (fieldInfo.FieldType.IsArray || (object)fieldInfo.FieldType == typeof(Array))
				{
					int num2 = (((bindingFlags & BindingFlags.GetField) == 0) ? (num - 1) : num);
					if (num2 > 0)
					{
						int[] array2 = new int[num2];
						for (int i = 0; i < num2; i++)
						{
							try
							{
								array2[i] = ((IConvertible)providedArgs[i]).ToInt32(null);
							}
							catch (InvalidCastException)
							{
								throw new ArgumentException(SR.Arg_IndexMustBeInt);
							}
						}
						Array array3 = (Array)fieldInfo.GetValue(target);
						if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
						{
							return array3.GetValue(array2);
						}
						array3.SetValue(providedArgs[num2], array2);
						return null;
					}
				}
				if (flag2)
				{
					if (num != 0)
					{
						throw new ArgumentException(SR.Arg_FldGetArgErr, "bindingFlags");
					}
					return fieldInfo.GetValue(target);
				}
				if (num != 1)
				{
					throw new ArgumentException(SR.Arg_FldSetArgErr, "bindingFlags");
				}
				fieldInfo.SetValue(target, providedArgs[0], bindingFlags, binder, culture);
				return null;
			}
			if ((bindingFlags & (BindingFlags)0xFFF300) == 0)
			{
				throw new MissingFieldException(FullName, name);
			}
		}
		bool flag4 = (bindingFlags & BindingFlags.GetProperty) != 0;
		bool flag5 = (bindingFlags & BindingFlags.SetProperty) != 0;
		if (flag4 | flag5)
		{
			if (flag4)
			{
				if (flag5)
				{
					throw new ArgumentException(SR.Arg_PropSetGet, "bindingFlags");
				}
			}
			else if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
			{
				throw new ArgumentException(SR.Arg_PropSetInvoke, "bindingFlags");
			}
		}
		MethodInfo[] array4 = null;
		MethodInfo methodInfo = null;
		if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
		{
			MethodInfo[] array5 = GetMethods(this, name, bindingFlags);
			List<MethodInfo> list = null;
			foreach (MethodInfo methodInfo2 in array5)
			{
				if (!FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo2, bindingFlags, CallingConventions.Any, new Type[num]))
				{
					continue;
				}
				if (methodInfo == null)
				{
					methodInfo = methodInfo2;
					continue;
				}
				if (list == null)
				{
					list = new List<MethodInfo>(array5.Length) { methodInfo };
				}
				list.Add(methodInfo2);
			}
			if (list != null)
			{
				array4 = list.ToArray();
			}
		}
		if (((methodInfo == null) & flag4) | flag5)
		{
			PropertyInfo[] array6 = GetProperties(this, name, bindingFlags);
			List<MethodInfo> list2 = null;
			for (int k = 0; k < array6.Length; k++)
			{
				MethodInfo methodInfo3 = null;
				methodInfo3 = ((!flag5) ? array6[k].GetGetMethod(nonPublic: true) : array6[k].GetSetMethod(nonPublic: true));
				if (methodInfo3 == null || !FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo3, bindingFlags, CallingConventions.Any, new Type[num]))
				{
					continue;
				}
				if (methodInfo == null)
				{
					methodInfo = methodInfo3;
					continue;
				}
				if (list2 == null)
				{
					list2 = new List<MethodInfo>(array6.Length) { methodInfo };
				}
				list2.Add(methodInfo3);
			}
			if (list2 != null)
			{
				array4 = list2.ToArray();
			}
		}
		if (methodInfo != null)
		{
			if (array4 == null && num == 0 && methodInfo.GetParametersAsSpan().Length == 0 && (bindingFlags & BindingFlags.OptionalParamBinding) == 0)
			{
				return methodInfo.Invoke(target, bindingFlags, binder, providedArgs, culture);
			}
			if (array4 == null)
			{
				array4 = new MethodInfo[1] { methodInfo };
			}
			if (providedArgs == null)
			{
				providedArgs = Array.Empty<object>();
			}
			object state = null;
			MethodBase methodBase = null;
			try
			{
				Binder binder2 = binder;
				BindingFlags bindingAttr = bindingFlags;
				MethodBase[] match = array4;
				methodBase = binder2.BindToMethod(bindingAttr, match, ref providedArgs, modifiers, culture, namedParams, out state);
			}
			catch (MissingMethodException)
			{
			}
			if (methodBase == null)
			{
				throw new MissingMethodException(FullName, name);
			}
			object? result = ((MethodInfo)methodBase).Invoke(target, bindingFlags, binder, providedArgs, culture);
			if (state != null)
			{
				binder.ReorderArgumentArray(ref providedArgs, state);
			}
			return result;
		}
		throw new MissingMethodException(FullName, name);
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "MemberTypes.Field is satisfied by (InvokeMemberMembers) on this method")]
		static FieldInfo[] GetFields(RuntimeType thisType, string name2, BindingFlags bindingAttr2)
		{
			return thisType.GetMember(name2, MemberTypes.Field, bindingAttr2) as FieldInfo[];
		}
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "MemberTypes.Method is satisfied by (InvokeMemberMembers) on this method")]
		static MethodInfo[] GetMethods(RuntimeType thisType, string name2, BindingFlags bindingAttr2)
		{
			return thisType.GetMember(name2, MemberTypes.Method, bindingAttr2) as MethodInfo[];
		}
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070", Justification = "MemberTypes.Property is satisfied by (InvokeMemberMembers) on this method")]
		static PropertyInfo[] GetProperties(RuntimeType thisType, string name2, BindingFlags bindingAttr2)
		{
			return thisType.GetMember(name2, MemberTypes.Property, bindingAttr2) as PropertyInfo[];
		}
	}

	private RuntimeType GetBaseType()
	{
		if (IsActualInterface)
		{
			return null;
		}
		if (RuntimeTypeHandle.IsGenericVariable(this))
		{
			Type[] genericParameterConstraints = GetGenericParameterConstraints();
			RuntimeType runtimeType = ObjectType;
			for (int i = 0; i < genericParameterConstraints.Length; i++)
			{
				RuntimeType runtimeType2 = (RuntimeType)genericParameterConstraints[i];
				if (runtimeType2.IsActualInterface)
				{
					continue;
				}
				if (runtimeType2.IsGenericParameter)
				{
					GenericParameterAttributes genericParameterAttributes = runtimeType2.GenericParameterAttributes;
					if ((genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == 0 && (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0)
					{
						continue;
					}
				}
				runtimeType = runtimeType2;
			}
			if (runtimeType == ObjectType && (GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None)
			{
				runtimeType = ValueType;
			}
			return runtimeType;
		}
		return GetParentType();
	}

	private static void ThrowIfTypeNeverValidGenericArgument(RuntimeType type)
	{
		if (type.IsPointer || type.IsFunctionPointer || type.IsByRef || type == typeof(void))
		{
			throw new ArgumentException(SR.Format(SR.Argument_NeverValidGenericArgument, type));
		}
	}

	internal static void SanityCheckGenericArguments(RuntimeType[] genericArguments, RuntimeType[] genericParameters)
	{
		ArgumentNullException.ThrowIfNull(genericArguments, "genericArguments");
		for (int i = 0; i < genericArguments.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(genericArguments[i]);
			ThrowIfTypeNeverValidGenericArgument(genericArguments[i]);
		}
		if (genericArguments.Length != genericParameters.Length)
		{
			throw new ArgumentException(SR.Format(SR.Argument_NotEnoughGenArguments, genericArguments.Length, genericParameters.Length));
		}
	}

	internal CorElementType GetUnderlyingCorElementType()
	{
		RuntimeType runtimeType = this;
		if (runtimeType.IsActualEnum)
		{
			runtimeType = (RuntimeType)Enum.GetUnderlyingType(runtimeType);
		}
		return runtimeType.GetCorElementType();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryGetByRefElementType(RuntimeType type, [NotNullWhen(true)] out RuntimeType elementType)
	{
		if (type.GetCorElementType() == CorElementType.ELEMENT_TYPE_BYREF)
		{
			elementType = RuntimeTypeHandle.GetElementType(type);
			return true;
		}
		elementType = null;
		return false;
	}

	internal bool CheckValue(ref object value)
	{
		if (IsInstanceOfType(value))
		{
			if (IsNullableOfT)
			{
				value = RuntimeMethodHandle.ReboxToNullable(value, this);
				return true;
			}
			return false;
		}
		bool copyBack = false;
		return TryChangeType(ref value, ref copyBack) switch
		{
			CheckValueStatus.Success => copyBack, 
			CheckValueStatus.ArgumentException => throw new ArgumentException(SR.Format(SR.Arg_ObjObjEx, value?.GetType(), this)), 
			CheckValueStatus.NotSupported_ByRefLike => throw new NotSupportedException(SR.NotSupported_ByRefLike), 
			_ => false, 
		};
	}

	internal bool CheckValue(ref object value, Binder binder, CultureInfo culture, BindingFlags invokeAttr)
	{
		if (IsInstanceOfType(value))
		{
			if (IsNullableOfT)
			{
				value = RuntimeMethodHandle.ReboxToNullable(value, this);
				return true;
			}
			return false;
		}
		bool copyBack = false;
		CheckValueStatus checkValueStatus = TryChangeType(ref value, ref copyBack);
		switch (checkValueStatus)
		{
		case CheckValueStatus.Success:
			return copyBack;
		case CheckValueStatus.ArgumentException:
			if ((invokeAttr & BindingFlags.ExactBinding) != BindingFlags.Default || binder == null || binder == Type.DefaultBinder)
			{
				break;
			}
			value = binder.ChangeType(value, this, culture);
			if (IsInstanceOfType(value))
			{
				if (IsNullableOfT)
				{
					value = RuntimeMethodHandle.ReboxToNullable(value, this);
				}
				return true;
			}
			checkValueStatus = TryChangeType(ref value, ref copyBack);
			if (checkValueStatus == CheckValueStatus.Success)
			{
				return copyBack;
			}
			break;
		}
		return checkValueStatus switch
		{
			CheckValueStatus.ArgumentException => throw new ArgumentException(SR.Format(SR.Arg_ObjObjEx, value?.GetType(), this)), 
			CheckValueStatus.NotSupported_ByRefLike => throw new NotSupportedException(SR.NotSupported_ByRefLike), 
			_ => false, 
		};
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:UnrecognizedReflectionPattern", Justification = "AllocateValueType is only called on a ValueType. You can always create an instance of a ValueType.")]
	[return: NotNullIfNotNull("value")]
	internal static object AllocateValueType(RuntimeType type, object value)
	{
		if (value != null)
		{
			return RuntimeHelpers.Box(ref value.GetRawData(), type.TypeHandle);
		}
		if (type.IsNullableOfT)
		{
			return RuntimeMethodHandle.ReboxToNullable(null, type);
		}
		return RuntimeHelpers.GetUninitializedObject(type);
	}

	private CheckValueStatus TryChangeType(ref object value, ref bool copyBack)
	{
		if (TryGetByRefElementType(this, out var elementType))
		{
			copyBack = true;
			if (elementType.IsInstanceOfType(value))
			{
				if (elementType.IsActualValueType)
				{
					if (elementType.IsNullableOfT)
					{
						value = RuntimeMethodHandle.ReboxToNullable(value, elementType);
					}
					else
					{
						value = AllocateValueType(elementType, value);
					}
				}
				return CheckValueStatus.Success;
			}
			if (value == null)
			{
				if (!elementType.IsActualValueType)
				{
					return CheckValueStatus.Success;
				}
				if (elementType.IsByRefLike)
				{
					return CheckValueStatus.NotSupported_ByRefLike;
				}
				value = AllocateValueType(elementType, null);
				return CheckValueStatus.Success;
			}
			return CheckValueStatus.ArgumentException;
		}
		if (value == null)
		{
			if (base.IsPointer || IsFunctionPointer)
			{
				value = (nint)0;
				return CheckValueStatus.Success;
			}
			if (!IsActualValueType)
			{
				return CheckValueStatus.Success;
			}
			if (IsByRefLike)
			{
				return CheckValueStatus.NotSupported_ByRefLike;
			}
			value = AllocateValueType(this, null);
			return CheckValueStatus.Success;
		}
		if (base.IsPointer || IsEnum || base.IsPrimitive || IsFunctionPointer)
		{
			return TryChangeTypeSpecial(ref value);
		}
		return CheckValueStatus.ArgumentException;
	}
}
