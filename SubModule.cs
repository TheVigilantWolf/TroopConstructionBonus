// Decompiled with JetBrains decompiler
// Type: UseYourArmyToBuildImprovements.SubModule
// Assembly: UseYourArmyToBuildImprovements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 216DB483-E325-47B6-8FD2-A1519E3CCBF5
// Assembly location: C:\Users\trist\Documents\Modding\ArmyImprove\UseYourArmyToBuildImprovements\bin\Win64_Shipping_Client\UseYourArmyToBuildImprovements.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

#nullable disable
namespace UseYourArmyToBuildImprovements
{
  public class SubModule : MBSubModuleBase
  {
    private static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UseYourArmyToBuildImprovements.txt");
    public static float MenPerBrick = 4f;
    public static float BricksPerEngineerSkillPoint = 0.25f;
    public static float MinimumLoyaltyThreshold = 10f;

    protected override void OnGameStart(Game game, IGameStarter starterObject)
    {
      base.OnGameStart(game, starterObject);
      starterObject.AddModel((GameModel) new ArmyBuildingConstructionModel());
    }

    protected override void OnSubModuleLoad()
    {
      base.OnSubModuleLoad();
      SubModule.LoadConfig();
    }

    private static void LoadConfig()
    {
      if (!File.Exists(SubModule.ConfigFilePath))
        return;
      try
      {
        List<string> list = ((IEnumerable<string>) File.ReadAllLines(SubModule.ConfigFilePath)).ToList<string>();
        string elementFromList1 = SubModule.GetElementFromList(list, "MenPerBrick");
        if (elementFromList1 != "")
        {
          float single = Convert.ToSingle(elementFromList1);
          if ((double) single >= 0.10000000149011612 && (double) single <= 100.0)
            SubModule.MenPerBrick = single;
        }
        string elementFromList2 = SubModule.GetElementFromList(list, "BricksPerEngineerSkillPoint");
        if (elementFromList2 != "")
        {
          float single = Convert.ToSingle(elementFromList2);
          if ((double) single >= 0.0 && (double) single <= 2.0)
            SubModule.BricksPerEngineerSkillPoint = single;
        }
        string elementFromList3 = SubModule.GetElementFromList(list, "MinimumLoyaltyThreshold");
        if (!(elementFromList3 != ""))
          return;
        float single1 = Convert.ToSingle(elementFromList3);
        if ((double) single1 >= 0.0 && (double) single1 <= 25.0)
          SubModule.MinimumLoyaltyThreshold = single1;
      }
      catch
      {
      }
    }

    private static string GetElementFromList(List<string> stringlist, string elementname)
    {
      if (elementname == "")
        return "";
      foreach (string str in stringlist)
      {
        if (str.Contains("="))
        {
          if (((IEnumerable<string>) str.Split('=')).First<string>().Trim() == elementname)
            return ((IEnumerable<string>) str.Split('=')).Last<string>().Trim();
        }
      }
      return "";
    }
  }
}
