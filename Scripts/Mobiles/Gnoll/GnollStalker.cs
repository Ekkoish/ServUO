using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gnoll corpse" )]
	public class GnollStalker : BaseCreature, IMediumPredator, IRodent
	{
		[Constructable]
		public GnollStalker() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a gnoll stalker";
			BodyValue = 102;
			BaseSoundID = 594;
			IsSneaky = true;

			SetStr( 66, 75 );
			SetDex( 126, 135 );
			SetInt( 35 );

			SetHits( 52, 59 );
			SetMana( 0 );

			SetDamage( 4, 6 );

			SetDamageType( ResistanceType.Piercing, 100 );

			SetResistance( ResistanceType.Blunt, 20, 25 );
			SetResistance( ResistanceType.Piercing, 30, 40 );
			SetResistance( ResistanceType.Slashing, 30, 40 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.Poisoning, 40.1, 60.0 );
			SetSkill( SkillName.MagicResist, 0.0 );
			SetSkill( SkillName.Tactics, 30.1, 50.0 );
			SetSkill( SkillName.UnarmedFighting, 40.1, 50.0 );

			Fame = 1600;
			Karma = -1600;

			VirtualArmor = 14;
			
		}

		public override bool HasFur{ get{ return true; } }
		public override int Meat{ get{ return 6; } }
		public override int Bones{ get{ return 6; } }
		public override int Hides{ get{ return 3; } }
		public override HideType HideType{ get{ return HideType.Thick; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager);
		}

		public GnollStalker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( BaseSoundID == 589 )
				BaseSoundID = 594;
		}
	}
}