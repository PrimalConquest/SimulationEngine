using LoadoutComunication;

namespace DBWrapper.Source.Models.Mappers
{
    public class UserInfoMapper : IDTOMapper<UserInfo, LoadoutDTO>
    {
        public static UserInfo FromDTO(LoadoutDTO DTO)
        {
            UserInfo userInfo = new();
            userInfo.UserId = DTO.PlayerId;
            userInfo.OfficerIds = DTO.OfficerIds;
            userInfo.CommanderId = DTO.CommanderId;
            return userInfo;
        }

        public static LoadoutDTO ToDTO(UserInfo Model)
        {
            LoadoutDTO dto = new LoadoutDTO();
            dto.PlayerId = Model.UserId;
            dto.OfficerIds = Model.OfficerIds;
            dto.CommanderId=Model.CommanderId;
            return dto;
        }

    }
}
