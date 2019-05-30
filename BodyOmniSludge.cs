using System;
using XRL.World;
using XRL.World.Parts;
using Lat = XRL.World.Capabilities.Laterality;

namespace XRL.World.PartBuilders
{
	public class BodyOmniSludge : IPartBuilder
	{
		public BodyOmniSludge()
		{
		}

		public void BuildPart(IPart iPart, string Context = null)
		{
			Body body = iPart as Body;
			if (body == null)
			{
				return;
			}
			BodyPart bodyPart = body.GetBody();
			bodyPart.AddPart("Nuclear Protrusion", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?()).AddPart("Sensory Bulb", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
			BodyPartType LHand = BodyPartType.VariantOf("Hand", "Left Tentacle", "Left side Manipulator for objects", null, null, null, "OmniSlimeTenticle", null, BodyPartCategory.PROTOPLASMIC, null, null, 2);
			bodyPart.AddPart(LHand, Lat.LEFT, "OmniSlimeTenticle", "Tentacles");
			BodyPartType RHand = BodyPartType.VariantOf("Hand", "Right Tentacle", "Right side Manipulator for objects", null, null, null, "OmniSlimeTenticle", null, BodyPartCategory.PROTOPLASMIC, null, null, 2);
			bodyPart.AddPart(RHand, Lat.RIGHT, "OmniSlimeTenticle", "Tentacles");
			BodyPartType Tentacles = BodyPartType.VariantOf("Hands", "Tentacles", null, null, null, null, "OmniSlimeTenticle");
			bodyPart.AddPart(Tentacles, 0);
			bodyPart.AddPart("Missile Weapon", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
			bodyPart.AddPart("Missile Weapon", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
			bodyPart.AddPart("Thrown Weapon", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
            bodyPart.AddPart("Pseudopod", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
			bodyPart.AddPart("Floating Nearby", 0, (string) null, (string) null, (string) null, (string) null, new int?(), new int?(), new int?(), new bool?(), new bool?(), new bool?(), new bool?(), new bool?());
	
			body.CategorizeAll(5);
			body.FinishBuild();
		}
	}
}