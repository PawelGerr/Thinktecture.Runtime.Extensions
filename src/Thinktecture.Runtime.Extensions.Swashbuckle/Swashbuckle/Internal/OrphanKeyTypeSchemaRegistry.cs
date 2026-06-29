using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Thinktecture.Swashbuckle.Internal;

/// <summary>
/// Tracks the schema ids of key-type components (e.g. an enum used as the key of a Smart Enum) that Swashbuckle
/// registers while generating the owning type, so that <see cref="RemoveOrphanedKeyTypeSchemasDocumentFilter"/>
/// can remove them again if they end up unreferenced.
/// </summary>
/// <remarks>
/// Entries are kept per <see cref="SchemaRepository"/> (i.e. per generated document) via a
/// <see cref="ConditionalWeakTable{TKey,TValue}"/>, so state never leaks between documents and is collected
/// automatically once the repository is no longer used.
/// </remarks>
internal sealed class OrphanKeyTypeSchemaRegistry
{
   private readonly ConditionalWeakTable<SchemaRepository, ConcurrentDictionary<string, byte>> _candidatesByRepository = new();

   /// <summary>
   /// Records <paramref name="schemaId"/> as a removal candidate for the document represented by
   /// <paramref name="schemaRepository"/>.
   /// </summary>
   public void Register(SchemaRepository schemaRepository, string schemaId)
   {
      var candidates = _candidatesByRepository.GetOrCreateValue(schemaRepository);
      candidates.TryAdd(schemaId, 0);
   }

   /// <summary>
   /// Gets the removal candidates recorded for the document represented by <paramref name="schemaRepository"/>.
   /// </summary>
   public IReadOnlyCollection<string> GetCandidates(SchemaRepository schemaRepository)
   {
      return _candidatesByRepository.TryGetValue(schemaRepository, out var candidates)
                ? candidates.Keys.ToArray()
                : [];
   }
}
