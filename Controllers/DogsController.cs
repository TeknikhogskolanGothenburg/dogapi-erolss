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
            
            return new ObjectResult(retVal);
        }

        // GET: api/Dogs/Bulldog
        [HttpGet("{breed}", Name = "Get")]
        public IActionResult Get(string breed)
        {
            var dogs = GetDogsAsJsonList();

            var retVal = dogs.Where(x => x.BreedName.ToLower() == breed.ToLower()).FirstOrDefault();

            if (retVal == null)
            {
                return BadRequest();
            }
            
            return new ObjectResult(retVal);

        }

        // POST: api/Dogs
        [HttpPost]
        public IActionResult Post([FromBody] Dog value)
        {

            if (value != null)
            {
                
                var dogs = GetDogsAsJsonList();
                if (!dogs.Exists(x => x.BreedName.ToLower() == value.BreedName.ToLower()))
                {
                    var path = "DogFiles\\" + value.BreedName + ".json";
                    var newVal = JsonConvert.SerializeObject(value);
                    System.IO.File.WriteAllText(path, newVal);

                    return new CreatedAtRouteResult("Get", new { breed = value.BreedName });
                }
            }

            return BadRequest();
        }

        // PUT: api/Dogs/5
        [HttpPut("{breed}")]
        public IActionResult Put(string breed, [FromBody]Dog value)
        {

            if (breed != null && value != null)
            {
                var files = GetFiles();

                foreach (var file in files)
                {
                    var content = System.IO.File.ReadAllText(file);
                    var dog = JsonConvert.DeserializeObject<Dog>(content);

                    if (dog.BreedName.ToLower() == breed.ToLower())
                    {
                        var newVal = value;
                        if (!String.IsNullOrEmpty(newVal.BreedName))
                        {
                            dog.BreedName = newVal.BreedName;
                        }

                        dog.Description = newVal.Description;
                        dog.WikipediaUrl = newVal.WikipediaUrl;

                        var newFileContent = JsonConvert.SerializeObject(dog);

                        System.IO.File.WriteAllText(file, newFileContent);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            else
            {
                return BadRequest();
            }

            return new NoContentResult();

        }

        // DELETE: api/Dogs
        [HttpDelete]
        public IActionResult Delete()
        {
            var files = GetFiles();

            if (files != null)
            {
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }

                return new NoContentResult();
            }
            else
            {
                return BadRequest();
            }

        }

        // DELETE: api/Dogs/5
        [HttpDelete("{breed}")]
        public IActionResult Delete(string breed)
        {
            var files = GetFiles();
            if (files != null)
            {
                foreach (var file in files)
                {
                    var content = System.IO.File.ReadAllText(file);
                    var dog = JsonConvert.DeserializeObject<Dog>(content);
                    if (dog.BreedName.ToLower() == breed.ToLower())
                    {
                        System.IO.File.Delete(file);
                    }
                }

                return new NoContentResult();
            }
            else
            {
                return BadRequest();
            }
        }



        private string[] GetFiles()
        {
            var files = System.IO.Directory.GetFiles("DogFiles", "*.json");

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
