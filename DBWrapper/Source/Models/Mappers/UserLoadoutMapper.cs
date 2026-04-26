using DBWrapper.Source.Models;
using LoadoutComunication;

namespace DBWrapper.Source.Models.Mappers
{
    public class UserLoadoutMapper : IDTOMapper<UserLoadout, LoadoutDTO>
    {
        public static UserLoadout FromDTO(LoadoutDTO dto)
        {
            return new UserLoadout
            {
                CommanderId = dto.CommanderId,
                OfficerIds  = dto.OfficerIds,
            };
        }

        public static LoadoutDTO ToDTO(UserLoadout model)
        {
            return new LoadoutDTO
            {
                CommanderId = model.CommanderId,
                OfficerIds  = model.OfficerIds,
            };
        }
    }
}
