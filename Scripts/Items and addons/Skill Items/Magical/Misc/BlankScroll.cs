using System;

namespace Server.Items
{
	public class BlankScroll : Item, ICommodity
	{
		[Constructable]
		public BlankScroll() : this( 1 )
		{
		}

		[Constructable]
		public BlankScroll( int amount ) : base( 0xEF3 )
		{
			Name = "blank scroll";
			Stackable = true;
			Weight = 0.1;
			Amount = amount;
		}

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return (Core.ML); } }

		public BlankScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			if (version == 0)
				Weight = 0.1;
		}
	}
}