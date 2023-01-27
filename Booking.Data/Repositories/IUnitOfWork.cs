namespace Booking.Data.Repositories
{
    public interface IUnitOfWork
    {
        IGymClassRepository GymClassRepository { get; }

        Task CompleteAsync();
    }
}