using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
	// Token: 0x02000650 RID: 1616
	public abstract class FlameCardController : CardController
	{
		// Token: 0x06002D85 RID: 11653 RVA: 0x0002422C File Offset: 0x0002242C
		public FlameCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		// Token: 0x06002D86 RID: 11654 RVA: 0x00070AA1 File Offset: 0x0006ECA1
		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c != base.Card && c.DoKeywordsContain("flame", false, false), "flame", true, false, null, null, false), false, null, null, null, SelectionType.DestroyCard, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}
	}
}
