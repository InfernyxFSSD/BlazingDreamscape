using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
    public class FlamecallerBlitzCharacterCardController : HeroCharacterCardController
    {
        public FlamecallerBlitzCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //search deck for an elemental, put it into play, shuffle deck afterwards
            IEnumerator findElemental = base.SearchForCards(base.HeroTurnTakerController, true, false, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), true, false, false, false, null, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findElemental);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findElemental);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //one player may discard hand, then draw discarded amount + 1
                        List<SelectTurnTakerDecision> sttd = new List<SelectTurnTakerDecision>();
                        List<DiscardCardAction> dca = new List<DiscardCardAction>();
                        IEnumerator selectPlayer = base.GameController.SelectHeroToDiscardTheirHand(this.DecisionMaker, false, true, false, null, sttd, dca, null, GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(selectPlayer);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(selectPlayer);
                        }
                        int X = base.GetNumberOfCardsDiscarded(dca);
                        if (base.DidDiscardHand(sttd))
                        {
                            SelectTurnTakerDecision sttd2 = sttd.FirstOrDefault<SelectTurnTakerDecision>();
                            IEnumerator drawCards = base.GameController.DrawCards(base.FindHeroTurnTakerController(sttd2.SelectedTurnTaker.ToHero()), X + 1, false, false, null, true, null, null);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(drawCards);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(drawCards);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        //one hero deals a target 3 fire
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator dealFire = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay && c.IsHeroCharacterCard, "hero", true, false, null, null, false), storedResults, false, false, null, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(dealFire);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(dealFire);
                        }
                        Card selectedCard = base.GetSelectedCard(storedResults);
                        dealFire = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, selectedCard), 3, DamageType.Fire, 1, false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(dealFire);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(dealFire);
                        }
                        break;
                    }
                case 2:
                    {
                        //the environment deals each non-hero 1 irreducible fire
                        var nonHeroTargets = GameController.FindCardsWhere(new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText && !c.IsHero));
                        foreach (Card c in nonHeroTargets)
                        {
                            IEnumerator envBurns = base.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.FindEnvironment(null).TurnTaker), c, 1, DamageType.Fire, true, false, null, null, null, null, false, false, GetCardSource(null));
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(envBurns);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(envBurns);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
