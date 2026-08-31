using System;
using System.Collections.Generic;

namespace BureauNewPDA;

public class SaveDataMaster
{
	public class saveDataShell
	{
		public int id;

		public string saveName = "";

		public DateTime saveDateTime = DateTime.UtcNow;
	}

	public int lastSavedId = 1;

	public List<saveDataShell> saveList = new List<saveDataShell>();

	public bool vibrationOn = true;

	public bool fastTextSkip;

	public bool skipAnimation;

	public bool invertY;

	private bool found;

	public void saveData(int id)
	{
		found = false;
		lastSavedId = id;
		foreach (saveDataShell save in saveList)
		{
			if (save.id == id)
			{
				found = true;
				save.saveDateTime = DateTime.UtcNow;
			}
		}
		if (!found)
		{
			saveDataShell saveDataShell2 = new saveDataShell();
			saveDataShell2.id = id;
			saveDataShell2.saveName = "Save Slot " + id;
			saveList.Add(saveDataShell2);
		}
	}
}
