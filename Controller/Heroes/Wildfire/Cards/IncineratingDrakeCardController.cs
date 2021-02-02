using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class IncineratingDrakeCardController : CardController
    {
        public IncineratingDrakeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.SelectTarget, base.FindCardsWhere((Card c) => c.DoKeywordsContain("elemental", false, false) && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(this.TurnTaker), false, null, false).Count<Card>() + 1, DamageType.Fire, false, false, 1, 1, null, null);
            base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), new TriggerType[] { TriggerType.PutIntoPlay }, null, null);
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dc)
        {
            IEnumerator findElem = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), true, false, false, false, null, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findElem);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findElem);
            }
            yield break;
        }
    }
}