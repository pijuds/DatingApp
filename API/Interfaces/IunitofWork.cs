namespace API.Interfaces;

public interface IuintofWork
{
    IUserRepositoty UserRepository{get;}
    IMessageRepository MessageRepository{get;}

    ILikesRepository LikeRepository{get;}
    IPhotoRepository PhotoRepository { get; }
    Task<bool> Complete();

    bool HasChanges();
}