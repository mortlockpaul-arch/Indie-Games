using System.Text;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Expressions;

public class EXP_FLOATTOSTRING : CExp
{
	public override void evaluate(CRun rhPtr)
	{
		rhPtr.rh4CurToken++;
		double expressionDouble = rhPtr.get_ExpressionDouble();
		rhPtr.rh4CurToken++;
		int expressionInt = rhPtr.get_ExpressionInt();
		if (expressionInt < 1)
		{
			expressionInt = 1;
		}
		rhPtr.rh4CurToken++;
		int expressionInt2 = rhPtr.get_ExpressionInt();
		string text = expressionDouble.ToString();
		StringBuilder stringBuilder = new StringBuilder();
		int num = text.IndexOf('.');
		if (num >= 0)
		{
			int i;
			for (i = num + 1; i < text.Length && text[i] == '0'; i++)
			{
			}
			if (i == text.Length)
			{
				num = -1;
			}
		}
		int j = 0;
		if (num >= 0)
		{
			if (expressionDouble < 0.0)
			{
				stringBuilder.Append("-");
				j++;
			}
			for (; j < num; j++)
			{
				stringBuilder.Append(text[j]);
			}
			if (expressionInt2 > 0)
			{
				stringBuilder.Append(".");
				j++;
				for (int i = 0; i < expressionInt2 && i + j < text.Length; i++)
				{
					stringBuilder.Append(text[j + i]);
				}
			}
			else if (expressionInt2 < 0)
			{
				stringBuilder.Append(".");
				for (j++; j < text.Length; j++)
				{
					stringBuilder.Append(text[j]);
				}
			}
		}
		else
		{
			for (; j < text.Length && text[j] != '.'; j++)
			{
				stringBuilder.Append(text[j]);
			}
			if (expressionInt2 > 0)
			{
				stringBuilder.Append(".");
				for (int i = 0; i < expressionInt2; i++)
				{
					stringBuilder.Append("0");
				}
			}
		}
		rhPtr.getCurrentResult().forceString(new string(stringBuilder.ToString().ToCharArray()));
	}
}
