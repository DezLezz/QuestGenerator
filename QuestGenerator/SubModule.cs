using HarmonyLib;
using System;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using ThePlotLords.QuestBuilder;

namespace ThePlotLords
{
    public class SubModule : MBSubModuleBase
    {        
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {

            bool campaign = game.GameType is Campaign;
                        

            if (campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new QuestGenTestCampaignBehavior());
            }


        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("ThePlotLords").PatchAll();
            ValueTuple<bool, TextObject> reason = new ValueTuple<bool, TextObject>(false, new TextObject());
            //Module.CurrentModule.AddInitialStateOption(new InitialStateOption("GenerateOneQuest", new TextObject("Generate one quest (debug purposes)", null), 9990, () => {
            //    var gen = new Generator(0);
            //    gen.GenerateOne();
            //    InformationManager.DisplayMessage(new InformationMessage("Quest Generated"));
            //}, () => reason));

            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("PreGenQuests", new TextObject("Pre-Add Quests", null), 9991, () => {
                var gen = new Generator(0);
                gen.GenerateOneHundred();
                InformationManager.DisplayMessage(new InformationMessage("Quests Added"));
            }, () => reason));
        }


    }
}