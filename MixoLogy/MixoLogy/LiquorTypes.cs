using System;
using System.Collections.Generic;
using System.Text;

namespace MixoLogy
{
    static class LiquorTypes
    {
        public static IEnumerable<string> List()
        {
            return new List<string>()
            {
                "vodka",
                "wiskey",
                "sambuka",
                "vermouth",
                "brandy",
                "cognac",
                "beer",
                "port",
                "rum",
                "gin"
            };
        }
    }
}
