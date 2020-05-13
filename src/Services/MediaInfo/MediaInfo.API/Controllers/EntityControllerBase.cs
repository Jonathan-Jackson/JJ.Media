using JJ.Media.Core.Entities;
using JJ.Media.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JJ.Media.MediaInfo.API.Controllers {

    public class EntityControllerBase<TEntity> : ControllerBase where TEntity : Entity {
        private readonly IRepository<TEntity> _repository;

        public EntityControllerBase(IRepository<TEntity> repository) {
            _repository = repository;
        }

        protected async Task<IActionResult> HandleEntityRequest<TInput, TResult>(TInput criteria, Func<TInput, Task<TEntity>> getAction, Func<TEntity, Task<TResult>> requestAction) {
            var entity = await getAction(criteria);
            if (entity == null)
                return NotFound();
            else
                return Ok(await requestAction(entity));
        }

        protected async Task<IActionResult> HandleEntityRequest<TInput, TResult>(TInput criteria, Func<TInput, Task<TEntity>> getAction, Func<TEntity, TResult> requestAction) {
            var entity = await getAction(criteria);
            if (entity == null)
                return NotFound();
            else
                return Ok(requestAction(entity));
        }

        protected async Task<IActionResult> HandleEntityRequest<TResult>(int id, Func<int, Task<TEntity>> getAction, Func<TEntity, Task<TResult>> requestAction) {
            if (id < 1)
                return BadRequest();

            var entity = await getAction(id);
            if (entity == null)
                return NotFound();
            else
                return Ok(await requestAction(entity));
        }

        protected async Task<IActionResult> HandleEntityRequest<TResult>(int id, Func<int, Task<TEntity>> getAction, Func<TEntity, TResult> requestAction) {
            if (id < 1)
                return BadRequest();

            var entity = await getAction(id);
            if (entity == null)
                return NotFound();
            else
                return Ok(requestAction(entity));
        }

        protected async Task<IActionResult> HandleEntityRequest<TResult>(int id, Func<TEntity, Task<TResult>> requestAction)
            => await HandleEntityRequest(id, _repository.FindAsync, requestAction);

        protected async Task<IActionResult> HandleEntityRequest<TResult>(int id, Func<TEntity, TResult> requestAction)
            => await HandleEntityRequest(id, _repository.FindAsync, requestAction);
    }
}