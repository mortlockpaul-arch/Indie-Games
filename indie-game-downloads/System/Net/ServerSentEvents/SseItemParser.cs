namespace System.Net.ServerSentEvents;

public delegate T SseItemParser<out T>(string eventType, ReadOnlySpan<byte> data);
