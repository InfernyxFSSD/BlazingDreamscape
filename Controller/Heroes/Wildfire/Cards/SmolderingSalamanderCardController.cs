using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class SmolderingSalamanderCardController : CardController
    {
        //When this card is destroyed, you may draw a card.
        //At the end of your turn, this card deals a target 2 fire damage.

        public SmolderingSalamanderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Deal a target damage at the end of your turn
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => true, TargetType.SelectTarget, 2, DamageType.Fire);
            //You may draw a card when this card is destroyed
            AddWhenDestroyedTrigger((DestroyCardAction dc) => DrawCard(HeroTurnTaker, true), TriggerType.DrawCard);
        }
    }
}