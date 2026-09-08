using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic;

internal struct Money
{
	public enum MoneyType
	{
		Coins,
		Cash
	}

	private MoneyType type;

	private int amount;

	public MoneyType Type => type;

	public int Amount => amount;

	public char MoneyChar
	{
		get
		{
			if (type != MoneyType.Coins)
			{
				return CashChar;
			}
			return CoinChar;
		}
	}

	public static char CoinChar => 'ஸ';

	public static char CashChar => 'ஹ';

	public static AvatarFarmOnline.Logic.Money Parse(XElement xe, string fieldName)
	{
		XElement xe2 = xe.Element(fieldName);
		return new AvatarFarmOnline.Logic.Money
		{
			type = (MoneyType)xe2.ParseIntAttribute("type"),
			amount = xe2.ParseIntAttribute("amount")
		};
	}

	public static AvatarFarmOnline.Logic.Money Parse(XElement xe, string fieldName, AvatarFarmOnline.Logic.Money defaultValue)
	{
		XElement xElement = xe.Element(fieldName);
		if (xElement == null)
		{
			return defaultValue;
		}
		return new AvatarFarmOnline.Logic.Money
		{
			type = (MoneyType)xElement.ParseIntAttribute("type"),
			amount = xElement.ParseIntAttribute("amount")
		};
	}

	public void ToXml(XElement xe, string fieldName)
	{
		XElement xElement = new XElement(fieldName);
		xElement.SetIntAttribute("type", (int)type);
		xElement.SetIntAttribute("amount", amount);
		xe.Add(xElement);
	}

	public Money(MoneyType type, int amount)
	{
		this.type = type;
		this.amount = amount;
	}

	public static AvatarFarmOnline.Logic.Money operator *(AvatarFarmOnline.Logic.Money m, int v)
	{
		return new AvatarFarmOnline.Logic.Money(m.type, m.amount * v);
	}

	public static AvatarFarmOnline.Logic.Money operator /(AvatarFarmOnline.Logic.Money m, int v)
	{
		return new AvatarFarmOnline.Logic.Money(m.type, m.amount / v);
	}

	public static AvatarFarmOnline.Logic.Money operator -(AvatarFarmOnline.Logic.Money m)
	{
		return new AvatarFarmOnline.Logic.Money(m.type, -m.amount);
	}
}
