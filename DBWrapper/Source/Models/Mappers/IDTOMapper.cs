namespace DBWrapper.Source.Models.Mappers
{
    public interface IDTOMapper<T, D>
    {
        static abstract T FromDTO(D DTO);
        static abstract D ToDTO(T Model);
    }
}
