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
    public class AG_SmallGoldenCarpetAddon : BaseAddon, IDyable
	{
        public virtual bool Dye( Mobile from, DyeTub sender )
        {
            return true;
        }

		public override BaseAddonDeed Deed
		{
			get
			{
				return new AG_SmallGoldenCarpetAddonDeed();
			}
		}

		[ Constructable ]
		public AG_SmallGoldenCarpetAddon( int hue )
		{
			AddonComponent ac;
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 2778 );
			ac.Hue = hue;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 2779 );
			ac.Hue = hue;
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 2780 );
			ac.Hue = hue;
			AddComponent( ac, -2, -2, 0 );
			ac = new AddonComponent( 2781 );
			ac.Hue = hue;
			AddComponent( ac, -2, 2, 0 );
			ac = new AddonComponent( 2782 );
			ac.Hue = hue;
			AddComponent( ac, 2, -2, 0 );
			ac = new AddonComponent( 2783 );
			ac.Hue = hue;
			AddComponent( ac, -2, -1, 0 );
			ac = new AddonComponent( 2783 );
			ac.Hue = hue;
			AddComponent( ac, -2, 0, 0 );
			ac = new AddonComponent( 2783 );
			ac.Hue = hue;
			AddComponent( ac, -2, 1, 0 );
			ac = new AddonComponent( 2784 );
			ac.Hue = hue;
			AddComponent( ac, -1, -2, 0 );
			ac = new AddonComponent( 2784 );
			ac.Hue = hue;
			AddComponent( ac, 0, -2, 0 );
			ac = new AddonComponent( 2784 );
			ac.Hue = hue;
			AddComponent( ac, 1, -2, 0 );
			ac = new AddonComponent( 2785 );
			ac.Hue = hue;
			AddComponent( ac, 2, -1, 0 );
			ac = new AddonComponent( 2785 );
			ac.Hue = hue;
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 2785 );
			ac.Hue = hue;
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 2786 );
			ac.Hue = hue;
			AddComponent( ac, -1, 2, 0 );
			ac = new AddonComponent( 2786 );
			ac.Hue = hue;
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 2786 );
			ac.Hue = hue;
			AddComponent( ac, 1, 2, 0 );

		}

		public AG_SmallGoldenCarpetAddon( Serial serial ) : base( serial )
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

	public class AG_SmallGoldenCarpetAddonDeed : BaseAddonDeed, IDyable
	{
		public override BaseAddon Addon
		{
			get
			{
				return new AG_SmallGoldenCarpetAddon( this.Hue );
			}
		}
		
		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		[Constructable]
		public AG_SmallGoldenCarpetAddonDeed()
		{
			Name = "Small Golden Carpet";
		}

		public AG_SmallGoldenCarpetAddonDeed( Serial serial ) : base( serial )
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
