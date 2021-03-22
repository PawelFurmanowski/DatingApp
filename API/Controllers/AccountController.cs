using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Sprawdzamy czy user istnieje,
            // jeśli istnieje zwracamy BadRequest 
            //możemy zwrócić BadRequest ponieważ używamy ActionResult
            //BadRequest zwraca status 400
            if (await UserExist(registerDto.Username))
                return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            //używamy singleordefault ponieważ jeśli znajdziemy więcej niż jedną wartość otrzymamy błąd
            //w przeciwieństwie do firstordefault która zwróci null jeśli nie znajdzie wartości
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            //po raz kolejny 
            //możemy użyć unauthorized ponieważ kożystamy z ActionResult co pozwala na zwracanie statusów
            if (user == null)
                return Unauthorized("Invalid username");

            //przeciążenie hmac inicjalizowane z wcześniej wygenerowanym byte[]
            using var hmac = new HMACSHA512(user.PasswordSalt);

            //generowanie hash'a na podstawie podanego hasła podczas logowania
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            //pętla sprawdzająca czy obliczony przez nas hash z podanego hasła zgadza się z hashem zapisanym na koncie
            //jeśli się ne zgadza zwracamy Unouthorized
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        //Metoda pomagająca stwierdzić czy user istnieje już w naszej bazie czy nie zwraca true lub false
        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}