using Qud.API;
using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Language;
using XRL.Messages;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts
{
    [Serializable]
    public class EvolutionO : IPart
    {
        public static Dictionary<int, string> PrefixMap = new Dictionary<int, string>()
        {
          {
            1,
            "mono"
          },
          {
            2,
            "di"
          },
          {
            3,
            "tri"
          },
          {
            4,
            "tetra"
          },
          {
            5,
            "penta"
          },
          {
            6,
            "hexa"
          },
          {
            7,
            "hepta"
          },
          {
            8,
            "octa"
          },
          {
            9,
            "ennea"
          },
          {
            10,
            "deca"
          },
          {
            11,
            "hendeca"
          },
          {
            12,
            "dodeca"
          },
          {
            13,
            "triskaideca"
          },
          {
            14,
            "tetrakaideca"
          },
          {
            15,
            "pentakaideca"
          },
          {
            16,
            "hexakaideca"
          },
          {
            17,
            "heptakaideca"
          },
          {
            18,
            "octakaideca"
          },
          {
            19,
            "enneakaideca"
          },
          {
            20,
            "icosa"
          }
        };

        public List<byte> ComponentLiquids = new List<byte>();
        public int totalHands = 4;
        public byte newLiquid = 99;
        public Cell C;
        public bool hasFormed;
        public bool isCatalyzing;

        public EvolutionO()
        {
        }

        public EvolutionO(byte _initLiquid, Cell _C)
        {
            this.newLiquid = _initLiquid;
            this.C = _C;
        }

        public override bool SameAs(IPart p)
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            //ActivatedAbilities part = this.ParentObject.GetPart<ActivatedAbilities>();
            //if (part != null)
            //{
            //    this.ActivatedAbilityID = part.AddAbility("Evolve", "CommandEvolve", "Physical Mutation", -1, false, false, "Evolve from liquid.", string.Empty + (object)'\x0003', false);
            //    this.ActivatedAbility = part.AbilityByGuid[this.ActivatedAbilityID];
            //}
            //return base.Mutate(GO, Level);
            Object.RegisterPartEvent((IPart)this, "EnteredCell");
            Object.RegisterPartEvent((IPart)this, "BeginTakeAction");
            Object.RegisterPartEvent((IPart)this, "ObjectCreated");
            Object.RegisterPartEvent((IPart)this, "CommandEvolve");
            base.Register(Object);
            this.EvolveSludge();
        }

        public static string GetPrefix(int index)
        {
            if (index > EvolutionO.PrefixMap.Count)
                return "poly";
            return EvolutionO.PrefixMap[index];
        }

        public void EvolveSludge()
        {
            if (this.newLiquid != (byte)99)
            {
                Mutations part1 = this.ParentObject.GetPart("Mutations") as Mutations;
                if (!part1.HasMutation("LiquidSpitter"))
                    part1.AddMutation((BaseMutation)new LiquidSpitter(LiquidVolume.ComponentLiquidTypes[this.newLiquid].GetName((LiquidVolume)null).Substring(2)), 1);
                LiquidSpitter mutation = part1.GetMutation("LiquidSpitter") as LiquidSpitter;
                mutation.extraLiquids.Add(this.newLiquid);
                if (mutation.extraLiquids.Count == 1 && this.newLiquid == (byte)18)
                    mutation.extraLiquids.Add((byte)0);
                this.ComponentLiquids.Add(this.newLiquid);
                string displayName = this.ParentObject.DisplayName;
                this.ParentObject.DisplayName = string.Empty;
                foreach (byte componentLiquid in this.ComponentLiquids)
                {
                    GameObject parentObject = this.ParentObject;
                    parentObject.DisplayName = parentObject.DisplayName + LiquidVolume.ComponentLiquidTypes[componentLiquid].GetAdjective((LiquidVolume)null) + "&y ";
                }
                GameObject parentObject1 = this.ParentObject;
                parentObject1.DisplayName = parentObject1.DisplayName + EvolutionO.GetPrefix(this.ComponentLiquids.Count) + "sludge";
                if (this.ComponentLiquids.Count >= 5)
                    JournalAPI.AddAccomplishment("You witnessed the rare formation of " + Grammar.A(EvolutionO.GetPrefix(this.ComponentLiquids.Count) + "sludge", false) + ".", "general", (string)null, -1L);
                MessageQueue.AddPlayerMessage("The " + LiquidVolume.ComponentLiquidTypes[this.newLiquid].GetName((LiquidVolume)null) + " &ycatalyzes " + this.ParentObject.the + displayName + " into " + this.ParentObject.a + this.ParentObject.DisplayName + ".");
                bool flag = false;
                Body part2 = this.ParentObject.GetPart<Body>();
                foreach (BodyPart bodyPart in part2.GetPart("Hand"))
                {
                    if (bodyPart.Equipped == null)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.totalHands += 2;
                    BodyPart body = part2.GetBody();
                    BodyPart InsertAfter = body.AddPartAt("Pseudopod", 0, (string)null, (string)null, (string)null, (string)null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?(), "Hand", "Missile Weapon");
                    body.AddPartAt(InsertAfter, "Pseudopod", 0, (string)null, (string)null, (string)null, (string)null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
                }
                if (this.newLiquid == (byte)0)
                    this.ParentObject.TakeObject("WateryOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)1)
                    this.ParentObject.TakeObject("SaltyOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)2)
                    this.ParentObject.TakeObject("TarryOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)3)
                {
                    this.ParentObject.TakeObject("MagmaticOmniPseudopod", false, new int?(0));
                    this.ParentObject.Statistics["HeatResistance"].BaseValue += 100;
                    this.ParentObject.pPhysics.Temperature = 1000;
                }
                else if (this.newLiquid == (byte)4)
                {
                    this.ParentObject.TakeObject("SlimyOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<DisarmOnHit>(new DisarmOnHit(100), true);
                }
                else if (this.newLiquid == (byte)5)
                {
                    this.ParentObject.TakeObject("OilyOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<DisarmOnHit>(new DisarmOnHit(100), true);
                }
                else if (this.newLiquid == (byte)6)
                {
                    this.ParentObject.TakeObject("BloodyOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<LifeDrainOnHit>(new LifeDrainOnHit("15-20", 100), true);
                }
                else if (this.newLiquid == (byte)7)
                {
                    this.ParentObject.TakeObject("AcidicOmniPseudopod", false, new int?(0));
                    this.ParentObject.Statistics["AcidResistance"].BaseValue += 100;
                }
                else if (this.newLiquid == (byte)8)
                    this.ParentObject.TakeObject("HoneyedOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)9)
                    this.ParentObject.TakeObject("LushOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)10)
                    this.ParentObject.TakeObject("SludgyOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)11)
                    this.ParentObject.TakeObject("GooeyOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)12)
                    this.ParentObject.TakeObject("PutridOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)13)
                {
                    this.ParentObject.TakeObject("UnctuousOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<DisarmOnHit>(new DisarmOnHit(100), true);
                }
                else if (this.newLiquid == (byte)14)
                    this.ParentObject.TakeObject("OozingOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)15)
                    this.ParentObject.TakeObject("SpicedOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)16)
                {
                    this.ParentObject.TakeObject("LuminousOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<LightSource>(new LightSource(), true);
                    (this.ParentObject.GetPart("LightSource") as LightSource).Radius = 6;
                    this.ParentObject.Statistics["ColdResistance"].BaseValue += 100;
                }
                else if (this.newLiquid == (byte)17)
                    this.ParentObject.TakeObject("NeutronicOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)18)
                {
                    this.ParentObject.TakeObject("HomogenizedOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<CloneOnHit>(new CloneOnHit(), true);
                }
                else if (this.newLiquid == (byte)20)
                    this.ParentObject.TakeObject("WaxenOmniPseudopod", false, new int?(0));
                else if (this.newLiquid == (byte)21)
                {
                    this.ParentObject.TakeObject("InkyOmniPseudopod", false, new int?(0));
                    this.ParentObject.AddPart<DisarmOnHit>(new DisarmOnHit(100), true);
                }
                else if (this.newLiquid == (byte)22)
                    this.ParentObject.TakeObject("SugaryOmniPseudopod", false, new int?(0));
                this.ParentObject.pBrain.PerformReequip(false);
                this.newLiquid = (byte)99;
            }
            this.isCatalyzing = false;
            if (this.ComponentLiquids.Count != 1)
                return;
            MessageQueue.AddPlayerMessage("The reacting liquids congeal into a " + this.ParentObject.ShortDisplayName + ".");
            this.C.AddObject(this.ParentObject);
            XRLCore.Core.Game.ActionManager.AddActiveObject(this.ParentObject);
        }

        public override bool Render(RenderEvent E)
        {
            if (this.ComponentLiquids.Count > 0)
            {
                int num = XRLCore.CurrentFrame % (15 * this.ComponentLiquids.Count);
                E.ColorString = "&" + LiquidVolume.ComponentLiquidTypes[this.ComponentLiquids[num / 15]].GetColor();
            }
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                if (this.isCatalyzing)
                    this.EvolveSludge();
            }
            else if (E.ID == "CommandEvolve")
            {
                if (!this.isCatalyzing)
                {
                    if (!this.hasFormed)
                        this.hasFormed = true;
                    Cell currentCell = this.ParentObject.pPhysics.CurrentCell;
                    if (currentCell != null)
                    {
                        List<GameObject> objectsWithPart = currentCell.GetObjectsWithPart("LiquidVolume");
                        if (objectsWithPart != null)
                        {
                            foreach (GameObject gameObject in objectsWithPart)
                            {
                                LiquidVolume part = gameObject.GetPart<LiquidVolume>();
                                if (part.MaxVolume == -1)
                                {
                                    if (!this.ComponentLiquids.Contains(part.bPrimary) && part.bPrimary != (byte)19)
                                        this.newLiquid = part.bPrimary;
                                    else if (part.bSecondary != byte.MaxValue && part.bSecondary != (byte)19 && !this.ComponentLiquids.Contains(part.bSecondary))
                                        this.newLiquid = part.bSecondary;
                                    if (this.newLiquid != (byte)99)
                                    {
                                        this.isCatalyzing = true;
                                        IPart.AddPlayerMessage(this.ParentObject.The + this.ParentObject.ShortDisplayName + "&y" + this.ParentObject.GetVerb("start", true, false) + " reacting with the " + LiquidVolume.ComponentLiquidTypes[this.newLiquid].GetName(part) + "&y.");
                                        part.UseDrams(500);
                                        if (part.Volume <= 0)
                                            gameObject.Destroy();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (E.ID == "ObjectCreated")
                this.ParentObject.SetStringProperty("AlwaysOffhand", "Yes");
            return true;
        }
    }
}