using System.Buffers;
using System.Text;

namespace System.Net.WebSockets;

internal static class WebSocketValidate
{
	private static readonly SearchValues<char> s_validSubprotocolChars = SearchValues.Create("!#$%&'*+-.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ^_`abcdefghijklmnopqrstuvwxyz|~".AsSpan());

	internal static void ValidateSubprotocol(string subProtocol)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(subProtocol, "subProtocol");
		int num = subProtocol.AsSpan().IndexOfAnyExcept(s_validSubprotocolChars);
		if (num >= 0)
		{
			char c = subProtocol[num];
			string p = (char.IsBetween(c, '!', '~') ? c.ToString() : $"[{c}]");
			throw new ArgumentException(System.SR.Format(System.SR.net_WebSockets_InvalidCharInProtocolString, subProtocol, p), "subProtocol");
		}
	}

	internal static void ValidateCloseStatus(WebSocketCloseStatus closeStatus, string statusDescription)
	{
		if (closeStatus == WebSocketCloseStatus.Empty && !string.IsNullOrEmpty(statusDescription))
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_WebSockets_ReasonNotNull, statusDescription, WebSocketCloseStatus.Empty), "statusDescription");
		}
		if ((closeStatus >= (WebSocketCloseStatus)0 && closeStatus <= (WebSocketCloseStatus)999) || closeStatus == (WebSocketCloseStatus)1006 || closeStatus == (WebSocketCloseStatus)1015)
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_WebSockets_InvalidCloseStatusCode, (int)closeStatus), "closeStatus");
		}
		if (!string.IsNullOrEmpty(statusDescription) && Encoding.UTF8.GetMaxByteCount(statusDescription.Length) > 123 && Encoding.UTF8.GetByteCount(statusDescription) > 123)
		{
			throw new ArgumentException(System.SR.Format(System.SR.net_WebSockets_InvalidCloseStatusDescription, statusDescription, 123), "statusDescription");
		}
	}

	internal static void ValidateArraySegment(ArraySegment<byte> arraySegment, string parameterName)
	{
		if (arraySegment.Array == null)
		{
			throw new ArgumentNullException(parameterName + ".Array");
		}
		if (arraySegment.Offset < 0 || arraySegment.Offset > arraySegment.Array.Length)
		{
			throw new ArgumentOutOfRangeException(parameterName + ".Offset");
		}
		if (arraySegment.Count < 0 || arraySegment.Count > arraySegment.Array.Length - arraySegment.Offset)
		{
			throw new ArgumentOutOfRangeException(parameterName + ".Count");
		}
	}
}
