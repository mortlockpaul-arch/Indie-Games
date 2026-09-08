using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization;

internal class CodeObject
{
	[CompilerGenerated]
	private IDictionary _003CUserData_003Ek__BackingField;

	public IDictionary UserData => _003CUserData_003Ek__BackingField ?? (_003CUserData_003Ek__BackingField = new ListDictionary());
}
