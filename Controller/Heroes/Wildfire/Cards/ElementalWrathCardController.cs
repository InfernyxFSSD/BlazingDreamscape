using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ElementalWrathCardController : CardController
	{
		//Each elemental in your play area deals a target 1 fire damage.

		public ElementalWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//How many elementals are in play?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.PlayArea, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"));
		}
		public override IEnumerator Play()
		{
			//Each elemental in your play area deals a target 1 Fire damage.
			IEnumerator elemPoke = GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("elemental") && c.Location.IsPlayAreaOf(TurnTaker), "elemental in your play area", false), SelectionType.CardToDealDamage, (Card c) => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, c), 1, DamageType.Fire, new int?(1), false, new int?(1), cardSource: GetCardSource()), cardSource: GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(elemPoke);
			}
			else
			{
				GameController.ExhaustCoroutine(elemPoke);
			}
			yield break;
		}
	}
}