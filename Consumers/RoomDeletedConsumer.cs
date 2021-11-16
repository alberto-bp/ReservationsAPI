using System.Threading.Tasks;
using MassTransit;
using ReservationsAPI.Entities;
using Restaurant.Common.Contracts;
using Restaurant.Common.Repositories;

namespace ReservationsAPI.Consumers
{
    public class RoomDeletedConsumer : IConsumer<RoomDeleted>
    {
        private readonly IRepository<Room> repository;

        public RoomDeletedConsumer(IRepository<Room> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<RoomDeleted> context)
        {
            var message = context.Message;

            var room = await repository.GetAsync(message.id);

            if (room is null)
            {
                return;
            } else 
            {
                await repository.DeleteAsync(room.Id);
            }
        }
    }
}