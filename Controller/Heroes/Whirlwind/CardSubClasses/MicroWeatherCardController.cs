using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public abstract class MicroWeatherCardController : CardController
	{
		public MicroWeatherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//How many Weather Effects are in play?
			SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("weather effect"), "weather effect", false, singular: "weather effect", plural: "weather effects"));
		}
	}
}