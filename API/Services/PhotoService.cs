using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Services.API;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account
        (
            config.Value.cloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(acc);

    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

         if (file.Length > 0)
         {
           using var stream = file.OpenReadStream();
           var uploadParams = new ImageUploadParams
           {
             File = new FileDescription(file.FileName, stream),
             Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
             Folder = "da-net7u"
            };
             uploadResult = await _cloudinary.UploadAsync(uploadParams);
           }

     return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
}