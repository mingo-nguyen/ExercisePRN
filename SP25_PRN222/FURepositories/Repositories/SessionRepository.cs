using FUBusiness.Data;
using FUBusiness.Entities;
using FURepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Collections.Specialized.BitVector32;

namespace FURepositories.Repositories
{
    public class SessionRepository : ISessionRepository
    {

        private readonly FUDbContext _context;
        public SessionRepository(FUDbContext context)
        {
            _context = context;
        }

        public async Task<Sessions> AddAsync(Sessions session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            // Check if the session already exists
            if (await _context.Sessions.AnyAsync(s => s.SessionId == session.SessionId))
                throw new InvalidOperationException($"A session with ID {session.SessionId} already exists.");

            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task DeleteAsync(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException(nameof(sessionId));

            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (sessions.Any())
            {
                _context.Sessions.RemoveRange(sessions);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentNullException(nameof(sessionId));

            return await _context.Sessions.AnyAsync(s => s.SessionId == sessionId);
        }


        public Task<IEnumerable<Sessions>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Sessions> GetByIdAsync(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException(nameof(sessionId));
            }
            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        public async Task<Sessions> GetByUserIdAsync(int userId)
        {
            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public Task UpdateAsync(Sessions session)
        {
            throw new NotImplementedException();
        }
    }
}
