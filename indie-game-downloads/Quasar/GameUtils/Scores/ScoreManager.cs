using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using Quasar.GameUtils.Logic;
using Quasar.GameUtils.Storage;
using Quasar.GameUtils.Tasks;
using Quasar.Global;

namespace Quasar.GameUtils.Scores;

public abstract class ScoreManager<T> where T : Highscore, IEquatable<T>, IComparable<T>, new()
{
	private const int SAVE_REFRESH_TIME = 10000;

	private Dictionary<string, List<T>> userScores = new Dictionary<string, List<T>>();

	private List<T>[] aggregateHighscores;

	private List<T>[] localAggregateHighscores;

	private List<T>[] altAggregateHighscores;

	private List<T>[] altLocalAggregateHighscores;

	private DateTime[] previousDates;

	private ScoreOrganizer<T> scoreOrganizer;

	private List<T> pendingNewScores;

	private List<T> pendingRemoveScores;

	private bool aggregateScoresPending;

	private bool saveSoon;

	private bool userWantsToLoad = true;

	protected bool storeLocalData = true;

	protected bool storeGlobalData;

	protected bool dataLoaded;

	protected bool supportsPeriods = true;

	private int sortTaskId;

	private int saveTaskId;

	private int loadTaskId;

	private int saveRefreshTime;

	private StringBuilder saveBuilder = new StringBuilder(128);

	public Dictionary<string, List<T>> UserScores => userScores;

	public ScoreOrganizer<T> ScoreOrganizer => scoreOrganizer;

	public List<T> PendingNewScores => pendingNewScores;

	public List<T> PendingRemoveScores => pendingRemoveScores;

	public bool AggregateScoresPending => aggregateScoresPending;

	public bool SaveSoon => saveSoon;

	public bool SupportsPeriods => supportsPeriods;

	public int SortTaskId => sortTaskId;

	public int SaveTaskId => saveTaskId;

	public int LoadTaskId => loadTaskId;

	public int SaveRefreshTime => saveRefreshTime;

	protected bool AreTasksRunning
	{
		get
		{
			if (sortTaskId == 0 && saveTaskId == 0 && loadTaskId == 0)
			{
				return saveRefreshTime > 0;
			}
			return true;
		}
	}

	protected bool AreNewScoresPending => pendingNewScores.Count > 0;

	protected virtual bool CanStartTasks => true;

	public event Action OnHighscoresChanged;

	public List<T> AggregateHighscores(ScorePeriod period)
	{
		return aggregateHighscores[(int)period];
	}

	public List<T> LocalAggregateHighscores(ScorePeriod period)
	{
		return localAggregateHighscores[(int)period];
	}

	protected ScoreManager(ScoreOrganizer<T> organizer)
	{
		scoreOrganizer = organizer;
		scoreOrganizer.ScoreManager = this;
		aggregateHighscores = new List<T>[4];
		localAggregateHighscores = new List<T>[4];
		altAggregateHighscores = new List<T>[4];
		altLocalAggregateHighscores = new List<T>[4];
		previousDates = new DateTime[4];
		pendingNewScores = new List<T>(organizer.MaxEntryCount(ScorePeriod.AllTime) / 4);
		pendingRemoveScores = new List<T>();
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			aggregateHighscores[i] = new List<T>(organizer.MaxEntryCount((ScorePeriod)i) / 2);
			localAggregateHighscores[i] = new List<T>(50);
			altAggregateHighscores[i] = new List<T>(organizer.MaxEntryCount((ScorePeriod)i) / 2);
			altLocalAggregateHighscores[i] = new List<T>(50);
			num += organizer.MaxEntryCount((ScorePeriod)i);
		}
		Pool<T>.SetCapacity(num * 3 / 2);
	}

	protected void AddScore(T score)
	{
		pendingNewScores.Add(score);
	}

	protected virtual void SetNewScore(T score)
	{
		AddScore(score);
	}

	protected T CreateEmptyHighScore()
	{
		return Pool<T>.Fetch();
	}

	protected void HighscoreRemoved(T hs)
	{
		Pool<T>.Insert(hs);
	}

	public void RemoveScore(T hs)
	{
		pendingRemoveScores.Add(hs);
	}

	private void LoadData(IsolatedStorageFile container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		try
		{
			if (storeGlobalData && container.FileExists("remote scores.dat"))
			{
				using Stream stream = container.OpenFile("remote scores.dat", FileMode.Open);
				using StreamReader streamReader = new StreamReader(stream);
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine();
					if (text == "")
					{
						break;
					}
					T hs = CreateEmptyHighScore();
					try
					{
						if (hs.Decode(text))
						{
							AddScore(hs, save: false);
						}
					}
					catch (Exception)
					{
					}
				}
			}
			if (storeLocalData && container.FileExists("local scores.dat"))
			{
				using Stream stream2 = container.OpenFile("local scores.dat", FileMode.Open);
				using StreamReader streamReader2 = new StreamReader(stream2);
				while (!streamReader2.EndOfStream)
				{
					string text2 = streamReader2.ReadLine();
					if (!(text2 == ""))
					{
						T val = CreateEmptyHighScore();
						if (val.Decode(text2))
						{
							val.IsLocal = true;
							AddScore(val, save: false);
						}
						continue;
					}
					break;
				}
			}
		}
		catch (Exception)
		{
		}
		dataLoaded = true;
	}

	private void SaveData(IsolatedStorageFile container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		try
		{
			if (storeGlobalData)
			{
				using Stream stream = container.OpenFile("remote scores.dat", FileMode.Create);
				using StreamWriter streamWriter = new StreamWriter(stream);
				foreach (KeyValuePair<string, List<T>> userScore in userScores)
				{
					foreach (T item in userScore.Value)
					{
						T current = item;
						if (!current.IsFake)
						{
							saveBuilder.Length = 0;
							current.Encode(saveBuilder);
							for (int i = 0; i < saveBuilder.Length; i++)
							{
								streamWriter.Write(saveBuilder[i]);
							}
							streamWriter.WriteLine();
						}
					}
				}
				streamWriter.WriteLine();
			}
			if (!storeLocalData)
			{
				return;
			}
			using Stream stream2 = container.OpenFile("local scores.dat", FileMode.Create);
			using StreamWriter streamWriter2 = new StreamWriter(stream2);
			foreach (KeyValuePair<string, List<T>> userScore2 in userScores)
			{
				foreach (T item2 in userScore2.Value)
				{
					T current2 = item2;
					if (current2.IsLocal && !current2.IsFake)
					{
						saveBuilder.Length = 0;
						current2.Encode(saveBuilder);
						for (int j = 0; j < saveBuilder.Length; j++)
						{
							streamWriter2.Write(saveBuilder[j]);
						}
						streamWriter2.WriteLine();
					}
				}
			}
			streamWriter2.WriteLine();
		}
		catch (Exception)
		{
		}
	}

	protected void AddScore(T hs, bool save)
	{
		if (!userScores.ContainsKey(hs.Gamer))
		{
			userScores.Add(hs.Gamer, new List<T>(2));
		}
		bool flag = false;
		foreach (T item in userScores[hs.Gamer])
		{
			if (item.Equals(hs))
			{
				flag = item.IsLocal || !hs.IsLocal;
				break;
			}
		}
		if (!flag)
		{
			userScores[hs.Gamer].Add(hs);
			aggregateScoresPending = true;
			if (save)
			{
				saveSoon = true;
			}
		}
		else
		{
			RemoveScore(hs);
		}
	}

	public virtual void Update()
	{
		if (saveRefreshTime > 0)
		{
			saveRefreshTime = Math.Max(0, saveRefreshTime - Timer.DefaultTimer.LastInterval);
		}
		if (loadTaskId != 0 || saveTaskId != 0 || sortTaskId != 0 || saveRefreshTime != 0)
		{
			return;
		}
		int num = Math.Max(0, pendingNewScores.Count - 20);
		while (pendingNewScores.Count > num)
		{
			int index = pendingNewScores.Count - 1;
			T val = pendingNewScores[index];
			if (val != null)
			{
				AddScore(val, save: true);
			}
			pendingNewScores.RemoveAt(index);
		}
		int num2 = Math.Max(0, pendingRemoveScores.Count - 80);
		while (pendingRemoveScores.Count > num2)
		{
			int index2 = pendingRemoveScores.Count - 1;
			T hs = pendingRemoveScores[index2];
			HighscoreRemoved(hs);
			pendingRemoveScores.RemoveAt(index2);
		}
		if (!CanStartTasks || pendingRemoveScores.Count != 0)
		{
			return;
		}
		if (userWantsToLoad)
		{
			if (storeGlobalData || storeLocalData)
			{
				loadTaskId = StorageManager.Instance.Post(LoadData, null, StorageManager.IOType.Read, OnLoadEnded);
			}
			userWantsToLoad = false;
		}
		else
		{
			if (pendingNewScores.Count != 0)
			{
				return;
			}
			if (aggregateScoresPending)
			{
				AggregateScores();
			}
			else if (saveSoon && StorageManager.Instance.SaveEnabled)
			{
				saveSoon = false;
				if (storeGlobalData || storeLocalData)
				{
					saveTaskId = StorageManager.Instance.Post(SaveData, null, StorageManager.IOType.WriteNotImportant, OnSaveEnded);
				}
			}
		}
	}

	private void AggregateScores()
	{
		aggregateScoresPending = false;
		sortTaskId = TaskManager.Post(_AggregateScores, null, OnAggregateScoresTaskFinished);
	}

	private void _AggregateScores(object parameters)
	{
		foreach (KeyValuePair<string, List<T>> userScore in userScores)
		{
			if (userScore.Value.Count <= 0)
			{
				continue;
			}
			if (userScore.Value.Count > 1)
			{
				userScore.Value.Sort();
			}
			T val = userScore.Value[0];
			int i = 1;
			for (int num = userScore.Value.Count; i != num; i++)
			{
				T val2 = userScore.Value[i];
				if (val.Equals((Highscore)val2))
				{
					if (!val2.IsLocal)
					{
						RemoveScore(val2);
						userScore.Value.RemoveAt(i);
					}
					else
					{
						RemoveScore(userScore.Value[i - 1]);
						userScore.Value.RemoveAt(i - 1);
						val = val2;
					}
					i--;
					num--;
				}
				else
				{
					val = val2;
				}
			}
			scoreOrganizer.CheckPrune(userScore.Value, ScorePeriod.AllTime, isAggregate: false);
		}
		for (int j = 0; j < 4; j++)
		{
			switch (j)
			{
			default:
			{
				ref DateTime reference4 = ref previousDates[j];
				reference4 = DateTime.MinValue;
				break;
			}
			case 3:
			{
				ref DateTime reference3 = ref previousDates[j];
				reference3 = DateTime.Now.Subtract(TimeSpan.FromDays(1.0));
				break;
			}
			case 2:
			{
				ref DateTime reference2 = ref previousDates[j];
				reference2 = DateTime.Now.Subtract(TimeSpan.FromDays(7.0));
				break;
			}
			case 1:
			{
				ref DateTime reference = ref previousDates[j];
				reference = DateTime.Now.Subtract(TimeSpan.FromDays(30.0));
				break;
			}
			}
			altAggregateHighscores[j].Clear();
			altLocalAggregateHighscores[j].Clear();
		}
		foreach (KeyValuePair<string, List<T>> userScore2 in userScores)
		{
			foreach (T item in userScore2.Value)
			{
				for (int k = 0; k < 4; k++)
				{
					if (!(item.When < previousDates[k]))
					{
						altAggregateHighscores[k].Add(item);
						if (item.IsLocal)
						{
							altLocalAggregateHighscores[k].Add(item);
						}
					}
				}
			}
		}
		for (int l = 0; l < 4; l++)
		{
			if (altAggregateHighscores[l].Count > 0)
			{
				altAggregateHighscores[l].Sort();
				scoreOrganizer.CheckPrune(altAggregateHighscores[l], (ScorePeriod)l, isAggregate: true);
			}
			altLocalAggregateHighscores[l].Sort();
		}
	}

	private void OnAggregateScoresTaskFinished(object parameters)
	{
		for (int i = 0; i < 4; i++)
		{
			List<T> list = aggregateHighscores[i];
			aggregateHighscores[i] = altAggregateHighscores[i];
			altAggregateHighscores[i] = list;
			list = localAggregateHighscores[i];
			localAggregateHighscores[i] = altLocalAggregateHighscores[i];
			altLocalAggregateHighscores[i] = list;
		}
		if (OnHighscoresChanged != null)
		{
			OnHighscoresChanged();
		}
		sortTaskId = 0;
	}

	public void Save()
	{
		if ((storeLocalData || storeGlobalData) && saveTaskId == 0)
		{
			StorageManager.Instance.Exec(SaveData, null, StorageManager.IOType.Write);
		}
	}

	private void OnLoadEnded(object parameters)
	{
		loadTaskId = 0;
	}

	private void OnSaveEnded(object parameters)
	{
		saveTaskId = 0;
		saveRefreshTime = 10000;
	}

	public virtual void Dispose()
	{
	}
}
