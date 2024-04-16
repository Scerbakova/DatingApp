using API.Interfaces;
using AutoMapper;

namespace API.Data;

public class UnitOfWork(DataContext context, IMapper mapper) : IUnitOfWork
{
    private readonly DataContext _context = context;
    private readonly IMapper _mapper = mapper;
    public IUserRepository UserRepository => new UserRepository(_context, _mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

    public ILikesRepository LikesRepository => new LikesRepository(_context);

    public IPhotoRepository PhotosRepository => new PhotoRepository(_context);

    public async Task<bool> Complete()
    {
         return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
