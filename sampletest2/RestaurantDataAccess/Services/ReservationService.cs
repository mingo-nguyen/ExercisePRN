using RestaurantBusiness.Entities;
using RestaurantDataAccess.ServiceContracts;
using RestaurantRepositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantDataAccess.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITableRepository _tableRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            ITableRepository tableRepository)
        {
            _reservationRepository = reservationRepository;
            _tableRepository = tableRepository;
        }

        public async Task<Reservation> AddAsync(Reservation reservation)
        {
            // Validate if table is available for the requested time
            var table = await _tableRepository.GetByIdAsync(reservation.TableId);
            if (table == null || table.Status != "Available")
            {
                throw new InvalidOperationException("The selected table is no longer available.");
            }

            // Check if user already has a reservation at this time
            var existingReservations = await _reservationRepository.GetAllAsync();
            var userReservation = existingReservations.Any(r =>
                r.UserId == reservation.UserId &&
                r.ReservationTime.Date == reservation.ReservationTime.Date &&
                Math.Abs((r.ReservationTime - reservation.ReservationTime).TotalHours) < 2);

            if (userReservation)
            {
                throw new InvalidOperationException("You already have a reservation within this time period.");
            }

            // Check if table is already booked for this time
            var tableReservation = existingReservations.Any(r =>
                r.TableId == reservation.TableId &&
                r.ReservationTime.Date == reservation.ReservationTime.Date &&
                Math.Abs((r.ReservationTime - reservation.ReservationTime).TotalHours) < 2);

            if (tableReservation)
            {
                throw new InvalidOperationException("The selected table is already reserved for this time.");
            }

            // Everything is valid, create the reservation
            return await _reservationRepository.AddAsync(reservation);
        }

        public async Task DeleteAsync(int id)
        {
            await _reservationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _reservationRepository.GetAllAsync();
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _reservationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(int userId)
        {
            var allReservations = await _reservationRepository.GetAllAsync();
            return allReservations.Where(r => r.UserId == userId);
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}
