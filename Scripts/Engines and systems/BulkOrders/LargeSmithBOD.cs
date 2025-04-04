using System;
using System.Collections;
using Server;
using Server.Items;
using Mat = Server.Engines.BulkOrders.BulkMaterialType;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
	[TypeAlias( "Scripts.Engines.BulkOrders.LargeSmithBOD" )]
	public class LargeSmithBOD : LargeBOD
	{
		public override int ComputeFame()
		{
			return SmithRewardCalculator.Instance.ComputeFame( this );
		}

		public override int ComputeGold()
		{
			return SmithRewardCalculator.Instance.ComputeGold( this );
		}

		[Constructable]
        public LargeSmithBOD() : this(false)
		{
		}

        [Constructable]
        public LargeSmithBOD(bool useMaterials = false)
		{
			LargeBulkEntry[] entries;
			
			int rand = Utility.Random( 8 );

			switch ( rand )
			{
				default:
				case  0: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeRing ); 	 break;
				case  1: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargePlate );	 break;
				case  2: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeChain );	 break;
				case  3: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeAxes );		 break;
				case  4: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeFencing );	 break;
				case  5: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeMaces );	 break;
				case  6: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargePolearms );	 break;
				case  7: entries = LargeBulkEntry.ConvertEntries( this, LargeBulkEntry.LargeSwords );	 break;
			}

			int hue = 0x44E;
			int amountMax = Utility.RandomList( 10, 15, 20, 20 );
			bool reqExceptional = ( 0.825 > Utility.RandomDouble() );

			BulkMaterialType material;

			if ( useMaterials )
			{
				material = GetRandomMaterial( BulkMaterialType.DullCopper, BulkMaterialType.Dwarven );
			}
			else
				material = BulkMaterialType.None;

			this.Hue = hue;
			this.AmountMax = amountMax;
			this.Entries = entries;
			this.RequireExceptional = reqExceptional;
			this.Material = material;
		}

		public LargeSmithBOD( int amountMax, bool reqExceptional, BulkMaterialType mat, LargeBulkEntry[] entries )
		{
			this.Hue = 0x44E;
			this.AmountMax = amountMax;
			this.Entries = entries;
			this.RequireExceptional = reqExceptional;
			this.Material = mat;
		}

		public override List<Item> ComputeRewards( bool full )
		{
			List<Item> list = new List<Item>();

			RewardGroup rewardGroup = SmithRewardCalculator.Instance.LookupRewards( SmithRewardCalculator.Instance.ComputePoints( this ) );

			if ( rewardGroup != null )
			{
				if ( full )
				{
					for ( int i = 0; i < rewardGroup.Items.Length; ++i )
					{
						Item item = rewardGroup.Items[i].Construct();

						if ( item != null )
							list.Add( item );
					}
				}
				else
				{
					RewardItem rewardItem = rewardGroup.AcquireItem();

					if ( rewardItem != null )
					{
						Item item = rewardItem.Construct();

						if ( item != null )
							list.Add( item );
					}
				}
			}

			return list;
		}

		public LargeSmithBOD( Serial serial ) : base( serial )
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