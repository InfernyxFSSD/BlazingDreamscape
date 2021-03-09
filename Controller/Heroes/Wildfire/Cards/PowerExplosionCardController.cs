using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class PowerExplosionCardController : CardController
    {
        //The first time you use a power on an equipment each turn, Wildfire may deal herself 2 fire damage. If Wildfire took damage this way, you may use a power now.
        //Power: Discard two cards. Search your deck for an equipment and put it into play. Shuffle your deck afterwards.

        public PowerExplosionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //Have you used an equipment power this turn?
            SpecialStringMaker.ShowHasBeenUsedThisTurn(UsedEquipPower);
        }

        private const string UsedEquipPower = "UsedEquipPower";

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard two cards.
            IEnumerator discardTwo = GameController.SelectAndDiscardCards(DecisionMaker, 2, false, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardTwo);
            }
            else
            {
                GameController.ExhaustCoroutine(discardTwo);
            }
            //Search your deck for an equipment and shuffle your deck afterwards
            IEnumerator searchDeck = SearchForCards(DecisionMaker, true, false, 1, 1, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), true, false, false, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(searchDeck);
            }
            else
            {
                GameController.ExhaustCoroutine(searchDeck);
            }
            yield break;
        }
        public override void AddTriggers()
        {
            //When you use a power on an equipment for the first time each turn, maybe hit yourself for more power
            AddTrigger<UsePowerAction>((UsePowerAction p) => !IsPropertyTrue("UsedEquipPower", null) && p.Power.TurnTakerController == TurnTakerController && IsEquipment(p.Power.CardSource.Card), new Func<UsePowerAction, IEnumerator>(DamageSelfToUsePowerResponse), new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
        }

        private IEnumerator DamageSelfToUsePowerResponse(UsePowerAction p)
        {
            //Whether you decide to hit yourself or not, it only checks on first equipment power use
            SetCardPropertyToTrueIfRealAction("UsedEquipPower", null);
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            //Decide whether to hit yourself
            IEnumerator hitSelf = GameController.DealDamageToSelf(DecisionMaker, (Card c) => c == CharacterCard, 2, DamageType.Fire, storedResults: storedResults, isOptional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitSelf);
            }
            else
            {
                GameController.ExhaustCoroutine(hitSelf);
            }
            if (DidDealDamage(storedResults, CharacterCard))
            {
                //If you did hit yourself, use another power
                IEnumerator usePower = GameController.SelectAndUsePower(DecisionMaker, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(usePower);
                }
                else
                {
                    GameController.ExhaustCoroutine(usePower);
                }
            }
            yield break;
        }
    }
}
