#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using RacingGame.GameScreens;
using RacingGame.Helpers;
using RacingGame.Tracks;

namespace RacingGame.GameLogic;

public class Replay : ICloneable
{
	public const float TrackMatrixIntervals = 0.2f;

	private static readonly string[] ReplayFilenames = new string[3] { "TrackBeginner.Replay", "TrackAdvanced.Replay", "TrackExpert.Replay" };

	private int trackNum;

	private float lapTime;

	private List<Matrix> trackMatrixValues;

	private List<float> checkpointTimes;

	public int TrackNumber => trackNum;

	public float LapTime
	{
		get
		{
			return lapTime;
		}
		set
		{
			lapTime = value;
		}
	}

	public int NumberOfTrackMatrices => trackMatrixValues.Count;

	public List<float> CheckpointTimes => checkpointTimes;

	public Matrix GetCarMatrixAtTime(float trackTime)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (trackMatrixValues.Count < 2)
		{
			return Matrix.Identity;
		}
		if (trackTime <= 0f)
		{
			return trackMatrixValues[0];
		}
		int num = (int)(trackTime / 0.2f);
		float num2 = (trackTime - (float)num * 0.2f) / 0.2f;
		if (num < 0)
		{
			num = 0;
		}
		if (num > trackMatrixValues.Count - 2)
		{
			return trackMatrixValues[0];
		}
		return Matrix.Lerp(trackMatrixValues[num], trackMatrixValues[num + 1], num2);
	}

	public Replay(int setTrackNum, bool createNew, Track track)
	{
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		trackMatrixValues = new List<Matrix>();
		checkpointTimes = new List<float>();
		base._002Ector();
		trackNum = setTrackNum;
		if (createNew)
		{
			return;
		}
		bool flag = false;
		FileHelper.StorageContainerMRE.WaitOne();
		FileHelper.StorageContainerMRE.Reset();
		try
		{
			StorageDevice xnaUserDevice = FileHelper.XnaUserDevice;
			if (xnaUserDevice != null && xnaUserDevice.IsConnected)
			{
				StorageContainer val = xnaUserDevice.OpenContainer("RacingGame");
				try
				{
					string path = Path.Combine(val.Path, ReplayFilenames[trackNum]);
					if (File.Exists(path))
					{
						flag = true;
						using FileStream input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						using BinaryReader binaryReader = new BinaryReader(input);
						lapTime = binaryReader.ReadSingle();
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							trackMatrixValues.Add(FileHelper.ReadMatrix(binaryReader));
						}
						int num2 = binaryReader.ReadInt32();
						for (int j = 0; j < num2; j++)
						{
							checkpointTimes.Add(binaryReader.ReadSingle());
						}
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Settings Load Failure: " + ex.ToString());
		}
		FileHelper.StorageContainerMRE.Set();
		if (!flag && File.Exists(Path.Combine(Directories.ContentDirectory, ReplayFilenames[trackNum])))
		{
			using FileStream input2 = FileHelper.LoadGameContentFile("Content\\" + ReplayFilenames[trackNum]);
			using BinaryReader binaryReader2 = new BinaryReader(input2);
			lapTime = binaryReader2.ReadSingle();
			int num3 = binaryReader2.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				trackMatrixValues.Add(FileHelper.ReadMatrix(binaryReader2));
			}
			int num4 = binaryReader2.ReadInt32();
			for (int l = 0; l < num4; l++)
			{
				checkpointTimes.Add(binaryReader2.ReadSingle());
			}
		}
		if (flag)
		{
			return;
		}
		lapTime = Highscores.GetTopLapTime(trackNum);
		int num5 = 1 + (int)(lapTime / 0.2f);
		float num6 = 0f;
		int num7 = 0;
		for (int m = 0; m < num5 * 2; m++)
		{
			float num8 = 1E-05f + (float)m / (float)(num5 - 1);
			float num9 = num8 - num6;
			num8 = num6 + num9 * 0.1f;
			num6 = num8;
			Matrix trackPositionMatrix = track.GetTrackPositionMatrix(num8, out var _, out var _);
			trackMatrixValues.Add(trackPositionMatrix);
			int num10 = (int)(num8 * (float)track.NumberOfSegments);
			if (num10 != num7)
			{
				for (int n = 0; n < track.CheckpointSegmentPositions.Count; n++)
				{
					if (track.CheckpointSegmentPositions[n] > num7 && track.CheckpointSegmentPositions[n] <= num10)
					{
						checkpointTimes.Add(lapTime * (float)m / (float)(num5 - 1));
						break;
					}
				}
			}
			num7 = num10;
			if (num8 >= 1f)
			{
				break;
			}
		}
		checkpointTimes.Add(lapTime);
	}

	public void Save()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		FileHelper.StorageContainerMRE.WaitOne();
		FileHelper.StorageContainerMRE.Reset();
		try
		{
			StorageDevice xnaUserDevice = FileHelper.XnaUserDevice;
			if (xnaUserDevice != null && xnaUserDevice.IsConnected)
			{
				StorageContainer val = xnaUserDevice.OpenContainer("RacingGame");
				try
				{
					string path = Path.Combine(val.Path, ReplayFilenames[trackNum]);
					using FileStream output = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
					using BinaryWriter binaryWriter = new BinaryWriter(output);
					binaryWriter.Write(lapTime);
					binaryWriter.Write(trackMatrixValues.Count);
					for (int i = 0; i < trackMatrixValues.Count; i++)
					{
						FileHelper.WriteMatrix(binaryWriter, trackMatrixValues[i]);
					}
					binaryWriter.Write(checkpointTimes.Count);
					for (int j = 0; j < checkpointTimes.Count; j++)
					{
						binaryWriter.Write(checkpointTimes[j]);
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Trace.WriteLine("Settings Load Failure: " + ex.ToString());
		}
		FileHelper.StorageContainerMRE.Set();
	}

	public object Clone()
	{
		Replay replay = (Replay)MemberwiseClone();
		replay.checkpointTimes = new List<float>(checkpointTimes);
		replay.trackMatrixValues = new List<Matrix>(trackMatrixValues);
		return replay;
	}

	public void AddCarMatrix(Matrix addMatrix)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		trackMatrixValues.Add(addMatrix);
	}
}
