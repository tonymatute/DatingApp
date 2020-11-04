using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
   public interface IUserRepository
    {
        void Update(AppUsers user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUsers>> GetUsersAsync();
        Task<AppUsers> GetUserByIdAsync(int id);
        Task<AppUsers> GetUserByUsernameAsync(string username);
        Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<MemberDto> GetMemberAsync(string username);
    }
}
