using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text.Json.Nodes;

class Program 
{
    static async Task Main(string[] args) 
    {
        //YOUR_BACKEND_API_URL
        string baseUrl = "";

        using(var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            var getAuthToken = await GetAuthenticationToken(client, "amoos@gmail.com", "test123");
            
            if(getAuthToken != null) 
            {   
                var token = getAuthToken.ToString();
                var companies = GetApiData(client, "/api/companies", token);
           
                Console.WriteLine("Companies:");

                if(companies != null)
                {
                    foreach (var company in companies)
                    {
                        Console.WriteLine($"ID: {company["id"]}, Name: {company["name"]}");
                    }
                }
                else 
                {
                    Console.WriteLine("Companies Not found!");
                }

                var locations = GetApiData(client, "/api/locations", token);
            
                Console.WriteLine("Locations:");

                if(locations != null)
                {
                    foreach (var location in locations)
                    {
                        Console.WriteLine($"ID: {location["id"]}, Name: {location["name"]}");
                    }
                }
                else 
                {
                    Console.WriteLine("Locations Not found!");
                }
                
            }
            else
            {
                Console.WriteLine("Failed to Authenticate!");
            }
        }
            
    }

    static async Task<JToken> GetAuthenticationToken(HttpClient client, string email, string password)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password),
        });
        HttpResponseMessage response = await client.PostAsync("/api/login", content);
        if (response.IsSuccessStatusCode)
        {
            JObject jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());
           
            if(jsonObject != null) 
            {   
                return jsonObject["access_token"];

            }
            
            return null;

        }
        else
        {
            return null;
        }


    }

    static JArray GetApiData(HttpClient client, string endpoint, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Security-Check", "true");

        var result = client.GetAsync(endpoint).Result;
        var json = result.Content.ReadAsStringAsync().Result;
        JObject jsonObject = JObject.Parse(json);
        if(jsonObject != null) 
        {
            return (JArray)jsonObject["data"];

        }
        else 
        {
            return null;
        }

    }
}