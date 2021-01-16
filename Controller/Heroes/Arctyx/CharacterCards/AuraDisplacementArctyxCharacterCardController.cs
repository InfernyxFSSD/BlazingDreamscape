using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class AuraDisplacementArctyxCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public AuraDisplacementArctyxCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						IEnumerator coroutine = base.SelectHeroToPlayCard(this.DecisionMaker, false, true, false, null, null, null, false, true);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(this.DecisionMaker, false, true, false, null, null, null, true, true, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}
						break;
					}
				case 2:
					{
						IEnumerator coroutine3 = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, false, true, false, null, null, null, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
			}
			yield break;
		}

        public override IEnumerator UsePower(int index = 0)
        {
			List<MoveCardAction> storedResults = new List<MoveCardAction>();
			IEnumerator coroutine = base.DiscardCardsFromTopOfDeck(base.TurnTakerController, 1, false, storedResults, true, base.TurnTaker);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			foreach (MoveCardAction moveCardAction in storedResults)
            {
				if (moveCardAction.CardToMove.DoKeywordsContain("aura", false, false))
                {
					coroutine = base.SelectHeroToPlayCard(this.DecisionMaker, false, true, false, null, null, new LinqTurnTakerCriteria((TurnTaker h) => h != base.TurnTaker, () => "a hero other than " + base.TurnTaker.Name), false, true);
				}
				else
                {
					coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, false, true, false, null, null, new int?(1), base.GetCardSource(null));
				}
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
}