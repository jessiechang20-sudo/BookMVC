using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(BookDbContext db)
        {
            // 確保 DB 已建立 / 已套用 migration
            await db.Database.MigrateAsync();

            if (await db.Books.AnyAsync())
                return;

            db.Books.AddRange(
                new Book { Isbn = "9780241003008", Title = "The Very Hungry Caterpillar", Author = "Eric Carle", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9780141033570", Title = "Thinking, Fast and Slow", Author = "Daniel Kahneman", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9780763680893", Title = "The Tale of Despereaux: Being the Story of a Mouse, a Princess, Some Soup, and a Spool of Thread", Author = "DiCamillo, Kate/ Ering, Timothy Basil (ILT)", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9798337402154", Title = "Dune: Bene Gesserit Tarot Deck and Guide", Author = "Daniel Kahneman" },
                new Book { Isbn = "9781837821402", Title = "The Witch’s Way Home Oracle: A 44-Card Deck and Guidebook", Author = "Griffin, Emma", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9798337400976", Title = "Wings and Crowns: A Romantasy Tarot Deck and Guidebook", Author = "Regina De Spada", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9789863987789", Title = "The Book of Joy", Author = "The Dalai Lama, Desmond Tutu, Douglas Abrams", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9780547577098", Title = "Number the Stars", Author = "Lowry, Lois", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9789575860967", Title = "Charlotte’s Web", Author = "White", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9780544336261", Title = "The Giver", Author = "Lowry, Lois", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9789863989493", Title = "The Almanack of Naval Ravikant: A Guide to Wealth and Happiness", Author = "Eric Jorgenson", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9780500978696", Title = "France: The Monocle Handbook", Author = "Brûlé, Tyler,Tuck, Andrew,Price, Molly", CreatedAt = DateTimeOffset.UtcNow },
                new Book { Isbn = "9781335534620", Title = "Game Changer", Author = "Reid, Rachel", CreatedAt = DateTimeOffset.UtcNow });
            await db.SaveChangesAsync();
        }
    }
}
