using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143B, 0x143A )]
	public class Maul : BaseBashing
	{
		//public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override string NameType { get { return "Maul"; } }
		
		public override int SheathedMaleWaistID{ get{ return 15222; } }
		public override int SheathedFemaleWaistID{ get{ return 15223; } }
		
		public override int AosStrengthReq{ get{ return 45; } }
		public override double OverheadPercentage{ get{ return 0.5; } }
		public override double SwingPercentage{ get{ return 0.4; } }
		public override double ThrustPercentage{ get{ return 0.1; } }
		public override double RangedPercentage{ get{ return 0; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 13; } }
		public override double AosSpeed{ get{ return 3.25; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 10; } }
		public override int OldMaxDamage{ get{ return 30; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public Maul() : base( 0x143B )
		{
			Weight = 7.0;
			AosElementDamages.Blunt = 100;
		}

		public Maul( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
