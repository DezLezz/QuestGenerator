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
using QuestGenerator.QuestBuilder.CustomBT;

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

                this.questGiver = Hero.FindFirst((Hero x) => x.Name.ToString() == setName);
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
            if (!actioncomplete)
            {
                if (this.index == 0)
                {
                    foreach (SkillObject s in Skills.All)
                    {
                        if (s.Name.ToString() == this.skillName)
                        {
                            this.currentLevel = Hero.MainHero.GetSkillValue(s);
                            break;
                        }
                    }
                    int l = rnd.Next(5, 20);
                    this.levelAmount = l + this.currentLevel;
                    TextObject textObject = new TextObject("Level up your {SKILL} by at least {AMOUNT} levels.", null);
                    textObject.SetTextVariable("SKILL", this.skillName);
                    textObject.SetTextVariable("AMOUNT", this.levelAmount - this.currentLevel);
                    questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, this.currentLevel, this.levelAmount, null, false);
                    this.actionInLog = true;

                }
                else
                {
                    if (questGen.actionsInOrder[this.index - 1].actioncomplete)
                    {
                        foreach (SkillObject s in Skills.All)
                        {
                            if (s.Name.ToString() == this.skillName)
                            {
                                this.currentLevel = Hero.MainHero.GetSkillValue(s);
                                break;
                            }
                        }
                        int l = rnd.Next(5, 20);
                        this.levelAmount = l + this.currentLevel;
                        TextObject textObject = new TextObject("Level up your {SKILL} by at least {AMOUNT} levels.", null);
                        textObject.SetTextVariable("SKILL", this.skillName);
                        textObject.SetTextVariable("AMOUNT", this.levelAmount - this.currentLevel);
                        questGen.journalLogs[index] = questGen.getDiscreteLog(textObject, textObject, this.currentLevel, this.levelAmount, null, false);
                        this.actionInLog = true;
                    }
                }
            }
            
            
        }

        private void useConsequences(int index, QuestBase questBase, QuestGenTestQuest questGen)
        {
            if (!this.actioncomplete)
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
                this.actioncomplete = true;
                questGen.chosenMission.run(CustomBTStep.questQ,questBase, questGen);
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

        public override TextObject getDescription(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Practice skill":
                    strat = new TextObject("I advise you to train {SKILL} for the future.", null);
                    strat.SetTextVariable("SKILL", this.skillName);
                    break;
            }
            return strat;
        }

        public override TextObject getTitle(string strategy)
        {
            TextObject strat = new TextObject("empty", null);
            switch (strategy)
            {
                case "Practice skill":
                    strat = new TextObject("Practice {SKILL}.", null);
                    strat.SetTextVariable("SKILL", this.skillName);
                    break;
            }
            return strat;
        }

        public override string getListenString(string strategy)
        {
            Dictionary<string, string> skills = new Dictionary<string, string>() { 
                {"One Handed","fighting with a one-handed short weapon. It will improve your one-handed weapon attack speed and damage." }, {"Two Handed","fighting with a two-handed sword weapon. It will improve your two-handed weapon attack speed and damage." }, {"Polearm","fighting with spears or other polearm-type weapons. It will improve your polearm weapon attack speed and damage." },
                {"Bow","shooting with a bow and arrow and performing long-distance shots. It will improve your bow damage, accuracy, and usable bow types." },{"Crossbow","shooting enemies with crossbow. It will improve your crossbow reload speed and accuracy." },{"Throwing","hitting enemies with thrown weapons. It will improve your thrown weapon speed, damage, and accuracy." },
                {"Riding ","exploring map with as much speed as possible and fighting on horseback. It will improve your mount speed, maneuverability, and usable mount types." },{"Athletics ","fighting and moving around the map while on foot. It will improve your running speed." },{"Smithing ","using the smithy to create weapons, refine materials, and smelt old equipment. It will improve your capability to smith more difficult weapons." },
                {"Scouting ","spoting tracks and hideouts and travel on difficult terrain. It will improve your tracking detection, information level, and spotting distance." },{"Tactics","commanding simulated battles, winning against difficult odds, or escaping encounters by sacrificing troops if necessary. It will improve your simulation advantages and sacrificed troop counts when escaping." },{"Roguery","ransoming prisoners, raiding caravans, leading bandit troops, infiltrating enemy towns, bribery, and escaping from captivity. It will improve your post-battle loot gains." },
                {"Charm","improving relations with other people, releasing captured nobles or socializing with them, and bartering. It will improve your relationships with people." },{"Leadership","maintaining high morale in your army and when you assemble and lead armies. It will improve your the morale of parties under your command and garrison size." },{"Trade","making a profit from trading and when operating caravans.It will reduce your trade penalty." },
                {"Steward","gaining party morale from food variety, improving settlement prosperity and building projects, and spending time in your settlements. It will boost your party’s size." },{"Medicine","helping soldiers heal in settlements. It will improve your casualty survival and healing rate." },{"Engineering","building and successfully operating siege engines. It will improve your production of siege engines and buildings and types of siege engines that can be used." }
            };
            TextObject strat = new TextObject("empty", null);
            
            switch (strategy)
            {
                case "Practice skill":
                    if (!skills.ContainsKey(this.skillName))
                    {
                        strat = new TextObject("There has been an erro with {SKILL} skill.", null);
                    }
                    else
                    {
                        strat = new TextObject("To improve your {SKILL} skill you should try " + skills[this.skillName], null);
                    }
                    
                    strat.SetTextVariable("SKILL", this.skillName);
                    break;
            }
            return strat.ToString();
        }
    }
}
