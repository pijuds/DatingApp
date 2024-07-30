using API.Interfaces;
using AutoMapper;

namespace API.Data;

public class UnitofWork : IuintofWork
{
    private readonly IMapper _mapper;

    private readonly DataContext _context;
    public UnitofWork(DataContext context,IMapper mapper)
    {
        _mapper=mapper;
        _context=context;

    }
    public IUserRepositoty UserRepository => new UserRepository(_context,_mapper);

    public IMessageRepository MessageRepository => new MessageRepository(_context,_mapper);

    public ILikesRepository LikeRepository => new LikesRepository(_context);

    public IPhotoRepository PhotoRepository => new PhotoRepository(_context);

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync()>0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}