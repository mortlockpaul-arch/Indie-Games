using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class AvatarRenderableModelCache : IDisposable
{
	private const int MAX_CACHED_ITEMS = 32;

	private Dictionary<AvatarRenderableModelCacheKey, LinkedListNode<AvatarRenderableModelCacheItem>> m_ItemsDictionary = new Dictionary<AvatarRenderableModelCacheKey, LinkedListNode<AvatarRenderableModelCacheItem>>();

	private LinkedList<AvatarRenderableModelCacheItem> m_ItemsLru = new LinkedList<AvatarRenderableModelCacheItem>();

	internal GraphicsDevice GraphicsDevice { get; set; }

	public AvatarRenderableModelCache(GraphicsDevice device)
	{
		GraphicsDevice = device;
	}

	internal AvatarRenderableModelCacheItem GetItem(AvatarRenderableModelCacheKey key)
	{
		lock (m_ItemsDictionary)
		{
			if (m_ItemsDictionary.TryGetValue(key, out var value))
			{
				m_ItemsLru.Remove(value);
				m_ItemsLru.AddFirst(value);
				return value.Value;
			}
		}
		return null;
	}

	internal void CleanCache()
	{
		lock (m_ItemsDictionary)
		{
			foreach (AvatarRenderableModelCacheItem item in m_ItemsLru)
			{
				item.RenderableModel.Release();
			}
			m_ItemsDictionary = new Dictionary<AvatarRenderableModelCacheKey, LinkedListNode<AvatarRenderableModelCacheItem>>();
			m_ItemsLru = new LinkedList<AvatarRenderableModelCacheItem>();
		}
	}

	internal void AddItem(AvatarRenderableModelCacheItem cacheItem)
	{
		lock (m_ItemsDictionary)
		{
			AvatarRenderableModel renderableModel = cacheItem.RenderableModel;
			AvatarRenderableModelCacheKey key = new AvatarRenderableModelCacheKey(renderableModel.Manifest, renderableModel.ModelIndex);
			if (!m_ItemsDictionary.ContainsKey(key))
			{
				renderableModel.AddReference();
				int num = m_ItemsLru.Count + 1;
				AvatarRenderableModel renderableModel2;
				while (num > 32)
				{
					LinkedListNode<AvatarRenderableModelCacheItem> last = m_ItemsLru.Last;
					AvatarRenderableModelCacheItem value = last.Value;
					renderableModel2 = value.RenderableModel;
					m_ItemsDictionary.Remove(new AvatarRenderableModelCacheKey(renderableModel2.Manifest, renderableModel2.ModelIndex));
					m_ItemsLru.Remove(last);
					num--;
					renderableModel2.Release();
				}
				LinkedListNode<AvatarRenderableModelCacheItem> linkedListNode = new LinkedListNode<AvatarRenderableModelCacheItem>(cacheItem);
				m_ItemsLru.AddFirst(linkedListNode);
				renderableModel2 = cacheItem.RenderableModel;
				m_ItemsDictionary.Add(key, linkedListNode);
			}
		}
	}

	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			foreach (AvatarRenderableModelCacheItem item in m_ItemsLru)
			{
				item.RenderableModel.Release();
			}
		}
		m_ItemsDictionary = null;
		m_ItemsLru = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
