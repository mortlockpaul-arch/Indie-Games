using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.XboxLive.Avatars.Internal;

public class AvatarAssetsDependenciesResolver
{
	public static volatile Dictionary<Guid, AvatarAssetDependency> s_ShapeOverrides;

	public static object s_ShapeOverridesLock = new object();

	public static void InitializeShapeDependencies(Stream stream)
	{
		if (stream == null)
		{
			return;
		}
		Dictionary<Guid, AvatarAssetDependency> dictionary = new Dictionary<Guid, AvatarAssetDependency>();
		byte[] array = new byte[16];
		while (true)
		{
			bool flag = true;
			if (stream.Read(array, 0, 16) != 16)
			{
				break;
			}
			Guid key = new Guid(array);
			if (stream.Read(array, 0, 16) != 16)
			{
				break;
			}
			AvatarAssetDependency avatarAssetDependency = new AvatarAssetDependency();
			avatarAssetDependency.m_DependentAssetId = new Guid(array);
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num += stream.ReadByte() << (i << 3);
			}
			if (num > 0)
			{
				if ((uint)num > 4u)
				{
					break;
				}
				avatarAssetDependency.m_ModifiedAssetList = new Guid[num];
				for (int j = 0; j < num; j++)
				{
					stream.Read(array, 0, 16);
					ref Guid reference = ref avatarAssetDependency.m_ModifiedAssetList[j];
					reference = new Guid(array);
				}
			}
			else
			{
				avatarAssetDependency.m_ModifiedAssetList = null;
			}
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, avatarAssetDependency);
			}
		}
		s_ShapeOverrides = dictionary;
	}

	public static void DownloadDependedAssetCompleted(object sender, DataRequestCompletedEventArgs e)
	{
		if (e.Error == null && !e.Cancelled)
		{
			InitializeShapeDependencies(e.Result);
		}
		AutoResetEvent autoResetEvent = (AutoResetEvent)e.UserState;
		autoResetEvent.Set();
	}

	public static void InvalidateDependenciesTable()
	{
		lock (s_ShapeOverridesLock)
		{
			s_ShapeOverrides = null;
		}
	}

	public static AvatarAssetDependency GetDependentAssets(IDataManager dataManager, Guid id)
	{
		Dictionary<Guid, AvatarAssetDependency> dictionary = s_ShapeOverrides;
		if (dictionary == null)
		{
			lock (s_ShapeOverridesLock)
			{
				if (s_ShapeOverrides == null)
				{
					AutoResetEvent autoResetEvent = new AutoResetEvent(initialState: false);
					dataManager.GetAssetAsync("TocAssetDependencies".ToLower(), DownloadDependedAssetCompleted, autoResetEvent);
					autoResetEvent.WaitOne();
					if (s_ShapeOverrides == null)
					{
						throw new AvatarException(Resources.ShapeOverridesFailedToLoad);
					}
				}
				dictionary = s_ShapeOverrides;
			}
		}
		AvatarAssetDependency value = new AvatarAssetDependency();
		dictionary.TryGetValue(id, out value);
		return value;
	}
}
