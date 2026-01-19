using Core.Dtos;

namespace Business.Services;

public interface ILoginService
{
    Task<UserLoginResultDto> LoginAsync(LoginDto loginDto);
}
