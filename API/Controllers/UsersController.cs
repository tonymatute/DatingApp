using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseAPIController
    {
       
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository,IMapper mapper)
        {
            
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<MemberDto>>> GetUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
           
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update User!");

        }

    }
}
