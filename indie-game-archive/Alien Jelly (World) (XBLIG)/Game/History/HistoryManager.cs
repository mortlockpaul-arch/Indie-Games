using System;
using GKEngine.Input;
using Game.QBits;
using Game.Scenes.Play;
using Game.Scenes.Play.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.History;

public class HistoryManager
{
	private const int CLOSED_COUNT = 10000;

	private const int OPEN_COUNT = 500;

	public const float REVERSE_SPEED_TIME = 2500f;

	public const float SLOWMO_SPEED = 1f;

	public const int TIME_REWIND_THRESHOLD = 10;

	public const int TIME_REWIND_START = 1500;

	public const int TIME_REWIND_PADDING = 500;

	private const int AUTO_REWIND_TIMEOUT = 7000;

	public static float[] REVERSE_SPEED = new float[5] { 1f, 1.5f, 2f, 4f, 6f };

	private HistoryItem _itemOpen = new HistoryItem();

	public PlayUniverse universe;

	public Player player;

	public int time;

	public int[] closedIndex = new int[10000];

	public HistoryItem[] closed = new HistoryItem[10000];

	public HistoryItem[] open = new HistoryItem[500];

	public HistoryItem[] reverse = new HistoryItem[500];

	public uint count;

	public bool reversing;

	private int reverseIndex;

	private float reverseSpeed;

	private float reverseSpeedTime;

	private int reverseSpeedIndex;

	private int reverseSpeedCount = REVERSE_SPEED.Length;

	public bool stopping;

	private int reverseAutoTimeOut;

	public HistoryManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		Init();
	}

	private void Init()
	{
		for (int i = 0; i < 10000; i++)
		{
			closed[i] = new HistoryItem();
			closedIndex[i] = i;
		}
		for (int i = 0; i < 500; i++)
		{
			open[i] = new HistoryItem();
		}
		for (int i = 0; i < 500; i++)
		{
			reverse[i] = new HistoryItem();
		}
	}

	public void Update(GameTime oGameTime)
	{
		if (reversing)
		{
			if (player != null && reverseSpeedIndex < reverseSpeedCount - 1)
			{
				reverseSpeedTime += oGameTime.ElapsedGameTime.Milliseconds;
				if (reverseSpeedTime > 2500f)
				{
					reverseSpeedIndex++;
					reverseSpeedTime = 0f;
					reverseSpeed = REVERSE_SPEED[reverseSpeedIndex];
					universe.players.ui.RewindSetSpeed(reverseSpeed);
				}
			}
			time -= (int)((float)oGameTime.ElapsedGameTime.Milliseconds * reverseSpeed);
			time = Math.Max(10, time);
			Reverse_Update(oGameTime);
		}
		else
		{
			time += oGameTime.ElapsedGameTime.Milliseconds;
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < 500; i++)
		{
			open[i] = null;
		}
		for (int i = 0; i < 500; i++)
		{
			reverse[i] = null;
		}
		for (int i = 0; i < 10000; i++)
		{
			closed[i] = null;
		}
	}

	public void Reverse(Player oPlayer)
	{
		if (!reversing && !stopping && time > 1500)
		{
			count++;
			player = oPlayer;
			universe.scene.audio.music.Pause();
			universe.scene.audio.soundRewind.Play();
			universe.scene.postRewind.Anim_In();
			universe.players.Input_Deactivate();
			universe.Event_Reverse_Start();
			if (player != null)
			{
				player.inputReverse.active = true;
			}
			else
			{
				reverseAutoTimeOut = 0;
			}
			CloseAll();
			universe.paused = true;
			time = Reverse_TrimTime_Start();
			reverseSpeedIndex = 0;
			reverseSpeedTime = 0f;
			if (player == null)
			{
				reverseSpeed = 1f;
			}
			else
			{
				reverseSpeed = REVERSE_SPEED[reverseSpeedIndex];
			}
			universe.players.ui.RewindShow(player == null, time, GetFristTime());
			universe.players.ui.RewindSetSpeed(reverseSpeed);
			reverseIndex = 0;
			reversing = true;
		}
	}

	public void Reverse_Update(GameTime oGameTime)
	{
		float num = 0f;
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		if (player == null)
		{
			reverseAutoTimeOut += oGameTime.ElapsedGameTime.Milliseconds;
		}
		universe.players.ui.RewindUpdate(time);
		while (reverseIndex < 10000 && closed[closedIndex[reverseIndex]].end.time > time)
		{
			int freeReverse = GetFreeReverse();
			if (freeReverse == -1)
			{
				break;
			}
			reverse[freeReverse].Copy(ref closed[closedIndex[reverseIndex]]);
			reverse[freeReverse].subject.History_Event_Reverse_Start(ref reverse[freeReverse]);
			reverseIndex++;
		}
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action != HistoryItem.Action.Nothing)
			{
				flag = false;
				if (reverse[i].action == HistoryItem.Action.Death && reverse[i].subject is QBit)
				{
					flag3 = true;
				}
				if (reverse[i].start.time > time)
				{
					reverse[i].subject.History_Reverse(ref reverse[i], 1f, oGameTime);
					reverse[i].subject.History_Event_Reverse_End(ref reverse[i]);
					reverse[i].Clear();
					Reverse_TrimTime();
				}
				else
				{
					num = (float)(time - reverse[i].start.time) / (float)(reverse[i].end.time - reverse[i].start.time);
					num = 1f - num;
					reverse[i].subject.History_Reverse(ref reverse[i], num, oGameTime);
				}
			}
		}
		if (!stopping && !flag3 && ((player == null && flag2) || (player == null && reverseAutoTimeOut >= 7000) || (player == null && universe.players.PressCheck_Reversing_Switch(oGameTime)) || (player != null && player.inputReverse.pressed) || (reverseIndex >= 10000 && flag) || time <= 10))
		{
			stopping = true;
			universe.players.ui.RewindStopping();
		}
		if (stopping && (Reverse_CanStop() || time <= 10))
		{
			Reverse_Stop();
		}
	}

	public void Reverse_Stop()
	{
		universe.players.ui.RewindHide();
		universe.scene.audio.soundRewind.cue.Stop(AudioStopOptions.Immediate);
		universe.scene.audio.music.Resume();
		universe.scene.postRewind.Anim_Out();
		reversing = false;
		ShiftBackClosed();
		Reverse_Resume();
		UniversalInput.FlushStates();
		universe.players.Input_Activate();
		stopping = false;
		universe.paused = false;
	}

	private bool Reverse_CanStop()
	{
		bool result = true;
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action != HistoryItem.Action.Nothing && reverse[i].subject.History_IsNotInteruptable(reverse[i].action))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public void Reverse_Resume()
	{
		HistoryItem oItem = new HistoryItem();
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action != HistoryItem.Action.Nothing)
			{
				Open(reverse[i]);
				oItem.Copy(ref reverse[i]);
				reverse[i].Clear();
				oItem.subject.History_Event_Resume(ref oItem);
				oItem.Clear();
			}
		}
	}

	public void Open(IReversible oSubject, HistoryItem.Action oAction)
	{
		int num = -1;
		if (reversing)
		{
			return;
		}
		num = GetOpen(oAction, oSubject);
		if (num == -1)
		{
			num = GetFreeOpen();
			if (num != -1)
			{
				open[num].action = oAction;
				open[num].subject = oSubject;
				open[num].start.time = time;
				open[num].subject.History_Set(ref open[num].start, oAction);
			}
		}
	}

	public void Open(HistoryItem oItem)
	{
		int num = -1;
		if (reversing)
		{
			return;
		}
		num = GetOpen(oItem.action, oItem.subject);
		if (num == -1)
		{
			num = GetFreeOpen();
			if (num != -1)
			{
				open[num].Copy(ref oItem);
			}
		}
	}

	public void Close(IReversible oSubject, HistoryItem.Action oAction)
	{
		if (!reversing)
		{
			int num = GetOpen(oAction, oSubject);
			if (num >= 0)
			{
				ShiftClosed();
				closed[closedIndex[0]].Copy(ref open[num]);
				closed[closedIndex[0]].end.time = time;
				open[num].Clear();
				oSubject.History_Set(ref closed[closedIndex[0]].end, oAction);
			}
		}
	}

	public void Close(HistoryItem oItem)
	{
		if (!reversing)
		{
			ShiftClosed();
			closed[closedIndex[0]].Copy(ref oItem);
			closed[closedIndex[0]].end.time = time;
			closed[closedIndex[0]].subject.History_Set(ref closed[closedIndex[0]].end, closed[closedIndex[0]].action);
		}
	}

	public void CloseAll()
	{
		for (int i = 0; i < 500; i++)
		{
			if (open[i].action != HistoryItem.Action.Nothing)
			{
				ShiftClosed();
				closed[closedIndex[0]].Copy(ref open[i]);
				closed[closedIndex[0]].end.time = time;
				open[i].Clear();
				closed[closedIndex[0]].subject.History_Set(ref closed[closedIndex[0]].end, closed[closedIndex[0]].action);
				closed[closedIndex[0]].subject.History_Event_ForceClose(ref closed[closedIndex[0]]);
			}
		}
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action != HistoryItem.Action.Nothing)
			{
				ShiftClosed();
				closed[closedIndex[0]].Copy(ref reverse[i]);
				reverse[i].Clear();
			}
		}
	}

	public void Reverse_TrimTime()
	{
		if (IsEmptyReverse() && reverseIndex < 10000 && closed[closedIndex[reverseIndex]].action != HistoryItem.Action.Nothing)
		{
			time = closed[closedIndex[reverseIndex]].end.time + 500;
		}
	}

	public int Reverse_TrimTime_Start()
	{
		int val = time;
		for (int i = 0; i < 10000; i++)
		{
			if (closed[closedIndex[i]].subject is QBit)
			{
				val = closed[closedIndex[i]].end.time;
				break;
			}
		}
		return Math.Min(time, val);
	}

	private int GetFreeOpen()
	{
		int result = -1;
		for (int i = 0; i < 500; i++)
		{
			if (open[i].action == HistoryItem.Action.Nothing)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private int GetFreeReverse()
	{
		int result = -1;
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action == HistoryItem.Action.Nothing)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private int GetOpen(HistoryItem.Action oAction, IReversible oSubject)
	{
		int result = -1;
		for (int i = 0; i < 500; i++)
		{
			if (open[i].subject == oSubject && open[i].action == oAction)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void ShiftClosed()
	{
		int num = closedIndex[9999];
		for (int num2 = 9999; num2 >= 1; num2--)
		{
			closedIndex[num2] = closedIndex[num2 - 1];
		}
		closedIndex[0] = num;
		closed[closedIndex[0]].Clear();
	}

	private void ShiftBackClosed()
	{
		int num = 0;
		reverseIndex = Math.Min(reverseIndex, 9999);
		int[] array = new int[reverseIndex];
		for (int i = 0; i < reverseIndex; i++)
		{
			array[i] = closedIndex[i];
		}
		for (int i = reverseIndex; i < 10000; i++)
		{
			closedIndex[i - reverseIndex] = closedIndex[i];
		}
		for (int i = 10000 - reverseIndex; i < 10000; i++)
		{
			closed[closedIndex[i]].Clear();
			closedIndex[i] = array[num];
			num++;
		}
	}

	private bool IsEmptyReverse()
	{
		bool result = true;
		for (int i = 0; i < 500; i++)
		{
			if (reverse[i].action != HistoryItem.Action.Nothing)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private int GetFristTime()
	{
		int result = 0;
		for (int i = 1; i < 10000; i++)
		{
			if (closed[closedIndex[i]].action == HistoryItem.Action.Nothing)
			{
				result = closed[closedIndex[i - 1]].start.time;
				break;
			}
		}
		return result;
	}
}
