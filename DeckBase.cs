﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blackjack_Rechner
{
    internal class DeckBase
    {
        protected readonly Random _Random = new Random();

        public static readonly ReadOnlyDictionary<string, string> CardColors = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
            { "HE", "Herz" },
            { "KA", "Karo" },
            { "KR", "Kreuz" },
            { "PI", "Pik" },
        });

        public static readonly ReadOnlyCollection<CardValue> CardValues = new ReadOnlyCollection<CardValue>(new List<CardValue>() {
            new CardValue( "2", "Zwei", new [] { 2 } ),
            new CardValue( "3", "Drei", new [] { 3 } ),
            new CardValue( "4", "Vier", new [] { 4 } ),
            new CardValue( "5", "Fünf", new [] { 5 } ),
            new CardValue( "6", "Sechs", new [] { 6 } ),
            new CardValue( "7", "Sieben", new [] { 7 } ),
            new CardValue( "8", "Acht", new [] { 8 } ),
            new CardValue( "9", "Neun", new [] { 9 } ),
            new CardValue( "10", "Zehn", new [] { 10 } ),
            new CardValue( "B", "Bube", new [] { 10 } ),
            new CardValue( "D", "Dame", new [] { 10 } ),
            new CardValue( "K", "König", new [] { 10 } ),
            new CardValue( "A", "Ass", new [] { 1, 11 } ),
        });

        public virtual List<string> GetNewDeck(int deckCount = 6)
        {
            List<string> NewDeck = new List<string>();

            for (int k = 0; k < deckCount; k++)
            {
                foreach (string Color in CardColors.Keys)
                {
                    foreach (string Value in CardValues.Select(x => x.ID)) NewDeck.Add($"{Color}:{Value}");
                }
            }

            return NewDeck;
        }

        public virtual List<string> Shuffle(List<string> deck)
        {
            List<string> ShuffledDeck = new List<string>();

            while (deck.Count > 0)
            {
                int RndNum = _Random.Next(deck.Count);
                ShuffledDeck.Add(deck[RndNum]);
                deck.RemoveAt(RndNum);
            }

            return ShuffledDeck;
        }

        public static List<string> GetCardsValues(List<string> cards, bool validate = true)
        {
            if (validate)
            {
                return AreCardsValid(cards) ? cards.Select(Card => Card.Split(':')[1]).ToList() : throw new ArgumentException("The given cards are invalid!", nameof(cards));
            }
            else
            {
                return cards.Select(Card => Card.Split(':')[1]).ToList();
            }
        }

        public static List<string> GetCardsName(List<string> cards, bool validate = true)
        {
            if (validate && !AreCardsValid(cards)) throw new ArgumentException("The given deck is invalid!", nameof(cards));

            return cards.Select(Card => {
                string[] CardComponents = Card.Split(':');
                return $"{CardColors[CardComponents[0]]} {CardValues.Where(x => x.ID == CardComponents[1]).Single().Name}";
            }).ToList();
        }

        public static bool AreCardsValid(List<string> cards)
        {
            foreach (string Card in cards)
            {
                if (!Card.Contains(':')) return false;

                string[] CardComponents = Card.Split(':');
                string Color = CardComponents[0];
                string Value = CardComponents[1];

                if (!CardColors.Keys.Contains(Color) || !CardValues.Any(x => x.ID == Value)) return false;
            }

            return true;
        }

        public class CardValue
        {
            public readonly string ID;
            public readonly string Name;
            public readonly int[] Values;

            public CardValue(string id, string name, int[] values)
            {
                ID = id;
                Name = name;
                Values = values;
            }
        }
    }
}
