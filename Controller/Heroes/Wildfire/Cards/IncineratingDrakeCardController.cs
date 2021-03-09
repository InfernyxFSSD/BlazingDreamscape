using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class IncineratingDrakeCardController : CardController
    {
        //When this card is destroyed, you may put an elemental from your trash into play.
        //X on this card is equal to the number of elementals in your play area when this card entered play plus 1.
        //At the end of your turn, this card deals a target X fire damage.

        public IncineratingDrakeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How much damage will this card deal?
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} will deal {(int)GetCardPropertyJournalEntryInteger(DamageToDeal)} Fire damage.").Condition = (() => Card.IsInPlayAndNotUnderCard);
        }

        public override void AddTriggers()
        {
            //Find the amount of damage that's supposed to be dealt
            int? damageToDeal(Card target) => new int?(GetValueOfDamageDealt());
            //Deal that damage at the end of your turn
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => true, TargetType.SelectTarget, 0, DamageType.Fire, dynamicAmount: damageToDeal);
            //When this card is destroyed, get an elemental from your trash
            AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(OnDestroyResponse), new TriggerType[] { TriggerType.PutIntoPlay });
            //When this card leaves play, reset the flag that determines how much damage to deal
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay("IncineratingDrakeDamageToDeal"), TriggerType.Hidden);
        }

        private int GetValueOfDamageDealt()
        {
            //fetch the damage amount
            return (int)GetCardPropertyJournalEntryInteger(DamageToDeal);
        }

        public override IEnumerator Play()
        {
            //Set the amount of damage that should be dealt
            int damageToDeal = FindCardsWhere((Card c) => c.DoKeywordsContain("elemental") && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(TurnTaker)).Count<Card>() + 1;
            SetCardProperty(DamageToDeal, damageToDeal);
            yield break;
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dc)
        {
            //When this is destroyed, find an elemental to play
            IEnumerator findElem = SearchForCards(DecisionMaker, false, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"), true, false, false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findElem);
            }
            else
            {
                GameController.ExhaustCoroutine(findElem);
            }
            yield break;
        }

        private const string DamageToDeal = "IncineratingDrakeDamageToDeal";
    }
}