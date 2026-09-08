using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace System.Reflection.Emit;

public sealed class PersistedAssemblyBuilder : AssemblyBuilder
{
	private readonly AssemblyName _assemblyName;

	private readonly Assembly _coreAssembly;

	private readonly MetadataBuilder _metadataBuilder;

	private ModuleBuilderImpl _module;

	private bool _isMetadataPopulated;

	internal List<CustomAttributeWrapper> _customAttributes;

	public override string? FullName => _assemblyName.FullName;

	public override Module ManifestModule => _module ?? throw new InvalidOperationException(System.SR.InvalidOperation_AModuleRequired);

	public PersistedAssemblyBuilder(AssemblyName name, Assembly coreAssembly, IEnumerable<CustomAttributeBuilder>? assemblyAttributes = null)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ArgumentException.ThrowIfNullOrEmpty(name.Name, "AssemblyName.Name");
		ArgumentNullException.ThrowIfNull(coreAssembly, "coreAssembly");
		_assemblyName = (AssemblyName)name.Clone();
		_coreAssembly = coreAssembly;
		_metadataBuilder = new MetadataBuilder();
		if (assemblyAttributes == null)
		{
			return;
		}
		foreach (CustomAttributeBuilder assemblyAttribute in assemblyAttributes)
		{
			SetCustomAttribute(assemblyAttribute);
		}
	}

	private void WritePEImage(Stream peStream, BlobBuilder ilBuilder, BlobBuilder fieldData)
	{
		ManagedPEBuilder managedPEBuilder = new ManagedPEBuilder(new PEHeaderBuilder(Machine.Unknown, 8192, 512, 4194304uL, 48, 0, 4, 0, 0, 0, 4, 0, Subsystem.WindowsCui, DllCharacteristics.DynamicBase | DllCharacteristics.NxCompatible | DllCharacteristics.NoSeh | DllCharacteristics.TerminalServerAware, Characteristics.ExecutableImage | Characteristics.Dll, 1048576uL, 4096uL, 1048576uL, 4096uL), new MetadataRootBuilder(_metadataBuilder), ilBuilder, fieldData, null, null, null, 0);
		BlobBuilder blobBuilder = new BlobBuilder();
		managedPEBuilder.Serialize(blobBuilder);
		blobBuilder.WriteContentTo(peStream);
	}

	public void Save(Stream stream)
	{
		SaveInternal(stream);
	}

	public void Save(string assemblyFileName)
	{
		ArgumentNullException.ThrowIfNull(assemblyFileName, "assemblyFileName");
		using FileStream stream = new FileStream(assemblyFileName, FileMode.Create, FileAccess.Write);
		SaveInternal(stream);
	}

	private void SaveInternal(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		PopulateAssemblyMetadata(out var ilStream, out var fieldData, out var _);
		WritePEImage(stream, ilStream, fieldData);
	}

	[CLSCompliant(false)]
	public MetadataBuilder GenerateMetadata(out BlobBuilder ilStream, out BlobBuilder mappedFieldData)
	{
		PopulateAssemblyMetadata(out ilStream, out mappedFieldData, out MetadataBuilder _);
		return _metadataBuilder;
	}

	[CLSCompliant(false)]
	public MetadataBuilder GenerateMetadata(out BlobBuilder ilStream, out BlobBuilder mappedFieldData, out MetadataBuilder pdbBuilder)
	{
		PopulateAssemblyMetadata(out ilStream, out mappedFieldData, out pdbBuilder);
		return _metadataBuilder;
	}

	private void PopulateAssemblyMetadata(out BlobBuilder ilStream, out BlobBuilder fieldData, out MetadataBuilder pdbBuilder)
	{
		if (_module == null)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_AModuleRequired);
		}
		if (_isMetadataPopulated)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_CannotPopulateMultipleTimes);
		}
		ilStream = new BlobBuilder();
		fieldData = new BlobBuilder();
		MetadataBuilder metadataBuilder = _metadataBuilder;
		StringHandle orAddString = _metadataBuilder.GetOrAddString(_assemblyName.Name);
		Version? version = _assemblyName.Version ?? new Version(0, 0, 0, 0);
		StringHandle culture = ((_assemblyName.CultureName == null) ? default(StringHandle) : _metadataBuilder.GetOrAddString(_assemblyName.CultureName));
		byte[] publicKey = _assemblyName.GetPublicKey();
		AssemblyDefinitionHandle assemblyDefinitionHandle = metadataBuilder.AddAssembly(orAddString, version, culture, (publicKey != null) ? _metadataBuilder.GetOrAddBlob(publicKey) : default(BlobHandle), AddContentType((AssemblyFlags)_assemblyName.Flags, _assemblyName.ContentType), (AssemblyHashAlgorithm)_assemblyName.HashAlgorithm);
		_module.WriteCustomAttributes(_customAttributes, assemblyDefinitionHandle);
		_module.AppendMetadata(new MethodBodyStreamEncoder(ilStream), fieldData, out pdbBuilder);
		_isMetadataPopulated = true;
	}

	private static AssemblyFlags AddContentType(AssemblyFlags flags, AssemblyContentType contentType)
	{
		return (AssemblyFlags)(((int)contentType << 9) | (int)flags);
	}

	protected override ModuleBuilder DefineDynamicModuleCore(string name)
	{
		if (_module != null)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_NoMultiModuleAssembly);
		}
		_module = new ModuleBuilderImpl(name, _coreAssembly, _metadataBuilder, this);
		return _module;
	}

	protected override ModuleBuilder? GetDynamicModuleCore(string name)
	{
		if (_module != null && _module.ScopeName.Equals(name))
		{
			return _module;
		}
		return null;
	}

	protected override void SetCustomAttributeCore(ConstructorInfo con, ReadOnlySpan<byte> binaryAttribute)
	{
		if (_customAttributes == null)
		{
			_customAttributes = new List<CustomAttributeWrapper>();
		}
		_customAttributes.Add(new CustomAttributeWrapper(con, binaryAttribute));
	}

	public override AssemblyName GetName(bool copiedName)
	{
		return (AssemblyName)_assemblyName.Clone();
	}
}
