using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class WhenRecipeIsRecordedMade
{
    private const string ActorUserId = "actor-user";
    private static readonly Guid RecipeId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
    private static readonly Guid CookbookId = Guid.Parse("11111111-2222-3333-4444-555555555555");

    private Recipe _recipe = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _recipe = Recipe();
        _recipe.RecordMade(ActorUserId);
    }

    [Test]
    public void MadeCountIsOne() => Assert.That(_recipe.MadeCount, Is.EqualTo(1));

    [Test]
    public void RaisesOneDomainEvent() => Assert.That(_recipe.DomainEvents, Has.Count.EqualTo(1));

    [Test]
    public void EventNamesTheActor() =>
        Assert.That(MadeEvent().ActorUserId, Is.EqualTo(ActorUserId));

    [Test]
    public void EventNamesTheRecipe() =>
        Assert.That(MadeEvent().RecipeId, Is.EqualTo(RecipeId));

    [Test]
    public void EventNamesTheCookbook() =>
        Assert.That(MadeEvent().CookbookId, Is.EqualTo(CookbookId));

    [Test]
    public void SecondRecordingIncrementsAgain()
    {
        var recipe = Recipe();

        recipe.RecordMade(ActorUserId);
        recipe.RecordMade(ActorUserId);

        Assert.That(recipe.MadeCount, Is.EqualTo(2));
    }

    [Test]
    public void BlankActorIsRejected()
    {
        var recipe = Recipe();

        Assert.That(() => recipe.RecordMade(" "), Throws.ArgumentException);
    }

    private RecipeMadeEvent MadeEvent() => _recipe.DomainEvents.OfType<RecipeMadeEvent>().Single();

    private static Recipe Recipe() => new()
    {
        Id = RecipeId,
        CookbookId = CookbookId,
        Title = "Test Recipe",
        Timing = new Timing(null, null, null),
        DietaryTags = new DietaryTags(null, null, null, null, null, null, null, null),
        MealTypes = new MealTypes(null, null, null, null, null)
    };
}
