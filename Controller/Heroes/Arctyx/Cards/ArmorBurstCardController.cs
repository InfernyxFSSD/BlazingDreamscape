using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class ArmorBurstCardController : CardController
    {

        //Destroy all Aura cards in your play area. X on this card is the number of Auras destroyed this way.
        //Choose Fire or Cold. Arctyx deals up to X targets 6 damage of that type each.
        //Until the start of your next turn, increase damage dealt to Arctyx by X.

        public ArmorBurstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Auras are in your play area?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.PlayArea, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura") && c.Location == TurnTaker.PlayArea, "auras in your play area"));
        }

        public override IEnumerator Play()
        {
            //Destroy all Auras in your play area...
            IEnumerator breakAuras = DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(this.DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura") && c.Location == TurnTaker.PlayArea, "auras in your play area"), new Func<int, IEnumerator>(this.DealDamageResponseAndIncreaseDamageTaken));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(breakAuras);
            }
            else
            {
                GameController.ExhaustCoroutine(breakAuras);
            }
            yield break;
        }

        private IEnumerator DealDamageResponseAndIncreaseDamageTaken(int X)
        {
            if (X > 0)
            {
                //...select Fire or Cold and deal X targets 6 damage of that type
                List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
                IEnumerator chooseDamageType = GameController.SelectDamageType(HeroTurnTakerController, storedResults, new DamageType[]
                {
                    DamageType.Fire,
                    DamageType.Cold
                }, null, SelectionType.DamageType, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(chooseDamageType);
                }
                else
                {
                    GameController.ExhaustCoroutine(chooseDamageType);
                }
                DamageType value = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;
                IEnumerator dealAuraDamage = GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, CharacterCard), 6, value, new int?(X), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(dealAuraDamage);
                }
                else
                {
                    GameController.ExhaustCoroutine(dealAuraDamage);
                }
                //increase damage dealt to Arctyx by X until the start of your next turn
                IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(X);
                increaseDamageStatusEffect.TargetCriteria.IsSpecificCard = CharacterCard;
                increaseDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
                IEnumerator increaseDamageTaken = AddStatusEffect(increaseDamageStatusEffect, true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(increaseDamageTaken);
                }
                else
                {
                    GameController.ExhaustCoroutine(increaseDamageTaken);
                }
                yield break;
            }
            else
            {
                yield break;
            }
        }
    }
}
