using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class TauntTheNewcomersCardController : CardController
	{
		public TauntTheNewcomersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddTargetEntersPlayTrigger((Card c) => !c.IsHero, (Card c) => base.DealDamage(base.CharacterCard, c, 1, DamageType.Sonic, false, true, false, null, null, null, false, null), TriggerType.DealDamage, TriggerTiming.After, false, false);
		}
	}
}
