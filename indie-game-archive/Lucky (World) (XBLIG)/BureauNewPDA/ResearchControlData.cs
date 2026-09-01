using System;
using System.Collections.Generic;

namespace BureauNewPDA;

public class ResearchControlData
{
	public class ResearchData
	{
		public enum activateType
		{
			PlayVideoReturn,
			GotoScene,
			PlayVideoPuzzle
		}

		public enum DisplayState
		{
			NA,
			Adding,
			Added,
			Updating,
			Updated,
			Normal,
			Remove,
			Removed
		}

		public int id = -1;

		public List<string> requiredVariables = new List<string>();

		public List<string> resultingVariables = new List<string>();

		public List<string> excludeIfVariables = new List<string>();

		public string headerTxt = "";

		public string bodyTxt = "";

		public string notAvailableText = "";

		public string completedBodyTxt = "";

		public string completedheaderTxt = "";

		public bool hasTimeRequirement;

		public bool hasLocationRequirement;

		public int startTime;

		public int endTime;

		public int baseDurationMinutes;

		public activateType type;

		public string gotoSceneName = "";

		public int gotoSceneId = 1;

		public string playVideoName = "";

		public int playVideoPuzzleId = -1;

		public DisplayState displayState;

		public bool hasBeenDisplayed;
	}

	public List<ResearchData> masterResearchList = new List<ResearchData>();

	public List<VariableEngine.variableData> variableList = new List<VariableEngine.variableData>();

	public void addData()
	{
		masterResearchList.Clear();
		int id = 1;
		addPlayVideoReturn(id, "Basic Person Check on Shooter", "FBISearchKeithKaran", 30);
		addBodyText(id, "By using the FBI search program, you can search for all public information available on the shooter.  This will search through all social networks, uploaded videos and comments made on various websites.  The program will also perform a personality analysis based upon his writing samples.");
		addCompletedText(id, "Complete - Basic Check on Keith Karan", "Keith Karan was born in Ohio.  He had a wife and daughter but is now divorced.  He was laid off in 2009 from a tire manufacturer and has since traveled to Arizona.  He has no known employment status.  Based on his writing, he has repressed sexual desires and a great deal of anger towards almost everyone.  He especially hates women, homosexuals, non-believers and foreigners.  His writing has become more and more righteous with his sinking self-esteem.");
		addRequiredVariables(id, getVar(1));
		addResultingVariables(id, getVar(2));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 2;
		addPlayVideoReturn(id, "Uzi Serial Number Check", "UziSearch", 60);
		addBodyText(id, "Uzi has a tracking number on it.  Using the FBI database, you might be able to find out where the gun has been and who sold it.");
		addCompletedText(id, "Complete - Uzi tracked", "Gun was flagged in Phoenix.  A local pawnshop refused to purchase it.  The seller was located in Glendale, Arizona.  Using the public search option, it appears that the owner started placing ads for the gun.  Using bank transactional data (private Bureau database) a likely purchaser is the owner of a local gun shop / strip club.");
		addRequiredVariables(id, getVar(3));
		addResultingVariables(id, getVar(4));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 3;
		addPlayVideoPuzzle(id, "Solve For Keith Karan", 1, 30, "KeithPuzzleA");
		addBodyText(id, "You now have enough information to solve the case for Keith Karan.  By selecting this option, you should be able to determine why he tried to kill Ramesh.");
		addCompletedText(id, "Solved for Keith Karan", "Keith is part of a group that thinks Ramesh is a terrorist working inside the FBI.  The group provided him with intel and the weapon to kill Ramesh.");
		addNotAvailableText(id, "Once you have all the basic information, you can determine why someone tried to kill Ramesh.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(2));
		addRequiredVariables(id, getVar(4));
		addResultingVariables(id, getVar(6));
		addResultingVariables(id, getVar(5));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 4;
		addPlayGotoScene(id, "Call Ramesh about the case", "SR6-RameshCaseSolved", 1, 30);
		addBodyText(id, "Since you solved the case, you need to call Ramesh and give him the details.");
		addCompletedText(id, "Complete - Called Ramesh about case", "Ramesh is acting weird.  He does not want to talk about the case until the morning.  I wonder what is going on?");
		addRequiredVariables(id, getVar(6));
		addResultingVariables(id, getVar(7));
		addResultingVariables(id, getVar(8));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 5;
		addPlayGotoScene(id, "Call Ramesh for help", "SR5-RameshHelp", 1, 5);
		addBodyText(id, "If you get stuck, you can call Ramesh to get help.  Only call him if you need extra help.");
		addExcludeIfVariables(id, getVar(5));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 6;
		addPlayVideoReturn(id, "Finish Working / Relax ", "KendallHouseNightToDay", 0);
		addBodyText(id, "It is time to finish working for the day.  Your Action list will change in the morning.");
		addCompletedText(id, "Sleep / Dressed / Ready to Start Work", "Note:  Completed items will be removed from your action list.");
		addRequiredVariables(id, getVar(8));
		addResultingVariables(id, getVar(0));
		addResultingVariables(id, getVar(10));
		addResultingVariables(id, getVar(11));
		addExcludeIfVariables(id, getVar(11));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 7;
		addPlayVideoReturn(id, "Basic Person Check on Ramesh", "FBISearchRameshPriyavardha", 30);
		addBodyText(id, "How much do I really know about Ramesh?  I could use the search program to see if anything turns up.");
		addCompletedText(id, "Complete - Basic Person Check on Ramesh", "Ramesh was born in Chennai, India and became a citizen of the United States in 1972.  He is a Hindu with fairly moderate political views.  He has been quoted in several articles expressing negative views on the United States prison system.  Out of all the countries, the United States has the highest percentage of people in prison.");
		addRequiredVariables(id, getVar(6));
		addResultingVariables(id, getVar(9));
		addExcludeIfVariables(id, getVar(12));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 8;
		addPlayGotoScene(id, "Car Pickup / Coffee with Ramesh", "SR7-KylerIntercepts", 1, 130);
		addBodyText(id, "Now that you are working again, you need to pickup a standard Bureau car.  After you do that, you can meet with Ramesh to discuss the Kieth Karan case.");
		addRequiredVariables(id, getVar(11));
		addResultingVariables(id, getVar(9));
		addResultingVariables(id, getVar(12));
		addExcludeIfVariables(id, getVar(12));
		addExcludeIfVariables(id, getVar(13));
		addExcludeIfVariables(id, getVar(17));
		id = 9;
		addPlayVideoReturn(id, "Yoga Meditation", "Meditation", 0);
		addBodyText(id, "Ramesh taught you some yoga / meditation moves that will help you focus and relax.  Warning - You can only do this activity at night - this will skip ahead to nighttime.  Bonus- this activity will allow you to make 1 mistake while trying to solve a puzzle.");
		addCompletedText(id, "Completed - Yoga Meditation", "You now have better focus.  Bonus - you get 1 retry during the next video puzzle.");
		addRequiredVariables(id, getVar(14));
		addResultingVariables(id, getVar(15));
		addResultingVariables(id, getVar(16));
		addExcludeIfVariables(id, getVar(15));
		addExcludeIfVariables(id, getVar(17));
		id = 10;
		addPlayGotoScene(id, "Finish Working / Relax", "CaseIntro", 1, 0);
		addBodyText(id, "It is time to finish working for the day.  Your Action list will change in the morning.");
		addCompletedText(id, "Sleep / Exercise / Ready to Start Work", "Note:  Completed items will be removed from your action list.");
		addRequiredVariables(id, getVar(13));
		addResultingVariables(id, getVar(0));
		addExcludeIfVariables(id, getVar(17));
		id = 11;
		addPlayGotoScene(id, "Go Home and get some sleep", "MorningRun", 1, 0);
		addBodyText(id, "You need to get some rest.");
		addCompletedText(id, "Sleep / Exercise / Ready to Start Work", "Note:  Completed items will be removed from your action list.");
		addRequiredVariables(id, getVar(17));
		addResultingVariables(id, getVar(21));
		addExcludeIfVariables(id, getVar(21));
		addExcludeIfVariables(id, getVar(52));
		id = 12;
		addPlayVideoReturn(id, "Call Officer Miller", "SFJailPhoneCall", 15);
		addBodyText(id, "Miller is the officer that was at the scene of the crime.  He is the arresting officer and was also the first officer on the scene of the crime.");
		addCompletedText(id, "Complete - Call Officer Miller", "Maybe it is a little late to be calling people.  I should go home and go to sleep.");
		addRequiredVariables(id, getVar(17));
		addResultingVariables(id, getVar(18));
		addExcludeIfVariables(id, getVar(21));
		addExcludeIfVariables(id, getVar(52));
		id = 13;
		addPlayGotoScene(id, "Call Van Ness Emergency Center", "HospitalCallNight", 1, 15);
		addBodyText(id, "William is staying at the Van Ness Emergency Center.  I should call first before I drive over there.");
		addCompletedText(id, "Completed - Call Emergency Center", "No help there.  I should go home and get some sleep.");
		addRequiredVariables(id, getVar(17));
		addResultingVariables(id, getVar(20));
		addExcludeIfVariables(id, getVar(21));
		addExcludeIfVariables(id, getVar(52));
		id = 14;
		addPlayGotoScene(id, "Visit Van Ness Emergency Center", "HospitalVisitNight", 1, 60);
		addBodyText(id, "William is staying at the Van Ness Emergency Center.  I need to get some info from him.");
		addCompletedText(id, "Completed - Visit Emergency Center", "No help there.  I should go home and get some sleep.");
		addRequiredVariables(id, getVar(17));
		addResultingVariables(id, getVar(20));
		addExcludeIfVariables(id, getVar(21));
		addExcludeIfVariables(id, getVar(52));
		id = 15;
		addPlayVideoReturn(id, "Basic Person Check on Jacob", "FBISearchJacobBaker", 30);
		addBodyText(id, "Basic search might give me some better insight into Jacob.");
		addCompletedText(id, "Complete - Basic Person Check on Jacob", "Jacob was born in Mayflower, Illinois.  He is 26 years old and lives with his mom.  His parents divorced in 2001.  Based on his online comments, Jacob shows typical signs of hostility towards others, homophobia, extreme insecurity and an obsession with his sister Molly.  Based on his writing samples, he has limited education and writes at the 3rd grade level.");
		addNotAvailableText(id, "Basic Search for Jacob needs to be done in secure environment - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(17));
		addResultingVariables(id, getVar(22));
		addExcludeIfVariables(id, getVar(52));
		id = 16;
		addPlayVideoPuzzle(id, "Solve For Jacob", 2, 60, "JacobPuzzle");
		addBodyText(id, "You now have enough information to solve for Jacob.  By selecting this option, you will be able to sort out the testimony and evidence collected that relates to Jacob.  Because this is a complex case, this is only one piece of the puzzle for solving the case.");
		addCompletedText(id, "Solved For Jacob", "Jacob admits hitting his sister and his testimony is filled with sexual undertones that don't match the evidence found in William's apartment.  He also admits seeing the key to the apartment - a weird detail to share with the police.  Other than the key, he has no physical evidence linking him to the crime.  Obsession with his sister is an easy motive and the key gives him the opportunity.");
		addNotAvailableText(id, "Once you have all the basic information, you can sort out the evidence collected about Jacob.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(17));
		addRequiredVariables(id, getVar(23));
		addRequiredVariables(id, getVar(22));
		addResultingVariables(id, getVar(35));
		addExcludeIfVariables(id, getVar(52));
		id = 17;
		addPlayGotoScene(id, "Visit William at Penthouse", "WilliamConversation", 1, 60);
		addBodyText(id, "According to Kyler, William is currently at his home.  I should visit him before he leaves for his parent's house.");
		addCompletedText(id, "Completed - Visit William at Penthouse", "William's apartment has been robbed including some fake diamonds.  There is something odd about the key found on Jacob.  Molly's key was out in the open and easy to steal but somehow Jacob has William's key instead.  William says he had his key after Jacob left.");
		addRequiredVariables(id, getVar(21));
		addResultingVariables(id, getVar(23));
		addExcludeIfVariables(id, getVar(52));
		id = 18;
		addPlayVideoReturn(id, "Basic Person Check on William", "FBISearchWilliamWright", 30);
		addBodyText(id, "Basic search might give me some better insight into William.");
		addCompletedText(id, "Complete - Basic Person Check on William", "William Wright is the son of billionaire James P. Wright.  He has gone to the best schools and has a positive relationship with his parents.  He has always excelled at school and has had no disciplinary problems.");
		addNotAvailableText(id, "Basic Search for William needs to be done in secure environment - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(23));
		addResultingVariables(id, getVar(24));
		addExcludeIfVariables(id, getVar(52));
		id = 19;
		addPlayVideoPuzzle(id, "Solve For William", 3, 80, "WilliamPuzzle");
		addBodyText(id, "You now have enough information to solve for William.  By selecting this option, you will be able to sort out his testimony and evidence collected for William.  Because this is a complex case, this is only one piece of the puzzle for solving the case.");
		addCompletedText(id, "Solved For William", "William is not a suspect for this crime.  He has no motive and was in the hospital at the time of the crime with a fractured hand.  William's apartment being robbed adds a wrinkle to the case.  It is also odd that Jacob has William's key.  William had given this key to Molly at the hospital.  Jacob should have had Molly's key.");
		addNotAvailableText(id, "Once you have all the basic information, you can sort out the evidence collected about William.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(23));
		addRequiredVariables(id, getVar(24));
		addRequiredVariables(id, getVar(38));
		addResultingVariables(id, getVar(36));
		addExcludeIfVariables(id, getVar(52));
		id = 20;
		addPlayGotoScene(id, "Go Home", "GoHomeFromWilliams", 1, 60);
		addBodyText(id, "Drive home.  Make sure you have nothing else to do here or you will waste time driving back into the city.");
		addCompletedText(id, "Sleep / Exercise / Ready to Start Work", "Note:  Completed items will be removed from your action list.");
		addRequiredVariables(id, getVar(23));
		addResultingVariables(id, getVar(28));
		addExcludeIfVariables(id, getVar(28));
		addExcludeIfVariables(id, getVar(52));
		id = 21;
		addPlayGotoScene(id, "Call Officer Miller", "CallMillerDay", 1, 20);
		addBodyText(id, "Miller is the officer that was at the scene of the crime.  He had responded to a domestic abuse call earlier and was the first officer to arrive after Molly's death.");
		addCompletedText(id, "Complete - Call Officer Miller", "Miller provides some details on the case.   From his perspective, it is an open and shut case against Jacob.  He had the motive and the key to the penthouse.  He was not aware of anything being stolen from William's apartment.");
		addRequiredVariables(id, getVar(23));
		addExcludeIfVariables(id, getVar(28));
		addExcludeIfVariables(id, getVar(32));
		addExcludeIfVariables(id, getVar(52));
		id = 22;
		addPlayGotoScene(id, "Interview Lobby Guard", "InterviewLobbyGuard", 1, 30);
		addBodyText(id, "The lobby guard should have seen who came into the building when Molly was killed.  He/she would be a good person to interview.  It would be better to do this now since I am already here.");
		addCompletedText(id, "Completed - Interview Lobby Guard", "Jack Henry was little help and did not see who killed Molly. He was on break when Molly and the killer entered the building, which gives us around a 20-minute timeframe.   However, he did mention that the police have the security footage from the lobby.");
		addRequiredVariables(id, getVar(23));
		addResultingVariables(id, getVar(25));
		addExcludeIfVariables(id, getVar(28));
		addExcludeIfVariables(id, getVar(52));
		id = 23;
		addPlayGotoScene(id, "Interview Lobby Guard", "GoLobbyGuardFromHome", 1, 90);
		addBodyText(id, "Drive back to the city and interview lobby guard.  The lobby guard should have seen who came into the building when Molly was killed.  He/she would be a good person to interview.  It would be better to do this now since I am already here.");
		addCompletedText(id, "Completed - Interview Lobby Guard", "Jack Henry was little help and did not see who killed Molly. He was on break when Molly and the killer entered the building, which gives us around a 20-minute timeframe.   However, he did mention that the police have the security footage from the lobby.");
		addRequiredVariables(id, getVar(28));
		addResultingVariables(id, getVar(25));
		addResultingVariables(id, "RemoveAtHome");
		addExcludeIfVariables(id, getVar(25));
		addExcludeIfVariables(id, getVar(52));
		id = 24;
		addPlayGotoScene(id, "Go Home", "GoHomeFromLobby2", 1, 60);
		addBodyText(id, "Drive home.  You cannot work on many of the items until you are in a secure location.");
		addCompletedText(id, "Sleep / Exercise / Ready to Start Work", "Note:  Completed items will be removed from your action list.");
		addRequiredVariables(id, getVar(29));
		addResultingVariables(id, getVar(30));
		addExcludeIfVariables(id, getVar(30));
		addExcludeIfVariables(id, getVar(52));
		id = 25;
		addPlayVideoReturn(id, "Basic Person Check on Jack (Lobby Guard)", "FBISearchJackHenry", 30);
		addBodyText(id, "Basic search might give me some better insight into the Lobby Guard.");
		addCompletedText(id, "Complete - Basic Person Check on Jack (Lobby Guard)", "Jack Henry has a history of crime and incarceration.  He currently has 2 strikes, which means any future crime could give him his 3rd strike.  Jack is also a recovering alcoholic and his crime sprees are associated with his drinking.   It appears that he has been sober for 10 years and has had a clean record for that same duration.  He has never been involved with a violent crime.");
		addNotAvailableText(id, "Basic Search needs to be done in secure environment - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(25));
		addResultingVariables(id, getVar(31));
		addExcludeIfVariables(id, getVar(52));
		id = 26;
		addPlayVideoReturn(id, "Lobby Surveillance Video", "VideoElevator", 30);
		addBodyText(id, "Video is taken from the apartment video surveillance system.");
		addCompletedText(id, "Complete - Lobby Surveillance Video", "Unfortunately, someone pushed the surveillance camera out of view prior to entering the elevators.  This happened while Jack was on break and right before Molly returned.  Although it does not tell us who broke into the apartment, it does tell us they were smart enough not to get caught on tape.  Jacob doesn't seem like a good candidate.");
		addNotAvailableText(id, "Video needs to be viewed in a secure environment - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(33));
		addResultingVariables(id, getVar(34));
		addExcludeIfVariables(id, getVar(52));
		id = 27;
		addPlayVideoPuzzle(id, "Solve For Molly Forensics", 4, 90, "DariMorguePuzzle");
		addBodyText(id, "Officer Miller sent you the Coroner's report on Molly.  By selecting this option, you will be able to figure out what the forensics evidence means in this case.");
		addCompletedText(id, "Solved For Molly Forensics", "Molly was killed instantly from the fall.  However, she has a bruise on her chest that forensics believes came from the killer.  She was kicked out of the window.  The killer was wearing a shoe size of 10 maybe 11.  Molly has a bruise on her eye from being hit by Jacob.  On her person, she has her driver's license and her key.  At the time she left the hospital, she did not have her license and she had William's key.");
		addNotAvailableText(id, "Once you have all the basic information, you can sort out the evidence collected about Molly.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(32));
		addResultingVariables(id, getVar(39));
		addExcludeIfVariables(id, getVar(52));
		id = 28;
		addPlayGotoScene(id, "Call Officer Miller about Video", "OfficerVideoOnly", 1, 20);
		addBodyText(id, "Jack Henry (lobby guard) told me that the police have the video surveillance tape from the lobby camera.  I need to have Officer Miller send me that tape.");
		addCompletedText(id, "Complete - Call Officer Miller Video", "Officer Miller has sent me the tape.");
		addRequiredVariables(id, getVar(32));
		addRequiredVariables(id, getVar(25));
		addRequiredVariables(id, getVar(40));
		addResultingVariables(id, getVar(33));
		addExcludeIfVariables(id, getVar(33));
		addExcludeIfVariables(id, getVar(52));
		id = 29;
		addPlayVideoReturn(id, "View Jacob Arrest Evidence", "EvidenceJacobKey", 90);
		addBodyText(id, "Officer Miller sent over a photo of the items collected from Jacob during his arrest.  I need to see what key he had on him.");
		addCompletedText(id, "Complete - View Jacob Arrest Evidence", "The key found on Jacob belongs to William.  According to William, he gave this key to Molly at the hospital.  It is really odd that he would have this key unless he met Molly before she went up to the apartment.");
		addNotAvailableText(id, "Video needs to be viewed evidence - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(32));
		addResultingVariables(id, getVar(38));
		addExcludeIfVariables(id, getVar(52));
		id = 30;
		addPlayGotoScene(id, "Call Officer Miller", "CallMillerDayCopy", 1, 20);
		addBodyText(id, "Miller is the officer that was at the scene of the crime.  He is the arresting officer and was also the first officer on the scene of the crime.");
		addCompletedText(id, "Complete - Call Officer Miller", "Miller provides some details on the case.   From his perspective, it is an open and closed case against Jacob.  He had the motive and the key to the penthouse.  He was not aware of anything being stolen from William's apartment.");
		addRequiredVariables(id, getVar(28));
		addRequiredVariables(id, getVar(40));
		addExcludeIfVariables(id, getVar(32));
		addExcludeIfVariables(id, getVar(52));
		id = 31;
		addPlayGotoScene(id, "Call Officer Miller", "CallMillerDay", 1, 20);
		addBodyText(id, "Miller is the officer that was at the scene of the crime.  He is the arresting officer and was also the first officer on the scene of the crime.");
		addCompletedText(id, "Complete - Call Officer Miller", "Miller provides some details on the case.   From his perspective, it is an open and closed case against Jacob.  He had the motive and the key to the penthouse.  He was not aware of anything being stolen from William's apartment.");
		addRequiredVariables(id, getVar(32));
		addResultingVariables(id, getVar(32));
		addExcludeIfVariables(id, getVar(52));
		id = 32;
		addPlayGotoScene(id, "Interview Lobby Guard", "InterviewLobbyGuard", 1, 30);
		addBodyText(id, "The lobby guard should have seen who came into the building when Molly was killed.  He/she would be a good person to interview.  It would be better to do this now since I am already here.");
		addCompletedText(id, "Completed - Interview Lobby Guard", "Jack Henry was little help and did see who killed Molly.  However, he did mention that the police have the security footage from the lobby.");
		addRequiredVariables(id, getVar(31));
		addResultingVariables(id, getVar(31));
		addExcludeIfVariables(id, getVar(52));
		id = 33;
		addPlayGotoScene(id, "Call Kyler for Help", "KylerHelpA", 1, 5);
		addBodyText(id, "If you get stuck, call Kyler and maybe he can help.");
		addRequiredVariables(id, getVar(40));
		addRequiredVariables(id, getVar(28));
		addExcludeIfVariables(id, getVar(52));
		id = 34;
		addPlayGotoScene(id, "Night call", "KylerNightCallA", 1, 5);
		addRequiredVariables(id, getVar(100));
		addExcludeIfVariables(id, getVar(52));
		id = 35;
		addPlayGotoScene(id, "Call Kyler about Shoe", "KylerShoeSize", 1, 15);
		addBodyText(id, "If you get stuck, call Kyler and maybe he can help.");
		addRequiredVariables(id, getVar(100));
		addExcludeIfVariables(id, getVar(52));
		id = 36;
		addPlayVideoReturn(id, "Basic Person Check on Officer Larry Miller", "FBISearchLarryMiller", 30);
		addBodyText(id, "Basic search might help with this case.  Missing solid evidence on this case and Kyler suggested I check out Officer Miller.");
		addCompletedText(id, "Complete - Basic Person Check on Officer Larry Miller", "Officer Larry Miller was born in the suburbs of Chicago.  He has over 30 years of experience in law enforcement and countless awards and community recognitions.  He moved to California 3 years ago - taking a lower ranking job with the SF police department.   Five years ago, he was allegedly caught shoplifting on video.  However, the case was dropped when the police department raised charges of illegal videotaping a police officer on duty, which is illegal in Illinois.");
		addNotAvailableText(id, "Basic Search needs to be done in secure environment - like at your home or FBI Office.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(43));
		addResultingVariables(id, getVar(44));
		addExcludeIfVariables(id, getVar(52));
		id = 37;
		addPlayVideoPuzzle(id, "Final - Put all the pieces together", 5, 90, "FinalPuzzle");
		addBodyText(id, "Using all the evidence you have found and processed, you can solve the case and determine who killed Molly.");
		addCompletedText(id, "Solved For Molly Forensics", "Jacob should have had Molly's keys instead he has William's.  Maybe the keys were planted on him.  The robbery is also odd and none of the stolen goods were found on Jacob including the missing cash.  The security camera was disabled by what looks like a nightstick.");
		addNotAvailableText(id, "Once you have all the evidence and solved all the other puzzles, you figure out who killed Molly.", isLocationSpecific: true);
		addRequiredVariables(id, getVar(35));
		addRequiredVariables(id, getVar(36));
		addRequiredVariables(id, getVar(34));
		addRequiredVariables(id, getVar(39));
		addRequiredVariables(id, getVar(44));
		addResultingVariables(id, getVar(45));
		addExcludeIfVariables(id, getVar(52));
		id = 38;
		addPlayGotoScene(id, "Case Closed", "DateNight", 1, 5);
		addRequiredVariables(id, getVar(100));
		addExcludeIfVariables(id, getVar(52));
	}

	public string getVar(int id)
	{
		foreach (VariableEngine.variableData variable in variableList)
		{
			if (variable.id == id)
			{
				return variable.variableName;
			}
		}
		return "Blank - Error";
	}

	public void addPlayVideoPuzzle(int id, string headerTxt, int puzzleId, int durationInMinutes, string videoName)
	{
		ResearchData researchData = new ResearchData();
		researchData.id = id;
		researchData.headerTxt = headerTxt;
		researchData.playVideoPuzzleId = puzzleId;
		researchData.baseDurationMinutes = durationInMinutes;
		researchData.type = ResearchData.activateType.PlayVideoPuzzle;
		researchData.playVideoName = videoName;
		addResearchData(researchData, checkDupesError: true);
	}

	public void addResearchData(ResearchData data, bool checkDupesError)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == data.id)
			{
				if (checkDupesError)
				{
					Console.WriteLine("Error - duplicate value " + data.id);
				}
				masterResearchList.Remove(masterResearch);
				break;
			}
		}
		masterResearchList.Add(data);
	}

	public void addRequiredVariables(int id, string text1)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.requiredVariables.Add(text1);
			}
		}
	}

	public void addResultingVariables(int id, string text1)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.resultingVariables.Add(text1);
			}
		}
	}

	public void addExcludeIfVariables(int id, string text1)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.excludeIfVariables.Add(text1);
			}
		}
	}

	public void addBodyText(int id, string text)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.bodyTxt = text;
				break;
			}
		}
	}

	public void addNotAvailableText(int id, string text, bool isLocationSpecific)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.notAvailableText = text;
				if (isLocationSpecific)
				{
					masterResearch.hasLocationRequirement = true;
				}
				break;
			}
		}
	}

	public void addPlayVideoReturn(int id, string headerTxt, string videoName, int durationInMinutes)
	{
		ResearchData researchData = new ResearchData();
		researchData.id = id;
		researchData.headerTxt = headerTxt;
		researchData.playVideoName = videoName;
		researchData.baseDurationMinutes = durationInMinutes;
		researchData.type = ResearchData.activateType.PlayVideoReturn;
		addResearchData(researchData, checkDupesError: true);
	}

	public void addPlayGotoScene(int id, string headerTxt, string sceneName, int sceneId, int durationInMinutes)
	{
		ResearchData researchData = new ResearchData();
		researchData.id = id;
		researchData.headerTxt = headerTxt;
		researchData.gotoSceneName = sceneName;
		researchData.gotoSceneId = sceneId;
		researchData.baseDurationMinutes = durationInMinutes;
		researchData.type = ResearchData.activateType.GotoScene;
		addResearchData(researchData, checkDupesError: true);
	}

	public void addCompletedText(int id, string header, string text)
	{
		foreach (ResearchData masterResearch in masterResearchList)
		{
			if (masterResearch.id == id)
			{
				masterResearch.completedheaderTxt = header;
				masterResearch.completedBodyTxt = text;
				break;
			}
		}
	}
}
