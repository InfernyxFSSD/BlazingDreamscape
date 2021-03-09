using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class InfernoWispCardController : CardController
    {
        //The first time each turn that Wildfire deals a target 2 or more fire damage at once, increase the next damage dealt by this card by 1.
        //At the end of your turn, this card deals a target 1 fire damage.

        public InfernoWispCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const string WildfireDealtEnoughDamage = "WildfireDealtEnoughDamage";

        public override void AddTriggers()
        {
            //Deal damage at the end of your turn
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => true, TargetType.SelectTarget, 1, DamageType.Fire);
            //Increase this cards next damage dealt the first time Wildfire deals enough damage each turn
            AddTrigger<DealDamageAction>((DealDamageAction dd) => !IsPropertyTrue(WildfireDealtEnoughDamage) && dd.DamageSource.IsSameCard(CharacterCard) && dd.Amount >= 2, new Func<DealDamageAction, IEnumerator>(IncreaseNextDamageResponse), new TriggerType[] { TriggerType.IncreaseDamage }, TriggerTiming.After);
        }

        private IEnumerator IncreaseNextDamageResponse(DealDamageAction _)
        {
            //When Wildfire first deals enough damage, increase the next damage from this card
            SetCardPropertyToTrueIfRealAction(WildfireDealtEnoughDamage);
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = Card;
            idse.NumberOfUses = 1;
            IEnumerator applyStatus = AddStatusEffect(idse);
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
}