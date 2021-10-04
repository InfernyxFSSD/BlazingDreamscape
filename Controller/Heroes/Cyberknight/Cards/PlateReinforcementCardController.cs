using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class PlateReinforcementCardController : CardController
	{
		//Reduce damage dealt to Cyberknight by 1.

		public PlateReinforcementCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
		}
	}
}