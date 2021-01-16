using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
    public class RubyCardController : CardController
    {
        public RubyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && this.Card.IsInPlayAndHasGameText, (PhaseChangeAction p) => this.StartOfTurnResponse(), new TriggerType[] { TriggerType.DestroyCard }, null, false);
        }

        private IEnumerator StartOfTurnResponse()
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator mayDealDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Fire, 1, false, 0, false, false, false, null, null, storedResults, null, null, false, null, null, false, null, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(mayDealDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(mayDealDamage);
            }
            DealDamageAction dealDamageAction = storedResults.FirstOrDefault<DealDamageAction>();
            if (dealDamageAction != null && dealDamageAction.DidDealDamage)
            {
                IEnumerator destroyThisCard = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(destroyThisCard);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(destroyThisCard);
                }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            Func<Card, IEnumerator> actionWithCard = (Card c) => base.GameController.ShuffleCardIntoLocation(this.DecisionMaker, c, this.TurnTaker.Deck, false, false, GetCardSource(null));
            IEnumerator selectConduit = base.GameController.SelectCardsAndDoAction(base.DecisionMaker, new LinqCardCriteria((Card c) => c.Location == this.TurnTaker.Trash && c.DoKeywordsContain("conduit", false, false), "conduit", true, false, null, null, false), SelectionType.ShuffleCardFromTrashIntoDeck, actionWithCard, 1, false, 1, null, false, null, null, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectConduit);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectConduit);
            }
            yield break;
        }
    }
}
