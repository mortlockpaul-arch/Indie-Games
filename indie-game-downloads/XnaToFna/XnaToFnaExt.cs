using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace XnaToFna;

public static class XnaToFnaExt
{
	public static byte[] ReadBytesUntil(this BinaryReader reader, long position)
	{
		return reader.ReadBytes((int)(position - reader.BaseStream.Position));
	}

	public static void KillIfAlive(this Process p)
	{
		try
		{
			if (p != null && !p.HasExited)
			{
				p.Kill();
			}
		}
		catch
		{
		}
	}

	public static Thread AsyncPipeErr(this Process p, bool nullify = false)
	{
		Thread obj = (nullify ? new Thread((ThreadStart)delegate
		{
			try
			{
				StreamReader standardError = p.StandardError;
				while (!p.HasExited)
				{
					standardError.ReadLine();
				}
			}
			catch
			{
			}
		})
		{
			Name = $"STDERR pipe thread for {p.ProcessName}",
			IsBackground = true
		} : new Thread((ThreadStart)delegate
		{
			try
			{
				StreamReader standardError = p.StandardError;
				while (!p.HasExited)
				{
					Console.WriteLine(standardError.ReadLine());
				}
			}
			catch
			{
			}
		})
		{
			Name = $"STDERR pipe thread for {p.ProcessName}",
			IsBackground = true
		});
		obj.Start();
		return obj;
	}

	public static Thread AsyncPipeOut(this Process p, bool nullify = false)
	{
		Thread obj = (nullify ? new Thread((ThreadStart)delegate
		{
			try
			{
				StreamReader standardOutput = p.StandardOutput;
				while (!p.HasExited)
				{
					standardOutput.ReadLine();
				}
			}
			catch
			{
			}
		})
		{
			Name = $"STDOUT pipe thread for {p.ProcessName}",
			IsBackground = true
		} : new Thread((ThreadStart)delegate
		{
			try
			{
				StreamReader standardOutput = p.StandardOutput;
				while (!p.HasExited)
				{
					Console.WriteLine(standardOutput.ReadLine());
				}
			}
			catch
			{
			}
		})
		{
			Name = $"STDOUT pipe thread for {p.ProcessName}",
			IsBackground = true
		});
		obj.Start();
		return obj;
	}

	public static T GetTarget<T>(this WeakReference<T> weak) where T : class
	{
		if (weak.TryGetTarget(out var target))
		{
			return target;
		}
		return null;
	}

	public static void EmitDefault(this ILProcessor il, TypeReference t, bool stind = false, bool arrayEmpty = true)
	{
		if (t == null)
		{
			il.Emit(OpCodes.Ldnull);
			if (stind)
			{
				il.Emit(OpCodes.Stind_Ref);
			}
		}
		else if (t.MetadataType != MetadataType.Void)
		{
			_ = t.IsArray & arrayEmpty;
			int value = 0;
			if (!stind)
			{
				value = il.Body.Variables.Count;
				il.Body.Variables.Add(new VariableDefinition(t));
				il.Emit(OpCodes.Ldloca, value);
			}
			il.Emit(OpCodes.Initobj, t);
			if (!stind)
			{
				il.Emit(OpCodes.Ldloc, value);
			}
		}
	}

	public static T GetDefault<T>()
	{
		return default(T);
	}
}
