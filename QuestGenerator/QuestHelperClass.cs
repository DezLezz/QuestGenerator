using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ThePlotLords
{
    public class QuestHelperClass
    {
        static Random rnd = new Random();

        public QuestHelperClass() { }

        public static List<int> motivationOccurrences = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public static Dictionary<string, int> goldPerAction = new Dictionary<string, int>() {
            {"capture",120 }, {"damage",150 }, {"exchange",80 }, {"explore",60 }, {"free",100 }, {"gather",70 }, {"give",70 }
        , {"goto",50 }, {"kill",150 }, {"listen",50 }, {"report",50 }, {"take",150 }, {"quest",50 }, {"use",70 }};

        public static Dictionary<string, float> daysPerAction = new Dictionary<string, float>() {
            {"capture",30.0f }, {"damage",30.0f }, {"exchange",10.0f }, {"explore",15.0f }, {"free",30.0f }, {"gather",15.0f }, {"give",10.0f }
        , {"goto",10.0f }, {"kill",30.0f }, {"listen",10.0f }, {"report",10.0f }, {"take",30.0f }, {"quest",40.0f }, {"use",40.0f }};

        public static Dictionary<string, string> motivationDesc = new Dictionary<string, string>() {
            {"Knowledge", "I have a thirst for knowledge!" },
            {"Comfort", "I need you to make life a bit more comfortable for me." },
            {"Reputation", "I want to invest in a champion to boost my reputation around here." },
            {"Serenity", "Life has been a bit chaotic lately and I feel like it needs to calm down a bit." },
            {"Protection", "The region has been dangerous lately. I need you to do something about that." },
            {"Conquest", "It's about time we expand our influence around here. Let us conquer the world." },
            {"Wealth", "I desiree riches." },
            {"Ability", "I see something in you, but you're still a bit green. Go train your abilities so that you can be a great asset to me later." },
            {"Equipment", "I need to replenish my item stock." }};

        public static Dictionary<string, string> subquest_Prepare = new Dictionary<string, string>() {
            {"Knowledge", "You should learn a thing or two before continuing your task. I'll help you out with it, since I own one to {HERO}. " },
            {"Comfort", "I've helped out {HERO} before, but he hasn't paid me back, so I'm going to borrow you for a bit. " },
            {"Reputation", "I've helped out {HERO} before, but he hasn't paid me back, so I'm going to borrow you for a bit. " },
            {"Serenity", "I know you're busy with your mission, but I really need your help with a few dangerous subjects. " },
            {"Protection", "I know you're busy with your mission, but I really need your help with a few dangerous subjects. " },
            {"Conquest", "I've helped out {HERO} before, but he hasn't paid me back, so I'm going to borrow you for a bit. " },
            {"Wealth", "I've helped out {HERO} before, but he hasn't paid me back, so I'm going to borrow you for a bit. " },
            {"Ability", "{HERO} has given you a task right? Hmm... But you don't look ready for it yet. I own one to {HERO} so let me help you prepare for it. " },
            {"Equipment", "{HERO} borrowed a few items from me before, but he hasn't given them back, so you're going to get some new ones for me. " }};

        public static Dictionary<string, string> descriptionChoser = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","I need you to deliver a certain item to me, think you could do that?" },
            {"Interview NPC","There's a person with information I require. Go talk to him and report back to me or to one of my companions if the information is usefull." },
            //Comfort
            {"Obtain luxuries","There are some things I've been craving. Do you think you could bring them to me?" },
            {"Kill pests","A certain group of individuals have been wreaking havoc lately. I need you to get rid of them." },
            //Reputation
            {"Obtain rare items","This item has been lacking and is now rare. Find it and bring it to me." },
            {"Kill enemies","A few of my enemies have been more active recently. I need you to teach them a lesson." }, 
            {"Visit dangerous place","There's a place I need you to scout. Report back to me or to one of my companions if you find anything worth considering." },
            //Serenity
            {"Revenge, Justice","These people have wronged me. I need you to take care of them so that justice can be served." },
            {"Capture Criminal","I need you to capture a certain criminal. I need help bringing him to justice." },
            {"Check on NPC","I need you to check up on someone and report back to me or to one of my companions if there is any problem." },
            {"Recover lost/stolen item","I've lost a certain item, think you could get it back for me?" },
            {"Rescue NPC","One of my companions has been arrested. I need you to set him free." },
            //Protection
            {"Attack threatening entities","Some people have been threatening the well-being of our settlement. I need you to deal with them." },
            {"Create Diversion","I'm planning a surprise attack on our enemies and I need you to distract them for me. Do you think you could create a diversion?" },
            {"Recruit","Find out if there are any soldiers worth recruiting nearby. Report back to me or to one of my companions with your findings." },
            //Conquest
            {"Attack enemy","We're planning an attack on one of our enemies. Will you join us?" },
            {"Steal stuff","There're some items I need you to steal. Are you up for the task?" },
            //Wealth
            {"Gather raw materials","There're some materials I need you to gather. Can you do that for me?" },
            {"Steal valuables for resale","There're some items I need you to steal. Are you up for the task?" },
            //Ability
            {"Practice combat","I advise you to train and polish your combat skills." },
            {"Practice skill","I advise you to train this skill for the future." },
            //Equipment
            {"Deliver supplies","We are in need of some supplies in our settlement. Can you get them for us?" },
            {"Steal supplies","There're some items I need you to steal. Are you up for the task?" },
            {"Trade for supplies","I have some items here we can exchange. Would you be interested in taking a look?" }};

        public static Dictionary<string, string> listenPlayerDialog1 = new Dictionary<string, string>() {
            //Knowledge
            {"Interview NPC","{QUEST_GIVER.LINK} has been looking for a man capable of fixing a few of his wagons for him, do you think you're up for the job?" },
            //Comfort
            //Reputation
            //Serenity
            {"Check on NPC","{QUEST_GIVER.LINK} has heard about the recent attacks near the settlement and has sent me to check how everything is." },
            //Protection
            {"Recruit","{QUEST_GIVER.LINK} needs some soldiers to help protect his settlement, think you could spare some?" },
            //Conquest
            //Wealth
            //Ability
            //Equipment
            };

        public static Dictionary<string, string> listenPlayerDialog2 = new Dictionary<string, string>() {
            //Knowledge
            {"Interview NPC","{QUEST_GIVER.LINK} has heard of a blacksmith with highly praised skills in this settlement and would like to hire him to forge some tools. Do you know anything about him?" },
            //Comfort
            //Reputation
            //Serenity
            {"Check on NPC","{QUEST_GIVER.LINK}'s son was sent to this settlement by horse a couple of days ago but no news of him have been heard. Have you seen him?" },
            //Protection
            {"Recruit","{QUEST_GIVER.LINK} has had a plentiful harvest, but lacks labourers to collect it all, do you think you could spare a few man for him?" },
            //Conquest
            //Wealth
            //Ability
            //Equipment
            };

        public static Dictionary<string, string> listenChoser1 = new Dictionary<string, string>() {
            //Knowledge
            {"Interview NPC","Yes, I have someone that can be of assistance. I'll send him right away." },
            //Comfort
            //Reputation
            //Serenity
            {"Check on NPC","Everything is under control already. We did have a small group of bandits causing trouble, but they have been dealt with already." },
            //Protection
            {"Recruit","Yes, we have some soldiers we can spare. I'll send them right away." },
            //Conquest
            //Wealth
            //Ability
            //Equipment
            };
        ////Knowledge
        //{"Deliver item for study","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Interview NPC","Tell the Lord everything is going as planed. He can count on me." },
        ////Comfort
        //{"Obtain luxuries","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Kill pests","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        ////Reputation
        //{"Obtain rare items","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Kill enemies","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Visit dangerous place","A few suspicious characters have been seen roaming around lately. You should be careful." },
        ////Serenity
        //{"Revenge, Justice","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Check on NPC","Tell the Lord everything is fine." },
        //{"Recover lost/stolen item","The item you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Rescue NPC","You'll probably find who you're looking for in either a town or a castle." },
        ////Protection
        //{"Attack threatening entities","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Create Diversion","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Recruit","We have some good soldiers we can spare." },
        ////Conquest
        //{"Attack enemy","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Steal stuff","Your target should be carrying the items you're looking for, but it's not guaranteed." },
        ////Wealth
        //{"Gather raw materials","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Steal valuables for resale","Your target should be carrying the items you're looking for, but it's not guaranteed." },
        ////Ability
        //{"Practice combat","Simply going into battle should help you polish your skills." },
        //{"Practice skill","Depending on the skill you need to train, a different method might be required." },
        ////Equipment
        //{"Deliver supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Steal supplies","Your target should be carrying the items you're looking for, but it's not guaranteed." },
        //{"Trade for supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item."}};

        public static Dictionary<string, string> listenChoser2 = new Dictionary<string, string>() {
            //Knowledge
            {"Interview NPC","Hmm... Yes I might know who he's talking about. I'll talk to him. Knowing him he'll most definitely accept the job, so I'll also send him right away." },
            //Comfort
            //Reputation
            //Serenity
            {"Check on NPC","Yes. He had a troublesome encounter with some bandits when he was reaching our settlement, but luckily was able to get rid of them. He's just healing is wounds now, you can tell his father he should be back in a couple of days." },
            //Protection
            {"Recruit","I have a few spare labourers, yes. I'll send them right away." },
            //Conquest
            //Wealth
            //Ability
            //Equipment
            };
        ////Knowledge
        //{"Deliver item for study","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Interview NPC","Tell the lord there have been some issues with plans, but we'll still make it work." },
        ////Comfort
        //{"Obtain luxuries","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Kill pests","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        ////Reputation
        //{"Obtain rare items","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Kill enemies","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Visit dangerous place","There doesn't seem to be any problem nearby." },
        ////Serenity
        //{"Revenge, Justice","These people have wronged me. I need you to take care of them so that justice can be served." },
        //{"Capture Criminal","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Check on NPC","Tell the Lord there have been some problems recently and help might be required." },
        //{"Recover lost/stolen item","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Rescue NPC","You'll probably find who you're looking for in either a town or a castle."  },
        ////Protection
        //{"Attack threatening entities","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Create Diversion","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Recruit","Unfortunately we can't spare any soldiers." },
        ////Conquest
        //{"Attack enemy","You can probably find who you're looking for roaming around this settlement. Otherwise I suggest trying to retrace their steps." },
        //{"Steal stuff","Your target should be carrying the items you're looking for, but it's not guaranteed." },
        ////Wealth
        //{"Gather raw materials","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Steal valuables for resale","Your target should be carrying the items you're looking for, but it's not guaranteed." },
        ////Ability
        //{"Practice combat","Simply going into battle should help you polish your skills." },
        //{"Practice skill","Depending on the skill you need to train, a different method might be required." },
        ////Equipment
        //{"Deliver supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." },
        //{"Steal supplies","Your target should be carrying the items you're looking for, but it's not guaranteed."},
        //{"Trade for supplies","The items you're looking for can probably be found in a settlement and you can trade for them. However there's chance that you might have to craft the required item." }};

        public static Dictionary<string, string> reportChoser1 = new Dictionary<string, string>() {
            //Knowledge
            {"Deliver item for study","How did you get here?" },
            {"Interview NPC","A cabable man is already on his away to come and repair the wagons." },
            //Comfort
            {"Obtain luxuries","How did you get here?" },
            {"Kill pests","The ones that troubled you have been dealt with." },
            //Reputation
            {"Obtain rare items","How did you get here?" },
            {"Kill enemies","The ones that troubled you have been dealt with."  },
            {"Visit dangerous place","I've come to report that there are some enemies roaming around that place." },
            //Serenity
            {"Revenge, Justice","The ones that troubled you have been dealt with." },
            {"Capture Criminal","The criminal has been captured and delievered." },
            {"Check on NPC","There were a few bandit attacks, however they have already been dealt with and everything is fine." },
            {"Recover lost/stolen item","How did you get here?" },
            {"Rescue NPC","Your comrade has been rescued and is already walking around free."  },
            //Protection
            {"Attack threatening entities","The ones that troubled you have been dealt with." },
            {"Create Diversion","I've created a diversion successfully. You should take the opportunity and attack the enemy."  },
            {"Recruit","They had some soldiers to spare and they are already on their way here." },
            //Conquest
            {"Attack enemy","I've completed my attack on the enemy." },
            {"Steal stuff","How did you get here?" },
            //Wealth
            {"Gather raw materials","The materials have been gathered as requested." },
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
            {"Deliver item for study","How did you get here?" },
            {"Interview NPC","The praised blacksmith did exist and he's already on is way." },
            //Comfort
            {"Obtain luxuries","How did you get here?" },
            {"Kill pests","The ones that troubled you have been dealt with." },
            //Reputation
            {"Obtain rare items","How did you get here?" },
            {"Kill enemies","The ones that troubled you have been dealt with."  },
            {"Visit dangerous place","I've come to report that there are some enemies roaming around that place." },
            //Serenity
            {"Revenge, Justice","The ones that troubled you have been dealt with." },
            {"Capture Criminal","The criminal has been captured and delievered." },
            {"Check on NPC","Your son had a small encounter with a group of bandits, but everything is fine. He's just healing and should be back in a couple of days." },
            {"Recover lost/stolen item","How did you get here?" },
            {"Rescue NPC","Your comrade has been rescued and is already walking around free."  },
            //Protection
            {"Attack threatening entities","The ones that troubled you have been dealt with." },
            {"Create Diversion","I've created a diversion successfully. You should take the opportunity and attack the enemy."  },
            {"Recruit","They had some labourers to spare and they are already on their way here." },
            //Conquest
            {"Attack enemy","I've completed my attack on the enemy." },
            {"Steal stuff","How did you get here?" },
            //Wealth
            {"Gather raw materials","The materials have been gathered as requested." },
            {"Steal valuables for resale","How did you get here?" },
            //Ability
            {"Practice combat","I've trained in the art of combat." },
            {"Practice skill","I've completed my training on the suggested skill."},
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

        public static float TimeCalculator(List<actionTarget> actions)
        {
            float result = 0.0f;
            foreach (actionTarget action in actions)
            {
                result += daysPerAction[action.action];
            }
            return result;
        }

        public static string DescriptionCalculator(string strategy)
        {
            return descriptionChoser[strategy];
        }

        public static string MotivationCalculator(string strategy)
        {
            return motivationDesc[strategy];
        }

        public static string SubquestPrepare(string strategy)
        {
            return subquest_Prepare[strategy];
        }

        public static string ListenPlayerDialog(string strategy, int pair)
        {
            if (pair == 1)
            {
                return listenPlayerDialog1[strategy];
            }
            else
            {
                return listenPlayerDialog2[strategy];
            }
        }

        public static string ListenDialog(string strategy, int pair)
        {
            if (pair == 1)
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

        public static void MotivationGiver()
        {
            
            var heroList = Hero.AllAliveHeroes;

            foreach (Hero h in heroList)
            {
                //List<float> weights = new List<float>() { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                if (!QuestGenTestCampaignBehavior.HeroMotivations.ContainsKey(h))
                {
                    //if (h.IsFactionLeader)
                    //{
                    //    weights[1] += 0.05f;
                    //    weights[2] += 0.05f;
                    //    weights[6] += 0.05f;
                    //}

                    //var mp = new MobilePartiesAroundPositionList(32);
                    //List<MobileParty> closeParties = new List<MobileParty>();

                    //if (h.CurrentSettlement != null)
                    //{
                    //    closeParties = mp.GetPartiesAroundPosition(h.CurrentSettlement.Position2D, 100);

                    //    if (!closeParties.IsEmpty())
                    //    {
                    //        foreach (MobileParty m in closeParties)
                    //        {
                    //            if (h.MapFaction != null && m.MapFaction != null)
                    //            {
                    //                if (m.IsBandit || h.MapFaction.IsAtWarWith(m.MapFaction))
                    //                {
                    //                    weights[3] += 0.05f;
                    //                    weights[4] += 0.05f;
                    //                    break;
                    //                }
                    //            }

                    //        }
                    //    }

                    //    if (h.CurrentSettlement.Village != null)
                    //    {
                    //        if (h.CurrentSettlement.Village.Bound.Town.Security <= 50f)
                    //        {
                    //            weights[3] += 0.05f;
                    //            weights[4] += 0.05f;
                    //        }

                    //        if (h.CurrentSettlement.Village.Bound.Town.Prosperity <= 100)
                    //        {
                    //            weights[6] += 0.05f;
                    //            weights[8] += 0.05f;
                    //        }
                    //    }
                    //    else if (h.CurrentSettlement.Town != null)
                    //    {
                    //        if (h.CurrentSettlement.Town.Security <= 50f)
                    //        {
                    //            weights[3] += 0.05f;
                    //            weights[4] += 0.05f;
                    //        }

                    //        if (h.CurrentSettlement.Town.Prosperity <= 100)
                    //        {
                    //            weights[6] += 0.05f;
                    //            weights[8] += 0.05f;
                    //        }
                    //    }


                    //}
                    //float sum = 0.0f;
                    //for (int i = 0; i < weights.Count; i++)
                    //{
                    //    sum += weights[i];
                    //}

                    //float parcel = sum / weights.Count;
                    //for (int i = 0; i < weights.Count; i++)
                    //{
                    //    weights[i] += parcel;
                    //}

                    //QuestHelperClass qc = new QuestHelperClass();
                    //int choice = qc.weightedChoice(weights);
                    List<string> motivations = new List<string>() { "Knowledge", "Comfort", "Reputation", "Serenity", "Protection", "Conquest", "Wealth", "Ability", "Equipment" };

                    if (h.IsRuralNotable)
                    {
                        motivations.Remove("Reputation");
                        motivations.Remove("Ability");
                        motivations.Remove("Comfort");
                    }

                    int r = rnd.Next(motivations.Count);
                    QuestGenTestCampaignBehavior.HeroMotivations.Add(h, motivations[r]);
                }
            }

        }



        public static void MotivationGiverOneHero(Hero h)
        {
            List<string> motivations = new List<string>() { "Knowledge", "Comfort", "Reputation", "Serenity", "Protection", "Conquest", "Wealth", "Ability", "Equipment" };

            //List<float> weights = new List<float>() { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

            //if (h.IsFactionLeader)
            //{
            //    weights[1] += 0.1f;
            //    weights[2] += 0.1f;
            //    weights[6] += 0.1f;
            //}

            //var mp = new MobilePartiesAroundPositionList(32);
            //List<MobileParty> closeParties = new List<MobileParty>();

            //if (h.CurrentSettlement != null)
            //{
            //    closeParties = mp.GetPartiesAroundPosition(h.CurrentSettlement.Position2D, 100);

            //    if (!closeParties.IsEmpty())
            //    {
            //        foreach (MobileParty m in closeParties)
            //        {
            //            if (h.MapFaction != null && m.MapFaction != null)
            //            {
            //                if (m.IsBandit || h.MapFaction.IsAtWarWith(m.MapFaction))
            //                {
            //                    weights[3] += 0.05f;
            //                    weights[4] += 0.05f;
            //                    break;
            //                }
            //            }
            //        }
            //    }

            //    if (h.CurrentSettlement.Village != null)
            //    {
            //        if (h.CurrentSettlement.Village.Bound.Town.Security <= 50f)
            //        {
            //            weights[3] += 0.05f;
            //            weights[4] += 0.05f;
            //        }

            //        if (h.CurrentSettlement.Village.Bound.Town.Prosperity <= 100)
            //        {
            //            weights[6] += 0.05f;
            //            weights[8] += 0.05f;
            //        }
            //    }
            //    else if (h.CurrentSettlement.Town != null)
            //    {
            //        if (h.CurrentSettlement.Town.Security <= 50f)
            //        {
            //            weights[3] += 0.05f;
            //            weights[4] += 0.05f;
            //        }

            //        if (h.CurrentSettlement.Town.Prosperity <= 100)
            //        {
            //            weights[6] += 0.05f;
            //            weights[8] += 0.05f;
            //        }
            //    }
            //}
            //float sum = 0.0f;
            //for (int i = 0; i < weights.Count; i++)
            //{
            //    sum += weights[i];
            //}

            //float parcel = sum / weights.Count;
            //for (int i = 0; i < weights.Count; i++)
            //{
            //    weights[i] += parcel;
            //}
            //QuestHelperClass qc = new QuestHelperClass();
            //int choice = qc.weightedChoice(weights);

            if (h.IsRuralNotable)
            {
                motivations.Remove("Reputation");
                motivations.Remove("Ability");
                motivations.Remove("Comfort");
            }

            int r = rnd.Next(motivations.Count);
            QuestGenTestCampaignBehavior.HeroMotivations[h] = motivations[r];

        }
        public int weightedChoice(List<float> weights)
        {
            var rand = new Random();

            var r = rand.NextDouble();
            float limit1 = 0.0f;
            float limit2 = 0.0f;
            int count = weights.Count;

            for (int i = 0; i < count; i++)
            {
                limit2 += weights[i];
                if (r >= limit1 && r <= limit2)
                {
                    return i;
                }
                else
                {
                    limit1 = limit2;
                }
            }

            int r1 = rand.Next(count);

            return r1;
        }
    }
}
