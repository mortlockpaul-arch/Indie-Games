using System;
using System.IO;
using System.Net;

internal static class SslKeyLogger
{
	private static readonly string s_keyLogFile;

	private static readonly FileStream s_fileStream;

	public static bool IsEnabled => s_fileStream != null;

	static SslKeyLogger()
	{
		s_keyLogFile = Environment.GetEnvironmentVariable("SSLKEYLOGFILE");
		s_fileStream = null;
		try
		{
			if ((AppContext.TryGetSwitch("System.Net.EnableSslKeyLogging", out var isEnabled) & isEnabled) && s_keyLogFile != null)
			{
				s_fileStream = File.Open(s_keyLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
			}
		}
		catch (Exception ex)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"Failed to open SSL key log file '{s_keyLogFile}': {ex}", ".cctor");
			}
		}
	}

	public static void WriteSecrets(ReadOnlySpan<byte> clientRandom, ReadOnlySpan<byte> clientHandshakeTrafficSecret, ReadOnlySpan<byte> serverHandshakeTrafficSecret, ReadOnlySpan<byte> clientTrafficSecret0, ReadOnlySpan<byte> serverTrafficSecret0, ReadOnlySpan<byte> clientEarlyTrafficSecret)
	{
		if (s_fileStream == null || clientRandom.IsEmpty || (clientHandshakeTrafficSecret.IsEmpty && serverHandshakeTrafficSecret.IsEmpty && clientTrafficSecret0.IsEmpty && serverTrafficSecret0.IsEmpty && clientEarlyTrafficSecret.IsEmpty))
		{
			return;
		}
		Span<byte> span = ((clientRandom.Length > 1024) ? ((Span<byte>)new byte[clientRandom.Length * 2]) : stackalloc byte[clientRandom.Length * 2]);
		Span<byte> span2 = span;
		HexEncode(clientRandom, span2);
		lock (s_fileStream)
		{
			WriteSecretCore("CLIENT_HANDSHAKE_TRAFFIC_SECRET"u8, span2, clientHandshakeTrafficSecret);
			WriteSecretCore("SERVER_HANDSHAKE_TRAFFIC_SECRET"u8, span2, serverHandshakeTrafficSecret);
			WriteSecretCore("CLIENT_TRAFFIC_SECRET_0"u8, span2, clientTrafficSecret0);
			WriteSecretCore("SERVER_TRAFFIC_SECRET_0"u8, span2, serverTrafficSecret0);
			WriteSecretCore("CLIENT_EARLY_TRAFFIC_SECRET"u8, span2, clientEarlyTrafficSecret);
			s_fileStream.Flush();
		}
	}

	private static void WriteSecretCore(ReadOnlySpan<byte> labelUtf8, ReadOnlySpan<byte> clientRandomUtf8, ReadOnlySpan<byte> secret)
	{
		if (secret.Length != 0)
		{
			int num = checked(labelUtf8.Length + 1 + clientRandomUtf8.Length + 1 + 2 * secret.Length + 1);
			Span<byte> span = (((uint)num > 1024u) ? ((Span<byte>)new byte[num]) : stackalloc byte[num]);
			Span<byte> span2 = span;
			labelUtf8.CopyTo(span2);
			span2[labelUtf8.Length] = 32;
			clientRandomUtf8.CopyTo(span2.Slice(labelUtf8.Length + 1));
			span2[labelUtf8.Length + 1 + clientRandomUtf8.Length] = 32;
			HexEncode(secret, span2.Slice(labelUtf8.Length + 1 + clientRandomUtf8.Length + 1));
			span2[span2.Length - 1] = 10;
			s_fileStream.Write(span2);
		}
	}

	private static void HexEncode(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		for (int i = 0; i < source.Length; i++)
		{
			System.HexConverter.ToBytesBuffer(source[i], destination.Slice(i * 2));
		}
	}
}
