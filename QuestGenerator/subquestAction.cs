using Helpers;
using System.Linq;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using ThePlotLords.QuestBuilder;
using ThePlotLords.QuestBuilder.CustomBT;
using static TaleWorlds.CampaignSystem.QuestBase;
using static ThePlotLords.QuestGenTestCampaignBehavior;

namespace ThePlotLords
{
    public class subquestAction : actionTarget
    {

        [XmlIgnore]
        public Hero heroTarget;

        [XmlIgnore]
        public Settlement settlementTarget;

        public string settlementTargetString;

        public bool questCreated = false;

        public subquestAction(string action, ThePlotLords.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public subquestAction() { }

        public override Hero GetHeroTarget()
        {
            return heroTarget;
        }

        public override void SetHeroTarget(Hero newH)
        {
            heroTarget = newH;
        }

        public override Settlement GetSettlementTarget()
        {
            return settlementTarget;
        }

        public override void SetSettlementTarget(Settlement newS)
        {
            settlementTarget = newS;
        }

        public override void bringTargetsBack()
        {
            if (settlementTarget == null)
            {
                var setName = settlementTargetString;

                Settlement[] array = (from x in Settlement.All where (x.Name.ToString() == setName) select x).ToArray<Settlement>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("subquest action - line 62"));
                }
                if (array.Length == 1)
                {
                    settlementTarget = array[0];
                }
            }

            if (heroTarget == null)
            {
                var setName = this.Action.param[0].target;

                heroTarget = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
            if (questGiver == null)
            {
                var setName = questGiverString;

                questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("npc"))
            {
                string npcNumb = this.Action.param[0].target;
                string targetHero = "none";
                Hero newHero = new Hero();

                if (index > 0)
                {
                    if (alternative)
                    {
                        if (questGen.alternativeActionsInOrder[index - 1].action == "goto")
                        {
                            Settlement settlement = questGen.alternativeActionsInOrder[index - 1].GetSettlementTarget();


                            foreach (Hero h in settlement.Notables)
                            {
                                if (h.Issue == null && h.CanHaveQuestsOrIssues())
                                {
                                    newHero = h;
                                    targetHero = h.Name.ToString();
                                    break;
                                }
                            }
                            settlementTargetString = settlement.Name.ToString();
                            settlementTarget = settlement;

                        }
                        else
                        {
                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                if (Enumerable.Any<Hero>(x.Notables, (Hero y) => y.CanHaveQuestsOrIssues()) && x.MapFaction == questGiver.MapFaction)
                                {
                                    return true;
                                }
                                return false;
                            });

                            foreach (Hero h in settlement.Notables)
                            {
                                if (h.Issue == null && h.CanHaveQuestsOrIssues())
                                {
                                    newHero = h;
                                    targetHero = newHero.Name.ToString();
                                    break;
                                }
                            }

                            settlementTargetString = settlement.Name.ToString();
                            settlementTarget = settlement;
                        }
                    }
                    else
                    {
                        if (questGen.actionsInOrder[index - 1].action == "goto")
                        {
                            Settlement settlement = questGen.actionsInOrder[index - 1].GetSettlementTarget();


                            foreach (Hero h in settlement.Notables)
                            {
                                if (h.Issue == null && h.CanHaveQuestsOrIssues())
                                {
                                    newHero = h;
                                    targetHero = h.Name.ToString();
                                    break;
                                }
                            }
                            settlementTargetString = settlement.Name.ToString();
                            settlementTarget = settlement;

                        }
                        else
                        {
                            Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                            {
                                if (Enumerable.Any<Hero>(x.Notables, (Hero y) => y.CanHaveQuestsOrIssues()) && x.MapFaction == questGiver.MapFaction)
                                {
                                    return true;
                                }
                                return false;
                            });

                            foreach (Hero h in settlement.Notables)
                            {
                                if (h.Issue == null && h.CanHaveQuestsOrIssues())
                                {
                                    newHero = h;
                                    targetHero = newHero.Name.ToString();
                                    break;
                                }
                            }

                            settlementTargetString = settlement.Name.ToString();
                            settlementTarget = settlement;
                        }
                    }
                }
                else if (index == 0)
                {
                    Settlement settlement = SettlementHelper.FindRandomSettlement(delegate (Settlement x)
                    {
                        if (Enumerable.Any<Hero>(x.Notables, (Hero y) => y.CanHaveQuestsOrIssues()) && x.MapFaction == questGiver.MapFaction)
                        {
                            return true;
                        }
                        return false;
                    });

                    foreach (Hero h in settlement.Notables)
                    {
                        if (h.Issue == null && h.CanHaveQuestsOrIssues())
                        {
                            newHero = h;
                            targetHero = newHero.Name.ToString();
                            break;
                        }
                    }

                    settlementTargetString = settlement.Name.ToString();
                    settlementTarget = settlement;
                }
                if (targetHero != "none")
                {
                    if (alternative)
                    {
                        questGen.alternativeMission.updateHeroTargets(npcNumb, newHero);
                    }
                    else
                    {
                        questGen.chosenMission.updateHeroTargets(npcNumb, newHero);
                    }
                }
                if (targetHero == "none")
                {
                    InformationManager.DisplayMessage(new InformationMessage("subquest action - line 243"));
                }
            }

            else if (settlementTarget == null)
            {
                settlementTarget = heroTarget.CurrentSettlement;
                settlementTargetString = settlementTarget.Name.ToString();
            }


            children[0].origin_quest_hero = questGiver.Name.ToString();

            if (heroTarget != null)
            {
                if (HeroMotivations.ContainsKey(heroTarget))
                {
                    HeroMotivations[heroTarget] = "WaitingForSubQuest";
                }
                else
                {
                    HeroMotivations.Add(heroTarget, "WaitingForSubQuest");
                }
            }

        }

        public IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
        {
            return new QuestGenTestCampaignBehavior.QuestGenTestIssue(issueOwner, children[0]);
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (heroTarget.CanHaveQuestsOrIssues() && !questCreated)
            {
                PotentialIssueData potentialIssueData = new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(QuestGenTestCampaignBehavior.QuestGenTestIssue), IssueBase.IssueFrequency.Common);
                Campaign.Current.IssueManager.CreateNewIssue(potentialIssueData, heroTarget);
                questCreated = true;
            }
            if (heroTarget.Issue == null && !actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    questGen.currentActionIndex++;
                    actioncomplete = true;
                    if (questGen.currentActionIndex < questGen.actionsInOrder.Count)
                    {
                        questGen.currentAction = questGen.actionsInOrder[questGen.currentActionIndex];
                    }
                    else
                    {
                        questGen.SuccessConsequences();
                    }
                }
                else if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                {
                    questGen.currentActionIndex++;
                    actioncomplete = true;
                    if (questGen.currentActionIndex < questGen.actionsInOrder.Count)
                    {
                        questGen.currentAction = questGen.actionsInOrder[questGen.currentActionIndex];
                    }
                    else
                    {
                        questGen.SuccessConsequences();
                    }
                }
            }
            else if (!actioncomplete && !actionInLog)
            {
                if (index == 0)
                {
                    actionInLog = true;
                    if (settlementTarget != null)
                    {
                        TextObject textObject = new TextObject("It appears someone wants to have a word with you. Talk with {HERO} in {SETTLEMENT} and find out what he wants.", null);
                        textObject.SetTextVariable("HERO", heroTarget.Name);
                        textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                        InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("subquest action - line 274"));
                    }
                }
                else
                {
                    if (questGen.actionsInOrder[index - 1].actioncomplete && questGen.currentActionIndex == index)
                    {
                        actionInLog = true;
                        if (settlementTarget != null)
                        {
                            TextObject textObject = new TextObject("It appears someone wants to have a word with you. Talk with {HERO} in {SETTLEMENT} and find out what he wants.", null);
                            textObject.SetTextVariable("HERO", heroTarget.Name);
                            textObject.SetTextVariable("SETTLEMENT", settlementTarget.Name);
                            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, 0, 1, null, false);
                            InformationManager.DisplayMessage(new InformationMessage("Next Task: " + textObject));
                        }
                        else
                        {
                            InformationManager.DisplayMessage(new InformationMessage("subquest action - line 274"));
                        }
                    }
                }
            }


        }

        public override void OnQuestCompletedEventQuest(QuestBase quest, QuestCompleteDetails questCompleteDetails, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (quest.QuestGiver == heroTarget)
            {
                if (!actioncomplete)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], 1);

                    questGen.currentActionIndex++;
                    actioncomplete = true;
                    questGen.chosenMission.run(CustomBTStep.questQ, questBase, questGen);
                    player_data.player_CompletedSubQuests++;
                    string playerData = @"..\..\Modules\ThePlotLords\PlayerData\" + player_data_ID + ".txt";
                    JsonSerialization.WriteToJsonFile<PlayerData>(playerData, player_data);
                    if (questGen.currentActionIndex < questGen.actionsInOrder.Count)
                    {
                        questGen.currentAction = questGen.actionsInOrder[questGen.currentActionIndex];
                    }
                    else
                    {
                        questGen.SuccessConsequences();
                    }
                }
            }

        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
            foreach (Parameter p in this.Action.param)
            {
                if (p.target == targetString)
                {
                    p.target = targetHero.Name.ToString();
                    heroTarget = targetHero;
                    break;
                }
            }
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
        }
    }
}
