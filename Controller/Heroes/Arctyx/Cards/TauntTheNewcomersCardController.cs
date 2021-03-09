using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	//Whenever a non-hero target enters play, Arctyx may deal that target 1 Sonic damage.

	public class TauntTheNewcomersCardController : CardController
	{
		public TauntTheNewcomersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Non-hero target enters play, Arctyx shouts at them
			AddTargetEntersPlayTrigger((Card c) => !c.IsHero, (Card c) => DealDamage(CharacterCard, c, 1, DamageType.Sonic, optional: true, cardSource: GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
		}
	}
}
