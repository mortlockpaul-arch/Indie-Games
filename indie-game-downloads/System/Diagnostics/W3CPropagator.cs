using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Diagnostics;

internal sealed class W3CPropagator : DistributedContextPropagator
{
	private static readonly SearchValues<char> s_validBaggageKeyChars = SearchValues.Create("!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ^_`abcdefghijklmnopqrstuvwxyz|~".AsSpan());

	private static readonly SearchValues<char> s_validTraceStateChars = SearchValues.Create("*-/0123456789@_abcdefghijklmnopqrstuvwxyz".AsSpan());

	private static readonly SearchValues<char> s_validTraceStateValueChars = SearchValues.Create("!\"#$%&'()*+-./0123456789:;<>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~".AsSpan());

	private static ulong[] s_baggageOctet = new ulong[2] { 17870265566011326464uL, 9223372036586340351uL };

	private static ulong[] s_traceParentMask = new ulong[2] { 287948901175001088uL, 541165879296uL };

	internal static DistributedContextPropagator Instance { get; } = new W3CPropagator();

	public override IReadOnlyCollection<string> Fields { get; } = new ReadOnlyCollection<string>(new string[4] { "traceparent", "tracestate", "baggage", "Correlation-Context" });

	public override void Inject(Activity activity, object carrier, PropagatorSetterCallback setter)
	{
		if (activity == null || setter == null || activity.IdFormat != ActivityIdFormat.W3C)
		{
			return;
		}
		string id = activity.Id;
		if (id != null)
		{
			setter(carrier, "traceparent", id);
			string traceStateString = activity.TraceStateString;
			if (traceStateString != null && traceStateString.Length > 0)
			{
				InjectTraceState(traceStateString, carrier, setter);
			}
			InjectW3CBaggage(carrier, activity.Baggage, setter);
		}
	}

	public override void ExtractTraceIdAndState(object carrier, PropagatorGetterCallback getter, out string traceId, out string traceState)
	{
		if (getter == null)
		{
			traceId = null;
			traceState = null;
			return;
		}
		getter(carrier, "traceparent", out traceId, out var fieldValues);
		if (IsInvalidTraceParent(traceId))
		{
			traceId = null;
		}
		getter(carrier, "tracestate", out var fieldValue, out fieldValues);
		traceState = ValidateTraceState(fieldValue);
	}

	public override IEnumerable<KeyValuePair<string, string>> ExtractBaggage(object carrier, PropagatorGetterCallback getter)
	{
		if (getter == null)
		{
			return null;
		}
		getter(carrier, "baggage", out var fieldValue, out var fieldValues);
		if (fieldValue == null)
		{
			getter(carrier, "Correlation-Context", out fieldValue, out fieldValues);
		}
		TryExtractBaggage(fieldValue, out var baggage);
		return baggage;
	}

	internal static bool TryExtractBaggage(string baggageString, out IEnumerable<KeyValuePair<string, string>> baggage)
	{
		baggage = null;
		List<KeyValuePair<string, string>> list = null;
		if (string.IsNullOrEmpty(baggageString))
		{
			return true;
		}
		ReadOnlySpan<char> readOnlySpan = baggageString.AsSpan();
		do
		{
			int num = readOnlySpan.IndexOf(',');
			ReadOnlySpan<char> span = ((num >= 0) ? readOnlySpan.Slice(0, num) : readOnlySpan);
			int num2 = span.IndexOf('=');
			if (num2 <= 0 || num2 >= span.Length - 1)
			{
				break;
			}
			ReadOnlySpan<char> keySpan = span.Slice(0, num2);
			ReadOnlySpan<char> valueSpan = span.Slice(num2 + 1);
			if (TryDecodeBaggageKey(keySpan, out var key) && TryDecodeBaggageValue(valueSpan, out var value))
			{
				if (list == null)
				{
					list = new List<KeyValuePair<string, string>>();
				}
				list.Add(new KeyValuePair<string, string>(key, value));
			}
			readOnlySpan = ((num >= 0) ? readOnlySpan.Slice(num + 1) : ReadOnlySpan<char>.Empty);
		}
		while (readOnlySpan.Length > 0);
		list?.Reverse();
		baggage = list;
		return list != null;
	}

	internal static string ValidateTraceState(string traceState)
	{
		if (string.IsNullOrEmpty(traceState))
		{
			return null;
		}
		int i;
		int num2;
		for (i = 0; i < traceState.Length; i += num2)
		{
			ReadOnlySpan<char> readOnlySpan = traceState.AsSpan(i);
			int num = readOnlySpan.IndexOf(',');
			ReadOnlySpan<char> span = ((num >= 0) ? readOnlySpan.Slice(0, num) : readOnlySpan);
			num2 = span.Length + ((num >= 0) ? 1 : 0);
			if (i + num2 > 256)
			{
				break;
			}
			int num3 = span.IndexOf('=');
			if (num3 <= 0 || num3 >= span.Length - 1 || IsInvalidTraceStateKey(Trim(span.Slice(0, num3))) || IsInvalidTraceStateValue(TrimSpaceOnly(span.Slice(num3 + 1))))
			{
				break;
			}
		}
		if (i > 0)
		{
			if (traceState[i - 1] == ',')
			{
				i--;
			}
			if (i > 0)
			{
				if (i < traceState.Length)
				{
					return traceState.AsSpan(0, i).ToString();
				}
				return traceState;
			}
		}
		return null;
	}

	internal static void InjectTraceState(string traceState, object carrier, PropagatorSetterCallback setter)
	{
		string text = ValidateTraceState(traceState);
		if (text != null)
		{
			setter(carrier, "tracestate", text);
		}
	}

	internal static void InjectW3CBaggage(object carrier, IEnumerable<KeyValuePair<string, string>> baggage, PropagatorSetterCallback setter)
	{
		using IEnumerator<KeyValuePair<string, string>> enumerator = baggage.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return;
		}
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder vsb = new System.Text.ValueStringBuilder(initialBuffer);
		int num = 0;
		int num2 = 0;
		do
		{
			KeyValuePair<string, string> current = enumerator.Current;
			if (EncodeBaggageKey(current.Key.AsSpan(), ref vsb))
			{
				vsb.Append(' ');
				vsb.Append('=');
				vsb.Append(' ');
				if (!string.IsNullOrEmpty(current.Value))
				{
					EncodeBaggageValue(current.Value.AsSpan(), ref vsb);
				}
				vsb.Append(", ");
				num++;
				if (vsb.Length < 8192)
				{
					num2 = vsb.Length;
				}
			}
		}
		while (enumerator.MoveNext() && num < 64 && vsb.Length < 8192);
		if (num2 - 2 > 0)
		{
			setter(carrier, "baggage", vsb.AsSpan(0, num2 - 2).ToString());
		}
		vsb.Dispose();
	}

	private static bool TryDecodeBaggageKey(ReadOnlySpan<char> keySpan, out string key)
	{
		key = null;
		keySpan = Trim(keySpan);
		if (keySpan.IsEmpty || IsInvalidBaggageKey(keySpan))
		{
			return false;
		}
		key = keySpan.ToString();
		return true;
	}

	private static bool TryDecodeBaggageValue(ReadOnlySpan<char> valueSpan, out string value)
	{
		value = null;
		valueSpan = Trim(valueSpan);
		Span<char> initialBuffer = stackalloc char[128];
		using System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		for (int i = 0; i < valueSpan.Length; i++)
		{
			char c = valueSpan[i];
			if (c > '\u007f')
			{
				return false;
			}
			if (c != '%')
			{
				valueStringBuilder.Append(c);
				continue;
			}
			if (!TryDecodeEscapedByte(valueSpan.Slice(i), out var value2))
			{
				return false;
			}
			if (value2 <= 127)
			{
				valueStringBuilder.Append((char)value2);
				i += 2;
				continue;
			}
			switch (value2)
			{
			case 194:
			case 195:
			case 196:
			case 197:
			case 198:
			case 199:
			case 200:
			case 201:
			case 202:
			case 203:
			case 204:
			case 205:
			case 206:
			case 207:
			case 208:
			case 209:
			case 210:
			case 211:
			case 212:
			case 213:
			case 214:
			case 215:
			case 216:
			case 217:
			case 218:
			case 219:
			case 220:
			case 221:
			case 222:
			case 223:
			{
				if (i + 5 >= valueSpan.Length || valueSpan[i + 3] != '%' || !TryDecodeEscapedByte(valueSpan.Slice(i + 3), out var value6) || (value6 & 0xC0) != 128)
				{
					valueStringBuilder.Append('\ufffd');
					i += 2;
				}
				else
				{
					valueStringBuilder.Append((char)(((value2 & 0x1F) << 6) | (value6 & 0x3F)));
					i += 5;
				}
				break;
			}
			case 224:
			case 225:
			case 226:
			case 227:
			case 228:
			case 229:
			case 230:
			case 231:
			case 232:
			case 233:
			case 234:
			case 235:
			case 236:
			case 237:
			case 238:
			case 239:
			{
				if (i + 8 >= valueSpan.Length || valueSpan[i + 3] != '%' || valueSpan[i + 6] != '%' || !TryDecodeEscapedByte(valueSpan.Slice(i + 3), out var value7) || !TryDecodeEscapedByte(valueSpan.Slice(i + 6), out var value8) || (value2 == 224 && value7 < 160) || (value2 == 237 && value7 >= 160))
				{
					valueStringBuilder.Append('\ufffd');
					i += 2;
				}
				else
				{
					valueStringBuilder.Append((char)(((value2 & 0xF) << 12) | ((value7 & 0x3F) << 6) | (value8 & 0x3F)));
					i += 8;
				}
				break;
			}
			case 240:
			case 241:
			case 242:
			case 243:
			case 244:
			{
				if (i + 11 >= valueSpan.Length || valueSpan[i + 3] != '%' || valueSpan[i + 6] != '%' || valueSpan[i + 9] != '%' || !TryDecodeEscapedByte(valueSpan.Slice(i + 3), out var value3) || !TryDecodeEscapedByte(valueSpan.Slice(i + 6), out var value4) || !TryDecodeEscapedByte(valueSpan.Slice(i + 9), out var value5) || (value3 & 0xC0) != 128 || (value4 & 0xC0) != 128 || (value5 & 0xC0) != 128)
				{
					valueStringBuilder.Append('\ufffd');
					i += 2;
					break;
				}
				int num = ((value2 & 7) << 18) | ((value3 & 0x3F) << 12) | ((value4 & 0x3F) << 6) | (value5 & 0x3F);
				if (num < 65536 || num > 1114111)
				{
					valueStringBuilder.Append('\ufffd');
					i += 2;
					break;
				}
				num -= 65536;
				valueStringBuilder.Append((char)((num >> 10) + 55296));
				valueStringBuilder.Append((char)((num & 0x3FF) + 56320));
				i += 11;
				break;
			}
			default:
				valueStringBuilder.Append('\ufffd');
				i += 2;
				break;
			}
		}
		value = valueStringBuilder.ToString();
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryDecodeEscapedByte(ReadOnlySpan<char> span, out byte value)
	{
		if (span.Length < 3 || !TryDecodeHexDigit(span[1], out var value2) || !TryDecodeHexDigit(span[2], out var value3))
		{
			value = 0;
			return false;
		}
		value = (byte)((value2 << 4) + value3);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool TryDecodeHexDigit(char c, out byte value)
	{
		value = (byte)System.HexConverter.FromChar(c);
		return value != byte.MaxValue;
	}

	private static bool IsInvalidBaggageKey(ReadOnlySpan<char> span)
	{
		return span.ContainsAnyExcept(s_validBaggageKeyChars);
	}

	private static bool IsInvalidTraceStateKey(ReadOnlySpan<char> key)
	{
		if (!key.IsEmpty && ((uint)(122 - key[0]) <= 25u || (uint)(57 - key[0]) <= 9u))
		{
			return key.ContainsAnyExcept(s_validTraceStateChars);
		}
		return true;
	}

	private static bool IsInvalidTraceStateValue(ReadOnlySpan<char> value)
	{
		if (!value.IsEmpty)
		{
			return value.ContainsAnyExcept(s_validTraceStateValueChars);
		}
		return true;
	}

	internal static bool EncodeBaggageKey(ReadOnlySpan<char> key, ref System.Text.ValueStringBuilder vsb)
	{
		key = Trim(key);
		if (key.IsEmpty || IsInvalidBaggageKey(key))
		{
			return false;
		}
		vsb.Append(key);
		return true;
	}

	internal static void EncodeBaggageValue(ReadOnlySpan<char> value, ref System.Text.ValueStringBuilder vsb)
	{
		value = Trim(value);
		for (int i = 0; i < value.Length; i++)
		{
			char c = value[i];
			if (!NeedToEscapeBaggageValueCharacter(c))
			{
				vsb.Append(c);
			}
			else if ((uint)c <= 127u)
			{
				EmitEscapedByte((byte)c, ref vsb);
			}
			else if ((uint)c <= 2047u)
			{
				EmitEscapedByte((byte)((uint)(c + 12288) >> 6), ref vsb);
				EmitEscapedByte((byte)((c & 0x3F) + 128), ref vsb);
			}
			else if (char.IsSurrogate(c))
			{
				if (i < value.Length - 1 && char.IsSurrogatePair(c, value[i + 1]))
				{
					int num = char.ConvertToUtf32(c, value[i + 1]);
					EmitEscapedByte((byte)((uint)(num + 62914560) >> 18), ref vsb);
					EmitEscapedByte((byte)(((num & 0x3F000) >>> 12) + 128), ref vsb);
					EmitEscapedByte((byte)(((num & 0xFC0) >>> 6) + 128), ref vsb);
					EmitEscapedByte((byte)((num & 0x3F) + 128), ref vsb);
					i++;
				}
				else
				{
					EmitEscapedByte(239, ref vsb);
					EmitEscapedByte(191, ref vsb);
					EmitEscapedByte(189, ref vsb);
				}
			}
			else
			{
				EmitEscapedByte((byte)(c + 917504 >> 12), ref vsb);
				EmitEscapedByte((byte)(((c & 0xFC0) >>> 6) + 128), ref vsb);
				EmitEscapedByte((byte)((c & 0x3F) + 128), ref vsb);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool NeedToEscapeBaggageValueCharacter(char c)
	{
		if (c >= '\u007f')
		{
			return true;
		}
		return (s_baggageOctet[(int)c >> 6] & (ulong)(1L << (int)c)) == 0;
	}

	private static bool IsInvalidTraceParent(string traceParent)
	{
		if (string.IsNullOrEmpty(traceParent) || traceParent.Length < 55)
		{
			return true;
		}
		if ((traceParent[0] == 'f' && traceParent[1] == 'f') || IsInvalidTraceParentCharacter(traceParent[0]) || IsInvalidTraceParentCharacter(traceParent[1]))
		{
			return true;
		}
		if (traceParent[0] == '0' && traceParent[1] == '0')
		{
			if (traceParent.Length != 55)
			{
				return true;
			}
		}
		else if (traceParent.Length > 55 && traceParent[55] != '-')
		{
			return true;
		}
		if (traceParent[2] != '-' || traceParent[35] != '-' || traceParent[52] != '-')
		{
			return true;
		}
		bool flag = true;
		for (int i = 3; i < 35; i++)
		{
			if (IsInvalidTraceParentCharacter(traceParent[i]))
			{
				return true;
			}
			flag &= traceParent[i] == '0';
		}
		if (flag)
		{
			return true;
		}
		flag = true;
		for (int j = 36; j < 52; j++)
		{
			if (IsInvalidTraceParentCharacter(traceParent[j]))
			{
				return true;
			}
			flag &= traceParent[j] == '0';
		}
		if (flag)
		{
			return true;
		}
		if (IsInvalidTraceParentCharacter(traceParent[53]) || IsInvalidTraceParentCharacter(traceParent[54]))
		{
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsInvalidTraceParentCharacter(char c)
	{
		if (c >= '\u007f')
		{
			return true;
		}
		return (s_traceParentMask[(int)c >> 6] & (ulong)(1L << (int)c)) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void EmitEscapedByte(byte b, ref System.Text.ValueStringBuilder vsb)
	{
		vsb.Append('%');
		vsb.Append("0123456789ABCDEF"[(b >> 4) & 0xF]);
		vsb.Append("0123456789ABCDEF"[b & 0xF]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> TrimSpaceOnly(ReadOnlySpan<char> span)
	{
		return span.Trim(' ');
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<char> Trim(ReadOnlySpan<char> span)
	{
		return span.Trim(" \t".AsSpan());
	}
}
