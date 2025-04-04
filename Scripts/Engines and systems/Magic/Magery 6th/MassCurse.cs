using System;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Spells.Fourth;

namespace Server.Spells.Sixth
{
	public class MassCurseSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mass Curse", "Vas Des Sanct",
				218,
				9031,
				false,
				Reagent.Garlic,
				Reagent.Nightshade,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public MassCurseSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );

				SpellHelper.GetSurfaceTop( ref p );

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					// Sorcerers get a larger radius
					int range = (Caster is PlayerMobile) && ((PlayerMobile)Caster).Sorcerer() ? 5 : 2;
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), range );

					foreach ( Mobile m in eable )
					{
						if ( Core.AOS && m == Caster )
							continue;

						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanSee( m ) && Caster.CanBeHarmful( m, false ) )
							targets.Add( m );
					}

					eable.Free();
				}

				int spellHue = Server.Items.CharacterDatabase.GetMySpellHue(Caster, 0);
                for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = targets[i];

					Caster.DoHarmful( m );

                    CurseSpell.ApplyCurse(Caster, m, spellHue, 0x1FB);
					
					HarmfulSpell( m );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MassCurseSpell m_Owner;

			public InternalTarget( MassCurseSpell owner ) : base( Core.ML ? 10 : 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}