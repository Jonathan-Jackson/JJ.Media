using JJ.Framework.Repository;
using JJ.Framework.Repository.Abstraction;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace JJ.Framework.Controller {

    public abstract class EntityController<TEntity> : ControllerBase where TEntity : Entity {
        private readonly IRepository<TEntity> _repository;

        public EntityController(IRepository<TEntity> repository) {
            _repository = repository;
        }

        protected async Task<IActionResult> AddEntity(TEntity entity) {
            if (entity.Id > 0)
                return BadRequest($"Entity has an id and cannot be saved.");

            entity.Id = await _repository.InsertAsync(entity);

            return Ok(JsonSerializer.Serialize(entity));
        }

        protected async Task<IActionResult> FindEntities(int id, Func<int, Task<TEntity[]>> findAction) {
            if (id < 1)
                return BadRequest($"Id must be greater than zero.");

            return Ok(JsonSerializer.Serialize(await findAction(id)));
        }

        protected async Task<IActionResult> FindEntity<TInput, TResult>(TInput criteria, Func<TInput, Task<TEntity>> findAction, Func<TEntity, Task<TResult>> requestAction) {
            var entity = await findAction(criteria);
            if (entity == null)
                return NotFound();
            else
                return Ok(JsonSerializer.Serialize(await requestAction(entity)));
        }

        protected async Task<IActionResult> FindEntity<TInput, TResult>(TInput criteria, Func<TInput, Task<TEntity>> findAction, Func<TEntity, TResult> requestAction) {
            var entity = await findAction(criteria);
            if (entity == null)
                return NotFound();
            else
                return Ok(JsonSerializer.Serialize(requestAction(entity)));
        }

        protected async Task<IActionResult> FindEntity<TResult>(int id, Func<int, Task<TEntity>> findAction, Func<TEntity, Task<TResult>> requestAction) {
            if (id < 1)
                return BadRequest($"Id must be greater than zero.");

            var entity = await findAction(id);
            if (entity == null)
                return NotFound();
            else
                return Ok(JsonSerializer.Serialize(await requestAction(entity)));
        }

        protected async Task<IActionResult> FindEntity<TResult>(int id, Func<int, Task<TEntity>> findAction, Func<TEntity, TResult> requestAction) {
            if (id < 1)
                return BadRequest($"Id must be greater than zero.");

            var entity = await findAction(id);
            if (entity == null)
                return NotFound();
            else
                return Ok(JsonSerializer.Serialize(requestAction(entity)));
        }

        protected async Task<IActionResult> FindEntity(int id) {
            if (id < 1)
                return BadRequest($"Id must be greater than zero.");

            var entity = await _repository.FindAsync(id);
            if (entity == null)
                return NotFound();
            else
                return Ok(JsonSerializer.Serialize(entity));
        }

        protected Task<IActionResult> FindEntity<TResult>(int id, Func<TEntity, Task<TResult>> requestAction)
            => FindEntity(id, _repository.FindAsync, requestAction);

        protected Task<IActionResult> FindEntity<TResult>(int id, Func<TEntity, TResult> requestAction)
            => FindEntity(id, _repository.FindAsync, requestAction);
    }
}