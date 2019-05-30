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
    public class OmniConsume : IPart
    {
        public string DamageMessage = "from %o digestive enzymes!";
        private List<string> excludedMutations = new List<string>(){"AcidSlimeGlands","Adaptation","AdrenalControl","Allurement",
        "Astral","Aura","BacterialSymbiosis","BaseMutation","Belcher","Berries","BiologicalGenius","BreatherBase","Burrowing",
        "ChameleonSkin","ColdAbsorption","CombatReflexes","ConcussionEntry","ConfusionBreather","CorrosiveBreather",
        "CyberElectricalGeneration","Decarbonizer","DensityControl","DensityState","DirectionSense","Displacement","DissolvingJuices",
        "DualBrain","Empathy","EnergyAbsorption","EnergyAssimilation","EnergyDissapation","EnergyGeneration","EnergyMetamorphosis",
        "EnergyReflection","EnhancedSkeleton","ExplosiveFruit","Farport","FattyHump","Fear","FearAura","FireBreather",
        "ForceFieldGeneration","FreezeBreath","FrostWebs","Gills","HeatAbsorption","HeightenedEgo","HeightenedIntelligence",
        "HeightenedPrecision","HeightenedSight","HeightenedSmell","HeightenedTaste","HeightenedTouch","HeightenedWillpower",
        "IceBreather","Illusion","Immunity","Infiltrate","Infravision","Interdiction","Intuition","Invisibility","KineticDampening",
        "Levitation","LiquidFont","LiquidSpitter","LivingSegments","MagneticManipulation","MagneticPulse","Mane","MentalHealing",
        "MentalInvisibility","Metamorphed","MilitaryGenius","MolecularDisruption","MolecularSense","MultiHorns","MultipleEyes",
        "NightVision","NormalityBreather","Oil","OldCryokinesis","OldElectricalGeneration","OldGasGeneration","OldPyrokinesis",
        "Paralyze","PlantControl","PoisionGeneration","PoisionSap","PoisonBreather","RepellingField","RepellingForce","Serenity",
        "Shapeshift","Shorter","Skittish","SleepBreather","SlogGlands","Sonar","SoundImitation","SpiderWebs","SporePuffer",
        "StickyTongue","StoneGaze","StunBreather","Summoning","Taller","Telekinesis","TelekineticFlight","TeleportObject",
        "TemporalHealing","Ultravision","UrchinBelcher","WallWalker","WaveformWorm"};
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
                if(descriptionSet == false)
                {
                    this.ParentObject.GetPart<Description>().Short = "I'm not a bad slime";
                    descriptionSet = true;
                }
                GameObject parameter = E.GetParameter<GameObject>("Object");
                if (parameter != null)
                {
                    Damage damage = new Damage(Stat.Random(this.ParentObject.Statistics["Toughness"].BaseValue / 4, this.ParentObject.Statistics["Toughness"].BaseValue));
                    Event E1 = Event.New("TakeDamage", 0, 0, 0);
                    E1.AddParameter("Damage", (object) damage);
                    E1.AddParameter("Owner", (object) this.ParentObject);
                    E1.AddParameter("Attacker", (object) this.ParentObject);
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
                            foreach (BaseMutation mutation in targetMutations.MutationList)
                            {
                                if(!playerMutations.HasMutation(mutation) && !excludedMutations.Contains(mutation.Name) && mutation.CompatibleWith(this.ParentObject))
                                {
                                    if(XRLCore.Core.Game.Player.Body == this.ParentObject || UI.Popup.ShowYesNoCancel("Integrate mutation with name " + mutation.Name) == DialogResult.Yes)
                                        playerMutations.AddMutation(mutation,1);
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
