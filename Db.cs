using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace MtgDb.Info.Driver
{
    public class Db
    {
        public string ApiUrl { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MtgDb.Info.Driver.Db"/> class.
        /// </summary>
        public Db()
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

        private T CallApi<T>(string uriFormat, params string[] OrderedArgs)
        {
            T result = default(T);
            try
            {
                using (WebClient client = new WebClient())
                {
                    string url;
                    if (OrderedArgs != null)
                    {
                        url = string.Format(uriFormat, this.ApiUrl, OrderedArgs);
                    }
                    else
                    {
                        url = string.Format(uriFormat, this.ApiUrl);
                    }

                    string json = client.DownloadString(url);
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch (Exception ex)
            {
                // TODO: implement exception handling soon :( 
            }

            return result;
        }
        public string[] GetCardRarityTypes()
        {
            return CallApi<string[]>("{0}/cards/rarity");
        }

        public string[] GetCardTypes()
        {
            return CallApi<string[]>("{0}/cards/types");
        }

        public string[] GetCardSubTypes()
        {
            return CallApi<string[]>("{0}/cards/subtypes");
        }

        /// <summary>
        /// Get a card by multiverse Id
        /// </summary>
        /// <returns>A card</returns>
        /// <param name="id">Multiverse Id</param>
        public Card GetCard(int id)
        {
            return CallApi<Card>("{0}/cards/{1}", id.ToString());
        }

        /// <summary>
        /// Gets the random card.
        /// </summary>
        /// <returns>The random card.</returns>
        public Card GetRandomCard()
        {
            return CallApi<Card>("{0}/cards/random");
        }

        /// <summary>
        /// Gets the random card in set.
        /// </summary>
        /// <returns>The random card in set.</returns>
        /// <param name="setId">Set identifier.</param>
        public Card GetRandomCardInSet(string setId)
        {
            return CallApi<Card>("{0}/sets/{1}/cards/random", setId);
        }

        /// <summary>
        /// Gets all sets on mtgdb.info
        /// </summary>
        /// <returns>Mtg Sets</returns>
        /// <param name="setIds">3 character identifier</param>
        public CardSet[] GetSets(params string[] setIds)
        {
            CardSet[] result;
            string combinedIds = string.Join(",", setIds);
            if (setIds.Length == 1)
            {
                result = new CardSet[1];
                result[0] = CallApi<CardSet>("{0}/sets/{1}", combinedIds);
            }
            else
            {
                result = CallApi<CardSet[]>("{0}/sets/{1}", combinedIds);
            }
            return result;
        }

        /// <summary>
        /// Get multiple cards by multiverse Id.
        /// </summary>
        /// <returns>return an array of Card objects</returns>
        /// <param name="multiverseIds">Multiverse identifiers.</param>
        public Card[] GetCards(params int[] multiverseIds)
        {
            Card[] result;
            string combinedIds = string.Join(",", multiverseIds);
            if (multiverseIds.Length == 1)
            {
                result = new Card[1];
                result[0] = CallApi<Card>("{0}/cards/{1}", combinedIds);
            }
            else
            {
                result = CallApi<Card[]>("{0}/cards/{1}", combinedIds);
            }

            return result;
        }

        /// <summary>
        /// Returns all prints of a card by name
        /// </summary>
        /// <returns>Array of Card Objects</returns>
        /// <param name="name">Name of the card</param>
        public Card[] GetCards(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot be null or blank", "name"); // this is the only original method that checks input quality. why?
            }

            Card[] result;
            name = StripNonAlphaNumeric(name);

            result = CallApi<Card[]>("{0}/cards/{1}", name);

            return result;
        }

        /// <summary>
        /// Gets the entire card database. Useful creating a local copy. This method may take some time to run
        /// </summary>
        /// <returns>Array of Card objects</returns>
        public Card[] GetCards()
        {
            return CallApi<Card[]>("{0}/cards/");
        }

        /// <summary>
        /// Filters the cards by Card field
        /// </summary>
        /// <returns>Array of Card objects.</returns>
        /// <param name="property">The field name you want to fileter by</param>
        /// <param name="value">the value of the field to match</param>
        public Card[] FilterCards(string property, string value)
        {
            return CallApi<Card[]>("{0}/cards/?{1}={2}", property, value);
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
            Card[] result;

            if (start > 0 || end > 0)
            {
                result = CallApi<Card[]>("{0}/sets/{1}/cards/?start={2}&end={3}", setId, start.ToString(), end.ToString());
            }
            else
            {
                result = CallApi<Card[]>("{0}/sets/{1}/cards/?start={2}&end={3}", setId);
            }

            return result;
        }

        public Card GetCardInSet(string setId, int setNumber)
        {
            return CallApi<Card>("{0}/sets/{1}/cards/{2}", setId, setNumber.ToString());
        }

        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <returns>A CardSet Object</returns>
        /// <param name="setId">Set identifier.</param>
        public CardSet GetSet(string setId)
        {
            return CallApi<CardSet>("{0}/sets/{1}", setId);
        }

        /// <summary>
        /// Gets all sets.
        /// </summary>
        /// <returns>An array of CardSet Objects.</returns>
        public CardSet[] GetSets()
        {
            return CallApi<CardSet[]>("{0}/sets/");
        }

        /// <summary>
        /// Search the for the specified test in the serach name field.
        /// It will strip out all characters that are not alpha-numeric.
        /// </summary>
        /// <param name="text">An array of Card objects</param>
        public Card[] Search(string text, int start = 0, int limit = 0, bool isComplex = false)
        {
            Card[] result;

            if (isComplex)
            {
                result = CallApi<Card[]>("{0}/search/?q={1}&start={2}&limit={3}", Uri.EscapeDataString(text), start.ToString(), limit.ToString());
            }
            else
            {

                result = CallApi<Card[]>("{0}/search/{1}?start={2}&limit={3}", StripNonAlphaNumeric(text), start.ToString(), limit.ToString());
            }

            return result;
        }

        // this is orders of magnitude faster than regex, which is critical in mobile apps.
        public string StripNonAlphaNumeric(string complexString)
        {
            char[] data = complexString.Where(x => (char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || x == '-')).ToArray().ToArray();
            return new string(data);
        }
    }
}