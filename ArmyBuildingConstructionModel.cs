// Decompiled with JetBrains decompiler
// Type: UseYourArmyToBuildImprovements.ArmyBuildingConstructionModel
// Assembly: UseYourArmyToBuildImprovements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 216DB483-E325-47B6-8FD2-A1519E3CCBF5
// Assembly location: C:\Users\trist\Documents\Modding\ArmyImprove\UseYourArmyToBuildImprovements\bin\Win64_Shipping_Client\UseYourArmyToBuildImprovements.dll

using Helpers;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using System.Linq;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace UseYourArmyToBuildImprovements
{
    internal class ArmyBuildingConstructionModel : DefaultBuildingConstructionModel
    {
        public new const int TownBoostCost = 500;
        public new const int TownBoostBonus = 50;
        public new const int CastleBoostCost = 250;
        public new const int CastleBoostBonus = 20;
        private const float HammerMultiplier = 0.01f;
        private const int VeryLowLoyaltyValue = 25;
        private const float MediumLoyaltyValue = 50f;
        private const float HighLoyaltyValue = 75f;
        private const float HighestLoyaltyValue = 100f;
        private static readonly TextObject ProductionFromMarketText = new TextObject("{=vaZDJGMx}Construction from Market", (Dictionary<string, object>)null);
        private static readonly TextObject BoostText = new TextObject("{=yX1RycON}Boost from Reserve", (Dictionary<string, object>)null);
        private static readonly TextObject HighLoyaltyBonusText = new TextObject("{=aSniKUJv}High Loyalty", (Dictionary<string, object>)null);
        private static readonly TextObject LowLoyaltyPenaltyText = new TextObject("{=SJ2qsRdF}Low Loyalty", (Dictionary<string, object>)null);
        private static readonly TextObject VeryLowLoyaltyPenaltyText = new TextObject("{=CcQzFnpN}Very Low Loyalty", (Dictionary<string, object>)null);
        private static readonly TextObject ArmyConstructionBonusText = new TextObject("{=armycon}Player Army Bonus", (Dictionary<string, object>)null);

        public override ExplainedNumber CalculateDailyConstructionPower(
            Town town,
            bool includeDescriptions = false)
        {
            ExplainedNumber result = new ExplainedNumber(0.0f, includeDescriptions, (TextObject)null);
            this.CalculateDailyConstructionPowerInternal(town, ref result);
            return result;
        }

        public override int CalculateDailyConstructionPowerWithoutBoost(Town town)
        {
            ExplainedNumber result = new ExplainedNumber(0.0f, false, (TextObject)null);
            return this.CalculateDailyConstructionPowerInternal(town, ref result, true);
        }

        private int CalculateDailyConstructionPowerInternal(Town town, ref ExplainedNumber result, bool omitBoost = false)
        {

            float num1 = town.Prosperity * 0.01f;
            result.Add(num1, GameTexts.FindText("str_prosperity", (string)null), (TextObject)null);

            if (!omitBoost && town.BoostBuildingProcess > 0)
            {
                int num2 = town.IsCastle ? 250 : 500;
                int boostAmount = this.GetBoostAmount(town);
                float num3 = Math.Min(1f, town.BoostBuildingProcess / (float)num2);
                float num4 = 0.0f;
                if (town.IsTown && town.Governor != null && town.Governor.GetPerkValue(DefaultPerks.Engineering.Clockwork))
                    num4 += DefaultPerks.Engineering.Clockwork.SecondaryBonus;
                int num5 = boostAmount + (int)MathF.Round(boostAmount * num4);
                result.Add(num5 * num3, BoostText, (TextObject)null);
            }

            if (town.Governor != null && town.Governor.CurrentSettlement?.Town == town)
            {
                SkillHelper.AddSkillBonusForTown(DefaultSkills.Engineering, DefaultSkillEffects.TownProjectBuildingBonus, town, ref result);
                PerkHelper.AddPerkBonusForTown(DefaultPerks.Steward.ForcedLabor, town, ref result);
            }

            if (town.Settlement.OwnerClan.Leader.CharacterObject.Culture.HasFeat(DefaultCulturalFeats.BattanianConstructionFeat))
                result.AddFactor(DefaultCulturalFeats.BattanianConstructionFeat.EffectBonus * 0.01f, DefaultCulturalFeats.BattanianConstructionFeat.Name);

            if (town.Governor != null && town.Governor.CurrentSettlement?.Town == town && !TaleWorlds.Core.Extensions.IsEmpty(town.BuildingsInProgress))
            {
                if (town.IsCastle && town.Governor.GetPerkValue(DefaultPerks.Engineering.MilitaryPlanner))
                    result.AddFactor(DefaultPerks.Engineering.MilitaryPlanner.SecondaryBonus, DefaultPerks.Engineering.MilitaryPlanner.Name);
                else if (town.IsTown && town.Governor.GetPerkValue(DefaultPerks.Engineering.Carpenters))
                    result.AddFactor(DefaultPerks.Engineering.Carpenters.SecondaryBonus, DefaultPerks.Engineering.Carpenters.Name);

                Building building = town.BuildingsInProgress.Peek();
                if ((building.BuildingType == DefaultBuildingTypes.Fortifications || building.BuildingType == DefaultBuildingTypes.CastleBarracks || building.BuildingType == DefaultBuildingTypes.CastleMilitiaBarracks || building.BuildingType == DefaultBuildingTypes.SettlementGarrisonBarracks || building.BuildingType == DefaultBuildingTypes.SettlementMilitiaBarracks || building.BuildingType == DefaultBuildingTypes.SettlementAquaducts) && town.Governor.GetPerkValue(DefaultPerks.Engineering.Stonecutters))
                    result.AddFactor(DefaultPerks.Engineering.Stonecutters.PrimaryBonus, DefaultPerks.Engineering.Stonecutters.Name);
            }

            int num6 = town.SoldItems.Sum(x => ((int)x.Category.Properties) != 1 ? 0 : x.Number);
            if (num6 > 0)
                result.Add(0.25f * num6, ProductionFromMarketText, null);

            BuildingType buildingType = TaleWorlds.Core.Extensions.IsEmpty(town.BuildingsInProgress) ? null : town.BuildingsInProgress.Peek().BuildingType;
            if (DefaultBuildingTypes.MilitaryBuildings.Contains(buildingType))
                PerkHelper.AddPerkBonusForTown(DefaultPerks.TwoHanded.Confidence, town, ref result);
            if (buildingType == DefaultBuildingTypes.SettlementMarketplace || buildingType == DefaultBuildingTypes.SettlementAquaducts || buildingType == DefaultBuildingTypes.SettlementLimeKilns)
                PerkHelper.AddPerkBonusForTown(DefaultPerks.Trade.SelfMadeMan, town, ref result);

            if (town.Loyalty >= 75.0)
            {
                float num7 = MBMath.Map(town.Loyalty, 75f, 100f, 0.0f, 20f);
                float num8 = result.ResultNumber * (num7 / 100f);
                result.Add(num8, HighLoyaltyBonusText, null);
            }
            else if (town.Loyalty > SubModule.MinimumLoyaltyThreshold && town.Loyalty <= 50.0)
            {
                float num9 = MBMath.Map(town.Loyalty, 25f, 50f, 0.0f, 50f);
                float num10 = result.ResultNumber * (num9 / 100f);
                result.Add(-num10, LowLoyaltyPenaltyText, null);
            }
            else if (town.Loyalty <= SubModule.MinimumLoyaltyThreshold)
            {
                result.Add(-result.ResultNumber, VeryLowLoyaltyPenaltyText, null);
            }

            float effectOfBuildings = this.GetEffectOfBuildings(town, BuildingEffectEnum.Construction);
            if (effectOfBuildings > 0.0)
                result.Add(effectOfBuildings, GameTexts.FindText("str_building_bonus", null), null);

            if (Hero.MainHero.CurrentSettlement == town.Settlement && town.OwnerClan == Hero.MainHero.Clan)
            {
                float armyEngineerBonus = this.GetArmyEngineerBonus();
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

            result.LimitMin(0.0f);
            return (int)result.ResultNumber;
        }

        internal float GetEffectOfBuildings(Town town, BuildingEffectEnum buildingEffect)
        {
            float effectOfBuildings = 0.0f;
            foreach (Building building in town.Buildings)
                effectOfBuildings += building.GetBuildingEffectAmount(buildingEffect);
            return effectOfBuildings;
        }

        private float GetArmyEngineerBonus()
        {
            MobileParty mainParty = MobileParty.MainParty;
            if (mainParty.EffectiveEngineer == null)
                return 0.0f;
            float num = mainParty.EffectiveEngineer.GetSkillValue(DefaultSkills.Engineering);
            if (num > 300.0f)
                num = 300f;
            return num * SubModule.BricksPerEngineerSkillPoint;
        }
    }
}
