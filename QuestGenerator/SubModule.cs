using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Localization;
using QuestGenerator.QuestBuilder;

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

            Module.CurrentModule.AddInitialStateOption(new InitialStateOption("GenerateOneQuest", new TextObject("Generate one quest (debug purposes)", null), 9990, () => {
                var gen = new Generator(0);
                gen.GenerateOne();
                InformationManager.DisplayMessage(new InformationMessage("Quest Generated"));
            }, () => false));

        }


    }
}