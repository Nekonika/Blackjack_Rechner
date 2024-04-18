using System;
using System.Collections.Generic;
using System.Linq;
using static Blackjack_Rechner.DeckBase;

namespace Blackjack_Rechner
{
    public class Blackjack
    {
        static void Main(string[] args)
        {
            int DeckCount = 1;

            List<string> CardsPlayer1 = new List<string>();
            List<string> CardsDealer = new List<string>();

            bool IsPlayerTurn = true;
            bool ForceWinner = false;
            bool Exit = false;

            while (true)
            {
                Exit = false;
                IDeck SessionDeck = null;

                // Choose a deck for you to play with.
                while (SessionDeck == null)
                {
                    Console.Clear();
                    Console.WriteLine("Chose one of the following decks to play with:");
                    Console.WriteLine("[1] Standard Deck      The default deck. Does not allow counting cards.");
                    Console.WriteLine("[2] Counting Deck      Allows you to use the count command and see the value of the cards remaining on the deck.");

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            SessionDeck = new StandardDeck(1);
                            break;

                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            SessionDeck = new CountingDeck(1);
                            break;
                    }
                }

                while (true)
                {
                    // player
                    if (CardsDealer.Count == 0 || CardsPlayer1.Count == 0)
                    {
                        // Start a new game.

                        SessionDeck.New();
                        SessionDeck.Shuffle();

                        CardsPlayer1 = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                        CardsDealer = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                    }
                    else
                    {
                        // Fill the deck to its full size (while taking the deck count into account)
                        // removing the cards that have already been pulled this game from the new deck.
                        // This will make sure that when for example using 6 decks there can never be
                        // more than 6 Aces of Hearts in the game.
                        // The count (value) of the deck will not be affected. (when using the counting deck)

                        SessionDeck.Clear();

                        List<string> NewDeck = SessionDeck.GetNewDeck(DeckCount);

                        // cannot use the "is" keyword here, as 'CoutingDeck is StandardDeck' would be true aswell.
                        if (SessionDeck.GetType() == typeof(StandardDeck))
                        {
                            StandardDeck StandardDeck = SessionDeck as StandardDeck;
                            StandardDeck.SetCards(NewDeck);
                        }
                        else if (SessionDeck.GetType() == typeof(CountingDeck))
                        {
                            CountingDeck CountingDeck = SessionDeck as CountingDeck;
                            List<string> PulledCards = CardsPlayer1.Concat(CardsDealer).ToList();
                            PulledCards.ForEach(PulledCard => NewDeck.Remove(PulledCard));

                            CountingDeck.SetCards(NewDeck, PulledCards);
                        }

                        SessionDeck.Shuffle();
                    }

                    while (SessionDeck.RemainingCards > 0)
                    {
                        Console.Clear();
                        Console.WriteLine($"Remaining Cards: {SessionDeck.RemainingCards}");
                        Console.WriteLine();

                        // end player turn if player has blackjack
                        if (CalculateValue(GetCardsValues(CardsPlayer1)) == 21) IsPlayerTurn = false;

                        // force winner if dealer has blackjack
                        //if (CalculateValue(Deck.GetCardsValues(CardsDealer)) == 21) ForceWinner = true;

                        // display cards
                        bool DealerCardHidden = IsPlayerTurn && !(CalculateValue(GetCardsValues(CardsDealer)) > 21) && !(CalculateValue(GetCardsValues(CardsPlayer1)) > 21);
                        if (DealerCardHidden) Console.WriteLine($"Dealer: {string.Join(", ", GetCardsName(new List<string>() { CardsDealer.First() }))},~hidden~ ({CalculateValue(new List<string>() { GetCardsValues(CardsDealer).First() })})");
                        else Console.WriteLine($"Dealer: {string.Join(", ", GetCardsName(CardsDealer))} ({CalculateValue(GetCardsValues(CardsDealer))})");
                        Console.WriteLine($"Player : {string.Join(", ", GetCardsName(CardsPlayer1))} ({CalculateValue(GetCardsValues(CardsPlayer1))})");
                        Console.WriteLine();

                        WinReason WinnerType = CheckWinner(CardsDealer, CardsPlayer1, ForceWinner);
                        if (WinnerType != WinReason.None)
                        {
                            ShowWinner(WinnerType);

                            if (SessionDeck.RemainingCards < 5)
                            {
                                CardsPlayer1 = new List<string>();
                                CardsDealer = new List<string>();

                                break;
                            }
                            else
                            {
                                CardsPlayer1 = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                                CardsDealer = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                            }

                            IsPlayerTurn = true;
                            ForceWinner = false;

                            continue;
                        }

                        Console.Write("Hold or Pull?: ");
                        if (IsPlayerTurn)
                        {
                            switch (Console.ReadLine().ToLower())
                            {
                                case "help":
                                    Console.WriteLine("You may use one of the following commands:");
                                    Console.WriteLine();
                                    Console.WriteLine("count        Prints the current card counting value of this game. (will only work when using the counting deck)");
                                    Console.WriteLine("             Cards that are hidden (such as the dealers second card) will not be counted.");
                                    Console.WriteLine("count all    Prints the current card counting value of this game. (will only work when using the counting deck)");
                                    Console.WriteLine("deck         Prints the remaining cards in the deck in the order that they will be pulled.");
                                    Console.WriteLine("exit         Brings you back to the previous menu.");
                                    Console.WriteLine("help         Used to show this page.");
                                    Console.WriteLine("hold         If you do not want to pull a card. (alias: h)");
                                    Console.WriteLine("new          Will exchange the remaining Cards with a completely new deck.");
                                    Console.WriteLine("peek bottom  Will show you the last card to be pulled.");
                                    Console.WriteLine("peek top     Will show you the next card to be pulled.");
                                    Console.WriteLine("pull         Gives you a new card from the top of the deck. (alias: p)");
                                    Console.WriteLine("shuffle      The remaining cards in the deck will be shuffled.");
                                    Console.ReadLine();
                                    break;

                                case "exit":
                                    Exit = true;
                                    break;

                                case "hold":
                                case "h":
                                    IsPlayerTurn = false;
                                    break;

                                case "pull":
                                case "p":
                                    CardsPlayer1.Add(SessionDeck.Pull());
                                    break;

                                case "deck":
                                    Console.WriteLine($"Deck: {string.Join(", ", SessionDeck.Cards)}");
                                    Console.ReadLine();
                                    break;

                                case "peek top":
                                    Console.WriteLine($"Peek top: {SessionDeck.PeekTop()}");
                                    Console.ReadLine();
                                    break;

                                case "peek bottom":
                                    Console.WriteLine($"Peek bottom: {SessionDeck.PeekBottom()}");
                                    Console.ReadLine();
                                    break;

                                case "shuffle":
                                    SessionDeck.Shuffle();
                                    break;

                                case "new":
                                    SessionDeck.New();
                                    break;

                                case "count":
                                    if (SessionDeck is CountingDeck)
                                    {
                                        CountingDeck CountingDeck = SessionDeck as CountingDeck;
                                        int DeckValue = CountingDeck.DeckValue;

                                        // adjust the count for the dealers card, that we cannot see.
                                        if (DealerCardHidden)
                                        {
                                            int HiddenCardValue = CardValues.IndexOf(CardValues.First(x => x.ID == GetCardsValues(new List<string>() { CardsDealer[1] }).Single())) + 2;
                                            if (HiddenCardValue < 7) DeckValue--;
                                            else if (HiddenCardValue > 9) DeckValue++;

                                            Console.WriteLine($"Deck Value: {DeckValue} (you can not see the dealers second card)");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Deck Value: {DeckValue}");
                                        }

                                        Console.ReadLine();
                                    }
                                    break;

                                case "count all":
                                    if (SessionDeck is CountingDeck)
                                    {
                                        CountingDeck CountingDeck = SessionDeck as CountingDeck;
                                        Console.WriteLine($"Deck Value: {CountingDeck.DeckValue}");
                                        Console.ReadLine();
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (CalculateValue(GetCardsValues(CardsDealer)) < 17)
                            {
                                Console.WriteLine("pull");
                                CardsDealer.Add(SessionDeck.Pull());
                            }
                            else
                            {
                                Console.WriteLine("hold");
                                ForceWinner = true;
                            }

                            System.Threading.Tasks.Task.Delay(1600).Wait();
                        }

                        if (Exit) break;
                    }

                    if (Exit) break;
                }
            }
        }

        public static WinReason CheckWinner(List<string> cardsDealer, List<string> cardsPlayer, bool forceWinner, int pointsLimit = 21)
        {
            List<string> CardsDealer = GetCardsValues(cardsDealer);
            List<string> CardsPlayer = GetCardsValues(cardsPlayer);

            int CardsValueDealer = CalculateValue(CardsDealer);
            int CardsValuePlayer = CalculateValue(CardsPlayer);

            if (forceWinner && CardsValuePlayer == pointsLimit)
            {
                // PLAYER BLACKJACK or PULL
                return CardsValueDealer != pointsLimit ? WinReason.PlayerBlackjack : WinReason.Push;
            }

            // PLAYER BUST
            if (CardsValuePlayer > pointsLimit) return WinReason.PlayerBust;

            // Dealer BLACKJACK
            if (forceWinner && CardsValueDealer == pointsLimit) return WinReason.DealerBlackjack;

            // Dealer BUST
            if (CardsValueDealer > pointsLimit) return WinReason.DealerBust;

            // PULL
            if (CardsValuePlayer == CardsValueDealer) return forceWinner ? WinReason.Push : WinReason.None;

            // most points win
            return forceWinner ? (CardsValuePlayer > CardsValueDealer ? WinReason.PlayerMorePoints : WinReason.DealerMorePoints) : WinReason.None;
        }

        private static void ShowWinner(WinReason winner)
        {
            switch (winner)
            {
                case WinReason.PlayerMorePoints:
                case WinReason.PlayerBlackjack:
                case WinReason.DealerBust:
                    Console.WriteLine("Player has won!");
                    break;

                case WinReason.DealerMorePoints:
                case WinReason.DealerBlackjack:
                case WinReason.PlayerBust:
                    Console.WriteLine("Dealer has won!");
                    break;

                case WinReason.Push:
                    Console.WriteLine("Push - there is no winner!");
                    break;
            }

            Console.WriteLine($"WinReason: {Enum.GetName(typeof(WinReason), winner)}");
            Console.ReadLine();
        }

        private static int CalculateValue(List<string> cards, int pointLimit = 21)
        {
            int[][] CardValues = cards.Select(Card => DeckBase.CardValues.Where(x => x.ID == Card).Single().Values).ToArray();

            int[][] MultiValueCards = CardValues
                .Where(Card => Card.Length > 1)
                .Select(Card => Card.OrderBy(x => x).ToArray())
                .OrderBy(Card => Card.Min())
                .ToArray();

            int[] SingleValueCards = CardValues
                .Where(Card => Card.Length == 1)
                .Select(Card => Card.First())
                .ToArray();

            int TotalValue = SingleValueCards.Sum() + MultiValueCards.Select(Cards => Cards.First()).Sum();

            int I = 1;
            while (TotalValue < pointLimit)
            {
                int[] CardOffsetValues = MultiValueCards
                    .Where(x => x.Length >= I + 1)
                    .Select(x => x[I] - x.First()).ToArray();

                if (CardOffsetValues.Length <= 0) break;

                foreach (int CardOffsetValue in CardOffsetValues)
                {
                    int NewValue = TotalValue + CardOffsetValue;
                    if (NewValue <= pointLimit) TotalValue = NewValue;
                }

                I++;
            }

            return TotalValue;
        }

        public enum WinReason
        {
            None,
            DealerBust,
            DealerMorePoints,
            DealerBlackjack,
            PlayerBust,
            PlayerMorePoints,
            PlayerBlackjack,
            Push
        }
    }
}
