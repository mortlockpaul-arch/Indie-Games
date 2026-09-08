namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class PhoneAttribute : DataTypeAttribute
{
	public PhoneAttribute()
		: base(DataType.PhoneNumber)
	{
		base.DefaultErrorMessage = System.SR.PhoneAttribute_Invalid;
	}

	public override bool IsValid(object? value)
	{
		if (value == null)
		{
			return true;
		}
		if (!(value is string text))
		{
			return false;
		}
		ReadOnlySpan<char> potentialPhoneNumber = text.Replace("+", string.Empty).AsSpan().TrimEnd();
		potentialPhoneNumber = RemoveExtension(potentialPhoneNumber);
		bool flag = false;
		ReadOnlySpan<char> readOnlySpan = potentialPhoneNumber;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (char.IsDigit(readOnlySpan[i]))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		ReadOnlySpan<char> readOnlySpan2 = potentialPhoneNumber;
		for (int i = 0; i < readOnlySpan2.Length; i++)
		{
			char c = readOnlySpan2[i];
			if (!char.IsDigit(c) && !char.IsWhiteSpace(c) && !"-.()".Contains(c))
			{
				return false;
			}
		}
		return true;
	}

	private static ReadOnlySpan<char> RemoveExtension(ReadOnlySpan<char> potentialPhoneNumber)
	{
		int num = potentialPhoneNumber.LastIndexOf("ext.".AsSpan(), StringComparison.OrdinalIgnoreCase);
		if (num >= 0 && MatchesExtension(potentialPhoneNumber.Slice(num + "ext.".Length)))
		{
			return potentialPhoneNumber.Slice(0, num);
		}
		num = potentialPhoneNumber.LastIndexOf("ext".AsSpan(), StringComparison.OrdinalIgnoreCase);
		if (num >= 0 && MatchesExtension(potentialPhoneNumber.Slice(num + "ext".Length)))
		{
			return potentialPhoneNumber.Slice(0, num);
		}
		num = potentialPhoneNumber.LastIndexOf("x".AsSpan(), StringComparison.OrdinalIgnoreCase);
		if (num >= 0 && MatchesExtension(potentialPhoneNumber.Slice(num + "x".Length)))
		{
			return potentialPhoneNumber.Slice(0, num);
		}
		return potentialPhoneNumber;
	}

	private static bool MatchesExtension(ReadOnlySpan<char> potentialExtension)
	{
		potentialExtension = potentialExtension.TrimStart();
		if (potentialExtension.Length == 0)
		{
			return false;
		}
		ReadOnlySpan<char> readOnlySpan = potentialExtension;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (!char.IsDigit(readOnlySpan[i]))
			{
				return false;
			}
		}
		return true;
	}
}
