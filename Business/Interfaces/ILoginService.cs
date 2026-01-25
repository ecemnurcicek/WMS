using Core.Dtos;

namespace Business.Interfaces;

public interface ILoginService
{
    Task<UserLoginResultDto> LoginAsync(LoginDto loginDto);
}
