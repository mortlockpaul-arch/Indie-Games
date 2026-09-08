using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit;

internal sealed class ModuleBuilderImpl : ModuleBuilder
{
	private readonly Assembly _coreAssembly;

	private readonly string _name;

	private readonly MetadataBuilder _metadataBuilder;

	private readonly PersistedAssemblyBuilder _assemblyBuilder;

	private readonly TypeBuilderImpl _globalTypeBuilder;

	private readonly Dictionary<Assembly, AssemblyReferenceHandle> _assemblyReferences = new Dictionary<Assembly, AssemblyReferenceHandle>();

	private readonly Dictionary<Type, EntityHandle> _typeReferences = new Dictionary<Type, EntityHandle>();

	private readonly Dictionary<object, EntityHandle> _memberReferences = new Dictionary<object, EntityHandle>();

	private readonly List<TypeBuilderImpl> _typeDefinitions = new List<TypeBuilderImpl>();

	private readonly Guid _moduleVersionId;

	private Dictionary<string, ModuleReferenceHandle> _moduleReferences;

	private List<CustomAttributeWrapper> _customAttributes;

	private Dictionary<SymbolDocumentWriter, DocumentHandle> _docHandles = new Dictionary<SymbolDocumentWriter, DocumentHandle>();

	private int _nextTypeDefRowId = 1;

	private int _nextMethodDefRowId = 1;

	private int _nextFieldDefRowId = 1;

	private int _nextParameterRowId = 1;

	private int _nextPropertyRowId = 1;

	private int _nextEventRowId = 1;

	private bool _coreTypesFullyPopulated;

	private bool _hasGlobalBeenCreated;

	private Type[] _coreTypes;

	private MetadataBuilder _pdbBuilder = new MetadataBuilder();

	private static readonly Type[] s_coreTypes = new Type[19]
	{
		typeof(void),
		typeof(object),
		typeof(bool),
		typeof(char),
		typeof(sbyte),
		typeof(byte),
		typeof(short),
		typeof(ushort),
		typeof(int),
		typeof(uint),
		typeof(long),
		typeof(ulong),
		typeof(float),
		typeof(double),
		typeof(string),
		typeof(nint),
		typeof(nuint),
		typeof(TypedReference),
		typeof(ValueType)
	};

	[RequiresAssemblyFiles("Returns <Unknown> for modules with no file path")]
	public override string Name => "<In Memory Module>";

	public override string ScopeName => _name;

	public override Assembly Assembly => _assemblyBuilder;

	public override Guid ModuleVersionId => _moduleVersionId;

	internal ModuleBuilderImpl(string name, Assembly coreAssembly, MetadataBuilder builder, PersistedAssemblyBuilder assemblyBuilder)
	{
		_coreAssembly = coreAssembly;
		_name = name;
		_metadataBuilder = builder;
		_assemblyBuilder = assemblyBuilder;
		_moduleVersionId = Guid.NewGuid();
		_globalTypeBuilder = new TypeBuilderImpl(this);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Types are preserved via s_coreTypes")]
	internal Type GetTypeFromCoreAssembly(CoreTypeId typeId)
	{
		if (_coreTypes == null)
		{
			if (_coreAssembly == typeof(object).Assembly)
			{
				_coreTypes = s_coreTypes;
				_coreTypesFullyPopulated = true;
			}
			else
			{
				_coreTypes = new Type[s_coreTypes.Length];
			}
		}
		return _coreTypes[(int)typeId] ?? (_coreTypes[(int)typeId] = _coreAssembly.GetType(s_coreTypes[(int)typeId].FullName, throwOnError: true));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Types are preserved via s_coreTypes")]
	internal CoreTypeId? GetTypeIdFromCoreTypes(Type type)
	{
		if (_coreTypes == null)
		{
			if (_coreAssembly == typeof(object).Assembly)
			{
				_coreTypes = s_coreTypes;
				_coreTypesFullyPopulated = true;
			}
			else
			{
				_coreTypes = new Type[s_coreTypes.Length];
			}
		}
		if (!_coreTypesFullyPopulated)
		{
			for (int i = 0; i < _coreTypes.Length; i++)
			{
				if (_coreTypes[i] == null)
				{
					_coreTypes[i] = _coreAssembly.GetType(s_coreTypes[i].FullName, throwOnError: false);
				}
			}
			_coreTypesFullyPopulated = true;
		}
		for (int j = 0; j < _coreTypes.Length; j++)
		{
			if (_coreTypes[j] == type)
			{
				return (CoreTypeId)j;
			}
		}
		return null;
	}

	internal void AppendMetadata(MethodBodyStreamEncoder methodBodyEncoder, BlobBuilder fieldDataBuilder, out MetadataBuilder pdbBuilder)
	{
		ModuleDefinitionHandle moduleDefinitionHandle = _metadataBuilder.AddModule(0, _metadataBuilder.GetOrAddString(_name), _metadataBuilder.GetOrAddGuid(_moduleVersionId), default(GuidHandle), default(GuidHandle));
		_metadataBuilder.AddTypeDefinition(TypeAttributes.NotPublic, default(StringHandle), _metadataBuilder.GetOrAddString("<Module>"), default(EntityHandle), MetadataTokens.FieldDefinitionHandle(1), MetadataTokens.MethodDefinitionHandle(1));
		WriteCustomAttributes(_customAttributes, moduleDefinitionHandle);
		List<GenericTypeParameterBuilderImpl> list = new List<GenericTypeParameterBuilderImpl>();
		PopulateTokensForTypesAndItsMembers();
		WriteFields(_globalTypeBuilder, fieldDataBuilder);
		WriteMethods(_globalTypeBuilder._methodDefinitions, list, methodBodyEncoder);
		foreach (TypeBuilderImpl typeDefinition in _typeDefinitions)
		{
			typeDefinition.ThrowIfNotCreated();
			EntityHandle parent = default(EntityHandle);
			if ((object)typeDefinition.BaseType != null)
			{
				parent = GetTypeHandle(typeDefinition.BaseType);
			}
			TypeDefinitionHandle typeDefinitionHandle = AddTypeDefinition(typeDefinition, parent, typeDefinition._firstMethodToken, typeDefinition._firstFieldToken);
			if (typeDefinition.IsGenericType)
			{
				Type[] genericTypeParameters = typeDefinition.GenericTypeParameters;
				for (int i = 0; i < genericTypeParameters.Length; i++)
				{
					GenericTypeParameterBuilderImpl genericTypeParameterBuilderImpl = (GenericTypeParameterBuilderImpl)genericTypeParameters[i];
					genericTypeParameterBuilderImpl._parentHandle = typeDefinitionHandle;
					list.Add(genericTypeParameterBuilderImpl);
				}
			}
			if ((typeDefinition.Attributes & TypeAttributes.ExplicitLayout) != TypeAttributes.NotPublic)
			{
				_metadataBuilder.AddTypeLayout(typeDefinitionHandle, (ushort)typeDefinition.PackingSize, (uint)typeDefinition.Size);
			}
			if (typeDefinition.DeclaringType != null)
			{
				_metadataBuilder.AddNestedType(typeDefinitionHandle, (TypeDefinitionHandle)GetTypeHandle(typeDefinition.DeclaringType));
			}
			WriteInterfaceImplementations(typeDefinition, typeDefinitionHandle);
			WriteCustomAttributes(typeDefinition._customAttributes, typeDefinitionHandle);
			WriteProperties(typeDefinition);
			WriteFields(typeDefinition, fieldDataBuilder);
			WriteMethods(typeDefinition._methodDefinitions, list, methodBodyEncoder);
			WriteEvents(typeDefinition);
		}
		list.Sort(delegate(GenericTypeParameterBuilderImpl x, GenericTypeParameterBuilderImpl y)
		{
			int num = CodedIndex.TypeOrMethodDef(x._parentHandle).CompareTo(CodedIndex.TypeOrMethodDef(y._parentHandle));
			return (num != 0) ? num : x.GenericParameterPosition.CompareTo(y.GenericParameterPosition);
		});
		foreach (GenericTypeParameterBuilderImpl item in list)
		{
			AddGenericTypeParametersAndConstraintsCustomAttributes(item._parentHandle, item);
		}
		pdbBuilder = _pdbBuilder;
	}

	private void WriteInterfaceImplementations(TypeBuilderImpl typeBuilder, TypeDefinitionHandle typeHandle)
	{
		if (typeBuilder._interfaces != null)
		{
			foreach (Type @interface in typeBuilder._interfaces)
			{
				_metadataBuilder.AddInterfaceImplementation(typeHandle, GetTypeHandle(@interface));
			}
		}
		if (typeBuilder._methodOverrides == null)
		{
			return;
		}
		foreach (List<(MethodInfo, MethodInfo)> value in typeBuilder._methodOverrides.Values)
		{
			foreach (var item in value)
			{
				_metadataBuilder.AddMethodImplementation(typeHandle, GetMemberHandle(item.Item2), GetMemberHandle(item.Item1));
			}
		}
	}

	private void WriteEvents(TypeBuilderImpl typeBuilder)
	{
		if (typeBuilder._eventDefinitions.Count == 0)
		{
			return;
		}
		AddEventMap(typeBuilder._handle, typeBuilder._firstEventToken);
		foreach (EventBuilderImpl eventDefinition in typeBuilder._eventDefinitions)
		{
			EventDefinitionHandle eventDefinitionHandle = AddEventDefinition(eventDefinition, GetTypeHandle(eventDefinition.EventType));
			WriteCustomAttributes(eventDefinition._customAttributes, eventDefinitionHandle);
			if (eventDefinition._addOnMethod is MethodBuilderImpl methodBuilderImpl)
			{
				AddMethodSemantics(eventDefinitionHandle, MethodSemanticsAttributes.Adder, methodBuilderImpl._handle);
			}
			if (eventDefinition._raiseMethod is MethodBuilderImpl methodBuilderImpl2)
			{
				AddMethodSemantics(eventDefinitionHandle, MethodSemanticsAttributes.Raiser, methodBuilderImpl2._handle);
			}
			if (eventDefinition._removeMethod is MethodBuilderImpl methodBuilderImpl3)
			{
				AddMethodSemantics(eventDefinitionHandle, MethodSemanticsAttributes.Remover, methodBuilderImpl3._handle);
			}
			if (eventDefinition._otherMethods == null)
			{
				continue;
			}
			foreach (MethodBuilderImpl otherMethod in eventDefinition._otherMethods)
			{
				AddMethodSemantics(eventDefinitionHandle, MethodSemanticsAttributes.Other, otherMethod._handle);
			}
		}
	}

	private void WriteProperties(TypeBuilderImpl typeBuilder)
	{
		if (typeBuilder._propertyDefinitions.Count == 0)
		{
			return;
		}
		AddPropertyMap(typeBuilder._handle, typeBuilder._firstPropertyToken);
		foreach (PropertyBuilderImpl propertyDefinition in typeBuilder._propertyDefinitions)
		{
			PropertyDefinitionHandle propertyDefinitionHandle = AddPropertyDefinition(propertyDefinition, MetadataSignatureHelper.GetPropertySignature(propertyDefinition, this));
			WriteCustomAttributes(propertyDefinition._customAttributes, propertyDefinitionHandle);
			if (propertyDefinition.GetMethod is MethodBuilderImpl methodBuilderImpl)
			{
				AddMethodSemantics(propertyDefinitionHandle, MethodSemanticsAttributes.Getter, methodBuilderImpl._handle);
			}
			if (propertyDefinition.SetMethod is MethodBuilderImpl methodBuilderImpl2)
			{
				AddMethodSemantics(propertyDefinitionHandle, MethodSemanticsAttributes.Setter, methodBuilderImpl2._handle);
			}
			if (propertyDefinition._otherMethods != null)
			{
				foreach (MethodBuilderImpl otherMethod in propertyDefinition._otherMethods)
				{
					AddMethodSemantics(propertyDefinitionHandle, MethodSemanticsAttributes.Other, otherMethod._handle);
				}
			}
			if (propertyDefinition._defaultValue != DBNull.Value)
			{
				AddDefaultValue(propertyDefinitionHandle, propertyDefinition._defaultValue);
			}
		}
	}

	private void PopulateFieldDefinitionHandles(List<FieldBuilderImpl> fieldDefinitions)
	{
		foreach (FieldBuilderImpl fieldDefinition in fieldDefinitions)
		{
			fieldDefinition._handle = MetadataTokens.FieldDefinitionHandle(_nextFieldDefRowId++);
		}
	}

	private void PopulateMethodDefinitionHandles(List<MethodBuilderImpl> methods)
	{
		foreach (MethodBuilderImpl method in methods)
		{
			method._handle = MetadataTokens.MethodDefinitionHandle(_nextMethodDefRowId++);
		}
	}

	private void PopulatePropertyDefinitionHandles(List<PropertyBuilderImpl> properties)
	{
		foreach (PropertyBuilderImpl property in properties)
		{
			property._handle = MetadataTokens.PropertyDefinitionHandle(_nextPropertyRowId++);
		}
	}

	private void PopulateEventDefinitionHandles(List<EventBuilderImpl> eventDefinitions)
	{
		foreach (EventBuilderImpl eventDefinition in eventDefinitions)
		{
			eventDefinition._handle = MetadataTokens.EventDefinitionHandle(_nextEventRowId++);
		}
	}

	private void PopulateTokensForTypesAndItsMembers()
	{
		foreach (TypeBuilderImpl typeDefinition in _typeDefinitions)
		{
			typeDefinition._handle = MetadataTokens.TypeDefinitionHandle(++_nextTypeDefRowId);
			typeDefinition._firstMethodToken = _nextMethodDefRowId;
			typeDefinition._firstFieldToken = _nextFieldDefRowId;
			typeDefinition._firstPropertyToken = _nextPropertyRowId;
			typeDefinition._firstEventToken = _nextEventRowId;
			PopulateMethodDefinitionHandles(typeDefinition._methodDefinitions);
			PopulateFieldDefinitionHandles(typeDefinition._fieldDefinitions);
			PopulatePropertyDefinitionHandles(typeDefinition._propertyDefinitions);
			PopulateEventDefinitionHandles(typeDefinition._eventDefinitions);
		}
	}

	private void WriteMethods(List<MethodBuilderImpl> methods, List<GenericTypeParameterBuilderImpl> genericParams, MethodBodyStreamEncoder methodBodyEncoder)
	{
		foreach (MethodBuilderImpl method in methods)
		{
			int offset = -1;
			ILGeneratorImpl iLGeneratorImpl = method.ILGeneratorImpl;
			if (iLGeneratorImpl != null)
			{
				FillMemberReferences(iLGeneratorImpl);
				iLGeneratorImpl.AddExceptionBlocks();
				StandaloneSignatureHandle standaloneSignatureHandle = ((iLGeneratorImpl.LocalCount == 0) ? default(StandaloneSignatureHandle) : _metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(MetadataSignatureHelper.GetLocalSignature(iLGeneratorImpl.Locals, this))));
				offset = AddMethodBody(method, iLGeneratorImpl, standaloneSignatureHandle, methodBodyEncoder);
				AddSymbolInfo(iLGeneratorImpl, standaloneSignatureHandle, method._handle);
			}
			MethodDefinitionHandle methodDefinitionHandle = AddMethodDefinition(method, method.GetMethodSignatureBlob(), offset, _nextParameterRowId);
			WriteCustomAttributes(method._customAttributes, methodDefinitionHandle);
			if (method.IsGenericMethodDefinition)
			{
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					GenericTypeParameterBuilderImpl genericTypeParameterBuilderImpl = (GenericTypeParameterBuilderImpl)genericArguments[i];
					genericTypeParameterBuilderImpl._parentHandle = methodDefinitionHandle;
					genericParams.Add(genericTypeParameterBuilderImpl);
				}
			}
			if (method._parameterBuilders != null)
			{
				ParameterBuilderImpl[] parameterBuilders = method._parameterBuilders;
				foreach (ParameterBuilderImpl parameterBuilderImpl in parameterBuilders)
				{
					if (parameterBuilderImpl != null)
					{
						ParameterHandle parameterHandle = AddParameter(parameterBuilderImpl);
						WriteCustomAttributes(parameterBuilderImpl._customAttributes, parameterHandle);
						_nextParameterRowId++;
						if (parameterBuilderImpl._marshallingData != null)
						{
							AddMarshalling(parameterHandle, parameterBuilderImpl._marshallingData.SerializeMarshallingData());
						}
						if (parameterBuilderImpl._defaultValue != DBNull.Value)
						{
							AddDefaultValue(parameterHandle, parameterBuilderImpl._defaultValue);
						}
					}
				}
			}
			if (method._dllImportData != null)
			{
				AddMethodImport(methodDefinitionHandle, method._dllImportData.EntryPoint ?? method.Name, method._dllImportData.Flags, GetModuleReference(method._dllImportData.ModuleName));
			}
		}
	}

	private void AddSymbolInfo(ILGeneratorImpl il, StandaloneSignatureHandle localSignatureHandle, MethodDefinitionHandle methodHandle)
	{
		if (il.DocumentToSequencePoints.Count == 0)
		{
			_pdbBuilder.AddMethodDebugInformation(default(DocumentHandle), default(BlobHandle));
		}
		else
		{
			Dictionary<SymbolDocumentWriter, List<SequencePoint>>.Enumerator enumerator = il.DocumentToSequencePoints.GetEnumerator();
			if (il.DocumentToSequencePoints.Count > 1)
			{
				_pdbBuilder.AddMethodDebugInformation(default(DocumentHandle), PopulateMultiDocSequencePointsBlob(enumerator, localSignatureHandle));
			}
			else
			{
				int previousNonHiddenStartLine = -1;
				int previousNonHiddenStartColumn = -1;
				enumerator.MoveNext();
				BlobBuilder blobBuilder = new BlobBuilder();
				blobBuilder.WriteCompressedInteger(MetadataTokens.GetRowNumber(localSignatureHandle));
				PopulateSequencePointsBlob(blobBuilder, enumerator.Current.Value, ref previousNonHiddenStartLine, ref previousNonHiddenStartColumn);
				_pdbBuilder.AddMethodDebugInformation(GetDocument(enumerator.Current.Key), _pdbBuilder.GetOrAddBlob(blobBuilder));
			}
		}
		Scope scope = il.Scope;
		scope._endOffset = il.ILOffset;
		AddLocalScope(methodHandle, default(ImportScopeHandle), MetadataTokens.LocalVariableHandle(_pdbBuilder.GetRowCount(TableIndex.LocalVariable) + 1), scope);
	}

	private BlobHandle PopulateMultiDocSequencePointsBlob(Dictionary<SymbolDocumentWriter, List<SequencePoint>>.Enumerator enumerator, StandaloneSignatureHandle localSignature)
	{
		BlobBuilder blobBuilder = new BlobBuilder();
		int previousNonHiddenStartLine = -1;
		int previousNonHiddenStartColumn = -1;
		enumerator.MoveNext();
		KeyValuePair<SymbolDocumentWriter, List<SequencePoint>> current = enumerator.Current;
		blobBuilder.WriteCompressedInteger(MetadataTokens.GetRowNumber(localSignature));
		blobBuilder.WriteCompressedInteger(MetadataTokens.GetRowNumber(GetDocument(current.Key)));
		PopulateSequencePointsBlob(blobBuilder, current.Value, ref previousNonHiddenStartLine, ref previousNonHiddenStartColumn);
		while (enumerator.MoveNext())
		{
			current = enumerator.Current;
			blobBuilder.WriteCompressedInteger(0);
			blobBuilder.WriteCompressedInteger(MetadataTokens.GetRowNumber(GetDocument(current.Key)));
			PopulateSequencePointsBlob(blobBuilder, current.Value, ref previousNonHiddenStartLine, ref previousNonHiddenStartColumn);
		}
		return _pdbBuilder.GetOrAddBlob(blobBuilder);
	}

	private static void PopulateSequencePointsBlob(BlobBuilder spBlobBuilder, List<SequencePoint> sequencePoints, ref int previousNonHiddenStartLine, ref int previousNonHiddenStartColumn)
	{
		for (int i = 0; i < sequencePoints.Count; i++)
		{
			if (i > 0)
			{
				spBlobBuilder.WriteCompressedInteger(sequencePoints[i].Offset - sequencePoints[i - 1].Offset);
			}
			else
			{
				spBlobBuilder.WriteCompressedInteger(sequencePoints[i].Offset);
			}
			if (sequencePoints[i].IsHidden)
			{
				spBlobBuilder.WriteUInt16(0);
				continue;
			}
			SerializeDeltaLinesAndColumns(spBlobBuilder, sequencePoints[i]);
			if (previousNonHiddenStartLine < 0)
			{
				spBlobBuilder.WriteCompressedInteger(sequencePoints[i].StartLine);
				spBlobBuilder.WriteCompressedInteger(sequencePoints[i].StartColumn);
			}
			else
			{
				spBlobBuilder.WriteCompressedSignedInteger(sequencePoints[i].StartLine - previousNonHiddenStartLine);
				spBlobBuilder.WriteCompressedSignedInteger(sequencePoints[i].StartColumn - previousNonHiddenStartColumn);
			}
			previousNonHiddenStartLine = sequencePoints[i].StartLine;
			previousNonHiddenStartColumn = sequencePoints[i].StartColumn;
		}
	}

	private void AddLocalScope(MethodDefinitionHandle methodHandle, ImportScopeHandle parentImport, LocalVariableHandle firstLocalVariableHandle, Scope scope)
	{
		parentImport = GetImportScopeHandle(scope._importNamespaces, parentImport);
		firstLocalVariableHandle = GetLocalVariableHandle(scope._locals, firstLocalVariableHandle);
		_pdbBuilder.AddLocalScope(methodHandle, parentImport, firstLocalVariableHandle, MetadataTokens.LocalConstantHandle(1), scope._startOffset, scope._endOffset - scope._startOffset);
		if (scope._children == null)
		{
			return;
		}
		foreach (Scope child in scope._children)
		{
			AddLocalScope(methodHandle, parentImport, MetadataTokens.LocalVariableHandle(_pdbBuilder.GetRowCount(TableIndex.LocalVariable) + 1), child);
		}
	}

	private LocalVariableHandle GetLocalVariableHandle(List<LocalBuilder> locals, LocalVariableHandle firstLocalHandleOfLastScope)
	{
		if (locals != null)
		{
			bool flag = false;
			foreach (LocalBuilderImpl local in locals)
			{
				if (!string.IsNullOrEmpty(local.Name))
				{
					LocalVariableHandle localVariableHandle = _pdbBuilder.AddLocalVariable(LocalVariableAttributes.None, local.LocalIndex, (local.Name == null) ? _pdbBuilder.GetOrAddString(string.Empty) : _pdbBuilder.GetOrAddString(local.Name));
					if (!flag)
					{
						firstLocalHandleOfLastScope = localVariableHandle;
						flag = true;
					}
				}
			}
		}
		return firstLocalHandleOfLastScope;
	}

	private ImportScopeHandle GetImportScopeHandle(List<string> importNamespaces, ImportScopeHandle parent)
	{
		if (importNamespaces == null)
		{
			return default(ImportScopeHandle);
		}
		BlobBuilder blobBuilder = new BlobBuilder();
		foreach (string importNamespace in importNamespaces)
		{
			blobBuilder.WriteByte(1);
			blobBuilder.WriteCompressedInteger(MetadataTokens.GetHeapOffset(_pdbBuilder.GetOrAddBlobUTF8(importNamespace)));
		}
		return _pdbBuilder.AddImportScope(parent, _pdbBuilder.GetOrAddBlob(blobBuilder));
	}

	private static void SerializeDeltaLinesAndColumns(BlobBuilder spBuilder, SequencePoint sequencePoint)
	{
		int num = sequencePoint.EndLine - sequencePoint.StartLine;
		int value = sequencePoint.EndColumn - sequencePoint.StartColumn;
		spBuilder.WriteCompressedInteger(num);
		if (num == 0)
		{
			spBuilder.WriteCompressedInteger(value);
		}
		else
		{
			spBuilder.WriteCompressedSignedInteger(value);
		}
	}

	private DocumentHandle GetDocument(SymbolDocumentWriter docWriter)
	{
		if (!_docHandles.TryGetValue(docWriter, out var value))
		{
			value = AddDocument(docWriter.URL, docWriter.Language, docWriter.HashAlgorithm, docWriter.Hash);
			_docHandles.Add(docWriter, value);
		}
		return value;
	}

	private DocumentHandle AddDocument(string url, Guid language, Guid hashAlgorithm, byte[] hash)
	{
		return _pdbBuilder.AddDocument(_pdbBuilder.GetOrAddDocumentName(url), (hashAlgorithm == default(Guid)) ? default(GuidHandle) : _pdbBuilder.GetOrAddGuid(hashAlgorithm), (hash == null) ? default(BlobHandle) : _pdbBuilder.GetOrAddBlob(hash), (language == default(Guid)) ? default(GuidHandle) : _pdbBuilder.GetOrAddGuid(language));
	}

	private void FillMemberReferences(ILGeneratorImpl il)
	{
		foreach (KeyValuePair<object, BlobWriter> memberReference in il.GetMemberReferences())
		{
			if (memberReference.Key is MemberInfo member)
			{
				memberReference.Value.WriteInt32(MetadataTokens.GetToken(GetMemberHandle(member)));
			}
			if (memberReference.Key is KeyValuePair<MethodInfo, Type[]> keyValuePair)
			{
				memberReference.Value.WriteInt32(MetadataTokens.GetToken(GetMethodReference(keyValuePair.Key, keyValuePair.Value)));
			}
		}
	}

	private static int AddMethodBody(MethodBuilderImpl method, ILGeneratorImpl il, StandaloneSignatureHandle signature, MethodBodyStreamEncoder bodyEncoder)
	{
		return bodyEncoder.AddMethodBody(il.Instructions, il.GetMaxStack(), signature, method.InitLocals ? MethodBodyAttributes.InitLocals : MethodBodyAttributes.None, il.HasDynamicStackAllocation);
	}

	private void WriteFields(TypeBuilderImpl typeBuilder, BlobBuilder fieldDataBuilder)
	{
		foreach (FieldBuilderImpl fieldDefinition in typeBuilder._fieldDefinitions)
		{
			FieldDefinitionHandle fieldDefinitionHandle = AddFieldDefinition(fieldDefinition, MetadataSignatureHelper.GetFieldSignature(fieldDefinition.FieldType, fieldDefinition.GetRequiredCustomModifiers(), fieldDefinition.GetOptionalCustomModifiers(), this));
			WriteCustomAttributes(fieldDefinition._customAttributes, fieldDefinitionHandle);
			if (fieldDefinition._offset >= 0 && (typeBuilder.Attributes & TypeAttributes.ExplicitLayout) != TypeAttributes.NotPublic)
			{
				AddFieldLayout(fieldDefinitionHandle, fieldDefinition._offset);
			}
			if (fieldDefinition.Attributes.HasFlag(FieldAttributes.HasFieldMarshal) && fieldDefinition._marshallingData != null)
			{
				AddMarshalling(fieldDefinitionHandle, fieldDefinition._marshallingData.SerializeMarshallingData());
			}
			if (fieldDefinition.Attributes.HasFlag(FieldAttributes.HasDefault) && fieldDefinition._defaultValue != DBNull.Value)
			{
				AddDefaultValue(fieldDefinitionHandle, fieldDefinition._defaultValue);
			}
			if (fieldDefinition.Attributes.HasFlag(FieldAttributes.HasFieldRVA) && fieldDefinition._rvaData != null)
			{
				_metadataBuilder.AddFieldRelativeVirtualAddress(fieldDefinitionHandle, fieldDataBuilder.Count);
				fieldDataBuilder.WriteBytes(fieldDefinition._rvaData);
				fieldDataBuilder.Align(8);
			}
		}
	}

	private ModuleReferenceHandle GetModuleReference(string moduleName)
	{
		if (_moduleReferences == null)
		{
			_moduleReferences = new Dictionary<string, ModuleReferenceHandle>();
		}
		if (!_moduleReferences.TryGetValue(moduleName, out var value))
		{
			value = AddModuleReference(moduleName);
			_moduleReferences.Add(moduleName, value);
		}
		return value;
	}

	internal void WriteCustomAttributes(List<CustomAttributeWrapper> customAttributes, EntityHandle parent)
	{
		if (customAttributes == null)
		{
			return;
		}
		foreach (CustomAttributeWrapper customAttribute in customAttributes)
		{
			_metadataBuilder.AddCustomAttribute(parent, GetMemberHandle(customAttribute.Ctor), _metadataBuilder.GetOrAddBlob(customAttribute.Data));
		}
	}

	private EntityHandle GetTypeReferenceOrSpecificationHandle(Type type)
	{
		type = type.UnderlyingSystemType;
		if (!_typeReferences.TryGetValue(type, out var value))
		{
			value = ((type.HasElementType || type.IsGenericParameter || (type.IsGenericType && !type.IsGenericTypeDefinition)) ? ((EntityHandle)AddTypeSpecification(type)) : ((!type.IsNested) ? ((EntityHandle)AddTypeReference(GetAssemblyReference(type.Assembly), type.Namespace, type.Name)) : ((EntityHandle)AddTypeReference(GetTypeReferenceOrSpecificationHandle(type.DeclaringType), null, type.Name))));
			_typeReferences.Add(type, value);
		}
		return value;
	}

	private TypeSpecificationHandle AddTypeSpecification(Type type)
	{
		return _metadataBuilder.AddTypeSpecification(_metadataBuilder.GetOrAddBlob(MetadataSignatureHelper.GetTypeSpecificationSignature(type, this)));
	}

	private MethodSpecificationHandle AddMethodSpecification(EntityHandle methodHandle, Type[] genericArgs)
	{
		return _metadataBuilder.AddMethodSpecification(methodHandle, _metadataBuilder.GetOrAddBlob(MetadataSignatureHelper.GetMethodSpecificationSignature(genericArgs, this)));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2055:RequiresDynamicCode", Justification = "Test")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL3050:RequiresUnreferencedCode", Justification = "Test")]
	private EntityHandle GetMemberReferenceHandle(MemberInfo memberInfo)
	{
		if (!_memberReferences.TryGetValue(memberInfo, out var value))
		{
			if (!(memberInfo is FieldInfo fieldInfo))
			{
				if (!(memberInfo is ConstructorInfo memberInfo2))
				{
					if (!(memberInfo is MethodInfo methodInfo))
					{
						throw new NotSupportedException();
					}
					if (methodInfo.IsConstructedGenericMethod)
					{
						value = AddMethodSpecification(GetMemberHandle(methodInfo.GetGenericMethodDefinition()), methodInfo.GetGenericArguments());
					}
					else if (methodInfo is ArrayMethod arrayMethod)
					{
						value = AddMemberReference(arrayMethod.Name, GetTypeHandle(arrayMethod.DeclaringType), GetMethodArrayMethodSignature(arrayMethod));
					}
					else
					{
						MethodInfo methodInfo2 = (MethodInfo)GetOriginalMemberIfConstructedType(methodInfo);
						value = AddMemberReference(methodInfo2.Name, GetTypeHandle(memberInfo.DeclaringType), GetMethodSignature(methodInfo2, null));
					}
				}
				else
				{
					ConstructorInfo constructorInfo = (ConstructorInfo)GetOriginalMemberIfConstructedType(memberInfo2);
					value = AddMemberReference(constructorInfo.Name, GetTypeHandle(memberInfo.DeclaringType), MetadataSignatureHelper.GetConstructorSignature(constructorInfo.GetParameters(), this));
				}
			}
			else
			{
				Type type = fieldInfo.DeclaringType;
				if (type.IsGenericTypeDefinition)
				{
					type = type.MakeGenericType(type.GetGenericArguments());
				}
				Type modifiedFieldType = ((FieldInfo)GetOriginalMemberIfConstructedType(fieldInfo)).GetModifiedFieldType();
				value = AddMemberReference(fieldInfo.Name, GetTypeHandle(type), MetadataSignatureHelper.GetFieldSignature(modifiedFieldType, fieldInfo.GetRequiredCustomModifiers(), fieldInfo.GetOptionalCustomModifiers(), this));
			}
			_memberReferences.Add(memberInfo, value);
		}
		return value;
	}

	private EntityHandle GetMethodReference(MethodInfo methodInfo, Type[] optionalParameterTypes)
	{
		MethodInfo methodInfo2 = (MethodInfo)GetOriginalMemberIfConstructedType(methodInfo);
		BlobBuilder methodSignature = GetMethodSignature(methodInfo2, optionalParameterTypes);
		KeyValuePair<MethodInfo, BlobBuilder> keyValuePair = new KeyValuePair<MethodInfo, BlobBuilder>(methodInfo2, methodSignature);
		if (!_memberReferences.TryGetValue(keyValuePair, out var value))
		{
			value = AddMemberReference(methodInfo2.Name, GetMemberHandle(methodInfo2), methodSignature);
			_memberReferences.Add(keyValuePair, value);
		}
		return value;
	}

	private BlobBuilder GetMethodSignature(MethodInfo method, Type[] optionalParameterTypes)
	{
		return MetadataSignatureHelper.GetMethodSignature(this, MetadataSignatureHelper.GetParameterTypes(method.GetParameters()), method.ReturnParameter.GetModifiedParameterType(), GetSignatureConvention(method.CallingConvention), method.GetGenericArguments().Length, !method.IsStatic, optionalParameterTypes);
	}

	private BlobBuilder GetMethodArrayMethodSignature(ArrayMethod method)
	{
		return MetadataSignatureHelper.GetMethodSignature(this, method.ParameterTypes, method.ReturnType, GetSignatureConvention(method.CallingConvention), 0, IsInstance(method.CallingConvention));
	}

	private static bool IsInstance(CallingConventions callingConvention)
	{
		if (!callingConvention.HasFlag(CallingConventions.HasThis) && !callingConvention.HasFlag(CallingConventions.ExplicitThis))
		{
			return false;
		}
		return true;
	}

	internal static SignatureCallingConvention GetSignatureConvention(CallingConventions callingConventions)
	{
		SignatureCallingConvention signatureCallingConvention = SignatureCallingConvention.Default;
		if ((callingConventions & CallingConventions.VarArgs) != 0)
		{
			signatureCallingConvention = SignatureCallingConvention.VarArgs;
		}
		return (SignatureCallingConvention)((uint)signatureCallingConvention | (uint)((byte)callingConventions & 0x60));
	}

	private MemberInfo GetOriginalMemberIfConstructedType(MemberInfo memberInfo)
	{
		Type declaringType = memberInfo.DeclaringType;
		if (declaringType.IsConstructedGenericType && !(declaringType.GetGenericTypeDefinition() is TypeBuilderImpl) && !ContainsTypeBuilder(declaringType.GetGenericArguments()))
		{
			return declaringType.GetGenericTypeDefinition().GetMemberWithSameMetadataDefinitionAs(memberInfo);
		}
		return memberInfo;
	}

	private AssemblyReferenceHandle GetAssemblyReference(Assembly assembly)
	{
		if (!_assemblyReferences.TryGetValue(assembly, out var value))
		{
			AssemblyName name = assembly.GetName();
			AssemblyFlags assemblyFlags = (AssemblyFlags)0;
			byte[] array = name.GetPublicKey();
			if (array != null && array.Length != 0)
			{
				assemblyFlags = AssemblyFlags.PublicKey;
			}
			else
			{
				array = name.GetPublicKeyToken();
			}
			value = AddAssemblyReference(name.Name, name.Version, name.CultureName, array, assemblyFlags);
			_assemblyReferences.Add(assembly, value);
		}
		return value;
	}

	private void AddGenericTypeParametersAndConstraintsCustomAttributes(EntityHandle parentHandle, GenericTypeParameterBuilderImpl gParam)
	{
		GenericParameterHandle genericParameterHandle = _metadataBuilder.AddGenericParameter(parentHandle, gParam.GenericParameterAttributes, _metadataBuilder.GetOrAddString(gParam.Name), gParam.GenericParameterPosition);
		WriteCustomAttributes(gParam._customAttributes, genericParameterHandle);
		Type[] genericParameterConstraints = gParam.GetGenericParameterConstraints();
		foreach (Type type in genericParameterConstraints)
		{
			_metadataBuilder.AddGenericParameterConstraint(genericParameterHandle, GetTypeHandle(type));
		}
	}

	private void AddDefaultValue(EntityHandle parentHandle, object defaultValue)
	{
		if (defaultValue != null)
		{
			Type type = defaultValue.GetType();
			if (type.IsEnum)
			{
				defaultValue = Convert.ChangeType(defaultValue, type.GetEnumUnderlyingType());
			}
		}
		_metadataBuilder.AddConstant(parentHandle, defaultValue);
	}

	private void AddMethodSemantics(EntityHandle parentHandle, MethodSemanticsAttributes attribute, MethodDefinitionHandle methodHandle)
	{
		_metadataBuilder.AddMethodSemantics(parentHandle, attribute, methodHandle);
	}

	private PropertyDefinitionHandle AddPropertyDefinition(PropertyBuilderImpl property, BlobBuilder signature)
	{
		return _metadataBuilder.AddProperty(property.Attributes, _metadataBuilder.GetOrAddString(property.Name), _metadataBuilder.GetOrAddBlob(signature));
	}

	private EventDefinitionHandle AddEventDefinition(EventBuilderImpl eventBuilder, EntityHandle eventType)
	{
		return _metadataBuilder.AddEvent(eventBuilder.Attributes, _metadataBuilder.GetOrAddString(eventBuilder.Name), eventType);
	}

	private void AddEventMap(TypeDefinitionHandle typeHandle, int firstEventToken)
	{
		_metadataBuilder.AddEventMap(typeHandle, MetadataTokens.EventDefinitionHandle(firstEventToken));
	}

	private void AddPropertyMap(TypeDefinitionHandle typeHandle, int firstPropertyToken)
	{
		_metadataBuilder.AddPropertyMap(typeHandle, MetadataTokens.PropertyDefinitionHandle(firstPropertyToken));
	}

	private FieldDefinitionHandle AddFieldDefinition(FieldBuilderImpl field, BlobBuilder fieldSignature)
	{
		return _metadataBuilder.AddFieldDefinition(field.Attributes, _metadataBuilder.GetOrAddString(field.Name), _metadataBuilder.GetOrAddBlob(fieldSignature));
	}

	private TypeDefinitionHandle AddTypeDefinition(TypeBuilderImpl type, EntityHandle parent, int methodToken, int fieldToken)
	{
		return _metadataBuilder.AddTypeDefinition(type.Attributes, (type.Namespace == null) ? default(StringHandle) : _metadataBuilder.GetOrAddString(type.Namespace), _metadataBuilder.GetOrAddString(type.Name), parent, MetadataTokens.FieldDefinitionHandle(fieldToken), MetadataTokens.MethodDefinitionHandle(methodToken));
	}

	private MethodDefinitionHandle AddMethodDefinition(MethodBuilderImpl method, BlobBuilder methodSignature, int offset, int parameterToken)
	{
		return _metadataBuilder.AddMethodDefinition(method.Attributes, method.GetMethodImplementationFlags(), _metadataBuilder.GetOrAddString(method.Name), _metadataBuilder.GetOrAddBlob(methodSignature), offset, MetadataTokens.ParameterHandle(parameterToken));
	}

	private TypeReferenceHandle AddTypeReference(EntityHandle resolutionScope, string ns, string name)
	{
		return _metadataBuilder.AddTypeReference(resolutionScope, (ns == null) ? default(StringHandle) : _metadataBuilder.GetOrAddString(ns), _metadataBuilder.GetOrAddString(name));
	}

	private MemberReferenceHandle AddMemberReference(string memberName, EntityHandle parent, BlobBuilder signature)
	{
		return _metadataBuilder.AddMemberReference(parent, _metadataBuilder.GetOrAddString(memberName), _metadataBuilder.GetOrAddBlob(signature));
	}

	private void AddMethodImport(MethodDefinitionHandle methodHandle, string name, MethodImportAttributes attributes, ModuleReferenceHandle moduleHandle)
	{
		_metadataBuilder.AddMethodImport(methodHandle, attributes, _metadataBuilder.GetOrAddString(name), moduleHandle);
	}

	private ModuleReferenceHandle AddModuleReference(string moduleName)
	{
		return _metadataBuilder.AddModuleReference(_metadataBuilder.GetOrAddString(moduleName));
	}

	private void AddFieldLayout(FieldDefinitionHandle fieldHandle, int offset)
	{
		_metadataBuilder.AddFieldLayout(fieldHandle, offset);
	}

	private void AddMarshalling(EntityHandle parent, BlobBuilder builder)
	{
		_metadataBuilder.AddMarshallingDescriptor(parent, _metadataBuilder.GetOrAddBlob(builder));
	}

	private ParameterHandle AddParameter(ParameterBuilderImpl parameter)
	{
		return _metadataBuilder.AddParameter((ParameterAttributes)parameter.Attributes, (parameter.Name != null) ? _metadataBuilder.GetOrAddString(parameter.Name) : default(StringHandle), parameter.Position);
	}

	private AssemblyReferenceHandle AddAssemblyReference(string name, Version version, string culture, byte[] publicKeyToken, AssemblyFlags assemblyFlags)
	{
		return _metadataBuilder.AddAssemblyReference((name == null) ? default(StringHandle) : _metadataBuilder.GetOrAddString(name), version ?? new Version(0, 0, 0, 0), (culture == null) ? default(StringHandle) : _metadataBuilder.GetOrAddString(culture), (publicKeyToken == null) ? default(BlobHandle) : _metadataBuilder.GetOrAddBlob(publicKeyToken), assemblyFlags, default(BlobHandle));
	}

	internal EntityHandle GetTypeHandle(Type type)
	{
		if (type is TypeBuilderImpl typeBuilderImpl && Equals(typeBuilderImpl.Module))
		{
			return typeBuilderImpl._handle;
		}
		if (type is EnumBuilderImpl enumBuilderImpl && Equals(enumBuilderImpl.Module))
		{
			return enumBuilderImpl._typeBuilder._handle;
		}
		return GetTypeReferenceOrSpecificationHandle(type);
	}

	internal EntityHandle GetMemberHandle(MemberInfo member)
	{
		if (member is TypeBuilderImpl typeBuilderImpl && Equals(typeBuilderImpl.Module))
		{
			return typeBuilderImpl._handle;
		}
		if (member is EnumBuilderImpl enumBuilderImpl && Equals(enumBuilderImpl.Module))
		{
			return enumBuilderImpl._typeBuilder._handle;
		}
		if (member is Type type)
		{
			return GetTypeReferenceOrSpecificationHandle(type);
		}
		if (member is MethodBuilderImpl methodBuilderImpl && Equals(methodBuilderImpl.Module))
		{
			return methodBuilderImpl._handle;
		}
		if (member is ConstructorBuilderImpl constructorBuilderImpl && Equals(constructorBuilderImpl.Module))
		{
			return constructorBuilderImpl._methodBuilder._handle;
		}
		if (member is FieldBuilderImpl fieldBuilderImpl && Equals(fieldBuilderImpl.Module) && !fieldBuilderImpl.DeclaringType.IsGenericTypeDefinition)
		{
			return fieldBuilderImpl._handle;
		}
		if (member is PropertyBuilderImpl propertyBuilderImpl && Equals(propertyBuilderImpl.Module))
		{
			return propertyBuilderImpl._handle;
		}
		return GetMemberReferenceHandle(member);
	}

	internal TypeBuilder DefineNestedType(string name, TypeAttributes attr, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parent, Type[] interfaces, PackingSize packingSize, int typesize, TypeBuilderImpl enclosingType)
	{
		TypeBuilderImpl typeBuilderImpl = new TypeBuilderImpl(name, attr, parent, this, interfaces, packingSize, typesize, enclosingType);
		_typeDefinitions.Add(typeBuilderImpl);
		return typeBuilderImpl;
	}

	public override bool IsDefined(Type attributeType, bool inherit)
	{
		throw new NotImplementedException();
	}

	public override int GetFieldMetadataToken(FieldInfo field)
	{
		return GetTokenForHandle(TryGetFieldHandle(field));
	}

	internal EntityHandle TryGetFieldHandle(FieldInfo field)
	{
		if (field is FieldBuilderImpl fieldBuilderImpl && Equals(fieldBuilderImpl.Module))
		{
			return fieldBuilderImpl._handle;
		}
		return GetHandleForMember(field);
	}

	private static int GetTokenForHandle(EntityHandle handle)
	{
		if (handle.IsNil)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_TokenNotPopulated);
		}
		return MetadataTokens.GetToken(handle);
	}

	private EntityHandle GetHandleForMember(MemberInfo member)
	{
		if (IsConstructedFromTypeBuilder(member.DeclaringType))
		{
			return default(EntityHandle);
		}
		return GetMemberReferenceHandle(member);
	}

	private bool IsConstructedFromTypeBuilder(Type type)
	{
		if (type.IsConstructedGenericType)
		{
			if (!(type.GetGenericTypeDefinition() is TypeBuilderImpl typeBuilderImpl) || !Equals(typeBuilderImpl.Module))
			{
				return ContainsTypeBuilder(type.GetGenericArguments());
			}
			return true;
		}
		if (type.HasElementType)
		{
			Type elementType = type.GetElementType();
			if (!(elementType is TypeBuilderImpl typeBuilderImpl2) || !Equals(typeBuilderImpl2.Module))
			{
				return IsConstructedFromTypeBuilder(elementType);
			}
			return true;
		}
		return false;
	}

	internal bool ContainsTypeBuilder(Type[] genericArguments)
	{
		foreach (Type type in genericArguments)
		{
			if ((type is TypeBuilderImpl typeBuilderImpl && Equals(typeBuilderImpl.Module)) || (type is GenericTypeParameterBuilderImpl genericTypeParameterBuilderImpl && Equals(genericTypeParameterBuilderImpl.Module)))
			{
				return true;
			}
			if (IsConstructedFromTypeBuilder(type))
			{
				return true;
			}
		}
		return false;
	}

	internal EntityHandle TryGetTypeHandle(Type type)
	{
		if (type is TypeBuilderImpl typeBuilderImpl && Equals(typeBuilderImpl.Module))
		{
			return typeBuilderImpl._handle;
		}
		if (type is EnumBuilderImpl enumBuilderImpl && Equals(enumBuilderImpl.Module))
		{
			return enumBuilderImpl._typeBuilder._handle;
		}
		if (IsConstructedFromTypeBuilder(type))
		{
			return default(EntityHandle);
		}
		return GetTypeReferenceOrSpecificationHandle(type);
	}

	public override int GetMethodMetadataToken(ConstructorInfo constructor)
	{
		return GetTokenForHandle(TryGetConstructorHandle(constructor));
	}

	internal EntityHandle TryGetConstructorHandle(ConstructorInfo constructor)
	{
		if (constructor is ConstructorBuilderImpl constructorBuilderImpl && Equals(constructorBuilderImpl.Module))
		{
			return constructorBuilderImpl._methodBuilder._handle;
		}
		return GetHandleForMember(constructor);
	}

	public override int GetMethodMetadataToken(MethodInfo method)
	{
		return GetTokenForHandle(TryGetMethodHandle(method));
	}

	internal EntityHandle TryGetMethodHandle(MethodInfo method)
	{
		if (method is MethodBuilderImpl methodBuilderImpl && Equals(methodBuilderImpl.Module))
		{
			return methodBuilderImpl._handle;
		}
		if (IsConstructedFromMethodBuilderOrTypeBuilder(method) || IsArrayMethodTypeIsTypeBuilder(method))
		{
			return default(EntityHandle);
		}
		return GetHandleForMember(method);
	}

	private bool IsArrayMethodTypeIsTypeBuilder(MethodInfo method)
	{
		if (method is ArrayMethod arrayMethod && arrayMethod.DeclaringType.GetElementType() is TypeBuilderImpl typeBuilderImpl)
		{
			return Equals(typeBuilderImpl.Module);
		}
		return false;
	}

	private bool IsConstructedFromMethodBuilderOrTypeBuilder(MethodInfo method)
	{
		if (method.IsConstructedGenericMethod)
		{
			if (!(method.GetGenericMethodDefinition() is MethodBuilderImpl methodBuilderImpl) || !Equals(methodBuilderImpl.Module))
			{
				return ContainsTypeBuilder(method.GetGenericArguments());
			}
			return true;
		}
		return false;
	}

	internal EntityHandle TryGetMethodHandle(MethodInfo method, Type[] optionalParameterTypes)
	{
		if ((method.CallingConvention & CallingConventions.VarArgs) == 0)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_NotAVarArgCallingConvention);
		}
		if (method is MethodBuilderImpl methodBuilderImpl && Equals(methodBuilderImpl.Module))
		{
			return methodBuilderImpl._handle;
		}
		if (IsConstructedFromMethodBuilderOrTypeBuilder(method))
		{
			return default(EntityHandle);
		}
		return GetMethodReference(method, optionalParameterTypes);
	}

	internal TypeBuilderImpl FindTypeBuilderWithName(string strTypeName, bool ignoreCase)
	{
		StringComparison comparisonType = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		foreach (TypeBuilderImpl typeDefinition in _typeDefinitions)
		{
			if (string.Equals(typeDefinition.Name, strTypeName, comparisonType))
			{
				return typeDefinition;
			}
		}
		return null;
	}

	public override int GetStringMetadataToken(string stringConstant)
	{
		return MetadataTokens.GetToken(_metadataBuilder.GetOrAddUserString(stringConstant));
	}

	public override int GetTypeMetadataToken(Type type)
	{
		return GetTokenForHandle(TryGetTypeHandle(type));
	}

	protected override void CreateGlobalFunctionsCore()
	{
		if (_hasGlobalBeenCreated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_GlobalsHaveBeenCreated);
		}
		_globalTypeBuilder.CreateTypeInfo();
		_hasGlobalBeenCreated = true;
	}

	protected override EnumBuilder DefineEnumCore(string name, TypeAttributes visibility, Type underlyingType)
	{
		EnumBuilderImpl enumBuilderImpl = new EnumBuilderImpl(name, underlyingType, visibility, this);
		_typeDefinitions.Add(enumBuilderImpl._typeBuilder);
		return enumBuilderImpl;
	}

	protected override MethodBuilder DefineGlobalMethodCore(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
	{
		if (_hasGlobalBeenCreated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_GlobalsHaveBeenCreated);
		}
		if ((attributes & MethodAttributes.Static) == 0)
		{
			throw new ArgumentException(System.SR.Argument_GlobalMembersMustBeStatic);
		}
		MethodBuilderImpl obj = (MethodBuilderImpl)_globalTypeBuilder.DefineMethod(name, attributes, callingConvention, returnType, requiredReturnTypeCustomModifiers, optionalReturnTypeCustomModifiers, parameterTypes, requiredParameterTypeCustomModifiers, optionalParameterTypeCustomModifiers);
		obj._handle = MetadataTokens.MethodDefinitionHandle(_nextMethodDefRowId++);
		return obj;
	}

	protected override FieldBuilder DefineInitializedDataCore(string name, byte[] data, FieldAttributes attributes)
	{
		if (_hasGlobalBeenCreated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_GlobalsHaveBeenCreated);
		}
		FieldBuilderImpl obj = (FieldBuilderImpl)_globalTypeBuilder.DefineInitializedData(name, data, attributes);
		obj._handle = MetadataTokens.FieldDefinitionHandle(_nextFieldDefRowId++);
		return obj;
	}

	[RequiresUnreferencedCode("P/Invoke marshalling may dynamically access members that could be trimmed.")]
	protected override MethodBuilder DefinePInvokeMethodCore(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
	{
		if ((attributes & MethodAttributes.Static) == 0)
		{
			throw new ArgumentException(System.SR.Argument_GlobalMembersMustBeStatic);
		}
		MethodBuilderImpl obj = (MethodBuilderImpl)_globalTypeBuilder.DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
		obj._handle = MetadataTokens.MethodDefinitionHandle(_nextMethodDefRowId++);
		return obj;
	}

	protected override TypeBuilder DefineTypeCore(string name, TypeAttributes attr, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type parent, Type[] interfaces, PackingSize packingSize, int typesize)
	{
		TypeBuilderImpl typeBuilderImpl = new TypeBuilderImpl(name, attr, parent, this, interfaces, packingSize, typesize, null);
		_typeDefinitions.Add(typeBuilderImpl);
		return typeBuilderImpl;
	}

	protected override FieldBuilder DefineUninitializedDataCore(string name, int size, FieldAttributes attributes)
	{
		if (_hasGlobalBeenCreated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_GlobalsHaveBeenCreated);
		}
		FieldBuilderImpl obj = (FieldBuilderImpl)_globalTypeBuilder.DefineUninitializedData(name, size, attributes);
		obj._handle = MetadataTokens.FieldDefinitionHandle(_nextFieldDefRowId++);
		return obj;
	}

	protected override MethodInfo GetArrayMethodCore(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
	{
		if (!arrayClass.IsArray)
		{
			throw new ArgumentException(System.SR.Argument_HasToBeArrayClass);
		}
		return new ArrayMethod(this, arrayClass, methodName, callingConvention, returnType, parameterTypes);
	}

	protected override void SetCustomAttributeCore(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		if (_customAttributes == null)
		{
			_customAttributes = new List<CustomAttributeWrapper>();
		}
		_customAttributes.Add(new CustomAttributeWrapper(con, binaryAttribute));
	}

	[RequiresUnreferencedCode("Methods might be removed")]
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		if (types == null)
		{
			return _globalTypeBuilder.GetMethod(name, bindingAttr);
		}
		return _globalTypeBuilder.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
	}

	[RequiresUnreferencedCode("Methods might be removed")]
	public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
	{
		return _globalTypeBuilder.GetMethods(bindingFlags);
	}

	public override int GetSignatureMetadataToken(SignatureHelper signature)
	{
		return MetadataTokens.GetToken(_metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(signature.GetSignature())));
	}

	internal int GetSignatureToken(CallingConventions callingConventions, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
	{
		return MetadataTokens.GetToken(_metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(MetadataSignatureHelper.GetMethodSignature(this, parameterTypes, returnType, GetSignatureConvention(callingConventions), 0, isInstance: false, optionalParameterTypes))));
	}

	internal int GetSignatureToken(CallingConvention callingConvention, Type returnType, Type[] parameterTypes)
	{
		return MetadataTokens.GetToken(_metadataBuilder.AddStandaloneSignature(_metadataBuilder.GetOrAddBlob(MetadataSignatureHelper.GetMethodSignature(this, parameterTypes, returnType, GetSignatureConvention(callingConvention)))));
	}

	private static SignatureCallingConvention GetSignatureConvention(CallingConvention callingConvention)
	{
		return callingConvention switch
		{
			CallingConvention.Winapi => SignatureCallingConvention.Default, 
			CallingConvention.Cdecl => SignatureCallingConvention.CDecl, 
			CallingConvention.StdCall => SignatureCallingConvention.StdCall, 
			CallingConvention.ThisCall => SignatureCallingConvention.ThisCall, 
			CallingConvention.FastCall => SignatureCallingConvention.FastCall, 
			_ => SignatureCallingConvention.Default, 
		};
	}

	protected override ISymbolDocumentWriter DefineDocumentCore(string url, Guid language = default(Guid))
	{
		return new SymbolDocumentWriter(url, language);
	}

	internal List<TypeBuilderImpl> GetNestedTypeBuilders(TypeBuilderImpl declaringType)
	{
		List<TypeBuilderImpl> list = new List<TypeBuilderImpl>();
		foreach (TypeBuilderImpl typeDefinition in _typeDefinitions)
		{
			if (typeDefinition.DeclaringType == declaringType)
			{
				list.Add(typeDefinition);
			}
		}
		return list;
	}
}
