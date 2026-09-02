using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using RacingGame.GameLogic;
using RacingGame.Graphics;

namespace RacingGame.Helpers;

public static class FileHelper
{
	public static ManualResetEvent StorageContainerMRE = new ManualResetEvent(initialState: true);

	private static StorageDevice xnaUserDevice = null;

	public static StorageDevice XnaUserDevice
	{
		get
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected O, but got Unknown
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			if (xnaUserDevice != null && !xnaUserDevice.IsConnected)
			{
				xnaUserDevice = null;
			}
			if (xnaUserDevice == null)
			{
				if (Guide.IsVisible)
				{
					return null;
				}
				IAsyncResult asyncResult = null;
				asyncResult = Guide.BeginShowStorageDeviceSelector(Gamer.SignedInGamers[Input.controllingPlayer].PlayerIndex, (AsyncCallback)null, (object)null);
				while (!asyncResult.IsCompleted)
				{
					Thread.Sleep(10);
					BaseGame.graphicsManager.GraphicsDevice.Clear(Color.Black);
					BaseGame.graphicsManager.GraphicsDevice.Present();
				}
				xnaUserDevice = Guide.EndShowStorageDeviceSelector(asyncResult);
				if (!Guide.IsVisible)
				{
					((GameComponent)BaseGame.GamerServicesComponent).Update(new GameTime());
				}
				if (Guide.IsVisible)
				{
					Thread.Sleep(10);
					((GameComponent)BaseGame.GamerServicesComponent).Update(new GameTime());
					BaseGame.graphicsManager.GraphicsDevice.Clear(Color.Black);
					BaseGame.graphicsManager.GraphicsDevice.Present();
				}
			}
			return xnaUserDevice;
		}
	}

	public static FileStream LoadGameContentFile(string relativeFilename)
	{
		string path = Path.Combine(StorageContainer.TitleLocation, relativeFilename);
		if (!File.Exists(path))
		{
			return null;
		}
		return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
	}

	public static string[] GetLines(string filename)
	{
		try
		{
			StreamReader streamReader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read), Encoding.UTF8);
			List<string> list = new List<string>();
			do
			{
				list.Add(streamReader.ReadLine());
			}
			while (streamReader.Peek() > -1);
			streamReader.Close();
			return list.ToArray();
		}
		catch (FileNotFoundException)
		{
			return null;
		}
		catch (DirectoryNotFoundException)
		{
			return null;
		}
		catch (IOException)
		{
			return null;
		}
	}

	public static void WriteVector3(BinaryWriter writer, Vector3 vec)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
	}

	public static void WriteVector4(BinaryWriter writer, Vector4 vec)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
		writer.Write(vec.W);
	}

	public static void WriteMatrix(BinaryWriter writer, Matrix matrix)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		writer.Write(matrix.M11);
		writer.Write(matrix.M12);
		writer.Write(matrix.M13);
		writer.Write(matrix.M14);
		writer.Write(matrix.M21);
		writer.Write(matrix.M22);
		writer.Write(matrix.M23);
		writer.Write(matrix.M24);
		writer.Write(matrix.M31);
		writer.Write(matrix.M32);
		writer.Write(matrix.M33);
		writer.Write(matrix.M34);
		writer.Write(matrix.M41);
		writer.Write(matrix.M42);
		writer.Write(matrix.M43);
		writer.Write(matrix.M44);
	}

	public static Vector3 ReadVector3(BinaryReader reader)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}

	public static Vector4 ReadVector4(BinaryReader reader)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}

	public static Matrix ReadMatrix(BinaryReader reader)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		return new Matrix(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
	}
}
