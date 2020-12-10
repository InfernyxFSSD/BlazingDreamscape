using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
	// Token: 0x020007D6 RID: 2006
	public class DragonHideCardController : CardController
	{
		// Token: 0x06003449 RID: 13385 RVA: 0x0002422C File Offset: 0x0002242C
		public DragonHideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		// Token: 0x0600344A RID: 13386 RVA: 0x0007D102 File Offset: 0x0007B302
		public override void AddTriggers()
		{
			base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
		}
	}
}
