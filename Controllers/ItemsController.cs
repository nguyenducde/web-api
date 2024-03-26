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
        public async Task<IEnumerable<ItemDto>> GetItems(){
            var items =(await repository.GetItemsAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public  async Task <ActionResult<ItemDto>> GetItemAsync(Guid id){
            var item = await repository.GetItemAsync(id);
            return item.AsDto();
        }

        [HttpPost]
        public async Task <ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto){
            Item item = new Item(){
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
           await repository.CreateItemAsync(item);
            
            return CreatedAtAction(nameof(GetItemAsync), new {id = item.Id}, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task <ActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto){
        
            var existingItem = await repository.GetItemAsync(id);
            if(existingItem.Name is null){
                return NotFound();
            }
            Item updatedItem = existingItem with {
                Name = itemDto.Name,
                Price = itemDto.Price
            };
           await repository.UpdateItemAsync(updatedItem);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync(Guid id){
             var existingItem = await repository.GetItemAsync(id);
            if(existingItem?.Name is null){
                return NotFound();
            }
            await repository.DeleteItemAsync(id);
            return NoContent();
        }

     }

}