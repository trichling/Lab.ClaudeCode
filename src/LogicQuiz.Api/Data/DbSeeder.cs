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
                Statement = "In einer Debatte über Maßnahmen gegen den Klimawandel präsentiert ein Wissenschaftler detaillierte Daten über CO2-Emissionen und deren Auswirkungen. Sein Diskussionspartner unterbricht ihn und sagt: Du kannst doch gar nicht glaubwürdig über Klimaschutz reden, du fährst ja selbst jeden Tag mit dem Auto zur Arbeit! Deine ganzen Argumente sind dadurch hinfällig.",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Ein junger Forscher veröffentlicht eine innovative Studie zur Quantenphysik, die mehrere bisherige Annahmen in Frage stellt. Die Arbeit wurde in einem renommierten Journal akzeptiert und von mehreren Experten positiv bewertet. Ein Professor aus einem anderen Fachbereich kommentiert: Seine Theorie kann unmöglich stimmen, schließlich ist er ja nur ein Student ohne Abschluss und hat keine jahrelange Erfahrung.",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Bei einer Diskussion über gesunde Ernährung und Bewegung erklärt ein Ernährungsberater die wissenschaftlichen Grundlagen ausgewogener Ernährung. Er zitiert aktuelle Studien und gibt konkrete Empfehlungen für einen gesünderen Lebensstil. Sein Gesprächspartner erwidert: Warum sollte ich überhaupt auf deine Gesundheitstipps hören? Du bist doch selbst übergewichtig, also kannst du mir nichts beibringen!",
                CorrectFallacyTypeId = adHominem.Id
            },
            new Question
            {
                Statement = "Ein Politiker hält eine Rede, in der er mehr Transparenz in der Regierung fordert und konkrete Maßnahmen zur Offenlegung von Interessenkonflikten vorschlägt. Seine Vorschläge sind detailliert ausgearbeitet und werden von Experten als sinnvoll bewertet. Ein Gegner kontert: Dieser Mann fordert Transparenz, aber er hat doch selbst vor Jahren Steuern hinterzogen! Alles, was er sagt, ist deshalb wertlos und unglaubwürdig.",
                CorrectFallacyTypeId = adHominem.Id
            },

            // Strohmann-Argument examples
            new Question
            {
                Statement = "In einer Parlamentsdebatte schlägt ein Abgeordneter vor, die Verteidigungsausgaben um 5% zu reduzieren und das Geld stattdessen in Cybersicherheit zu investieren. Er argumentiert mit veränderten Bedrohungslagen im digitalen Zeitalter. Sein Gegner springt auf und ruft empört: Du willst also die Verteidigungsausgaben senken? Das bedeutet, du willst unser Land völlig wehrlos und schutzlos machen! Du gefährdest die nationale Sicherheit!",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "Eine Bürgerin äußert Bedenken gegen eine geplante Steuererhöhung von 2% und schlägt vor, stattdessen durch effizientere Verwaltung Kosten einzusparen. Sie hat konkrete Beispiele für Einsparpotenziale recherchiert. Der Bürgermeister antwortet: Sie sind also gegen diese moderate Steuererhöhung? Dann wollen Sie wohl, dass wir alle Schulen und Krankenhäuser in unserer Stadt schließen müssen! Wie können Sie so verantwortungslos sein?",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "In einer Biologiestunde erklärt ein Schüler, dass er die Evolutionstheorie als wissenschaftlich fundiert ansieht, basierend auf fossilen Beweisen und genetischen Studien. Er betont die graduelle Entwicklung über Millionen von Jahre. Ein Mitschüler ruft dazwischen: Du glaubst also an die Evolution? Das heißt, du denkst ernsthaft, dass Menschen direkt von Affen abstammen und dein Urgroßvater ein Schimpanse war! Das ist doch absurd!",
                CorrectFallacyTypeId = strawMan.Id
            },
            new Question
            {
                Statement = "Nach einem tragischen Vorfall diskutiert eine Gemeinde über Waffengesetze. Ein Bürger schlägt vor, Hintergrundüberprüfungen zu verschärfen und Wartezeiten einzuführen, betont aber, dass legaler Waffenbesitz weiterhin möglich bleiben soll. Der Vorsitzende der örtlichen Sportschützen reagiert: Du willst also strengere Waffengesetze? Das bedeutet, du willst allen gesetzestreuen Bürgern ihre Waffen wegnehmen und uns zu wehrlosen Opfern machen!",
                CorrectFallacyTypeId = strawMan.Id
            },

            // Falsche Dichotomie examples
            new Question
            {
                Statement = "Die Regierung hat ein umfangreiches Gesetzespaket zur Inneren Sicherheit vorgelegt, das verschiedene Maßnahmen von Videoüberwachung bis zu erweiterten Polizeibefugnissen umfasst. Einige Bürger äußern Bedenken bezüglich einzelner Punkte und schlagen Modifikationen vor. Der Innenminister verkündet daraufhin in einer Pressekonferenz: Entweder Sie unterstützen diese Maßnahmen vollständig und ohne Wenn und Aber, oder Sie sind gegen die Sicherheit unseres Landes und stehen auf der Seite der Kriminellen!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "Ein Unternehmen plant eine umstrittene Umstrukturierung, die einige Mitarbeiter kritisch sehen. Sie schlagen alternative Ansätze vor und wünschen sich mehr Dialog. Der CEO beruft eine Versammlung ein und erklärt den Angestellten: Wir stehen vor einer wichtigen Entscheidung für die Zukunft unserer Firma. Du bist entweder vollständig mit uns und unterstützt diesen Weg, oder du bist gegen uns - es gibt hier keinen Mittelweg und keine Kompromisse!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "In einer Gemeindeversammlung wird über die zukünftige Energieversorgung diskutiert. Mehrere Experten haben verschiedene Optionen vorgestellt, darunter Windkraft, Solarenergie und Energieeffizienzmaßnahmen. Der Vorsitzende der lokalen Energiewerke meldet sich zu Wort: Entweder wir bauen jetzt sofort neue Kohlekraftwerke, oder wir werden in wenigen Jahren keinen Strom mehr haben. Das sind die einzigen beiden Möglichkeiten, die uns bleiben!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },
            new Question
            {
                Statement = "Nach einem aufsehenerregenden Kriminalfall entbrennt eine öffentliche Debatte über das Strafrechtssystem. Einige fordern härtere Strafen, während andere präventive Maßnahmen und Resozialisierung befürworten. Ein Talkshow-Gast erklärt mit erhobener Stimme: Die Sache ist ganz einfach - wenn du nicht für die Todesstrafe bist und die härtesten Strafen forderst, dann bist du automatisch auf der Seite der Verbrecher und unterstützt deren Taten!",
                CorrectFallacyTypeId = falscheDichotomie.Id
            },

            // Argumentum ad Populum examples
            new Question
            {
                Statement = "Eine neue Diät erobert die sozialen Medien im Sturm und wird von zahlreichen Influencern beworben. Wissenschaftliche Studien zur Wirksamkeit und Sicherheit dieser Ernährungsform fehlen jedoch weitgehend. Ein Verkäufer von Diätprodukten argumentiert: Millionen von Menschen weltweit folgen bereits dieser Diät und schwören darauf! So viele Menschen können sich unmöglich irren - diese Diät muss einfach funktionieren und gesund sein!",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "In einem Dorf gibt es seit Generationen den Glauben, dass ein bestimmter Stein Glück bringt und Krankheiten heilt. Obwohl es keine wissenschaftlichen Belege dafür gibt, wird die Tradition aufrechterhalten. Der älteste Bewohner des Dorfes erklärt Besuchern: Die allermeisten Menschen hier glauben fest an die heilende Kraft dieses Steins, und das schon seit Jahrhunderten. Wenn so viele Generationen daran glauben, dann muss es einfach wahr sein!",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "Ein neues Smartphone kommt auf den Markt und wird durch massive Werbekampagnen beworben. Obwohl Tests zeigen, dass es technisch nicht besser als günstigere Alternativen ist, wird es zum Verkaufsschlager. Ein Verkäufer im Elektronikladen überzeugt Kunden: Schauen Sie sich die Verkaufszahlen an! Praktisch alle kaufen mittlerweile dieses Modell. Es muss einfach das beste Smartphone auf dem Markt sein, sonst würden nicht so viele Menschen es kaufen!",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },
            new Question
            {
                Statement = "Eine Stadtregierung plant den Bau eines großen Einkaufszentrums, obwohl Umweltgutachten vor negativen Folgen warnen. Eine Umfrage unter den Bürgern zeigt breite Zustimmung, hauptsächlich wegen der versprochenen Arbeitsplätze. Der Bürgermeister verkündet triumphierend: Die Umfrage zeigt klar, dass 99% unserer Bürger für dieses Projekt sind! Bei so einer überwältigenden Mehrheit - wie könnte diese Maßnahme dann falsch oder schädlich sein?",
                CorrectFallacyTypeId = argumentumAdPopulum.Id
            },

            // Post Hoc Ergo Propter Hoc examples
            new Question
            {
                Statement = "In einer mittelgroßen Stadt wurde vor zwei Jahren ein neuer Bürgermeister gewählt. In der gleichen Zeit ist die Kriminalitätsrate um 15% gesunken, was allerdings Teil eines landesweiten Trends ist. Auch investierte die Landesregierung zusätzliche Mittel in die Polizeiarbeit. Bei seiner Wiederwahl-Kampagne erklärt der Bürgermeister stolz: Seit ich gewählt wurde, ist die Kriminalität deutlich gesunken. Das beweist, dass ich ein ausgezeichneter Bürgermeister bin!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "Ein Mann fühlt sich erkältet und hat leichtes Fieber. Am Abend nimmt er hochdosiertes Vitamin C und geht früh schlafen. Am nächsten Morgen fühlt er sich besser, was bei Erkältungen nach ausreichend Schlaf normal ist. Begeistert erzählt er seinen Kollegen: Ich hatte gestern eine schlimme Erkältung, aber dann habe ich Vitamin C genommen, und am nächsten Tag war sie komplett weg! Vitamin C heilt definitiv Erkältungen!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "In den 1990er Jahren wurden Videospiele zunehmend populär und gleichzeitig stiegen die gemeldeten Fälle von Jugendgewalt in einigen Regionen. Diese Gewalt hatte jedoch komplexe sozioökonomische Ursachen, die in Studien dokumentiert wurden. Ein besorgter Bürger schreibt an die Zeitung: Nach der Einführung und Verbreitung von gewalttätigen Videospielen ist die Jugendgewalt massiv gestiegen. Es ist offensichtlich, dass Videospiele Gewalt verursachen!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },
            new Question
            {
                Statement = "Ein leidenschaftlicher Fußballfan hat einen alten Hut, den er zu Spielen seiner Lieblingsmannschaft trägt. Durch Zufall gewann die Mannschaft bei mehreren Spielen, bei denen er diesen Hut trug. Er ignoriert die vielen Male, bei denen die Mannschaft trotz Hut verlor. Nun schwört er: Jedes Mal wenn ich meinen speziellen Glückshut beim Spiel trage, gewinnt meine Mannschaft! Der Hut bringt uns eindeutig Glück!",
                CorrectFallacyTypeId = postHocErgoPropterHoc.Id
            },

            // Zirkelschluss examples
            new Question
            {
                Statement = "Bei einer theologischen Diskussion wird über die Grundlagen des Glaubens debattiert. Ein Teilnehmer wird nach Beweisen für seine Überzeugungen gefragt und erklärt: Die Bibel ist absolut wahr und unfehlbar, weil sie von Gott selbst geschrieben wurde. Und woher wissen wir, dass Gott tatsächlich existiert und allmächtig ist? Nun, das steht ganz klar in der Bibel geschrieben, und da die Bibel wahr ist, muss es stimmen!",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "Ein Zeuge wird vor Gericht zu seiner Glaubwürdigkeit befragt. Der Richter möchte wissen, warum man seinen Aussagen vertrauen sollte. Der Zeuge antwortet selbstsicher: Sie können mir vollkommen vertrauen, denn ich bin eine absolut vertrauenswürdige Person, die immer die Wahrheit sagt. Und Sie können mir glauben, dass ich wirklich immer die Wahrheit sage, weil ich eben so vertrauenswürdig bin!",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "In einer Rechtsphilosophie-Vorlesung diskutieren Studenten über die Legitimität von Gesetzen. Ein Student argumentiert zur Verteidigung eines umstrittenen Gesetzes: Dieses Gesetz ist absolut gerecht und fair, weil es genau das vorschreibt, was moralisch richtig ist. Und woher wissen wir, dass es moralisch richtig ist, was vorgeschrieben wird? Ganz einfach - weil es im Gesetz steht, und das Gesetz ist ja gerecht!",
                CorrectFallacyTypeId = circularReasoning.Id
            },
            new Question
            {
                Statement = "In den sozialen Medien wird eine Nachricht tausendfach geteilt. Ein skeptischer Nutzer fragt nach der Quelle und der Verlässlichkeit der Information. Der ursprüngliche Poster antwortet: Diese Nachrichtenquelle ist absolut zuverlässig und vertrauenswürdig, weil sie immer akkurate und wahre Informationen liefert. Und wie können Sie sicher sein, dass die Informationen wirklich akkurat sind? Weil sie eben von einer zuverlässigen und vertrauenswürdigen Quelle stammen!",
                CorrectFallacyTypeId = circularReasoning.Id
            },

            // Tu Quoque examples
            new Question
            {
                Statement = "Zwei Freunde unterhalten sich über gesunde Lebensweise. Der eine erklärt ausführlich die Vorteile einer ausgewogenen Ernährung mit viel Gemüse und Vollkornprodukten und empfiehlt, Fast Food zu meiden. Der andere fühlt sich angegriffen und antwortet defensiv: Du willst mir jetzt ernsthaft Vorträge über gesunde Ernährung halten und meine Essgewohnheiten kritisieren? Du isst doch selbst mindestens zweimal pro Woche Fast Food! Deine Ratschläge sind damit völlig wertlos!",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "Zwei Kollegen fahren gemeinsam zur Arbeit. Einer von ihnen erwähnt beiläufig, dass zu schnelles Fahren gefährlich ist und zeigt auf aktuelle Unfallstatistiken. Der andere, der gerade etwas zu schnell fährt, reagiert gereizt: Wie kannst du es wagen, mich jetzt für zu schnelles Fahren zu kritisieren? Du wurdest doch letzte Woche selbst geblitzt! Also halt bitte den Mund und belehre mich nicht!",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "Eine Mutter erwischt ihren Sohn bei einer kleinen Lüge über seine Hausaufgaben. Sie erklärt ihm, wie wichtig Ehrlichkeit ist und dass Lügen Vertrauen zerstört. Der Sohn erinnert sich an eine Situation und kontert trotzig: Du sagst, ich soll nicht lügen und immer ehrlich sein? Aber du hast doch letztes Jahr auch gelogen, als Oma nach ihrem Geschenk gefragt hat! Du bist genauso schlimm wie ich!",
                CorrectFallacyTypeId = tuQuoque.Id
            },
            new Question
            {
                Statement = "In einer Parlamentsdebatte hält ein Senator eine leidenschaftliche Rede gegen Korruption in der Regierung. Er schlägt konkrete Maßnahmen zur Bekämpfung von Bestechung und Vorteilsnahme vor. Ein Oppositionspolitiker unterbricht ihn und ruft: Dieser Senator kritisiert hier Korruption und gibt sich als Saubermann! Aber er hat doch selbst vor Jahren Bestechungsgelder angenommen! Seine Argumente gegen Korruption sind damit vollkommen ungültig!",
                CorrectFallacyTypeId = tuQuoque.Id
            },

            // Argumentum ad Ignorantiam examples
            new Question
            {
                Statement = "Bei einer Diskussion über außerirdisches Leben präsentiert ein Ufologe seine Theorien über Besuche von Aliens auf der Erde. Wissenschaftler fordern Beweise für diese außergewöhnlichen Behauptungen. Der Ufologe erwidert selbstbewusst: Niemand auf diesem Planeten hat jemals wissenschaftlich bewiesen, dass Außerirdische nicht existieren oder uns nicht besucht haben. Da dieser Beweis fehlt, müssen Außerirdische zwangsläufig existieren und regelmäßig die Erde besuchen!",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "Ein Hersteller von Nahrungsergänzungsmitteln bewirbt eine neue, teure Behandlung gegen Müdigkeit. Klinische Studien zur Wirksamkeit liegen nicht vor. Auf Nachfrage erklärt der Verkäufer potenziellen Kunden: Es wurde bisher noch nie wissenschaftlich nachgewiesen, dass diese spezielle Behandlung nicht funktioniert oder unwirksam ist. Da ein solcher Beweis der Unwirksamkeit fehlt, muss unsere Behandlung definitiv wirksam sein!",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "Ein Hobbyhistoriker hat eine alternative Theorie zur Geschichte entwickelt, die von etablierten Historikern widerlegt wurde. Bei einer Diskussion werden ihm die Widersprüche in seiner Theorie aufgezeigt. Er antwortet unbeeindruckt: Alle renommierten Historiker mögen anderer Meinung sein, aber da niemand mit absoluter Sicherheit das Gegenteil beweisen kann, muss meine Theorie richtig sein. Die fehlende Widerlegung ist der Beweis für meine Richtigkeit!",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            },
            new Question
            {
                Statement = "In einem alten Herrenhaus sollen angeblich paranormale Aktivitäten stattfinden. Wissenschaftler haben keine Hinweise auf übernatürliche Phänomene gefunden. Ein selbsternannter Geisterjäger führt Touristen durch das Haus und erklärt: Trotz aller skeptischen Untersuchungen gibt es keinen einzigen wissenschaftlichen Beweis dafür, dass Geister nicht existieren. Diese fehlende Widerlegung beweist eindeutig ihre Existenz, deshalb glaube ich fest an sie!",
                CorrectFallacyTypeId = appellanIgnorantiam.Id
            }
        };

        context.Questions.AddRange(questions);
        await context.SaveChangesAsync();
    }
}
