using System.Collections.Generic;

namespace OluXNA;

internal class WaitCond
{
	public List<int> include;

	public List<int> notInclude;

	private string _cueName;

	public string cueName
	{
		get
		{
			return _cueName;
		}
		set
		{
			_cueName = value;
		}
	}

	public WaitCond()
	{
		include = new List<int>();
		notInclude = new List<int>();
	}

	~WaitCond()
	{
		Dispose();
	}

	public void Dispose()
	{
		include.Clear();
		notInclude.Clear();
	}

	public WaitCond(string _name)
		: this()
	{
		_cueName = _name;
	}

	public WaitCond(string _name, int _include)
		: this(_name)
	{
		include.Add(_include);
	}

	public WaitCond(string _name, Beats _inc)
		: this(_name, (int)_inc)
	{
	}

	public WaitCond(string _name, int _include, int _nInclude)
		: this(_name, _include)
	{
		notInclude.Add(_nInclude);
	}

	public WaitCond(string _name, Beats _inc, Beats _nInc)
		: this(_name, (int)_inc, (int)_nInc)
	{
	}

	public bool Check(int beat)
	{
		bool flag = true;
		foreach (int item in include)
		{
			if (beat % item != 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			foreach (int item2 in notInclude)
			{
				if (beat % item2 == 0)
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}
}
