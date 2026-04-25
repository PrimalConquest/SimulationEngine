using LoadoutComunication;

namespace DBWrapper.Source.Models.Mappers
{
    public class UserMapper : IDTOMapper<User, AccountDTO>
    {
        public static User FromDTO(AccountDTO DTO)
        {
            User user = new();
            user.UserName = DTO.UserName;
            user.Password = DTO.Password;
            user.Email = DTO.Email;
            return user;
        }

        public static AccountDTO ToDTO(User Model)
        {
            AccountDTO dto = new();
            dto.UserName = Model.UserName;
            dto.Password = Model.Password;
            dto.Email = Model.Email;
            return dto;
        }
    }
}
