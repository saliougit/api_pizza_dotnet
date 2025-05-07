using Microsoft.EntityFrameworkCore;

namespace Pizzéria.Models
{
    public class PizzaEhodDB : DbContext
    {
        public PizzaEhodDB(DbContextOptions<PizzaEhodDB> options)
            : base(options) { }

        public DbSet<PizzaEhod> Pizzas => Set<PizzaEhod>();
    }
}
