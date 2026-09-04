using System;

namespace RuntimeXNA.Services;

public class CFuncVal
{
	public int intValue;

	public double doubleValue;

	public virtual int parse(string s)
	{
		try
		{
			if (s.Length >= 3)
			{
				if (s[0] == '0' && (s[1] == 'x' || s[2] == 'X'))
				{
					string value = s.Substring(2);
					try
					{
						intValue = Convert.ToInt32(value, 16);
					}
					catch (FormatException ex)
					{
						ex.GetType();
					}
					catch (ArgumentOutOfRangeException ex2)
					{
						ex2.GetType();
					}
					return 0;
				}
				if (s[0] == '0' && (s[1] == 'b' || s[2] == 'B'))
				{
					string value = s.Substring(2);
					try
					{
						intValue = Convert.ToInt32(value, 2);
					}
					catch (FormatException ex3)
					{
						ex3.GetType();
					}
					catch (ArgumentOutOfRangeException ex4)
					{
						ex4.GetType();
					}
					return 0;
				}
			}
			double num = 0.0;
			if (s.Length > 0)
			{
				try
				{
					num = double.Parse(s);
				}
				catch (FormatException ex5)
				{
					ex5.GetType();
				}
				catch (OverflowException ex6)
				{
					ex6.GetType();
				}
			}
			double num2 = Math.Round(num);
			if (num - num2 != 0.0)
			{
				doubleValue = num;
				return 1;
			}
			intValue = (int)num;
			return 0;
		}
		catch (FormatException ex7)
		{
			ex7.GetType();
		}
		intValue = 0;
		return 0;
	}
}
