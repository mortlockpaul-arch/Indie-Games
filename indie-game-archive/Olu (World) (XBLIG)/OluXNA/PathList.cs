using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PathList
{
	private List<IPath> paths;

	public int curPathIndex;

	public int _loopIndex;

	public List<IPath> publicPaths => paths;

	public int loopIndex => _loopIndex;

	public PathList()
	{
		paths = new List<IPath>();
	}

	public void Dispose()
	{
		paths.Clear();
	}

	public PathList Clone()
	{
		PathList pathList = new PathList();
		pathList.paths = new List<IPath>();
		pathList.curPathIndex = curPathIndex;
		pathList.curPathIndex = 0;
		pathList._loopIndex = _loopIndex;
		foreach (IPath path2 in paths)
		{
			IPath path = path2.copy();
			path.reset();
			pathList.paths.Add(path);
		}
		return pathList;
	}

	public bool Update(GameTime gametime)
	{
		bool flag = false;
		if (paths.Count > 0)
		{
			float num = paths[curPathIndex].advance();
			while (num > 0.001f && !flag)
			{
				curPathIndex++;
				if (curPathIndex >= paths.Count)
				{
					if (loopIndex >= 0)
					{
						curPathIndex = loopIndex;
					}
					else
					{
						flag = true;
					}
				}
				if (!flag)
				{
					paths[curPathIndex].reset();
					num = paths[curPathIndex].advance();
				}
			}
		}
		return flag;
	}

	public void ResetCurrent()
	{
		if (paths.Count > 0)
		{
			paths[curPathIndex].reset();
		}
	}

	public void SetLoop(int _lIndex)
	{
		_loopIndex = _lIndex;
	}

	public Vector3 curLocation()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (paths.Count > 0)
		{
			return paths[curPathIndex].curLocation();
		}
		return Vector3.Zero;
	}

	public Vector3 curDir()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (paths.Count > 0)
		{
			return Vector3.Normalize(paths[curPathIndex].dir());
		}
		return Vector3.Forward;
	}

	public float maxSpeed()
	{
		if (paths.Count > 0)
		{
			return paths[curPathIndex].maxSpeed();
		}
		return 0f;
	}

	public void Add(IPath _iPath)
	{
		paths.Add(_iPath);
	}

	public void addPathComboList(List<IPath> _paths, IPath _comboPart)
	{
		foreach (IPath _path in _paths)
		{
			paths.Add(new PComboPath(_path, _comboPart));
		}
	}
}
