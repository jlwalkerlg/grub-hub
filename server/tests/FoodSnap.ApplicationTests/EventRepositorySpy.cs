using System.Collections.Generic;
using System.Threading.Tasks;
using FoodSnap.Application;

namespace FoodSnap.ApplicationTests
{
    public class EventRepositorySpy : IEventRepository
    {
        public List<Event> Events = new List<Event>();

        public Task Add<TEvent>(TEvent ev) where TEvent : Event
        {
            Events.Add(ev);
            return Task.CompletedTask;
        }
    }
}
