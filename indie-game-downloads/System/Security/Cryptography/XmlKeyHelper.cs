using System.Buffers.Binary;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Security.Cryptography;

internal static class XmlKeyHelper
{
	internal struct ParseState
	{
		private static class Functions
		{
			[UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "Parse")]
			[return: UnsafeAccessorType("System.Xml.Linq.XDocument, System.Private.Xml.Linq")]
			private static extern object XDocument_Parse([UnsafeAccessorType("System.Xml.Linq.XDocument, System.Private.Xml.Linq")] object _, string xmlString);

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Root")]
			[return: UnsafeAccessorType("System.Xml.Linq.XElement, System.Private.Xml.Linq")]
			private static extern object XDocument_GetRoot([UnsafeAccessorType("System.Xml.Linq.XDocument, System.Private.Xml.Linq")] object xDocument);

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Elements")]
			[return: UnsafeAccessorType("System.Collections.Generic.IEnumerable`1[[System.Xml.Linq.XElement, System.Private.Xml.Linq]], System.Runtime")]
			private static extern object XContainer_Elements([UnsafeAccessorType("System.Xml.Linq.XContainer, System.Private.Xml.Linq")] object xElement);

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Name")]
			[return: UnsafeAccessorType("System.Xml.Linq.XName, System.Private.Xml.Linq")]
			private static extern object XElement_GetName([UnsafeAccessorType("System.Xml.Linq.XElement, System.Private.Xml.Linq")] object xElement);

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Value")]
			private static extern string XElement_GetValue([UnsafeAccessorType("System.Xml.Linq.XElement, System.Private.Xml.Linq")] object xElement);

			[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_LocalName")]
			private static extern string XName_GetLocalName([UnsafeAccessorType("System.Xml.Linq.XName, System.Private.Xml.Linq")] object xName);

			internal static object ParseDocument(string xmlString)
			{
				return XDocument_GetRoot(XDocument_Parse(null, xmlString));
			}

			internal static IEnumerable GetElements(object element)
			{
				return (IEnumerable)XContainer_Elements(element);
			}

			internal static string GetLocalName(object element)
			{
				return XName_GetLocalName(XElement_GetName(element));
			}

			internal static string GetValue(object element)
			{
				return XElement_GetValue(element);
			}
		}

		private IEnumerable _enumerable;

		private IEnumerator _enumerator;

		private int _index;

		internal static ParseState ParseDocument(string xmlString)
		{
			object element = Functions.ParseDocument(xmlString);
			return new ParseState
			{
				_enumerable = Functions.GetElements(element),
				_enumerator = null,
				_index = -1
			};
		}

		internal bool HasElement(string localName)
		{
			bool num = GetValue(localName) != null;
			if (num)
			{
				_index--;
			}
			return num;
		}

		internal string GetValue(string localName)
		{
			if (_enumerable == null)
			{
				return null;
			}
			if (_enumerator == null)
			{
				_enumerator = _enumerable.GetEnumerator();
			}
			int index = _index;
			int num = index;
			if (!_enumerator.MoveNext())
			{
				num = -1;
				_enumerator = _enumerable.GetEnumerator();
				if (!_enumerator.MoveNext())
				{
					_enumerable = null;
					return null;
				}
			}
			for (num++; num != index; num++)
			{
				string localName2 = Functions.GetLocalName(_enumerator.Current);
				if (localName == localName2)
				{
					_index = num;
					return Functions.GetValue(_enumerator.Current);
				}
				if (!_enumerator.MoveNext())
				{
					num = -1;
					if (index < 0)
					{
						_enumerator = null;
						return null;
					}
					_enumerator = _enumerable.GetEnumerator();
					if (!_enumerator.MoveNext())
					{
						_enumerable = null;
						return null;
					}
				}
			}
			return null;
		}
	}

	internal static ParseState ParseDocument(string xmlString)
	{
		ArgumentNullException.ThrowIfNull(xmlString, "xmlString");
		try
		{
			return ParseState.ParseDocument(xmlString);
		}
		catch (Exception inner)
		{
			throw new CryptographicException(System.SR.Cryptography_FromXmlParseError, inner);
		}
	}

	internal static bool HasElement(ref ParseState state, string name)
	{
		return state.HasElement(name);
	}

	internal static byte[] ReadCryptoBinary(ref ParseState state, string name, int sizeHint = -1)
	{
		string value = state.GetValue(name);
		if (value == null)
		{
			return null;
		}
		if (value.Length == 0)
		{
			return Array.Empty<byte>();
		}
		if (sizeHint < 0)
		{
			return Convert.FromBase64String(value);
		}
		byte[] array = new byte[sizeHint];
		if (Convert.TryFromBase64Chars(value.AsSpan(), array, out var bytesWritten))
		{
			if (bytesWritten == sizeHint)
			{
				return array;
			}
			int num = sizeHint - bytesWritten;
			Buffer.BlockCopy(array, 0, array, num, bytesWritten);
			array.AsSpan(0, num).Clear();
			return array;
		}
		return Convert.FromBase64String(value);
	}

	internal static int ReadCryptoBinaryInt32(byte[] buf)
	{
		int num = 0;
		for (int i = Math.Max(0, buf.Length - 4); i < buf.Length; i++)
		{
			num <<= 8;
			num |= buf[i];
		}
		return num;
	}

	internal static void WriteCryptoBinary(string name, int value, StringBuilder builder)
	{
		if (value == 0)
		{
			WriteCryptoBinary(name, new ReadOnlySpan<byte>((byte)0), builder);
			return;
		}
		Span<byte> destination = stackalloc byte[4];
		BinaryPrimitives.WriteInt32BigEndian(destination, value);
		int i;
		for (i = 0; destination[i] == 0; i++)
		{
		}
		WriteCryptoBinary(name, destination.Slice(i), builder);
	}

	internal static void WriteCryptoBinary(string name, ReadOnlySpan<byte> value, StringBuilder builder)
	{
		builder.Append('<');
		builder.Append(name);
		builder.Append('>');
		int num = 0;
		int num2 = value.Length;
		Span<char> chars = stackalloc char[256];
		while (num2 > 0)
		{
			int num3 = Math.Min(192, num2);
			if (!Convert.TryToBase64Chars(value.Slice(num, num3), chars, out var charsWritten))
			{
				throw new CryptographicException();
			}
			builder.Append(chars.Slice(0, charsWritten));
			num2 -= num3;
			num += num3;
		}
		builder.Append('<');
		builder.Append('/');
		builder.Append(name);
		builder.Append('>');
	}
}
