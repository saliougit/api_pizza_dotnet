using Microsoft.EntityFrameworkCore;
using Pizzéria.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration de SQLite
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";
builder.Services.AddSqlite<PizzaEhodDB>(connectionString);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// === Fonction de validation des pizzas ===
bool IsValid(PizzaEhod pizza, out string error)
{
    if (string.IsNullOrWhiteSpace(pizza.Nom))
    {
        error = "Le champ 'Nom' est requis et ne peut pas être vide.";
        return false;
    }
    if (string.IsNullOrWhiteSpace(pizza.Description))
    {
        error = "Le champ 'Description' est requis et ne peut pas être vide.";
        return false;
    }

    error = string.Empty;
    return true;
}


// === ROUTES ===

// GET - Lire toutes les pizzas
app.MapGet("/pizzas", async (PizzaEhodDB db) =>
    await db.Pizzas.ToListAsync());

// GET - Lire une pizza par ID
app.MapGet("/pizza/{id}", async (PizzaEhodDB db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    return pizza is not null
        ? Results.Ok(pizza)
        : Results.NotFound($"Pizza avec ID {id} introuvable.");
});

// POST - Créer une pizza
app.MapPost("/pizza", async (PizzaEhodDB db, PizzaEhod pizza) =>
{
    if (!IsValid(pizza, out var error))
        return Results.BadRequest(error);

    db.Pizzas.Add(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});

// PUT - Modifier une pizza
app.MapPut("/pizza/{id}", async (PizzaEhodDB db, int id, PizzaEhod input) =>
{
    if (!IsValid(input, out var error))
        return Results.BadRequest(error);

    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
        return Results.NotFound($"Aucune pizza trouvée avec l'ID {id}");

    pizza.Nom = input.Nom;
    pizza.Description = input.Description;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// DELETE - Supprimer une pizza
app.MapDelete("/pizza/{id}", async (PizzaEhodDB db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
        return Results.NotFound($"Impossible de supprimer : aucune pizza avec ID {id}");

    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok($"Pizza avec ID {id} supprimée avec succès.");
});

app.Run();




//using Microsoft.EntityFrameworkCore;
//using Pizzéria.Models;

//var builder = WebApplication.CreateBuilder(args);

////// Ajouter EF Core InMemory
////builder.Services.AddDbContext<PizzaEhodDB>(options => options.UseInMemoryDatabase("items"));

//var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";
//builder.Services.AddSqlite<PizzaEhodDB>(connectionString);



//// Ajouter Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Middleware Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// ROUTES

//// Lire toutes les pizzas
//app.MapGet("/pizzas", async (PizzaEhodDB db) =>
//    await db.Pizzas.ToListAsync());

//// Lire une pizza par ID
//app.MapGet("/pizza/{id}", async (PizzaEhodDB db, int id) =>
//{
//    var pizza = await db.Pizzas.FindAsync(id);
//    return pizza is not null ? Results.Ok(pizza) : Results.NotFound($"Pizza avec ID {id} introuvable.");
//});


//// Créer une pizza
//app.MapPost("/pizza", async (PizzaEhodDB db, PizzaEhod pizza) =>
//{
//    if (string.IsNullOrWhiteSpace(pizza.Nom) || string.IsNullOrWhiteSpace(pizza.Description))
//    {
//        return Results.BadRequest("Le nom et la description sont obligatoires.");
//    }

//    db.Pizzas.Add(pizza);
//    await db.SaveChangesAsync();
//    return Results.Created($"/pizza/{pizza.Id}", pizza);
//});


//// Mettre à jour une pizza
//app.MapPut("/pizza/{id}", async (PizzaEhodDB db, int id, PizzaEhod input) =>
//{
//    if (string.IsNullOrWhiteSpace(input.Nom) || string.IsNullOrWhiteSpace(input.Description))
//    {
//        return Results.BadRequest("Le nom et la description sont obligatoires.");
//    }

//    var pizza = await db.Pizzas.FindAsync(id);
//    if (pizza is null) return Results.NotFound($"Aucune pizza trouvée avec l'ID {id}");

//    pizza.Nom = input.Nom;
//    pizza.Description = input.Description;
//    await db.SaveChangesAsync();

//    return Results.NoContent();
//});


//// Supprimer une pizza
//app.MapDelete("/pizza/{id}", async (PizzaEhodDB db, int id) =>
//{
//    var pizza = await db.Pizzas.FindAsync(id);
//    if (pizza is null) return Results.NotFound($"Impossible de supprimer : aucune pizza avec ID {id}");

//    db.Pizzas.Remove(pizza);
//    await db.SaveChangesAsync();
//    return Results.Ok($"Pizza avec ID {id} supprimée avec succès.");
//});


//app.Run();
