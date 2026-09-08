using System;
using System.Collections;
using System.Collections.Generic;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Network;
using Quasar.Global;

namespace Quasar.GameUtils.XBLIG.Network;

public class XBLIGNetworkInterface : NetworkInterface
{
	private sealed class EmptyAvailableSessionCollection : IAvailableSessionCollection
	{
		public IEnumerable<IAvailableSession> Sessions
		{
			get
			{
				yield break;
			}
		}

		public IAvailableSession this[int index] => null;

		public int Count => 0;
	}

	private sealed class BasicSessionProperties : ISessionProperties, IEnumerable<int?>, IEnumerable
	{
		private readonly int?[] values = new int?[8];

		public int Count => values.Length;

		public int? this[int index]
		{
			get
			{
				return values[index];
			}
			set
			{
				values[index] = value;
			}
		}

		public IEnumerator<int?> GetEnumerator()
		{
			int?[] array = values;
			for (int i = 0; i < array.Length; i++)
			{
				yield return array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private static XBLIGNetworkInterface stubInstance;

	public override int FoundSessions => 0;

	public static void Init(XNAGame game)
	{
		if (stubInstance == null)
		{
			stubInstance = new XBLIGNetworkInterface();
			NetworkInterface.instance = stubInstance;
		}
	}

	public override void RegisterInviteAccepted(Action<object, InviteAcceptedArgs> eventHandler)
	{
	}

	public override Session JoinInvited(List<ISignedInGamer> gamers)
	{
		return null;
	}

	public override IAvailableSessionCollection Find(SessionType sessionType, List<ISignedInGamer> players, ISessionProperties properties)
	{
		return new EmptyAvailableSessionCollection();
	}

	public override Session Create(SessionType sessionType, List<ISignedInGamer> players, int playerNumber, int privateSlots, ISessionProperties properties)
	{
		return null;
	}

	public override Session Join(IAvailableSession availableSession)
	{
		return null;
	}

	public override Session JoinByAddress(List<ISignedInGamer> players, string text)
	{
		return null;
	}

	public override ISessionProperties CreateSessionProperties()
	{
		return new BasicSessionProperties();
	}
}
