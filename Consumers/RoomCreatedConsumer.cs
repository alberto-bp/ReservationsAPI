using System.Threading.Tasks;
using MassTransit;
using ReservationsAPI.Entities;
using Restaurant.Common.Contracts;
using Restaurant.Common.Repositories;

namespace ReservationsAPI.Consumers
{
    public class RoomCreatedConsumer : IConsumer<RoomCreated>
    {
        private readonly IRepository<Room> repository;
        public RoomCreatedConsumer(IRepository<Room> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<RoomCreated> context)
        {
            var message = context.Message;

            var room = await repository.GetAsync(message.id);

            if (room is null)
            {
                var newRoom = new Room {
                    Id = message.id,
                    Name = message.Name,
                    Capacity = message.Capacity
                };
                
                await repository.CreateAsync(newRoom);
            }

            return;
        }
    }
}