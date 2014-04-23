using System;
using NUnit.Framework;
using MtgDb.Info;
using MtgDb.Info.Driver;

namespace MtgDb.Info.Test.CSharp
{
    [TestFixture()]
    public class TestMtgDriver
    {
        Db mtginfo = new Db("http://127.0.0.1:8082");

        //Db mtginfo = new Db();

        [Test()]
        public void Test_search_complex ()
        {
            Card [] cards = mtginfo.Search ("color eq blue and type m 'Creature' and description m 'flying' " +
                "and convertedmanacost lt 3 and name m 'Cloud'", true);
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }
            
        [Test()]
        public void Test_search_cards ()
        {
            Card [] cards = mtginfo.Search ("giant");
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }

        [Test()]
        public void Test_get_random_card ()
        {
            Card card = mtginfo.GetRandomCard ();
            System.Console.WriteLine (card.Id.ToString());
            Assert.IsNotNull (card);
        }

        [Test()]
        public void Test_get_random_card_in_set ()
        {
            Card card = mtginfo.GetRandomCardInSet ("lea");
            System.Console.WriteLine (card.ReleasedAt.ToLongDateString ());
            Assert.IsNotNull (card);
        }
            
        [Test()]
        public void Test_get_card ()
        {
            Card card = mtginfo.GetCard (14456);
            System.Console.WriteLine (card.ReleasedAt.ToLongDateString ());
            Assert.AreEqual (14456, card.Id);
        }
       
        [Test()]
        public void Test_get_cards_by_name ()
        {
            Card [] cards = mtginfo.GetCards ("ankh of mishra");
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }

        [Test()]
        public void Test_get_cards_by_name_no_match ()
        {
            Card [] cards = null;

            try
            {
                cards = mtginfo.GetCards ("");
            }
            catch(Exception e)
            {
                Assert.IsNull (cards);
            }
        }
            
        [Test()]
        public void Test_get_cards_by_multiverseIds ()
        {
            int[] multiverseIds = new int[]{1,2};

            Card [] cards = mtginfo.GetCards (multiverseIds);
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }

        [Test()]
        public void Test_get_cards ()
        {
            Card [] cards = mtginfo.GetCards ();
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }

        [Test()]
        public void Test_get_set ()
        {
            CardSet set = mtginfo.GetSet ("10E");
            System.Console.WriteLine (set.Id);
            Assert.AreEqual ("10E", set.Id);
        }

        [Test()]
        public void Test_get_card_in_set ()
        {
            Card card = mtginfo.GetCardInSet("10E",1);

            System.Console.WriteLine (card.CardSetId);
            Assert.AreEqual(1, card.SetNumber);
            Assert.AreEqual("10E", card.CardSetId);
        }

        [Test()]
        public void Test_get_card_in_set_not_found ()
        {
            Card card = mtginfo.GetCardInSet("10E",1000);
            Assert.IsNull(card);
        }
            
        [Test()]
        public void Test_get_set_cards ()
        {
            Card[] cards = mtginfo.GetSetCards ("10E");
            System.Console.WriteLine (cards.Length.ToString());
            Assert.GreaterOrEqual (cards.Length,1);
        }

        [Test()]
        public void Test_get_set_cards_with_range ()
        {
            Card[] cards = mtginfo.GetSetCards ("10E",1,10);
            System.Console.WriteLine (cards.Length.ToString());
            Assert.AreEqual (cards.Length,10);
        }

        [Test()]
        public void Test_get_sets ()
        {
            CardSet[] sets = mtginfo.GetSets();
            System.Console.WriteLine (sets.Length.ToString());
            Assert.GreaterOrEqual (sets.Length,1);
        }

        [Test()]
        public void Test_get_multiple_sets ()
        {
            CardSet[] sets = mtginfo.GetSets(new string[]{"10e","all","ths"});
            System.Console.WriteLine (sets.Length.ToString());
            Assert.AreEqual(3,sets.Length);
        }
    }
}

