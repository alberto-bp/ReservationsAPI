using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ReservationsAPI.Entities;

namespace ReservationsAPI.Services
{
    public class RoomsService
    {
        private readonly HttpClient client;

        public RoomsService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Room> GetRoom(Guid id)
        {
            var room = await client.GetFromJsonAsync<Room>($"/rooms/{id}");
            return room;
        }
    }
}