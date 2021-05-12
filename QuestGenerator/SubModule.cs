using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace QuestGenerator
{
    public class SubModule : MBSubModuleBase
    {
        
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {

            bool campaign = game.GameType is Campaign;

            if (campaign)
            {
                InformationManager.DisplayMessage(new InformationMessage("Behavior added \n"));
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new QuestGenTestCampaignBehavior());
            }


        }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            new Harmony("QuestGenerator").PatchAll();
        }


    }
}