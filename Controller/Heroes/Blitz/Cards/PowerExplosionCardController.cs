using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace SybithosInfernyx.Blitz
{
    public class PowerExplosionCardController : CardController
    {
        public PowerExplosionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const string UsedFocusPower = "UsedFocusPower";
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator discardTwo = base.GameController.SelectAndDiscardCards(this.DecisionMaker, 2, false, 2, null, false, null, null, null, null, SelectionType.DiscardCard, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardTwo);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardTwo);
            }
            IEnumerator searchDeck = base.SearchForCards(this.DecisionMaker, true, false, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("focus", false, false), "focus", true, false, null, null, false), true, false, false, false, null, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(searchDeck);
            }
            else
            {
                base.GameController.ExhaustCoroutine(searchDeck);
            }
            yield break;
        }
        public override void AddTriggers()
        {
            base.AddTrigger<UsePowerAction>((UsePowerAction p) => !base.IsPropertyTrue("UsedFocusPower", null) && p.Power.TurnTakerController == this.TurnTakerController && p.Power.CardSource.Card.DoKeywordsContain("focus", false, false), new Func<UsePowerAction, IEnumerator>(this.DamageSelfToUsePowerResponse), new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
        }

        private IEnumerator DamageSelfToUsePowerResponse(UsePowerAction p)
        {
            base.SetCardPropertyToTrueIfRealAction("UsedFocusPower", null);
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator hitSelf = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Fire, 1, true, 1, false, false, false, (Card c) => c == base.CharacterCard, null, storedResults, null, null, false, null, null, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(hitSelf);
            }
            else
            {
                base.GameController.ExhaustCoroutine(hitSelf);
            }
            if (base.DidDealDamage(storedResults, base.CharacterCard, null))
            {
                IEnumerator usePower = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, true, null, 1, true, null, false, false, true, true, null, false, false, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(usePower);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(usePower);
                }
            }
            yield break;
        }
    }
}
