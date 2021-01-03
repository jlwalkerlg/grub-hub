using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Restaurants;
using Domain.Restaurants;

namespace ApplicationTests.Restaurants
{
    public class CuisineRepositorySpy : ICuisineRepository
    {
        public List<Cuisine> Cuisines = new();

        public Task<List<Cuisine>> All()
        {
            return Task.FromResult(Cuisines);
        }

        public Task Add(Cuisine cuisine)
        {
            Cuisines.Add(cuisine);
            return Task.CompletedTask;
        }
    }
}
