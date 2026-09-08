using System;
using System.ComponentModel;

namespace Microsoft.VisualBasic;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class MyGroupCollectionAttribute : Attribute
{
	public string MyGroupName { get; }

	public string CreateMethod { get; }

	public string DisposeMethod { get; }

	public string DefaultInstanceAlias { get; }

	public MyGroupCollectionAttribute(string typeToCollect, string createInstanceMethodName, string disposeInstanceMethodName, string defaultInstanceAlias)
	{
		MyGroupName = typeToCollect;
		CreateMethod = createInstanceMethodName;
		DisposeMethod = disposeInstanceMethodName;
		DefaultInstanceAlias = defaultInstanceAlias;
	}
}
