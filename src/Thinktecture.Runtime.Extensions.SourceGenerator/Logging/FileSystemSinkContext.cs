using Thinktecture.CodeAnalysis;

namespace Thinktecture.Logging;

public class FileSystemSinkContext
{
   private readonly object _lock;
   private readonly List<WeakReference<ThinktectureSourceGeneratorBase>> _owners;

   public string OriginalFilePath { get; }
   public bool FilePathIsUnique { get; }
   public FileSystemLoggingSink Sink { get; }

   public FileSystemSinkContext(
      string originalFilePath,
      bool filePathIsUnique,
      FileSystemLoggingSink sink,
      ThinktectureSourceGeneratorBase owner)
   {
      OriginalFilePath = originalFilePath;
      FilePathIsUnique = filePathIsUnique;
      Sink = sink;
      _lock = new object();
      _owners = new List<WeakReference<ThinktectureSourceGeneratorBase>> { new(owner) };
   }

   public bool HasOwners()
   {
      lock (_lock)
      {
         return _owners.Count > 0;
      }
   }

   public void AddOwner(ThinktectureSourceGeneratorBase owner)
   {
      lock (_lock)
      {
         _owners.Add(new(owner));
      }
   }

   public void RemoveOwner(ThinktectureSourceGeneratorBase owner)
   {
      lock (_lock)
      {
         // The owner might be in the collection multiple times, remove it just once.
         // This happens if the log level but not the log file path is changed.
         var isRemovedOnce = false;

         _owners.RemoveAll(r =>
                           {
                              if (!r.TryGetTarget(out var generator))
                                 return true;

                              if (isRemovedOnce || generator != owner)
                                 return false;

                              isRemovedOnce = true;
                              return true;
                           });
      }
   }

   public void RemoveReclaimedOwners()
   {
      lock (_lock)
      {
         _owners.RemoveAll(r => !r.TryGetTarget(out _));
      }
   }
}
