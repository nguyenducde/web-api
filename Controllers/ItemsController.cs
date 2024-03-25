using Catalog.Dtos;
using Catalog.Entities;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;
using Catalog;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase{
        private readonly IItemRepository repository;
        public ItemsController(IItemRepository repository){

            this.repository  = repository;
        }

        [HttpGet]
        public IEnumerable<ItemDto> GetItems(){
            var items =repository.GetItemsAsync().Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItem(Guid id){
            var item = repository.GetItemAsync(id);
            return item.AsDto();
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto){
            Item item = new Item(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            repository.CreateItemAsync(item);
            
            return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.AsDto());
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto){
        
            var existingItem = repository.GetItemAsync(id);
            if(existingItem.Name is null){
                return NotFound();
            }
            Item updatedItem = existingItem with {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
            repository.UpdateItemAsync(updatedItem);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteItem(Guid id){
             var existingItem = repository.GetItemAsync(id);
            if(existingItem?.Name is null){
                return NotFound();
            }
            repository.DeleteItemAsync(id);
            return NoContent();
        }

     }

}