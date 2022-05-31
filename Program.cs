using System;
using System.Collections.Generic;
using System.Linq;

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

            while (true)
            {
                // get a new deck
                CountingDeck SessionDeck;
                Console.WriteLine("New Deck!\n");

                // player
                if (CardsDealer.Count == 0 || CardsPlayer1.Count == 0)
                {
                    SessionDeck = new CountingDeck(DeckCount);

                    CardsPlayer1 = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                    CardsDealer = new List<string>() { SessionDeck.Pull(), SessionDeck.Pull() };
                }
                else
                {
                    SessionDeck = CountingDeck.Empty;

                    List<string> NewDeck = Deck.GetNewDeck(DeckCount);
                    List<string> PulledCards = CardsPlayer1.Concat(CardsDealer).ToList();
                    PulledCards.ForEach(PulledCard => NewDeck.Remove(PulledCard));

                    SessionDeck.SetCards(NewDeck, PulledCards);
                }

                SessionDeck.Shuffle();

                while (SessionDeck.RemainingCards > 0)
                {
                    Console.Clear();
                    Console.WriteLine($"Remaining Cards: {SessionDeck.RemainingCards}");
                    Console.WriteLine();

                    // end player turn if player has blackjack
                    if (CalculateValue(Deck.GetCardsValues(CardsPlayer1)) == 21) IsPlayerTurn = false;

                    // force winner if dealer has blackjack
                    if (CalculateValue(Deck.GetCardsValues(CardsDealer)) == 21) ForceWinner = true;

                    // display cards
                    if (IsPlayerTurn && !(CalculateValue(Deck.GetCardsValues(CardsDealer)) >= 21) && !(CalculateValue(Deck.GetCardsValues(CardsPlayer1)) >= 21)) Console.WriteLine($"Dealer: {string.Join(", ", Deck.GetCardsName(new List<string>() { CardsDealer.First() }))},~hidden~ ({CalculateValue(new List<string>() { Deck.GetCardsValues(CardsDealer).First() })})");
                    else Console.WriteLine($"Dealer: {string.Join(", ", Deck.GetCardsName(CardsDealer))} ({CalculateValue(Deck.GetCardsValues(CardsDealer))})");
                    Console.WriteLine($"Player : {string.Join(", ", Deck.GetCardsName(CardsPlayer1))} ({CalculateValue(Deck.GetCardsValues(CardsPlayer1))})");
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
                                Console.WriteLine($"Deck Value {SessionDeck.DeckValue}");
                                Console.ReadLine();
                                break;
                        }
                    }
                    else
                    {
                        if (CalculateValue(Deck.GetCardsValues(CardsDealer)) < 17)
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
                }
            }
        }

        public static WinReason CheckWinner(List<string> cardsDealer, List<string> cardsPlayer, bool forceWinner, int pointsLimit = 21)
        {
            List<string> CardsDealer = Deck.GetCardsValues(cardsDealer);
            List<string> CardsPlayer = Deck.GetCardsValues(cardsPlayer);

            int CardsValueDealer = CalculateValue(CardsDealer);
            int CardsValuePlayer = CalculateValue(CardsPlayer);

            if (CardsValuePlayer == pointsLimit)
            {
                // PLAYER BLACKJACK or PULL
                return CardsValueDealer != pointsLimit ? WinReason.PlayerBlackjack : WinReason.Push;
            }

            // PLAYER BUST
            if (CardsValuePlayer > pointsLimit) return WinReason.PlayerBust;

            // Dealer BLACKJACK
            if (CardsValueDealer == pointsLimit) return WinReason.DealerBlackjack;

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
            int[][] CardValues = cards.Select(Card => Deck.CardValues.Where(x => x.ID == Card).Single().Values).ToArray();

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
