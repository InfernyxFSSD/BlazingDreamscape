using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class HeroicInterventionCardController : CardController
	{
		//May redirect damage dealt to a hero by a non-hero to Cyberknight

		public HeroicInterventionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddRedirectDamageTrigger((DealDamageAction dda) => dda.Target.IsHero && dda.Target != base.CharacterCard && !dda.DamageSource.IsHero, () => base.CharacterCard, optional: true);
		}
	}
}