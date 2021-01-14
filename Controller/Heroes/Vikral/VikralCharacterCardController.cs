using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class VikralCharacterCardController : HeroCharacterCardController
    {
        public VikralCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                {
                    List<Card> selectedTargets = new List<Card>();
                    for (int i = 0; i < 2; i++)
                    {
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerable<Card> choices = base.FindCardsWhere((Card c) => !c.IsHero && c.IsInPlay && c.IsTarget && !selectedTargets.Contains(c), false, null, false);
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTarget, choices, storedResults, false, false, null, null, null, null, null, false, false, null);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Card card = (from d in storedResults select d.SelectedCard).FirstOrDefault<Card>();
                        if (card != null)
                        {
                            selectedTargets.Add(card);
                            coroutine = base.DealDamage(card, card, 1, DamageType.Radiant, false, false, false, null, null, null, false, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        else
                        {
                            yield break;
                        }
                    }
                    break;
                }
                case 1:
                {
                    IEnumerator coroutine3 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, (CardSource)GetCardSource());
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
                //Until the start of your next turn, whenever a target is destroyed, one target deals itself 2 Radiant damage.
                case 2:
                {
                        WhenCardIsDestroyedStatusEffect wcidse = new WhenCardIsDestroyedStatusEffect(base.CardWithoutReplacements, nameof(WhenCardIsDestroyedChooseTargetToDealSelfDamage), "Until the start of your next turn, whenever a target is destroyed, one target deals itself 2 Radiant damage.", new TriggerType[] { TriggerType.DealDamage }, base.TurnTaker.ToHero(), base.Card, null);
                        wcidse.CardDestroyedCriteria.IsTarget = true;
                        wcidse.UntilStartOfNextTurn(base.TurnTaker);
                        wcidse.DoesDealDamage = true;
                        IEnumerator coroutine = base.AddStatusEffect(wcidse, true);
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
            }
        }

        public IEnumerator WhenCardIsDestroyedChooseTargetToDealSelfDamage(DestroyCardAction _, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            //Select a target
            IEnumerator hitSelf = base.GameController.SelectTargetsToDealDamageToSelf(this.DecisionMaker, 2, DamageType.Radiant, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(hitSelf);
            }
            else
            {
                base.GameController.ExhaustCoroutine(hitSelf);
            }
        }


        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator e = DrawCards(this.HeroTurnTakerController, 1);
            if (UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(e);
            }
            else
            {
                this.GameController.ExhaustCoroutine(e);
            }
            List<SelectCardDecision> storedResultsMicrostorm = new List<SelectCardDecision>();
            IEnumerator e2 = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.RemoveTokens, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm", false, false) && c.IsInPlayAndHasGameText, "microstorm", false, false, null, null, false), storedResultsMicrostorm, true, false, null, true, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(e2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(e2);
            }
            Card selectedMicrostorm = storedResultsMicrostorm.FirstOrDefault()?.SelectedCard;
            if (selectedMicrostorm != null)
            {
                Card card = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == selectedMicrostorm.Identifier).FirstOrDefault();
                string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
                TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
                e2 = base.GameController.RemoveTokensFromPool(torrentPool, 1, null, false, null, null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(e2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(e2);
                }
                IEnumerator drawCard = DrawCards(this.HeroTurnTakerController, 1);
                if (UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(drawCard);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(drawCard);
                }
            }
        }
    }
}