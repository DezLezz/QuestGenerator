using QuestGenerator.QuestBuilder.CustomBT;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using static TaleWorlds.CampaignSystem.QuestBase;

namespace QuestGenerator
{
    [XmlInclude(typeof(gotoAction)), XmlInclude(typeof(giveAction)), XmlInclude(typeof(gatherAction)), 
        XmlInclude(typeof(exchangeAction)), XmlInclude(typeof(exploreAction)), XmlInclude(typeof(listenAction)), 
        XmlInclude(typeof(reportAction)), XmlInclude(typeof(subquestAction)), XmlInclude(typeof(captureAction))
        , XmlInclude(typeof(killAction)), XmlInclude(typeof(freeAction)), XmlInclude(typeof(takeAction))]
    public abstract class actionTarget
    {
        public string action;

        public int index;
        public List<CustomBTNode> children;

        [XmlIgnore]
        public Hero questGiver;

        public string questGiverString;

        public QuestGenerator.QuestBuilder.Action Action { get; set; }

        public actionTarget(string action, QuestGenerator.QuestBuilder.Action action1)
        {
            this.action = action;
            this.Action = action1;
        }

        public actionTarget() { }

        public abstract void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative);
        public abstract void QuestQ(QuestBase questBase, QuestGenTestQuest questGen);

        public abstract void bringTargetsBack();

        public abstract void updateHeroTargets(string targetString, Hero targetHero);
        public abstract void updateSettlementTargets(string targetString, Settlement targetSettlement);
        public abstract void updateItemTargets(string targetString, ItemObject targetItem);

        public virtual DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return null;
        }

        public virtual Settlement GetSettlementTarget()
        {
            return null;
        }

        public virtual Hero GetHeroTarget()
        {
            return null;
        }

        public virtual void SetSettlementTarget(Settlement newS)
        {
        }

        public virtual void SetHeroTarget(Hero newH)
        {
        }

        public virtual ItemObject GetItemTarget()
        {
            return null;
        }

        public virtual void SetItemTarget(ItemObject newI)
        {
        }

        public virtual int GetItemAmount()
        {
            return 0;
        }

        public virtual void SetItemAmount(int newI)
        {
        }

        public virtual void OnSettlementEnteredQuest(MobileParty party, Settlement settlement, Hero hero, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
        }

        public virtual void OnPlayerInventoryExchangeQuest(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
        }

        public virtual void OnPartyConsumedFoodQuest(MobileParty party, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
        }

        public virtual void OnHeroSharedFoodWithAnotherHeroQuest(Hero supporterHero, Hero supportedHero, float influence, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
        }

        public virtual void OnEquipmentSmeltedByHeroEventQuest(Hero hero, EquipmentElement equipmentElement, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void OnNewItemCraftedEventQuest(ItemObject item, Crafting.OverrideData crafted, bool flag, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void OnItemProducedEventQuest(ItemObject itemObject, Settlement settlement, int count, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void OnQuestCompletedEventQuest(QuestBase quest, QuestCompleteDetails questCompleteDetails, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }
        public virtual void OnPrisonerTakenEvent(FlattenedTroopRoster rooster, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void HeroPrisonerTaken(PartyBase capturer, Hero prisoner, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void HeroKilledEvent(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void MapEventEnded(MapEvent mapEvent, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void HeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void PrisonersChangeInSettlement(Settlement settlement, FlattenedTroopRoster prisonerRoster, Hero prisonerHero, bool isReleased, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }
    }
}
