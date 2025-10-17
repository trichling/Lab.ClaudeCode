using FluentAssertions;
using LogicQuiz.Api.Data;
using LogicQuiz.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Xunit;

namespace LogicQuiz.Api.Tests.Data;

public class DbSeederTests : IDisposable
{
    private readonly QuizDbContext _context;
    private const int ExpectedQuestionCount = 32;
    private const int MaxCharacterLimit = 1000;
    private const int MinSentencesPerQuestion = 2;
    private const int MaxSentencesPerQuestion = 5;  // Allow up to 5 sentences for contextual paragraphs
    private const int MinQuestionsPerFallacy = 3;

    public DbSeederTests()
    {
        var options = new DbContextOptionsBuilder<QuizDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new QuizDbContext(options);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task SeedAsync_ShouldSeedExactly32Questions()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questionCount = await _context.Questions.CountAsync();
        questionCount.Should().Be(ExpectedQuestionCount,
            "the DbSeeder should seed exactly 32 questions as specified in the requirements");
    }

    [Fact]
    public async Task SeedAsync_AllQuestions_ShouldRespectCharacterLimit()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questions = await _context.Questions.ToListAsync();
        questions.Should().NotBeEmpty();

        foreach (var question in questions)
        {
            question.Statement.Length.Should().BeLessOrEqualTo(MaxCharacterLimit,
                $"question '{question.Statement.Substring(0, Math.Min(50, question.Statement.Length))}...' " +
                $"exceeds the maximum character limit of {MaxCharacterLimit}");
        }
    }

    [Fact]
    public async Task SeedAsync_AllQuestions_ShouldHaveValidFallacyTypeAssociations()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questions = await _context.Questions
            .Include(q => q.CorrectFallacyType)
            .ToListAsync();

        questions.Should().NotBeEmpty();

        foreach (var question in questions)
        {
            question.CorrectFallacyTypeId.Should().BeGreaterThan(0,
                "each question should have a valid fallacy type ID");

            question.CorrectFallacyType.Should().NotBeNull(
                "each question should have a valid fallacy type association");
        }
    }

    [Fact]
    public async Task SeedAsync_ShouldSeed8FallacyTypes()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var fallacyTypes = await _context.FallacyTypes.ToListAsync();
        fallacyTypes.Should().HaveCount(8,
            "the DbSeeder should seed exactly 8 fallacy types");
    }

    [Fact]
    public async Task SeedAsync_EachFallacyType_ShouldHaveAtLeastThreeQuestions()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var fallacyTypes = await _context.FallacyTypes
            .Include(f => f.Questions)
            .ToListAsync();

        fallacyTypes.Should().NotBeEmpty();

        foreach (var fallacyType in fallacyTypes)
        {
            fallacyType.Questions.Should().HaveCountGreaterOrEqualTo(MinQuestionsPerFallacy,
                $"fallacy type '{fallacyType.Name}' should have at least {MinQuestionsPerFallacy} questions");
        }
    }

    [Fact]
    public async Task SeedAsync_AllQuestions_ShouldContainContextualStructure()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questions = await _context.Questions.ToListAsync();
        questions.Should().NotBeEmpty();

        foreach (var question in questions)
        {
            // Count sentences by looking for sentence-ending punctuation (., !, ?)
            var sentenceCount = CountSentences(question.Statement);

            sentenceCount.Should().BeInRange(MinSentencesPerQuestion, MaxSentencesPerQuestion,
                $"question '{question.Statement.Substring(0, Math.Min(50, question.Statement.Length))}...' " +
                $"should contain {MinSentencesPerQuestion}-{MaxSentencesPerQuestion} sentences for contextual depth");
        }
    }

    [Fact]
    public async Task SeedAsync_AllQuestions_ShouldBeInGermanLanguage()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questions = await _context.Questions.ToListAsync();
        questions.Should().NotBeEmpty();

        foreach (var question in questions)
        {
            // Check for common German indicators
            var hasGermanCharacters = ContainsGermanIndicators(question.Statement);

            hasGermanCharacters.Should().BeTrue(
                $"question '{question.Statement.Substring(0, Math.Min(50, question.Statement.Length))}...' " +
                "should be in German language");
        }
    }

    [Fact]
    public async Task SeedAsync_WhenCalledMultipleTimes_ShouldNotDuplicateData()
    {
        // Act
        await DbSeeder.SeedAsync(_context);
        var firstQuestionCount = await _context.Questions.CountAsync();
        var firstFallacyCount = await _context.FallacyTypes.CountAsync();

        // Seed again
        await DbSeeder.SeedAsync(_context);
        var secondQuestionCount = await _context.Questions.CountAsync();
        var secondFallacyCount = await _context.FallacyTypes.CountAsync();

        // Assert
        firstQuestionCount.Should().Be(secondQuestionCount,
            "seeding multiple times should not create duplicate questions");

        firstFallacyCount.Should().Be(secondFallacyCount,
            "seeding multiple times should not create duplicate fallacy types");
    }

    [Fact]
    public async Task SeedAsync_WhenDatabaseHasExistingData_ShouldNotSeed()
    {
        // Arrange - Add a single fallacy type to simulate existing data
        _context.FallacyTypes.Add(new FallacyType
        {
            Name = "Existing",
            Description = "Existing fallacy",
            Difficulty = 1
        });
        await _context.SaveChangesAsync();

        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var fallacyCount = await _context.FallacyTypes.CountAsync();
        fallacyCount.Should().Be(1,
            "seeding should be skipped when fallacy types already exist");

        var questionCount = await _context.Questions.CountAsync();
        questionCount.Should().Be(0,
            "no questions should be seeded when seeding is skipped");
    }

    [Fact]
    public async Task SeedAsync_AdHominemQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var adHominem = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Ad Hominem");

        adHominem.Should().NotBeNull();
        adHominem!.Questions.Should().HaveCount(4,
            "Ad Hominem should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_StrawManQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var strawMan = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Strohmann-Argument");

        strawMan.Should().NotBeNull();
        strawMan!.Questions.Should().HaveCount(4,
            "Strohmann-Argument should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_FalseDichotomyQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var falseDichotomy = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Falsche Dichotomie");

        falseDichotomy.Should().NotBeNull();
        falseDichotomy!.Questions.Should().HaveCount(4,
            "Falsche Dichotomie should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_ArgumentumAdPopulumQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var argumentumAdPopulum = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Argumentum ad Populum");

        argumentumAdPopulum.Should().NotBeNull();
        argumentumAdPopulum!.Questions.Should().HaveCount(4,
            "Argumentum ad Populum should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_PostHocQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var postHoc = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Post Hoc Ergo Propter Hoc");

        postHoc.Should().NotBeNull();
        postHoc!.Questions.Should().HaveCount(4,
            "Post Hoc Ergo Propter Hoc should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_CircularReasoningQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var circularReasoning = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Zirkelschluss");

        circularReasoning.Should().NotBeNull();
        circularReasoning!.Questions.Should().HaveCount(4,
            "Zirkelschluss should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_TuQuoqueQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var tuQuoque = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Tu Quoque");

        tuQuoque.Should().NotBeNull();
        tuQuoque!.Questions.Should().HaveCount(4,
            "Tu Quoque should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_ArgumentumAdIgnorantiamQuestions_ShouldHaveCorrectFallacyType()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var argumentumAdIgnorantiam = await _context.FallacyTypes
            .Include(f => f.Questions)
            .FirstOrDefaultAsync(f => f.Name == "Argumentum ad Ignorantiam");

        argumentumAdIgnorantiam.Should().NotBeNull();
        argumentumAdIgnorantiam!.Questions.Should().HaveCount(4,
            "Argumentum ad Ignorantiam should have exactly 4 questions based on the seeder");
    }

    [Fact]
    public async Task SeedAsync_FallacyTypes_ShouldHaveCorrectDifficultyLevels()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var easyFallacies = await _context.FallacyTypes
            .Where(f => f.Difficulty == 1)
            .ToListAsync();
        easyFallacies.Should().HaveCount(3,
            "there should be 3 easy (difficulty 1) fallacy types");

        var mediumFallacies = await _context.FallacyTypes
            .Where(f => f.Difficulty == 2)
            .ToListAsync();
        mediumFallacies.Should().HaveCount(2,
            "there should be 2 medium (difficulty 2) fallacy types");

        var hardFallacies = await _context.FallacyTypes
            .Where(f => f.Difficulty == 3)
            .ToListAsync();
        hardFallacies.Should().HaveCount(3,
            "there should be 3 hard (difficulty 3) fallacy types");
    }

    [Fact]
    public async Task SeedAsync_AllQuestions_ShouldNotBeEmpty()
    {
        // Act
        await DbSeeder.SeedAsync(_context);

        // Assert
        var questions = await _context.Questions.ToListAsync();
        questions.Should().NotBeEmpty();

        foreach (var question in questions)
        {
            question.Statement.Should().NotBeNullOrWhiteSpace(
                "all questions should have non-empty statements");

            question.Statement.Length.Should().BeGreaterThan(50,
                "contextual questions should be substantially longer than simple sentences");
        }
    }

    [Fact]
    public async Task SeedAsync_DatabaseConstraints_ShouldBeRespected()
    {
        // Act
        await DbSeeder.SeedAsync(_context);
        await _context.SaveChangesAsync();

        // Assert - If SaveChanges completes without exception, constraints are respected
        var questions = await _context.Questions.ToListAsync();
        questions.Should().HaveCount(ExpectedQuestionCount);

        var fallacyTypes = await _context.FallacyTypes.ToListAsync();
        fallacyTypes.Should().HaveCount(8);

        // Verify all foreign key relationships are valid
        foreach (var question in questions)
        {
            var fallacyExists = await _context.FallacyTypes
                .AnyAsync(f => f.Id == question.CorrectFallacyTypeId);

            fallacyExists.Should().BeTrue(
                $"question with ID {question.Id} should reference a valid fallacy type");
        }
    }

    // Helper Methods

    private static int CountSentences(string text)
    {
        // Count sentences by finding periods, exclamation marks, and question marks
        // followed by a space or end of string
        var matches = Regex.Matches(text, @"[.!?](?:\s|$)");
        return matches.Count;
    }

    private static bool ContainsGermanIndicators(string text)
    {
        // Check for German-specific characteristics:
        // 1. German umlauts (ä, ö, ü, ß)
        // 2. Common German words
        // 3. German sentence structure indicators

        var hasUmlauts = Regex.IsMatch(text, @"[äöüßÄÖÜ]");

        var germanWords = new[]
        {
            "der", "die", "das", "und", "ist", "nicht", "mit", "ein", "eine",
            "dass", "für", "auf", "von", "zu", "im", "werden", "kann", "sind",
            "wenn", "aber", "oder", "weil", "als", "über", "auch", "sein", "seine"
        };

        var lowerText = text.ToLower();
        var hasGermanWords = germanWords.Any(word =>
            Regex.IsMatch(lowerText, $@"\b{word}\b"));

        return hasUmlauts || hasGermanWords;
    }
}
