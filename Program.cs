using System.Net.Security;

internal class Program
{


    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }


    public List<Song> Songs =
[
    new Song() { Id = 1, Title = "Alabama Sundown", AuthorId = 3},
    new Song() { Id = 2, Title = "New Beginning", AuthorId = 1},
    new Song() { Id = 3, Title = "To the Stars", AuthorId = 5},
    new Song() { Id = 4, Title = "First Time Ever", AuthorId = 2},
    new Song() { Id = 5, Title = "Nobody Listens", AuthorId = 4},
    new Song() { Id = 6, Title = "New Love", AuthorId = 1},
         new Song() { Id = 10, Title = "One Love ", AuthorId = 10},
         new Song() { Id = 11, Title = "Vilan", AuthorId = 11},
         new Song() { Id = 12, Title = "Dhinka Chika", AuthorId = 12},
         new Song() { Id = 13, Title = "Born to Shine", AuthorId = 13},
        new Song() { Id = 14, Title = "Dum", AuthorId = 31},
         new Song() { Id = 15, Title = "Peg", AuthorId = 22},
         new Song() { Id = 16, Title = "Candy shop", AuthorId = 22},
         new Song() { Id = 17, Title = "Naag", AuthorId = 31},
         new Song() { Id = 2, Title = "Ak", AuthorId = 1},
    new Song() { Id = 3, Title = "Raaz", AuthorId = 5},
    new Song() { Id = 4, Title = "Dooriyan ", AuthorId = 2},
];

    public List<Author> Authors =
    [
        new Author() { Id = 1, Name = "Sue Sherrington"},
    new Author() { Id = 2, Name = "Luke Palmer" },
    new Author() { Id = 3, Name = "Gwen Felice" },
    new Author() { Id = 6, Name = "Brian Moore" },
    new Author() { Id = 7, Name = "Roy Cobbler" },
        new Author() { Id = 31, Name = "Sachin 31" },
        new Author() { Id = 22, Name = "Dhoni 22" },
        new Author() { Id = 25, Name = "Gulzar 25" },
    ];

    public static List<AuthorSongsCount> SongsGroupByAuthor(List<Song> songs , List<Author> authors)
    {
        var res = authors
            .Join(songs,
            a => a.Id,
            s => s.AuthorId,
            (a, s) => new { Authors = a, Songs = s })
            .GroupBy(x => x.Authors.Name)
            .Select(x => new AuthorSongsCount
            {
                 AuthorName = x.Key,
                 SongsCount = x.Count()
            });

        return res.ToList();

    }

    public class AuthorWitSongsList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Song> Songs { get; set; }
    }

    public static List<AuthorWitSongsList> AuthorWithSongsList(List<Author> authors , List<Song> songs)
    {
        var res = authors
            .Join(songs,
            a => a.Id,
            s => s.AuthorId,
            (a, s) => new { Authors = a, Songs = s })
            .GroupBy(x => x.Authors.Id)
            .Select(x => new AuthorWitSongsList
            {
                
                Id= x.Key,
                Name =  x.First().Authors.Name,
                Songs = x.Select(x => new Song
                {
                    Id  = x.Songs.Id,
                    Title = x.Songs.Title,
                    AuthorId = x.Songs.AuthorId,
                }).ToList()
            });

        return res.ToList() ;
    }


    public class SongWithAuthor
    {
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
    }

    public class AuthorSongsCount
    {
        public int SongsCount  { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }


    public static List<SongWithAuthor> PerformLeftJoinWithMethodSyntax(
     List<Song> songs, List<Author> authors)
    {
        var results = songs
                      .GroupJoin(
                          authors,
                          song => song.AuthorId,
                          author => author.Id,
                          (song, author) => new { song, author }
                      )
                      .SelectMany(
                          left => left.author.DefaultIfEmpty(),
                          (song, author) => new SongWithAuthor
                          { 
                              
                              Title = song.song.Title,
                              AuthorName = author == null ? "unknown" : author.Name
                          }
                      ).OrderBy(x => x.Title);

        return results.ToList();
    }


    public static List<SongWithAuthor> Right(
    List<Song> songs, List<Author> authors)
    {
        var res = authors.GroupJoin(songs,
            a => a.Id,
            s => s.AuthorId,
            (a, s) => new { a, s })
            .SelectMany(x => x.s.DefaultIfEmpty(),
            (a, s) => new SongWithAuthor
            {
                 AuthorName = a.a.Name,
                 Title =  s != null ? s.Title : "unknown title"
            });

        return res.ToList();
    }


    public static List<SongWithAuthor> FullJoin(List<Song> songs, List<Author> authors)
    {
        var left = songs.GroupJoin(
            authors,
            s => s.AuthorId,
            a => a.Id,
            (s, a) => new { Songs = s, Authers = a })
             .SelectMany(x => x.Authers.DefaultIfEmpty(),
             (s, a) => new SongWithAuthor
             {
                  Title = s.Songs.Title,
                  AuthorName = a != null ? a.Name : "No Auther"
             });


        var right = authors.GroupJoin(
            songs,
            a => a.Id,
            s => s.AuthorId,
            (a, s) => new { Authers = a, Songs = s })
            .SelectMany(x => x.Songs.DefaultIfEmpty(),
            (a, s) => new SongWithAuthor
            {
                Title = s != null ? s.Title : "no title",
                AuthorName = a.Authers.Name
            }
            );


        var fullOuter = left.Union( right );

        return fullOuter.ToList();  
    }

    public static List<SongWithAuthor> CrossJoinSongsAndAuthors(List<Song> songs, List<Author> authors)
    {
        var crossjoin = songs
            .SelectMany(s => authors,
            (s , authors)=> new SongWithAuthor
            {
                
                 AuthorName = authors.Name,
                 Title = s.Title,
            }).ToList();

        return crossjoin;
    }


    private static void Main(string[] args)
    {

        var program = new Program();

        //var result = PerformLeftJoinWithMethodSyntax(program.Songs, program.Authors);

        //foreach (var item in result)
        //{
        //    Console.WriteLine($"Title: {item.Title}, Author: {item.AuthorName}");
        //}

        //var res = Right(program.Songs, program.Authors);

        //Console.WriteLine("Right");

        //foreach (var item in res)
        //{
            
        //    Console.WriteLine($"Title: {item.Title}, Author: {item.AuthorName}");
        //}

        //var fullOuter = FullJoin(program.Songs, program.Authors);

        //Console.WriteLine("fullOuter");

        //foreach (var item in fullOuter)
        //{

        //    Console.WriteLine($"Title: {item.Title}, Author: {item.AuthorName}");
        //}

        //var crossJoin = CrossJoinSongsAndAuthors(program.Songs, program.Authors);

        //Console.WriteLine("Cross Join");

        //foreach (var item in crossJoin)
        //{

        //    Console.WriteLine($"Title: {item.Title}, Author: {item.AuthorName}");
        //}


        var authorSongsList = SongsGroupByAuthor(program.Songs, program.Authors);

        //foreach(var author in authorSongsList)
        //{
        //    Console.WriteLine($" Author Name : {author.AuthorName}, Songs Count: {author.SongsCount}");
        //}

        var authorWithSongsList = AuthorWithSongsList(program.Authors, program.Songs);

        Console.WriteLine(authorWithSongsList);

        foreach (var item in authorWithSongsList)
        {
            Console.WriteLine($" Id:{item.Id}, Author :  {item.Name}, Id: {item.Songs.ToList()} ");
        }

    }
}