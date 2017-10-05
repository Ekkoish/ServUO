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
    public class AG_MediumBlueCarpetAddon : BaseAddon, IDyable
	{
        public virtual bool Dye( Mobile from, DyeTub sender )
        {
            return true;
        }

		public override BaseAddonDeed Deed
		{
			get
			{
				return new AG_MediumBlueCarpetAddonDeed();
			}
		}

		[ Constructable ]
		public AG_MediumBlueCarpetAddon( int hue )
		{
			AddonComponent ac;
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 0, 2, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 1, 1, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 1, 2, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 2, 1, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 0, -1, 0 );
			ac = new AddonComponent( 2754 );
			ac.Hue = hue;
			AddComponent( ac, 3, 3, 0 );
			ac = new AddonComponent( 2756 );
			ac.Hue = hue;
			AddComponent( ac, -2, 3, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, 2, 0, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 2, -1, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 2, 2, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, -1, -1, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, -1, 2, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 2749 );
			ac.Hue = hue;
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 1, 0, 0 );
			ac = new AddonComponent( 2755 );
			ac.Hue = hue;
			AddComponent( ac, -2, -2, 0 );
			ac = new AddonComponent( 2750 );
			ac.Hue = hue;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 2757 );
			ac.Hue = hue;
			AddComponent( ac, 3, -2, 0 );
			ac = new AddonComponent( 2806 );
			ac.Hue = hue;
			AddComponent( ac, -2, -1, 0 );
			ac = new AddonComponent( 2806 );
			ac.Hue = hue;
			AddComponent( ac, -2, 0, 0 );
			ac = new AddonComponent( 2806 );
			ac.Hue = hue;
			AddComponent( ac, -2, 1, 0 );
			ac = new AddonComponent( 2806 );
			ac.Hue = hue;
			AddComponent( ac, -2, 2, 0 );
			ac = new AddonComponent( 2807 );
			ac.Hue = hue;
			AddComponent( ac, -1, -2, 0 );
			ac = new AddonComponent( 2807 );
			ac.Hue = hue;
			AddComponent( ac, 0, -2, 0 );
			ac = new AddonComponent( 2807 );
			ac.Hue = hue;
			AddComponent( ac, 1, -2, 0 );
			ac = new AddonComponent( 2807 );
			ac.Hue = hue;
			AddComponent( ac, 2, -2, 0 );
			ac = new AddonComponent( 2808 );
			ac.Hue = hue;
			AddComponent( ac, 3, -1, 0 );
			ac = new AddonComponent( 2808 );
			ac.Hue = hue;
			AddComponent( ac, 3, 0, 0 );
			ac = new AddonComponent( 2808 );
			ac.Hue = hue;
			AddComponent( ac, 3, 1, 0 );
			ac = new AddonComponent( 2808 );
			ac.Hue = hue;
			AddComponent( ac, 3, 2, 0 );
			ac = new AddonComponent( 2809 );
			ac.Hue = hue;
			AddComponent( ac, -1, 3, 0 );
			ac = new AddonComponent( 2809 );
			ac.Hue = hue;
			AddComponent( ac, 0, 3, 0 );
			ac = new AddonComponent( 2809 );
			ac.Hue = hue;
			AddComponent( ac, 1, 3, 0 );
			ac = new AddonComponent( 2809 );
			ac.Hue = hue;
			AddComponent( ac, 2, 3, 0 );

		}

		public AG_MediumBlueCarpetAddon( Serial serial ) : base( serial )
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

	public class AG_MediumBlueCarpetAddonDeed : BaseAddonDeed, IDyable
	{
		public override BaseAddon Addon
		{
			get
			{
				return new AG_MediumBlueCarpetAddon( this.Hue );
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
		public AG_MediumBlueCarpetAddonDeed()
		{
			Name = "Medium Blue Carpet";
		}

		public AG_MediumBlueCarpetAddonDeed( Serial serial ) : base( serial )
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