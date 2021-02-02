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
            //...deal a target 2 fire damage
            int powerNumeral = base.GetPowerNumeral(0, 2);
            IEnumerator dealFire = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), powerNumeral, DamageType.Fire, 1, false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealFire);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealFire);
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
                case 1:
                    {
                        //Select a hero. Until the start of your next turn, whenever that hero uses a power, that player's hero deals a target 1 Fire damage.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator selectPlayer = base.GameController.SelectHeroTurnTaker(this.DecisionMaker, SelectionType.TurnTaker, false, false, storedResults, null, null, false, null, null, true, null, GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(selectPlayer);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(selectPlayer);
                        }
                        if (storedResults.Any((SelectTurnTakerDecision d) => d.Completed && d.SelectedTurnTaker != null && d.SelectedTurnTaker.IsHero))
                        {
                            TurnTaker selectedTurnTaker = base.GetSelectedTurnTaker(storedResults);
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
                                ddaupse.UntilStartOfNextTurn(this.TurnTaker);
                                IEnumerator dealDamageEffect = base.AddStatusEffect(ddaupse, true);
                                if (base.UseUnityCoroutines)
                                {
                                    yield return base.GameController.StartCoroutine(dealDamageEffect);
                                }
                                else
                                {
                                    base.GameController.ExhaustCoroutine(dealDamageEffect);
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //one hero may use a power
                        IEnumerator usePower = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, false, true, false, null, null, null, true, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(usePower);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(usePower);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}