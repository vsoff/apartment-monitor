namespace Apartment.Data
{
    public interface IDatabaseContextProvider
    {
        ApplicationContext Create();
    }
}