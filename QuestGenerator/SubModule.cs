using HarmonyLib;
using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using ThePlotLords.QuestBuilder;

namespace ThePlotLords
{
    public class SubModule : MBSubModuleBase
    {

        public QuestGenTestCampaignBehavior QuestGenTest;

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            bool campaign = game.GameType is Campaign;

            if (!Directory.Exists(@"..\..\Modules\ThePlotLords\PlayerData\"))
            {
                Directory.CreateDirectory(@"..\..\Modules\ThePlotLords\PlayerData\");
            }

            if (!Directory.Exists(@"..\..\Modules\ThePlotLords\SaveFiles\"))
            {
                Directory.CreateDirectory(@"..\..\Modules\ThePlotLords\SaveFiles\");
            }

            if (campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(QuestGenTest = new QuestGenTestCampaignBehavior());
            }


        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("ThePlotLords").PatchAll();
            ValueTuple<bool, TextObject> reason = new ValueTuple<bool, TextObject>(false, new TextObject());
            //Module.CurrentModule.AddInitialStateOption(new InitialStateOption("GenerateOneQuest", new TextObject("Generate one quest (debug purposes)", null), 9990, () => {
            //    var gen = new Generator(0);
            //    gen.GenerateOneDebug();
            //    InformationManager.DisplayMessage(new InformationMessage("Quest Generated"));
            //}, () => reason));

            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("PreGenQuests", new TextObject("Pre-Add Quests", null), 9991, () => {
                var gen = new Generator(0);
                gen.GenerateOneHundred();
                InformationManager.DisplayMessage(new InformationMessage("Quests Added"));
            }, () => reason));
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (this.QuestGenTest != null)
            {
                this.QuestGenTest.TickCampaignBehavior();

                if (Input.IsKeyDown(InputKey.LeftControl))
                {
                    if (Input.IsKeyReleased(InputKey.K))
                    {
                        RemoveQuests.UnnistalQuests();
                    }
                }

            }
        }
    }
}