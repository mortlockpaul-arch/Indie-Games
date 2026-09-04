using System;
using System.IO;
using BinaryRead;
using Microsoft.Xna.Framework.Storage;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CIni
{
	private CRunkcini ini;

	private CArrayList strings;

	private string currentFileName;

	private short flags;

	public CIni(CRunkcini i, short f)
	{
		ini = i;
		flags = f;
	}

	public void loadFromProject()
	{
		CFile cFile = null;
		CEmbeddedFile embeddedFile = ini.rh.rhApp.getEmbeddedFile(currentFileName);
		if (embeddedFile != null)
		{
			cFile = embeddedFile.open();
		}
		if (cFile == null)
		{
			string text = currentFileName;
			int num = text.LastIndexOf('.');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			Data data = null;
			try
			{
				data = ini.rh.rhApp.content.Load<Data>(text);
			}
			catch (Exception ex)
			{
				ex.GetType();
			}
			if (data != null)
			{
				cFile = new CFile(data.data);
			}
		}
		if (cFile == null)
		{
			return;
		}
		if ((flags & 8) != 0)
		{
			cFile.setUnicode(unicode: false);
		}
		while (!cFile.isEOF())
		{
			string text2 = cFile.readAStringEOL();
			if (text2 == null)
			{
				break;
			}
			strings.add(text2);
		}
	}

	public void loadIni(string fileName)
	{
		bool flag = true;
		if (currentFileName != null && string.Compare(currentFileName, fileName, StringComparison.OrdinalIgnoreCase) == 0)
		{
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		saveIni();
		strings = new CArrayList();
		if (ini.ho.hoAdRunHeader.rhApp.storageDevice != null && ini.ho.hoAdRunHeader.rhApp.storageDevice.IsConnected)
		{
			currentFileName = fileName;
			IAsyncResult asyncResult = ini.ho.hoAdRunHeader.rhApp.storageDevice.BeginOpenContainer(ini.ho.hoAdRunHeader.rhApp.appName, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = ini.ho.hoAdRunHeader.rhApp.storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (!storageContainer.FileExists(fileName))
			{
				storageContainer.Dispose();
				loadFromProject();
				return;
			}
			Stream stream = storageContainer.OpenFile(fileName, FileMode.Open);
			StreamReader streamReader = new StreamReader(stream);
			try
			{
				while (true)
				{
					string text = streamReader.ReadLine();
					if (text != null)
					{
						strings.add(text);
						continue;
					}
					break;
				}
			}
			catch (IOException ex)
			{
				ex.GetType();
			}
			streamReader.Close();
			streamReader.Dispose();
			storageContainer.Dispose();
		}
		else
		{
			ini.ho.hoAdRunHeader.rhApp.storageDevice = null;
		}
	}

	public void saveIni()
	{
		if (ini.ho.hoAdRunHeader.rhApp.storageDevice == null || !ini.ho.hoAdRunHeader.rhApp.storageDevice.IsConnected)
		{
			ini.ho.hoAdRunHeader.rhApp.storageDevice = null;
		}
		else
		{
			if (strings == null || currentFileName == null)
			{
				return;
			}
			IAsyncResult asyncResult = ini.ho.hoAdRunHeader.rhApp.storageDevice.BeginOpenContainer(ini.ho.hoAdRunHeader.rhApp.appName, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = ini.ho.hoAdRunHeader.rhApp.storageDevice.EndOpenContainer(asyncResult);
			asyncResult.AsyncWaitHandle.Close();
			if (!storageContainer.FileExists(currentFileName))
			{
				storageContainer.DeleteFile(currentFileName);
			}
			Stream stream = storageContainer.CreateFile(currentFileName);
			StreamWriter streamWriter = new StreamWriter(stream);
			try
			{
				for (int i = 0; i < strings.size(); i++)
				{
					streamWriter.WriteLine((string)strings.get(i));
				}
			}
			catch (IOException ex)
			{
				ex.GetType();
			}
			streamWriter.Close();
			streamWriter.Dispose();
			storageContainer.Dispose();
		}
	}

	private int findSection(string sectionName)
	{
		for (int i = 0; i < strings.size(); i++)
		{
			string text = (string)strings.get(i);
			if (text[0] != '[')
			{
				continue;
			}
			int num = text.LastIndexOf(']');
			if (num >= 1)
			{
				string strA = text.Substring(1, num - 1);
				if (string.Compare(strA, sectionName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private int findKey(int l, string keyName)
	{
		while (l < strings.size())
		{
			string text = (string)strings.get(l);
			if (text[0] == '[')
			{
				return -1;
			}
			int num = text.IndexOf('=');
			if (num >= 0)
			{
				string strA = text.Substring(0, num);
				if (string.Compare(strA, keyName) == 0)
				{
					return l;
				}
			}
			l++;
		}
		return -1;
	}

	public string getPrivateProfileString(string sectionName, string keyName, string defaultString, string fileName)
	{
		loadIni(fileName);
		int num = findSection(sectionName);
		if (num >= 0)
		{
			num = findKey(num + 1, keyName);
			if (num >= 0)
			{
				string text = (string)strings.get(num);
				int num2 = text.IndexOf('=');
				int i;
				for (i = num2 + 1; i < text.Length && text[i] == ' '; i++)
				{
				}
				int num3 = text.Length;
				while (num3 > i && text[num3 - 1] == ' ')
				{
					num3--;
				}
				if (num3 > i)
				{
					return text.Substring(i, num3 - i);
				}
			}
		}
		return defaultString;
	}

	public void writePrivateProfileString(string sectionName, string keyName, string name, string fileName)
	{
		loadIni(fileName);
		int num = findSection(sectionName);
		string o;
		if (num < 0)
		{
			o = "[" + sectionName + "]";
			strings.add(o);
			o = keyName + "=" + name;
			strings.add(o);
			return;
		}
		int num2 = findKey(num + 1, keyName);
		if (num2 >= 0)
		{
			o = keyName + "=" + name;
			strings.set(num2, o);
			return;
		}
		for (num2 = num + 1; num2 < strings.size(); num2++)
		{
			o = (string)strings.get(num2);
			if (o[0] == '[')
			{
				o = keyName + "=" + name;
				strings.add(num2, o);
				return;
			}
		}
		o = keyName + "=" + name;
		strings.add(o);
	}

	public void deleteItem(string group, string item, string iniName)
	{
		loadIni(iniName);
		int num = findSection(group);
		if (num >= 0)
		{
			int num2 = findKey(num + 1, item);
			if (num2 >= 0)
			{
				strings.remove(num2);
			}
		}
	}

	public void deleteGroup(string group, string iniName)
	{
		loadIni(iniName);
		int num = findSection(group);
		if (num >= 0)
		{
			strings.remove(num);
			while (num < strings.size() && ((string)strings.get(num))[0] != '[')
			{
				strings.remove(num);
			}
		}
	}
}
