using QuestGenerator.QuestBuilder;
using QuestGenerator.QuestBuilder.CustomBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
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
        //private static List<heroPatch> HeroMotivations = new List<heroPatch>();

        private string path = @"..\..\Modules\QuestGenerator\MissionList.xml";

        public static Generator QuestGen = new Generator(0);

        private List<CustomBTNode> missions;

        private CustomBTNode chosenMission;
        float lasttime = 0;
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
            CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
        }

        private void TickEvent(float f)
        {
            if (Input.IsKeyDown(InputKey.G))
            {
                QuestGen.GenerateOne();
                lasttime = f;
            }
        }

        private void OnCheckForIssue(Hero hero)
        {

            if (HeroMotivations.Count == 0)
            {

                List<string> motivations = new List<string>() { "Knowledge", "Comfort", "Reputation", "Serenity", "Protection", "Conquest", "Wealth", "Ability", "Equipment" };
                var heroList = Hero.AllAliveHeroes;

                foreach (Hero h in heroList)
                {
                    int r = rnd.Next(motivations.Count);
                    if (!HeroMotivations.ContainsKey(h))
                    {
                        HeroMotivations.Add(h, motivations[r]);
                    }
                }

                InformationManager.DisplayMessage(new InformationMessage("Motivations made \n"));
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

            foreach (Hero h in HeroMotivations.Keys)
            {
                if (issueOwner.Name == h.Name)
                {
                    motiv = HeroMotivations[h];
                    break;
                }
            }

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

            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, this.chosenMission);
        }

        private bool ConditionsHold(Hero issueGiver)
        {
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

            if (this.missions.IsEmpty())
            {
                this.missions = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path); 
            }

            if (motiv == "none")
            {
                return false;
            }

            if (this.missions.IsEmpty())
            {
                return false;
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
            public int id = 0;

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

                saveMissions();
            }

            private void saveMissions()
            {
                int r = rnd.Next(1, 9999999);
                this.id = r;
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + ".xml";

                while (File.Exists(path2))
                {
                    r = rnd.Next(1, 9999999);
                    this.id = r;
                    path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + ".xml";
                }

                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { this.chosenMission });

                if (this.alternativeMission != null)
                {
                    string path2_alt = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml";
                    XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2_alt, new List<CustomBTNode>() { this.alternativeMission });
                }
            }

            public override TextObject Title {
                get {
                    TextObject textObject;
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        textObject = new TextObject(this.chosenMission.name + ": " + this.chosenMission.info, null);
                    }
                    else
                    {
                        textObject = new TextObject(this.chosenMission.Children[0].name + ": " + this.chosenMission.Children[0].info, null);
                    }
                    return textObject;
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
                    string textObject = "";
                    foreach (actionTarget aT in actionsInOrder)
                    {
                        textObject += aT.action + ", ";
                    }
                    return new TextObject(textObject, null);
                }
            }

            public override TextObject IssueAcceptByPlayer {
                get {
                    return new TextObject("Yes, I think I can handle that.", null);
                }
            }
            public override TextObject IssueQuestSolutionExplanationByIssueGiver {
                get {

                    return new TextObject("IssueQuestSolutionExplanationByIssueGiver", null);
                }
            }
            public override TextObject IssueQuestSolutionAcceptByPlayer {
                get {
                    return new TextObject("IssueQuestSolutionAcceptByPlayer", null);
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
                InformationManager.DisplayMessage(new InformationMessage("Quest Created \n"));
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
                    QuestBase AlternativeQuest = new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId + "_alternative", null, this.alternativeMission, this.alternativeActionsInOrder, alternativeJournalLogs, true);
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, this.chosenMission, this.actionsInOrder, this.journalLogs, false, (QuestGenTestQuest)AlternativeQuest);
                }
                else
                {
                    return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, this.chosenMission, this.actionsInOrder, this.journalLogs, false);
                }

            }

            protected override void OnGameLoad()
            {
                this.actionsInOrder = new List<actionTarget>();
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + ".xml";
                this.chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];
                this.chosenMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, false);

                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];

                    aT.bringTargetsBack();
                }
                this.chosenMission.run(CustomBTStep.issueQ, (IssueBase)this, this, false);
                if (File.Exists(@"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + "_alternative" + ".xml"))
                {
                    this.alternativeActionsInOrder = new List<actionTarget>();
                    string path2_alt = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + this.id + "_alternative" + ".xml";
                    this.alternativeMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2_alt)[0];

                    this.alternativeMission.run(CustomBTStep.actionTarget, (IssueBase)this, this, true);

                    for (int i = 0; i < this.alternativeActionsInOrder.Count; i++)
                    {
                        var aT = this.alternativeActionsInOrder[i];

                        aT.bringTargetsBack();
                    }
                    this.alternativeMission.run(CustomBTStep.issueQ, (IssueBase)this, this, true);
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
                    TextObject textObject = new TextObject(this.chosenMission.name + ": " +this.chosenMission.info, null);
                    if (alternativeFlag) textObject = new TextObject(textObject.ToString() + " alternative path given by " + this.actionsInOrder[0].questGiver.Name.ToString() , null);
                    return textObject;
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

            public QuestGenTestQuest(string questId, Hero questGiver, CustomBTNode chosenMission, List<actionTarget> actionsInOrder, List<JournalLog> journalLogs, bool alternative, QuestGenTestQuest alternativeReference = null) : base(questId, questGiver, CampaignTime.DaysFromNow(200f), 0)
            {
                this.alternativeFlag = alternative;
                InformationManager.DisplayMessage(new InformationMessage("Create Quest"));
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

                    aT.bringTargetsBack();
                }

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
                this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("DiscussDialog1.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver).BeginPlayerOptions().PlayerOption(new TextObject("DiscussDialog2.", null), null).NpcLine(new TextObject("DiscussDialog3.", null), null, null).CloseDialog().PlayerOption(new TextObject("DiscussDialog4.", null), null).NpcLine(new TextObject("DiscussDialog6.", null), null, null).CloseDialog().EndPlayerOptions().CloseDialog();
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

                this.currentAction = actionsInOrder[0];
                this.currentActionIndex = 0;

                this.chosenMission.run(CustomBTStep.questQ, (QuestBase)this, this);

                saveActionTargets();

            }

            private void saveActionTargets()
            {
                string id = base.StringId;

                string path1 = @"..\..\Modules\QuestGenerator\SaveFiles\actionsSaveFile_" + id + ".xml";
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + ".xml";
                XmlSerialization.WriteToXmlFile<List<actionTarget>>(path1, this.actionsInOrder);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path2, new List<CustomBTNode>() { this.chosenMission });
            }

            private void loadActionTargets()
            {
                string id = base.StringId;

                string path1 = @"..\..\Modules\QuestGenerator\SaveFiles\actionsSaveFile_" + id + ".xml";
                string path2 = @"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + ".xml";
                this.actionsInOrder = XmlSerialization.ReadFromXmlFile<List<actionTarget>>(path1);
                this.chosenMission = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path2)[0];

                if (File.Exists(@"..\..\Modules\QuestGenerator\SaveFiles\missionSaveFile_" + id + "_alternative" + ".xml") && alternativeFlag == false)
                {
                    foreach(QuestBase qb in Campaign.Current.QuestManager.Quests)
                    {
                        if (qb.StringId == base.StringId + "_alternative")
                        {
                            this.alternativeQuest = (QuestGenTestQuest)qb;
                            this.alternativeQuest.RegisterEvents();
                            this.alternativeQuest.InitializeQuestOnGameLoad();
                            using (List<QuestTaskBase>.Enumerator enumerator3 = this.alternativeQuest.TaskList.GetEnumerator())
                            {
                                while (enumerator3.MoveNext())
                                {
                                    QuestTaskBase questTaskBase = enumerator3.Current;
                                    if (questTaskBase.IsActive)
                                    {
                                        questTaskBase.SetReferences();
                                        questTaskBase.AddTaskDialogs();
                                    }
                                }
                                continue;
                            }
                        }
                    }
                }
                
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
                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 100, false);
                }
                TextObject textObject = new TextObject("You have completed the mission", null);
                base.AddLog(textObject, true);
                base.CompleteQuestWithSuccess();
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
                //CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
                //CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
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
                    switch (this.currentAction.action)
                    {
                        case "goto":
                            this.currentAction.OnSettlementEnteredQuest(party, settlement, hero, -1, this, (QuestBase)this);
                            break;
                        case "explore":
                            this.currentAction.OnSettlementEnteredQuest(party, settlement, hero, -1, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "give":
                            aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                            break;

                        case "gather":
                            aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                            break;

                        case "exchange":
                            aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                            break;
                    }

                }

            }

            private void OnPartyConsumedFood(MobileParty party)
            {
                if (party.IsMainParty)
                {
                    for (int i = 0; i < this.actionsInOrder.Count; i++)
                    {
                        var aT = this.actionsInOrder[i];
                        switch (aT.action)
                        {
                            case "give":
                                aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                                break;

                            case "exchange":
                                aT.OnPartyConsumedFoodQuest(party, i, this, (QuestBase)this);
                                break;
                        }

                    }
                }
            }

            private void OnHeroSharedFoodWithAnotherHero(Hero supporterHero, Hero supportedHero, float influence)
            {
                if (supporterHero == Hero.MainHero || supportedHero == Hero.MainHero)
                {
                    for (int i = 0; i < this.actionsInOrder.Count; i++)
                    {
                        var aT = this.actionsInOrder[i];
                        switch (aT.action)
                        {
                            case "give":
                                aT.OnHeroSharedFoodWithAnotherHeroQuest(supporterHero, supportedHero, influence, i, this, (QuestBase)this);
                                break;

                            case "exchange":
                                aT.OnHeroSharedFoodWithAnotherHeroQuest(supporterHero, supportedHero, influence, i, this, (QuestBase)this);
                                break;
                        }

                    }
                }
            }

            private void OnEquipmentSmeltedByHeroEvent(Hero hero, EquipmentElement equipmentElement)
            {
                if (hero.PartyBelongedTo == MobileParty.MainParty)
                {
                    for (int i = 0; i < this.actionsInOrder.Count; i++)
                    {
                        var aT = this.actionsInOrder[i];
                        switch (aT.action)
                        {
                            case "gather":
                                aT.OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                                break;

                            case "exchange":
                                aT.OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                                break;
                        }

                    }
                }
            }

            private void OnNewItemCraftedEvent(ItemObject item, Crafting.OverrideData crafted, bool flag)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "gather":
                            aT.OnNewItemCraftedEventQuest(item, crafted, flag, i, this, (QuestBase)this);
                            break;
                        case "exchange":
                            aT.OnNewItemCraftedEventQuest(item, crafted, flag, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void OnItemProducedEvent(ItemObject itemObject, Settlement settlement, int count)
            {

            }

            private void OnQuestCompletedEvent(QuestBase quest, QuestCompleteDetails questCompleteDetails)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "quest":
                            aT.OnQuestCompletedEventQuest(quest, questCompleteDetails, i, this, (QuestBase)this);
                            break;
                    }
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
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "capture":
                            aT.OnPrisonerTakenEvent(rooster, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void HeroPrisonerTaken(PartyBase capturer, Hero prisoner)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "capture":
                            aT.HeroPrisonerTaken(capturer, prisoner, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "capture":
                            aT.HeroKilledEvent(victim, killer, detail, showNotification, i, this, (QuestBase)this);
                            break;
                        case "kill":
                            aT.HeroKilledEvent(victim, killer, detail, showNotification, i, this, (QuestBase)this);
                            break;
                        case "damage":
                            aT.HeroKilledEvent(victim, killer, detail, showNotification, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void MapEventEnded(MapEvent mapEvent)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "kill":
                            aT.MapEventEnded(mapEvent, i, this, (QuestBase)this);
                            break;
                        case "take":
                            aT.MapEventEnded(mapEvent, i, this, (QuestBase)this);
                            break;
                        case "damage":
                            aT.MapEventEnded(mapEvent, i, this, (QuestBase)this);
                            break;
                    }
                }
            }
            
            private void HeroPrisonerReleased (Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "capture":
                            aT.HeroPrisonerReleased(prisoner, party, capturerFaction, detail, i, this, (QuestBase)this);
                            break;
                        case "free":
                            aT.HeroPrisonerReleased(prisoner, party, capturerFaction, detail, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "free":
                            aT.PrisonersChangeInSettlement(settlement, prisonerRoster, prisonerHero, isReleased, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void HeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "use":
                            aT.HeroGainedSkill(hero, skill, hasNewPerk, change, shouldNotify, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void RaidCompletedEvent(BattleSideEnum winnerSide, MapEvent mapEvent)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "damage":
                            aT.RaidCompletedEvent(winnerSide, mapEvent, i, this, (QuestBase)this);
                            break;
                    }
                }
            }
        }
    }
}
