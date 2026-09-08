using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MonoMod;

namespace XnaToFna;

public class Program
{
	public static void Main(string[] args)
	{
		XnaToFnaUtil xnaToFnaUtil = new XnaToFnaUtil();
		xnaToFnaUtil.Log($"[Version] {MonoModder.Version}");
		FNAHooks.Hook();
		bool flag = true;
		Queue<string> queue = new Queue<string>(args);
		while (queue.Count > 0)
		{
			string text = queue.Dequeue();
			if (text == "--version" || text.ToLowerInvariant() == "-v")
			{
				return;
			}
			switch (text)
			{
			case "--mm-strict":
				((MonoModder)xnaToFnaUtil.Modder).Strict = true;
				continue;
			case "--skip-entrypoint":
				Console.WriteLine("Skipping entry point hook. This will limit and even disable some runtime features.");
				xnaToFnaUtil.HookEntryPoint = false;
				continue;
			case "--skip-content":
				flag = false;
				continue;
			case "--skip-xnb":
				xnaToFnaUtil.PatchXNB = false;
				continue;
			case "--skip-xact":
				xnaToFnaUtil.PatchXACT = false;
				continue;
			case "--skip-windowsmedia":
			case "--skip-wm":
				xnaToFnaUtil.PatchWindowsMedia = false;
				continue;
			case "--skip-wavebanks":
			case "--skip-xwb":
			case "--skip-soundbanks":
			case "--skip-xsb":
			case "--skip-xactsettings":
			case "--skip-xgs":
				Console.WriteLine("WARNING: --skip-xwb, --skip-xsb and --skip-xsg have been replaced with --skip-xact.");
				continue;
			case "--skip-video":
			case "--skip-wma":
				Console.WriteLine("WARNING: --skip-video and --skip-wma have been replaced with --skip-wm.");
				continue;
			case "--skip-locks":
			case "--keep-locks":
				xnaToFnaUtil.DestroyLocks = false;
				continue;
			case "--decompress-xnb":
			case "--skip-gzip":
				ContentHelper.XNBCompressGZip = false;
				continue;
			case "--anycpu":
			case "--force-anycpu":
				Console.WriteLine("WARNING: --anycpu / --force-anycpu is now default. To set the preferred platform, use --platform x86 / x64 instead.");
				continue;
			case "--platform":
				if (queue.Count >= 1)
				{
					xnaToFnaUtil.PreferredPlatform = ParseEnum(queue.Dequeue(), ILPlatform.AnyCPU);
					continue;
				}
				break;
			}
			if (text.StartsWith("--platform="))
			{
				xnaToFnaUtil.PreferredPlatform = ParseEnum(text.Substring("--platform=".Length), ILPlatform.AnyCPU);
				continue;
			}
			switch (text)
			{
			case "--keep-mixed-deps":
				xnaToFnaUtil.StubMixedDeps = false;
				xnaToFnaUtil.DestroyMixedDeps = false;
				continue;
			case "--stub-mixed-deps":
				xnaToFnaUtil.StubMixedDeps = true;
				xnaToFnaUtil.DestroyMixedDeps = false;
				continue;
			case "--remove-mixed-deps":
				xnaToFnaUtil.StubMixedDeps = false;
				xnaToFnaUtil.DestroyMixedDeps = true;
				continue;
			case "--remove-public-key-token":
				if (queue.Count >= 1)
				{
					xnaToFnaUtil.DestroyPublicKeyTokens.Add(queue.Dequeue());
					continue;
				}
				break;
			}
			if (text.StartsWith("--remove-public-key-token="))
			{
				xnaToFnaUtil.DestroyPublicKeyTokens.Add(text.Substring("--remove-public-key-token=".Length));
				continue;
			}
			switch (text)
			{
			case "--fix-old-mono-xml":
				Console.WriteLine("YOU SHOULD REALLY UPDATE YOUR COPY OF MONO!... Unless you're stuck with Xamarin.Android.");
				xnaToFnaUtil.FixOldMonoXML = true;
				continue;
			case "--update-xna":
			case "--xna3":
			case "--enable-flux-capacitor":
				Console.WriteLine("Please get yourself a copy of XnaToFna from the \"timemachine\" branch to enable the time machine.");
				return;
			case "--hook-istrialmode":
				Console.WriteLine("Do what you want cause a pirate is free! You are a pirate!");
				xnaToFnaUtil.HookIsTrialMode = true;
				continue;
			case "--content":
				if (queue.Count >= 1)
				{
					xnaToFnaUtil.ContentDirectoryNames.Add(queue.Dequeue());
					continue;
				}
				break;
			}
			if (text.StartsWith("--content="))
			{
				xnaToFnaUtil.ContentDirectoryNames.Add(text.Substring("--content=".Length));
				continue;
			}
			switch (text)
			{
			case "--skip-binaryformatter":
			case "--skip-bf":
				xnaToFnaUtil.HookBinaryFormatter = false;
				continue;
			case "--skip-reflection":
				xnaToFnaUtil.HookReflection = false;
				continue;
			case "--fix-path-arg":
				if (queue.Count >= 1)
				{
					xnaToFnaUtil.FixPathsFor.Add(queue.Dequeue());
					continue;
				}
				break;
			}
			if (text.StartsWith("--fix-path-arg="))
			{
				xnaToFnaUtil.FixPathsFor.Add(text.Substring("--fix-path-arg=".Length));
			}
			else
			{
				xnaToFnaUtil.ScanPath(text);
			}
		}
		if (!Debugger.IsAttached)
		{
			xnaToFnaUtil.ScanPath(Directory.GetCurrentDirectory());
		}
		xnaToFnaUtil.OrderModules();
		xnaToFnaUtil.RelinkAll();
		if (flag)
		{
			xnaToFnaUtil.LoadModules();
			xnaToFnaUtil.Modules.Clear();
			xnaToFnaUtil.ModulesToStub.Clear();
			xnaToFnaUtil.UpdateContent();
		}
		xnaToFnaUtil.Log("[Main] Done!");
		if (Debugger.IsAttached)
		{
			Console.ReadKey();
		}
	}

	private static T ParseEnum<T>(string value, T defaultResult) where T : struct
	{
		if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
		{
			return result;
		}
		return defaultResult;
	}
}
