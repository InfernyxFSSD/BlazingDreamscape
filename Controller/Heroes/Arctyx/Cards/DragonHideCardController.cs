using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class DragonHideCardController : CardController
	{
		//Reduce damage dealt to Arctyx by 1.

		public DragonHideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
		}
	}
}
