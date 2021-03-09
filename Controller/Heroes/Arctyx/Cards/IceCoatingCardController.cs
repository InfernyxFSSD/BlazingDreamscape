using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class IceCoatingCardController : FrostCardController
    {
		//When this card enters play, destroy any other frost cards in play.
		//When Arctyx deals a target cold damage, reduce damage dealt by that target by 1 until the start of your next turn.
		//Power: Arctyx deals a target 2 cold damage.

        public IceCoatingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
			//When Arctyx deals cold, reduce damage dealt by that target by 1.
            AddTrigger<DealDamageAction>((DealDamageAction dd2) => dd2.DidDealDamage && dd2.DamageSource.IsSameCard(CharacterCard) && dd2.DamageType == DamageType.Cold, (DealDamageAction dd) => ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd, 1), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = GetPowerNumeral(0, 2);
			//Arctyx deals a target 2 cold
			IEnumerator dealCold = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeral, DamageType.Cold, 1, false, 1, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(dealCold);
			}
			else
			{
				GameController.ExhaustCoroutine(dealCold);
			}
			yield break;
		}
	}
}