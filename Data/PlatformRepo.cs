using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        void IPlatformRepo.CreatePlatform(Platform plat)
        {
            if(plat == null)
            {
                throw new System.ArgumentNullException(nameof(plat));
            }

            _context.Platforms.Add(plat);
        }

        IEnumerable<Platform> IPlatformRepo.GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        Platform IPlatformRepo.GetPlatformById(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }
    }
}