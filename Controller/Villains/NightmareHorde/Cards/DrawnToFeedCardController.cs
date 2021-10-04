using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class DrawnToFeedCardController : CardController
    {
        public DrawnToFeedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DealDamageAction> bappers = new List<DealDamageAction>();
            IEnumerator bapCity = base.MultipleDamageSourcesDealDamage(new LinqCardCriteria((Card c) => c.IsVillainTarget && c.IsInPlayAndHasGameText), TargetType.LowestHP, new int?(1), new LinqCardCriteria((Card c) => !c.IsVillain), 1, DamageType.Melee, false, bappers);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(bapCity);
            }
            else
            {
                GameController.ExhaustCoroutine(bapCity);
            }
            IEnumerator healBappers = base.GameController.GainHP(this.DecisionMaker, (Card c) => (from dd in bappers
                                      where dd.DidDealDamage && dd.DamageSource.IsCard
                                      select dd.DamageSource.Card).Contains(c), 3, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(healBappers);
            }
            else
            {
                GameController.ExhaustCoroutine(healBappers);
            }
            yield break;
        }
    }
}