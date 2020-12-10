using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
	// Token: 0x0200064C RID: 1612
	public class TauntTheNewcomersCardController : CardController
	{
		// Token: 0x06002D73 RID: 11635 RVA: 0x0002422C File Offset: 0x0002242C
		public TauntTheNewcomersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		// Token: 0x06002D74 RID: 11636 RVA: 0x000708A5 File Offset: 0x0006EAA5
		public override void AddTriggers()
		{
			base.AddTargetEntersPlayTrigger((Card c) => !c.IsHero, (Card c) => base.DealDamage(base.CharacterCard, c, 1, DamageType.Sonic, false, true, false, null, null, null, false, null), TriggerType.DealDamage, TriggerTiming.After, false, false);
		}
	}
}
