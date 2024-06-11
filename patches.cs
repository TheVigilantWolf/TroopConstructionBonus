using System;
using System.Diagnostics;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Localization;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Party;

namespace TroopConstructionBonus
{
    [HarmonyPatch(typeof(DefaultBuildingConstructionModel), "CalculateDailyConstructionPowerInternal")]
    class patches
    {
        public const int TownBoostCost = 500;
        public const int TownBoostBonus = 50;
        public const int CastleBoostCost = 250;
        public const int CastleBoostBonus = 20;
        private static readonly TextObject ArmyConstructionBonusText = new TextObject("{=armycon}Player Army Bonus", (Dictionary<string, object>)null);
       
        private static void Postfix(ref int __result, Town town, ref ExplainedNumber result, bool omitBoost = false)
        {
            {
                result.Add(10, new TextObject("test"));
                __result = (int)result.ResultNumber;
                /*if (Hero.MainHero.CurrentSettlement == town.Settlement && town.OwnerClan == Hero.MainHero.Clan)
                {
                    float armyEngineerBonus = GetArmyEngineerBonus();
                    float manpowerBonus = 0.0f; // Declare the variable here
                    if (MobileParty.MainParty.Army == null)
                    {
                        manpowerBonus = MobileParty.MainParty.Party.NumberOfHealthyMembers;
                    }
                    else
                    {
                        foreach (MobileParty andAttachedParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
                        {
                            if (andAttachedParty.CurrentSettlement == Hero.MainHero.CurrentSettlement)
                                manpowerBonus += andAttachedParty.Party.NumberOfHealthyMembers;
                        }
                    }
                    float totalArmyBonus = armyEngineerBonus + (manpowerBonus / SubModule.MenPerBrick);
                    result.Add(totalArmyBonus, ArmyConstructionBonusText, null);
                }
                result.LimitMin(0.0f);*/
            }
        }
        /*private static float GetArmyEngineerBonus()
        {
            MobileParty mainParty = MobileParty.MainParty;
            if (mainParty.EffectiveEngineer == null)
                return 0.0f;
            float num = mainParty.EffectiveEngineer.GetSkillValue(DefaultSkills.Engineering);
            if (num > 300.0f)
                num = 300f;
            return num * SubModule.BricksPerEngineerSkillPoint;
        }*/
    }
}
