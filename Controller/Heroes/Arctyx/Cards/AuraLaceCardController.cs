using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class AuraLaceCardController : CardController
    {
        public AuraLaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsTarget && dd.Target == base.CharacterCard && dd.DidDealDamage && dd.Amount > 0 && base.FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("aura", false, false), false, null, false).Count<Card>() > 0, new Func<DealDamageAction, IEnumerator>(this.AddStatusEffectResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 2);
            IEnumerator coroutine = base.GameController.SelectAndDiscardCards(base.HeroTurnTakerController, powerNumeral, false, null, null, false, null, null, null, null, SelectionType.DiscardCard, null, this.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.SearchForCards(base.HeroTurnTakerController, true, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), true, false, false, false, null, false, null, null);
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> list = new List<Card>();
            list.AddRange(base.TurnTaker.Trash.Cards);
            base.AddInhibitorException((GameAction ga) => ga is ShuffleCardsAction);
            coroutine = base.GameController.ShuffleCardsIntoLocation(this.DecisionMaker, list, base.TurnTaker.Deck, true, base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator AddStatusEffectResponse(DealDamageAction dd)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = this.CharacterCard;
            increaseDamageStatusEffect.TargetCriteria.IsSpecificCard = dd.DamageSource.Card;
            increaseDamageStatusEffect.NumberOfUses = 1;
            IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
            if(UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}