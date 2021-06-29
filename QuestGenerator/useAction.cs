using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using System.Xml.Serialization;
using static QuestGenerator.QuestGenTestCampaignBehavior;
using QuestGenerator.QuestBuilder;
using TaleWorlds.Library;

namespace QuestGenerator
{
    public class useAction : actionTarget
    {
        static Random rnd = new Random();

        public string skillName;

        public int levelAmount = 0;

        public int currentLevel = 0;
        public useAction(string action, QuestGenerator.QuestBuilder.Action action1) : base(action, action1)
        {
        }
        public useAction() { }

        public override void bringTargetsBack()
        {
            if (this.questGiver == null)
            {
                var setName = this.questGiverString;
                
                Hero[] array = (from x in Hero.AllAliveHeroes where (x.Name.ToString() == setName) select x).ToArray<Hero>();

                if (array.Length > 1 || array.Length == 0)
                {
                    InformationManager.DisplayMessage(new InformationMessage("use action - line 40"));
                }
                if (array.Length == 1)
                {
                    this.questGiver = array[0];
                }
            }
        }

        public override void IssueQ(IssueBase questBase, QuestGenTestCampaignBehavior.QuestGenTestIssue questGen, bool alternative)
        {
            if (this.Action.param[0].target.Contains("item"))
            {
                MBReadOnlyList<SkillObject> skills = Skills.All;
                int r = rnd.Next(skills.Count);

                this.skillName = skills[r].Name.ToString();
                //this.skillName = "Athletics";
                
                this.Action.param[0].target = this.skillName;
            }
        }

        public override void QuestQ(QuestBase questBase, QuestGenTestCampaignBehavior.QuestGenTestQuest questGen)
        {
            foreach (SkillObject s in Skills.All)
            {
                if (s.Name.ToString() == this.skillName)
                {
                    this.currentLevel = Hero.MainHero.GetSkillValue(s);
                    break;
                }
            }
            //int l = rnd.Next(5, 20);
            this.levelAmount = 5000 + this.currentLevel;
            TextObject textObject = new TextObject("Level up your {SKILL} by at least {AMOUNT} levels.", null);
            textObject.SetTextVariable("SKILL", this.skillName);
            textObject.SetTextVariable("AMOUNT", this.levelAmount - this.currentLevel);
            questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, this.currentLevel, this.levelAmount, null, false);
        }

        private void useConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!questGen.journalLogs[this.index].HasBeenCompleted())
            {
                questGen.currentActionIndex++;
                foreach (SkillObject s in Skills.All)
                {
                    if (s.Name.ToString() == this.skillName)
                    {
                        this.currentLevel = Hero.MainHero.GetSkillValue(s);
                        break;
                    }
                }
                questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], this.currentLevel);

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

        public override void HeroGainedSkill(Hero hero, SkillObject skill, bool hasNewPerk, int change, bool shouldNotify, int index, QuestGenTestQuest questGen, QuestBase questBase)
        {
            if (hero == Hero.MainHero && skill.Name.ToString() == this.skillName)
            {
                if (this.currentLevel + change < levelAmount)
                {
                    questGen.UpdateQuestTaskS(questGen.journalLogs[this.index], this.currentLevel + change);
                    this.currentLevel += change;
                }
                else
                {
                    this.useConsequences(index, questBase, questGen);
                }

            }
        }

        public override void updateHeroTargets(string targetString, Hero targetHero)
        {
        }

        public override void updateItemTargets(string targetString, ItemObject targetItem)
        {
            
        }

        public override void updateSettlementTargets(string targetString, Settlement targetSettlement)
        {
        }
    }
}
