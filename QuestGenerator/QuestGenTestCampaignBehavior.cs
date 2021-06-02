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
            }
        }

        private void OnCheckForIssue(Hero hero)
        {

            if (HeroMotivations.Count == 0)
            {

                List<string> motivations = new List<string>() { "Knowledge", "Comfort", "Reputation", "Serenity", "Protection", "Conquest", "Wealth", "Ability", "Equipment" };
                var heroList = Hero.All;

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
            InformationManager.DisplayMessage(new InformationMessage("Issue Created for hero " + issueOwner.Name.ToString()));
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

            motiv = HeroMotivations[issueGiver];

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
            public Hero missionHero;

            public CustomBTNode chosenMission;

            public CustomBTNode alternativeMission;

            //public List<string> subQuests;

            //public int subQuestCounter;

            public List<JournalLog> journalLogs = new List<JournalLog>();

            public List<actionTarget> actionsInOrder = new List<actionTarget>();

            public List<actionTarget> alternativeActionsInOrder = new List<actionTarget>();

            protected override bool IssueQuestCanBeDuplicated {
                get {
                    return true;
                }
            }
            public QuestGenTestIssue(Hero issueOwner, CustomBTNode chosenMission) : base(issueOwner, CampaignTime.DaysFromNow(200f))
            {
                this.missionHero = issueOwner;

                if (chosenMission.nodeType == CustomBTType.motivation)
                {
                    this.chosenMission = chosenMission;
                    this.chosenMission.run(CustomBTStep.issueQ, (IssueBase)this, this, false);
                }
                else
                {
                    this.chosenMission = chosenMission.Children[0];
                    this.chosenMission.run(CustomBTStep.issueQ, (IssueBase)this, this, false);

                    this.alternativeMission = chosenMission.Children[1];
                    this.alternativeMission.run(CustomBTStep.issueQ, (IssueBase)this, this, true);

                }
            }


            public override TextObject Title {
                get {
                    TextObject textObject;
                    if (chosenMission.nodeType == CustomBTType.motivation)
                    {
                        textObject = new TextObject(this.chosenMission.name, null);
                    }
                    else
                    {
                        textObject = new TextObject(this.chosenMission.Children[0].name, null);
                    }
                    return textObject;
                }
            }
            public override TextObject Description {
                get {
                    string textObject = "";
                    foreach (actionTarget aT in actionsInOrder)
                    {
                        textObject += aT.action ;
                    }
                    return new TextObject(textObject, null);
                }
            }
            protected override TextObject IssueBriefByIssueGiver {
                get {
                    string textObject = "";
                    foreach (actionTarget aT in actionsInOrder)
                    {
                        textObject += aT.action ;
                    }
                    return new TextObject(textObject, null);
                }
            }

            protected override TextObject IssueAcceptByPlayer {
                get {
                    return new TextObject("IssueAcceptByPlayer", null);
                }
            }
            protected override TextObject IssueQuestSolutionExplanationByIssueGiver {
                get {

                    return new TextObject("IssueQuestSolutionExplanationByIssueGiver", null);
                }
            }
            protected override TextObject IssueQuestSolutionAcceptByPlayer {
                get {
                    return new TextObject("IssueQuestSolutionAcceptByPlayer", null);
                }
            }
            protected override bool IsThereAlternativeSolution {
                get {
                    return false;
                }
            }

            protected override bool IsThereLordSolution {
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
                throw new NotImplementedException();
            }


            protected override QuestBase GenerateIssueQuest(string questId)
            {
                InformationManager.DisplayMessage(new InformationMessage("Quest Created \n"));
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
            public bool alternativeFlag;

            public QuestGenTestQuest alternativeQuest;

            //private JournalLog _gotoQuestLog;
            //private JournalLog _listenQuestLog;
            //private JournalLog _reportQuestLog;

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
                    TextObject textObject = new TextObject(this.chosenMission.name, null);
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
                    this.missionHero.AddEventForOccupiedHero(base.StringId);
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
                    switch (aT.action)
                    {

                        case "listen":
                            Campaign.Current.ConversationManager.AddDialogFlow(aT.getDialogFlows(i, this.missionHero, (QuestBase)this, this), this);
                            break;
                        case "report":
                            Campaign.Current.ConversationManager.AddDialogFlow(aT.getDialogFlows(i, this.missionHero, (QuestBase)this, this), this);
                            break;
                        case "give":
                            if (aT.GetHeroTarget() != null)
                            {
                                Campaign.Current.ConversationManager.AddDialogFlow(aT.getDialogFlows(i, this.missionHero, (QuestBase)this, this), this);
                            }
                            break;
                        case "exchange":
                            if (aT.GetHeroTarget() != null)
                            {
                                Campaign.Current.ConversationManager.AddDialogFlow(aT.getDialogFlows(i, this.missionHero, (QuestBase)this, this), this);
                            }
                            break;
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
                            this.alternativeQuest.InitializeQuestOnGameLoad();
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
                CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, Crafting.OverrideData>(this.OnNewItemCraftedEvent));
                CampaignEvents.OnItemProducedEvent.AddNonSerializedListener(this, new Action<ItemObject, Settlement, int>(this.OnItemProducedEvent));
                CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestCompleteDetails>(this.OnQuestCompletedEvent));
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
                                if (aT.GetHeroTarget() != null)
                                {
                                    aT.OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                                }
                                break;
                        }

                    }
                }
            }

            private void OnNewItemCraftedEvent(ItemObject item, Crafting.OverrideData crafted)
            {
                for (int i = 0; i < this.actionsInOrder.Count; i++)
                {
                    var aT = this.actionsInOrder[i];
                    switch (aT.action)
                    {
                        case "gather":
                            aT.OnNewItemCraftedEventQuest(item, crafted, i, this, (QuestBase)this);
                            break;
                        case "exchange":
                            if (aT.GetHeroTarget() != null)
                            {
                                aT.OnNewItemCraftedEventQuest(item, crafted, i, this, (QuestBase)this);
                            }
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
                    if (quest.StringId + "_alternative" == base.StringId && this.currentActionIndex == this.actionsInOrder.Count) 
                    {
                        this.SuccessConsequences();
                    }
                }
                else
                {
                    if (quest.StringId.Replace("_alternative", "") == base.StringId && this.currentActionIndex == this.actionsInOrder.Count)
                    {
                        this.SuccessConsequences();
                    }
                }
            }
        }
    }
}
