using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
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

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //powinniśmy dostać username z tokenu któr został pobrany podczas autentykacji 
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            //Mapujemy obiekt memberUpdateDto na user autoMapper "przepisuje" wartości
            //user.City = memberUpdateDto.City
            //user.Country = mebberUpdateDto.Country ...
            _mapper.Map(memberUpdateDto, user);

            //aktualizujemy usera w bazie danych
            _userRepository.Update(user);

            //jeśli uda się poprawinie zapisać zmiany kończymy funkcję jeśli nie zwracamy badRequest
            if (await _userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Failed to update user");

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // 1 - potrzebujemy usera więc go wczytujemy po nazwie
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            //2 - następnie wczytujemy wynik z PhotoService
            var result = await _photoService.AddPhotoAsync(file);

            //3 - sprawdzamy czy nie ma żadnego błędu podczas wczytywania zdjęcia 
            if(result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            //4 - tworzymy nowe zdjęcie jeśli przeszlićmy wcześniejszy punkt
            var photo = new Photo 
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            //5 - sprawdzamy czy user ma jakieś zdjęcia, jeśli nie ma to ustawiamy pierwsze zdjęcie jako main
            if(user.Photos.Count == 0)
                photo.IsMain = true;
            
            //6 - dodajemy zdjęcie 
            user.Photos.Add(photo);

            //7 - zwracamy zdjęcie
            if(await _userRepository.SaveAllAsync())
                return _mapper.Map<PhotoDto>(photo);

            //8 - jeśłi nie udało się zwrócić zdjęcia zwracamy bad request
            return BadRequest("Problem adding photo");


        }


    }
}