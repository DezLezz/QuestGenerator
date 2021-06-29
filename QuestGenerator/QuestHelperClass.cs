using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestGenerator
{
    static class QuestHelperClass
    {
        static Random rnd = new Random();

        public static Dictionary<string, int> goldPerAction = new Dictionary<string, int>() {
            {"capture",120 }, {"damage",150 }, {"exchange",80 }, {"explore",60 }, {"free",100 }, {"gather",70 }, {"give",70 }
        , {"goto",50 }, {"kill",150 }, {"listen",50 }, {"report",50 }, {"take",150 }, {"quest",50 }, {"use",70 }};

        public static Dictionary<string, string> descriptionChoser = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","There are some items I need delivered to me, think you could do that?" },
            {"Interview NPC","There's a person with information I require. Go talk to him and report back if the information is usefull." },
            //Comfort
            {"Obtain luxuries","There are some things I've been craving. Do you think you could bring them to me?" },
            {"Kill pests","A certain group of individuals have been wreaking havoc lately. I need you to get rid of them." },
            //Reputation
            {"Obtain rare items","This item has been lacking and is now rare. Find it and bring it to me." },
            {"Kill enemies","A few of my enemies have been more active recently. I need you to teach them a lesson." },
            {"Visit dangerous place","There's a place I need you to scout. Report back to me if you find anything worth considering." },
            //Serenity
            {"Revenge, Justice","These people have wronged me. I need you to take care of them so that justice can be served." },
            {"Capture Criminal","I need you to capture a certain criminal. I need help bringing him to justice." },
            {"Check on NPC","I need you to check up on someone and report back if there is any problem." },
            {"Recover lost/stolen item","I've lost a certain item, think you could get it back for me?" },
            {"Rescue NPC","One of my companions has been arrested. I need you to set him free." },
            //Protection
            {"Attack threatening entities","Some people have been threatening the well-being of our settlement. I need you to deal with them." },
            {"Capture Criminal","I need you to capture a certain criminal. I need help bringing him to justice." },
            {"Create Diversion","I'm planning a surprise attack on our enemies and I need you to distract them for me. Do you think you could create a diversion?" },
            {"Recruit","Find out if there are any soldiers worth recruiting nearby. Report back with your findings." },
            //Conquest
            {"Attack enemy","We're planning an attack on one of our enemies. Will you join us?" },
            {"Steal stuff","There're some items I need you to steal. Are you up for the task?" },
            {"Recruit","Find out if there are any soldiers worth recruiting nearby. Report back with your findings." },
            //Wealth
            {"Gather raw materials","There're some materials I need you to gather. Can you do that for me?" },
            {"Steal valuables for resale","There're some items I need you to steal. Are you up for the task?" },
            //Ability
            {"Practice combat","I advise you to train and polish your combat skills." },
            {"Practice skill","I advise you to train this skill for the future." },
            //Equipment
            {"Deliver supplies","We are in need some supplies in our settlement. Can you get them for us?" },
            {"Steal supplies","There're some items I need you to steal. Are you up for the task?" },
            {"Trade for supplies","I have some items here we can exchange. Would you be interested in taking a look?" }};

        public static Dictionary<string, string> listenChoser1 = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Interview NPC","Tell the Lord everything is going as planed. He can count on me." },
            //Comfort
            {"Obtain luxuries","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Kill pests","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            //Reputation
            {"Obtain rare items","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Kill enemies","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Visit dangerous place","A few suspicious characters have been seen roaming around lately. You should be careful." },
            //Serenity
            {"Revenge, Justice","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Check on NPC","Tell the Lord everything is fine." },
            {"Recover lost/stolen item","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Rescue NPC","You'll probably find who you're looking for in either a town or a castle." },
            //Protection
            {"Attack threatening entities","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Create Diversion","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Recruit","We have some good soldiers we can spare." },
            //Conquest
            {"Attack enemy","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Steal stuff","Your target should be carrying the items you're looking for, but it's not guaranteed." },
            {"Recruit","We have some good soldiers we can spare." },
            //Wealth
            {"Gather raw materials","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Steal valuables for resale","Your target should be carrying the items you're looking for, but it's not guaranteed." },
            //Ability
            {"Practice combat","Simply going into battle should help you polish your skills." },
            {"Practice skill","Depending on the skill you need to train, a different method might be required." },
            //Equipment
            {"Deliver supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Steal supplies","Your target should be carrying the items you're looking for, but it's not guaranteed." },
            {"Trade for supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item."}};

        public static Dictionary<string, string> listenChoser2 = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Interview NPC","Tell the lord there have been some issues with plans, but we'll still make it work." },
            //Comfort
            {"Obtain luxuries","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Kill pests","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            //Reputation
            {"Obtain rare items","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Kill enemies","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Visit dangerous place","There doesn't seem to be any problem nearby." },
            //Serenity
            {"Revenge, Justice","These people have wronged me. I need you to take care of them so that justice can be served." },
            {"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Check on NPC","Tell the Lord there have been some problems recently and help might be required." },
            {"Recover lost/stolen item","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Rescue NPC","You'll probably find who you're looking for in either a town or a castle."  },
            //Protection
            {"Attack threatening entities","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Create Diversion","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Recruit","Unfortunately we can't spare any soldiers." },
            //Conquest
            {"Attack enemy","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
            {"Steal stuff","Your target should be carrying the items you're looking for, but it's not guaranteed." },
            {"Recruit","Unfortunately we can't spare any soldiers." },
            //Wealth
            {"Gather raw materials","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Steal valuables for resale","Your target should be carrying the items you're looking for, but it's not guaranteed." },
            //Ability
            {"Practice combat","Simply going into battle should help you polish your skills." },
            {"Practice skill","Depending on the skill you need to train, a different method might be required." },
            //Equipment
            {"Deliver supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Steal supplies","Your target should be carrying the items you're looking for, but it's not guaranteed."},
            {"Trade for supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." }};

        public static Dictionary<string, string> reportChoser1 = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","How did you get here?" },
            {"Interview NPC","I've come to report that everything is moving along fine." },
            //Comfort
            {"Obtain luxuries","How did you get here?" },
            {"Kill pests","The ones that troubled you have been dealt with." },
            //Reputation
            {"Obtain rare items","How did you get here?" },
            {"Kill enemies","The ones that troubled you have been dealt with."  },
            {"Visit dangerous place","I've come to report that there are some enemies roaming around that place." },
            //Serenity
            {"Revenge, Justice","The ones that troubled you have been dealt with." },
            {"Capture Criminal","I've captured the criminal." },
            {"Check on NPC","I've come to report that everything is fine." },
            {"Recover lost/stolen item","How did you get here?" },
            {"Rescue NPC","I've rescued your comrade."  },
            //Protection
            {"Attack threatening entities","The ones that troubled you have been dealt with." },
            {"Capture Criminal","The criminal has been captured." },
            {"Create Diversion","I've created a diversion successfully."  },
            {"Recruit","I've come to report that they have soldiers to spare." },
            //Conquest
            {"Attack enemy","I've completed my attack on the enemy." },
            {"Steal stuff","How did you get here?" },
            {"Recruit","I've come to report that they have soldiers to spare." },
            //Wealth
            {"Gather raw materials","I've gathered the materiasl." },
            {"Steal valuables for resale","How did you get here?" },
            //Ability
            {"Practice combat","I've trained in the art of combat." },
            {"Practice skill","I've completed my training on the suggested skill."},
            //Equipment
            {"Deliver supplies","How did you get here?" },
            {"Steal supplies","How did you get here?"},
            {"Trade for supplies","How did you get here?" }};

        public static Dictionary<string, string> reportChoser2 = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
            {"Interview NPC","I've come to report that there have been some issues, but they'll make it work somehow."  },
            //Comfort
            {"Obtain luxuries","How did you get here?" },
            {"Kill pests","The ones that troubled you have been dealt with."  },
            //Reputation
            {"Obtain rare items","How did you get here?" },
            {"Kill enemies","The ones that troubled you have been dealt with."  },
            {"Visit dangerous place","I've come to report that the information you received about that place might have been wrong." },
            //Serenity
            {"Revenge, Justice","The ones that troubled you have been dealt with." },
            {"Capture Criminal","The criminal has been captured." },
            {"Check on NPC","I've come to report that there have been some problems recently and that they might require help." },
            {"Recover lost/stolen item","How did you get here?" },
            {"Rescue NPC","I've rescued your comrade."  },
            //Protection
            {"Attack threatening entities","The ones that troubled you have been dealt with." },
            {"Capture Criminal","I've captured the criminal." },
            {"Create Diversion","I've created a diversion successfully." },
            {"Recruit","I've come to report that they don't have soldiers to spare." },
            //Conquest
            {"Attack enemy","I've completed my attack on the enemy." },
            {"Steal stuff","How did you get here?" },
            {"Recruit","I've come to report that they don't have soldiers to spare." },
            //Wealth
            {"Gather raw materials","I've gathered the materiasl." },
            {"Steal valuables for resale","How did you get here?" },
            //Ability
            {"Practice combat","I've trained in the art of combat." },
            {"Practice skill","I've completed my training on the suggested skill." },
            //Equipment
            {"Deliver supplies","How did you get here?" },
            {"Steal supplies","How did you get here?"},
            {"Trade for supplies","How did you get here?" }};

        public static int GoldCalculator(List<actionTarget> actions)
        {
            int result = 0;
            foreach (actionTarget action in actions)
            {
                result += goldPerAction[action.action];
            }
            return result;
        }

        public static string DescriptionCalculator(string strategy)
        {
            return descriptionChoser[strategy];
        }

        public static string ListenDialog(string strategy, int pair)
        {
            if (pair ==1)
            {
                return listenChoser1[strategy];
            }
            else
            {
                return listenChoser2[strategy];
            }
        }

        public static string ReportDialog(string strategy, int pair)
        {
            if (pair == 1)
            {
                return reportChoser1[strategy];
            }
            else
            {
                return reportChoser2[strategy];
            }
        }
    }
}
