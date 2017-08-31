using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace HeroExplorer
{
    public class MarvelFacade
    {
        private const string PrivateKey = "a8ea1c8f7efe48fe5f973e677b86ddf88d65f21a";
        private const string PublicKey = "02dc232ea2107b7239a83499a460ec37";
        private const int MaxCharacters = 1500;
        private const string NoImage = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";

        public static async Task PopularMarvelCharactersAsync(ObservableCollection<Character> marvelCharacters)
        {
            try
            {
                var characterDataWrapper = await GetCharacterDataWrapperAsync();
                 var characters = characterDataWrapper.data.results;

            foreach (var character in characters)
            {
                if (character.thumbnail != null &&
                    character.thumbnail.path != "" &&
                    character.thumbnail.path != NoImage)
                { 
                    character.thumbnail.small = String.Format("{0}/standard_small.{1}", character.thumbnail.path, character.thumbnail.extension);
                    character.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}", character.thumbnail.path, character.thumbnail.extension);
                    marvelCharacters.Add(character);
                }
            }
            }
            catch (Exception)
            {
                return;
            }
           
        }

        public static async Task PopularMarvelComicsAsync(ObservableCollection<ComicBook> marvelComics,int characterId)
        {
            try
            {
                var comicDataWrapper = await GetComicDataWrapperAsync(characterId);
                var comics = comicDataWrapper.data.results;

                foreach (var comic in comics)
                {
                    if (comic.thumbnail != null &&
                        comic.thumbnail.path != "" &&
                        comic.thumbnail.path != NoImage)
                    {
                        comic.thumbnail.small = String.Format("{0}/portrait_medium.{1}", comic.thumbnail.path, comic.thumbnail.extension);
                        comic.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}", comic.thumbnail.path, comic.thumbnail.extension);
                        marvelComics.Add(comic);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

        }

        public static async Task<ComicDataWrapper> GetComicDataWrapperAsync(int characterId)
        {
            // Assemble the URL
            Random random = new Random();
            var offset = random.Next(MaxCharacters);

            // Get the MD5 Hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hash = CreateHash(timeStamp);

            string url = String.Format("http://gateway.marvel.com:80/v1/public/comics?characters={0}?limit=10&apikey={1}&ts={2}&hash={3}", characterId, PublicKey, timeStamp, hash);

            // Call out to Marvel
            HttpClient http = new HttpClient();
            try
            {
                var response = await http.GetAsync(url);
                var jsonMessage = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(ComicDataWrapper));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

                var result = (ComicDataWrapper)serializer.ReadObject(ms);
                return result;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
            }
            return null;
        }

        public static async Task<CharacterDataWrapper> GetCharacterDataWrapperAsync()
        {
            // Assemble the URL
            Random random = new Random();
            var offset = random.Next(MaxCharacters);

            // Get the MD5 Hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hash = CreateHash(timeStamp);

            string url = String.Format("http://gateway.marvel.com:80/v1/public/characters?limit=100&offset={0}&apikey={1}&ts={2}&hash={3}", offset, PublicKey, timeStamp, hash);

            // Call out to Marvel
            HttpClient http = new HttpClient();
            try
            {
                var response = await http.GetAsync(url);
                var jsonMessage = await response.Content.ReadAsStringAsync();
                var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
                var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

                var result = (CharacterDataWrapper)serializer.ReadObject(ms);
                return result;
            }
            catch (Exception ex) {
                var a = ex.Message;
            } 
            return null;
        }

        private static string CreateHash(string timeStamp)
        {

            var toBeHashed = timeStamp + PrivateKey + PublicKey;
            var hashedMessage = ComputeMD5(toBeHashed);
            return hashedMessage;
        }

       
        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

    }
}
