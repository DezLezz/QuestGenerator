using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using static TaleWorlds.CampaignSystem.QuestBase;

namespace QuestGenerator
{
    [Serializable]
    public abstract class actionTarget
    {
        public string action;

        public string target;

        public actionTarget(string action, string target)
        {
            this.action = action;
            this.target = target;
        }

        public actionTarget() { }

        public abstract void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver);
        public abstract void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index);

        public abstract void bringTargetsBack();

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

        public virtual void OnNewItemCraftedEventQuest(ItemObject item, Crafting.OverrideData crafted, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void OnItemProducedEventQuest(ItemObject itemObject, Settlement settlement, int count, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

        public virtual void OnQuestCompletedEventQuest(QuestBase quest, QuestCompleteDetails questCompleteDetails,int index, QuestGenTestQuest questGen, QuestBase questBase)
        {

        }

    }
}
