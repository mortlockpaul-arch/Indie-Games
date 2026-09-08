using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization;

public class ExportOptions
{
	[CompilerGenerated]
	private Collection<Type> _003CKnownTypes_003Ek__BackingField;

	public ISerializationSurrogateProvider? DataContractSurrogate { get; set; }

	public Collection<Type> KnownTypes => _003CKnownTypes_003Ek__BackingField ?? (_003CKnownTypes_003Ek__BackingField = new Collection<Type>());
}
