using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.VisualBasic.CompilerServices;

internal sealed class VBGetMemberBinder : GetMemberBinder, IInvokeOnGetBinder
{
	private static readonly int s_hash = typeof(VBGetMemberBinder).GetHashCode();

	private bool InvokeOnGet => false;

	public VBGetMemberBinder(string name)
		: base(name, ignoreCase: true)
	{
	}

	public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
	{
		if (errorSuggestion != null)
		{
			return errorSuggestion;
		}
		return new DynamicMetaObject(Expression.Constant(IDOBinder.missingMemberSentinel), IDOUtils.CreateRestrictions(target));
	}

	public override bool Equals(object _other)
	{
		if (_other is VBGetMemberBinder vBGetMemberBinder)
		{
			return string.Equals(base.Name, vBGetMemberBinder.Name);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return s_hash ^ base.Name.GetHashCode();
	}
}
