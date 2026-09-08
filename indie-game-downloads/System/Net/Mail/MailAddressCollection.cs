using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace System.Net.Mail;

public class MailAddressCollection : Collection<MailAddress>
{
	public void Add(string addresses)
	{
		ArgumentException.ThrowIfNullOrEmpty(addresses, "addresses");
		ParseValue(addresses);
	}

	protected override void SetItem(int index, MailAddress item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		base.SetItem(index, item);
	}

	protected override void InsertItem(int index, MailAddress item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		base.InsertItem(index, item);
	}

	internal void ParseValue(string addresses)
	{
		List<MailAddress> list = MailAddressParser.ParseMultipleAddresses(addresses);
		for (int i = 0; i < list.Count; i++)
		{
			Add(list[i]);
		}
	}

	public override string ToString()
	{
		return string.Join(", ", this);
	}

	internal string Encode(int charsConsumed, bool allowUnicode)
	{
		Span<char> initialBuffer = stackalloc char[256];
		System.Text.ValueStringBuilder valueStringBuilder = new System.Text.ValueStringBuilder(initialBuffer);
		using (IEnumerator<MailAddress> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MailAddress current = enumerator.Current;
				if (valueStringBuilder.Length == 0)
				{
					valueStringBuilder.Append(current.Encode(charsConsumed, allowUnicode));
					continue;
				}
				valueStringBuilder.Append(", ");
				valueStringBuilder.Append(current.Encode(1, allowUnicode));
			}
		}
		return valueStringBuilder.ToString();
	}
}
