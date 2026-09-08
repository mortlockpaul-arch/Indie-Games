using System.Reflection;

namespace Microsoft.VisualBasic.CompilerServices;

internal interface IRecordEnum
{
	bool Callback(FieldInfo FieldInfo, ref object Value);
}
