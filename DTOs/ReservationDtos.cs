using System;
using System.ComponentModel.DataAnnotations;

namespace ReservationsAPI.DTOs
{
    public record ReservationDto(Guid id, string Room, string Responsible, int PartySize, DateTimeOffset DueDate);
    public record CreateReservationDto([Required] Guid RoomId, [Required] string Responsible, [Range(2, 30)] int PartySize, [Required] string PhoneNumber, [Required] DateTimeOffset DueDate);
    public record UpdateReservationDto([Required] Guid RoomId, [Required] string Responsible, [Range(2, 30)] int PartySize, [Required] string PhoneNumber, [Required] DateTimeOffset DueDate);
}