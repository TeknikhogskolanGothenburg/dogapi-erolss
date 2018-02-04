using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using dogapi_erolss.Models;

namespace dogapi_erolss.Controllers
{
    [Produces("application/json")]
    [Route("api/Dogs")]
    public class DogsController : Controller
    {

        // GET: api/Dogs
        [HttpGet]
        public IActionResult Get()
        {
            var dogs = GetDogsAsJsonList();

            var retVal = dogs.Select(x => x.BreedName);
            

            if (retVal == null)
            {
                return BadRequest();
            }

            Response.ContentType = "application/json";
            return new ObjectResult(JsonConvert.SerializeObject(retVal));
        }

        // GET: api/Dogs/Bulldog
        [HttpGet("{name}", Name = "Get")]
        public IActionResult Get(string name)
        {
            var dogs = GetDogsAsJsonList();

            var retVal = dogs.Where(x => x.BreedName.ToLower() == name.ToLower()).FirstOrDefault();           

            if (retVal == null)
            {
                return BadRequest();
            }

            Response.ContentType = "application/json";
            return new ObjectResult(JsonConvert.SerializeObject(retVal));

        }

        // POST: api/Dogs
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Dogs/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Dogs
        [HttpDelete]
        public void Delete()
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json");
            
            foreach (var file in files)
            {
                System.IO.File.Delete(file);
            }

        }

        // DELETE: api/Dogs/5
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json");

            foreach (var file in files)
            {
                var content = System.IO.File.ReadAllText(file);
                var dog = JsonConvert.DeserializeObject<Dog>(content);
                if (dog.BreedName.ToLower() == name.ToLower())
                {
                    System.IO.File.Delete(file);
                }
            }

        }



        private List<string> GetFileList()
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json").ToList();

            return files;
        }

        private List<Dog> GetDogsAsJsonList()
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json");
            var dogs = new List<Dog>();
            foreach (var file in files)
            {
                var content = System.IO.File.ReadAllText(file);
                dogs.Add(JsonConvert.DeserializeObject<Dog>(content));
            }
            return dogs;
        }
    }
}
