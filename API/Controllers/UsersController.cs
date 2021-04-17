using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        //Zwracamy ACTIONRESULT w <> oznaczamy typ zmiennych które zwracamy
        //Do zwracania listy userów użyjemy IEnumerable
        //IEnumerable pozwala na prostą iterację po na kolekcji 
        //Zamiast IEnumerable moglibyśmy użyć List<AppUser> ale lista oferuje zbyt wiele metod a my potrzebujemy jedynie zwrócić userów
        //zmiana kodu synchronicznego na asynchroniczny => dodanie async przed metodą następnie owinięcie zwracanej wartości w TASK<>
        //zmiana metod na asynchroniczne i dodanie await przed metodą asynchroniczną
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            /*
            //Przed wprowadzeniem repository pattern
            //ToList() potrzebuje LINQ
            //ToListAsync() potrzebuje Microsoft.EntityFrameworkCore;
            var users = await _context.Users.ToListAsync();

            return users;
            */


            var users = await _userRepository.GetMembersAsync();

           

            return Ok(users);

        }

        //  api/users/3 => zwróci pojedyńczego usera
        // zwracamy pojedyńczego usera więc IEnumerable nie jest nam potrzbene (to nie jest lista)

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            /*
            //przed repository pattern
            //Find() potrzebuje LINQ zwraca zmienną o określonym id
            //FindAsync() potrzebuje Microsoft.EntityFrameworkCore; zwraca zmienną o określonym id
            var user = await _context.Users.FindAsync(id);

            return user;

            */
            return await _userRepository.GetMemberAsync(username);
            
        }


    }
}