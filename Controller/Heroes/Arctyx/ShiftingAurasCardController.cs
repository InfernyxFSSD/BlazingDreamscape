using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class ShiftingAurasCardController : CardController
    {
        public ShiftingAurasCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            WhenCardIsDestroyedStatusEffect whenCardIsDestroyedStatusEffect = new WhenCardIsDestroyedStatusEffect(base.CardWithoutReplacements, "DealFireDamageResponse", string.Concat(new string[]
            {
                "Whenever a Flame Aura is destroyed, ",
                base.Card.Title,
                " deals a target 1 Fire damage."
            }), new TriggerType[]
            {
                TriggerType.DealDamage
            }, base.HeroTurnTaker, base.Card, null);
            whenCardIsDestroyedStatusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords.Any((string s) => Card.HasGameText && Card.DoKeywordsContain("flame", false, false));
            whenCardIsDestroyedStatusEffect.CanEffectStack = true;
            whenCardIsDestroyedStatusEffect.UntilStartOfNextTurn(base.GameController.FindNextTurnTaker());
            whenCardIsDestroyedStatusEffect.DoesDealDamage = true;
            IEnumerator coroutine = base.AddStatusEffect(whenCardIsDestroyedStatusEffect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            WhenCardIsDestroyedStatusEffect whenCardIsDestroyedStatusEffect2 = new WhenCardIsDestroyedStatusEffect(base.CardWithoutReplacements, "DealColdDamageResponse", string.Concat(new string[]
            {
                "Whenever a Frost Aura is destroyed, ",
                base.Card.Title,
                " deals a target 1 Cold damage."
            }), new TriggerType[]
            {
                TriggerType.DealDamage
            }, base.HeroTurnTaker, base.Card, null);
            whenCardIsDestroyedStatusEffect2.CardDestroyedCriteria.HasAnyOfTheseKeywords.Any((string s) => Card.HasGameText && Card.DoKeywordsContain("frost", false, false));
            whenCardIsDestroyedStatusEffect2.CanEffectStack = true;
            whenCardIsDestroyedStatusEffect2.UntilStartOfNextTurn(base.GameController.FindNextTurnTaker());
            whenCardIsDestroyedStatusEffect2.DoesDealDamage = true;
            IEnumerator coroutine2 = base.AddStatusEffect(whenCardIsDestroyedStatusEffect2, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            IEnumerable<Card> source = from c in base.HeroTurnTaker.Hand.Cards
                                       where c.DoKeywordsContain("aura", false, false)
                                       select c;
            IEnumerator coroutine3 = base.SelectAndPlayCardsFromHand(this.DecisionMaker, source.Count<Card>(), false, new int?(0), new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), false, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }

        public IEnumerator DealFireDamageResponse(HeroTurnTaker hero, StatusEffect effect)
        {
            Card card = base.Card;
            if (hero != null && hero.IsHero)
            {
                card = hero.CharacterCard;
            }
            HeroTurnTakerController heroTurnTakerController = base.FindHeroTurnTakerController(hero);
            if (heroTurnTakerController == null)
            {
                heroTurnTakerController = this.DecisionMaker;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(heroTurnTakerController, new DamageSource(base.GameController, card), 1, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(effect));
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

        public IEnumerator DealColdDamageResponse(HeroTurnTaker hero, StatusEffect effect)
        {
            Card card = base.Card;
            if (hero != null && hero.IsHero)
            {
                card = hero.CharacterCard;
            }
            HeroTurnTakerController heroTurnTakerController = base.FindHeroTurnTakerController(hero);
            if (heroTurnTakerController == null)
            {
                heroTurnTakerController = this.DecisionMaker;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(heroTurnTakerController, new DamageSource(base.GameController, card), 1, DamageType.Cold, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(effect));
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