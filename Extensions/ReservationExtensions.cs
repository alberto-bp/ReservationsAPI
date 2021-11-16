using System;
using ReservationsAPI.DTOs;
using ReservationsAPI.Entities;

namespace ReservationsAPI.Extensions
{
    public static class ReservationExtensions
    {
        public static ReservationDto AsDto(this Reservation reservation, string room)
        {
            return new ReservationDto(reservation.Id, room, reservation.Responsible, reservation.PartySize, reservation.DueDate);
        }
    }
}