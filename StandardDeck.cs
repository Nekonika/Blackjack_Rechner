using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blackjack_Rechner
{
    internal class StandardDeck : DeckBase, IDeck
    {
        protected Queue<string> _Cards;
        protected readonly int _DeckCount = 0;

        public ReadOnlyCollection<string> Cards => new ReadOnlyCollection<string>(_Cards.ToArray());
        public int RemainingCards => _Cards.Count;
        public IDeck Empty
        {
            get 
            {
                IDeck EmptyDeck = new StandardDeck(_DeckCount);
                EmptyDeck.Clear();

                return EmptyDeck;
            }
        }

        public StandardDeck(int deckCount = 6)
        {
            _DeckCount = deckCount;
            _Cards = new Queue<string>(GetNewDeck(_DeckCount));
        }

        public virtual void SetCards(List<string> cards, bool validate = true)
        {
            if (!AreCardsValid(cards)) throw new ArgumentException("Invalid Deck!", nameof(cards));

            _Cards = new Queue<string>(cards);
        }

        public virtual void New() => _Cards = new Queue<string>(GetNewDeck(_DeckCount));
        public virtual void Clear() => _Cards.Clear();
        public virtual void Shuffle() => _Cards = new Queue<string>(Shuffle(_Cards.ToList()));
        public virtual string Pull() => _Cards.Dequeue();
        public virtual string PeekTop() => _Cards.First();
        public virtual string PeekBottom() => _Cards.Last();
    }
}
