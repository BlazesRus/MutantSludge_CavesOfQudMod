using System;
using XRL.UI;
using XRL.World;

namespace XRL.World.Parts
{
	[Serializable]
	public class RestrictedPodSlot : IActivePart
	{
		public RestrictedPodSlot()
		{
			this.WorksOnSelf = true;
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID != "EndTurn")
			{
				//if (E.ID == "AllowHugeHands")
				//{
				//	return false;
				//}
				if (E.ID == "CanBeUnequipped")
				{
					if (E.GetIntParameter("Forced", 0) <= 0 && base.IsReady(false, false, false, false, false, false))
					{
						return false;
					}
				}
			    else if (E.ID == "Equipped")
			    {
				    GameObject gameObjectParameter = E.GetGameObjectParameter("EquippingObject");
			    }
				else if (E.ID == "BeginBeingUnequipped" && E.GetIntParameter("Forced", 0) <= 0 && base.IsReady(false, false, false, false, false, false))
				{
					if (!E.IsSilent() && this.ParentObject.pPhysics != null && this.ParentObject.pPhysics.Equipped != null && this.ParentObject.pPhysics.Equipped.IsPlayer())
					{
						Popup.Show(string.Concat("You can't remove ", this.ParentObject.the, this.ParentObject.DisplayNameOnly, "!"), true);
					}
					return false;
				}
			}
			else
			{
				base.UseChargeIfOperational(false, false, false, false);
			}
			return base.FireEvent(E);
		}

		public override void Register(GameObject Object)
		{
			//Object.RegisterPartEvent(this, "AllowHugeHands");
			Object.RegisterPartEvent(this, "BeginBeingUnequipped");
			Object.RegisterPartEvent(this, "CanBeUnequipped");
			Object.RegisterPartEvent(this, "EndTurn");
			base.Register(Object);
		}
	}
}