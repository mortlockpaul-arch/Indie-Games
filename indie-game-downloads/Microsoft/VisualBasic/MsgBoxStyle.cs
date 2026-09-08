using System;

namespace Microsoft.VisualBasic;

[Flags]
public enum MsgBoxStyle
{
	OkOnly = 0,
	OkCancel = 1,
	AbortRetryIgnore = 2,
	YesNoCancel = OkCancel | AbortRetryIgnore,
	YesNo = 4,
	RetryCancel = OkCancel | YesNo,
	Critical = 0x10,
	Question = 0x20,
	Exclamation = Critical | Question,
	Information = 0x40,
	DefaultButton1 = 0,
	DefaultButton2 = 0x100,
	DefaultButton3 = 0x200,
	ApplicationModal = 0,
	SystemModal = 0x1000,
	MsgBoxHelp = 0x4000,
	MsgBoxRight = 0x80000,
	MsgBoxRtlReading = 0x100000,
	MsgBoxSetForeground = 0x10000
}
