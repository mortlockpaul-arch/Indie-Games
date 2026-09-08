namespace System;

public delegate void EventHandler(object? sender, EventArgs e);
public delegate void EventHandler<in TEventArgs>(object? sender, TEventArgs e) where TEventArgs : allows ref struct;
public delegate void EventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TSender : allows ref struct where TEventArgs : allows ref struct;
