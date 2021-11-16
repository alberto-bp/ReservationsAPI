using System;
using Restaurant.Common.Entities;

namespace ReservationsAPI.Entities
{

    public class Reservation : IEntity
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string Responsible { get; set; }
        public int PartySize { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

    }
}