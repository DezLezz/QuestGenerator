using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using static QuestGenerator.QuestGenTestCampaignBehavior;

namespace QuestGenerator
{
    [Serializable]
    class listenAction : actionTarget
    {
        [NonSerialized]
        public Hero heroTarget;
        public listenAction(string action, string target) : base(action, target)
        {
        }

        public listenAction() { }

        public override Hero GetHeroTarget()
        {
            return this.heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            this.heroTarget = newH;
        }

        public override void bringTargetsBack()
        {
            if (this.heroTarget == null)
            {
                var setName = this.target;

                Hero[] array = (from x in Hero.All where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Everything is on fire BTB listen"));
                }
                if (array.Length == 1)
                {
                    this.heroTarget = array[0];
                }
            }
        }
        public override void IssueQ(List<actionTarget> list, Settlement issueSettlement, Hero issueGiver)
        {
            if (this.target.Contains("npc"))
            {
                string npcNumb = this.target;
                string targetHero = "none";
                Hero newHero = new Hero();
                int i = list.IndexOf(this);
                if (i > 0)
                {
                    if (list[i - 1].action == "goto")
                    {
                        Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == list[i - 1].target) select x).ToArray<Settlement>();

                        if (array.Length > 1)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Everything is on fire Issue"));
                        }
                        if (array.Length == 1)
                        {
                            newHero = array[0].Notables.GetRandomElement();
                            targetHero = newHero.Name.ToString();

                        }

                        if (targetHero != "none")
                        {
                            foreach (actionTarget nextAction in list)
                            {
                                if (nextAction.target == npcNumb)
                                {
                                    nextAction.target = targetHero;
                                    nextAction.SetHeroTarget(newHero);
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (Hero hero in issueSettlement.Notables)
                        {
                            if (hero != issueGiver)
                            {
                                targetHero = hero.Name.ToString();
                                newHero = hero;
                            }
                        }

                        if (targetHero != "none")
                        {
                            foreach (actionTarget nextAction in list)
                            {
                                if (nextAction.target == npcNumb)
                                {
                                    nextAction.target = targetHero;
                                    nextAction.SetHeroTarget(newHero);
                                }
                            }
                        }
                    }
                }

                else if (i == 0)
                {
                    foreach (Hero hero in issueSettlement.Notables)
                    {
                        if (hero != issueGiver)
                        {
                            targetHero = hero.Name.ToString();
                            newHero = hero;
                            break;
                        }
                    }

                    if (targetHero != "none")
                    {
                        foreach (actionTarget nextAction in list)
                        {
                            if (nextAction.target == npcNumb)
                            {
                                nextAction.target = targetHero;
                                nextAction.SetHeroTarget(newHero);
                                break;
                            }
                        }
                    }
                }

                if (targetHero == "none")
                {
                    InformationManager.DisplayMessage(new InformationMessage("Target Hero is on fire"));
                }
                

            }
        }

        public override void QuestQ(List<actionTarget> list, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen, int index)
        {
            if (this.heroTarget != null)
            {

                questBase.AddTrackedObject(this.heroTarget);
                TextObject textObject = new TextObject("{=YXbKXUDu}Listen to {HERO}", null);
                textObject.SetTextVariable("HERO", this.heroTarget.Name);
                questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                InformationManager.DisplayMessage(new InformationMessage("Hero " + this.heroTarget.Name + " tracked to listen"));

                Campaign.Current.ConversationManager.AddDialogFlow(this.GetListenActionDialogFlow(this.heroTarget, index, questGiver, questBase, questGen), this);

            }
        }

        public override DialogFlow getDialogFlows(int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            return GetListenActionDialogFlow(this.heroTarget, index,questGiver,questBase,questGen);
        }

        private DialogFlow GetListenActionDialogFlow(Hero target, int index, Hero questGiver, QuestBase questBase, QuestGenTestQuest questGen)
        {
            TextObject npcLine1 = new TextObject("Hello there.", null);
            TextObject playerLine1 = new TextObject("{QUEST_GIVER.LINK} told me to come and listen to what you have to say.", null);
            StringHelpers.SetCharacterProperties("QUEST_GIVER", questGiver.CharacterObject, playerLine1);
            TextObject npcLine2 = new TextObject("Yes, so I've heard. I only have to inform you that this quest is working.", null);
            InformationManager.DisplayMessage(new InformationMessage("return listen dialog flow"));
            return DialogFlow.CreateDialogFlow("start", 125).NpcLine(npcLine1, null, null).Condition(() => Hero.OneToOneConversationHero == target && index == questGen.currentActionIndex).PlayerLine(playerLine1, null).NpcLine(npcLine2, null, null).Consequence(delegate
            {
                this.listenConsequences(index, questBase, questGen);
            }).CloseDialog();
        }

        private void listenConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            questGen.currentActionIndex++;
            questGen.UpdateQuestTaskS(questGen.journalLogs[index], 1);

            if (questGen.currentActionIndex < questGen.actionsTargets.Count)
            {
                questGen.currentAction = questGen.actionsTargets[questGen.currentActionIndex];
            }
            else
            {
                questGen.SuccessConsequences();
            }
        }
    }
}
