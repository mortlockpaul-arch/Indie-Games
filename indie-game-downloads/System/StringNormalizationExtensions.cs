using System.Globalization;
using System.Text;

namespace System;

public static class StringNormalizationExtensions
{
	public static bool IsNormalized(this string strInput)
	{
		return IsNormalized(strInput, NormalizationForm.FormC);
	}

	public static bool IsNormalized(this string strInput, NormalizationForm normalizationForm)
	{
		ArgumentNullException.ThrowIfNull(strInput, "strInput");
		return strInput.IsNormalized(normalizationForm);
	}

	public static bool IsNormalized(this ReadOnlySpan<char> source, NormalizationForm normalizationForm = NormalizationForm.FormC)
	{
		return Normalization.IsNormalized(source, normalizationForm);
	}

	public static string Normalize(this string strInput)
	{
		return Normalize(strInput, NormalizationForm.FormC);
	}

	public static string Normalize(this string strInput, NormalizationForm normalizationForm)
	{
		ArgumentNullException.ThrowIfNull(strInput, "strInput");
		return strInput.Normalize(normalizationForm);
	}

	public static bool TryNormalize(this ReadOnlySpan<char> source, Span<char> destination, out int charsWritten, NormalizationForm normalizationForm = NormalizationForm.FormC)
	{
		return Normalization.TryNormalize(source, destination, out charsWritten, normalizationForm);
	}

	public static int GetNormalizedLength(this ReadOnlySpan<char> source, NormalizationForm normalizationForm = NormalizationForm.FormC)
	{
		return Normalization.GetNormalizedLength(source, normalizationForm);
	}
}
