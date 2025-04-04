using System;
using Server;
using System.Collections;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;

namespace Server.Items
{
    public class GuildSewing : Item
    {
        [Constructable]
        public GuildSewing() : base(0x4C81)
        {
            Name = "Extraordinary Sewing Kit";
			if ( Utility.RandomBool() ){ ItemID = 0x4C80; }
			Weight = 2.0;
			Hue = 0x430;
        }

        public GuildSewing(Serial serial) : base(serial)
		{
		}

        public override void OnDoubleClick( Mobile from )
        {
			if ( from is PlayerMobile )
			{
				int canDo = 0;

				foreach ( Mobile m in this.GetMobilesInRange( 20 ) )
				{
					if ( m is TailorGuildmaster )
						++canDo;
				}
				foreach ( Item i in this.GetItemsInRange( 20 ) )
				{
					if ( i is TailorShoppe && !i.Movable )
					{
						TailorShoppe b = (TailorShoppe)i;

						if ( b.ShoppeOwner == from )
							++canDo;
					}
				}
				if ( from.Map == Map.TerMur && from.X > 1054 && from.X < 1126 && from.Y > 1907 && from.Y < 1983 ){ ++canDo; }

				PlayerMobile pc = (PlayerMobile)from;
				if ( (pc.NpcGuild != NpcGuild.TailorsGuild && from.Skills[SkillName.Tailoring].Value < 110)  )
				{
					from.SendMessage( "Only those of the Tailors Guild may use this!" );
				}
				else if ( from.Skills[SkillName.Tailoring].Value < 90 )
				{
					from.SendMessage( "Only a master tailor can use this!" );
				}
				else if ( canDo == 0 )
				{
					from.SendMessage( "You need to be near a tailor guildmaster, or a tailor shoppe you own, to use this!" );
				}
				else
				{
					from.SendMessage("Select the clothing you would like to enhance...");
					from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(OnTarget));
				}
			}
        }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		{ 
			base.GetContextMenuEntries( from, list ); 
			list.Add( new SpeechGumpEntry( from ) );
		} 

		public class SpeechGumpEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			
			public SpeechGumpEntry( Mobile from ) : base( 6121, 3 )
			{
				m_Mobile = from;
			}

			public override void OnClick()
			{
			    if( !( m_Mobile is PlayerMobile ) )
				return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;
				{
					if ( ! mobile.HasGump( typeof( SpeechGump ) ) )
					{
						mobile.SendGump(new SpeechGump( "Enhancing Items", SpeechFunctions.SpeechText( m_Mobile.Name, m_Mobile.Name, "Enhance" ) ));
					}
				}
            }
        }

        public void OnTarget(Mobile from, object obj)
        {
            if ( obj is Item )
            {
				Item item = (Item)obj;

                if (((Item)obj).RootParent != from)
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
				else if (item is BootsofHermes)
				{
					from.SendMessage( "The magic in these boots prevents them from being enhanced." );
				}
				else if ( item is ILevelable )
				{
					from.SendMessage( "You cannot enhance legendary artifacts!" );
				}
				else if ( item is BaseJewel && ( item is MagicRobe || item is MagicHat || item is MagicCloak || item is MagicBoots || MaterialInfo.IsMagicBelt(item) || MaterialInfo.IsMagicSash(item) ) )
				{
					GuildCraftingProcess process = new GuildCraftingProcess(from, (Item)obj);
					process.BeginProcess();
				}
				else if ( item is BaseWeapon && ( item is IThrowingGloves || item is IPugilistGloves ) )
				{
					GuildCraftingProcess process = new GuildCraftingProcess(from, (Item)obj);
					process.BeginProcess();
				}
				else if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

					if ( Server.Misc.MaterialInfo.IsAnyKindOfClothItem( item ) )
                    {
						GuildCraftingProcess process = new GuildCraftingProcess(from, (Item)obj);
						process.BeginProcess();
					}
					else
					{
						from.SendMessage( "You cannot enhance this item!" );
					}
				}
				else if ( item is BaseClothing )
				{
					GuildCraftingProcess process = new GuildCraftingProcess(from, (Item)obj);
					process.BeginProcess();
				}
                else
                {
					from.SendMessage( "You cannot enhance this item!" );
                }
            }
        }
        
        public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( ItemID != 0x4C80 && ItemID != 0x4C81 ){ ItemID = 0x4C81; }
		}
    }
}