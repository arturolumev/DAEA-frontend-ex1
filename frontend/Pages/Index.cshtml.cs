using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string apiEndpoint = "http://ip172-18-0-23-ck5rnsmfml8g00cmfpd0-5200.direct.labs.play-with-docker.com/api/"; // Reemplace el dominio aquí

        [BindProperty(SupportsGet = true)]
        public string Url { get; set; }
        public string Error { get; set; }
        public List<GifLink> Links { get; set; }
        public Dictionary<string, int> DomainCounts { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var json = await httpClient.GetStringAsync(apiEndpoint + Url);
                        Links = JsonConvert.DeserializeObject<List<GifLink>>(json);
                        DomainCounts = Links
                            .GroupBy(link => link.Href)
                            .ToDictionary(group => group.Key, group => group.Count());
                    }
                }
                catch (HttpRequestException ex)
                {
                    Error = "Error al realizar la solicitud HTTP: " + ex.Message;

                    // Verifica si hay una excepción interna
                    if (ex.InnerException != null)
                    {
                        Error += " Inner Exception: " + ex.InnerException.Message;
                    }
                }
                catch (Exception ex)
                {
                    Error = "Otro error: " + ex.Message;
                }
            }
        }
    }

    public class GifLink
    {
        public string Text { get; set; }
        public string Href { get; set; }
    }
}
