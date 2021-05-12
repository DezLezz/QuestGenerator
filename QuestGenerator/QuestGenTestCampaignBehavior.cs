using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.SandBox;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace QuestGenerator
{
    public class QuestGenTestCampaignBehavior : CampaignBehaviorBase
    {

        private const IssueBase.IssueFrequency QuestTestIssueFrequency = IssueBase.IssueFrequency.Common;

        [SaveableField(100)]
        private static List<heroPatch> HeroMotivations = new List<heroPatch>();

        private string path = @"..\..\Modules\QuestGenerator\missionList.txt";

        //[SaveableField(101)]
        private string[] missions;

        //[SaveableField(102)]
        private string chosenMission;

        static Random rnd = new Random();

        //[SaveableField(103)]

        public QuestGenTestCampaignBehavior()
        {
            string[] newMissions = File.ReadAllLines(path);
            this.missions = newMissions;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
            //OnGameLoadFinishedEvents
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
                    heroPatch hP = new heroPatch(h, motivations[r]);
                    HeroMotivations.Add(hP);
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

            foreach (heroPatch hP in HeroMotivations)
            {
                if (issueOwner.Name == hP.hero.Name)
                {
                    motiv = hP.motivation;
                    break;
                }
            }

            foreach (string s in this.missions)
            {
                string[] newS = s.Split(',');
                if (newS[0] == motiv)
                {
                    this.chosenMission = s;
                    File.WriteAllLines(this.path, File.ReadLines(this.path).Where(l => l != s ).ToList());
                    this.missions = this.missions.Where(val => val != s).ToArray();
                    break;
                }
            }
            

            
            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, this.chosenMission);
        }

        private bool ConditionsHold(Hero issueGiver)
        {
            string motiv = "none";

            foreach (heroPatch hP in HeroMotivations)
            {
                if (issueGiver.Name == hP.hero.Name)
                {
                    motiv = hP.motivation;
                    break;
                }
            }

            if (this.missions.IsEmpty())
            {
                string[] newMissions = File.ReadAllLines(path);
                this.missions = newMissions;
            }

            if (motiv == "none")
            {
                return false;
            }

            if (this.missions.IsEmpty())
            {
                return false;
            }

            foreach (string s in this.missions)
            {
                string[] newS = s.Split(',');
                if (newS[0] == motiv)
                {
                    return true;
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
        internal class QuestGenTestIssue : IssueBase
        {
            public Hero missionHero;

            public string[] chosenMission;

            public List<string> subQuests;

            public int subQuestCounter;

            public List<actionTarget> actionsTargets = new List<actionTarget>();

            public List<JournalLog> journalLogs = new List<JournalLog>();

            protected override bool IssueQuestCanBeDuplicated
            {
                get
                {
                    return true;
                }
            }
            public QuestGenTestIssue(Hero issueOwner, string chosenMission) : base(issueOwner, CampaignTime.DaysFromNow(200f))
            {
                //Knowledge,goto place1,listen npc1
                this.missionHero = issueOwner;

                if(chosenMission.Contains("|"))
                {
                    subQuests = new List<string>();
                    subQuestCounter = 0;
                    string[] missions = chosenMission.Split('|');
                    this.chosenMission = missions[0].Split(',');
                    for (int i = 1; i < missions.Length; i++)
                    {
                        subQuests.Add(missions[i]);
                    }

                }
                else
                {
                    this.chosenMission = chosenMission.Split(',');
                }

                

                for (int i = 1; i < this.chosenMission.Count(); i++) 
                {
                    string[] ap = this.chosenMission[i].Split(' ');
                    switch (ap[0])
                    {
                        case "goto":
                            actionTarget newGoto = new gotoAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newGoto);
                            break;
                        case "listen":
                            actionTarget newListen = new listenAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newListen);
                            break;
                        case "report":
                            actionTarget newReport = new reportAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newReport);
                            break;
                        case "give":
                            actionTarget newGive= new giveAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newGive);
                            break;
                        case "gather":
                            actionTarget newGather = new gatherAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newGather);
                            break;
                        case "explore":
                            actionTarget newExplore = new exploreAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newExplore);
                            break;
                        case "quest":
                            actionTarget newQuest = new subquestAction(ap[0], ap[1]);
                            this.actionsTargets.Add(newQuest);
                            break;
                    }
                }
                Settlement issueS = base.IssueSettlement;
                foreach (actionTarget currentAction in this.actionsTargets)
                {
                    currentAction.IssueQ(actionsTargets, issueS, base.IssueOwner);

                    if (currentAction.action == "quest")
                    {
                        PotentialIssueData potentialIssueData = new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common);
                        Campaign.Current.IssueManager.CreateNewIssue(potentialIssueData, currentAction.GetHeroTarget());
                    }
                }
                
            }

            public IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
            {

                return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, this.subQuests[subQuestCounter]);
            }

            public override TextObject Title
            {
                get
                {
                    int r = rnd.Next(5);
                    TextObject textObject = new TextObject(this.chosenMission[0] + ": " + r, null);
                    return textObject;
                }
            }
            public override TextObject Description
            {
                get
                {
                    string textObject = "";
                    foreach (actionTarget aT in actionsTargets)
                    {
                        textObject += aT.action + " to " + aT.target + " ";
                    }
                    return new TextObject(textObject, null);
                }
            }
            protected override TextObject IssueBriefByIssueGiver

            {
                get
                {
                    string textObject = "";
                    foreach (actionTarget aT in actionsTargets)
                    {
                        textObject += aT.action + " to " + aT.target + " ";
                    }
                    return new TextObject(textObject, null);
                }
            }

            protected override TextObject IssueAcceptByPlayer
            {
                get
                {
                    return new TextObject("IssueAcceptByPlayer", null);
                }
            }
            protected override TextObject IssueQuestSolutionExplanationByIssueGiver
            {
                get
                {
                    
                    return new TextObject("IssueQuestSolutionExplanationByIssueGiver", null);
                }
            }
            protected override TextObject IssueQuestSolutionAcceptByPlayer
            {
                get
                {
                    return new TextObject("IssueQuestSolutionAcceptByPlayer", null);
                }
            }
            protected override bool IsThereAlternativeSolution
            {
                get
                {
                    return false;
                }
            }

            protected override bool IsThereLordSolution
            {
                get
                {
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
                foreach (actionTarget aT in actionsTargets)
                {
                    this.journalLogs.Add(new JournalLog(this.IssueDueTime, new TextObject(aT.action + "JournalLog", null)));
                }
                return new QuestGenTestCampaignBehavior.QuestGenTestQuest(questId, base.IssueOwner, this.chosenMission, this.actionsTargets, this.journalLogs);
            }

            protected override void OnGameLoad()
            {
            }
            

        }

        public class QuestGenTestQuest : QuestBase
        {
            [SaveableField(110)]
            public Hero missionHero;

            [SaveableField(109)]
            public string[] chosenMission;

            public List<actionTarget> actionsTargets;

            public actionTarget currentAction;

            [SaveableField(112)]
            public int currentActionIndex;

            [SaveableField(113)]
            public List<JournalLog> journalLogs;

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
                //collectedObjects.Add(this._gotoQuestLog);
                //collectedObjects.Add(this._listenQuestLog);
                //collectedObjects.Add(this._reportQuestLog);
            }

            //internal static object AutoGeneratedGetMemberValue_gotoQuestLog(object o)
            //{
            //    return ((QuestGenTestCampaignBehavior.QuestGenTestQuest)o)._gotoQuestLog;
            //}

            //internal static object AutoGeneratedGetMemberValue_listenQuestLog(object o)
            //{
            //    return ((QuestGenTestCampaignBehavior.QuestGenTestQuest)o)._listenQuestLog;
            //}

            //internal static object AutoGeneratedGetMemberValue_reportQuestLog(object o)
            //{
            //    return ((QuestGenTestCampaignBehavior.QuestGenTestQuest)o)._reportQuestLog;
            //}

            public override bool IsRemainingTimeHidden
            {
                get
                {
                    return false;
                }
            }

            public override TextObject Title
            {
                get
                {
                    TextObject textObject = new TextObject(this.chosenMission[0], null);
                    return textObject;
                }
            }

            private TextObject SuccessQuestLogText
            {
                get
                {
                    TextObject textObject = new TextObject("You did it.", null);
                    return textObject;
                }
            }

            private TextObject FailTimedOutQuestLogText
            {
                get
                {
                    TextObject textObject = new TextObject("Timed out quest test.", null);
                    return textObject;
                }
            }

            public QuestGenTestQuest(string questId, Hero questGiver, string[] chosenMission, List<actionTarget> actionsTargets, List<JournalLog> journalLogs) : base(questId, questGiver, CampaignTime.DaysFromNow(200f), 0)
            {

                InformationManager.DisplayMessage(new InformationMessage("Create Quest"));
                this.missionHero = questGiver;
                this.chosenMission = chosenMission;
                this.actionsTargets = actionsTargets;
                this.journalLogs = journalLogs;

                this.missionHero.AddEventForOccupiedHero(base.StringId);

                

                this.SetDialogs();
                base.InitializeQuestOnCreation();

            }
            protected override void InitializeQuestOnGameLoad()
            {
                this.SetDialogs();
                loadActionTargets();
                this.currentAction = this.actionsTargets[this.currentActionIndex];

                for (int i = 0; i < this.actionsTargets.Count; i++)
                {
                    var aT = this.actionsTargets[i];

                    aT.bringTargetsBack();
                }

                for (int i = 0; i < this.actionsTargets.Count; i++)
                {
                    var aT = this.actionsTargets[i];
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

                this.currentAction = actionsTargets[0];
                this.currentActionIndex = 0;

                for (int i = 0; i < actionsTargets.Count; i++)
                {
                    var cAction = actionsTargets[i];
                    cAction.QuestQ(this.actionsTargets, this.missionHero, (QuestBase)this, this, i);
                }

                saveActionTargets();
            }

            private void saveActionTargets() 
            {
                string id = base.StringId;
                string path = @"..\..\Modules\QuestGenerator\SaveFiles\saveFile" + id + ".bin";
                BinarySerialization.WriteToBinaryFile<List<actionTarget>>(path, this.actionsTargets);
            }

            private void loadActionTargets()
            {
                string id = base.StringId;
                string path = @"..\..\Modules\QuestGenerator\SaveFiles\saveFile" + id + ".bin";
                this.actionsTargets = BinarySerialization.ReadFromBinaryFile<List<actionTarget>>(path);
            }

            public bool ReturnItemClickableConditions(out TextObject explanation)
            {
                if(this.actionsTargets[currentActionIndex+1].GetItemTarget() != null)
                {
                    if (this.journalLogs[currentActionIndex].CurrentProgress >= this.actionsTargets[currentActionIndex + 1].GetItemAmount())
                    {
                        explanation = TextObject.Empty;
                        return true;
                    }
                }

                
                explanation = new TextObject("You don't have enough of that item.", null);
                return false;
            }

            

            public void SuccessConsequences()
            {
                GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 100, false);
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
                for (int i = 0; i < this.actionsTargets.Count; i++)
                {
                    var aT = this.actionsTargets[i];
                    switch (aT.action)
                    {
                        case "give":
                            aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                            break;

                        case "gather":
                            aT.OnPlayerInventoryExchangeQuest(purchasedItems, soldItems, isTrading, i, this, (QuestBase)this);
                            break;
                    }

                }

            }

            private void OnPartyConsumedFood(MobileParty party)
            {
                if (party.IsMainParty)
                {
                    for (int i = 0; i < this.actionsTargets.Count; i++)
                    {
                        var aT = this.actionsTargets[i];
                        switch (aT.action)
                        {
                            case "give":  
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
                    for (int i = 0; i < this.actionsTargets.Count; i++)
                    {
                        var aT = this.actionsTargets[i];
                        switch (aT.action)
                        {
                            case "give":  
                                aT.OnHeroSharedFoodWithAnotherHeroQuest(supporterHero,supportedHero, influence, i, this, (QuestBase)this);
                                break;
                        }

                    }
                }
            }

            private void OnEquipmentSmeltedByHeroEvent(Hero hero, EquipmentElement equipmentElement)
            {
                if (hero.PartyBelongedTo == MobileParty.MainParty)
                {
                    for (int i = 0; i < this.actionsTargets.Count; i++)
                    {
                        var aT = this.actionsTargets[i];
                        switch (aT.action)
                        {
                            case "gather":
                                aT.OnEquipmentSmeltedByHeroEventQuest(hero, equipmentElement, i, this, (QuestBase)this);
                                break;
                        }

                    }
                }
            }

            private void OnNewItemCraftedEvent(ItemObject item, Crafting.OverrideData crafted)
            {
                for (int i = 0; i < this.actionsTargets.Count; i++)
                {
                    var aT = this.actionsTargets[i];
                    switch (aT.action)
                    {
                        case "gather":
                            aT.OnNewItemCraftedEventQuest(item, crafted, i, this, (QuestBase)this);
                            break;
                    }
                }
            }

            private void OnItemProducedEvent(ItemObject itemObject, Settlement settlement, int count) 
            {
                
            }

            private void OnQuestCompletedEvent(QuestBase quest, QuestCompleteDetails questCompleteDetails)
            {
                for (int i = 0; i < this.actionsTargets.Count; i++)
                {
                    var aT = this.actionsTargets[i];
                    switch (aT.action)
                    {
                        case "quest":
                            aT.OnQuestCompletedEventQuest(quest, questCompleteDetails, i, this, (QuestBase)this);
                            break;
                    }
                }
            }
        }
    }
}
