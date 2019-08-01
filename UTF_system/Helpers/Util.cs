using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UTF_system.Helpers
{
    public class Util
    {
        public static string GetLetra(double nota)
        {
            if (nota >= 90)
                return "A";
            else if (nota >= 80)
                return "B";
            else if (nota >= 70)
                return "C";
            else if (nota >= 60)
                return "D";
            else
                return "F";
        }

        public static string GeneratePassword()
        {
            string alphabet = "qwertyuiopasdfghjklzxcvbnm";
            string digits = "0123456789";

            Random rand = new Random();

            string pass = new string(alphabet.ToCharArray().OrderBy(s => (rand.Next(2) % 2) == 0).ToArray());
            pass += new string(digits.ToCharArray().OrderBy(s => (rand.Next(2) % 2) == 0).ToArray());
            pass = new string(pass.ToCharArray().OrderBy(s => (rand.Next(2) % 2) == 0).ToArray());

            return pass.Substring(0,7);
        }
        
    }


}