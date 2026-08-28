using System;
using System.IO;

namespace ZXBox;

public class Binary
{
	private BinaryReader br;

	private FileStream fs;

	private string filename;

	public byte[] bytes;

	private int _byteposition;

	public int BytePosition => _byteposition;

	public Binary(string infilename)
	{
		filename = infilename;
	}

	public void Open()
	{
		fs = new FileStream(filename, FileMode.Open);
		br = new BinaryReader(fs);
		bytes = new byte[br.BaseStream.Length];
		br.Read(bytes, 0, Convert.ToInt32(br.BaseStream.Length));
	}

	public void Close()
	{
		fs.Close();
		br.Close();
	}

	public byte[] Readbytes(int position, int length)
	{
		SetPosition(position);
		return ReadNextbytes(length);
	}

	public void SetPosition(int position)
	{
		_byteposition = position;
	}

	public byte[] ReadNextbytes(int length)
	{
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = bytes[_byteposition++];
		}
		return array;
	}

	public char[] ReadChars(int position, int length)
	{
		_byteposition = position;
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = (char)bytes[_byteposition++];
		}
		return array;
	}

	public long Lenght()
	{
		return bytes.Length;
	}

	private int GetIntelWord()
	{
		_byteposition += 2;
		return bytes[_byteposition - 2] | (bytes[_byteposition - 1] << 8);
	}

	private int GetIntelDWord()
	{
		_byteposition += 4;
		return bytes[_byteposition - 4] | (bytes[_byteposition - 3] << 8) | (bytes[_byteposition - 2] << 16) | (bytes[_byteposition - 1] << 24);
	}
}
