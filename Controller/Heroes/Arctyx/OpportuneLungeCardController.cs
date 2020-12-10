using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
	// Token: 0x020007CF RID: 1999
	public class OpportuneLungeCardController : CardController
	{
		// Token: 0x06003437 RID: 13367 RVA: 0x0002422C File Offset: 0x0002242C
		public OpportuneLungeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		// Token: 0x06003438 RID: 13368 RVA: 0x0007D011 File Offset: 0x0007B211
		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			IEnumerator e2 = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, null, false, false, true, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(e2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(e2);
			}
			yield break;
		}
	}
}
