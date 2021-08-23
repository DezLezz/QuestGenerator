using System;
using System.Collections.Generic;
using System.IO;
using ThePlotLords.QuestBuilder.CustomBT;

namespace ThePlotLords.QuestBuilder
{
    public class Generator
    {

        public Generator() { }

        public Generator(int i)
        {
            none_p = new Parameter("none", -1);

            capture_p = new Parameter("Enemy", 0);

            damage_p1 = new Parameter("Location", 2, 0);
            damage_p2 = new Parameter("Enemy", 0);

            exchange_p1 = new Parameter("Character", 1);
            exchange_p2 = new Parameter("Item", 2, 0);
            exchange_p3 = new Parameter("Item", 1);

            explore_p = new Parameter("Location", 0);

            free_p = new Parameter("Character", 0);

            gather_p = new Parameter("Item", 0);

            give_p1 = new Parameter("Character", 0);
            give_p2 = new Parameter("Item", 0);

            goto_p = new Parameter("Location", 0);

            kill_p = new Parameter("Enemy", 0);

            listen_p = new Parameter("Character", 1);

            report_p = new Parameter("Character", 0);

            take_p1 = new Parameter("Enemy", 2, 1);
            take_p2 = new Parameter("Item", 0);

            use_p = new Parameter("Item", 1);

            quest_p = new Parameter("Character", 0);

            subquest_p = new Parameter("Character", 1);

            goto_NT_p = new Parameter("Location", 1);

            learn_NT_p = new Parameter("Location", 0);

            prepare_NT_p = new Parameter("none", -1);

            get_NT_p = new Parameter("Item", 1);

            steal_NT_p = new Parameter("Item", 1);

            capture_NT_p = new Parameter("Enemy", 1);

            defeat_NT_p = new Parameter("Enemy", 1);

            report_NT_p = new Parameter("Character", 1);

            give_NT_p1 = new Parameter("Character", 1);
            give_NT_p2 = new Parameter("Item", 2, -2);

            rescue_NT_p = new Parameter("Character", 1);

            none = new Action("", "Terminal", 1, "None", new List<Parameter>() { none_p });
            capture = new Action("capture", "Terminal", 2, "Enemy", new List<Parameter>() { capture_p });
            damage = new Action("damage", "Terminal", 3, "Enemy", new List<Parameter>() { damage_p1, damage_p2 });
            exchange = new Action("exchange", "Terminal", 7, "Item", new List<Parameter>() { exchange_p1, exchange_p2, exchange_p3 });
            explore = new Action("explore", "Terminal", 9, "Location", new List<Parameter>() { explore_p });
            free = new Action("free", "Terminal", 11, "Character", new List<Parameter>() { free_p });
            gather = new Action("gather", "Terminal", 12, "Item", new List<Parameter>() { gather_p });
            give = new Action("give", "Terminal", 13, "Item", new List<Parameter>() { give_p1, give_p2 });
            go_to = new Action("goto", "Terminal", 14, "Location", new List<Parameter>() { goto_p });
            kill = new Action("kill", "Terminal", 15, "Enemy", new List<Parameter>() { kill_p });
            listen = new Action("listen", "Terminal", 16, "Character", new List<Parameter>() { listen_p });
            report = new Action("report", "Terminal", 19, "Character", new List<Parameter>() { report_p });
            take = new Action("take", "Terminal", 22, "Item", new List<Parameter>() { take_p1, take_p2 });
            use = new Action("use", "Terminal", 23, "Item", new List<Parameter>() { use_p });
            quest = new Action("quest", "SubQuest", 25, "None", new List<Parameter>() { quest_p });

            subquest = new Action("<subquest>", "NonTerminal", 26, "None", new List<Parameter>() { subquest_p });
            go_to_NT = new Action("<goto>", "NonTerminal", 27, "None", new List<Parameter>() { goto_NT_p });
            learn_NT = new Action("<learn>", "NonTerminal", 28, "None", new List<Parameter>() { learn_NT_p });
            prepare_NT = new Action("<prepare>", "NonTerminal", 29, "None", new List<Parameter>() { prepare_NT_p });
            get_NT = new Action("<get>", "NonTerminal", 30, "None", new List<Parameter>() { get_NT_p });
            steal_NT = new Action("<steal>", "NonTerminal", 31, "None", new List<Parameter>() { steal_NT_p });
            capture_NT = new Action("<capture>", "NonTerminal", 32, "None", new List<Parameter>() { capture_NT_p });
            defeat_NT = new Action("<defeat>", "NonTerminal", 33, "None", new List<Parameter>() { defeat_NT_p });
            report_NT = new Action("<report>", "NonTerminal", 34, "None", new List<Parameter>() { report_NT_p });
            give_NT = new Action("<give>", "NonTerminal", 35, "None", new List<Parameter>() { give_NT_p1, give_NT_p2 });
            rescue_NT = new Action("<rescue>", "NonTerminal", 36, "None", new List<Parameter>() { rescue_NT_p });

            subquestRule = new Rules("<subquest>", "NonTerminal", 1, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { quest, go_to_NT } }, new List<float>() { 0.3f, 0.7f });
            gotoRule = new Rules("<goto>", "NonTerminal", 2, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { go_to }, new List<Action>() { explore }, new List<Action>() { learn_NT, go_to_NT }, new List<Action>() { prepare_NT, go_to_NT } }, new List<float>() { 0.3f, 0.49f, 0.01f, 0.1f, 0.1f });
            learnRule = new Rules("<learn>", "NonTerminal", 3, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { go_to_NT, subquest, listen } }, new List<float>() { 0.3f, 0.7f });
            prepareRule = new Rules("<prepare>", "NonTerminal", 4, new List<List<Action>>() { new List<Action>() { go_to_NT, subquest } }, new List<float>() { 1.0f });
            getRule = new Rules("<get>", "NonTerminal", 5, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { steal_NT }, new List<Action>() { go_to_NT, gather }, new List<Action>() { get_NT, go_to_NT, exchange }, new List<Action>() { get_NT, subquest } }, new List<float>() { 0.15f, 0.35f, 0.35f, 0.1f, 0.05f });
            stealRule = new Rules("<steal>", "NonTerminal", 6, new List<List<Action>>() { new List<Action>() { go_to_NT, defeat_NT, take } }, new List<float>() { 1.0f });
            captureRule = new Rules("<capture>", "NonTerminal", 7, new List<List<Action>>() { new List<Action>() { go_to_NT, damage, capture }, new List<Action>() { go_to_NT, capture } }, new List<float>() { 0.45f, 0.55f });
            defeatRule = new Rules("<defeat>", "NonTerminal", 8, new List<List<Action>>() { new List<Action>() { go_to_NT, damage }, new List<Action>() { go_to_NT, kill } }, new List<float>() { 0.3f, 0.7f });
            reportRule = new Rules("<report>", "NonTerminal", 9, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { go_to_NT, report } }, new List<float>() { 0.3f, 0.7f });
            giveRule = new Rules("<give>", "NonTerminal", 10, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { go_to_NT, give } }, new List<float>() { 0.1f, 0.9f });
            rescueRule = new Rules("<rescue>", "NonTerminal", 11, new List<List<Action>>() { new List<Action>() { free }, new List<Action>() { defeat_NT, free } }, new List<float>() { 0.3f, 0.7f });

            itemForStudy = new Strategy("Knowledge", "Deliver item for study", 1, new List<Action>() { get_NT, give_NT });
            interviewNPC = new Strategy("Knowledge", "Interview NPC", 3, new List<Action>() { go_to_NT, listen, report_NT });

            obtainLux = new Strategy("Comfort", "Obtain luxuries", 1, new List<Action>() { get_NT, give_NT });
            killPests = new Strategy("Comfort", "Kill pests", 2, new List<Action>() { go_to_NT, defeat_NT, report_NT });

            obtainRareItems = new Strategy("Reputation", "Obtain rare items", 1, new List<Action>() { get_NT, give_NT });
            killEnemies = new Strategy("Reputation", "Kill enemies", 2, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            visitDangerousPlace = new Strategy("Reputation", "Visit dangerous place", 3, new List<Action>() { go_to_NT, report_NT });

            revengeJustice = new Strategy("Serenity", "Revenge, Justice", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            captureCriminal = new Strategy("Serenity", "Capture Criminal", 2, new List<Action>() { go_to_NT, capture_NT, report_NT });
            checkOnNPC1 = new Strategy("Serenity", "Check on NPC", 3, new List<Action>() { go_to_NT, listen, report_NT });
            recoverLostStolenItem = new Strategy("Serenity", "Recover lost/stolen item", 5, new List<Action>() { get_NT, give_NT });
            rescueNPC = new Strategy("Serenity", "Rescue NPC", 6, new List<Action>() { go_to_NT, rescue_NT, report_NT });

            attackThreateningEntities = new Strategy("Protection", "Attack threatening entities", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            captureCriminal2 = new Strategy("Protection", "Capture Criminal", 2, new List<Action>() { go_to_NT, capture_NT, report_NT });
            createDiversion2 = new Strategy("Protection", "Create Diversion", 6, new List<Action>() { go_to_NT, damage, report_NT });
            recruit = new Strategy("Protection", "Recruit", 9, new List<Action>() { go_to_NT, listen, report_NT });

            attackEnemy = new Strategy("Conquest", "Attack enemy", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            stealStuff = new Strategy("Conquest", "Steal stuff", 2, new List<Action>() { go_to_NT, steal_NT, give_NT });
            recruit2 = new Strategy("Conquest", "Recruit", 3, new List<Action>() { go_to_NT, listen, report_NT });

            gatherRawMaterials = new Strategy("Wealth", "Gather raw materials", 1, new List<Action>() { go_to_NT, get_NT, report_NT });
            stealValuablesforResale = new Strategy("Wealth", "Steal valuables for resale", 2, new List<Action>() { go_to_NT, steal_NT, give_NT });

            practiceCombat = new Strategy("Ability", "Practice combat", 4, new List<Action>() { go_to_NT, damage });
            praticeSkill = new Strategy("Ability", "Practice skill", 5, new List<Action>() { go_to_NT, use });

            deliverSupplies = new Strategy("Equipment", "Deliver supplies", 1, new List<Action>() { get_NT, give_NT });
            stealSupplies = new Strategy("Equipment", "Steal supplies", 1, new List<Action>() { steal_NT, give_NT });
            tradeforSupplies = new Strategy("Equipment", "Trade for supplies", 1, new List<Action>() { get_NT, go_to_NT, exchange });

            knowledge = new Motivation("Knowledge", new List<Strategy>() { itemForStudy, interviewNPC });
            comfort = new Motivation("Comfort", new List<Strategy>() { obtainLux, killPests });
            reputation = new Motivation("Reputation", new List<Strategy>() { obtainRareItems, killEnemies, visitDangerousPlace });
            serenity = new Motivation("Serenity", new List<Strategy>() { revengeJustice, captureCriminal, checkOnNPC1, recoverLostStolenItem, rescueNPC });
            protection = new Motivation("Protection", new List<Strategy>() { attackThreateningEntities, captureCriminal2, createDiversion2, recruit });
            conquest = new Motivation("Conquest", new List<Strategy>() { attackEnemy, stealStuff });
            wealth = new Motivation("Wealth", new List<Strategy>() { gatherRawMaterials, stealValuablesforResale });
            ability = new Motivation("Ability", new List<Strategy>() { practiceCombat, praticeSkill });
            equipment = new Motivation("Equipment", new List<Strategy>() { deliverSupplies, stealSupplies, tradeforSupplies });
        }

        public Parameter none_p;

        public Parameter capture_p;

        public Parameter damage_p1;
        public Parameter damage_p2;

        public Parameter exchange_p1;
        public Parameter exchange_p2;
        public Parameter exchange_p3;

        public Parameter explore_p;

        public Parameter free_p;

        public Parameter gather_p;

        public Parameter give_p1;
        public Parameter give_p2;

        public Parameter goto_p;

        public Parameter kill_p;

        public Parameter listen_p;

        public Parameter report_p;

        public Parameter take_p1;
        public Parameter take_p2;

        public Parameter use_p;

        public Parameter quest_p;

        public Parameter subquest_p;

        public Parameter goto_NT_p;

        public Parameter learn_NT_p;

        public Parameter prepare_NT_p;

        public Parameter get_NT_p;

        public Parameter steal_NT_p;

        public Parameter capture_NT_p;

        public Parameter defeat_NT_p;

        public Parameter report_NT_p;

        public Parameter give_NT_p1;
        public Parameter give_NT_p2;

        public Parameter rescue_NT_p;

        public Action none;
        public Action capture;
        public Action damage;
        public Action exchange;
        public Action explore;
        public Action free;
        public Action gather;
        public Action give;
        public Action go_to;
        public Action kill;
        public Action listen;
        public Action report;
        public Action take;
        public Action use;
        public Action quest;

        public Action subquest;
        public Action go_to_NT;
        public Action learn_NT;
        public Action prepare_NT;
        public Action get_NT;
        public Action steal_NT;
        public Action capture_NT;
        public Action defeat_NT;
        public Action report_NT;
        public Action give_NT;
        public Action rescue_NT;

        public Rules subquestRule;
        public Rules gotoRule;
        public Rules learnRule;
        public Rules prepareRule;
        public Rules getRule;
        public Rules stealRule;
        public Rules captureRule;
        public Rules defeatRule;
        public Rules reportRule;
        public Rules giveRule;
        public Rules rescueRule;

        //Knowledge
        public Strategy itemForStudy;
        public Strategy interviewNPC;
        //Comfort                               
        public Strategy obtainLux;
        public Strategy killPests;
        //Reputation                               
        public Strategy obtainRareItems;
        public Strategy killEnemies;
        public Strategy visitDangerousPlace;
        //Serenity                               
        public Strategy revengeJustice;
        public Strategy captureCriminal;
        public Strategy checkOnNPC1;
        public Strategy recoverLostStolenItem;
        public Strategy rescueNPC;
        //Protection
        public Strategy attackThreateningEntities;
        public Strategy captureCriminal2;
        public Strategy createDiversion2;
        public Strategy recruit;
        //Conquest                                 
        public Strategy attackEnemy;
        public Strategy stealStuff;
        public Strategy recruit2;
        //Wealth                                 
        public Strategy gatherRawMaterials;
        public Strategy stealValuablesforResale;
        //Ability                             
        public Strategy practiceCombat;
        public Strategy praticeSkill;
        //Equipment                                  
        public Strategy deliverSupplies;
        public Strategy stealSupplies;
        public Strategy tradeforSupplies;

        public Motivation knowledge;
        public Motivation comfort;
        public Motivation reputation;
        public Motivation serenity;
        public Motivation protection;
        public Motivation conquest;
        public Motivation wealth;
        public Motivation ability;
        public Motivation equipment;

        public List<String> chars = new List<string>() { "npc1", "npc2", "npc3", "npc4", "npc5", "npc6", "npc7", "npc8", "npc9", "npc10", "npc11", "npc12", "npc13", "npc14", "npc15", "npc16", "npc17", "npc18", "npc19", "npc20" };
        public List<String> items = new List<string>() { "item1", "item2", "item3", "item4", "item5", "item6", "item7", "item8", "item9", "item10", "item11", "item12", "item13", "item14", "item15", "item16", "item17", "item18", "item19", "item20" };
        public List<String> enemies = new List<string>() { "enemy1", "enemy2", "enemy3", "enemy4", "enemy5", "enemy6", "enemy7", "enemy8", "enemy9", "enemy10", "enemy11", "enemy12", "enemy13", "enemy14", "enemy15", "enemy16", "enemy17", "enemy18", "enemy19", "enemy20" };
        public List<String> locations = new List<string>() { "place1", "place2", "place3", "place4", "place5", "place6", "place7", "place8", "place9", "place10", "place11", "place12", "place13", "place14", "place15", "place16", "place17", "place18", "place19", "place20" };

        public void GenerateOne()
        {
            QuestGiver testGiver = new QuestGiver("npc1", new List<Motivation>() { knowledge, comfort, reputation, serenity, protection, conquest, wealth, ability, equipment }, chars, items, enemies, locations);
            Quest testQuest = testGiver.CreateQuest(1, new List<Strategy>() { itemForStudy, interviewNPC, obtainLux, killPests, obtainRareItems, killEnemies, visitDangerousPlace, revengeJustice, captureCriminal, checkOnNPC1, recoverLostStolenItem, rescueNPC, attackThreateningEntities, captureCriminal2, createDiversion2, recruit, attackEnemy, stealStuff, recruit2, gatherRawMaterials, stealValuablesforResale, practiceCombat, praticeSkill, deliverSupplies, stealSupplies, tradeforSupplies }, new List<Rules>() { subquestRule, gotoRule, learnRule, prepareRule, getRule, stealRule, captureRule, defeatRule, reportRule, giveRule, rescueRule });

            string path = @"..\..\Modules\ThePlotLords\MissionList.xml";

            if (new FileInfo(path).Length == 0)
            {
                List<CustomBTNode> tempList = new List<CustomBTNode>();
                tempList.Add(testGiver.customBTTree);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, tempList);
            }
            else
            {
                var tempList = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
                tempList.Add(testGiver.customBTTree);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, tempList);
            }

            subquestRule.weights = new List<float>() { 0.3f, 0.7f };
            gotoRule.weights = new List<float>() { 0.2f, 0.34f, 0.01f, 0.2f, 0.25f };
            learnRule.weights = new List<float>() { 0.3f, 0.7f };
            prepareRule.weights = new List<float>() { 1.0f };
            getRule.weights = new List<float>() { 0.1f, 0.35f, 0.35f, 0.1f, 0.1f };
            stealRule.weights = new List<float>() { 1.0f };
            captureRule.weights = new List<float>() { 0.45f, 0.55f };
            defeatRule.weights = new List<float>() { 0.3f, 0.7f };
            reportRule.weights = new List<float>() { 0.3f, 0.7f };
            giveRule.weights = new List<float>() { 0.1f, 0.9f };
            rescueRule.weights = new List<float>() { 0.3f, 0.7f };
        }

        public void GenerateOneDebug()
        {
            QuestGiver testGiver = new QuestGiver("npc1", new List<Motivation>() { knowledge, comfort, reputation, serenity, protection, conquest, wealth, ability, equipment }, chars, items, enemies, locations);
            Quest testQuest = testGiver.CreateQuest(1, new List<Strategy>() { itemForStudy, interviewNPC, obtainLux, killPests, obtainRareItems, killEnemies, visitDangerousPlace, revengeJustice, captureCriminal, checkOnNPC1, recoverLostStolenItem, rescueNPC, attackThreateningEntities, captureCriminal2, createDiversion2, recruit, attackEnemy, stealStuff, recruit2, gatherRawMaterials, stealValuablesforResale, practiceCombat, praticeSkill, deliverSupplies, stealSupplies, tradeforSupplies }, new List<Rules>() { subquestRule, gotoRule, learnRule, prepareRule, getRule, stealRule, captureRule, defeatRule, reportRule, giveRule, rescueRule });

            string path = @"..\..\Modules\ThePlotLords\MissionList.xml";


            List<CustomBTNode> tempList = new List<CustomBTNode>();
            tempList.Add(testGiver.customBTTree);
            XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, tempList);


            subquestRule.weights = new List<float>() { 0.3f, 0.7f };
            gotoRule.weights = new List<float>() { 0.2f, 0.34f, 0.01f, 0.2f, 0.25f };
            learnRule.weights = new List<float>() { 0.3f, 0.7f };
            prepareRule.weights = new List<float>() { 1.0f };
            getRule.weights = new List<float>() { 0.1f, 0.35f, 0.35f, 0.1f, 0.1f };
            stealRule.weights = new List<float>() { 1.0f };
            captureRule.weights = new List<float>() { 0.45f, 0.55f };
            defeatRule.weights = new List<float>() { 0.3f, 0.7f };
            reportRule.weights = new List<float>() { 0.3f, 0.7f };
            giveRule.weights = new List<float>() { 0.1f, 0.9f };
            rescueRule.weights = new List<float>() { 0.3f, 0.7f };
        }

        public void GenerateOneHundred()
        {
            string path = @"..\..\Modules\ThePlotLords\MissionList.xml";
            List<CustomBTNode> tempList = new List<CustomBTNode>();

            for (int i = 0; i < 100; i++)
            {
                QuestGiver testGiver = new QuestGiver("npc1", new List<Motivation>() { knowledge, comfort, reputation, serenity, protection, conquest, wealth, ability, equipment }, chars, items, enemies, locations);
                Quest testQuest = testGiver.CreateQuest(1, new List<Strategy>() { itemForStudy, interviewNPC, obtainLux, killPests, obtainRareItems, killEnemies, visitDangerousPlace, revengeJustice, captureCriminal, checkOnNPC1, recoverLostStolenItem, rescueNPC, attackThreateningEntities, captureCriminal2, createDiversion2, recruit, attackEnemy, stealStuff, recruit2, gatherRawMaterials, stealValuablesforResale, practiceCombat, praticeSkill, deliverSupplies, stealSupplies, tradeforSupplies }, new List<Rules>() { subquestRule, gotoRule, learnRule, prepareRule, getRule, stealRule, captureRule, defeatRule, reportRule, giveRule, rescueRule });
                if (testQuest.steps.Count > 0)
                {
                    tempList.Add(testGiver.customBTTree);
                }
                else
                {
                    i--;
                }

                subquestRule.weights = new List<float>() { 0.3f, 0.7f };
                gotoRule.weights = new List<float>() { 0.2f, 0.34f, 0.01f, 0.2f, 0.25f };
                learnRule.weights = new List<float>() { 0.3f, 0.7f };
                prepareRule.weights = new List<float>() { 1.0f };
                getRule.weights = new List<float>() { 0.1f, 0.35f, 0.35f, 0.1f, 0.1f };
                stealRule.weights = new List<float>() { 1.0f };
                captureRule.weights = new List<float>() { 0.45f, 0.55f };
                defeatRule.weights = new List<float>() { 0.3f, 0.7f };
                reportRule.weights = new List<float>() { 0.3f, 0.7f };
                giveRule.weights = new List<float>() { 0.1f, 0.9f };
                rescueRule.weights = new List<float>() { 0.3f, 0.7f };

            }

            XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, tempList);
        }


    }
}
