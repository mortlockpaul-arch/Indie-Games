using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Xml.Xsl.IlGen;

internal struct StorageDescriptor
{
	private ItemLocation _location;

	private object _locationObject;

	private Type _itemStorageType;

	private bool _isCached;

	public ItemLocation Location => _location;

	public int ParameterLocation => (int)_locationObject;

	public LocalBuilder LocalLocation => _locationObject as LocalBuilder;

	public CurrentContext CurrentLocation => _locationObject as CurrentContext;

	public MethodInfo GlobalLocation => _locationObject as MethodInfo;

	public bool IsCached => _isCached;

	public Type ItemStorageType => _itemStorageType;

	public static StorageDescriptor None()
	{
		return default(StorageDescriptor);
	}

	public static StorageDescriptor Stack(Type itemStorageType, bool isCached)
	{
		return new StorageDescriptor
		{
			_location = ItemLocation.Stack,
			_itemStorageType = itemStorageType,
			_isCached = isCached
		};
	}

	public static StorageDescriptor Parameter(int paramIndex, Type itemStorageType, bool isCached)
	{
		return new StorageDescriptor
		{
			_location = ItemLocation.Parameter,
			_locationObject = paramIndex,
			_itemStorageType = itemStorageType,
			_isCached = isCached
		};
	}

	[RequiresDynamicCode("Calls System.Type.MakeGenericType")]
	public static StorageDescriptor Local(LocalBuilder loc, Type itemStorageType, bool isCached)
	{
		return new StorageDescriptor
		{
			_location = ItemLocation.Local,
			_locationObject = loc,
			_itemStorageType = itemStorageType,
			_isCached = isCached
		};
	}

	public static StorageDescriptor Current(LocalBuilder locIter, MethodInfo currentMethod, Type itemStorageType)
	{
		return new StorageDescriptor
		{
			_location = ItemLocation.Current,
			_locationObject = new CurrentContext(locIter, currentMethod),
			_itemStorageType = itemStorageType
		};
	}

	[RequiresDynamicCode("Calls System.Type.MakeGenericType")]
	public static StorageDescriptor Global(MethodInfo methGlobal, Type itemStorageType, bool isCached)
	{
		return new StorageDescriptor
		{
			_location = ItemLocation.Global,
			_locationObject = methGlobal,
			_itemStorageType = itemStorageType,
			_isCached = isCached
		};
	}

	public StorageDescriptor ToStack()
	{
		return Stack(_itemStorageType, _isCached);
	}

	[RequiresDynamicCode("Calls StorageDescriptor.Local")]
	public StorageDescriptor ToLocal(LocalBuilder loc)
	{
		return Local(loc, _itemStorageType, _isCached);
	}

	public StorageDescriptor ToStorageType(Type itemStorageType)
	{
		StorageDescriptor result = this;
		result._itemStorageType = itemStorageType;
		return result;
	}
}
