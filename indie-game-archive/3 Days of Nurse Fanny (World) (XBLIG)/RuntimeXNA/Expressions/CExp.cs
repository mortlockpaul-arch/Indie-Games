using System;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Expressions;

public abstract class CExp
{
	public int code;

	public CExp()
	{
	}

	public static CExp create(CFile file)
	{
		int filePointer = file.getFilePointer();
		CExp cExp = null;
		int num = file.readAInt();
		cExp = num switch
		{
			0 => new EXP_END(), 
			131072 => new EXP_PLUS(), 
			262144 => new EXP_MINUS(), 
			393216 => new EXP_MULT(), 
			524288 => new EXP_DIV(), 
			655360 => new EXP_MOD(), 
			786432 => new EXP_POW(), 
			917504 => new EXP_AND(), 
			1048576 => new EXP_OR(), 
			1179648 => new EXP_XOR(), 
			65535 => new EXP_LONG(), 
			131071 => new EXP_RANDOM(), 
			196607 => new EXP_VARGLO(), 
			262143 => new EXP_STRING(), 
			327679 => new EXP_STR(), 
			393215 => new EXP_VAL(), 
			458751 => new EXP_PATH(), 
			524287 => new EXP_PATH(), 
			589823 => new EXP_PATH(), 
			720895 => new EXP_SIN(), 
			786431 => new EXP_COS(), 
			851967 => new EXP_TAN(), 
			917503 => new EXP_SQR(), 
			983039 => new EXP_LOG(), 
			1048575 => new EXP_LN(), 
			1114111 => new EXP_HEX(), 
			1179647 => new EXP_BIN(), 
			1245183 => new EXP_EXP(), 
			1310719 => new EXP_LEFT(), 
			1376255 => new EXP_RIGHT(), 
			1441791 => new EXP_MID(), 
			1507327 => new EXP_LEN(), 
			1572863 => new EXP_DOUBLE(), 
			1638399 => new EXP_VARGLONAMED(), 
			1900543 => new EXP_INT(), 
			1966079 => new EXP_ABS(), 
			2031615 => new EXP_CEIL(), 
			2097151 => new EXP_FLOOR(), 
			2162687 => new EXP_ACOS(), 
			2228223 => new EXP_ASIN(), 
			2293759 => new EXP_ATAN(), 
			2359295 => new EXP_NOT(), 
			2686975 => new EXP_MIN(), 
			2752511 => new EXP_MAX(), 
			2818047 => new EXP_GETRGB(), 
			2883583 => new EXP_GETRED(), 
			2949119 => new EXP_GETGREEN(), 
			3014655 => new EXP_GETBLUE(), 
			3080191 => new EXP_LOOPINDEX(), 
			3145727 => new EXP_NEWLINE(), 
			3211263 => new EXP_ROUND(), 
			3276799 => new EXP_STRINGGLO(), 
			3342335 => new EXP_STRINGGLONAMED(), 
			3407871 => new EXP_LOWER(), 
			3473407 => new EXP_UPPER(), 
			3538943 => new EXP_FIND(), 
			3604479 => new EXP_REVERSEFIND(), 
			3866623 => new EXP_FLOATTOSTRING(), 
			3932159 => new EXP_ATAN2(), 
			3997695 => new EXP_ZERO(), 
			4063231 => new EXP_EMPTY(), 
			-1 => new EXP_PARENTH1(), 
			-65537 => new EXP_PARENTH2(), 
			-131073 => new EXP_VIRGULE(), 
			65534 => new EXP_GETSAMPLEMAINVOL(), 
			131070 => new EXP_GETSAMPLEVOL(), 
			196606 => new EXP_GETCHANNELVOL(), 
			262142 => new EXP_GETSAMPLEMAINPAN(), 
			327678 => new EXP_GETSAMPLEPAN(), 
			393214 => new EXP_GETCHANNELPAN(), 
			589822 => new EXP_GETSAMPLEDUR(), 
			655358 => new EXP_GETCHANNELDUR(), 
			720894 => new EXP_GETSAMPLEFREQ(), 
			786430 => new EXP_GETCHANNELFREQ(), 
			65533 => new EXP_GAMLEVEL(), 
			131069 => new EXP_GAMNPLAYER(), 
			196605 => new EXP_PLAYXLEFT(), 
			262141 => new EXP_PLAYXRIGHT(), 
			327677 => new EXP_PLAYYTOP(), 
			393213 => new EXP_PLAYYBOTTOM(), 
			458749 => new EXP_PLAYWIDTH(), 
			524285 => new EXP_PLAYHEIGHT(), 
			589821 => new EXP_GAMLEVELNEW(), 
			655357 => new EXP_GETCOLLISIONMASK(), 
			720893 => new EXP_FRAMERATE(), 
			786429 => new EXP_GETVIRTUALWIDTH(), 
			851965 => new EXP_GETVIRTUALHEIGHT(), 
			917501 => new EXP_GETFRAMEBKDCOLOR(), 
			983037 => new EXP_ZERO(), 
			1048573 => new EXP_ZERO(), 
			1114109 => new EXP_ZERO(), 
			1179645 => new EXP_FRAMERGBCOEF(), 
			1245181 => new EXP_ZERO(), 
			65532 => new EXP_TIMVALUE(), 
			131068 => new EXP_TIMCENT(), 
			196604 => new EXP_TIMSECONDS(), 
			262140 => new EXP_TIMHOURS(), 
			327676 => new EXP_TIMMINITS(), 
			65530 => new EXP_XMOUSE(), 
			131066 => new EXP_YMOUSE(), 
			196602 => new EXP_MOUSEWHEELDELTA(), 
			65529 => new EXP_PLASCORE(), 
			131065 => new EXP_PLALIVES(), 
			196601 => new EXP_GETINPUT(), 
			262137 => new EXP_GETINPUTKEY(), 
			327673 => new EXP_GETPLAYERNAME(), 
			65531 => new EXP_CRENUMBERALL(), 
			5242883 => new EXP_STRNUMBER(), 
			5308419 => new EXP_STRGETCURRENT(), 
			5373955 => new EXP_STRGETNUMBER(), 
			5439491 => new EXP_STRGETNUMERIC(), 
			5505027 => new EXP_STRGETNPARA(), 
			5242882 => new EXP_GETRGBAT(), 
			5308418 => new EXP_GETSCALEX(), 
			5373954 => new EXP_GETSCALEY(), 
			5439490 => new EXP_GETANGLE(), 
			5242887 => new EXP_CVALUE(), 
			5308423 => new EXP_CGETMIN(), 
			5373959 => new EXP_CGETMAX(), 
			5439495 => new EXP_CGETCOLOR1(), 
			5505031 => new EXP_CGETCOLOR2(), 
			5242889 => new EXP_CCAGETFRAMENUMBER(), 
			5308425 => new EXP_CCAGETGLOBALVALUE(), 
			5373961 => new EXP_CCAGETGLOBALSTRING(), 
			_ => (num & -65536) switch
			{
				65536 => new EXP_EXTYSPR(), 
				131072 => new EXP_EXTISPR(), 
				196608 => new EXP_EXTSPEED(), 
				262144 => new EXP_EXTACC(), 
				327680 => new EXP_EXTDEC(), 
				393216 => new EXP_EXTDIR(), 
				458752 => new EXP_EXTXLEFT(), 
				524288 => new EXP_EXTXRIGHT(), 
				589824 => new EXP_EXTYTOP(), 
				655360 => new EXP_EXTYBOTTOM(), 
				720896 => new EXP_EXTXSPR(), 
				786432 => new EXP_EXTIDENTIFIER(), 
				851968 => new EXP_EXTFLAG(), 
				917504 => new EXP_EXTNANI(), 
				983040 => new EXP_EXTNOBJECTS(), 
				1048576 => new EXP_EXTVAR(), 
				1114112 => new EXP_EXTGETSEMITRANSPARENCY(), 
				1179648 => new EXP_EXTNMOVE(), 
				1245184 => new EXP_EXTVARSTRING(), 
				1310720 => new EXP_EXTGETFONTNAME(), 
				1376256 => new EXP_EXTGETFONTSIZE(), 
				1441792 => new EXP_EXTGETFONTCOLOR(), 
				1507328 => new EXP_EXTGETLAYER(), 
				1572864 => new EXP_EXTGETGRAVITY(), 
				1638400 => new EXP_EXTXAP(), 
				1703936 => new EXP_EXTYAP(), 
				1769472 => new EXP_EXTALPHACOEF(), 
				1835008 => new EXP_EXTRGBCOEF(), 
				1900544 => new EXP_ZERO(), 
				1966080 => new EXP_EXTVARBYINDEX(), 
				2031616 => new EXP_EXTVARSTRINGBYINDEX(), 
				_ => new CExpExtension(), 
			}, 
		};
		if (cExp != null)
		{
			cExp.code = num;
			if (num != 0)
			{
				short num2 = file.readAShort();
				switch (num)
				{
				case 262143:
					((EXP_STRING)cExp).pString = file.readAString();
					break;
				case 65535:
					((EXP_LONG)cExp).value = file.readAInt();
					break;
				case 1572863:
					((EXP_DOUBLE)cExp).value = file.readADouble();
					break;
				case 1638399:
					file.skipBytes(4);
					((EXP_VARGLONAMED)cExp).number = file.readAShort();
					break;
				case 3342335:
					file.skipBytes(4);
					((EXP_STRINGGLONAMED)cExp).number = file.readAShort();
					break;
				default:
				{
					short num3 = (short)num;
					if (num3 >= 2 || num3 == -7)
					{
						CExpOi cExpOi = (CExpOi)cExp;
						cExpOi.oi = file.readAShort();
						cExpOi.oiList = file.readAShort();
						switch (num & -65536)
						{
						case 1048576:
							((EXP_EXTVAR)cExp).number = file.readAShort();
							break;
						case 1245184:
							((EXP_EXTVARSTRING)cExp).number = file.readAShort();
							break;
						}
					}
					break;
				}
				}
				file.seek(filePointer + num2);
			}
		}
		else
		{
			Console.Out.WriteLine("*** Missing expression!");
		}
		return cExp;
	}

	public abstract void evaluate(CRun rhPtr);
}
