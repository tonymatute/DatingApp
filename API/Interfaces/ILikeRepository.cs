using API.DTOs;
using API.Entities;
using API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);
        Task<AppUsers> GetUserWithLikes(int userId);
        Task<PageList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}
