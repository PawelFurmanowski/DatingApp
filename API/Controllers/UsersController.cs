using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        //Zwracamy ACTIONRESULT w <> oznaczamy typ zmiennych które zwracamy
        //Do zwracania listy userów użyjemy IEnumerable
        //IEnumerable pozwala na prostą iterację po na kolekcji 
        //Zamiast IEnumerable moglibyśmy użyć List<AppUser> ale lista oferuje zbyt wiele metod a my potrzebujemy jedynie zwrócić userów
        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            //ToList() potrzebuje LINQ
            var users = _context.Users.ToList();

            return users;
        }

        //  api/users/3 => zwróci pojedyńczego usera
        // zwracamy pojedyńczego usera więc IEnumerable nie jest nam potrzbene (to nie jest lista)
        //id bierzemy z 
        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUser(int id)
        {
            //Find() potrzebuje LINQ zwraca zmienną o określonym id
            var user = _context.Users.Find(id);

            return user;
        }


    }
}