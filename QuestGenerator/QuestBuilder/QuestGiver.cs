using QuestGenerator.QuestBuilder.CustomBT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;

namespace QuestGenerator.QuestBuilder
{
    public class QuestGiver
    {
        public string QuestGiverName { get; set; }

        public string Motivation { get; set; }

        public List<Motivation> Motivations { get; set; }

        public List<string> Characters { get; set; }

        public List<string> Items { get; set; }

        public List<string> Enemies { get; set; }

        public List<string> Locations { get; set; }

        //public List<Quest> subQuests { get; set; }
        static Random rnd = new Random();

        public CustomBTNode customBTTree { get; set; }

        public QuestGiver() { }

        public QuestGiver(string questGiverName, List<Motivation> motivations, List<string> characters, List<string> items, List<string> enemies, List<string> locations)
        {
            QuestGiverName = questGiverName;
            Motivations = motivations;
            Characters = characters;
            Items = items;
            Enemies = enemies;
            Locations = locations;
            //subQuests = new List<Quest>();
        }

        public Quest CreateQuest(int questNumb, List<Strategy> strategies, List<Rules> rules)
        {

            //int twoways = rnd.Next(1, 10);
            int twoways = 0;
            if (twoways <= 0)
            {
                int r = rnd.Next(this.Motivations.Count);

                customBTTree = new CustomBTSelector(null, CustomBTType.selector);
                customBTTree.name = this.Motivations[r].name;

                CustomBTNode motivNode1 = new CustomBTMotivation(null, CustomBTType.motivation);
                motivNode1.name = this.Motivations[r].name;
                customBTTree.addNode(motivNode1);

                CustomBTNode motivNode2 = new CustomBTMotivation(null, CustomBTType.motivation);
                motivNode2.name = this.Motivations[r].name;
                customBTTree.addNode(motivNode2);

                Quest q = new Quest(questNumb, this, strategies, rules);
                QuestNode qNode = new QuestNode(this.Motivations[r].name, q, null);
                qNode.motivation = this.Motivations[r];
                q.root = qNode;
                Expand(q, q.root, questNumb, strategies, rules, motivNode1);

                Quest q1 = new Quest(questNumb, this, strategies, rules);
                QuestNode qNode1 = new QuestNode(this.Motivations[r].name, q1, null);
                qNode1.motivation = this.Motivations[r];
                q1.root = qNode1;
                Expand(q1, q1.root, questNumb, strategies, rules, motivNode2);

                return q;
            }

            else
            {
                int r = rnd.Next(this.Motivations.Count);


                customBTTree = new CustomBTMotivation(null, CustomBTType.motivation);

                customBTTree.name = this.Motivations[r].name;

                Quest q = new Quest(questNumb, this, strategies, rules);

                QuestNode qNode = new QuestNode(this.Motivations[r].name, q, null);

                qNode.motivation = this.Motivations[r];

                q.root = qNode;

                Expand(q, q.root, questNumb, strategies, rules, customBTTree);

                return q;
            }


        }

        public void Expand(Quest q, QuestNode node, int questNumb, List<Strategy> strategies, List<Rules> rules, CustomBTNode currentTreeNode)
        {

            List<Action> sequence = new List<Action>();
            if (node.action != null && node.action.type == "Terminal")
            {
                q.steps.Add(node.action);
            }
            else
            {
                if (node.motivation != null)
                {
                    int r = rnd.Next(node.motivation.strategies.Count);
                    sequence = node.motivation.strategies[r].actions;
                    currentTreeNode.info = strategies[r].name;

                }
                else
                {
                    if (node.action.type == "SubQuest")
                    {
                        q.steps.Add(node.action);

                        //int qn = questNumb + 1;
                        //this.subQuests.Add(this.CreateQuest(qn, strategies, rules));

                        int r = rnd.Next(this.Motivations.Count);

                        QuestNode qNode = new QuestNode(this.Motivations[r].name, q, node);
                        qNode.motivation = this.Motivations[r];
                        node.childNodes.Add(qNode);

                        CustomBTNode newNode = new CustomBTMotivation(null, CustomBTType.motivation, currentTreeNode);
                        newNode.name = this.Motivations[r].name;
                        currentTreeNode.addNode(newNode);

                    }
                    else
                    {
                        int index = weightedChoice(node.rule.weights);

                        sequence = node.rule.getNewAction(index);

                        node.rule.weights = updateWeights(node.rule.actions, node.rule.weights, index, q.root.Depth.Count);

                    }
                }

                foreach (Action aS in sequence)
                {
                    List<Parameter> newParamList = new List<Parameter>();
                    foreach (Parameter p in aS.param)
                    {
                        newParamList.Add(new Parameter(p.type, p.flag, p.sibling_ref));
                    }


                    Action a = new Action(aS.name, aS.type, aS.index, aS.type_of_Target, newParamList);
                    if (a.name.Contains("<"))
                    {
                        QuestNode qNode = new QuestNode(a.name, q, node);
                        foreach (Rules r in rules)
                        {
                            if (r.name == a.name)
                            {
                                qNode.rule = new Rules(r.name, r.type, -1, r.actions, r.weights);
                                break;
                            }
                        }

                        qNode.action = a;
                        node.childNodes.Add(qNode);

                        CustomBTNode newNode = new CustomBTSequence(null, CustomBTType.sequence, currentTreeNode);
                        newNode.name = "Sequence";
                        currentTreeNode.addNode(newNode);
                    }
                    else
                    {
                        if (a.name != "")
                        {
                            QuestNode qNode = new QuestNode(a.name, q, node);
                            qNode.action = a;
                            node.childNodes.Add(qNode);
                            CustomBTNode newNode = new CustomBTAction(a, CustomBTType.action, currentTreeNode);
                            newNode.name = a.name;
                            currentTreeNode.addNode(newNode);

                        }

                    }
                }
                BindParameters(node);
                for (int i = 0; i < node.childNodes.Count; i++)
                {
                    var child = node.childNodes[i];
                    var treeChild = currentTreeNode.Children[i];
                    Expand(q, child, questNumb, strategies, rules, treeChild);
                }
            }


        }

        public void BindParameters(QuestNode node) 
        {
            foreach (QuestNode n in node.childNodes)
            {
                if (n.action != null)
                {
                    foreach (Parameter param1 in n.action.param)
                    {
                        if (param1.flag == 0)
                        {
                            if (node.action != null)
                            {
                                foreach (Parameter param2 in node.action.param)
                                {
                                    if (param1.type == param2.type)
                                    {
                                        param1.target = param2.target;
                                    }
                                }
                            }
                        }
                        else if (param1.flag == 1)
                        {

                            if (param1.type == "Character")
                            {
                                param1.target = node.parentQuest.QuestGiver.Characters[node.parentQuest.charCounter];
                                node.parentQuest.charCounter++;
                            }
                            else if (param1.type == "Item")
                            {
                                param1.target = node.parentQuest.QuestGiver.Items[node.parentQuest.itemCounter];
                                node.parentQuest.itemCounter++;
                            }
                            else if (param1.type == "Location")
                            {                                
                                param1.target = node.parentQuest.QuestGiver.Locations[node.parentQuest.locCounter];
                                node.parentQuest.locCounter++;
                            }
                            else if (param1.type == "Enemy")
                            {

                                param1.target = node.parentQuest.QuestGiver.Enemies[node.parentQuest.enemyCounter];
                                node.parentQuest.enemyCounter++;
                            }
                        }
                        else
                        {
                            if (param1.sibling_ref == -2)
                            {
                                param1.sibling_ref = node.childNodes.IndexOf(n) - 1;
                            }
                            if (param1.sibling_ref > -1)
                            {
                                if (node.childNodes[param1.sibling_ref].action != null)
                                {
                                    foreach (Parameter param2 in node.childNodes[param1.sibling_ref].action.param)
                                    {
                                        if (param1.type == param2.type)
                                        {
                                            param1.target = param2.target;
                                        }
                                    }
                                }

                            }

                        }
                    }
                }

            }
        }

        public int weightedChoice(List<float> weights)
        {
            var rand = new Random();

            var r = rand.NextDouble();
            float limit1 = 0.0f;
            float limit2 = 0.0f;
            int count = weights.Count;

            for (int i = 0; i < count; i++)
            {
                limit2 += weights[i];
                if (r >= limit1 && r <= limit2)
                {
                    return i;
                }
                else
                {
                    limit1 = limit2;
                }
            }

            int r1 = rand.Next(count);

            return r1;
        }

        public List<float> updateWeights(List<List<Action>> ruleActions, List<float> w, int index, int depth)
        {
            w[index] = (float)(w[index] / Math.Pow(depth, 2));
            float portion = (1 - w[index]) / w.Count;

            for (int i = 0; i < w.Count; i++)
            {
                foreach (Action a in ruleActions[i])
                {
                    if (a.type == "Terminal")
                    {
                        w[i] += portion;
                    }
                }
            }

            return w;

        }

    }


}