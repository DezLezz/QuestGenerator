using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuestGenerator.QuestBuilder.CustomBT;
using System.Xml.Serialization;

namespace QuestGenerator.QuestBuilder
{
    public class Generator
    {

        public Generator() { }

        public Generator(int i)
        {
            none_p = new Parameter("none", -1);

            capture_p = new Parameter("Enemy", 0);

            damage_p1 = new Parameter("Location", 2, 0);
            damage_p2 = new Parameter("Enemy", 1);

            defend_p1 = new Parameter("Location", 2, 0);
            defend_p2 = new Parameter("Character", 1);

            escort_p1 = new Parameter("Character", 0);
            escort_p2 = new Parameter("Location", 1);

            examine_p1 = new Parameter("Location", 2, 0);
            examine_p2 = new Parameter("Character", 1);

            exchange_p1 = new Parameter("Character", 1);
            exchange_p2 = new Parameter("Item", 2, 0);
            exchange_p3 = new Parameter("Item", 1);

            experiment_p = new Parameter("Item", 2, 0);

            explore_p = new Parameter("Location", 0);

            free_p = new Parameter("Character", 0);

            gather_p = new Parameter("Item", 0);

            give_p1 = new Parameter("Character", 0);
            give_p2 = new Parameter("Item", 0);

            goto_p = new Parameter("Location", 0);

            kill_p = new Parameter("Enemy", 1);

            listen_p = new Parameter("Character", 1);

            read_p = new Parameter("Item", 2, 1);

            repair_p = new Parameter("Item", 1);

            report_p = new Parameter("Character", 0);

            spy_p1 = new Parameter("Enemy", 1);
            spy_p2 = new Parameter("Item", 1);

            take_p1 = new Parameter("Character", 1);
            take_p2 = new Parameter("Item", 1);

            use_p = new Parameter("Item", 1);

            quest_p = new Parameter("Character", 0);

            subquest_p = new Parameter("Character", 1);

            goto_NT_p = new Parameter("Location", 1);

            learn_NT_p = new Parameter("Location", 0);

            prepare_NT_p = new Parameter("none", -1);

            get_NT_p = new Parameter("Item", 1);

            steal_NT_p = new Parameter("Item", 1);

            capture_NT_p = new Parameter("Enemy", 1);

            defeat_NT_p = new Parameter("none", -1);

            report_NT_p = new Parameter("Character", 1);

            give_NT_p1 = new Parameter("Character", 1);
            give_NT_p2 = new Parameter("Item", 1);

            rescue_NT_p = new Parameter("Character", 1);

            none = new Action("", "Terminal", 1, "None", new List<Parameter>() { none_p });
            //capture = new Action("capture", "Terminal", 2, "Enemy", new List<Parameter>() { capture_p });
            //damage = new Action("damage", "Terminal", 3, "Enemy", new List<Parameter>() { damage_p1, damage_p2 });
            //defend = new Action("defend", "Terminal", 4, "Character", new List<Parameter>() { defend_p1, defend_p2 });
            //escort = new Action("escort", "Terminal", 5, "Character", new List<Parameter>() { escort_p1, escort_p2 });
            //examine = new Action("examine", "Terminal", 6, "Item", new List<Parameter>() { examine_p1, examine_p2 });
            //exchange = new Action("exchange", "Terminal", 7, "Item", new List<Parameter>() { exchange_p1, exchange_p2, exchange_p3 });
            //experiment = new Action("experiment", "Terminal", 9, "Item", new List<Parameter>() { experiment_p });
            //explore = new Action("explore", "Terminal", 9, "Location", new List<Parameter>() { explore_p });
            //free = new Action("free", "Terminal", 11, "Character", new List<Parameter>() { free_p });
            //gather = new Action("gather", "Terminal", 12, "Item", new List<Parameter>() { gather_p });
            //give = new Action("give", "Terminal", 13, "Item", new List<Parameter>() { give_p1, give_p2 });
            go_to = new Action("goto", "Terminal", 14, "Location", new List<Parameter>() { goto_p });
            //kill = new Action("kill", "Terminal", 15, "Enemy", new List<Parameter>() { kill_p });
            listen = new Action("listen", "Terminal", 16, "Character", new List<Parameter>() { listen_p });
            //read = new Action("read", "Terminal", 17, "Item", new List<Parameter>() { read_p });
            //repair = new Action("repair", "Terminal", 18, "Item", new List<Parameter>() { repair_p });
            //report = new Action("report", "Terminal", 19, "Character", new List<Parameter>() { report_p });
            //spy = new Action("spy", "Terminal", 20, "Enemy", new List<Parameter>() { spy_p1, spy_p2 });
            //take = new Action("take", "Terminal", 22, "Item", new List<Parameter>() { take_p1, take_p2 });
            //use = new Action("use", "Terminal", 23, "Item", new List<Parameter>() { use_p });
            quest = new Action("quest", "SubQuest", 25, "None", new List<Parameter>() { quest_p });

            subquest = new Action("<subquest>", "NonTerminal", 26, "None", new List<Parameter>() { subquest_p });
            go_to_NT = new Action("<goto>", "NonTerminal", 27, "None", new List<Parameter>() { goto_NT_p });
            learn_NT = new Action("<learn>", "NonTerminal", 28, "None", new List<Parameter>() { learn_NT_p });
            //prepare_NT = new Action("<prepare>", "NonTerminal", 29, "None", new List<Parameter>() { prepare_NT_p });
            //get_NT = new Action("<get>", "NonTerminal", 30, "None", new List<Parameter>() { get_NT_p });
            //steal_NT = new Action("<steal>", "NonTerminal", 31, "None", new List<Parameter>() { steal_NT_p });
            //capture_NT = new Action("<capture>", "NonTerminal", 32, "None", new List<Parameter>() { capture_NT_p });
            //defeat_NT = new Action("<defeat>", "NonTerminal", 33, "None", new List<Parameter>() { defeat_NT_p });
            //report_NT = new Action("<report>", "NonTerminal", 34, "None", new List<Parameter>() { report_NT_p });
            //give_NT = new Action("<give>", "NonTerminal", 35, "None", new List<Parameter>() { give_NT_p1, give_NT_p2 });
            //rescue_NT = new Action("<rescue>", "NonTerminal", 36, "None", new List<Parameter>() { rescue_NT_p });

            subquestRule = new Rules("<subquest>", "NonTerminal", 1, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { quest/*, go_to_NT */} }, new List<float>() { 0.3f, 0.7f });
            gotoRule = new Rules("<goto>", "NonTerminal", 2, new List<List<Action>>() { /*new List<Action>() { none }, new List<Action>() { go_to }, new List<Action>() { explore },*/ new List<Action>() { learn_NT/*, go_to_NT*/ }/*, new List<Action>() { prepare_NT, go_to_NT } */}, new List<float>() {/* 0.0f,0.15f, 0.1f,*/ 0.3f/*, 0.35f */});
            learnRule = new Rules("<learn>", "NonTerminal", 3, new List<List<Action>>() {/* new List<Action>() { none },*/ new List<Action>() {/* go_to_NT,*/ subquest, listen }/*, new List<Action>() { go_to_NT, get_NT, read }, new List<Action>() { go_to_NT, subquest, examine } */}, new List<float>() {/* 0.0f,*/ 0.4f/*, 0.3f, 0.3f*/ });
            //prepareRule = new Rules("<prepare>", "NonTerminal", 4, new List<List<Action>>() { new List<Action>() { go_to_NT, subquest } }, new List<float>() { 1.0f });
            //getRule = new Rules("<get>", "NonTerminal", 5, new List<List<Action>>() { new List<Action>() { none }, new List<Action>() { steal_NT }, new List<Action>() { go_to_NT, gather }, new List<Action>() { go_to_NT, take }, new List<Action>() { get_NT, go_to_NT, exchange }, new List<Action>() { get_NT, subquest } }, new List<float>() { 0.0f, 0.2f, 0.35f, 0.35f, 0.05f, 0.05f });
            //stealRule = new Rules("<steal>", "NonTerminal", 6, new List<List<Action>>() { new List<Action>() { go_to_NT, defeat_NT, take } }, new List<float>() { 1.0f });
            //captureRule = new Rules("<capture>", "NonTerminal", 7, new List<List<Action>>() { new List<Action>() { go_to_NT, use, capture }, new List<Action>() { go_to_NT, damage, capture }, new List<Action>() { go_to_NT, capture } }, new List<float>() { 0.3f, 0.3f, 0.4f });
            //defeatRule = new Rules("<defeat>", "NonTerminal", 8, new List<List<Action>>() { new List<Action>() { go_to_NT, damage }, new List<Action>() { go_to_NT, kill } }, new List<float>() { 0.3f, 0.7f });
            //reportRule = new Rules("<report>", "NonTerminal", 9, new List<List<Action>>() { /*new List<Action>() { none }, */new List<Action>() {/* go_to_NT, */report } }, new List<float>() {/* 0.1f, */0.9f });
            //giveRule = new Rules("<give>", "NonTerminal", 10, new List<List<Action>>() { /*new List<Action>() { none }, */new List<Action>() { /*go_to_NT,*/ give } }, new List<float>() {/* 0.1f, */0.9f });
            //rescueRule = new Rules("<rescue>", "NonTerminal", 11, new List<List<Action>>() { new List<Action>() { free }, new List<Action>() { defeat_NT, free }, new List<Action>() { escort }, new List<Action>() { defeat_NT, escort } }, new List<float>() { 0.15f, 0.4f, 0.15f, 0.3f });

            //itemForStudy = new Strategy("Knowledge", "Deliver item for study", 1, new List<Action>() { get_NT, give_NT });
            //spy_Strat = new Strategy("Knowledge", "Spy", 2, new List<Action>() { go_to_NT, spy, report_NT });
            //interviewNPC = new Strategy("Knowledge", "Interview NPC", 3, new List<Action>() { go_to_NT, listen, report_NT });
            //useItemOnField = new Strategy("Knowledge", "Use item on field", 4, new List<Action>() { get_NT, go_to_NT, use, give_NT });

            //obtainLux = new Strategy("Comfort", "Obtain luxuries", 1, new List<Action>() { get_NT, give_NT });
            //killPests = new Strategy("Comfort", "Kill pests", 2, new List<Action>() { go_to_NT, defeat_NT, report_NT });

            //obtainRareItems = new Strategy("Reputation", "Obtain rare items", 1, new List<Action>() { get_NT, give_NT });
            //killEnemies = new Strategy("Reputation", "Kill enemies", 2, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            //visitDangerousPlace = new Strategy("Reputation", "Visit dangerous place", 3, new List<Action>() { go_to_NT, report_NT });

            //revengeJustice = new Strategy("Serenity", "Revenge, Justice", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            //captureCriminal = new Strategy("Serenity", "Capture Criminal", 2, new List<Action>() { go_to_NT, capture_NT, report_NT });
            //checkOnNPC1 = new Strategy("Serenity", "Check on NPC (1)", 3, new List<Action>() { go_to_NT, listen, report_NT });
            //checkOnNPC2 = new Strategy("Serenity", "Check on NPC (2)", 4, new List<Action>() { go_to_NT, take, report_NT });
            //recoverLostStolenItem = new Strategy("Serenity", "Recover lost/stolen item", 5, new List<Action>() { get_NT, give_NT });
            //rescueNPC = new Strategy("Serenity", "Rescue NPC", 6, new List<Action>() { go_to_NT, rescue_NT, report_NT });

            //attackThreateningEntities = new Strategy("Protection", "Attack threatening entities", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            //captureCriminal2 = new Strategy("Protection", "Capture Criminal", 2, new List<Action>() { go_to_NT, capture_NT, report_NT });
            //treatOrRepair1 = new Strategy("Protection", "Treat or Repair (1)", 3, new List<Action>() { get_NT, go_to_NT, use, report_NT });
            //treatOrRepair2 = new Strategy("Protection", "Treat or Repair (2)", 4, new List<Action>() { go_to_NT, repair, report_NT });
            //createDiversion1 = new Strategy("Protection", "Create Diversion (1)", 5, new List<Action>() { get_NT, go_to_NT, use, report_NT });
            //createDiversion2 = new Strategy("Protection", "Create Diversion (2)", 6, new List<Action>() { go_to_NT, damage, report_NT });
            //assembleFortification = new Strategy("Protection", "Assemble fortification", 7, new List<Action>() { go_to_NT, repair, report_NT });
            //guardEntity = new Strategy("Protection", "Guard entity", 8, new List<Action>() { go_to_NT, defend, report_NT });
            //recruit = new Strategy("Protection", "Recruit", 9, new List<Action>() { /*go_to_NT,*/ listen/*, report_NT*/ });

            //attackEnemy = new Strategy("Conquest", "Attack enemy", 1, new List<Action>() { go_to_NT, defeat_NT, report_NT });
            //stealStuff = new Strategy("Conquest", "Steal stuff", 2, new List<Action>() { go_to_NT, steal_NT, give_NT });
            recruit2 = new Strategy("Conquest", "Recruit", 3, new List<Action>() { go_to_NT/*, listen, report_NT*/ });

            //gatherRawMaterials = new Strategy("Wealth", "Gather raw materials", 1, new List<Action>() { go_to_NT, get_NT, report_NT });
            //stealValuablesforResale = new Strategy("Wealth", "Steal valuables for resale", 2, new List<Action>() { go_to_NT, steal_NT, give_NT });
            //makeValuablesforResale = new Strategy("Wealth", "Make valuables for resale", 3, new List<Action>() { go_to_NT, repair, give_NT });

            //assembleTollforNewSkill = new Strategy("Ability", "Assemble tool for new skill", 1, new List<Action>() { go_to_NT, repair, use });
            //obtainTrainingMaterials = new Strategy("Ability", "Obtain training materials", 2, new List<Action>() { get_NT, use });
            //useExistingTools = new Strategy("Ability", "Use existing tools", 3, new List<Action>() { go_to_NT, use });
            //practiceCombat = new Strategy("Ability", "Practive combat", 4, new List<Action>() { go_to_NT, damage });
            //praticeSkill = new Strategy("Ability", "Pratice skill", 5, new List<Action>() { go_to_NT, use });
            //researchSkill1 = new Strategy("Ability", "Research skill (1)", 6, new List<Action>() { get_NT, use, report_NT });
            //researchSkill2 = new Strategy("Ability", "Research skill (2)", 7, new List<Action>() { get_NT, experiment, report_NT });

            //assemble = new Strategy("Equipment", "Assemble", 1, new List<Action>() {/* go_to_NT, repair,*/ give_NT });
            //deliverSupplies = new Strategy("Equipment", "Deliver supplies", 1, new List<Action>() { get_NT, give_NT });
            //stealSupplies = new Strategy("Equipment", "Steal supplies", 1, new List<Action>() { steal_NT, give_NT });
            //tradeforSupplies = new Strategy("Equipment", "Trade for supplies", 1, new List<Action>() { get_NT, go_to_NT, exchange });

            //knowledge = new Motivation("Knowledge", new List<Strategy>() { itemForStudy, spy_Strat, interviewNPC, useItemOnField });
            //comfort = new Motivation("Comfort", new List<Strategy>() { obtainLux, killPests });
            //reputation = new Motivation("Reputation", new List<Strategy>() { obtainRareItems, killEnemies, visitDangerousPlace });
            //serenity = new Motivation("Serenity", new List<Strategy>() { revengeJustice, captureCriminal, checkOnNPC1, checkOnNPC2, recoverLostStolenItem, rescueNPC });
            //protection = new Motivation("Protection", new List<Strategy>() { attackThreateningEntities, captureCriminal2, treatOrRepair1, treatOrRepair2, createDiversion1, createDiversion2, assembleFortification, guardEntity, recruit });
            conquest = new Motivation("Conquest", new List<Strategy>() { /*attackEnemy, stealStuff, */recruit2 });
            //wealth = new Motivation("Wealth", new List<Strategy>() { gatherRawMaterials, stealValuablesforResale, makeValuablesforResale });
            //ability = new Motivation("Ability", new List<Strategy>() { assembleTollforNewSkill, obtainTrainingMaterials, useExistingTools, practiceCombat, praticeSkill, researchSkill1, researchSkill2 });
            //equipment = new Motivation("Equipment", new List<Strategy>() { assemble/*, deliverSupplies, stealSupplies, tradeforSupplies*/ });
        }

        public Parameter none_p;

        public Parameter capture_p;

        public Parameter damage_p1;
        public Parameter damage_p2;

        public Parameter defend_p1;
        public Parameter defend_p2;

        public Parameter escort_p1;
        public Parameter escort_p2;

        public Parameter examine_p1;
        public Parameter examine_p2;

        public Parameter exchange_p1;
        public Parameter exchange_p2;
        public Parameter exchange_p3;

        public Parameter experiment_p;

        public Parameter explore_p;

        public Parameter free_p;

        public Parameter gather_p;

        public Parameter give_p1;
        public Parameter give_p2;

        public Parameter goto_p;

        public Parameter kill_p;

        public Parameter listen_p;

        public Parameter read_p;

        public Parameter repair_p;

        public Parameter report_p;

        public Parameter spy_p1;
        public Parameter spy_p2;

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
        //public Action capture;
        //public Action damage;
        //public Action defend;
        //public Action escort;
        //public Action examine;
        //public Action exchange;
        //public Action experiment;
        //public Action explore;
        //public Action free;
        //public Action gather;
        //public Action give;
        public Action go_to;
        //public Action kill;
        public Action listen;
        //public Action read;
        //public Action repair;
        //public Action report;
        //public Action spy;
        //public Action take;
        //public Action use;
        public Action quest; 

        public Action subquest;
        public Action go_to_NT;
        public Action learn_NT;
        //public Action prepare_NT;
        //public Action get_NT;
        //public Action steal_NT;
        //public Action capture_NT;
        //public Action defeat_NT;
        //public Action report_NT;
        //public Action give_NT;
        //public Action rescue_NT; 

        public Rules subquestRule;
        public Rules gotoRule;
        public Rules learnRule;
        //public Rules prepareRule;
        //public Rules getRule;
        //public Rules stealRule;
        //public Rules captureRule;
        //public Rules defeatRule;
        //public Rules reportRule;
        //public Rules giveRule;
        //public Rules rescueRule;

        public Strategy itemForStudy;
        public Strategy spy_Strat;
        public Strategy interviewNPC;
        public Strategy useItemOnField;
                                       
        public Strategy obtainLux;
        public Strategy killPests;
                                       
        public Strategy obtainRareItems;
        public Strategy killEnemies;
        public Strategy visitDangerousPlace;
                                       
        public Strategy revengeJustice;
        public Strategy captureCriminal;
        public Strategy checkOnNPC1;
        public Strategy checkOnNPC2;
        public Strategy recoverLostStolenItem;
        public Strategy rescueNPC;

        public Strategy attackThreateningEntities;
        public Strategy captureCriminal2;
        public Strategy treatOrRepair1;
        public Strategy treatOrRepair2;
        public Strategy createDiversion1;
        public Strategy createDiversion2;
        public Strategy assembleFortification;
        public Strategy guardEntity;
        public Strategy recruit;
                                         
        public Strategy attackEnemy;
        public Strategy stealStuff;
        public Strategy recruit2;
                                         
        public Strategy gatherRawMaterials;
        public Strategy stealValuablesforResale;
        public Strategy makeValuablesforResale;
                                          
        public Strategy assembleTollforNewSkill;
        public Strategy obtainTrainingMaterials;
        public Strategy useExistingTools;
        public Strategy practiceCombat;
        public Strategy praticeSkill;
        public Strategy researchSkill1;
        public Strategy researchSkill2;
                                          
        public Strategy assemble;
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

        public List<String> chars = new List<string>() { "npc1", "npc2" , "npc3" , "npc4" , "npc5" , "npc6" , "npc7" , "npc8" , "npc9", "npc10", "npc11", "npc12", "npc13", "npc14", "npc15", "npc16", "npc17", "npc18", "npc19", "npc20" };
        public List<String> items = new List<string>() { "item1", "item2", "item3", "item4", "item5", "item6", "item7", "item8", "item9", "item10", "item11", "item12", "item13", "item14", "item15", "item16", "item17", "item18", "item19", "item20" };
        public List<String> enemies = new List<string>() {"enemy1", "enemy2", "enemy3", "enemy4", "enemy5", "enemy6", "enemy7", "enemy8", "enemy9", "enemy10", "enemy11", "enemy12", "enemy13", "enemy14", "enemy15", "enemy16", "enemy17", "enemy18", "enemy19", "enemy20" };
        public List<String> locations = new List<string>() { "place1", "place2", "place3", "place4", "place5", "place6", "place7", "place8", "place9", "place10", "place11", "place12", "place13", "place14", "place15", "place16", "place17", "place18", "place19", "place20" };

        public void GenerateOne()
        {
            QuestGiver testGiver = new QuestGiver("npc1", new List<Motivation>() {/*knowledge, comfort, reputation, serenity, protection,*/ conquest/*, wealth, ability, equipment*/ }, chars, items, enemies, locations);
            Quest testQuest = testGiver.CreateQuest(1, new List<Strategy>() {/* itemForStudy, spy_Strat, interviewNPC, useItemOnField, obtainLux, killPests, obtainRareItems, killEnemies, visitDangerousPlace, revengeJustice, captureCriminal, checkOnNPC1, checkOnNPC2, recoverLostStolenItem, rescueNPC, attackThreateningEntities, captureCriminal2, treatOrRepair1, treatOrRepair2, createDiversion1, createDiversion2, assembleFortification, guardEntity, recruit, attackEnemy, stealStuff, */recruit2/*, gatherRawMaterials, stealValuablesforResale, makeValuablesforResale, assembleTollforNewSkill, obtainTrainingMaterials, useExistingTools, practiceCombat, praticeSkill, researchSkill1, researchSkill2, assemble, deliverSupplies, stealSupplies, tradeforSupplies */}, new List<Rules>() { subquestRule, gotoRule, learnRule/*, prepareRule, getRule, stealRule, captureRule, defeatRule, reportRule, giveRule, rescueRule */});

            string path = @"..\..\Modules\QuestGenerator\MissionList.xml";

            if (new FileInfo(path).Length == 0)
            {
                List<CustomBTNode> tempList = new List<CustomBTNode>();
                tempList.Add(testGiver.customBTTree);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path,tempList);
            }
            else
            {
                var tempList = XmlSerialization.ReadFromXmlFile<List<CustomBTNode>>(path);
                tempList.Add(testGiver.customBTTree);
                XmlSerialization.WriteToXmlFile<List<CustomBTNode>>(path, tempList);
            }


            
        }


    }
}
