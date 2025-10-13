using LogicQuiz.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LogicQuiz.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        // Check if data already exists
        if (await context.FallacyTypes.AnyAsync())
        {
            return;
        }

        // Difficulty 1 - Easy level fallacies
        var adHominem = new FallacyType
        {
            Name = "Ad Hominem",
            Description = "Angriff auf die Person statt auf das Argument",
            Difficulty = 1
        };

        var strawMan = new FallacyType
        {
            Name = "Strohmann-Argument",
            Description = "Verzerren oder vereinfachen der gegnerischen Position, um sie leichter angreifen zu können",
            Difficulty = 1
        };

        var falscheDichotomie = new FallacyType
        {
            Name = "Falsche Dichotomie",
            Description = "Präsentation von nur zwei Optionen, obwohl mehr existieren",
            Difficulty = 1
        };

        // Difficulty 2 - Medium level fallacies
        var argumentumAdPopulum = new FallacyType
        {
            Name = "Argumentum ad Populum",
            Description = "Etwas ist wahr, weil viele Menschen es glauben",
            Difficulty = 2
        };

        var postHocErgoPropterHoc = new FallacyType
        {
            Name = "Post Hoc Ergo Propter Hoc",
            Description = "Falsche Kausalität - nur weil A vor B geschah, hat A nicht unbedingt B verursacht",
            Difficulty = 2
        };

        // Difficulty 3 - Hard level fallacies
        var circularReasoning = new FallacyType
        {
            Name = "Zirkelschluss",
            Description = "Die Schlussfolgerung ist bereits in den Prämissen enthalten",
            Difficulty = 3
        };

        var tuQuoque = new FallacyType
        {
            Name = "Tu Quoque",
            Description = "Ablenkung durch Hinweis auf Heuchelei des Gegners",
            Difficulty = 3
        };

        var appellanIgnorantiam = new FallacyType
        {
            Name = "Argumentum ad Ignorantiam",
            Description = "Etwas ist wahr, weil es nicht widerlegt wurde",
            Difficulty = 3
        };

        context.FallacyTypes.AddRange(
            adHominem, strawMan, falscheDichotomie,
            argumentumAdPopulum, postHocErgoPropterHoc,
            circularReasoning, tuQuoque, appellanIgnorantiam
        );

        await context.SaveChangesAsync();

        // Create questions with at least 3 examples per fallacy type
        var questions = new List<Question>
        {
            // Ad Hominem examples
            new Question
            {
                Statement = "Du kannst nicht über Klimawandel reden, du fährst ja selbst Auto!",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Seine Theorie kann nicht stimmen, er ist ja nur ein Student ohne Abschluss.",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Warum sollte ich auf deine Gesundheitstipps hören? Du bist doch selbst übergewichtig!",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Der Politiker fordert mehr Transparenz, aber er hat selbst Steuern hinterzogen!",
                CorrectFallacyTypeId = adHominem.Id
            },

            // Strohmann-Argument examples
            new Question
            {
                Statement = "Du willst die Verteidigungsausgaben senken? Also willst du unser Land wehrlos machen!",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "Du bist gegen diese Steuererhöhung? Du willst also, dass Schulen und Krankenhäuser geschlossen werden!",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "Du glaubst an die Evolution? Also denkst du, dass Menschen direkt von Affen abstammen!",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "Du willst strengere Waffengesetze? Du willst also allen Menschen ihre Waffen wegnehmen!",
                CorrectFallacyTypeId = strawMan.Id
            },

            // Falsche Dichotomie examples
            new Question
            {
                Statement = "Entweder unterstützt du diese Maßnahme vollständig oder du bist gegen unser Land!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "Du bist entweder mit uns oder gegen uns - es gibt keinen Mittelweg!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "Entweder wir bauen neue Kohlekraftwerke oder wir haben bald keinen Strom mehr.",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "Wenn du nicht für die Todesstrafe bist, dann bist du auf der Seite der Verbrecher.",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },

            // Argumentum ad Populum examples
            new Question
            {
                Statement = "Millionen Menschen können nicht irren - diese Diät muss funktionieren!",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "Die meisten Menschen glauben daran, also muss es wahr sein.",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "Alle kaufen dieses Produkt, es muss das Beste sein!",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "99% der Bürger sind für diese Maßnahme, wie kann sie dann falsch sein?",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },

            // Post Hoc Ergo Propter Hoc examples
            new Question
            {
                Statement = "Seit dem neuen Bürgermeister gewählt wurde, ist die Kriminalität gesunken. Er muss ein ausgezeichneter Bürgermeister sein!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "Ich habe Vitamin C genommen und am nächsten Tag war meine Erkältung weg. Vitamin C heilt Erkältungen!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "Nach der Einführung von Videospielen ist die Jugendgewalt gestiegen. Videospiele verursachen Gewalt!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "Jedes Mal wenn ich meinen Glückshut trage, gewinnt meine Mannschaft. Der Hut bringt uns Glück!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },

            // Zirkelschluss examples
            new Question
            {
                Statement = "Die Bibel ist wahr, weil Gott sie geschrieben hat. Und wir wissen, dass Gott existiert, weil es in der Bibel steht.",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "Ich bin vertrauenswürdig, weil ich immer die Wahrheit sage. Und du kannst mir glauben, dass ich die Wahrheit sage, weil ich vertrauenswürdig bin.",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "Das Gesetz ist gerecht, weil es das Richtige vorschreibt. Und wir wissen, dass es richtig ist, weil es im Gesetz steht.",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "Diese Nachrichtenquelle ist zuverlässig, weil sie akkurate Informationen liefert. Und die Informationen sind akkurat, weil sie von einer zuverlässigen Quelle stammen.",
                CorrectFallacyTypeId = circularReasoning.Id
            },

            // Tu Quoque examples
            new Question
            {
                Statement = "Du kritisierst meine Ernährung? Du isst doch selbst Fast Food!",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "Wie kannst du mich für zu schnelles Fahren kritisieren, wenn du letzte Woche selbst geblitzt wurdest?",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "Du sagst, ich soll nicht lügen? Aber du hast letztes Jahr auch gelogen!",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "Der Senator kritisiert Korruption, aber er hat selbst Bestechungsgelder angenommen!",
                CorrectFallacyTypeId = tuQuoque.Id
            },

            // Argumentum ad Ignorantiam examples
            new Question
            {
                Statement = "Niemand hat je bewiesen, dass Außerirdische nicht existieren, also müssen sie existieren!",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "Es wurde nie nachgewiesen, dass diese Behandlung nicht funktioniert, also muss sie wirksam sein.",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "Da niemand das Gegenteil beweisen kann, muss meine Theorie richtig sein.",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "Es gibt keinen Beweis, dass Geister nicht existieren, deshalb glaube ich an sie.",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            }
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
    }
}
