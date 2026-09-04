using System;
using RuntimeXNA.Application;
using RuntimeXNA.Events;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;

namespace RuntimeXNA.Actions;

public abstract class CAct : CEvent
{
	public const byte ACTFLAGS_REPEAT = 1;

	public CAct()
	{
	}

	public static CAct create(CRunApp app)
	{
		int filePointer = app.file.getFilePointer();
		short num = app.file.readAShort();
		CAct cAct = null;
		int num2 = app.file.readAInt();
		cAct = num2 switch
		{
			65535 => new ACT_SKIP(), 
			262143 => new ACT_SETVARG(), 
			327679 => new ACT_SUBVARG(), 
			393215 => new ACT_ADDVARG(), 
			458751 => new ACT_GRPACTIVATE(), 
			524287 => new ACT_GRPDEACTIVATE(), 
			983039 => new ACT_STARTLOOP(), 
			1048575 => new ACT_STOPLOOP(), 
			1114111 => new ACT_SETLOOPINDEX(), 
			1179647 => new ACT_RANDOMIZE(), 
			1310719 => new ACT_SETGLOBALSTRING(), 
			1572863 => new ACT_OPENDEBUGGER(), 
			1638399 => new ACT_PAUSEDEBUGGER(), 
			65534 => new ACT_PLAYSAMPLE(), 
			131070 => new ACT_STOPSAMPLE(), 
			327678 => new ACT_PLAYLOOPSAMPLE(), 
			458750 => new ACT_STOPSPESAMPLE(), 
			524286 => new ACT_PAUSESAMPLE(), 
			589822 => new ACT_RESUMESAMPLE(), 
			786430 => new ACT_PLAYCHANNEL(), 
			851966 => new ACT_PLAYLOOPCHANNEL(), 
			917502 => new ACT_PAUSECHANNEL(), 
			983038 => new ACT_RESUMECHANNEL(), 
			1048574 => new ACT_STOPCHANNEL(), 
			1179646 => new ACT_SETCHANNELVOL(), 
			1245182 => new ACT_SETCHANNELPAN(), 
			1376254 => new ACT_SETSAMPLEMAINVOL(), 
			1441790 => new ACT_SETSAMPLEVOL(), 
			1507326 => new ACT_SETSAMPLEMALNPAN(), 
			1572862 => new ACT_SETSAMPLEPAN(), 
			1638398 => new ACT_PAUSEALLCHANNELS(), 
			1703934 => new ACT_RESUMEALLCHANNELS(), 
			2031614 => new ACT_LOCKCHANNEL(), 
			2097150 => new ACT_UNLOCKCHANNEL(), 
			2162686 => new ACT_SETCHANNELFREQ(), 
			2228222 => new ACT_SETSAMPLEFREQ(), 
			65533 => new ACT_NEXTLEVEL(), 
			131069 => new ACT_PREVLEVEL(), 
			196605 => new ACT_GOLEVEL(), 
			262141 => new ACT_PAUSE(), 
			327677 => new ACT_ENDGAME(), 
			393213 => new ACT_RESTARTGAME(), 
			458749 => new ACT_RESTARTLEVEL(), 
			524285 => new ACT_CDISPLAY(), 
			589821 => new ACT_CDISPLAYX(), 
			655357 => new ACT_CDISPLAYY(), 
			983037 => new ACT_FULLSCREENMODE(), 
			1048573 => new ACT_WINDOWEDMODE(), 
			1114109 => new ACT_SETFRAMERATE(), 
			1179645 => new ACT_PAUSEKEY(), 
			1245181 => new ACT_PAUSEANYKEY(), 
			1441789 => new ACT_SETVIRTUALWIDTH(), 
			1507325 => new ACT_SETVIRTUALHEIGHT(), 
			1572861 => new ACT_SETFRAMEBDKCOLOR(), 
			1638397 => new ACT_DELCREATEDBKDAT(), 
			1703933 => new ACT_DELALLCREATEDBKD(), 
			1769469 => new ACT_SETFRAMEWIDTH(), 
			1835005 => new ACT_SETFRAMEHEIGHT(), 
			2162685 => new ACT_SKIP(), 
			2228221 => new ACT_SKIP(), 
			2293757 => new ACT_SKIP(), 
			2359293 => new ACT_SKIP(), 
			2424829 => new ACT_SKIP(), 
			65532 => new ACT_SETTIMER(), 
			65530 => new ACT_HIDECURSOR(), 
			131066 => new ACT_SHOWCURSOR(), 
			65529 => new ACT_SETSCORE(), 
			131065 => new ACT_SETLIVES(), 
			196601 => new ACT_NOINPUT(), 
			262137 => new ACT_RESTINPUT(), 
			327673 => new ACT_ADDSCORE(), 
			393209 => new ACT_ADDLIVES(), 
			458745 => new ACT_SUBSCORE(), 
			524281 => new ACT_SUBLIVES(), 
			589817 => new ACT_SETINPUT(), 
			655353 => new ACT_SETINPUTKEY(), 
			720889 => new ACT_SETPLAYERNAME(), 
			65531 => new ACT_CREATE(), 
			5242883 => new ACT_STRDESTROY(), 
			5308419 => new ACT_STRDISPLAY(), 
			5373955 => new ACT_STRDISPLAYDURING(), 
			5439491 => new ACT_STRSETCOLOUR(), 
			5505027 => new ACT_STRSET(), 
			5570563 => new ACT_STRPREV(), 
			5636099 => new ACT_STRNEXT(), 
			5701635 => new ACT_STRDISPLAYSTRING(), 
			5767171 => new ACT_STRSETSTRING(), 
			5242882 => new ACT_SPRPASTE(), 
			5308418 => new ACT_SPRFRONT(), 
			5373954 => new ACT_SPRBACK(), 
			5439490 => new ACT_SPRADDBKD(), 
			5505026 => new ACT_SPRREPLACECOLOR(), 
			5570562 => new ACT_SPRSETSCALE(), 
			5636098 => new ACT_SPRSETSCALEX(), 
			5701634 => new ACT_SPRSETSCALEY(), 
			5767170 => new ACT_SPRSETANGLE(), 
			5242887 => new ACT_CSETVALUE(), 
			5308423 => new ACT_CADDVALUE(), 
			5373959 => new ACT_CSUBVALUE(), 
			5439495 => new ACT_CSETMIN(), 
			5505031 => new ACT_CSETMAX(), 
			5570567 => new ACT_CSETCOLOR1(), 
			5636103 => new ACT_CSETCOLOR2(), 
			5242884 => new ACT_QASK(), 
			5242889 => new ACT_CCARESTARTAPP(), 
			5308425 => new ACT_CCARESTARTFRAME(), 
			5373961 => new ACT_CCANEXTFRAME(), 
			5439497 => new ACT_CCAPREVIOUSFRAME(), 
			5505033 => new ACT_CCAENDAPP(), 
			5636105 => new ACT_CCAJUMPFRAME(), 
			5701641 => new ACT_CCASETGLOBALVALUE(), 
			5767177 => new ACT_CCASHOW(), 
			5832713 => new ACT_CCAHIDE(), 
			5898249 => new ACT_CCASETGLOBALSTRING(), 
			5963785 => new ACT_CCAPAUSEAPP(), 
			6029321 => new ACT_CCARESUMEAPP(), 
			_ => (num2 & -65536) switch
			{
				65536 => new ACT_EXTSETPOS(), 
				131072 => new ACT_EXTSETX(), 
				196608 => new ACT_EXTSETY(), 
				262144 => new ACT_EXTSTOP(), 
				327680 => new ACT_EXTSTART(), 
				393216 => new ACT_EXTSPEED(), 
				458752 => new ACT_EXTMAXSPEED(), 
				524288 => new ACT_EXTWRAP(), 
				589824 => new ACT_EXTBOUNCE(), 
				655360 => new ACT_EXTREVERSE(), 
				720896 => new ACT_EXTNEXTMOVE(), 
				786432 => new ACT_EXTPREVMOVE(), 
				851968 => new ACT_EXTSELMOVE(), 
				917504 => new ACT_EXTLOOKAT(), 
				983040 => new ACT_EXTSTOPANIM(), 
				1048576 => new ACT_EXTSTARTANIM(), 
				1114112 => new ACT_EXTFORCEANIM(), 
				1179648 => new ACT_EXTFORCEDIR(), 
				1245184 => new ACT_EXTFORCESPEED(), 
				1310720 => new ACT_EXTRESTANIM(), 
				1376256 => new ACT_EXTRESTDIR(), 
				1441792 => new ACT_EXTRESTSPEED(), 
				1507328 => new ACT_EXTSETDIR(), 
				1572864 => new ACT_EXTDESTROY(), 
				1638400 => new ACT_EXTSHUFFLE(), 
				1703936 => new ACT_EXTHIDE(), 
				1769472 => new ACT_EXTSHOW(), 
				1835008 => new ACT_EXTDISPLAYDURING(), 
				1900544 => new ACT_EXTSHOOT(), 
				1966080 => new ACT_EXTSHOOTTOWARD(), 
				2031616 => new ACT_EXTSETVAR(), 
				2097152 => new ACT_EXTADDVAR(), 
				2162688 => new ACT_EXTSUBVAR(), 
				2228224 => new ACT_EXTDISPATCHVAR(), 
				2293760 => new ACT_EXTSETFLAG(), 
				2359296 => new ACT_EXTCLRFLAG(), 
				2424832 => new ACT_EXTCHGFLAG(), 
				2490368 => new ACT_EXTINKEFFECT(), 
				2555904 => new ACT_EXTSETSEMITRANSPARENCY(), 
				2621440 => new ACT_EXTFORCEFRAME(), 
				2686976 => new ACT_EXTRESTFRAME(), 
				2752512 => new ACT_EXTSETACCELERATION(), 
				2818048 => new ACT_EXTSETDECELERATION(), 
				2883584 => new ACT_EXTSETROTATINGSPEED(), 
				2949120 => new ACT_EXTSETDIRECTIONS(), 
				3014656 => new ACT_EXTBRANCHNODE(), 
				3080192 => new ACT_EXTSETGRAVITY(), 
				3145728 => new ACT_EXTGOTONODE(), 
				3211264 => new ACT_EXTSETVARSTRING(), 
				3276800 => new ACT_EXTSETFONTNAME(), 
				3342336 => new ACT_EXTSETFONTSIZE(), 
				3407872 => new ACT_EXTSETBOLD(), 
				3670016 => new ACT_EXTSETTEXTCOLOR(), 
				3735552 => new ACT_EXTSPRFRONT(), 
				3801088 => new ACT_EXTSPRBACK(), 
				3866624 => new ACT_EXTMOVEBEFORE(), 
				3932160 => new ACT_EXTMOVEAFTER(), 
				3997696 => new ACT_EXTMOVETOLAYER(), 
				4063232 => new ACT_EXTADDTODEBUGGER(), 
				4128768 => new ACT_EXTSETEFFECT(), 
				4194304 => new ACT_EXTSETEFFECTPARAM(), 
				4259840 => new ACT_EXTSETALPHACOEF(), 
				4325376 => new ACT_EXTSETRGBCOEF(), 
				4390912 => new ACT_EXTSETEFFECTPARAMTEXTURE(), 
				_ => new CActExtension(), 
			}, 
		};
		if (cAct != null)
		{
			cAct.evtCode = num2;
			cAct.evtOi = app.file.readAShort();
			cAct.evtOiList = app.file.readAShort();
			cAct.evtFlags = app.file.readByte();
			cAct.evtFlags2 = app.file.readByte();
			cAct.evtNParams = app.file.readByte();
			cAct.evtDefType = app.file.readByte();
			if (cAct.evtNParams > 0)
			{
				cAct.evtParams = new CParam[cAct.evtNParams];
				for (int i = 0; i < cAct.evtNParams; i++)
				{
					cAct.evtParams[i] = CParam.create(app);
				}
			}
		}
		else
		{
			Console.Out.WriteLine("*** Missing action!");
		}
		app.file.seek(filePointer + num);
		return cAct;
	}

	public abstract void execute(CRun rhPtr);
}
