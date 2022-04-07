using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        readonly DataContext _context;
        public PhotoRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _context.Photos
                                        .IgnoreQueryFilters()
                                        .SingleOrDefaultAsync(photo => photo.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _context.Photos
                                        .IgnoreQueryFilters()
                                        .Where(p => p.IsApproved == false)
                                        .Select(u => new PhotoForApprovalDto
                                                                             {
                                                                                PhotoId = u.Id,
                                                                                Username = u.AppUser.UserName,
                                                                                PhotoUrl = u.Url,
                                                                                IsApproved = u.IsApproved
                                                                             })
                                                                                .ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}