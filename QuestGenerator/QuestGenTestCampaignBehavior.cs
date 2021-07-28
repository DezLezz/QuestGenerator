using QuestGenerator.QuestBuilder;
using QuestGenerator.QuestBuilder.CustomBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment.Managers;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace QuestGenerator
{
    public class QuestGenTestCampaignBehavior : CampaignBehaviorBase
    {

        private const IssueBase.IssueFrequency QuestTestIssueFrequency = IssueBase.IssueFrequency.Common;

        [SaveableField(100)]
        public static Dictionary<Hero, string> HeroMotivations;

        private string path = @"..\..\Modules\QuestGenerator\MissionList.xml";

        public static Generator QuestGen = new Generator(0);

        private List<CustomBTNode> missions;
        private CustomBTNode chosenMission;
        static Random rnd = new Random();

        //[SaveableField(103)]

        public QuestGenTestCampaignBehavior()
        {
            if (new FileInfo(path).Length != 0)
            {
                this.missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
            }
            else
            {
                this.missions = new List<CustomBTNode>();
            }

            HeroMotivations = new Dictionary<Hero, string>();

            
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new System.Action(this.HourlyTick));
        }

        private void HourlyTick()
        {
            if (Input.IsKeyDown(InputKey.G))
            {
                QuestGen.GenerateOne();
            }
        }

        private void OnCheckForIssue(Hero hero)
        {

            if (HeroMotivations.Count == 0)
            {
                QuestHelperClass.MotivationGiver();                
            }

            if (this.ConditionsHold(hero))
            {
                Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common));
            }
        }

        public IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
        {

            PotentialIssueData potentialIssueData = pid;
            string place = "";
            if (issueOwner.CurrentSettlement != null) place = issueOwner.CurrentSettlement.Name.ToString();
            InformationManager.DisplayMessage(new InformationMessage("Issue Created for hero " + issueOwner.Name.ToString() + "," + place));
            string motiv = "none";
            motiv = HeroMotivations[issueOwner];

            foreach (CustomBTNode btNode in this.missions)
            {
                if (btNode.name == motiv)
                {
                    this.chosenMission = btNode;
                    this.missions.Remove(btNode);
                    XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, this.missions);
                    
                    break;
                }
            }
            QuestHelperClass.MotivationGiverOneHero(issueOwner);
            if (issueOwner == null )
            {
                return null;
            }
            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, this.chosenMission);
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

            if (this.missions.IsEmpty() && new FileInfo(path).Length != 0)
            {
                this.missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path); 
            }

            if (motiv == "none")
            {
                return false;
            }

            if (this.missions.IsEmpty())
            {
                int i = rnd.Next(1, 6);
                if (i == 0)
                {
                    QuestGen.GenerateOne();
                    this.missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
                }
                else
                {
                    return false;
                }
                
            }

            foreach (CustomBTNode btNode in this.missions)
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
                this.missionHero = issueOwner;
                this.actionsInOrder = new List<actionTarget>();
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

                    this.alternativeActionsInOrder = new List<actionTarget>();
                    this.alternativeMission = chosenMission.Children[1];
                    this.alternativeMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, true);
                    this.alternativeMission.run(CustomBTStep.issueQ, (IssueBase)this, this, true);

                }
                this.id = this.IssueOwner.Name.ToString() + (int)this.IssueOwner.Age;
                saveMissions();
            }

            private void saveMissions()
            {
                
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + ".xml";

                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { this.chosenMission });

                if (this.alternativeMission != null)
                {
                    string path2_alt = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml";
                    XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2_alt, new List<CustomBTNode>() { this.alternativeMission });
                }
            }

            protected override int RewardGold {
                get {
                    return QuestHelperClass.GoldCalculator(this.actionsInOrder);
                }
            }
            public override TextObject Title {
                get {

                    string textObject = "";
                    string strat = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        strat = this.chosenMission.info;
                    }
                    else
                    {
                        strat = this.chosenMission.Children[0].info;
                    }
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in this.actionsInOrder)
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
                            textObject += this.chosenMission.info;
                        }
                        else
                        {
                            textObject += this.chosenMission.Children[0].info;
                        }
                    }
                    else
                    {
                        textObject += stratObj.ToString();
                    }

                    TextObject t = new TextObject(textObject, null);

                    return t;

                }
            }
            public override TextObject Description {
                get {
                    TextObject textObject;
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        textObject = new TextObject(this.chosenMission.info, null);
                    }
                    else
                    {
                        textObject = new TextObject(this.chosenMission.Children[0].info, null);
                    }
                    return textObject;
                }
            }
            public override TextObject IssueBriefByIssueGiver {
                get {
                    TextObject textObject;
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        textObject = new TextObject(QuestHelperClass.MotivationCalculator(this.chosenMission.name), null);
                    }
                    else
                    {
                        textObject = new TextObject(QuestHelperClass.MotivationCalculator(this.chosenMission.Children[0].name), null);
                    }

                    if (this.chosenMission.subquest_info != "none" && this.chosenMission.subquest_info != null)
                    {
                        if (this.chosenMission.subquest_info == "get")
                        {
                            TextObject get = new TextObject("You're collecting materials for {HERO} right? That rascal has been owning me a favor for a while. Do this task for me and I'll consider it paid. " + textObject.ToString(), null);
                            get.SetTextVariable("HERO", this.chosenMission.origin_quest_hero);
                            return get;
                        }
                        else if (this.chosenMission.subquest_info == "prepare")
                        {
                            TextObject prepare = new TextObject(QuestHelperClass.MotivationCalculator(this.chosenMission.name) + textObject.ToString(), null);
                            prepare.SetTextVariable("HERO", this.chosenMission.origin_quest_hero);
                            return prepare;
                        }
                        else if (this.chosenMission.subquest_info == "learn")
                        {
                            TextObject learn = new TextObject("You're looking for information? I could give it to you for free... Or you could do something for me first. " + textObject.ToString(), null);

                            return learn;
                        }
                    }

                    return textObject;

                }
            }

            public override TextObject IssueAcceptByPlayer {
                get {
                    return new TextObject("What do I need to do then?", null);
                }
            }
            public override TextObject IssueQuestSolutionExplanationByIssueGiver {
                get {

                    string textObject = "";
                    string strat = "";
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        strat = this.chosenMission.info;
                    }
                    else
                    {
                        strat = this.chosenMission.Children[0].info;
                    }
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getDescription(strat);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "exchange")
                                {
                                    stratObj = a.getDescription(strat);
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

                    textObject += " As a reward, I'll give you ";

                    TextObject t = new TextObject(textObject + "{GOLD}{GOLD_ICON}.", null);
                    t.SetTextVariable("GOLD", this.RewardGold);
                    t.SetTextVariable("GOLD_ICON", "{=!}<img src=\"Icons\\Coin@2x\" extend=\"8\">");

                    return t;
                }
            }
            public override TextObject IssueQuestSolutionAcceptByPlayer {
                get {
                    return new TextObject("Then I shall be on my way.", null);
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
                this.journalLogs = new List<JournalLog>();
                foreach (actionTarget aT in actionsInOrder)
                {
                    this.journalLogs.Add(new JournalLog(this.IssueDueTime, new TextObject(aT.action + "JournalLog", null)));
                }

                if (this.alternativeMission != null)
                {
                    List<JournalLog> alternativeJournalLogs = new List<JournalLog>();
                    foreach (actionTarget aT in alternativeActionsInOrder)
                    {
                        alternativeJournalLogs.Add(new JournalLog(this.IssueDueTime, new TextObject(aT.action + "JournalLog", null)));
                    }
                    QuestBase AlternativeQuest = new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId + "_alternative", null, this.alternativeMission, this.alternativeActionsInOrder, alternativeJournalLogs,true, QuestHelperClass.TimeCalculator(this.alternativeActionsInOrder), QuestHelperClass.GoldCalculator(this.alternativeActionsInOrder));
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, this.chosenMission, this.actionsInOrder, this.journalLogs, false, QuestHelperClass.TimeCalculator(this.actionsInOrder), this.RewardGold, (QuestGenTestQuest)AlternativeQuest);
                }
                else
                {
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, this.chosenMission, this.actionsInOrder, this.journalLogs, false, QuestHelperClass.TimeCalculator(this.actionsInOrder), this.RewardGold);
                }

            }

            protected override void OnGameLoad()
            {
                this.actionsInOrder = new List<actionTarget>();
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + ".xml";
                this.chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];

                this.chosenMission.bringTargetsBack((IssueBase)this, this, false);

                if (File.Exists(@"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + "_alternative" + ".xml"))
                {
                    this.alternativeActionsInOrder = new List<actionTarget>();
                    string path2_alt = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + "_alternative" + ".xml";
                    this.alternativeMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2_alt)[0];

                    this.alternativeMission.bringTargetsBack((IssueBase)this, this, true);
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

            public QuestGenTestQuest alternativeQuest;

            [SaveableField(117)]
            public int ListenReportPair = rnd.Next(1, 3);

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
                    strat = this.chosenMission.info;
                    TextObject stratObj = new TextObject("empty", null);
                    switch (strat)
                    {
                        case "Deliver item for study":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Interview NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain luxuries":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill pests":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Obtain rare items":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Kill enemies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Visit dangerous place":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "goto" || a.action == "explore")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Revenge, Justice":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Capture Criminal":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "capture")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Check on NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recover lost/stolen item":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Rescue NPC":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "free")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack threatening entities":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Create Diversion":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Recruit":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "listen")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Attack enemy":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage" || a.action == "kill")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal stuff":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Gather raw materials":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take" || a.action == "gather" || a.action == "exchange")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal valuables for resale":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice combat":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "damage")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Practice skill":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "use")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Deliver supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "give")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Steal supplies":
                            foreach (actionTarget a in this.actionsInOrder)
                            {
                                if (a.action == "take")
                                {
                                    stratObj = a.getTitle(strat);
                                    break;
                                }
                            }
                            break;
                        case "Trade for supplies":
                            foreach (actionTarget a in this.actionsInOrder)
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
                        textObject += this.chosenMission.info;
                    }
                    else
                    {
                        textObject += stratObj.ToString();
                    }
                    if (alternativeFlag) textObject +=  " alternative path given by " + this.actionsInOrder[0].questGiver.Name.ToString();

                    TextObject t = new TextObject(textObject, null);

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

            public QuestGenTestQuest(string questId, Hero questGiver, CustomBTNode chosenMission, List<actionTarget> actionsInOrder, List<JournalLog> journalLogs, bool alternative, float time , int reward, QuestGenTestQuest alternativeReference = null) : base(questId, questGiver, CampaignTime.DaysFromNow(time), reward)
            {
                this.alternativeFlag = alternative;
                this.missionHero = questGiver;
                this.chosenMission = chosenMission;
                this.actionsInOrder = actionsInOrder;
                this.journalLogs = journalLogs;
                if (!alternative)
                {
                    this.alternativeQuest = alternativeReference;
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
                
                this.SetDialogs();
                loadActionTargets();
                this.currentAction = this.actionsInOrder[this.currentActionIndex];

                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];

                    DialogFlow d = aT.getDialogFlows(i, null, (QuestBase)this, this);

                    if (d != null)
                    {
                        Campaign.Current.ConversationManager.AddDialogFlow(d, this);
                    }

                }

            }

            protected override void SetDialogs()
            {
                this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("Good Luck.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver).Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences)).CloseDialog();
                this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("Yes, yes. Hurry it up now, time's running out.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver).BeginPlayerOptions().PlayerOption(new TextObject("Alright, will do.", null), null).EndPlayerOptions().CloseDialog();
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
                base.StartQuest();

                this.currentAction = this.actionsInOrder[0];
                this.currentActionIndex = 0;

                this.chosenMission.run(CustomBTStep.questQ, (QuestBase)this, this);

                saveActionTargets();

            }

            public void OnSaveOverEvent(bool isSuccessful)
            {
                if (isSuccessful) saveActionTargets();
            }

            private void saveActionTargets()
            {
                string id = base.StringId;

                //string path1 = @"..\..\Modules\QuestGenerator\SaveFiles\actionsSaveFile_" + id + ".xml";
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + ".xml";
                //XmlSerialization.WriteToXmlFile<List<actionTarget>>(path1, this.actionsInOrder);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { this.chosenMission });
            }

            private void loadActionTargets()
            {
                string id = base.StringId;

                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + ".xml";
                this.actionsInOrder = new List<actionTarget>();
                this.chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];

                this.chosenMission.bringTargetsBack((QuestBase)this, this);

                //if (File.Exists(@"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml") && alternativeFlag == false)
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

            public bool ReturnItemClickableConditions(out TextObject explanation)
            {
                if (this.actionsInOrder[currentActionIndex].GetItemTarget() != null)
                {
                    if (PartyBase.MainParty.ItemRoster.GetItemNumber(this.actionsInOrder[currentActionIndex].GetItemTarget()) >= this.actionsInOrder[currentActionIndex].GetItemAmount())
                    {
                        explanation = TextObject.Empty;
                        return true;
                    }
                }

                explanation = new TextObject("You don't have enough of that item.", null);
                return false;
            }

            public bool ReturnItemClickableConditionsExchange(out TextObject explanation)
            {

                if (this.journalLogs[currentActionIndex].CurrentProgress == 0 && this.journalLogs[currentActionIndex].TaskName == new TextObject("Exhange {ITEM_AMOUNT1} {ITEM_NAME1} for {ITEM_AMOUNT2} {ITEM_NAME2} with {HERO}", null))
                {
                    explanation = TextObject.Empty;
                    return true;
                }
                
                explanation = new TextObject("You don't have enough of that item.", null);
                return false;
            }

            public void SuccessConsequences()
            {
                if (!alternativeFlag)
                {
                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
                    GainRenownAction.Apply(Hero.MainHero, 1f, false);
                    TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
                    {
                    new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
                    });
                    this.RelationshipChangeWithQuestGiver = 5;
                    base.QuestGiver.AddPower(5f);
                    this.missionHero.CurrentSettlement.Prosperity += 10f;
                    
                }
                TextObject textObject = new TextObject("You have completed the mission", null);
                base.AddLog(textObject, true);
                
                base.CompleteQuestWithSuccess();
            }

            public void FailConsequences()
            {
                if (!alternativeFlag)
                {
                    this.RelationshipChangeWithQuestGiver = -5;
                    base.QuestGiver.AddPower(-5f);
                    this.missionHero.CurrentSettlement.Prosperity += -10f;
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
                    switch (this.actionsInOrder[currentActionIndex].action)
                    {
                        case "goto":
                            this.actionsInOrder[currentActionIndex].OnSettlementEnteredQuest(party, settlement, hero, currentActionIndex, this, (QuestBase)this);
                            break;
                        case "explore":
                            this.actionsInOrder[currentActionIndex].OnSettlementEnteredQuest(party, settlement, hero, currentActionIndex, this, (QuestBase)this);
                            break;
                    }
                    
                }
            }

            private void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    //case "give":
                    //    aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                    //    break;

                    case "gather":
                        this.actionsInOrder[currentActionIndex].OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, currentActionIndex, this, (QuestBase)this);
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
                    switch (this.actionsInOrder[currentActionIndex].action)
                    {
                        case "gather":
                            this.actionsInOrder[currentActionIndex].OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, currentActionIndex, this, (QuestBase)this);
                            break;

                        //case "exchange":
                        //    this.actionsInOrder[currentActionIndex].OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                        //    break;
                        
                    }
                }
            }

            private void OnNewItemCraftedEvent(ItemObject item, Crafting.OverrideData crafted, bool flag)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "gather":
                        this.actionsInOrder[currentActionIndex].OnNewItemCraftedEventQuest(item, crafted, flag, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "exchange":
                        this.actionsInOrder[currentActionIndex].OnNewItemCraftedEventQuest(item, crafted, flag, currentActionIndex, this, (QuestBase)this);
                        break;
                   
                }
            }

            private void OnItemProducedEvent(ItemObject itemObject, Settlement settlement, int count)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "gather":
                        this.actionsInOrder[currentActionIndex].OnItemProducedEventQuest(itemObject, settlement, count, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void OnQuestCompletedEvent(QuestBase quest, QuestCompleteDetails questCompleteDetails)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "quest":
                        this.actionsInOrder[currentActionIndex].OnQuestCompletedEventQuest(quest, questCompleteDetails, currentActionIndex, this, (QuestBase)this);
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
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        this.actionsInOrder[currentActionIndex].OnPrisonerTakenEvent(rooster, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                this.actionsInOrder[cAIndex + 1].OnPrisonerTakenEvent(rooster, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                this.actionsInOrder[cAIndex + 1].OnPrisonerTakenEvent(rooster, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                }
            }

            private void HeroPrisonerTaken(PartyBase capturer, Hero prisoner)
            {
                int cAIndex = 0;
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        this.actionsInOrder[currentActionIndex].HeroPrisonerTaken(capturer, prisoner, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                this.actionsInOrder[cAIndex + 1].HeroPrisonerTaken(capturer, prisoner, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "capture")
                            {
                                this.actionsInOrder[cAIndex + 1].HeroPrisonerTaken(capturer, prisoner, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                }
            }

            private void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "capture":
                        this.actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "kill":
                        this.actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "damage":
                        this.actionsInOrder[currentActionIndex].HeroKilledEvent(victim, killer, detail, showNotification, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }


            private void MapEventEnded(MapEvent mapEvent)
            {
                int cAIndex = 0;
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "kill":
                        cAIndex = currentActionIndex;
                        this.actionsInOrder[cAIndex].MapEventEnded(mapEvent, cAIndex, this, (QuestBase)this);
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "take")
                            {
                                this.actionsInOrder[cAIndex + 1].MapEventEnded(mapEvent, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "take":
                        this.actionsInOrder[currentActionIndex].MapEventEnded(mapEvent, currentActionIndex, this, (QuestBase)this);
                        break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        this.actionsInOrder[cAIndex].MapEventEnded(mapEvent, cAIndex, this, (QuestBase)this);
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "take")
                            {
                                this.actionsInOrder[cAIndex + 1].MapEventEnded(mapEvent, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    
                }
            }
            
            private void HeroPrisonerReleased (Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
            {
                foreach (actionTarget a in this.actionsInOrder)
                {
                    if (a.action == "capture") this.actionsInOrder[currentActionIndex].HeroPrisonerReleased(prisoner, party, capturerFaction, detail, currentActionIndex, this, (QuestBase)this);
                }
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "free":
                        this.actionsInOrder[currentActionIndex].HeroPrisonerReleased(prisoner, party, capturerFaction, detail, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "free":
                        this.actionsInOrder[currentActionIndex].PrisonersChangeInSettlement(settlement, prisonerRoster, prisonerHero, isReleased, currentActionIndex, this, (QuestBase)this);
                        break;
                }
            }

            private void HeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "use":
                        this.actionsInOrder[currentActionIndex].HeroGainedSkill(hero, skill, hasNewPerk, change, shouldNotify, currentActionIndex, this, (QuestBase)this);
                        break;
                    
                }
            }

            private void RaidCompletedEvent(BattleSideEnum winnerSide, MapEvent mapEvent)
            {
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    case "damage":
                        this.actionsInOrder[currentActionIndex].RaidCompletedEvent(winnerSide, mapEvent, currentActionIndex, this, (QuestBase)this);
                        break;
                    
                }
            }

            private void ItemsLooted(ItemRoster items)
            {
                //int cAIndex = 0;
                switch (this.actionsInOrder[currentActionIndex].action)
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
                        this.actionsInOrder[currentActionIndex].ItemsLooted(items, currentActionIndex, this, (QuestBase)this);
                        break;
                }
                
            }

            private void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
            {
                int cAIndex = 0;
                switch (this.actionsInOrder[currentActionIndex].action)
                {
                    //case "give":
                    //    aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                    //    break;

                    //case "exchange":
                    //    this.actionsInOrder[currentActionIndex].OnPartyConsumedFoodQuest(party, currentActionIndex, this, (QuestBase)this);
                    //    break;
                    case "damage":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "take")
                            {
                                this.actionsInOrder[cAIndex + 1].MapEventStarted(mapEvent, attackerParty, defenderParty, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "kill":
                        cAIndex = currentActionIndex;
                        if (cAIndex < actionsInOrder.Count-1)
                        {
                            if (this.actionsInOrder[cAIndex + 1].action == "take")
                            {
                                this.actionsInOrder[cAIndex + 1].MapEventStarted(mapEvent, attackerParty, defenderParty, cAIndex + 1, this, (QuestBase)this);
                            }
                        }
                        break;
                    case "take":
                        this.actionsInOrder[currentActionIndex].MapEventStarted(mapEvent, attackerParty, defenderParty, currentActionIndex, this, (QuestBase)this);
                        break;
                }

            }

        }
    }
}
