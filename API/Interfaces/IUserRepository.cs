using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserRepositoty
{
    void Update(AppUser user);

    Task<bool> SaveAllAsync();

    Task<IEnumerable<AppUser>> GetUserAsync();

    Task<AppUser> GetUserByIdAsync(int id);

    Task<AppUser> GetUserByNameAsync(string name);

    Task<IEnumerable<MemberDto>> GetMembersAsync();
    Task<MemberDto> GetMemberAsync(string username);
}