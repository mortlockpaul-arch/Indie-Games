using System;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class Financial
{
	public static double DDB(double Cost, double Salvage, double Life, double Period, double Factor = 2.0)
	{
		if (Factor <= 0.0 || Salvage < 0.0 || Period <= 0.0 || Period > Life)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Factor"));
		}
		if (Cost <= 0.0)
		{
			return 0.0;
		}
		if (Life < 2.0)
		{
			return Cost - Salvage;
		}
		if (Life == 2.0 && Period > 1.0)
		{
			return 0.0;
		}
		if (Life == 2.0 && Period <= 1.0)
		{
			return Cost - Salvage;
		}
		double num2;
		double num;
		if (Period <= 1.0)
		{
			num = Cost * Factor / Life;
			num2 = Cost - Salvage;
			if (num > num2)
			{
				return num2;
			}
			return num;
		}
		num2 = (Life - Factor) / Life;
		double y = Period - 1.0;
		num = Factor * Cost / Life * Math.Pow(num2, y);
		double num3 = Cost * (1.0 - Math.Pow(num2, Period)) - Cost + Salvage;
		if (num3 > 0.0)
		{
			num -= num3;
		}
		if (num >= 0.0)
		{
			return num;
		}
		return 0.0;
	}

	public static double FV(double Rate, double NPer, double Pmt, double PV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		return FV_Internal(Rate, NPer, Pmt, PV, Due);
	}

	private static double FV_Internal(double Rate, double NPer, double Pmt, double PV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		if (Rate == 0.0)
		{
			return 0.0 - PV - Pmt * NPer;
		}
		double num = ((Due == DueDate.EndOfPeriod) ? 1.0 : (1.0 + Rate));
		double num2 = Math.Pow(1.0 + Rate, NPer);
		return (0.0 - PV) * num2 - Pmt / Rate * num * (num2 - 1.0);
	}

	public static double IPmt(double Rate, double Per, double NPer, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		double num = ((Due == DueDate.EndOfPeriod) ? 1.0 : 2.0);
		if (Per <= 0.0 || Per >= NPer + 1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Per"));
		}
		if (Due != DueDate.EndOfPeriod && Per == 1.0)
		{
			return 0.0;
		}
		double num2 = PMT_Internal(Rate, NPer, PV, FV, Due);
		if (Due != DueDate.EndOfPeriod)
		{
			PV += num2;
		}
		return FV_Internal(Rate, Per - num, num2, PV) * Rate;
	}

	public static double IRR(ref double[] ValueArray, double Guess = 0.1)
	{
		int upperBound;
		try
		{
			upperBound = ValueArray.GetUpperBound(0);
		}
		catch (StackOverflowException ex)
		{
			throw ex;
		}
		catch (OutOfMemoryException ex2)
		{
			throw ex2;
		}
		catch (Exception)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "ValueArray"));
		}
		checked
		{
			int num = upperBound + 1;
			if (Guess <= -1.0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Guess"));
			}
			if (num <= 1)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "ValueArray"));
			}
			double num2 = ((!(ValueArray[0] > 0.0)) ? (0.0 - ValueArray[0]) : ValueArray[0]);
			int num3 = upperBound;
			int i;
			for (i = 0; i <= num3; i++)
			{
				if (ValueArray[i] > num2)
				{
					num2 = ValueArray[i];
				}
				else if (0.0 - ValueArray[i] > num2)
				{
					num2 = 0.0 - ValueArray[i];
				}
			}
			double num4 = num2 * 1E-07 * 0.01;
			double num5 = Guess;
			double num6 = OptPV2(ref ValueArray, num5);
			double num7 = ((!(num6 > 0.0)) ? (num5 - 1E-05) : (num5 + 1E-05));
			if (num7 <= -1.0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Rate"));
			}
			double num8 = OptPV2(ref ValueArray, num7);
			i = 0;
			do
			{
				if (num8 == num6)
				{
					num5 = ((!(num7 > num5)) ? (num5 + 1E-05) : (num5 - 1E-05));
					num6 = OptPV2(ref ValueArray, num5);
					if (num8 == num6)
					{
						throw new ArgumentException(System.SR.Argument_InvalidValue);
					}
				}
				num5 = num7 - (num7 - num5) * num8 / (num8 - num6);
				if (num5 <= -1.0)
				{
					num5 = (num7 - 1.0) * 0.5;
				}
				num6 = OptPV2(ref ValueArray, num5);
				num2 = ((!(num5 > num7)) ? (num7 - num5) : (num5 - num7));
				double num9 = ((!(num6 > 0.0)) ? (0.0 - num6) : num6);
				if (num9 < num4 && num2 < 1E-07)
				{
					return num5;
				}
				num2 = num6;
				num6 = num8;
				num8 = num2;
				num2 = num5;
				num5 = num7;
				num7 = num2;
				i++;
			}
			while (i <= 39);
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
	}

	public static double MIRR(ref double[] ValueArray, double FinanceRate, double ReinvestRate)
	{
		if (ValueArray.Rank != 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RankEQOne1, "ValueArray"));
		}
		int num = 0;
		int num2 = checked(ValueArray.GetUpperBound(0) - num + 1);
		if (FinanceRate == -1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "FinanceRate"));
		}
		if (ReinvestRate == -1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "ReinvestRate"));
		}
		if (num2 <= 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "ValueArray"));
		}
		double num3 = LDoNPV(FinanceRate, ref ValueArray, -1);
		if (num3 == 0.0)
		{
			throw new DivideByZeroException(System.SR.Financial_CalcDivByZero);
		}
		double num4 = LDoNPV(ReinvestRate, ref ValueArray, 1);
		double x = ReinvestRate + 1.0;
		double y = num2;
		double num5 = (0.0 - num4) * Math.Pow(x, y) / (num3 * (FinanceRate + 1.0));
		if (num5 < 0.0)
		{
			throw new ArgumentException(System.SR.Argument_InvalidValue);
		}
		x = 1.0 / ((double)num2 - 1.0);
		return Math.Pow(num5, x) - 1.0;
	}

	public static double NPer(double Rate, double Pmt, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		if (Rate <= -1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Rate"));
		}
		if (Rate == 0.0)
		{
			if (Pmt == 0.0)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Pmt"));
			}
			return (0.0 - (PV + FV)) / Pmt;
		}
		double num = ((Due == DueDate.EndOfPeriod) ? (Pmt / Rate) : (Pmt * (1.0 + Rate) / Rate));
		double num2 = 0.0 - FV + num;
		double num3 = PV + num;
		if (num2 < 0.0 && num3 < 0.0)
		{
			num2 = -1.0 * num2;
			num3 = -1.0 * num3;
		}
		else if (num2 <= 0.0 || num3 <= 0.0)
		{
			throw new ArgumentException(System.SR.Financial_CannotCalculateNPer);
		}
		double d = Rate + 1.0;
		return (Math.Log(num2) - Math.Log(num3)) / Math.Log(d);
	}

	public static double NPV(double Rate, ref double[] ValueArray)
	{
		if (ValueArray == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidNullValue1, "ValueArray"));
		}
		if (ValueArray.Rank != 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RankEQOne1, "ValueArray"));
		}
		int num = 0;
		int num2 = checked(ValueArray.GetUpperBound(0) - num + 1);
		if (Rate == -1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Rate"));
		}
		if (num2 < 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "ValueArray"));
		}
		return LDoNPV(Rate, ref ValueArray, 0);
	}

	public static double Pmt(double Rate, double NPer, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		return PMT_Internal(Rate, NPer, PV, FV, Due);
	}

	private static double PMT_Internal(double Rate, double NPer, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		if (NPer == 0.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "NPer"));
		}
		if (Rate == 0.0)
		{
			return (0.0 - FV - PV) / NPer;
		}
		double num = ((Due == DueDate.EndOfPeriod) ? 1.0 : (1.0 + Rate));
		double num2 = Math.Pow(Rate + 1.0, NPer);
		return (0.0 - FV - PV * num2) / (num * (num2 - 1.0)) * Rate;
	}

	public static double PPmt(double Rate, double Per, double NPer, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		if (Per <= 0.0 || Per >= NPer + 1.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.PPMT_PerGT0AndLTNPer, "Per"));
		}
		double num = PMT_Internal(Rate, NPer, PV, FV, Due);
		double num2 = IPmt(Rate, Per, NPer, PV, FV, Due);
		return num - num2;
	}

	public static double PV(double Rate, double NPer, double Pmt, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod)
	{
		if (Rate == 0.0)
		{
			return 0.0 - FV - Pmt * NPer;
		}
		double num = ((Due == DueDate.EndOfPeriod) ? 1.0 : (1.0 + Rate));
		double num2 = Math.Pow(1.0 + Rate, NPer);
		return (0.0 - (FV + Pmt * num * ((num2 - 1.0) / Rate))) / num2;
	}

	public static double Rate(double NPer, double Pmt, double PV, double FV = 0.0, DueDate Due = DueDate.EndOfPeriod, double Guess = 0.1)
	{
		if (NPer <= 0.0)
		{
			throw new ArgumentException(System.SR.Rate_NPerMustBeGTZero);
		}
		double num = Guess;
		double num2 = LEvalRate(num, NPer, Pmt, PV, FV, Due);
		double num3 = ((!(num2 > 0.0)) ? (num * 2.0) : (num / 2.0));
		double num4 = LEvalRate(num3, NPer, Pmt, PV, FV, Due);
		int num5 = 0;
		do
		{
			if (num4 == num2)
			{
				num = ((!(num3 > num)) ? (num - -1E-05) : (num - 1E-05));
				num2 = LEvalRate(num, NPer, Pmt, PV, FV, Due);
				if (num4 == num2)
				{
					throw new ArgumentException(System.SR.Financial_CalcDivByZero);
				}
			}
			num = num3 - (num3 - num) * num4 / (num4 - num2);
			num2 = LEvalRate(num, NPer, Pmt, PV, FV, Due);
			if (Math.Abs(num2) < 1E-07)
			{
				return num;
			}
			double num6 = num2;
			num2 = num4;
			num4 = num6;
			double num7 = num;
			num = num3;
			num3 = num7;
			num5 = checked(num5 + 1);
		}
		while (num5 <= 39);
		throw new ArgumentException(System.SR.Financial_CannotCalculateRate);
	}

	public static double SLN(double Cost, double Salvage, double Life)
	{
		if (Life == 0.0)
		{
			throw new ArgumentException(System.SR.Financial_LifeNEZero);
		}
		return (Cost - Salvage) / Life;
	}

	public static double SYD(double Cost, double Salvage, double Life, double Period)
	{
		if (Salvage < 0.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Financial_ArgGEZero1, "Salvage"));
		}
		if (Period > Life)
		{
			throw new ArgumentException(System.SR.Financial_PeriodLELife);
		}
		if (Period <= 0.0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Financial_ArgGTZero1, "Period"));
		}
		return (Cost - Salvage) / (Life * (Life + 1.0)) * (Life + 1.0 - Period) * 2.0;
	}

	private static double LEvalRate(double Rate, double NPer, double Pmt, double PV, double dFv, DueDate Due)
	{
		if (Rate == 0.0)
		{
			return PV + Pmt * NPer + dFv;
		}
		double num = Math.Pow(Rate + 1.0, NPer);
		double num2 = ((Due == DueDate.EndOfPeriod) ? 1.0 : (1.0 + Rate));
		return PV * num + Pmt * num2 * (num - 1.0) / Rate + dFv;
	}

	private static double LDoNPV(double Rate, ref double[] ValueArray, int iWNType)
	{
		bool flag = iWNType < 0;
		bool flag2 = iWNType > 0;
		double num = 1.0;
		double num2 = 0.0;
		int upperBound = ValueArray.GetUpperBound(0);
		int num3 = upperBound;
		for (int i = 0; i <= num3; i = checked(i + 1))
		{
			double num4 = ValueArray[i];
			num += num * Rate;
			if ((!flag || !(num4 > 0.0)) && (!flag2 || !(num4 < 0.0)))
			{
				num2 += num4 / num;
			}
		}
		return num2;
	}

	private static double OptPV2(ref double[] ValueArray, double Guess = 0.1)
	{
		int i = 0;
		int upperBound = ValueArray.GetUpperBound(0);
		double num = 0.0;
		double num2 = 1.0 + Guess;
		checked
		{
			for (; i <= upperBound && ValueArray[i] == 0.0; i++)
			{
			}
			int num3 = i;
			for (int j = upperBound; j >= num3; j += -1)
			{
				num /= num2;
				num += ValueArray[j];
			}
			return num;
		}
	}
}
