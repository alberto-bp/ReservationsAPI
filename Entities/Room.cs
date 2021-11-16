using System;
using Restaurant.Common.Entities;

namespace ReservationsAPI.Entities
{

    public class Room : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }

    }
}