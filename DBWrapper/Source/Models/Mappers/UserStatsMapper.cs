using DBWrapper.Source.Models;
using LoadoutComunication;

namespace DBWrapper.Source.Models.Mappers
{
    public class UserStatsMapper : IDTOMapper<UserStats, UserStatsDTO>
    {
        public static UserStats FromDTO(UserStatsDTO dto)
        {
            return new UserStats
            {
                RankPoints = dto.RankPoints,
            };
        }

        public static UserStatsDTO ToDTO(UserStats model)
        {
            return new UserStatsDTO
            {
                RankPoints = model.RankPoints,
            };
        }
    }
}
