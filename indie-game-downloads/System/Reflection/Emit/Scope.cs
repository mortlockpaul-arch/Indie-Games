using System.Collections.Generic;

namespace System.Reflection.Emit;

internal sealed class Scope
{
	internal Scope _parent;

	internal List<Scope> _children;

	internal List<LocalBuilder> _locals;

	internal List<string> _importNamespaces;

	internal int _startOffset;

	internal int _endOffset;

	internal Scope(int offset, Scope parent)
	{
		_startOffset = offset;
		_parent = parent;
	}

	internal List<LocalBuilder> GetAllLocals()
	{
		List<LocalBuilder> list = new List<LocalBuilder>();
		if (_locals != null)
		{
			list.AddRange(_locals);
		}
		if (_children != null)
		{
			foreach (Scope child in _children)
			{
				list.AddRange(child.GetAllLocals());
			}
		}
		return list;
	}
}
