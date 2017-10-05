using System;
using System.Collections;
using Server;
using Server.Multis;

namespace Server.Mobiles
{
	public class VendorInventory
	{
		public static readonly TimeSpan GracePeriod = TimeSpan.FromDays( 7.0 );

		private BaseHouse m_House;
		private string m_VendorName;
		private string m_ShopName;
		private Mobile m_Owner;

		private ArrayList m_Items;
		private int m_Copper;

		private DateTime m_ExpireTime;
		private Timer m_ExpireTimer;

		public VendorInventory( BaseHouse house, Mobile owner, string vendorName, string shopName )
		{
			m_House = house;
			m_Owner = owner;
			m_VendorName = vendorName;
			m_ShopName = shopName;

			m_Items = new ArrayList();

			m_ExpireTime = DateTime.Now + GracePeriod;
			m_ExpireTimer = new ExpireTimer( this, GracePeriod );
			m_ExpireTimer.Start();
		}

		public BaseHouse House
		{
			get{ return m_House; }
			set{ m_House = value; }
		}

		public string VendorName
		{
			get{ return m_VendorName; }
			set{ m_VendorName = value; }
		}

		public string ShopName
		{
			get{ return m_ShopName; }
			set{ m_ShopName = value; }
		}

		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

		public ArrayList Items
		{
			get{ return m_Items; }
		}

		public int Copper
		{
			get{ return m_Copper; }
			set{ m_Copper = value; }
		}

		public DateTime ExpireTime
		{
			get{ return m_ExpireTime; }
		}

		public void AddItem( Item item )
		{
			item.Internalize();
			m_Items.Add( item );
		}

		public void Delete()
		{
			foreach ( Item item in Items )
			{
				item.Delete();
			}

			Items.Clear();
			Copper = 0;

			if ( House != null )
				House.VendorInventories.Remove( this );

			m_ExpireTimer.Stop();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			writer.Write( (Mobile) m_Owner );
			writer.Write( (string) m_VendorName );
			writer.Write( (string) m_ShopName );

			writer.WriteItemList( m_Items, true );
			writer.Write( (int) m_Copper );

			writer.WriteDeltaTime( m_ExpireTime );
		}

		public VendorInventory( BaseHouse house, GenericReader reader )
		{
			m_House = house;

			int version = reader.ReadEncodedInt();

			m_Owner = reader.ReadMobile();
			m_VendorName = reader.ReadString();
			m_ShopName = reader.ReadString();

			m_Items = reader.ReadItemList();
			m_Copper = reader.ReadInt();

			m_ExpireTime = reader.ReadDeltaTime();

			if ( m_Items.Count == 0 && m_Copper == 0 )
			{
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Delete ) );
			}
			else
			{
				TimeSpan delay = m_ExpireTime - DateTime.Now;
				m_ExpireTimer = new ExpireTimer( this, delay > TimeSpan.Zero ? delay : TimeSpan.Zero );
				m_ExpireTimer.Start();
			}
		}

		private class ExpireTimer : Timer
		{
			private VendorInventory m_Inventory;

			public ExpireTimer( VendorInventory inventory, TimeSpan delay ) : base( delay )
			{
				m_Inventory = inventory;

				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				BaseHouse house = m_Inventory.House;

				if ( house != null )
				{
					if ( m_Inventory.Copper > 0 )
					{
						if ( house.MovingCrate == null )
							house.MovingCrate = new MovingCrate( house );

						Banker.Deposit( house.MovingCrate, m_Inventory.Copper );
					}

					foreach ( Item item in m_Inventory.Items )
					{
						if ( !item.Deleted )
							house.DropToMovingCrate( item );
					}

					m_Inventory.Copper = 0;
					m_Inventory.Items.Clear();
				}

				m_Inventory.Delete();
			}
		}
	}
}