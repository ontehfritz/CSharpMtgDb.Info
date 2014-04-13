using System;
using System.Net;
using Newtonsoft.Json;
using MtgDb.Info;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MtgDb.Info.Driver
{
    public class Db
    {
        public string ApiUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MtgDb.Info.Driver.Db"/> class.
        /// </summary>
        public Db ()
        {
            ApiUrl = "https://api.mtgdb.info";
        }
            
        /// <summary>
        /// Initializes a new instance of the <see cref="Mtgdb.Info.Wrapper.Db"/> class.
        /// Only use this method if you running a local version MtgDB api. 
        /// </summary>
        /// <param name="url">Custom Url if not using: "http://api.mtgdb.info";</param>
        public Db(string url)
        {
            ApiUrl = url;
        }

        /// <summary>
        /// Get a card by multiverse Id
        /// </summary>
        /// <returns>A card</returns>
        /// <param name="id">Multiverse Id</param>
        public Card GetCard(int id)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/cards/{1}", this.ApiUrl, id.ToString());
                var json = client.DownloadString(url);
                Card card = JsonConvert.DeserializeObject<Card>(json);

                return card;
            }
        }

        /// <summary>
        /// Gets the random card.
        /// </summary>
        /// <returns>The random card.</returns>
        public Card GetRandomCard()
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/cards/random", this.ApiUrl);
                var json = client.DownloadString(url);
                Card card = JsonConvert.DeserializeObject<Card>(json);

                return card;
            }
        }

        /// <summary>
        /// Gets the random card in set.
        /// </summary>
        /// <returns>The random card in set.</returns>
        /// <param name="setId">Set identifier.</param>
        public Card GetRandomCardInSet(string setId)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/sets/{1}/cards/random", this.ApiUrl,setId);
                var json = client.DownloadString(url);
                Card card = JsonConvert.DeserializeObject<Card>(json);

                return card;
            }
        }

        /// <summary>
        /// Gets all sets on mtgdb.info
        /// </summary>
        /// <returns>Mtg Sets</returns>
        /// <param name="setIds">3 character identifier</param>
        public CardSet[] GetSets(string [] setIds)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/sets/{1}", this.ApiUrl, 
                    string.Join(",", setIds));

                var json = client.DownloadString(url);
                List<CardSet> sets = new List<CardSet>();

                if(setIds.Length == 1)
                {
                    sets.Add (JsonConvert.DeserializeObject<CardSet>(json));
                }
                else
                {
                    sets.AddRange (JsonConvert.DeserializeObject<CardSet[]>(json));
                }

                return sets.ToArray();
            }
        }
           
        /// <summary>
        /// Get multiple cards by multiverse Id.
        /// </summary>
        /// <returns>return an array of Card objects</returns>
        /// <param name="multiverseIds">Multiverse identifiers.</param>
        public Card[] GetCards(int [] multiverseIds)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/cards/{1}", this.ApiUrl, 
                    string.Join(",", multiverseIds));

                var json = client.DownloadString(url);
                List<Card> cards = new List<Card>();

                if(multiverseIds.Length == 1)
                {
                    cards.Add (JsonConvert.DeserializeObject<Card>(json));
                }
                else
                {
                    cards.AddRange (JsonConvert.DeserializeObject<Card[]>(json));
                }

                return cards.ToArray();
            }
        }

        /// <summary>
        /// Returns all prints of a card by name
        /// </summary>
        /// <returns>Array of Card Objects</returns>
        /// <param name="name">Name of the card</param>
        public Card[] GetCards(string name)
        {
            if(name == null || name == "")
            {
                throw new ArgumentException("Cannot be null or blank", "name");
            }

            using (var client = new WebClient())
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                name = rgx.Replace(name, "");
                string url = string.Format ("{0}/cards/{1}", this.ApiUrl, name);
                var json = client.DownloadString(url);

                Card[] cards = null;

                try
                {
                    cards = JsonConvert.DeserializeObject<Card[]>(json);
                }
                catch(Exception e)
                {
                    cards = null;
                }
                    
                return cards;
            }
        }

        /// <summary>
        /// Gets the entire card database. Useful creating a local copy. This method may take some time to run
        /// </summary>
        /// <returns>Array of Card objects</returns>
        public Card[] GetCards()
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/cards/", this.ApiUrl);
                var json = client.DownloadString(url);

                Card[] cards = JsonConvert.DeserializeObject<Card[]>(json);

                return cards;
            }
        }

        /// <summary>
        /// Filters the cards by Card field
        /// </summary>
        /// <returns>Array of Card objects.</returns>
        /// <param name="property">The field name you want to fileter by</param>
        /// <param name="value">the value of the field to match</param>
        public Card[] FilterCards(string property, string value)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/cards/?{1}={2}", this.ApiUrl, property, value);
                var json = client.DownloadString(url);

                Card[] cards = JsonConvert.DeserializeObject<Card[]>(json);

                return cards;
            }
        }

        /// <summary>
        /// Gets cards in a set. You can use start and end to page through the card sets. This uses the card number.
        /// </summary>
        /// <returns>Array of Card objects.</returns>
        /// <param name="setId">Set identifier.</param>
        /// <param name="start">Optional: which card number to start at</param>
        /// <param name="end">Optional: which card to retrieve up to and including</param>
        public Card[] GetSetCards(string setId, int start = 0, int end = 0)
        {
            using (var client = new WebClient())
            {
                string url = null;
                if(start > 0 || end > 0)
                {
                    url = string.Format ("{0}/sets/{1}/cards/?start={2}&end={3}", 
                        this.ApiUrl, setId, start, end);
                }
                else
                {
                    url = string.Format ("{0}/sets/{1}/cards/", this.ApiUrl, setId);
                }
                 
                var json = client.DownloadString(url);

                Card[] cards = JsonConvert.DeserializeObject<Card[]>(json);

                return cards;
            }
        }

        public Card GetCardInSet(string setId, int setNumber)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/sets/{1}/cards/{2}", this.ApiUrl, 
                    setId, setNumber.ToString());

                var json = client.DownloadString(url);
                Card card = JsonConvert.DeserializeObject<Card>(json);

                return card;
            }
        }

        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <returns>A CardSet Object</returns>
        /// <param name="setId">Set identifier.</param>
        public CardSet GetSet(string setId)
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/sets/{1}", this.ApiUrl, setId);
                var json = client.DownloadString(url);

                CardSet set = JsonConvert.DeserializeObject<CardSet>(json);

                return set;
            }
        }

        /// <summary>
        /// Gets all sets.
        /// </summary>
        /// <returns>An array of CardSet Objects.</returns>
        public CardSet[] GetSets()
        {
            using (var client = new WebClient())
            {
                string url = string.Format ("{0}/sets/", this.ApiUrl);
                var json = client.DownloadString(url);

                CardSet[] sets = JsonConvert.DeserializeObject<CardSet[]>(json);

                return sets;
            }
        }

        /// <summary>
        /// Search the for the specified test in the serach name field.
        /// It will strip out all characters that are not alpha-numeric.
        /// </summary>
        /// <param name="text">An array of Card objects</param>
        public Card[] Search(string text)
        {
            using (var client = new WebClient())
            {
                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                text = rgx.Replace(text, "");
                string url = string.Format ("{0}/search/{1}", this.ApiUrl, text);
                var json = client.DownloadString(url);

                Card[] cards = JsonConvert.DeserializeObject<Card[]>(json);

                return cards;
            }
        }
    }
}

