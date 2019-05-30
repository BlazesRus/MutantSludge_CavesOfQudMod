using System;
using XRL.UI;
using XRL.World;

namespace XRL.World.Parts
{
	[Serializable]
	public class CursedPodWP : IActivePart
	{
		public CursedPodWP()
		{
			this.WorksOnSelf = true;
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID != "EndTurn")
			{
				if (E.ID == "AllowHugeHands")
				{
					return false;
				}
				if (E.ID == "Equipped")
				{
					GameObject gameObjectParameter = E.GetGameObjectParameter("EquippingObject");
					if (this.IsObjectActivePartSubject(gameObjectParameter))
					{
					}
				}
				else if (E.ID == "Unequipped")//Destroy pod if try to remove instead
				{
					GameObject gameObject = E.GetGameObjectParameter("UnequippingObject");
					//if (this.AbilitiesAppliedToEquipper)
					//{
					//
					//}
				}
				//else if (E.ID == "CanBeUnequipped")
				//{
				//	if (E.GetIntParameter("Forced", 0) <= 0 && base.IsReady(false, false, false, false, false, false))
				//	{
				//		return false;
				//	}
				//}//if (E.ID == "BeginBeingUnequipped" && E.GetIntParameter("Forced", 0) <= 0 && base.IsReady(false, false, false, false, false, false))
				//else if (E.ID == "BeginBeingUnequipped" && E.GetIntParameter("Forced", 0) <= 0 && base.IsReady(false, false, false, false, false, false))
				//{
				//	//if (!E.IsSilent() && this.ParentObject.pPhysics != null && this.ParentObject.pPhysics.Equipped != null && this.ParentObject.pPhysics.Equipped.IsPlayer())
				//	//{
				//	//	Popup.Show(string.Concat("You can't remove ", this.ParentObject.the, this.ParentObject.DisplayNameOnly, "!"), true);
				//	//}
				//	return true;
				//}
			}
			else
			{
				base.UseChargeIfOperational(false, false, false, false);
			}
			return base.FireEvent(E);
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "AllowHugeHands");
			//Object.RegisterPartEvent(this, "BeginBeingUnequipped");
			//Object.RegisterPartEvent(this, "CanBeUnequipped");
			Object.RegisterPartEvent(this, "Equipped");
			Object.RegisterPartEvent(this, "Unequipped");
			Object.RegisterPartEvent(this, "EndTurn");
			//Object.RegisterPartEvent(this, "GetInventoryActions");
			//Object.RegisterPartEvent(this, "InvCommandActivate");
			base.Register(Object);
		}
	}
}