using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReservationsAPI.DTOs;
using ReservationsAPI.Entities;
using ReservationsAPI.Extensions;
using ReservationsAPI.Services;
using Restaurant.Common.Repositories;

namespace ReservationsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IRepository<Reservation> _reservationsRepository;
        private readonly IRepository<Room> roomsRepository;

        public ReservationsController(IRepository<Reservation> reservationsRepository, IRepository<Room> roomsRepository)
        {
            _reservationsRepository = reservationsRepository;
            this.roomsRepository = roomsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ReservationDto>> Get()
        {
            var reservations = await _reservationsRepository.GetAllAsync();
            var rooms = await roomsRepository.GetAllAsync();

            var reservationsDTOs = reservations.Select(r => r.AsDto(rooms.Where(ro => ro.Id == r.RoomId).Single().Name));

            return reservationsDTOs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetById(Guid id)
        {
            var reservation = await _reservationsRepository.GetAsync(id);

            if (reservation is null)
            {
                return NotFound();
            }

            var room = await roomsRepository.GetAsync(reservation.RoomId);

            return reservation.AsDto(room.Name);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> Post(CreateReservationDto newReservation)
        {
            var reservation = new Reservation
            {
                RoomId = newReservation.RoomId,
                Responsible = newReservation.Responsible,
                PartySize = newReservation.PartySize,
                DueDate = newReservation.DueDate,
                CreatedDate = DateTimeOffset.UtcNow,
                PhoneNumber = newReservation.PhoneNumber
            };

            await _reservationsRepository.CreateAsync(reservation);

            var room = await roomsRepository.GetAsync(reservation.RoomId);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation.AsDto(room.Name));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateReservationDto updateReservation)
        {
            var existing = await _reservationsRepository.GetAsync(id);

            if (existing is null)
            {
                return NotFound();
            }

            existing.Responsible = updateReservation.Responsible;
            existing.DueDate = updateReservation.DueDate;
            existing.PartySize = updateReservation.PartySize;
            existing.PhoneNumber = updateReservation.PhoneNumber;
            existing.RoomId = updateReservation.RoomId;

            await _reservationsRepository.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _reservationsRepository.GetAsync(id);

            if (existing is null)
            {
                return NotFound();
            }

            await _reservationsRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}