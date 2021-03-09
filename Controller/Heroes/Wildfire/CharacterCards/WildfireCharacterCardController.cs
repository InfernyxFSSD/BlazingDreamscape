using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class WildfireCharacterCardController : HeroCharacterCardController
    {
        public WildfireCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Wildfire deals a target 2 fire damage
            int powerNumeral = GetPowerNumeral(0, 2);
            IEnumerator dealFire = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), powerNumeral, DamageType.Fire, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(dealFire);
            }
            else
            {
                GameController.ExhaustCoroutine(dealFire);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //one hero deals a target 3 fire damage
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator selectHero = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay && c.IsHeroCharacterCard, "hero", false), storedResults, false, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectHero);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectHero);
                        }
                        Card selectedCard = GetSelectedCard(storedResults);
                        IEnumerator dealFire = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, selectedCard), 3, DamageType.Fire, 1, false, 1, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(dealFire);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(dealFire);
                        }
                        break;
                    }
                case 1:
                    {
                        //Select a hero. Until the start of your next turn, whenever that hero uses a power, that player's hero deals a target 1 Fire damage.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        //First, select a hero
                        IEnumerator selectPlayer = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.TurnTaker, false, false, storedResults, canBeCancelled: false, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectPlayer);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectPlayer);
                        }
                        if (storedResults.Any((SelectTurnTakerDecision d) => d.Completed && d.SelectedTurnTaker != null && d.SelectedTurnTaker.IsHero))
                        {
                            //If a hero was successfully selected, they deal fire damage after each power
                            TurnTaker selectedTurnTaker = GetSelectedTurnTaker(storedResults);
                            if (selectedTurnTaker.IsHero)
                            {
                                HeroTurnTaker htt = selectedTurnTaker.ToHero();
                                Card damageSource;
                                if (htt.HasMultipleCharacterCards)
                                {
                                    damageSource = null;
                                }
                                else
                                {
                                    damageSource = htt.CharacterCard;
                                }
                                DealDamageAfterUsePowerStatusEffect ddaupse = new DealDamageAfterUsePowerStatusEffect(htt, damageSource, null, 1, DamageType.Fire, 1, false);
                                ddaupse.TurnTakerCriteria.IsSpecificTurnTaker = htt;
                                if (!htt.HasMultipleCharacterCards)
                                {
                                    ddaupse.CardDestroyedExpiryCriteria.Card = htt.CharacterCard;
                                }
                                ddaupse.UntilStartOfNextTurn(TurnTaker);
                                IEnumerator applyStatus = AddStatusEffect(ddaupse, true);
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(applyStatus);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(applyStatus);
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //one hero may use a power
                        IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(usePower);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(usePower);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}