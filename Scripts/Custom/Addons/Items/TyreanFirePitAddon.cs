/////////////////////////////////////////////////
//                                             //
// Automatically generated by the              //
// AddonGenerator script by Arya               //
//                                             //
/////////////////////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class AG_TyreanFirePitAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new AG_TyreanFirePitAddonDeed();
			}
		}

		[ Constructable ]
		public AG_TyreanFirePitAddon()
		{
			AddonComponent ac;
			ac = new AddonComponent( 1942 );
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 1943 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 1930 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 10749 );
			ac.Light = LightType.Circle225;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 1941 );
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 1944 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 1932 );
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 1931 );
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 1929 );
			AddComponent( ac, 0, -1, 0 );

		}

		public AG_TyreanFirePitAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class AG_TyreanFirePitAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new AG_TyreanFirePitAddon();
			}
		}

		[Constructable]
		public AG_TyreanFirePitAddonDeed()
		{
			Name = "AG_TyreanFirePit";
		}

		public AG_TyreanFirePitAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
