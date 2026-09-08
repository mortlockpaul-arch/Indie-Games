using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Stage;

internal class PlayerData
{
	private int coins;

	private int cash;

	private int xp;

	private int level = 1;

	private Vector2 position;

	public int Coins => coins;

	public int Cash => cash;

	public int Xp => xp;

	public int XpToNextLevel
	{
		get
		{
			if (level < 100)
			{
				return AvatarFarmOnline.Logic.GameGlobals.FarmLevelUpXp(level);
			}
			return 0;
		}
	}

	public int Level => level;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public int HackCheck => coins * xp * 20 + level * cash * 21;

	public event Action<AvatarFarmOnline.Logic.Stage.PlayerData> OnCoinsChange;

	public event Action<AvatarFarmOnline.Logic.Stage.PlayerData> OnCashChange;

	public event Action<AvatarFarmOnline.Logic.Stage.PlayerData, int, int> OnLevelUp;

	public event Action<AvatarFarmOnline.Logic.Stage.PlayerData> OnXpChange;

	public void IncCoins(int value)
	{
		coins += value;
		if (OnCoinsChange != null)
		{
			OnCoinsChange(this);
		}
	}

	public void DecCoins(int value)
	{
		coins = Math.Max(0, coins - value);
		if (OnCoinsChange != null)
		{
			OnCoinsChange(this);
		}
	}

	public void IncCash(int value)
	{
		cash += value;
		if (OnCashChange != null)
		{
			OnCashChange(this);
		}
	}

	public void DecCash(int value)
	{
		cash = Math.Max(0, cash - value);
		if (OnCashChange != null)
		{
			OnCashChange(this);
		}
	}

	public PlayerData()
	{
		position = AvatarFarmOnline.Logic.GameGlobals.InitialPosition;
	}

	public void IncMoney(AvatarFarmOnline.Logic.Money money)
	{
		if (money.Type == AvatarFarmOnline.Logic.Money.MoneyType.Coins)
		{
			IncCoins(money.Amount);
		}
		else
		{
			IncCash(money.Amount);
		}
	}

	public void Init()
	{
		coins = 2000;
		cash = 10;
		xp = 0;
		level = 1;
	}

	public bool TryBuy(AvatarFarmOnline.Logic.Money money)
	{
		if (money.Type == AvatarFarmOnline.Logic.Money.MoneyType.Cash)
		{
			if (money.Amount > cash)
			{
				return false;
			}
			DecCash(money.Amount);
			return true;
		}
		if (money.Amount > coins)
		{
			return false;
		}
		DecCoins(money.Amount);
		return true;
	}

	public bool CanBuy(AvatarFarmOnline.Logic.Money money)
	{
		if (money.Type == AvatarFarmOnline.Logic.Money.MoneyType.Cash)
		{
			return cash >= money.Amount;
		}
		return coins >= money.Amount;
	}

	public bool IncXp(int xp)
	{
		this.xp += xp;
		bool result = false;
		if (level < 100)
		{
			while (this.xp >= XpToNextLevel)
			{
				LevelUp();
				result = true;
				if (level >= 100)
				{
					break;
				}
			}
		}
		if (OnXpChange != null)
		{
			OnXpChange(this);
		}
		return result;
	}

	public void LevelUp()
	{
		level++;
		int num = AvatarFarmOnline.Logic.GameGlobals.LevelUpCashReward(level);
		IncCash(num);
		int num2 = AvatarFarmOnline.Logic.GameGlobals.LevelUpCoinsReward(level);
		IncCoins(num2);
		if (OnLevelUp != null)
		{
			OnLevelUp(this, num2, num);
		}
	}

	public void FromXml(XElement xe)
	{
		coins = xe.ParseIntAttribute("coins");
		cash = xe.ParseIntAttribute("cash");
		xp = xe.ParseIntAttribute("xp");
		level = xe.ParseIntAttribute("level");
		position = xe.ParseVector2Attribute("position");
	}

	public void ToXml(XElement xe)
	{
		xe.SetIntAttribute("coins", coins);
		xe.SetIntAttribute("cash", cash);
		xe.SetIntAttribute("xp", xp);
		xe.SetIntAttribute("level", level);
		xe.SetVector2Attribute("position", position);
	}

	public void WritePacket(IPacketWriter writer)
	{
		writer.Write(coins);
		writer.Write(cash);
		writer.Write(xp);
		writer.Write((short)level);
	}

	public void ReceiveData(IPacketReader reader)
	{
		coins = reader.ReadInt32();
		cash = reader.ReadInt32();
		xp = reader.ReadInt32();
		level = reader.ReadInt16();
	}
}
