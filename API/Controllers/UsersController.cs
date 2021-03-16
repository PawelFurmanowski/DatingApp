using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task< ActionResult<IEnumerable<AppUser>> >GetUsers()
        {
            //ToList() potrzebuje LINQ
            //ToListAsync() potrzebuje Microsoft.EntityFrameworkCore;
            var users = await _context.Users.ToListAsync();

            return users;
        }

        //  api/users/3 => zwróci pojedyńczego usera
        // zwracamy pojedyńczego usera więc IEnumerable nie jest nam potrzbene (to nie jest lista)
         
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>>GetUser(int id)
        {
            //Find() potrzebuje LINQ zwraca zmienną o określonym id
            //FindAsync() potrzebuje Microsoft.EntityFrameworkCore; zwraca zmienną o określonym id
            var user = await _context.Users.FindAsync(id);

            return user;
        }


    }
}