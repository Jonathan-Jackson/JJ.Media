using JJ.Media.MediaInfo.Core.Models;
using JJ.Media.MediaInfo.Data.Interfaces;
using JJ.Media.MediaInfo.Data.Models;
using JJ.Media.MediaInfo.Services.Interfaces;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.Data.Repositories {

    public class ShowRepository : Repository<Show>, IShowRepository {
        private const string ShowTitlesTable = "ShowTitles";

        public ShowRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("Shows", dbFactory, sqlCompiler) {
        }

        public async Task<Show?> FindAsync(string title) {
            int showId = await Execute(async (DisposableQuery db)
                    => await db.Query(ShowTitlesTable)
                        .Select("ShowId")
                        .Where("Title", title)
                        .FirstOrDefaultAsync<int>()
            );

            if (showId > 0) {
                return await FindAsync(showId);
            }
            else {
                return null;
            }
        }

        public async Task<IEnumerable<Show>> FindAsync(IEnumerable<string> titles) {
            int[] showIds = await Execute(async (DisposableQuery db)
                    => (await db.Query(ShowTitlesTable)
                        .Select("ShowId")
                        .Where("Title", titles)
                        .GetAsync<int>()).ToArray()
            );

            if (showIds.Length > 0) {
                return await FindAsync(showIds);
            }
            else {
                return Enumerable.Empty<Show>();
            }
        }
    }
}