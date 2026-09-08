namespace Microsoft.CSharp.RuntimeBinder.Semantics;

internal sealed class ExpressionIterator
{
	private ExprList _pList;

	private Expr _pCurrent;

	public ExpressionIterator(Expr pExpr)
	{
		Init(pExpr);
	}

	public bool AtEnd()
	{
		if (_pCurrent == null)
		{
			return _pList == null;
		}
		return false;
	}

	public Expr Current()
	{
		return _pCurrent;
	}

	public void MoveNext()
	{
		if (!AtEnd())
		{
			if (_pList == null)
			{
				_pCurrent = null;
			}
			else
			{
				Init(_pList.OptionalNextListNode);
			}
		}
	}

	public static int Count(Expr pExpr)
	{
		int num = 0;
		ExpressionIterator expressionIterator = new ExpressionIterator(pExpr);
		while (!expressionIterator.AtEnd())
		{
			num++;
			expressionIterator.MoveNext();
		}
		return num;
	}

	private void Init(Expr pExpr)
	{
		if (pExpr == null)
		{
			_pList = null;
			_pCurrent = null;
		}
		else if (pExpr is ExprList exprList)
		{
			_pList = exprList;
			_pCurrent = exprList.OptionalElement;
		}
		else
		{
			_pList = null;
			_pCurrent = pExpr;
		}
	}
}
