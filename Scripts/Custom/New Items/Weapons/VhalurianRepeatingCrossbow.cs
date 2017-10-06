using System;
using Server.Network;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x26C3, 0x26CD )]
	public class RepeatingCrossbow : BaseRanged
	{
		public override string NameType{ get{ return "repeating crossbow"; } }
		
		public override int EffectID{ get{ return 0x1BFE; } }
		public override Type AmmoType{ get{ return typeof( Bolt ); } }
		public override Item Ammo{ get{ return new Bolt(); } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override double OverheadPercentage{ get{ return 0; } }
		public override double SwingPercentage{ get{ return 0; } }
		public override double ThrustPercentage{ get{ return 0; } }
		public override double RangedPercentage{ get{ return 1; } }
		public override int AosMinDamage{ get{ return 1; } }
		public override int AosMaxDamage{ get{ return 1; } }
		public override double AosSpeed{ get{ return 2.5; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldMinDamage{ get{ return 10; } }
		public override int OldMaxDamage{ get{ return 12; } }
		public override int OldSpeed{ get{ return 41; } }

		public override int DefMaxRange{ get{ return 4; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public RepeatingCrossbow() : base( 0x26C3 )
		{
			Weight = 8.0;
			AosElementDamages.Piercing = 100;
			Name = "repeating crossbow";
		}

		public RepeatingCrossbow( Serial serial ) : base( serial )
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