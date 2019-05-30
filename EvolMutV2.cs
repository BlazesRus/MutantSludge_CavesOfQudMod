// Decompiled with JetBrains decompiler
// Type: XRL.World.Parts.Mutation.EvolMut
// Assembly: Assembly-CSharp, Version=2.0.171.0, Culture=neutral, PublicKeyToken=null
// MVID: 381B18E0-ECC5-49F8-BA6E-A941B3110645
// Assembly location: E:\Program Files (x86)\Steam\steamapps\common\Caves of Qud\CoQ_Data\Managed\Assembly-CSharp.dll

using Qud.API;
using System;
using XRL.Core;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts.Effects;

namespace XRL.World.Parts.Mutation
{
  [Serializable]
  public class EvolMutV2 : BaseMutation
  {
    public Guid ActivatedAbilityID;
    public ActivatedAbilityEntry ActivatedAbility;

    public EvolMutV2()
    {
        this.DisplayName = "Evolution";
    }

    public override void Register(GameObject Object)
    {
      base.Register(Object);
    }

    public override bool FireEvent(Event E)
    {
        return true;
    }

    public override string GetDescription()
    {
      return "You can evolve at will.";
    }

    public override bool CanLevel()
    {
      return false;
    }

    public override string GetLevelText(int Level)
    {
      return string.Empty;
    }

    public override bool Mutate(GameObject GO, int Level)
    {
      ActivatedAbilities part = GO.GetPart<ActivatedAbilities>();
      if (part != null)
      {
        this.ActivatedAbilityID = part.AddAbility("Evolve", "CommandEvolve", "Physical Mutation", -1, false, false, "Evolve from liquid.", string.Empty + (object) '\x0003', false);
        this.ActivatedAbility = part.AbilityByGuid[this.ActivatedAbilityID];
      }
      return base.Mutate(GO, Level);
    }

    public override bool Unmutate(GameObject GO)
    {
      if (this.ActivatedAbilityID != Guid.Empty)
        GO.GetPart<ActivatedAbilities>().RemoveAbility(this.ActivatedAbilityID);
      return base.Unmutate(GO);
    }
  }
}
