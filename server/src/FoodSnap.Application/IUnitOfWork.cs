using System.Threading.Tasks;
using FoodSnap.Application.Restaurants;

namespace FoodSnap.Application
{
    public interface IUnitOfWork
    {
        IRestaurantRepository RestaurantRepository { get; }
        IRestaurantManagerRepository RestaurantManagerRepository { get; }
        IEventRepository EventRepository { get; }

        Task Commit();
    }
}
