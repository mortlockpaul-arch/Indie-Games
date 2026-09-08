using System;
using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
[DebuggerNonUserCode]
public sealed class Constants
{
	public const string vbCrLf = "\r\n";

	public const int vbObjectError = -2147221504;

	[Obsolete("vbNewLine has been deprecated. For a carriage return and line feed, use vbCrLf. For the current platform's newline, use System.Environment.NewLine.")]
	public const string vbNewLine = "\r\n";

	public const string vbCr = "\r";

	public const string vbLf = "\n";

	public const string vbBack = "\b";

	public const string vbFormFeed = "\f";

	public const string vbTab = "\t";

	public const string vbVerticalTab = "\v";

	public const string vbNullChar = "\0";

	public const string vbNullString = null;

	public const AppWinStyle vbHide = AppWinStyle.Hide;

	public const AppWinStyle vbNormalFocus = AppWinStyle.NormalFocus;

	public const AppWinStyle vbMinimizedFocus = AppWinStyle.MinimizedFocus;

	public const AppWinStyle vbMaximizedFocus = AppWinStyle.MaximizedFocus;

	public const AppWinStyle vbNormalNoFocus = AppWinStyle.NormalNoFocus;

	public const AppWinStyle vbMinimizedNoFocus = AppWinStyle.MinimizedNoFocus;

	public const CallType vbMethod = CallType.Method;

	public const CallType vbGet = CallType.Get;

	public const CallType vbLet = CallType.Let;

	public const CallType vbSet = CallType.Set;

	public const CompareMethod vbBinaryCompare = CompareMethod.Binary;

	public const CompareMethod vbTextCompare = CompareMethod.Text;

	public const DateFormat vbGeneralDate = DateFormat.GeneralDate;

	public const DateFormat vbLongDate = DateFormat.LongDate;

	public const DateFormat vbShortDate = DateFormat.ShortDate;

	public const DateFormat vbLongTime = DateFormat.LongTime;

	public const DateFormat vbShortTime = DateFormat.ShortTime;

	public const FirstDayOfWeek vbUseSystemDayOfWeek = FirstDayOfWeek.System;

	public const FirstDayOfWeek vbSunday = FirstDayOfWeek.Sunday;

	public const FirstDayOfWeek vbMonday = FirstDayOfWeek.Monday;

	public const FirstDayOfWeek vbTuesday = FirstDayOfWeek.Tuesday;

	public const FirstDayOfWeek vbWednesday = FirstDayOfWeek.Wednesday;

	public const FirstDayOfWeek vbThursday = FirstDayOfWeek.Thursday;

	public const FirstDayOfWeek vbFriday = FirstDayOfWeek.Friday;

	public const FirstDayOfWeek vbSaturday = FirstDayOfWeek.Saturday;

	public const FileAttribute vbNormal = FileAttribute.Normal;

	public const FileAttribute vbReadOnly = FileAttribute.ReadOnly;

	public const FileAttribute vbHidden = FileAttribute.Hidden;

	public const FileAttribute vbSystem = FileAttribute.System;

	public const FileAttribute vbVolume = FileAttribute.Volume;

	public const FileAttribute vbDirectory = FileAttribute.Directory;

	public const FileAttribute vbArchive = FileAttribute.Archive;

	public const FirstWeekOfYear vbUseSystem = FirstWeekOfYear.System;

	public const FirstWeekOfYear vbFirstJan1 = FirstWeekOfYear.Jan1;

	public const FirstWeekOfYear vbFirstFourDays = FirstWeekOfYear.FirstFourDays;

	public const FirstWeekOfYear vbFirstFullWeek = FirstWeekOfYear.FirstFullWeek;

	public const VbStrConv vbUpperCase = VbStrConv.Uppercase;

	public const VbStrConv vbLowerCase = VbStrConv.Lowercase;

	public const VbStrConv vbProperCase = VbStrConv.ProperCase;

	public const VbStrConv vbWide = VbStrConv.Wide;

	public const VbStrConv vbNarrow = VbStrConv.Narrow;

	public const VbStrConv vbKatakana = VbStrConv.Katakana;

	public const VbStrConv vbHiragana = VbStrConv.Hiragana;

	public const VbStrConv vbSimplifiedChinese = VbStrConv.SimplifiedChinese;

	public const VbStrConv vbTraditionalChinese = VbStrConv.TraditionalChinese;

	public const VbStrConv vbLinguisticCasing = VbStrConv.LinguisticCasing;

	public const TriState vbUseDefault = TriState.UseDefault;

	public const TriState vbTrue = TriState.True;

	public const TriState vbFalse = TriState.False;

	public const VariantType vbEmpty = VariantType.Empty;

	public const VariantType vbNull = VariantType.Null;

	public const VariantType vbInteger = VariantType.Integer;

	public const VariantType vbLong = VariantType.Long;

	public const VariantType vbSingle = VariantType.Single;

	public const VariantType vbDouble = VariantType.Double;

	public const VariantType vbCurrency = VariantType.Currency;

	public const VariantType vbDate = VariantType.Date;

	public const VariantType vbString = VariantType.String;

	public const VariantType vbObject = VariantType.Object;

	public const VariantType vbBoolean = VariantType.Boolean;

	public const VariantType vbVariant = VariantType.Variant;

	public const VariantType vbDecimal = VariantType.Decimal;

	public const VariantType vbByte = VariantType.Byte;

	public const VariantType vbUserDefinedType = VariantType.UserDefinedType;

	public const VariantType vbArray = VariantType.Array;

	public const MsgBoxResult vbOK = MsgBoxResult.Ok;

	public const MsgBoxResult vbCancel = MsgBoxResult.Cancel;

	public const MsgBoxResult vbAbort = MsgBoxResult.Abort;

	public const MsgBoxResult vbRetry = MsgBoxResult.Retry;

	public const MsgBoxResult vbIgnore = MsgBoxResult.Ignore;

	public const MsgBoxResult vbYes = MsgBoxResult.Yes;

	public const MsgBoxResult vbNo = MsgBoxResult.No;

	public const MsgBoxStyle vbOKOnly = MsgBoxStyle.OkOnly;

	public const MsgBoxStyle vbOKCancel = MsgBoxStyle.OkCancel;

	public const MsgBoxStyle vbAbortRetryIgnore = MsgBoxStyle.AbortRetryIgnore;

	public const MsgBoxStyle vbYesNoCancel = MsgBoxStyle.YesNoCancel;

	public const MsgBoxStyle vbYesNo = MsgBoxStyle.YesNo;

	public const MsgBoxStyle vbRetryCancel = MsgBoxStyle.RetryCancel;

	public const MsgBoxStyle vbCritical = MsgBoxStyle.Critical;

	public const MsgBoxStyle vbQuestion = MsgBoxStyle.Question;

	public const MsgBoxStyle vbExclamation = MsgBoxStyle.Exclamation;

	public const MsgBoxStyle vbInformation = MsgBoxStyle.Information;

	public const MsgBoxStyle vbDefaultButton1 = MsgBoxStyle.OkOnly;

	public const MsgBoxStyle vbDefaultButton2 = MsgBoxStyle.DefaultButton2;

	public const MsgBoxStyle vbDefaultButton3 = MsgBoxStyle.DefaultButton3;

	public const MsgBoxStyle vbApplicationModal = MsgBoxStyle.OkOnly;

	public const MsgBoxStyle vbSystemModal = MsgBoxStyle.SystemModal;

	public const MsgBoxStyle vbMsgBoxHelp = MsgBoxStyle.MsgBoxHelp;

	public const MsgBoxStyle vbMsgBoxRight = MsgBoxStyle.MsgBoxRight;

	public const MsgBoxStyle vbMsgBoxRtlReading = MsgBoxStyle.MsgBoxRtlReading;

	public const MsgBoxStyle vbMsgBoxSetForeground = MsgBoxStyle.MsgBoxSetForeground;
}
