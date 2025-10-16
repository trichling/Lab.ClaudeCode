using Microsoft.EntityFrameworkCore;
using LogicQuiz.Api.Data;
using LogicQuiz.Api.Models;
using LogicQuiz.Api.Services;

namespace LogicQuiz.Api.Tests.Services;

public class QuizServiceTests
{
    private QuizDbContext CreateInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        return new QuizDbContext(options);
    }

    private async Task<QuizDbContext> SeedTestData(string databaseName)
    {
        var context = CreateInMemoryContext(databaseName);

        // Clear existing data
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Create fallacy types with different difficulties
        var fallacyTypes = new List<FallacyType>
        {
            new() { Id = 1, Name = "Ad Hominem", Description = "Attacking the person", Difficulty = 1 },
            new() { Id = 2, Name = "Strohmann", Description = "Misrepresenting argument", Difficulty = 1 },
            new() { Id = 3, Name = "Falsches Dilemma", Description = "Only two options", Difficulty = 1 },
            new() { Id = 4, Name = "Slippery Slope", Description = "Chain reaction", Difficulty = 2 },
            new() { Id = 5, Name = "Hasty Generalization", Description = "Too quick conclusion", Difficulty = 2 },
            new() { Id = 6, Name = "Post Hoc", Description = "False causality", Difficulty = 2 },
            new() { Id = 7, Name = "Tu Quoque", Description = "You too fallacy", Difficulty = 3 },
            new() { Id = 8, Name = "Zirkelschluss", Description = "Circular reasoning", Difficulty = 3 },
            new() { Id = 9, Name = "Appeal to Authority", Description = "Wrong authority", Difficulty = 3 },
            new() { Id = 10, Name = "Red Herring", Description = "Distraction", Difficulty = 3 }
        };

        context.FallacyTypes.AddRange(fallacyTypes);

        // Create questions with different correct answers
        var questions = new List<Question>
        {
            new() { Id = 1, Statement = "Question 1", CorrectFallacyTypeId = 1 },
            new() { Id = 2, Statement = "Question 2", CorrectFallacyTypeId = 2 },
            new() { Id = 3, Statement = "Question 3", CorrectFallacyTypeId = 3 },
            new() { Id = 4, Statement = "Question 4", CorrectFallacyTypeId = 4 },
            new() { Id = 5, Statement = "Question 5", CorrectFallacyTypeId = 5 },
            new() { Id = 6, Statement = "Question 6", CorrectFallacyTypeId = 6 },
            new() { Id = 7, Statement = "Question 7", CorrectFallacyTypeId = 7 },
            new() { Id = 8, Statement = "Question 8", CorrectFallacyTypeId = 8 },
            new() { Id = 9, Statement = "Question 9", CorrectFallacyTypeId = 9 },
            new() { Id = 10, Statement = "Question 10", CorrectFallacyTypeId = 10 },
            new() { Id = 11, Statement = "Question 11", CorrectFallacyTypeId = 1 },
            new() { Id = 12, Statement = "Question 12", CorrectFallacyTypeId = 2 }
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task StartGameAsync_Easy_ReturnsGameWithCorrectFallacyNamesIncluded()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count); // Easy = 3 questions
        Assert.Equal(3, result.AvailableFallacies.Count); // Easy = 3 options

        // Verify all correct answers are in the fallacy types
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_Medium_ReturnsGameWithCorrectFallacyNamesIncluded()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Questions.Count); // Medium = 5 questions
        Assert.Equal(5, result.AvailableFallacies.Count); // Medium = 5 options

        // Verify all correct answers are in the fallacy types
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_Hard_ReturnsGameWithCorrectFallacyNamesIncluded()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Questions.Count); // Hard = 10 questions
        
        // Hard mode should have at least 8 options, but may have more if all correct answers need to be included
        Assert.True(result.AvailableFallacies.Count >= 8, 
            $"Expected at least 8 fallacies, but got {result.AvailableFallacies.Count}");

        // Verify all correct answers are in the fallacy types
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_MultipleGames_CorrectFallacyNamesAlwaysIncluded()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act & Assert - Run multiple games to test randomization
        for (int i = 0; i < 10; i++)
        {
            var result = await service.StartGameAsync($"Player{i}", 2);

            Assert.NotNull(result);
            Assert.Equal(5, result.Questions.Count);
            Assert.Equal(5, result.AvailableFallacies.Count);

            // Verify all correct answers are in the fallacy types
            foreach (var question in result.Questions)
            {
                Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
            }
        }
    }

    [Fact]
    public async Task StartGameAsync_DuplicateCorrectFallacyNames_HandledCorrectly()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        
        // Add more questions with duplicate correct answers
        var additionalQuestions = new List<Question>
        {
            new() { Id = 13, Statement = "Question 13", CorrectFallacyTypeId = 1 },
            new() { Id = 14, Statement = "Question 14", CorrectFallacyTypeId = 1 },
            new() { Id = 15, Statement = "Question 15", CorrectFallacyTypeId = 1 }
        };
        context.Questions.AddRange(additionalQuestions);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count);
        Assert.Equal(3, result.AvailableFallacies.Count);

        // Even if multiple questions have the same correct answer, 
        // all questions should still be answerable
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_FallacyTypesAreShuffled()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act - Run multiple games and collect the order of fallacy types
        var orders = new List<List<int>>();
        for (int i = 0; i < 10; i++)
        {
            var result = await service.StartGameAsync($"Player{i}", 1);
            orders.Add(result.AvailableFallacies.Select(ft => ft.Id).ToList());
        }

        // Assert - Not all orders should be the same (with high probability)
        // Compare first order with others
        var firstOrder = orders[0];
        var allSame = orders.All(order => order.SequenceEqual(firstOrder));
        
        Assert.False(allSame, "Fallacy types should be shuffled, but they appear in the same order every time");
    }

    [Fact]
    public async Task StartGameAsync_HighDifficultyQuestionsCanAppearInEasyMode()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Create fallacy types - only difficulty 1
        var fallacyTypes = new List<FallacyType>
        {
            new() { Id = 1, Name = "Fallacy 1", Description = "Desc 1", Difficulty = 1 },
            new() { Id = 2, Name = "Fallacy 2", Description = "Desc 2", Difficulty = 1 },
            new() { Id = 3, Name = "Fallacy 3", Description = "Desc 3", Difficulty = 1 }
        };
        context.FallacyTypes.AddRange(fallacyTypes);

        // Create questions where one has a high-difficulty correct answer
        var questions = new List<Question>
        {
            new() { Id = 1, Statement = "Question 1", CorrectFallacyTypeId = 1 },
            new() { Id = 2, Statement = "Question 2", CorrectFallacyTypeId = 2 },
            new() { Id = 3, Statement = "Question 3", CorrectFallacyTypeId = 3 }
        };
        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count);
        
        // All correct answers must be included
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_MoreRequiredFallaciesThanTargetCount_AllRequiredIncluded()
    {
        // Arrange - Create a scenario where we have more unique correct answers than the target count
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Create 5 fallacy types
        var fallacyTypes = new List<FallacyType>
        {
            new() { Id = 1, Name = "Fallacy 1", Description = "Desc 1", Difficulty = 1 },
            new() { Id = 2, Name = "Fallacy 2", Description = "Desc 2", Difficulty = 1 },
            new() { Id = 3, Name = "Fallacy 3", Description = "Desc 3", Difficulty = 1 },
            new() { Id = 4, Name = "Fallacy 4", Description = "Desc 4", Difficulty = 2 },
            new() { Id = 5, Name = "Fallacy 5", Description = "Desc 5", Difficulty = 2 }
        };
        context.FallacyTypes.AddRange(fallacyTypes);

        // Create 3 questions with 3 different correct answers (for Easy mode which expects 3 options)
        var questions = new List<Question>
        {
            new() { Id = 1, Statement = "Question 1", CorrectFallacyTypeId = 1 },
            new() { Id = 2, Statement = "Question 2", CorrectFallacyTypeId = 2 },
            new() { Id = 3, Statement = "Question 3", CorrectFallacyTypeId = 3 }
        };
        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count);
        
        // All correct answers must be included
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
        
        // Should have exactly 3 fallacy types (the 3 required ones)
        Assert.Equal(3, result.AvailableFallacies.Count);
    }

    [Fact]
    public async Task StartGameAsync_FewerRequiredFallaciesThanTargetCount_FilledWithDistractors()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Create 6 fallacy types
        var fallacyTypes = new List<FallacyType>
        {
            new() { Id = 1, Name = "Fallacy 1", Description = "Desc 1", Difficulty = 1 },
            new() { Id = 2, Name = "Fallacy 2", Description = "Desc 2", Difficulty = 1 },
            new() { Id = 3, Name = "Fallacy 3", Description = "Desc 3", Difficulty = 1 },
            new() { Id = 4, Name = "Fallacy 4", Description = "Desc 4", Difficulty = 1 },
            new() { Id = 5, Name = "Fallacy 5", Description = "Desc 5", Difficulty = 1 },
            new() { Id = 6, Name = "Fallacy 6", Description = "Desc 6", Difficulty = 1 }
        };
        context.FallacyTypes.AddRange(fallacyTypes);

        // Create 3 questions, but 2 have the same correct answer
        // So we only have 2 unique required fallacy IDs
        var questions = new List<Question>
        {
            new() { Id = 1, Statement = "Question 1", CorrectFallacyTypeId = 1 },
            new() { Id = 2, Statement = "Question 2", CorrectFallacyTypeId = 1 }, // Same as Q1
            new() { Id = 3, Statement = "Question 3", CorrectFallacyTypeId = 2 }
        };
        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count);
        Assert.Equal(3, result.AvailableFallacies.Count); // Should be filled to 3
        
        // All correct answers must be included
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
        
        // Should contain IDs 1 and 2 (required) plus 1 additional distractor
        Assert.Contains(result.AvailableFallacies, ft => ft.Id == 1);
        Assert.Contains(result.AvailableFallacies, ft => ft.Id == 2);
    }

    [Fact]
    public async Task StartGameAsync_NotEnoughDistractors_ReturnsOnlyRequiredFallacies()
    {
        // Arrange - Create exactly 2 fallacy types but need 3 for Easy mode
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Create only 2 fallacy types
        var fallacyTypes = new List<FallacyType>
        {
            new() { Id = 1, Name = "Fallacy 1", Description = "Desc 1", Difficulty = 1 },
            new() { Id = 2, Name = "Fallacy 2", Description = "Desc 2", Difficulty = 1 }
        };
        context.FallacyTypes.AddRange(fallacyTypes);

        // Create 3 questions with 2 different correct answers
        var questions = new List<Question>
        {
            new() { Id = 1, Statement = "Question 1", CorrectFallacyTypeId = 1 },
            new() { Id = 2, Statement = "Question 2", CorrectFallacyTypeId = 2 },
            new() { Id = 3, Statement = "Question 3", CorrectFallacyTypeId = 1 }
        };
        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act
        var result = await service.StartGameAsync("TestPlayer", 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Questions.Count);
        
        // Should only have 2 fallacy types (no additional distractors available)
        Assert.Equal(2, result.AvailableFallacies.Count);
        
        // All correct answers must be included
        foreach (var question in result.Questions)
        {
            Assert.Contains(result.AvailableFallacies, ft => ft.Id == question.CorrectFallacyTypeId);
        }
    }

    [Fact]
    public async Task StartGameAsync_InvalidDifficulty_ThrowsArgumentException()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.StartGameAsync("TestPlayer", 0));
        
        await Assert.ThrowsAsync<ArgumentException>(() => 
            service.StartGameAsync("TestPlayer", 4));
    }

    [Fact]
    public async Task StartGameAsync_NotEnoughQuestions_ThrowsInvalidOperationException()
    {
        // Arrange
        var context = CreateInMemoryContext(Guid.NewGuid().ToString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Add only 1 question but Easy mode needs 3
        var fallacyType = new FallacyType 
        { 
            Id = 1, 
            Name = "Fallacy 1", 
            Description = "Desc 1", 
            Difficulty = 1 
        };
        context.FallacyTypes.Add(fallacyType);

        var question = new Question 
        { 
            Id = 1, 
            Statement = "Question 1", 
            CorrectFallacyTypeId = 1 
        };
        context.Questions.Add(question);
        await context.SaveChangesAsync();

        var service = new QuizService(context);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.StartGameAsync("TestPlayer", 1));
    }

    [Fact]
    public async Task SubmitAnswerAsync_CorrectFallacyName_ReturnsTrue()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);
        
        var gameState = await service.StartGameAsync("TestPlayer", 1);
        var firstQuestion = gameState.Questions.First();

        // Act
        var result = await service.SubmitAnswerAsync(
            gameState.SessionId, 
            firstQuestion.Id, 
            firstQuestion.CorrectFallacyTypeId);

        // Assert
        Assert.True(result.IsCorrect);
        Assert.NotNull(result.CorrectFallacyName);
    }

    [Fact]
    public async Task SubmitAnswerAsync_InCorrectFallacyName_ReturnsFalse()
    {
        // Arrange
        var context = await SeedTestData(Guid.NewGuid().ToString());
        var service = new QuizService(context);
        
        var gameState = await service.StartGameAsync("TestPlayer", 1);
        var firstQuestion = gameState.Questions.First();
        
        // Get a wrong answer (any fallacy ID that's not the correct one)
        var wrongAnswerId = gameState.AvailableFallacies
            .First(ft => ft.Id != firstQuestion.CorrectFallacyTypeId).Id;

        // Act
        var result = await service.SubmitAnswerAsync(
            gameState.SessionId, 
            firstQuestion.Id, 
            wrongAnswerId);

        // Assert
        Assert.False(result.IsCorrect);
        Assert.NotNull(result.CorrectFallacyName);
    }
}
