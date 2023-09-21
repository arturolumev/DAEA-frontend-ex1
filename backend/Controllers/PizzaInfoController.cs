/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PizzaInfoController : ControllerBase
    {
        private static readonly PizzaInfo[] TheMenu = new[]
        {
            new PizzaInfo { PizzaName = "The Mighty Meatball", Ingredients = "Meatballs and cheese", Cost = 40, InStock = "yes"},
            new PizzaInfo { PizzaName = "Crab Apple", Ingredients = "Dungeness crab and apples", Cost = 35, InStock = "no"},
            new PizzaInfo { PizzaName = "Forest Floor", Ingredients = "Mushrooms, rutabagas, and walnuts", Cost = 20, InStock = "yes"},
            new PizzaInfo { PizzaName = "Don't At Me", Ingredients = "Pineapple, Canadian bacon, jalapeños", Cost = 25, InStock = "yes"},
            new PizzaInfo { PizzaName = "Vanilla", Ingredients = "Sausage and pepperoni", Cost = 15, InStock = "no"},
            new PizzaInfo { PizzaName = "Spice Coming At Ya", Ingredients = "Peppers, chili sauce, spicy andouille", Cost = 50, InStock = "yes"}
        };

        private readonly ILogger<PizzaInfoController> _logger;

        public PizzaInfoController(ILogger<PizzaInfoController> logger)
        {
            _logger = logger;
        }

         [HttpGet]
         public IEnumerable<PizzaInfo> Get()
         {
             return TheMenu;
         }
    }
}
*/

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace backend.Controllers
{
    [Route("api/{searchTerm}")]
    [ApiController]
    public class GifController : ControllerBase
    {
        private readonly string apikey = "LIVDSRZULELA";
        private readonly int limit = 1;

        [HttpGet]
        public async Task<IEnumerable<GifLink>> Get(string searchTerm)
        {
            var links = await ExtractLinks(limit, searchTerm);
            return links;
        }

        private async Task<IEnumerable<GifLink>> ExtractLinks(int lmt, string searchTerm)
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"https://g.tenor.com/v1/search?q={searchTerm}&key={apikey}&limit={lmt}";
                var response = await httpClient.GetStringAsync(url);
                var top_8gifs = JsonConvert.DeserializeObject<TenorResponse>(response);

                var links = new List<GifLink>();

                foreach (var result in top_8gifs.Results)
                {
                    links.Add(new GifLink
                    {
                        Text = result.Id,
                        Href = result.Media[0].Webm.Preview
                    });
                }

                return links;
            }
        }
    }

    public class TenorResponse
    {
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public string Id { get; set; }
        public List<Media> Media { get; set; }
    }

    public class Media
    {
        public Webm Webm { get; set; }
    }

    public class Webm
    {
        public string Preview { get; set; }
    }

    public class GifLink
    {
        public string Text { get; set; }
        public string Href { get; set; }
    }
}

