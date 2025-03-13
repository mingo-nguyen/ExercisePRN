using FUBusiness.Data;
using FUBusiness.Entities;
using FURepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FURepositories.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly FUDbContext _context;

        public EnrollmentRepository(FUDbContext context)
        {
            _context = context;
        }

        public async Task<EnrollmentRecords> AddAsync(EnrollmentRecords enrollment)
        {
            await _context.EnrollmentRecords.AddAsync(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await _context.EnrollmentRecords.FindAsync(id);
            if (enrollment != null)
            {
                _context.EnrollmentRecords.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EnrollmentRecords.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EnrollmentRecords>> GetAllAsync()
        {
            return await _context.EnrollmentRecords
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<EnrollmentRecords> GetByIdAsync(int id)
        {
            return await _context.EnrollmentRecords
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EnrollmentRecords>> GetByCourseIdAsync(int courseId)
        {
            return await _context.EnrollmentRecords
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentRecords>> GetByUserIdAsync(int userId)
        {
            return await _context.EnrollmentRecords
                .Include(e => e.User)
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> isEnrolledAsync(int userId, int courseId)
        {
            return await _context.EnrollmentRecords
                .AnyAsync(e => e.UserId == userId &&
                          e.CourseId == courseId &&
                          !e.Dropped);
        }

        public async Task UpdateAsync(EnrollmentRecords enrollment)
        {
            _context.Entry(enrollment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
