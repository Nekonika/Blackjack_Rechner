using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blackjack_Rechner
{
    public class CountingDeck : Deck
    {
        private List<string> _PulledCards = new List<string>();
        public ReadOnlyCollection<string> PulledCards => new ReadOnlyCollection<string>(_PulledCards);

        private List<string> _HighCards = new List<string>();
        public ReadOnlyCollection<string> HighCards => new ReadOnlyCollection<string>(_HighCards);

        private List<string> _LowCards = new List<string>();
        public ReadOnlyCollection<string> LowCards => new ReadOnlyCollection<string>(_LowCards);

        public CountingDeck(int deckCount) : base(deckCount) { }

        public static new CountingDeck Empty = new CountingDeck(0);

        public override string Pull()
        {
            string Card = base.Pull();

            _PulledCards.Add(Card);

            int Value = CardValues.IndexOf(CardValues.First(x => x.ID == GetCardsValues(new List<string>() { Card }).Single())) + 2;
            if (Value < 7) _LowCards.Add(Card);
            else if (Value > 9) _HighCards.Add(Card);

            return Card;
        }

        public override void New()
        {
            base.New();

            _PulledCards = new List<string>();
            _HighCards = new List<string>();
            _LowCards = new List<string>();
        }

        public void SetCards(List<string> cards, List<string> pulledCards, bool validate = true)
        {
            base.SetCards(cards, validate);

            foreach (string Card in pulledCards ?? new List<string>())
            {
                _PulledCards.Add(Card);

                int Value = CardValues.IndexOf(CardValues.First(x => x.ID == GetCardsValues(new List<string>() { Card }).Single())) + 2;
                if (Value < 7) _LowCards.Add(Card);
                else if (Value > 9) _HighCards.Add(Card);
            }
        }

        public int DeckValue => _LowCards.Count - _HighCards.Count;
        public int CardsCount => _PulledCards.Count;
    }
}
