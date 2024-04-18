using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Blackjack_Rechner
{
    public interface IDeck
    {
        ReadOnlyCollection<string> Cards { get; }
        int RemainingCards { get; }
        IDeck Empty { get; }

        List<string> GetNewDeck(int deckCount);
        void New();
        void Clear();
        void Shuffle();
        string Pull();
        string PeekTop();
        string PeekBottom();
    }
}
