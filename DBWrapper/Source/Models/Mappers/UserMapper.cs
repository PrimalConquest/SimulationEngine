using AccountComunication;
using DBWrapper.Source.Models;

namespace DBWrapper.Source.Models.Mappers
{
    public class UserMapper : IDTOMapper<User, RegisterDTO>
    {
        public static User FromDTO(RegisterDTO dto)
        {
            return new User
            {
                UserName = dto.UserName,
                Email    = dto.Email
            };
        }

        public static RegisterDTO ToDTO(User model)
        {
            return new RegisterDTO
            {
                UserName = model.UserName ?? "",
                Email    = model.Email    ?? "",
            };
        }
    }
}
