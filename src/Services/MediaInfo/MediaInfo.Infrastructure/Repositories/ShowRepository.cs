﻿using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using JJ.Media.MediaInfo.Services.Interfaces;
using MediaInfo.Domain.Helpers.DTOs.Shows;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaInfo.Infrastructure.Repositories {

    /// <summary>
    /// Processes entity data logic for Episodes.
    /// </summary>
    public class ShowRepository : Repository<Show>, IShowRepository {
        private const string ShowTitlesTable = "ShowTitles";

        public ShowRepository(IDbConnectionFactory dbFactory, Compiler sqlCompiler)
            : base("Shows", dbFactory, sqlCompiler) {
        }

        public override async Task<Pagination<Show>> FindPaginatedAsync(int count, int skip) {
            var pagination = await base.FindPaginatedAsync(count, skip);
            await PopulateShowTitles(pagination.Items);

            return pagination;
        }

        /// <summary>
        /// Returns a matching Show with the title.
        /// </summary>
        public async Task<Show?> FindAsync(string title) {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            int showId = await Execute((DisposableQueryFactory db)
                    => db.Query(ShowTitlesTable)
                        .Select("ShowId")
                        .Where("Title", title)
                        .FirstOrDefaultAsync<int>()
            );

            if (showId > 0)
                return await FindAsync(showId);
            else
                return null;
        }

        /// <summary>
        /// Returns matching Shows with the titles.
        /// </summary>
        public async Task<IEnumerable<Show>> FindAsync(IEnumerable<string> titles) {
            if (titles?.Any() != true)
                return Enumerable.Empty<Show>();

            int[] showIds = (await Execute((DisposableQueryFactory db)
                    => db.Query(ShowTitlesTable)
                        .Select("ShowId")
                        .WhereIn("Title", titles)
                        .GetAsync<int>()))
                   .ToArray();

            if (showIds.Length > 0)
                return await FindAsync(showIds);
            else
                return Enumerable.Empty<Show>();
        }

        public override async Task<Show> FindAsync(int id) {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id));

            // load shows
            var show = await base.FindAsync(id);
            if (show != null) {
                // load titles
                var titles = await Execute((DisposableQueryFactory db)
                                => db.Query(ShowTitlesTable)
                                            .Where("ShowId", id)
                                            .GetAsync<ShowTitle>());

                show.Titles = titles.ToList();
            }

            return show;
        }

        /// <summary>
        /// Returns matching Shows with the ids supplied.
        /// </summary>
        public override async Task<IEnumerable<Show>> FindAsync(IEnumerable<int> ids) {
            if (ids?.Any() != true)
                return Enumerable.Empty<Show>();

            var shows = await base.FindAsync(ids);
            await PopulateShowTitles(shows, ids);

            return shows;
        }

        /// <summary>
        /// Returns the titles related to a collection of show ids.
        /// </summary>
        public async Task<ShowTitle[]> FindTitlesAsync(IEnumerable<int> ids) {
            return (await Execute((DisposableQueryFactory db)
                => db.Query(ShowTitlesTable)
                        .WhereIn("ShowId", ids)
                        .GetAsync<ShowTitle>())
                ).ToArray();
        }

        private async Task PopulateShowTitles(IEnumerable<Show> shows, IEnumerable<int> ids = null) {
            if (ids == null)
                ids = shows.Select(show => show.Id);

            var titles = await FindTitlesAsync(ids);
            foreach (var show in shows) {
                show.Titles = titles.Where(title => title.ShowId == show.Id).ToList();
            }
        }

        /// <summary>
        /// Inserts a show into the repository.
        /// </summary>
        /// <returns>The new Id.</returns>
        public override async Task<int> InsertAsync(Show show) {
            if (show == null)
                throw new ArgumentNullException(nameof(show));

            // Look into setting up a transaction!
            int showId = await Execute((DisposableQueryFactory db)
                => db.Query(_tableName).InsertGetIdAsync<int>(new {
                    show.Overview,
                    show.AirDate,
                    show.TvDbId
                })
            );

            // Need to look into doing this as bulk!
            // This entire idea needs revisting!
            foreach (var showTitle in show.Titles) {
                await Execute((DisposableQueryFactory db)
                    => db.Query(ShowTitlesTable).InsertAsync(new {
                        showTitle.IsPrimary,
                        showTitle.Title,
                        ShowId = showId
                    })
                );
            }

            return showId;
        }

        /// <summary>
        /// Inserts a collection of entities.
        /// </summary>
        /// <returns>The new Ids.</returns>
        public override async Task<IEnumerable<int>> InsertAsync(IEnumerable<Show> shows) {
            if (shows?.Any() != true)
                return Enumerable.Empty<int>();

            return await Task.WhenAll(shows.Select(InsertAsync));
        }

        /// <summary>
        /// Updates a show within the repository.
        /// </summary>
        /// <returns>Records changed count.</returns>
        public override Task<int> UpdateAsync(Show show) {
            if (show == null)
                throw new ArgumentNullException(nameof(show));

            // Some complex checking here to also insert any missing titles.
            throw new System.NotImplementedException();
        }
    }
}