using System.Threading.Tasks;
using MassTransit;
using ReservationsAPI.Entities;
using Restaurant.Common.Contracts;
using Restaurant.Common.Repositories;

namespace ReservationsAPI.Consumers
{
    public class RoomUpdatedConsumer : IConsumer<RoomUpdated>
    {
        private readonly IRepository<Room> repository;

        public RoomUpdatedConsumer(IRepository<Room> repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<RoomUpdated> context)
        {
            var message = context.Message;

            var room = await repository.GetAsync(message.id);

            if (room is null)
            {
                room = new Room {
                    Id = message.id,
                    Name = message.Name,
                    Capacity = message.Capacity
                };

                await repository.CreateAsync(room);
            } else 
            {
                room.Name = message.Name;
                room.Capacity = message.Capacity;

                await repository.UpdateAsync(room);
            }
        }
    }
}