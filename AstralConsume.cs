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
    public class AstralConsume : IPart
    {
        public string DamageMessage = "from %o digestive enzymes!";
        private List<string> ForbiddenMutations = new List<string>() {
        "SleepBreather", "Decarbonizer", "MultiHorns", "SoundImitation", "MentalInvisibility", "Telekinesis","TelekineticFlight","TeleportObject","Illusion",
        "HeightenedEgo","HeightenedIntelligence", "Levitation", "Intuition","PoisonBreather", "HeatAbsorption",
        "HeightenedPrecision","HeightenedSight","HeightenedSmell","HeightenedTaste","HeightenedTouch","HeightenedWillpower",
        "DensityControl", "Fear", "StoneGaze", "ConfusionBreather","Infiltrate"
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
            Object.RegisterPartEvent((IPart)this, "EndTurnEngulfing");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurnEngulfing")
            {
                if (descriptionSet == false)
                {
                    IPart.ThePlayer.GetPart<Description>().Short = "I'm not a bad slime";
                    descriptionSet = true;
                }
                GameObject parameter = E.GetParameter<GameObject>("Object");
                if (parameter != null)
                {
                    Damage damage = new Damage(Stat.Random(IPart.ThePlayer.Statistics["Toughness"].BaseValue / 2, IPart.ThePlayer.Statistics["Toughness"].BaseValue));
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
                        Mutations playerMutations = IPart.ThePlayer.GetPart<Mutations>();
                        if (targetMutations != null)
                        {
                            bool CasterIsPlayer = XRLCore.Core.Game.Player.Body == this.ParentObject ? true : false;
                            string PopupText;
                            foreach (BaseMutation mutation in targetMutations.MutationList)
                            {
                                //mutation.GetStat() == "Ego"
                                if (!playerMutations.HasMutation(mutation) && mutation.CompatibleWith(this.ParentObject))
                                {
                                    if (mutation.GetMutationEntry().Category.Name == "Mental")
                                    {
                                        PopupText = "Integrate Mental mutation with name of " + mutation.Name;
                                        if (CasterIsPlayer || UI.Popup.ShowYesNoCancel(PopupText) == DialogResult.Yes)
                                            playerMutations.AddMutation(mutation, 1);
                                    }
                                    else if(CasterIsPlayer && ForbiddenMutations.Contains(mutation.Name))//Forbidden Mutations
                                    {
                                        if (mutation.AffectsBodyParts())
                                            PopupText = "Integrate Forbidden-" + mutation.GetMutationEntry().Category.Name + " BodyBased mutation with name of " + mutation.Name;
                                        else
                                            PopupText = "Integrate Forbidden-" + mutation.GetMutationEntry().Category.Name + " mutation with name of " + mutation.Name;
                                        if (UI.Popup.ShowYesNoCancel(PopupText) == DialogResult.Yes)
                                            playerMutations.AddMutation(mutation, 1);
                                    }
                                }
                            }
                        }
                        Stomach part = IPart.ThePlayer.GetPart<Stomach>();
                        IPart.ThePlayer.RemoveEffect("Famished", false);
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
                    IPart.ThePlayer.Statistics["MP"].BaseValue += 1;
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