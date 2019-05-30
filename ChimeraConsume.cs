using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;
using XRL.Core;
using XRL.UI;
using XRL.Messages;

namespace XRL.World.Parts
{
    [Serializable]
    public class ChimeraConsume : IPart
    {
        public string DamageMessage = "from %o digestive enzymes!";
        private List<string> excludedMutations = new List<string>(){"AcidSlimeGlands","Adaptation","AdrenalControl","Allurement",
        "Astral","Aura","BacterialSymbiosis","BaseMutation","Belcher","Berries","BiologicalGenius","BreatherBase","Burrowing",
        "ChameleonSkin","ColdAbsorption","CombatReflexes","ConcussionEntry","CorrosiveBreather",
        "CyberElectricalGeneration","DensityState","DirectionSense","Displacement","DissolvingJuices",
        "DualBrain","Empathy","EnergyAbsorption","EnergyAssimilation","EnergyDissapation","EnergyGeneration","EnergyMetamorphosis",
        "EnergyReflection","EnhancedSkeleton","ExplosiveFruit","Farport","FattyHump","FearAura","FireBreather",
        "ForceFieldGeneration","FreezeBreath","FrostWebs","Gills",
        "IceBreather","Immunity","Infravision","Interdiction","Invisibility","KineticDampening",
        "LiquidFont","LiquidSpitter","LivingSegments","MagneticManipulation","MagneticPulse","Mane","MentalHealing",
        "Metamorphed","MilitaryGenius","MolecularDisruption","MolecularSense","MultipleEyes",
        "NightVision","NormalityBreather","Oil","OldCryokinesis","OldElectricalGeneration","OldGasGeneration","OldPyrokinesis",
        "Paralyze","PlantControl","PoisionGeneration","PoisionSap","RepellingField","RepellingForce","Serenity",
        "Shapeshift","Shorter","Skittish","SlogGlands","Sonar","SpiderWebs","SporePuffer",
        "StickyTongue","StunBreather","Summoning","Taller", "Ultravision",
        "TemporalHealing","UrchinBelcher","WallWalker","WaveformWorm"};
        /// <summary>Potentially overpowered NPC mutations</summary>
        private List<string> ForbiddenMutations = new List<string>() {
        "SleepBreather", "Decarbonizer", "MultiHorns", "SoundImitation", "MentalInvisibility", "Telekinesis","TelekineticFlight","TeleportObject","Illusion",
        "HeightenedEgo","HeightenedIntelligence", "Levitation", "Intuition","PoisonBreather", "HeatAbsorption",
        "HeightenedPrecision","HeightenedSight","HeightenedSmell","HeightenedTaste","HeightenedTouch","HeightenedWillpower",
        "DensityControl", "Fear", "StoneGaze", "ConfusionBreather","Infiltrate"
        };
        private List<string> FuseableMutations = new List<string>() {
        };
        public int creatureCount;
        public int attributeCount;
        public int mutationPointCount;
        public bool descriptionSet = false;

        public override bool SameAs(IPart p)
        {
            return false;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart) this, "EndTurnEngulfing");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurnEngulfing")
            {
                if (descriptionSet == false)
                {
                    this.ParentObject.GetPart<Description>().Short = "I'm not a bad slime";
                    descriptionSet = true;
                }
                GameObject parameter = E.GetParameter<GameObject>("Object");
                if (parameter != null)
                {
                    Damage damage = new Damage(Stat.Random(this.ParentObject.Statistics["Toughness"].BaseValue / 4, this.ParentObject.Statistics["Toughness"].BaseValue));
                    Event E1 = Event.New("TakeDamage", 0, 0, 0);
                    E1.AddParameter("Damage", (object)damage);
                    E1.AddParameter("Owner", (object)this.ParentObject);
                    E1.AddParameter("Attacker", (object)this.ParentObject);
                    E1.AddParameter("Message", this.DamageMessage);
                    parameter.FireEvent(E1);
                    if (parameter.hitpoints <= 0)
                    {
                        if (DifficultyEvaluation.GetDifficultyDescription(parameter) == "&wAverage")
                        {
                            this.statGains(1);
                        }
                        else if (DifficultyEvaluation.GetDifficultyDescription(parameter) == "&WTough")
                        {
                            this.statGains(2);
                        }
                        else if (DifficultyEvaluation.GetDifficultyDescription(parameter) == "&rVery Tough")
                        {
                            this.statGains(4);
                        }
                        else if (DifficultyEvaluation.GetDifficultyDescription(parameter) == "&RImpossible")
                        {
                            this.statGains(8);
                        }
                        Mutations targetMutations = parameter.GetPart<Mutations>();
                        Mutations playerMutations = this.ParentObject.GetPart<Mutations>();
                        if (targetMutations != null)
                        {
                            bool CasterIsPlayer = XRLCore.Core.Game.Player.Body == this.ParentObject ? true : false;
                            bool HasMut;
                            bool AffectsBody;
                            string PopupText;
                            foreach (BaseMutation mutation in targetMutations.MutationList)
                            {
                                HasMut = playerMutations.HasMutation(mutation);
                                AffectsBody = mutation.AffectsBodyParts();
                                //if (HasMut&&FuseableMutations.Contains(mutation.Name))
                                //{
                                //
                                //}
                                if (!HasMut && mutation.CompatibleWith(this.ParentObject))
                                {
                                    if (CasterIsPlayer && ForbiddenMutations.Contains(mutation.Name))//Forbidden Mutations
                                    {
                                        if (AffectsBody)
                                            PopupText = "Integrate Forbidden-" + mutation.GetMutationEntry().Category.Name + " BodyBased mutation with name of " + mutation.Name;
                                        else
                                            PopupText = "Integrate Forbidden-" + mutation.GetMutationEntry().Category.Name + " mutation with name of " + mutation.Name;
                                        if(UI.Popup.ShowYesNoCancel(PopupText) == DialogResult.Yes)
                                            playerMutations.AddMutation(mutation, 1);
                                    }
                                    else if (!excludedMutations.Contains(mutation.Name))
                                    {
                                        if (AffectsBody)
                                            PopupText = "Integrate " + mutation.GetMutationEntry().Category.Name + " BodyBased mutation with name of " + mutation.Name;
                                        else
                                            PopupText = "Integrate " + mutation.GetMutationEntry().Category.Name + " mutation with name of " + mutation.Name;
                                        if (CasterIsPlayer || UI.Popup.ShowYesNoCancel(PopupText) == DialogResult.Yes)
                                            playerMutations.AddMutation(mutation, 1);
                                    }
                                }

                            }
                        }
                        Stomach part = this.ParentObject.GetPart<Stomach>();
                        this.ParentObject.RemoveEffect("Famished", false);
                        part.HungerLevel = 0;
                        part.CookCount = 0;
                        part.CookingCounter = 0;
                        part.Water = 30000;
                    }
                }
            }
            return true;
        }
        //                 if (targetMutations != null)
        //                 {
        //string MutCat;
        //                     foreach (BaseMutation mutation in targetMutations.MutationList)
        //                     {
        //	MutCat = mutation.GetMutationEntry().Category.Name;
        //                         if(!playerMutations.HasMutation(mutation) && mutation.CompatibleWith(IPart.ThePlayer))
        //                         {
        //		if(MutCat=="Mental")
        //		{
        //			playerMutations.AddMutation(mutation,1);
        //		}
        //		else if(MutCat=="Physical")
        //		{
        //                                 if (!mutation.AffectsBodyParts())
        //                                 {
        //                                     playerMutations.AddMutation(mutation, 1);
        //                                 }
        //                                 else if (!excludedMutations.Contains(mutation.Name)||!(mutation.DisplayName.Contains("Wings")||mutation.DisplayName.Contains("Legs")))
        //                                 {
        //                                     playerMutations.AddMutation(mutation, 1);
        //                                 }										
        //		}
        //                         }
        //                     }
        //                 }

        public void statGains(int amount)
        {
            if (mutationPointCount != IPart.ThePlayer.Statistics["Level"].BaseValue * 3)
            {
                if (creatureCount == 10)
                {
                    this.ParentObject.Statistics["MP"].BaseValue += 1;
                    if (XRLCore.Core.Game.Player.Body == this.ParentObject)
                        MessageQueue.AddPlayerMessage("You gain a &Cmutation&y point.");

                    creatureCount = 0;
                    mutationPointCount += 1;
                }
                else
                {
                    creatureCount += 1;
                }
            }
        }
    }
}
