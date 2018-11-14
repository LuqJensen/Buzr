using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Buzr.Models
{
    public class MyCredentials
    {
        public static string CONSUMER_KEY = "DnY8NrtPxUXiSUkckbAPUZkFM";
        public static string CONSUMER_SECRET = "1iyOCp2CG5NE65HjuiUMNJXTsok6Ns06MLOvxHAlMk1My8Ef3O";
        public static string ACCESS_TOKEN = "934004251-m04TW0zvDHL5uvoOlwAqc1SUCgs9Fsi2UkUcOotl";
        public static string ACCESS_TOKEN_SECRET = "hsM16vQtoagp6tnlQXAuw6PibiNfllMWMNCLVOjJCB1rz";

        public static ITwitterCredentials GenerateCredentials()
        {
            return new TwitterCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
        }
    }
}
