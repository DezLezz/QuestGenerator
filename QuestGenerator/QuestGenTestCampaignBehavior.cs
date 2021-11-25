using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;

namespace ThePlotLords
{
    public class QuestGenTestCampaignBehavior : CampaignBehaviorBase
    {

        private const IssueBase.IssueFrequency QuestTestIssueFrequency = IssueBase.IssueFrequency.Common;

        [SaveableField(100)]
        public static Dictionary<Hero, string> HeroMotivations;

        public static PlayerData player_data = new PlayerData();

        [SaveableField(119)]
        public static int player_data_ID;

        [SaveableField(120)]
        public static bool initial_quests_created = false;

        [SaveableField(121)]
        public static int quests_created;

        private string path = @"..\..\Modules\ThePlotLords\MissionList.xml";

        public static Generator QuestGen = new Generator(0);

        private List<CustomBTNode> missions;
        private CustomBTNode chosenMission;
        static Random rnd = new Random();

        //[SaveableField(103)]

        public QuestGenTestCampaignBehavior()
        {
            DirectoryInfo di = new DirectoryInfo(@"..\..\Modules\ThePlotLords\PlayerData\");
            FileInfo[] files = di.GetFiles("*.txt");

            if (files.Length > 0)
            {
                player_data_ID = Int32.Parse(files[0].ToString().Replace(".txt", ""));
            }
            else
            {
                int r = rnd.Next(1, 999999);
                player_data_ID = r;
            }

            if (new FileInfo(path).Length != 0)
            {
                missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
            }
            else
            {
                missions = new List<CustomBTNode>();
            }

            HeroMotivations = new Dictionary<Hero, string>();
            string playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
            if (File.Exists(playerData))
            {
                player_data = JsonSerialization.ReadFromJsonFile<PlayerData>(playerData);
                if (player_data == null)
                {
                    player_data = new PlayerData();
                    player_data.player_Run_ID = player_data_ID;
                    playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
                    JsonSerialization.WriteToJsonFile<PlayerData>(playerData, player_data);
                }
            }
            else
            {
                player_data.player_Run_ID = player_data_ID;
                playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
                JsonSerialization.WriteToJsonFile<PlayerData>(playerData, player_data);
            }

        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
            //CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new System.Action(this.HourlyTick));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new System.Action(dailyTickEvent));
        }

        public void dailyTickEvent()
        {
            if (quests_created < 150)
            {
                for (int i = 0; i < 7; i++)
                {
                    int r = rnd.Next(Settlement.All.Count);

                    if (Settlement.All.Count > 0)
                    {
                        if (Settlement.All[r] != null)
                        {
                            if (Settlement.All[r].Notables != null)
                            {
                                var notables = Settlement.All[r].Notables;

                                int r2 = rnd.Next(notables.Count);

                                if (notables.Count > 0)
                                {
                                    if (notables[r2] != null)
                                    {
                                        if (notables[r2].CanHaveQuestsOrIssues() && this.ConditionsHold(notables[r2]))
                                        {
                                            PotentialIssueData potentialIssueData = new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common);
                                            Campaign.Current.IssueManager.CreateNewIssue(potentialIssueData, notables[r2]);
                                            quests_created++;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        public void TickCampaignBehavior()
        {
            if (Input.IsKeyDown(InputKey.LeftControl))
            {
                if (Input.IsKeyReleased(InputKey.T))
                {
                    foreach (IssueBase i in Campaign.Current.IssueManager.Issues.Values)
                    {
                        if (i.GetType().Namespace.Contains("ThePlotLords"))
                        {
                            if (i.IssueOwner != null)
                            {
                                if (i.IssueOwner.CurrentSettlement != null)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage("PL quest: " + i.IssueOwner.Name.ToString() + " in " + i.IssueOwner.CurrentSettlement.ToString()));
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage("PL quest: " + i.IssueOwner.Name.ToString()));
                                }
                            }
                        }
                        //if (i.GetType().Name.ToString() == "LandlordTrainingForRetainersIssue")
                        //{
                        //    if (i.IssueOwner != null)
                        //    {
                        //        if (i.IssueOwner.CurrentSettlement != null)
                        //        {
                        //            InformationManager.DisplayMessage(new InformationMessage("lord train quest: " + i.IssueOwner.Name.ToString() + " in " + i.IssueOwner.CurrentSettlement.ToString()));
                        //        }
                        //        else
                        //        {
                        //            InformationManager.DisplayMessage(new InformationMessage("lord train quest: " + i.IssueOwner.Name.ToString()));
                        //        }
                        //    }
                        //}
                    }

                }
            }
        }


        private void OnCheckForIssue(Hero hero)
        {

            if (HeroMotivations.Count == 0)
            {
                QuestHelperClass.MotivationGiver();
            }

            if (!initial_quests_created)
            {
                int counter = 0;
                foreach (Settlement s in Settlement.All)
                {
                    if (s.Culture != null)
                    {
                        if (s.Culture.ToString() == "Empire")
                        {
                            foreach (Hero h in s.Notables)
                            {
                                if (h.CanHaveQuestsOrIssues())
                                {
                                    int r = rnd.Next(1, 6);

                                    if (this.ConditionsHold(h) && r == 1)
                                    {
                                        PotentialIssueData potentialIssueData = new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common);
                                        Campaign.Current.IssueManager.CreateNewIssue(potentialIssueData, h);
                                        counter++;
                                        quests_created++;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (counter >= 3)
                    {
                        break;
                    }
                }
                initial_quests_created = true;
            }

        }

        public IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
        {

            PotentialIssueData potentialIssueData = pid;

            player_data.player_CreatedQuests++;
            string playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
            JsonSerialization.WriteToJsonFile<PlayerData>(playerData, player_data);

            string motiv = "none";
            motiv = HeroMotivations[issueOwner];

            foreach (CustomBTNode btNode in missions)
            {
                if (btNode.name == motiv)
                {
                    chosenMission = btNode;
                    missions.Remove(btNode);
                    XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, missions);

                    break;
                }
            }

            if (issueOwner == null)
            {
                return null;
            }

            QuestHelperClass.MotivationGiverOneHero(issueOwner);

            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, chosenMission);
        }

        private bool ConditionsHold(Hero issueGiver)
        {
            if (issueGiver == null)
            {
                return false;
            }

            if (issueGiver.CurrentSettlement == null)
            {
                return false;
            }

            string motiv = "none";

            if (HeroMotivations.ContainsKey(issueGiver))
            {
                motiv = HeroMotivations[issueGiver];
            }
            else
            {
                List<string> motivations = new List<string>() { "Knowledge", "Comfort", "Reputation", "Serenity", "Protection", "Conquest", "Wealth", "Ability", "Equipment" };
                int r = rnd.Next(motivations.Count);
                HeroMotivations.Add(issueGiver, motivations[r]);
            }

            if (missions.IsEmpty() && new FileInfo(path).Length != 0)
            {
                missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
            }

            if (motiv == "none" || motiv == "WaitingForSubQuest")
            {
                return false;
            }

            if (missions.IsEmpty())
            {
                int i = rnd.Next(1, 6);
                if (i == 1)
                {
                    QuestGen.GenerateOne();
                    missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
                }
                else
                {
                    return false;
                }

            }

            foreach (CustomBTNode btNode in missions)
            {
                if (btNode.nodeType == CustomBTType.motivation)
                {
                    if (btNode.name == motiv)
                    {
                        return true;
                    }
                }
                else
                {
                    if (btNode.Children[0].name == motiv)
                    {
                        return true;
                    }
                }


            }

            return false;
        }


        public override void SyncData(IDataStore dataStore)
        {
        }
        public class QuestGenTestCampaignBehaviorTypeDefiner : SaveableTypeDefiner
        {

            // Token: 0x06003689 RID: 13961 RVA: 0x000E7A64 File Offset: 0x000E5C64
            public QuestGenTestCampaignBehaviorTypeDefiner() : base(424242)
            {
            }

            // Token: 0x0600368A RID: 13962 RVA: 0x000E7A71 File Offset: 0x000E5C71
            protected override void DefineClassTypes()
            {
                base.AddClassDefinition(typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), 1);
                base.AddClassDefinition(typeof(QuestGenTestCampaignBehavior.QuestGenTestQuest), 2);
            }
        }
        public class QuestGenTestIssue : IssueBase
        {
            [SaveableField(115)]
            public Hero missionHero;

            [SaveableField(116)]
            public string id = "";

            [SaveableField(117)]
            public int ListenReportPair = rnd.Next(1, 3);

            public CustomBTNode chosenMission;

            public CustomBTNode alternativeMission;

            //public List<string> subQuests;

            //public int subQuestCounter;
            public List<JournalLog> journalLogs = new List<JournalLog>();

            public List<actionTarget> actionsInOrder;

            public List<actionTarget> alternativeActionsInOrder;

            protected override bool IssueQuestCanBeDuplicated {
                get {
                    return true;
                }
            }
            public QuestGenTestIssue(Hero issueOwner, CustomBTNode chosenMission) : base(issueOwner, CampaignTime.DaysFromNow(200f))
            {
                missionHero = issueOwner;
                actionsInOrder = new List<actionTarget>();

                if (issueOwner != null)
                {
                    if (issueOwner.CurrentSettlement != null)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Plot Lords quest added: " + issueOwner.Name.ToString() + " in " + issueOwner.CurrentSettlement.ToString()));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Plot Lords quest added: " + issueOwner.Name.ToString()));
                    }
                }
                quests_created++;
                if (chosenMission.nodeType == CustomBTType.motivation)
                {
                    this.chosenMission = chosenMission;
                    this.chosenMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, false);
                    this.chosenMission.run(CustomBTStep.issueQ, (IssueBase)this, this, false);

                }
                else
                {
                    this.chosenMission = chosenMission.Children[0];
                    this.chosenMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, false);
                    this.chosenMission.run(CustomBTStep.issueQ, (IssueBase)this, this, false);

                    alternativeActionsInOrder = new List<actionTarget>();
                    alternativeMission = chosenMission.Children[1];
                    alternativeMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, true);
                    alternativeMission.run(CustomBTStep.issueQ, (IssueBase)this, this, true);

                }
                id = this.IssueOwner.Name.ToString() + (int)this.IssueOwner.Age;
                this.saveMissions();
            }

            private void saveMissions()
            {

                string path2 = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + ".xml";

                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { chosenMission });

                if (alternativeMission != null)
                {
                    string path2_alt = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml";
                    XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2_alt, new List<CustomBTNode>() { alternativeMission });
                }
            }

            protected override int RewardGold {
                get {

                    int extra = MobileParty.MainParty.TotalWage + (int)QuestHelperClass.TimeCalculator(actionsInOrder) / 10;

                    return QuestHelperClass.GoldCalculator(actionsInOrder) + extra;
                }
            }
            public override TextObject Title {
                get {

                    string textObject = "";
                    string strat = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        strat = chosenMission.info;
                    }
                    else
                    {
                        strat = chosenMission.Children[0].info;
                    }
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                    }

                    if (stratObj.ToString() == "empty")
                    {
                        if (chosenMission.nodeType == CustomBTType.motivation)
                        {
                            textObject += chosenMission.info;
                        }
                        else
                        {
                            textObject += chosenMission.Children[0].info;
                        }
                    }
                    else
                    {
                        textObject += stratObj.ToString();
                    }

                    TextObject t = new TextObject("[PL] - " + textObject, null);

                    //TextObject temp_t = new TextObject("Plot Lords - A Procedural Quest Generator for Mount & Blade II: Bannerlord",null);

                    return t;

                }
            }
            public override TextObject Description {
                get {
                    TextObject textObject;
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        textObject = new TextObject(chosenMission.info, null);
                    }
                    else
                    {
                        textObject = new TextObject(chosenMission.Children[0].info, null);
                    }
                    return textObject;
                }
            }
            public override TextObject IssueBriefByIssueGiver {
                get {
                    string textObject = "";
                    string motivationC = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        motivationC += QuestHelperClass.MotivationCalculator(chosenMission.name);
                    }
                    else
                    {
                        motivationC += QuestHelperClass.MotivationCalculator(chosenMission.Children[0].name);
                    }

                    if (chosenMission.subquest_info != "none" && chosenMission.subquest_info != null)
                    {
                        if (chosenMission.subquest_info == "get")
                        {
                            TextObject get = new TextObject("You're collecting materials for {HERO} right? That rascal has been owning me a favor for a while. Do this task for me and I'll consider it paid. ", null);
                            get.SetTextVariable("HERO", chosenMission.origin_quest_hero);
                            textObject += get.ToString();
                        }
                        else if (chosenMission.subquest_info == "prepare")
                        {
                            if (chosenMission.nodeType == CustomBTType.motivation)
                            {
                                TextObject prepare = new TextObject(QuestHelperClass.SubquestPrepare(chosenMission.name), null);
                                prepare.SetTextVariable("HERO", chosenMission.origin_quest_hero);
                                textObject += prepare.ToString();
                            }
                            else
                            {
                                TextObject prepare = new TextObject(QuestHelperClass.SubquestPrepare(chosenMission.Children[0].name), null);
                                prepare.SetTextVariable("HERO", chosenMission.origin_quest_hero);
                                textObject += prepare.ToString();
                            }

                        }
                        else if (chosenMission.subquest_info == "learn")
                        {
                            TextObject learn = new TextObject("You're looking for information? I could give it to you for free... Or you could do something for me first. ", null);

                            textObject += learn.ToString();
                        }
                    }

                    string strat = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        strat = chosenMission.info;
                    }
                    else
                    {
                        strat = chosenMission.Children[0].info;
                    }
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "exchange")
                                {
                                    stratObj = a.getDescription(strat, ListenReportPair);
                                    break;
                                }
                            }
                            break;
                    }

                    if (stratObj.ToString() == "empty")
                    {
                        textObject += QuestHelperClass.DescriptionCalculator(strat);
                    }
                    else
                    {
                        textObject += stratObj.ToString();
                    }

                    return new TextObject(motivationC + " " + textObject, null);

                }
            }

            public override TextObject IssueAcceptByPlayer {
                get {
                    return new TextObject("Alright, anything else you can tell me?", null);
                }
            }
            public override TextObject IssueQuestSolutionExplanationByIssueGiver {
                get {

                    string textObject = "Yes. The main steps you need to take are: ";

                    string strat = "";
                    string alternativeStrat = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        strat = chosenMission.info;
                    }
                    else
                    {
                        strat = chosenMission.Children[0].info;
                        alternativeStrat = chosenMission.Children[1].info;
                    }
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study": //get give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC": //goto listen report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }


                            break;
                        case "Obtain luxuries": //get give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Kill pests": //goto defeat report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Obtain rare items": //get give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Kill enemies": //goto defeat report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Visit dangerous place": //goto report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Revenge, Justice": //goto defeat report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Capture Criminal": //goto capture report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Check on NPC": //goto listen report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item": //get give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC": //goto rescue report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }

                            }
                            break;
                        case "Attack threatening entities": //goto defeat report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Create Diversion": //goto damage report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Recruit": //goto listen report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            break;
                        case "Attack enemy": //goto defeat report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }

                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();

                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff": //goto steal give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials": //goto get report
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "report")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale": //goto steal give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Practice combat": //goto damage
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Practice skill": //goto use
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies": //get give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies": //steal give
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies": //get goto exchange
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "exchange")
                                {
                                    stratObj = a.getStepDescription(strat);
                                    textObject += stratObj.ToString();
                                    break;
                                }
                            }
                            break;
                    }

                    if (alternativeStrat != "")
                    {
                        textObject += " Alternatively you can: ";

                        stratObj = new TextObject("empty", null);
                        switch (alternativeStrat)
                        {
                            case "Deliver item for study": //get give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Interview NPC": //goto listen report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "listen")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }


                                break;
                            case "Obtain luxuries": //get give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Kill pests": //goto defeat report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage" || a.action == "kill")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Obtain rare items": //get give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Kill enemies": //goto defeat report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage" || a.action == "kill")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Visit dangerous place": //goto report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Revenge, Justice": //goto defeat report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage" || a.action == "kill")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Capture Criminal": //goto capture report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "capture")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Check on NPC": //goto listen report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "listen")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Recover lost/stolen item": //get give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Rescue NPC": //goto rescue report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "free")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }

                                }
                                break;
                            case "Attack threatening entities": //goto defeat report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage" || a.action == "kill")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Create Diversion": //goto damage report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Recruit": //goto listen report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "listen")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                break;
                            case "Attack enemy": //goto defeat report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }

                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage" || a.action == "kill")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();

                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Steal stuff": //goto steal give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Gather raw materials": //goto get report
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "report")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Steal valuables for resale": //goto steal give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Practice combat": //goto damage
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "damage")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Practice skill": //goto use
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "use")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Deliver supplies": //get give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Steal supplies": //steal give
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "give")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                            case "Trade for supplies": //get goto exchange
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "goto" || a.action == "explore")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                foreach (actionTarget a in actionsInOrder)
                                {
                                    if (a.action == "exchange")
                                    {
                                        stratObj = a.getStepDescription(alternativeStrat);
                                        textObject += stratObj.ToString();
                                        break;
                                    }
                                }
                                break;
                        }
                    }

                    textObject += " As a reward, I'll give you ";

                    TextObject t = new TextObject(textObject + "{GOLD}{GOLD_ICON}.", null);
                    t.SetTextVariable("GOLD", this.RewardGold);
                    t.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");

                    return t;
                }
            }
            public override TextObject IssueQuestSolutionAcceptByPlayer {
                get {
                    return new TextObject("I shall be on my way then.", null);
                }
            }
            public override bool IsThereAlternativeSolution {
                get {
                    return false;
                }
            }

            public override bool IsThereLordSolution {
                get {
                    return false;
                }
            }

            public override IssueFrequency GetFrequency()
            {
                return IssueBase.IssueFrequency.Common;
            }

            public override bool IssueStayAliveConditions()
            {
                return true;
            }

            protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
            {
                relationHero = null;
                skill = null;
                flag = IssueBase.PreconditionFlags.None;

                return flag == IssueBase.PreconditionFlags.None;
            }

            protected override void CompleteIssueWithTimedOutConsequences()
            {
            }


            protected override QuestBase GenerateIssueQuest(string questId)
            {
                journalLogs = new List<JournalLog>();
                foreach (actionTarget aT in actionsInOrder)
                {
                    journalLogs.Add(new JournalLog(IssueDueTime, new TextObject(aT.action + "JournalLog", null)));
                }

                if (alternativeMission != null)
                {
                    List<JournalLog> alternativeJournalLogs = new List<JournalLog>();
                    foreach (actionTarget aT in alternativeActionsInOrder)
                    {
                        alternativeJournalLogs.Add(new JournalLog(IssueDueTime, new TextObject(aT.action + "JournalLog", null)));
                    }
                    QuestBase AlternativeQuest = new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId + "_alternative", null, alternativeMission, alternativeActionsInOrder, alternativeJournalLogs, true, QuestHelperClass.TimeCalculator(alternativeActionsInOrder), QuestHelperClass.GoldCalculator(alternativeActionsInOrder), ListenReportPair);
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, chosenMission, actionsInOrder, journalLogs, false, QuestHelperClass.TimeCalculator(actionsInOrder), this.RewardGold, ListenReportPair, (QuestGenTestQuest)AlternativeQuest);
                }
                else
                {
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, chosenMission, actionsInOrder, journalLogs, false, QuestHelperClass.TimeCalculator(actionsInOrder), this.RewardGold, ListenReportPair);
                }

            }

            protected override void OnGameLoad()
            {
                string path2 = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + ".xml";
                if (File.Exists(path2))
                {
                    actionsInOrder = new List<actionTarget>();

                    chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];

                    chosenMission.bringTargetsBack((IssueBase)this, this, false);

                    if (File.Exists(@"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml"))
                    {
                        alternativeActionsInOrder = new List<actionTarget>();
                        string path2_alt = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml";
                        alternativeMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2_alt)[0];

                        alternativeMission.bringTargetsBack((IssueBase)this, this, true);
                    }
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("[PL] - Some quest files were not found so the quests have been cancelled. If this was not intended please find the quest files and place them on the SaveFiles folder."));
                    Campaign.Current.IssueManager.DeactivateIssue(this);
                }
            }

        }

        public class QuestGenTestQuest : QuestBase
        {
            [SaveableField(110)]
            public Hero missionHero;

            public CustomBTNode chosenMission;

            public List<actionTarget> actionsInOrder;

            public actionTarget currentAction;

            [SaveableField(112)]
            public int currentActionIndex;

            [SaveableField(113)]
            public List<JournalLog> journalLogs;

            [SaveableField(114)]
            public bool alternativeFlag = false;

            [SaveableField(118)]
            public int ListenReportPair = rnd.Next(1, 3);

            public QuestGenTestQuest alternativeQuest;



            internal static void AutoGeneratedStaticCollectObjectsIssueQuest(object o, List<object> collectedObjects)
            {
                ((QuestGenTestCampaignBehavior.QuestGenTestQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
            }

            protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
            {
                base.AutoGeneratedInstanceCollectObjects(collectedObjects);

                foreach (JournalLog jL in journalLogs)
                {
                    collectedObjects.Add(jL);
                }
            }

            public override bool IsRemainingTimeHidden {
                get {
                    return false;
                }
            }

            public override TextObject Title {
                get {

                    string textObject = "";
                    string strat = "";
                    if (chosenMission != null)
                    {
                        strat = chosenMission.info;
                    }
                    
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in actionsInOrder)
                            {
                                if (a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                    }

                    if (stratObj.ToString() == "empty" && chosenMission != null)
                    {
                        textObject += chosenMission.info;
                    }
                    else
                    {
                        textObject += stratObj.ToString();
                    }
                    if (alternativeFlag && actionsInOrder.Count > 0) textObject += " alternative path given by " + actionsInOrder[0].questGiver.Name.ToString();

                    TextObject t = new TextObject("[PL] - " + textObject, null);

                    return t;
                }
            }

            private TextObject SuccessQuestLogText {
                get {
                    TextObject textObject = new TextObject("You did it.", null);
                    return textObject;
                }
            }

            private TextObject FailTimedOutQuestLogText {
                get {
                    TextObject textObject = new TextObject("Timed out quest test.", null);
                    return textObject;
                }
            }

            public override bool IsSpecialQuest {
                get {
                    if (alternativeFlag)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public QuestGenTestQuest(string questId, Hero questGiver, CustomBTNode chosenMission, List<actionTarget> actionsInOrder, List<JournalLog> journalLogs, bool alternative, float time, int reward, int listenrepPair, QuestGenTestQuest alternativeReference = null) : base(questId, questGiver, CampaignTime.DaysFromNow(time), reward)
            {
                alternativeFlag = alternative;
                missionHero = questGiver;
                this.chosenMission = chosenMission;
                this.actionsInOrder = actionsInOrder;
                this.journalLogs = journalLogs;
                this.ListenReportPair = listenrepPair;
                if (!alternative)
                {
                    alternativeQuest = alternativeReference;
                }

                this.SetDialogs();
                base.InitializeQuestOnCreation();

                if (alternative)
                {
                    this.QuestAcceptedConsequences();
                }
            }
            protected override void InitializeQuestOnGameLoad()
            {
                string id = base.StringId;

                string path2 = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + ".xml";

                this.SetDialogs();

                if (File.Exists(path2))
                {
                    this.loadActionTargets();
                    currentAction = actionsInOrder[currentActionIndex];

                    for (int i = 0; i < actionsInOrder.Count; i++)
                    {
                        var aT = actionsInOrder[i];

                        DialogFlow d = aT.getDialogFlows(i, null, (QuestBase)this, this);

                        if (d != null)
                        {
                            Campaign.Current.ConversationManager.AddDialogFlow(d, this);
                        }

                    }
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage("[PL] - Some quest files were not found so the quests have been cancelled. If this was not intended please find the quest files and place them on the SaveFiles folder."));
                    this.CompleteQuestWithCancel(null);
                }

            }

            protected override void SetDialogs()
            {
                OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("Good Luck.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver).Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences)).CloseDialog();
                DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("Yes, yes. Hurry it up now, time's running out.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver).BeginPlayerOptions().PlayerOption(new TextObject("Alright, will do.", null), null).EndPlayerOptions().CloseDialog();
            }

            public JournalLog getDiscreteLog(TextObject text, TextObject taskName, int currentProgress, int targetProgress, TextObject shortText = null, bool hideInformation = false)
            {
                return base.AddDiscreteLog(text, taskName, currentProgress, targetProgress, shortText, hideInformation);
            }

            public void UpdateQuestTaskS(JournalLog questLog, int currentProgress)
            {
                base.UpdateQuestTaskStage(questLog, currentProgress);
            }

            private void QuestAcceptedConsequences()
            {
                if (this.actionsInOrder.Count <= 0)
                {
                    this.CompleteQuestWithSuccess();
                }
                else
                {
                    base.StartQuest();

                    currentAction = actionsInOrder[0];
                    currentActionIndex = 0;

                    chosenMission.run(CustomBTStep.questQ, (QuestBase)this, this);
                    
                    this.saveActionTargets();
                }

            }

            public void OnSaveOverEvent(bool isSuccessful)
            {
                if (isSuccessful) this.saveActionTargets();
            }

            private void saveActionTargets()
            {
                string id = base.StringId;

                //string path1 = @"..\..\Modules\ThePlotLords\SaveFiles\actionsSaveFile_" + id + ".xml";
                string path2 = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + ".xml";
                //XmlSerialization.WriteToXmlFile<List<actionTarget>>(path1, this.actionsInOrder);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { chosenMission });
            }

            private void loadActionTargets()
            {
                string id = base.StringId;

                string path2 = @"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + ".xml";
                actionsInOrder = new List<actionTarget>();
                chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];

                chosenMission.bringTargetsBack((QuestBase)this, this);
                

                //if (File.Exists(@"..\..\Modules\ThePlotLords\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml") && alternativeFlag == false)
                //{
                //    foreach(QuestBase qb in Campaign.Current.QuestManager.Quests)
                //    {
                //        if (qb.StringId == base.StringId + "_alternative")
                //        {
                //            this.alternativeQuest = (QuestGenTestQuest)qb;
                //            this.alternativeQuest.RegisterEvents();
                //            this.alternativeQuest.InitializeQuestOnGameLoad();
                //            using (List<QuestTaskBase>.Enumerator enumerator3 = this.alternativeQuest.TaskList.GetEnumerator())
                //            {
                //                while (enumerator3.MoveNext())
                //                {
                //                    QuestTaskBase questTaskBase = enumerator3.Current;
                //                    if (questTaskBase.IsActive)
                //                    {
                //                        questTaskBase.SetReferences();
                //                        questTaskBase.AddTaskDialogs();
                //                    }
                //                }
                //                continue;
                //            }
                //        }
                //    }
                //}

            }

            public void SuccessConsequences()
            {
                if (!alternativeFlag)
                {
                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, RewardGold, false);
                    GainRenownAction.Apply(Hero.MainHero, 1f, false);
                    TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
                    {
                    new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
                    });
                    this.RelationshipChangeWithQuestGiver = 5;
                    base.QuestGiver.AddPower(5f);
                    missionHero.CurrentSettlement.Prosperity += 10f;
                    player_data.player_CompletedQuests++;
                    string playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
                    JsonSerialization.WriteToJsonFile<PlayerData>(playerData, player_data);
                }
                TextObject textObject = new TextObject("You have completed the mission", null);
                base.AddLog(textObject, true);
                quests_created--;
                base.CompleteQuestWithSuccess();
            }

            public void FailConsequences()
            {
                if (!alternativeFlag)
                {
                    this.RelationshipChangeWithQuestGiver = -5;
                    base.QuestGiver.AddPower(-5f);
                    missionHero.CurrentSettlement.Prosperity += -10f;
                }
                base.CompleteQuestWithFail();
            }

            protected override void RegisterEvents()
            {
                CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
                CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.OnPlayerInventoryExchange));
                CampaignEvents.OnPartyConsumedFoodEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyConsumedFood));
                CampaignEvents.OnHeroSharedFoodWithAnotherHeroEvent.AddNonSerializedListener(this, new Action<Hero, Hero, float>(this.OnHeroSharedFoodWithAnotherHero));
                CampaignEvents.OnEquipmentSmeltedByHeroEvent.AddNonSerializedListener(this, new Action<Hero, EquipmentElement>(this.OnEquipmentSmeltedByHeroEvent));
                CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, Crafting.OverrideData, bool>(this.OnNewItemCraftedEvent));
                CampaignEvents.OnItemProducedEvent.AddNonSerializedListener(this, new Action<ItemObject, Settlement, int>(this.OnItemProducedEvent));
                CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestCompleteDetails>(this.OnQuestCompletedEvent));
                CampaignEvents.OnPrisonerTakenEvent.AddNonSerializedListener(this, new Action<FlattenedTroopRoster>(this.OnPrisonerTakenEvent));
                CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.HeroPrisonerTaken));
                CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.HeroKilledEvent));
                CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
                CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail>(this.HeroPrisonerReleased));
                CampaignEvents.PrisonersChangeInSettlement.AddNonSerializedListener(this, new Action<Settlement, FlattenedTroopRoster, Hero, bool>(this.PrisonersChangeInSettlement));
                CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, bool, int, bool>(this.HeroGainedSkill));
                CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, MapEvent>(this.RaidCompletedEvent));
                CampaignEvents.ItemsLooted.AddNonSerializedListener(this, new Action<ItemRoster>(this.ItemsLooted));
                CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.MapEventStarted));
                CampaignEvents.OnSaveOverEvent.AddNonSerializedListener(this, new Action<bool>(this.OnSaveOverEvent));
                //CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
                //CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
                //CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
                //CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction>(this.OnWarDeclared));
                //CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnHourlyTickParty));
                //CampaignEvents.HeroRelationChanged.AddNonSerializedListener(this, new Action<Hero, Hero, int, bool>(this.OnHeroRelationChanged));
            }

            private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
            {
                if (party == MobileParty.MainParty && settlement.GatePosition.NearlyEquals(MobileParty.MainParty.Position2D, MobileParty.MainParty.SeeingRange + 2f))
                {
                    switch (actionsInOrder[currentActionIndex].action)
                    {
                        case "goto":
                            actionsInOrder[currentActionIndex].OnSettlementEnteredQuest(party, settlement, hero, currentActionIndex, this, (QuestBase)this);
                            break;
                        case "explore":
                            actionsInOrder[currentActionIndex].OnSettlementEnteredQuest(party, settlement, hero, currentActionIndex, this, (QuestBase)this);
                            break;
                    }

                }
            }

            private void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    //case "give":
                    //    aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                    //    break;

                    case "gather":
                        actionsInOrder[currentActionIndex].OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, currentActionIndex, this, (QuestBase)this);
                        break;

                        //case "exchange":
                        //    this.actionsInOrder[currentActionIndex].OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, currentActionIndex, this, (QuestBase)this);
                        //    break;

                }

            }

            private void OnPartyConsumedFood(MobileParty party)
            {
                if (party.IsMainParty)
                {
                    //switch (this.actionsInOrder[currentActionIndex].action) 
                    //{ 
                    //case "give":
                    //    aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                    //    break;

                    //case "exchange":
                    //    this.actionsInOrder[currentActionIndex].OnPartyConsumedFoodQuest(party, currentActionIndex, this, (QuestBase)this);
                    //    break;


                    //}
                }
            }

            private void OnHeroSharedFoodWithAnotherHero(Hero supporterHero, Hero supportedHero, float influence)
            {
                if (supporterHero == Hero.MainHero || supportedHero == Hero.MainHero)
                {
                    //switch (this.actionsInOrder[currentActionIndex].action)
                    //{
                    //case "give":
                    //    aT.OnHeroSharedFoodWithAnotherHeroQuest(supporterHero, supportedHero, influence, i, this, (QuestBase)this);
                    //    break;

                    //case "exchange":
                    //    this.actionsInOrder[currentActionIndex].OnHeroSharedFoodWithAnotherHeroQuest(supporterHero, supportedHero, influence, i, this, (QuestBase)this);
                    //    break;

                    //}
                }
            }

            private void OnEquipmentSmeltedByHeroEvent(Hero hero, EquipmentElement equipmentElement)
            {
                if (hero.PartyBelongedTo == MobileParty.MainParty)
                {
                    switch (actionsInOrder[currentActionIndex].action)
                    {
                        case "gather":
                            actionsInOrder[currentActionIndex].OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, currentActionIndex, this, (QuestBase)this);
                            break;

                            //case "exchange":
                            //    this.actionsInOrder[currentActionIndex].OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                            //    break;

                    }
                }
            }

            private void OnNewItemCraftedEvent(ItemObject item, Crafting.OverrideData crafted, bool flag)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "gather":
                        actionsInOrder[currentActionIndex].OnNewItemCraftedEventQuest(item, crafted, flag, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "exchange":
                        actionsInOrder[currentActionIndex].OnNewItemCraftedEventQuest(item, crafted, flag, currentActionIndex, this, (QuestBase)this);
                        break;

                }
            }

            private void OnItemProducedEvent(ItemObject itemObject, Settlement settlement, int count)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "gather":
                        actionsInOrder[currentActionIndex].OnItemProducedEventQuest(itemObject, settlement, count, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void OnQuestCompletedEvent(QuestBase quest, QuestCompleteDetails questCompleteDetails)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "quest":
                        actionsInOrder[currentActionIndex].OnQuestCompletedEventQuest(quest, questCompleteDetails, currentActionIndex, this, (QuestBase)this);
                        break;
                }

                if (alternativeFlag)
                {
                    if (quest.StringId + "_alternative" == base.StringId)
                    {
                        this.SuccessConsequences();
                    }
                }
                else
                {
                    if (quest.StringId.Replace("_alternative", "") == base.StringId)
                    {
                        this.SuccessConsequences();
                    }
                }
            }

            private void OnPrisonerTakenEvent(FlattenedTroopRoster rooster)
            {
                int cAIndex = 0;
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        actionsInOrder[currentActionIndex].OnPrisonerTakenEvent(rooster, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                actionsInOrder[cAIndex + 1].OnPrisonerTakenEvent(rooster, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                actionsInOrder[cAIndex + 1].OnPrisonerTakenEvent(rooster, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                }
            }

            private void HeroPrisonerTaken(PartyBase capturer, Hero prisoner)
            {
                int cAIndex = 0;
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        actionsInOrder[currentActionIndex].HeroPrisonerTaken(capturer, prisoner, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                actionsInOrder[cAIndex + 1].HeroPrisonerTaken(capturer, prisoner, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                actionsInOrder[cAIndex + 1].HeroPrisonerTaken(capturer, prisoner, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                }
            }

            private void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "damage":
                        actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }


            private void MapEventEnded(MapEvent mapEvent)
            {
                int cAIndex = 0;
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "kill":
                        cAIndex = currentActionIndex;
                        actionsInOrder[cAIndex].MapEventEnded(mapEvent, cAIndex, this, (QuestBase)this);
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "take")
                            {
                                actionsInOrder[cAIndex + 1].MapEventEnded(mapEvent, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "take":
                        actionsInOrder[currentActionIndex].MapEventEnded(mapEvent, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        actionsInOrder[cAIndex].MapEventEnded(mapEvent, cAIndex, this, (QuestBase)this);
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "take")
                            {
                                actionsInOrder[cAIndex + 1].MapEventEnded(mapEvent, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;

                }
            }

            private void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
            {
                foreach (actionTarget a in actionsInOrder)
                {
                    if (a.action == "capture") actionsInOrder[currentActionIndex].HeroPrisonerReleased(prisoner, party, capturerFaction, detail, currentActionIndex, this, (QuestBase)this);
                }
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "free":
                        actionsInOrder[currentActionIndex].HeroPrisonerReleased(prisoner, party, capturerFaction, detail, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "free":
                        actionsInOrder[currentActionIndex].PrisonersChangeInSettlement(settlement, prisonerRoster, prisonerHero, isReleased, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void HeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "use":
                        actionsInOrder[currentActionIndex].HeroGainedSkill(hero, skill, hasNewPerk, change, shouldNotify, currentActionIndex, this, (QuestBase)this);
                        break;

                }
            }

            private void RaidCompletedEvent(BattleSideEnum winnerSide, MapEvent mapEvent)
            {
                switch (actionsInOrder[currentActionIndex].action)
                {
                    case "damage":
                        actionsInOrder[currentActionIndex].RaidCompletedEvent(winnerSide, mapEvent, currentActionIndex, this, (QuestBase)this);
                        break;

                }
            }

            private void ItemsLooted(ItemRoster items)
            {
                //int cAIndex = 0;
                switch (actionsInOrder[currentActionIndex].action)
                {
                    //case "give":
                    //    aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                    //    break;

                    //case "exchange":
                    //    this.actionsInOrder[currentActionIndex].OnPartyConsumedFoodQuest(party, currentActionIndex, this, (QuestBase)this);
                    //    break;
                    //case "damage":
                    //    cAIndex = currentActionIndex;
                    //    if (cAIndex < actionsInOrder.Count-1)
                    //    {
                    //        if (this.actionsInOrder[cAIndex + 1].action == "take")
                    //        {
                    //            this.actionsInOrder[cAIndex + 1].ItemsLooted(items, cAIndex + 1, this, (QuestBase)this);
                    //        }
                    //    }
                    //    break;
                    //case "kill":
                    //    cAIndex = currentActionIndex;
                    //    if (cAIndex < actionsInOrder.Count-1)
                    //    {
                    //        if (this.actionsInOrder[cAIndex + 1].action == "take")
                    //        {
                    //            this.actionsInOrder[cAIndex + 1].ItemsLooted(items, cAIndex + 1, this, (QuestBase)this);
                    //        }
                    //    }
                    //    break;
                    //case "take":
                    //    this.actionsInOrder[currentActionIndex].ItemsLooted(items, currentActionIndex, this, (QuestBase)this);
                    //    break;
                    case "gather":
                        actionsInOrder[currentActionIndex].ItemsLooted(items, currentActionIndex, this, (QuestBase)this);
                        break;
                }

            }

            private void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
            {
                int cAIndex = 0;
                switch (actionsInOrder[currentActionIndex].action)
                {
                    //case "give":
                    //    aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                    //    break;

                    //case "exchange":
                    //    this.actionsInOrder[currentActionIndex].OnPartyConsumedFoodQuest(party, currentActionIndex, this, (QuestBase)this);
                    //    break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "take")
                            {
                                actionsInOrder[cAIndex + 1].MapEventStarted(mapEvent, attackerParty, defenderParty, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count - 1)
                        {
                            if (actionsInOrder[cAIndex + 1].action == "take")
                            {
                                actionsInOrder[cAIndex + 1].MapEventStarted(mapEvent, attackerParty, defenderParty, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "take":
                        actionsInOrder[currentActionIndex].MapEventStarted(mapEvent, attackerParty, defenderParty, currentActionIndex, this, (QuestBase)this);
                        break;
                }

            }

        }
    }
}
