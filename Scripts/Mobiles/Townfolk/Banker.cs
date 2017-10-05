using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
//using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class Banker : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public Banker() : base( "the banker" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBBanker() );
		}

		public static int GetBalance( Mobile from )
		{
			Item[] gold, checks;

			return GetBalance( from, out gold, out checks );
		}

		public static int GetBalance( Mobile from, out Item[] gold, out Item[] checks )
		{
			int balance = 0;

			Container bank = from.BankBox;

			if ( bank != null )
			{
				gold = bank.FindItemsByType( typeof( Copper ) );
				checks = bank.FindItemsByType( typeof( BankCheck ) );

				for ( int i = 0; i < gold.Length; ++i )
					balance += gold[i].Amount;

				for ( int i = 0; i < checks.Length; ++i )
					balance += ((BankCheck)checks[i]).Worth;
			}
			else
			{
				gold = checks = new Item[0];
			}

			return balance;
		}

		public static bool Withdraw( Mobile from, int amount )
		{
			Item[] gold, checks;
			int balance = GetBalance( from, out gold, out checks );

			if ( balance < amount )
				return false;

			for ( int i = 0; amount > 0 && i < gold.Length; ++i )
			{
				if ( gold[i].Amount <= amount )
				{
					amount -= gold[i].Amount;
					gold[i].Delete();
				}
				else
				{
					gold[i].Amount -= amount;
					amount = 0;
				}
			}

			for ( int i = 0; amount > 0 && i < checks.Length; ++i )
			{
				BankCheck check = (BankCheck)checks[i];

				if ( check.Worth <= amount )
				{
					amount -= check.Worth;
					check.Delete();
				}
				else
				{
					check.Worth -= amount;
					amount = 0;
				}
			}

			return true;
		}

		public static bool Deposit( Mobile from, int amount )
		{
			BankBox box = from.BankBox;
			if ( box == null )
				return false;

			ArrayList items = new ArrayList();

			while ( amount > 0 )
			{
				Item item;
				if ( amount < 5000 )
				{
					item = new Copper( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
				{
					items.Add( item );
				}
				else
				{
					item.Delete();
					foreach ( Item curItem in items )
					{
						curItem.Delete();
					}

					return false;
				}
			}

			return true;
		}

		public static int DepositUpTo( Mobile from, int amount )
		{
			BankBox box = from.BankBox;
			if ( box == null )
				return 0;

			int amountLeft = amount;
			while ( amountLeft > 0 )
			{
				Item item;
				int amountGiven;

				if ( amountLeft < 5000 )
				{
					item = new Copper( amountLeft );
					amountGiven = amountLeft;
				}
				else if ( amountLeft <= 1000000 )
				{
					item = new BankCheck( amountLeft );
					amountGiven = amountLeft;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amountGiven = 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
				{
					amountLeft -= amountGiven;
				}
				else
				{
					item.Delete();
					break;
				}
			}

			return amount - amountLeft;
		}

		public static void Deposit( Container cont, int amount )
		{
			while ( amount > 0 )
			{
				Item item;

				if ( amount < 5000 )
				{
					item = new Copper( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				cont.DropItem( item );
			}
		}

		public Banker( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}
		
		public bool RaciallyCompatible( Nation nation )
		{
			if( (this.Nation == Nation.Western || this.Nation == Nation.Haluaroc) && (nation == Nation.Western || nation == Nation.Haluaroc) )
				return true;
			
			if( (this.Nation == Nation.Northern || this.Nation == Nation.Tirebladd) && (nation == Nation.Northern || nation == Nation.Tirebladd) )
				return true;
			
			if( (this.Nation == Nation.Mhordul || this.Nation == Nation.Southern) && (nation == Nation.Mhordul || nation == Nation.Southern) )
				return true;
			
			return false;
		}
		
		public bool CanUseTheBank( Mobile m )
		{
			bool canuse = true;
			
			/*PlayerMobile pm = m as PlayerMobile;
			
			Nation nation = Nation.None;
			
			if( m is PlayerMobile )
				nation = pm.GetDisguisedNation();
			
			if( !RaciallyCompatible(nation) )
				canuse = false;
			
			if( ( this.GuardBoard != null || this.AssignGuardBoard( this.Nation ) ) && m is PlayerMobile && ( this.GuardBoard.ArrestByNameList.Contains( pm.Name ) || this.GuardBoard.KillByNameList.Contains( pm.Name ) ) )
				canuse = false;
			
			if( !canuse )
				m.SendMessage( "You are either a criminal or an outlander and cannot use the bank." );*/
			
			return canuse;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.Mobile.InRange( this.Location, 4 ) && CanUseTheBank( e.Mobile ) )
			{
				for ( int i = 0; i < e.Keywords.Length; ++i )
				{
					int keyword = e.Keywords[i];

					switch ( keyword )
					{
						case 0x0000: // *withdraw*
						{
							e.Handled = true;

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

								try
								{
									amount = Convert.ToInt32( split[1] );
								}
								catch
								{
									break;
								}

								if ( amount > 5000 )
								{
									this.Say( 500381 ); // Thou canst not withdraw so much at one time!
								}
								else if ( amount > 0 )
								{
									BankBox box = e.Mobile.BankBox;

									if ( box == null || !box.ConsumeTotal( typeof( Copper ), amount ) )
									{
										this.Say( "Ah, art thou trying to fool me? Thou hast not so much copper!" ); 
									}
									else
									{
										e.Mobile.AddToBackpack( new Copper( amount ) );

										this.Say( "Thou hast withdrawn copper from thy account." );
									}
								}
							}

							break;
						}
						case 0x0001: // *balance*
						{
							e.Handled = true;

							BankBox box = e.Mobile.BankBox;

							if ( box != null )
							{
								this.Say( "Thy current bank balance is " + box.TotalGold.ToString() + " copper." );
							}

							break;
						}
						case 0x0002: // *bank*
						{
							e.Handled = true;

							BankBox box = e.Mobile.BankBox;
							
							PlayerMobile pm = e.Mobile as PlayerMobile;

							if ( box != null )
								box.Open();

							break;
						}
						case 0x0003: // *check*
						{
							e.Handled = true;

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

								try
								{
									amount = Convert.ToInt32( split[1] );
								}
								catch
								{
									break;
								}

								if ( amount < 5000 )
								{
									this.Say( "We cannot create checks for such a paltry amount of copper!" ); //
								}
								else if ( amount > 1000000 )
								{
									this.Say( 1010007 ); // Our policies prevent us from creating checks worth that much!
								}
								else
								{
									BankCheck check = new BankCheck( amount );

									BankBox box = e.Mobile.BankBox;

									if ( box == null || !box.TryDropItem( e.Mobile, check, false ) )
									{
										this.Say( 500386 ); // There's not enough room in your bankbox for the check!
										check.Delete();
									}
									else if ( !box.ConsumeTotal( typeof( Copper ), amount ) )
									{
										this.Say( "Ah, art thou trying to fool me? Thou hast not so much copper!" ); // 
										check.Delete();
									}
									else
									{
										this.Say( 1042673, AffixType.Append, amount.ToString(), "" ); // Into your bank box I have placed a check in the amount of:
									}
								}
							}

							break;
						}
					}
				}
			}

			base.OnSpeech( e );
		}

		/*public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
				list.Add( new OpenBankEntry( from, this ) );

			base.AddCustomContextEntries( from, list );
		}*/

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

            writer.Write((int)0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
