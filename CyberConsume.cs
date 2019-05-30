using System;
//using System.Collections.Generic;
using XRL.Rules;
using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;
using XRL.Core;
using XRL.UI;
using XRL.Messages;

namespace XRL.World.Parts
{
    [Serializable]
    public class CyberConsume : IPart
    {
        public string DamageMessage = "from %o digestive enzymes!";
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
            if (mutationPointCount != this.ParentObject.Statistics["Level"].BaseValue)
            {
                if (creatureCount == 10)
                {
                    this.ParentObject.Statistics["CyberneticsLicensePoints"].BaseValue += 1;
                    if (XRLCore.Core.Game.Player.Body == this.ParentObject)
                        MessageQueue.AddPlayerMessage("You gain a &CCyberneticsLicense&y point.");
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
